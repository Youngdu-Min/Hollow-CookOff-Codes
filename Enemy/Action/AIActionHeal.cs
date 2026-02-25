using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This Decision will return true if any object on its TargetLayer layermask enters its line of sight. It will also set the Brain's Target to that object. You can choose to have it in ray mode, in which case its line of sight will be an actual line (a raycast), or have it be wider (in which case it'll use a spherecast). You can also specify an offset for the ray's origin, and an obstacle layer mask that will block it.
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Decisions/AI Action Heal")]
    // [RequireComponent(typeof(Character))]
    public class AIActionHeal : AIAction
    {
        public enum RefillModes { Linear, Bursts }

        [Header("Mode")]

        /// the selected refill mode 
        [Tooltip("the selected refill mode ")]
        public RefillModes RefillMode;

        [Header("Cooldown")]

        /// how much time, in seconds, should pass before the refill kicks in
        [Tooltip("how much time, in seconds, should pass before the refill kicks in")]
        public float CooldownAfterHit = 1f;

        [Header("Refill Settings")]

        /// if this is true, health will refill itself when not at full health
        [Tooltip("if this is true, health will refill itself when not at full health")]
        public bool RefillHealth = true;
        /// the amount of health per second to restore when in linear mode
        [MMEnumCondition("RefillMode", (int)RefillModes.Linear)]
        [Tooltip("the amount of health per second to restore when in linear mode")]
        public int HealthPerSecond;
        /// the amount of health to restore per burst when in burst mode
        [MMEnumCondition("RefillMode", (int)RefillModes.Bursts)]
        [Tooltip("the amount of health to restore per burst when in burst mode")]
        public int HealthPerBurst = 5;
        /// the duration between two health bursts, in seconds
        [MMEnumCondition("RefillMode", (int)RefillModes.Bursts)]
        [Tooltip("the duration between two health bursts, in seconds")]
        public float DurationBetweenBursts = 2f;

        protected HealthExpend _health;
        protected float _lastHitTime = 0f;
        protected float _healthToGive = 0f;
        protected float _lastBurstTimestamp;

        /// <summary>
        /// On Init we grab our character
        /// </summary>
        protected override void Initialization()
        {
            _health = _brain.Target.GetComponent<HealthExpend>();
        }

        /// <summary>
        /// On Decide we look for a target
        /// </summary>
        /// <returns></returns>
        public override void PerformAction()
        {
            Heal();
        }

        protected virtual void Heal()
        {
            if (!RefillHealth)
            {
                return;
            }

            if (Time.time - _lastHitTime < CooldownAfterHit)
            {
                return;
            }

            if (_health.CurrentHealth < _health.MaximumHealth)
            {
                switch (RefillMode)
                {
                    case RefillModes.Bursts:
                        if (Time.time - _lastBurstTimestamp > DurationBetweenBursts)
                        {
                            _health.GetHealth(HealthPerBurst, this.gameObject);
                            _lastBurstTimestamp = Time.time;
                        }
                        break;

                    case RefillModes.Linear:
                        _healthToGive += HealthPerSecond * Time.deltaTime;
                        if (_healthToGive > 1f)
                        {
                            int givenHealth = (int)_healthToGive;
                            _healthToGive -= givenHealth;
                            _health.GetHealth(givenHealth, this.gameObject);
                        }
                        break;
                }
            }
        }
    }
}
