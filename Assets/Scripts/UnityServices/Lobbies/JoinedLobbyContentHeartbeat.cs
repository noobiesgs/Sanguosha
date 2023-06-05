using Noobie.Sanguosha.Infrastructure;
using VContainer;

namespace Noobie.Sanguosha.UnityServices.Lobbies
{
    public class JoinedLobbyContentHeartbeat
    {
        [Inject] private LocalLobby m_LocalLobby;
        [Inject] private LocalLobbyUser m_LocalUser;
        [Inject] private UpdateRunner m_UpdateRunner;
        [Inject] private LobbyServiceFacade m_LobbyServiceFacade;

        private int m_AwaitingQueryCount;
        private bool m_ShouldPushData;

        public void BeginTracking()
        {
            m_UpdateRunner.Subscribe(OnUpdate, 1.5f);
            m_LocalLobby.changed += OnLocalLobbyChanged;
            m_ShouldPushData = true;
        }

        public void EndTracking()
        {
            m_ShouldPushData = false;
            m_UpdateRunner.Unsubscribe(OnUpdate);
            m_LocalLobby.changed -= OnLocalLobbyChanged;
        }

        void OnLocalLobbyChanged(LocalLobby lobby)
        {
            if (string.IsNullOrEmpty(lobby.LobbyId))
            {
                EndTracking();
            }

            m_ShouldPushData = true;
        }

        async void OnUpdate(float dt)
        {
            if (m_AwaitingQueryCount > 0)
            {
                return;
            }

            if (m_LocalUser.IsHost)
            {
                m_LobbyServiceFacade.DoLobbyHeartbeat(dt);
            }

            if (m_ShouldPushData)
            {
                m_ShouldPushData = false;

                if (m_LocalUser.IsHost)
                {
                    m_AwaitingQueryCount++;
                    await m_LobbyServiceFacade.UpdateLobbyDataAsync(m_LocalLobby.GetDataForUnityServices());
                    m_AwaitingQueryCount--;
                }

                m_AwaitingQueryCount++;
                await m_LobbyServiceFacade.UpdatePlayerDataAsync(m_LocalUser.GetDataForUnityServices());
                m_AwaitingQueryCount--;
            }
        }
    }
}
