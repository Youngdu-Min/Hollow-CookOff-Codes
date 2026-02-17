using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This action directs the CharacterHorizontalMovement ability to move in the direction of the target.
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Change Target Event")]
    // [RequireComponent(typeof(CharacterHorizontalMovement))]
    public class AIActionChangeTargetEvent : AIAction
    {
        [SerializeField] private Transform _target;
        [SerializeField] private bool _isRevertTarget;
        private Transform _prevTarget;

        public override void OnEnterState()
        {
            base.OnEnterState();
            _prevTarget = _brain.Target;
            _brain.Target = _target;
            //actionEvent.Invoke(_brain);
        }

        /// <summary>
        /// When exiting the state we reset our movement.
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            if (_isRevertTarget)
            {
                _brain.Target = _prevTarget;
            }
        }

        public override void PerformAction()
        {

        }
    }
}
