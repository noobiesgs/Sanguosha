using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Noobie.Sanguosha.Infrastructure
{
    public class NetworkedMessageChannel<T> : MessageChannel<T> where T : unmanaged, INetworkSerializeByMemcpy
    {
        private NetworkManager m_NetworkManager;

        private readonly string m_Name;

        public NetworkedMessageChannel()
        {
            m_Name = $"{typeof(T).FullName}NetworkMessageChannel";
        }

        [Inject]
        void InjectDependencies(NetworkManager networkManager)
        {
            m_NetworkManager = networkManager;
            m_NetworkManager.OnClientConnectedCallback += OnClientConnected;
            if (m_NetworkManager.IsListening)
            {
                RegisterHandler();
            }
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                if (m_NetworkManager != null && m_NetworkManager.CustomMessagingManager != null)
                {
                    m_NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(m_Name);
                }
            }
            base.Dispose();
        }

        private void OnClientConnected(ulong clientId)
        {
            RegisterHandler();
        }

        private void RegisterHandler()
        {
            // Only register message handler on clients
            if (!m_NetworkManager.IsServer)
            {
                m_NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(m_Name, ReceiveMessageThroughNetwork);
            }
        }

        public override void Publish(T message)
        {
            if (m_NetworkManager.IsServer)
            {
                // send message to clients, then publish locally
                SendMessageThroughNetwork(message);
                base.Publish(message);
            }
            else
            {
                Debug.LogError("Only a server can publish in a NetworkedMessageChannel");
            }
        }

        private void SendMessageThroughNetwork(T message)
        {
            var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
            writer.WriteValueSafe(message);
            m_NetworkManager.CustomMessagingManager.SendNamedMessageToAll(m_Name, writer);
        }

        private void ReceiveMessageThroughNetwork(ulong clientId, FastBufferReader reader)
        {
            reader.ReadValueSafe(out T message);
            base.Publish(message);
        }
    }
}
