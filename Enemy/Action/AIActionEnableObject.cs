using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// An Action that shoots using the currently equipped weapon. If your weapon is in auto mode, will shoot until you exit the state, and will only shoot once in SemiAuto mode. You can optionnally have the character face (left/right) the target, and aim at it (if the weapon has a WeaponAim component).
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Enable")]
    // [RequireComponent(typeof(Character))]
    // [RequireComponent(typeof(CharacterHandleWeapon))]
    public class AIActionEnableObject : AIAction
    {
        [SerializeField]
        private GameObject targetObj;
        [SerializeField]
        private float waitTime;
        [SerializeField] private bool waitAnimate = true;
        [SerializeField]
        private bool blockEndDisable;
        [SerializeField]
        Animator animator;

        protected override void Initialization()
        {
            base.Initialization();
            if (animator != null)
                return;

            animator = _brain.gameObject.GetComponent<Animator>();
            if (animator == null)
                animator = _brain.transform.parent.GetComponentInChildren<Animator>();
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
            Debug.Log("타깃 활성화 시작");
            base.OnEnterState();
            if (!waitAnimate)
                animator?.SetBool(targetObj.name, true);

            if (waitTime > 0)
                Invoke(nameof(EnableTarget), waitTime);
            else
                EnableTarget();
        }

        private void EnableTarget()
        {
            Debug.Log("타깃 활성화");
            targetObj.SetActive(true);
            if (waitAnimate)
                animator?.SetBool(targetObj.name, true);
        }

        /// <summary>
        /// When exiting the state we make sure we're not shooting anymore
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            if (!blockEndDisable)
            {
                targetObj.SetActive(false);
                animator?.SetBool(targetObj.name, false);
            }
            CancelInvoke();
        }
    }
}
