using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// An Action that shoots using the currently equipped weapon. If your weapon is in auto mode, will shoot until you exit the state, and will only shoot once in SemiAuto mode. You can optionnally have the character face (left/right) the target, and aim at it (if the weapon has a WeaponAim component).
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Disable Brain Self")]
    public class AIActionDisableBrainSelf : AIAction
    {
        [SerializeField] AIBrain targetBrain;
        [SerializeField] string targetStateName;

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
            _brain.BrainActive = false;
            targetBrain.TransitionToState(targetStateName);
        }

        /// <summary>
        /// When exiting the state we make sure we're not shooting anymore
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
        }
    }
}
