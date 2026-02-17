using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Set DamageOnTouch Damage")]
    public class AIActionSetDamageOnTouchDamage : AIAction
    {
        [SerializeField]
        private int damageOnTouchDamage;
        [SerializeField] private DamageOnTouch damageOnTouch;
        private int initialDamageOnTouchDamage;
        protected override void Initialization()
        {
            base.Initialization();
            initialDamageOnTouchDamage = damageOnTouch.DamageCaused;
        }

        /// <summary>
        /// On PerformAction we face and aim if needed, and we shoot
        /// </summary>
        public override void PerformAction()
        {

        }

        /// <summary>
        /// Faces the target if required
        /// </summary>

        public override void OnEnterState()
        {
            base.OnEnterState();
            damageOnTouch.DamageCaused = damageOnTouchDamage;
        }

        /// <summary>
        /// When exiting the state we make sure we're not shooting anymore
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            damageOnTouch.DamageCaused = initialDamageOnTouchDamage;
        }
    }
}
