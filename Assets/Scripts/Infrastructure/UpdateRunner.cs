using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Noobie.Sanguosha.Infrastructure
{
    public class UpdateRunner : MonoBehaviour
    {
        class SubscriberData
        {
            public float Period;
            public float NextCallTime;
            public float LastCallTime;
        }

        private readonly Queue<Action> m_PendingHandlers = new();
        private readonly HashSet<Action<float>> m_Subscribers = new();
        private readonly Dictionary<Action<float>, SubscriberData> m_SubscriberData = new();

        private void OnDestroy()
        {
            m_Subscribers.Clear();
            m_SubscriberData.Clear();
            m_PendingHandlers.Clear();
        }

        public void Subscribe(Action<float> onUpdate, float updatePeriod)
        {
            if (onUpdate == null)
            {
                return;
            }
            if (onUpdate.Target == null)
            {
                Debug.LogError("Can't subscribe to a local function that can go out of scope and can't be unsubscribed from");
                return;
            }
            if (onUpdate.Method.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any())
            {
                Debug.LogError("Can't subscribe with an anonymous function that cannot be Unsubscribed, by checking for a character that can't exist in a declared method name.");
                return;
            }

            if (!m_Subscribers.Contains(onUpdate))
            {
                m_PendingHandlers.Enqueue(() =>
                {
                    if (m_Subscribers.Add(onUpdate))
                    {
                        m_SubscriberData.Add(onUpdate, new SubscriberData { LastCallTime = Time.time, NextCallTime = 0, Period = updatePeriod });
                    }
                });
            }
        }

        public void Unsubscribe(Action<float> onUpdate)
        {
            m_PendingHandlers.Enqueue(() =>
            {
                m_Subscribers.Remove(onUpdate);
                m_SubscriberData.Remove(onUpdate);
            });
        }

        private void Update()
        {
            while (m_PendingHandlers.Count > 0)
            {
                m_PendingHandlers.Dequeue().Invoke();
            }

            foreach (var subscriber in m_Subscribers)
            {
                var data = m_SubscriberData[subscriber];
                if (!(Time.time >= data.NextCallTime)) continue;
                subscriber.Invoke(Time.time - data.LastCallTime);
                data.LastCallTime = Time.time;
                data.NextCallTime = Time.time + data.Period;
            }
        }
    }
}

