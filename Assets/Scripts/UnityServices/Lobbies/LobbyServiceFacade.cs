using Noobie.Sanguosha.Infrastructure;
using Noobie.Sanguosha.UnityServices.Lobbies.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Noobie.Sanguosha.UnityServices.Lobbies
{
    public class LobbyServiceFacade : IDisposable, IStartable
    {
        [Inject] private LifetimeScope m_ParentScope;
        [Inject] private UpdateRunner m_UpdateRunner;
        [Inject] private LocalLobby m_LocalLobby;
        [Inject] private LocalLobbyUser m_LocalUser;
        [Inject] private IPublisher<UnityServiceErrorMessage> m_UnityServiceErrorMessagePub;
        [Inject] private IPublisher<LobbyListFetchedMessage> m_LobbyListFetchedPub;

        private const float k_HeartbeatPeriod = 8;
        private float m_HeartbeatTime;

        private LifetimeScope m_ServiceScope;
        private LobbyApiInterface m_LobbyApiInterface;
        private JoinedLobbyContentHeartbeat m_JoinedLobbyContentHeartbeat;

        private RateLimitCooldown m_RateLimitQuery;
        private RateLimitCooldown m_RateLimitJoin;
        private RateLimitCooldown m_RateLimitQuickJoin;
        private RateLimitCooldown m_RateLimitHost;

        public Lobby CurrentUnityLobby { get; private set; }

        private bool m_IsTracking;

        public void Start()
        {
            m_ServiceScope = m_ParentScope.CreateChild(builder =>
            {
                builder.Register<JoinedLobbyContentHeartbeat>(Lifetime.Singleton);
                builder.Register<LobbyApiInterface>(Lifetime.Singleton);
            });

            m_LobbyApiInterface = m_ServiceScope.Container.Resolve<LobbyApiInterface>();
            m_JoinedLobbyContentHeartbeat = m_ServiceScope.Container.Resolve<JoinedLobbyContentHeartbeat>();

            //See https://docs.unity.com/lobby/rate-limits.html
            m_RateLimitQuery = new RateLimitCooldown(1f);
            m_RateLimitJoin = new RateLimitCooldown(3f);
            m_RateLimitQuickJoin = new RateLimitCooldown(10f);
            m_RateLimitHost = new RateLimitCooldown(3f);
        }

        public void Dispose()
        {
            EndTracking();
            m_ServiceScope?.Dispose();
        }

        public void SetRemoteLobby(Lobby lobby)
        {
            CurrentUnityLobby = lobby;
            m_LocalLobby.ApplyRemoteData(lobby);
        }

        public void BeginTracking()
        {
            if (!m_IsTracking)
            {
                m_IsTracking = true;

                m_UpdateRunner.Subscribe(UpdateLobby, 5f);
                m_JoinedLobbyContentHeartbeat.BeginTracking();
            }
        }

        public Task EndTracking()
        {
            var task = Task.CompletedTask;
            if (CurrentUnityLobby != null)
            {
                CurrentUnityLobby = null;

                var lobbyId = m_LocalLobby?.LobbyId;

                if (!string.IsNullOrEmpty(lobbyId))
                {
                    task = m_LocalUser.IsHost ? DeleteLobbyAsync(lobbyId) : LeaveLobbyAsync(lobbyId);
                }

                m_LocalUser.ResetState();
                m_LocalLobby?.Reset(m_LocalUser);
            }

            if (m_IsTracking)
            {
                m_UpdateRunner.Unsubscribe(UpdateLobby);
                m_IsTracking = false;
                m_HeartbeatTime = 0;
                m_JoinedLobbyContentHeartbeat.EndTracking();
            }

            return task;
        }

        public async Task<(bool Success, Lobby Lobby)> TryCreateLobbyAsync(string lobbyName, int maxPlayers, bool isPrivate)
        {
            if (!m_RateLimitHost.CanCall)
            {
                Debug.LogWarning("Create Lobby hit the rate limit.");
                return (false, null);
            }

            try
            {
                var lobby = await m_LobbyApiInterface.CreateLobby(AuthenticationService.Instance.PlayerId, lobbyName, maxPlayers, isPrivate, m_LocalUser.GetDataForUnityServices(), null);
                return (true, lobby);
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    m_RateLimitHost.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }

            return (false, null);
        }

        public async Task<(bool Success, Lobby Lobby)> TryJoinLobbyAsync(string lobbyId, string lobbyCode)
        {
            if (!m_RateLimitJoin.CanCall || (lobbyId == null && lobbyCode == null))
            {
                Debug.LogWarning("Join Lobby hit the rate limit.");
                return (false, null);
            }

            try
            {
                if (!string.IsNullOrEmpty(lobbyCode))
                {
                    var lobby = await m_LobbyApiInterface.JoinLobbyByCode(AuthenticationService.Instance.PlayerId, lobbyCode, m_LocalUser.GetDataForUnityServices());
                    return (true, lobby);
                }
                else
                {
                    var lobby = await m_LobbyApiInterface.JoinLobbyById(AuthenticationService.Instance.PlayerId, lobbyId, m_LocalUser.GetDataForUnityServices());
                    return (true, lobby);
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    m_RateLimitJoin.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }

            return (false, null);
        }

        public async Task<(bool Success, Lobby Lobby)> TryQuickJoinLobbyAsync()
        {
            if (!m_RateLimitQuickJoin.CanCall)
            {
                Debug.LogWarning("Quick Join Lobby hit the rate limit.");
                return (false, null);
            }

            try
            {
                var lobby = await m_LobbyApiInterface.QuickJoinLobby(AuthenticationService.Instance.PlayerId, m_LocalUser.GetDataForUnityServices());
                return (true, lobby);
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    m_RateLimitQuickJoin.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }

            return (false, null);
        }

        public async Task RetrieveAndPublishLobbyListAsync()
        {
            if (!m_RateLimitQuery.CanCall)
            {
                Debug.LogWarning("Retrieve Lobby list hit the rate limit. Will try again soon...");
                return;
            }

            try
            {
                var response = await m_LobbyApiInterface.QueryAllLobbies();
                m_LobbyListFetchedPub.Publish(new LobbyListFetchedMessage(LocalLobby.CreateLocalLobbies(response)));
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    m_RateLimitQuery.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }
        }

        public async Task LeaveLobbyAsync(string lobbyId)
        {
            string uasId = AuthenticationService.Instance.PlayerId;
            try
            {
                await m_LobbyApiInterface.RemovePlayerFromLobby(uasId, lobbyId);
            }
            catch (LobbyServiceException e)
            {
                // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost)
                {
                    PublishError(e);
                }
            }
        }

        public async void RemovePlayerFromLobbyAsync(string uasId, string lobbyId)
        {
            if (m_LocalUser.IsHost)
            {
                try
                {
                    await m_LobbyApiInterface.RemovePlayerFromLobby(uasId, lobbyId);
                }
                catch (LobbyServiceException e)
                {
                    PublishError(e);
                }
            }
            else
            {
                Debug.LogError("Only the host can remove other players from the lobby.");
            }
        }

        public async Task<Lobby> ReconnectToLobbyAsync(string lobbyId)
        {
            try
            {
                return await m_LobbyApiInterface.ReconnectToLobby(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost)
                {
                    PublishError(e);
                }
            }

            return null;
        }

        public async Task DeleteLobbyAsync(string lobbyId)
        {
            if (m_LocalUser.IsHost)
            {
                try
                {
                    await m_LobbyApiInterface.DeleteLobby(lobbyId);
                }
                catch (LobbyServiceException e)
                {
                    PublishError(e);
                }
            }
            else
            {
                Debug.LogError("Only the host can delete a lobby.");
            }
        }

        async void UpdateLobby(float _)
        {
            if (!m_RateLimitQuery.CanCall)
            {
                return;
            }

            try
            {
                var lobby = await m_LobbyApiInterface.GetLobby(m_LocalLobby.LobbyId);

                CurrentUnityLobby = lobby;
                m_LocalLobby.ApplyRemoteData(lobby);

                // as client, check if host is still in lobby
                if (!m_LocalUser.IsHost)
                {
                    foreach (var lobbyUser in m_LocalLobby.LobbyUsers)
                    {
                        if (lobbyUser.Value.IsHost)
                        {
                            return;
                        }
                    }
                    m_UnityServiceErrorMessagePub.Publish(new UnityServiceErrorMessage("Host left the lobby", "Disconnecting.", UnityServiceErrorMessage.Service.Lobby));
                    await EndTracking();
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    m_RateLimitQuery.PutOnCooldown();
                }
                else if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost)
                {
                    PublishError(e);
                }
            }
        }

        public void DoLobbyHeartbeat(float dt)
        {
            m_HeartbeatTime += dt;
            if (m_HeartbeatTime > k_HeartbeatPeriod)
            {
                m_HeartbeatTime -= k_HeartbeatPeriod;
                try
                {
                    m_LobbyApiInterface.SendHeartbeatPing(CurrentUnityLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                    if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost)
                    {
                        PublishError(e);
                    }
                }
            }
        }

        public async Task UpdatePlayerDataAsync(Dictionary<string, PlayerDataObject> data)
        {
            if (!m_RateLimitQuery.CanCall)
            {
                return;
            }

            try
            {
                var result = await m_LobbyApiInterface.UpdatePlayer(CurrentUnityLobby.Id, AuthenticationService.Instance.PlayerId, data, null, null);

                if (result != null)
                {
                    CurrentUnityLobby = result;
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    m_RateLimitQuery.PutOnCooldown();
                }
                else if (e.Reason != LobbyExceptionReason.LobbyNotFound && !m_LocalUser.IsHost)
                {
                    PublishError(e);
                }
            }
        }

        public async Task UpdatePlayerRelayInfoAsync(string allocationId, string connectionInfo)
        {
            if (!m_RateLimitQuery.CanCall)
            {
                return;
            }

            try
            {
                await m_LobbyApiInterface.UpdatePlayer(CurrentUnityLobby.Id, AuthenticationService.Instance.PlayerId, new Dictionary<string, PlayerDataObject>(), allocationId, connectionInfo);
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    m_RateLimitQuery.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }
        }

        public async Task UpdateLobbyDataAsync(Dictionary<string, DataObject> data)
        {
            if (!m_RateLimitQuery.CanCall)
            {
                return;
            }

            var dataCurrent = CurrentUnityLobby.Data ?? new Dictionary<string, DataObject>();

            foreach (var dataNew in data)
            {
                if (dataCurrent.ContainsKey(dataNew.Key))
                {
                    dataCurrent[dataNew.Key] = dataNew.Value;
                }
                else
                {
                    dataCurrent.Add(dataNew.Key, dataNew.Value);
                }
            }

            var shouldLock = string.IsNullOrEmpty(m_LocalLobby.RelayJoinCode);

            try
            {
                var result = await m_LobbyApiInterface.UpdateLobby(CurrentUnityLobby.Id, dataCurrent, shouldLock);

                if (result != null)
                {
                    CurrentUnityLobby = result;
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    m_RateLimitQuery.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }
        }

        void PublishError(LobbyServiceException e)
        {
            var reason = $"{e.Message} ({e.InnerException?.Message})";
            m_UnityServiceErrorMessagePub.Publish(new UnityServiceErrorMessage("Lobby Error", reason, UnityServiceErrorMessage.Service.Lobby, e));
        }
    }
}
