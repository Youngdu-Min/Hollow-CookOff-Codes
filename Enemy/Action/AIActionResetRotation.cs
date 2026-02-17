using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This action directs the CharacterHorizontalMovement ability to move in the direction of the target.
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Reset Rotation")]
    // [RequireComponent(typeof(CharacterHorizontalMovement))]
    public class AIActionResetRotation : AIAction
    {
        [SerializeField] private Transform target;
        private Vector3 initEuler;
        private SpriteRenderer sprite;
        private bool isFlip;
        private Vector3 initLocalScale = Vector3.one;

        protected override void Awake()
        {
            base.Awake();
            initEuler = target.eulerAngles;
            initLocalScale = target.localScale;
            sprite = target.GetComponent<SpriteRenderer>();
            if (sprite)
                isFlip = sprite.flipX;
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            target.eulerAngles = initEuler;
            target.localScale = initLocalScale;
            if (sprite)
                sprite.flipX = isFlip;
        }

        /// <summary>
        /// When exiting the state we reset our movement.
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
        }

        public override void PerformAction()
        {

        }
    }
}
