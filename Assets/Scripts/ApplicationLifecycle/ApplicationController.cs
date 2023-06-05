using Noobie.Sanguosha.ApplicationLifecycle.Messages;
using Noobie.Sanguosha.Infrastructure;
using Noobie.Sanguosha.UnityServices;
using Noobie.Sanguosha.UnityServices.Auth;
using Noobie.Sanguosha.UnityServices.Lobbies;
using Noobie.Sanguosha.UnityServices.Lobbies.Messages;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Noobie.Sanguosha.ApplicationLifecycle
{
    public class ApplicationController : LifetimeScope
    {
        [SerializeField]
        private UpdateRunner m_UpdateRunner;

        private LocalLobby m_LocalLobby;
        private LobbyServiceFacade m_LobbyServiceFacade;

        private IDisposable m_Subscriptions;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(m_UpdateRunner);

            builder.Register<LocalLobbyUser>(Lifetime.Singleton);
            builder.Register<LocalLobby>(Lifetime.Singleton);

            builder.RegisterInstance(new MessageChannel<QuitApplicationMessage>()).AsImplementedInterfaces();
            builder.RegisterInstance(new MessageChannel<UnityServiceErrorMessage>()).AsImplementedInterfaces();

            builder.RegisterInstance(new BufferedMessageChannel<LobbyListFetchedMessage>()).AsImplementedInterfaces();

            builder.Register<AuthenticationServiceFacade>(Lifetime.Singleton);
            builder.RegisterEntryPoint<LobbyServiceFacade>().AsSelf();
        }

        private void Start()
        {
            m_LocalLobby = Container.Resolve<LocalLobby>();
            m_LobbyServiceFacade = Container.Resolve<LobbyServiceFacade>();

            var quitApplicationSub = Container.Resolve<ISubscriber<QuitApplicationMessage>>();
            var subHandles = new DisposableGroup();
            subHandles.Add(quitApplicationSub.Subscribe(QuitGame));
            m_Subscriptions = subHandles;

            Application.wantsToQuit += OnWantToQuit;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(m_UpdateRunner.gameObject);
            Application.targetFrameRate = 60;
            SceneManager.LoadScene("MainMenu");
        }

        protected override void OnDestroy()
        {
            m_Subscriptions?.Dispose();
            m_LobbyServiceFacade?.EndTracking();
            base.OnDestroy();
        }

        private bool OnWantToQuit()
        {
            var canQuit = string.IsNullOrEmpty(m_LocalLobby?.LobbyId);
            if (!canQuit)
            {
                LeaveBeforeQuit().Forget();
            }
            return canQuit;
        }

        private async UniTaskVoid LeaveBeforeQuit()
        {
            // We want to quit anyways, so if anything happens while trying to leave the Lobby, log the exception then carry on
            try
            {
                m_LobbyServiceFacade.EndTracking().AsUniTask().Forget();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            await UniTask.NextFrame();
            Application.Quit();
        }

        private static void QuitGame(QuitApplicationMessage msg)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
