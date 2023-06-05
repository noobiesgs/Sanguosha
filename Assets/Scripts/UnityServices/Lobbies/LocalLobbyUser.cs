using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Noobie.Sanguosha.UnityServices.Lobbies
{
    public sealed class LocalLobbyUser
    {
        private UserData m_UserData;

        public LocalLobbyUser()
        {
            m_UserData = new UserData(isHost: false, displayName: null, id: null, portraitId: 0);
        }

        public event Action<LocalLobbyUser> changed;

        public void ResetState()
        {
            m_UserData = new UserData(false, m_UserData.DisplayName, m_UserData.Id, m_UserData.PortraitId);
        }

        private void OnChanged()
        {
            changed?.Invoke(this);
        }

        public bool IsHost
        {
            get => m_UserData.IsHost;
            set
            {
                if (m_UserData.IsHost == value) return;
                m_UserData.IsHost = value;
                LastChanged = UserMembers.IsHost;
                OnChanged();
            }
        }

        public string DisplayName
        {
            get => m_UserData.DisplayName;
            set
            {
                if (m_UserData.DisplayName == value) return;
                m_UserData.DisplayName = value;
                LastChanged = UserMembers.DisplayName;
                OnChanged();
            }
        }

        public string Id
        {
            get => m_UserData.Id;
            set
            {
                if (m_UserData.Id == value) return;
                m_UserData.Id = value;
                LastChanged = UserMembers.Id;
                OnChanged();
            }
        }

        public uint PortraitId
        {
            get => m_UserData.PortraitId;
            set
            {
                if (m_UserData.PortraitId == value) return;
                m_UserData.PortraitId = value;
                LastChanged = UserMembers.PortraitId;
                OnChanged();
            }
        }

        public UserMembers LastChanged { get; private set; }

        public void CopyDataFrom(LocalLobbyUser lobby)
        {
            var data = lobby.m_UserData;
            var lastChanged = UserMembers.None;

            if (m_UserData.IsHost != data.IsHost)
            {
                lastChanged |= UserMembers.IsHost;
            }
            if (m_UserData.DisplayName != data.DisplayName)
            {
                lastChanged |= UserMembers.DisplayName;
            }
            if (m_UserData.Id != data.Id)
            {
                lastChanged |= UserMembers.Id;
            }
            if (m_UserData.PortraitId != data.PortraitId)
            {
                lastChanged |= UserMembers.PortraitId;
            }

            if (lastChanged == UserMembers.None)
            {
                return;
            }

            m_UserData = data;
            LastChanged = lastChanged;

            OnChanged();
        }

        public Dictionary<string, PlayerDataObject> GetDataForUnityServices() =>
            new()
            {
                {"DisplayName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, DisplayName)},
                {"PortraitId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PortraitId.ToString())}
            };
    }

    [Flags]
    public enum UserMembers : byte
    {
        None = 0,
        IsHost = 1,
        DisplayName = 1 << 1,
        Id = 1 << 2,
        PortraitId = 1 << 3,
    }

    public struct UserData
    {
        public bool IsHost { get; set; }
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public uint PortraitId { get; set; }

        public UserData(bool isHost, string displayName, string id, uint portraitId)
        {
            IsHost = isHost;
            DisplayName = displayName;
            Id = id;
            PortraitId = portraitId;
        }
    }
}
