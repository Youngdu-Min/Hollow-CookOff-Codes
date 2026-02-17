using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{
    [System.Serializable]
    public class ActionEvent : UnityEvent<AIBrain> { }
    /// <summary>
    /// This action directs the CharacterHorizontalMovement ability to move in the direction of the target.
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Custom Event")]
    // [RequireComponent(typeof(CharacterHorizontalMovement))]
    public class AIActionCustomEvent: AIAction
    {
        [SerializeField]
        ActionEvent actionEvent;


        public override void OnEnterState()
        {
            base.OnEnterState();
            actionEvent.Invoke(_brain);
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
