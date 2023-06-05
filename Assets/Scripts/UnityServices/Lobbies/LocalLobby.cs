using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Noobie.Sanguosha.UnityServices.Lobbies
{
    public sealed class LocalLobby
    {
        private Dictionary<string, LocalLobbyUser> m_LobbyUsers = new();
        private LobbyData m_Data;
        private LobbyMembers m_LastChanged;

        public static List<LocalLobby> CreateLocalLobbies(QueryResponse response)
        {
            return response.Results.Select(Create).ToList();
        }

        public event Action<LocalLobby> changed;

        public Dictionary<string, LocalLobbyUser> LobbyUsers => m_LobbyUsers;

        public LobbyData Data => new(m_Data);

        public LobbyMembers LastChanged => m_LastChanged;

        public string LobbyId
        {
            get => m_Data.LobbyId;
            set
            {
                if (value == m_Data.LobbyId)
                {
                    return;
                }

                m_LastChanged = LobbyMembers.LobbyId;
                m_Data.LobbyId = value;
                OnChanged();
            }
        }

        public string LobbyCode
        {
            get => m_Data.LobbyCode;
            set
            {
                if (value == m_Data.LobbyCode)
                {
                    return;
                }

                m_LastChanged = LobbyMembers.LobbyCode;
                m_Data.LobbyCode = value;
                OnChanged();
            }
        }

        public string RelayJoinCode
        {
            get => m_Data.RelayJoinCode;
            set
            {
                if (value == m_Data.RelayJoinCode)
                {
                    return;
                }

                m_LastChanged = LobbyMembers.RelayJoinCode;
                m_Data.RelayJoinCode = value;
                OnChanged();
            }
        }

        public string LobbyName
        {
            get => m_Data.LobbyName;
            set
            {
                if (value == m_Data.LobbyName)
                {
                    return;
                }

                m_LastChanged = LobbyMembers.LobbyName;
                m_Data.LobbyName = value;
                OnChanged();
            }
        }

        public bool Private
        {
            get => m_Data.Private;
            set
            {
                if (value == m_Data.Private)
                {
                    return;
                }

                m_LastChanged = LobbyMembers.Private;
                m_Data.Private = value;
                OnChanged();
            }
        }

        public int MaxPlayerCount
        {
            get => m_Data.MaxPlayerCount;
            set
            {
                if (value == m_Data.MaxPlayerCount)
                {
                    return;
                }

                m_LastChanged = LobbyMembers.MaxPlayerCount;
                m_Data.MaxPlayerCount = value;
                OnChanged();
            }
        }

        public int PlayerCount => m_LobbyUsers.Count;

        private void OnChanged()
        {
            changed?.Invoke(this);
        }

        private void OnChangedUser(LocalLobbyUser user)
        {
            OnChanged();
        }

        private void DoRemoveUser(LocalLobbyUser user)
        {
            if (!m_LobbyUsers.ContainsKey(user.Id))
            {
                Debug.LogWarning($"Player {user.DisplayName}({user.Id}) does not exist in lobby: {LobbyId}");
                return;
            }

            m_LobbyUsers.Remove(user.Id);
            user.changed -= OnChangedUser;
        }

        private void DoAddUser(LocalLobbyUser user)
        {
            m_LobbyUsers.Add(user.Id, user);
            user.changed += OnChangedUser;
        }


        public static LocalLobby Create(Lobby lobby)
        {
            var data = new LocalLobby();
            data.ApplyRemoteData(lobby);
            return data;
        }

        public void AddUser(LocalLobbyUser user)
        {
            if (m_LobbyUsers.ContainsKey(user.Id)) return;
            DoAddUser(user);
            OnChanged();
        }

        public void CopyDataFrom(LobbyData data, Dictionary<string, LocalLobbyUser> currentUsers)
        {
            var changes = DetectChanges(data);
            m_Data = data;

            if (currentUsers == null && m_LobbyUsers.Any())
            {
                foreach (var user in m_LobbyUsers.Values)
                {
                    user.changed -= OnChangedUser;
                }
                m_LobbyUsers = new Dictionary<string, LocalLobbyUser>();
                changes |= LobbyMembers.PlayerCount;
            }
            else if (currentUsers != null)
            {
                List<LocalLobbyUser> toRemove = new List<LocalLobbyUser>();
                foreach (var oldUser in m_LobbyUsers)
                {
                    if (currentUsers.ContainsKey(oldUser.Key))
                    {
                        oldUser.Value.CopyDataFrom(currentUsers[oldUser.Key]);
                    }
                    else
                    {
                        toRemove.Add(oldUser.Value);
                    }
                }

                foreach (var remove in toRemove)
                {
                    DoRemoveUser(remove);
                    changes |= LobbyMembers.PlayerCount;
                }

                foreach (var currentUser in currentUsers)
                {
                    if (!m_LobbyUsers.ContainsKey(currentUser.Key))
                    {
                        DoAddUser(currentUser.Value);
                        changes |= LobbyMembers.PlayerCount;
                    }
                }
            }

            if (changes != LobbyMembers.None)
            {
                m_LastChanged = changes;
                OnChanged();
            }
        }

        private LobbyMembers DetectChanges(LobbyData data)
        {
            var changes = LobbyMembers.None;

            if (data.Private != m_Data.Private)
            {
                changes |= LobbyMembers.Private;
            }

            if (data.LobbyCode != m_Data.LobbyCode)
            {
                changes |= LobbyMembers.LobbyCode;
            }

            if (data.LobbyId != m_Data.LobbyId)
            {
                changes |= LobbyMembers.LobbyId;
            }

            if (data.LobbyName != m_Data.LobbyName)
            {
                changes |= LobbyMembers.LobbyName;
            }

            if (data.RelayJoinCode != m_Data.RelayJoinCode)
            {
                changes |= LobbyMembers.Private;
            }

            if (data.MaxPlayerCount != m_Data.MaxPlayerCount)
            {
                changes |= LobbyMembers.MaxPlayerCount;
            }

            return changes;
        }

        public void ApplyRemoteData(Lobby lobby)
        {
            var info = new LobbyData
            {
                LobbyId = lobby.Id,
                LobbyCode = lobby.LobbyCode,
                Private = lobby.IsPrivate,
                LobbyName = lobby.Name,
                MaxPlayerCount = lobby.MaxPlayers
            };

            if (lobby.Data != null)
            {
                info.RelayJoinCode = lobby.Data.ContainsKey("RelayJoinCode") ? lobby.Data["RelayJoinCode"].Value : null;
            }
            else
            {
                info.RelayJoinCode = null;
            }

            var lobbyUsers = new Dictionary<string, LocalLobbyUser>();
            foreach (var player in lobby.Players)
            {
                if (player.Data != null)
                {
                    if (LobbyUsers.ContainsKey(player.Id))
                    {
                        lobbyUsers.Add(player.Id, LobbyUsers[player.Id]);
                        continue;
                    }
                }

                var incomingData = new LocalLobbyUser
                {
                    IsHost = lobby.HostId.Equals(player.Id),
                    DisplayName = player.Data?.ContainsKey("DisplayName") == true ? player.Data["DisplayName"].Value : default,
                    PortraitId = player.Data?.ContainsKey("PortraitId") == true ? uint.Parse(player.Data["PortraitId"].Value) : default,
                    Id = player.Id
                };

                lobbyUsers.Add(incomingData.Id, incomingData);
            }

            CopyDataFrom(info, lobbyUsers);
        }

        public Dictionary<string, DataObject> GetDataForUnityServices() =>
            new()
            {
                {"RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public,  RelayJoinCode)}
            };

        public void Reset(LocalLobbyUser localUser)
        {
            CopyDataFrom(new LobbyData(), new Dictionary<string, LocalLobbyUser>());
            AddUser(localUser);
        }
    }

    public struct LobbyData
    {
        public string LobbyId { get; set; }
        public string LobbyCode { get; set; }
        public string RelayJoinCode { get; set; }
        public string LobbyName { get; set; }
        public bool Private { get; set; }
        public int MaxPlayerCount { get; set; }

        public LobbyData(LobbyData existing)
        {
            LobbyId = existing.LobbyId;
            LobbyCode = existing.LobbyCode;
            RelayJoinCode = existing.RelayJoinCode;
            LobbyName = existing.LobbyName;
            Private = existing.Private;
            MaxPlayerCount = existing.MaxPlayerCount;
        }

        public LobbyData(string lobbyCode)
        {
            LobbyId = null;
            LobbyCode = lobbyCode;
            RelayJoinCode = null;
            LobbyName = null;
            Private = false;
            MaxPlayerCount = -1;
        }
    }

    [Flags]
    public enum LobbyMembers : byte
    {
        None = 0,
        LobbyId = 1,
        LobbyCode = 1 << 1,
        RelayJoinCode = 1 << 2,
        LobbyName = 1 << 3,
        Private = 1 << 4,
        MaxPlayerCount = 1 << 5,
        PlayerCount = 1 << 6,
    }
}
