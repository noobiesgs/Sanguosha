using UnityEngine;

namespace Noobie.Sanguosha.UnityServices
{
    internal class RateLimitCooldown
    {
        public float CooldownTimeLength { get; }

        private float m_CooldownFinishedTime;

        public RateLimitCooldown(float cooldownTimeLength)
        {
            CooldownTimeLength = cooldownTimeLength;
            m_CooldownFinishedTime = -1f;
        }

        public bool CanCall => Time.unscaledTime > m_CooldownFinishedTime;

        public void PutOnCooldown()
        {
            m_CooldownFinishedTime = Time.unscaledTime + CooldownTimeLength;
        }
    }
}
