using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// An Action that shoots using the currently equipped weapon. If your weapon is in auto mode, will shoot until you exit the state, and will only shoot once in SemiAuto mode. You can optionnally have the character face (left/right) the target, and aim at it (if the weapon has a WeaponAim component).
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Bounce")]
    // [RequireComponent(typeof(Character))]
    // [RequireComponent(typeof(CharacterHandleWeapon))]
    public class AIActionBounce : AIAction
    {
        [SerializeField] BouncyProjectile bouncy;

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
            bouncy.enabled = true;
            bouncy.CallInit();
            _brain.Target = bouncy.GetStartTransform();
        }

        /// <summary>
        /// When exiting the state we make sure we're not shooting anymore
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            DOVirtual.DelayedCall(Time.deltaTime, () => StartCoroutine(bouncy.DelayFinishBounce(_brain)));
        }
    }
}
