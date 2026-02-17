using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using TMPro;
using static MoreMountains.CorgiEngine.Phases;

namespace MoreMountains.CorgiEngine
{
    [System.Serializable]
    public class Phases
    {
        public PhaseType[] phaseType;

        [System.Serializable]
        public class PhaseType
        {
            public GameObject[] objects;
        }
    }

    /// <summary>
    /// An Action that shoots using the currently equipped weapon. If your weapon is in auto mode, will shoot until you exit the state, and will only shoot once in SemiAuto mode. You can optionnally have the character face (left/right) the target, and aim at it (if the weapon has a WeaponAim component).
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Phase Enable")]
    // [RequireComponent(typeof(Character))]
    // [RequireComponent(typeof(CharacterHandleWeapon))]
    public class AIActionPhaseEnable : AIAction
    {
        [SerializeField] private float waitTime;
        [SerializeField] private Phases[] phases;
        [SerializeField] private BossHealthDisplay bossHealthDisplay;
        private bool isInvoked;
        private int randomIdx = -1;
        private PhaseType[] phaseTypes;
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
            if (waitTime > 0)
                Invoke(nameof(EnableTarget), waitTime);
            else
                EnableTarget();
        }

        private void EnableTarget()
        {
            if(isInvoked)
                return;
            isInvoked = true;
            phaseTypes = phases[bossHealthDisplay.CurrPhase - 1].phaseType;
            randomIdx = Random.Range(0, phaseTypes.Length);
            for (int i = 0; i < phaseTypes[randomIdx].objects.Length; i++)
            {
                print("활성화 " + phaseTypes[randomIdx].objects[i].name);
                phaseTypes[randomIdx].objects[i].SetActive(true);
            }
            //targetObj.SetActive(true);
        }

        public void CancelEnableTarget()
        {
            if (phaseTypes == null || phaseTypes.Length == 0 || randomIdx < 0) return;
            for (int i = 0; i < phaseTypes[randomIdx].objects.Length; i++)
            {
                phaseTypes[randomIdx].objects[i].SetActive(false);
            }
        }

        /// <summary>
        /// When exiting the state we make sure we're not shooting anymore
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            //targetObj.SetActive(false);
            CancelInvoke();
            isInvoked = false;
        }
    }
}
