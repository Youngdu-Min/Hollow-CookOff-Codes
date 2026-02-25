using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// An Action that shoots using the currently equipped weapon. If your weapon is in auto mode, will shoot until you exit the state, and will only shoot once in SemiAuto mode. You can optionnally have the character face (left/right) the target, and aim at it (if the weapon has a WeaponAim component).
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Active Other Brains")]
    public class AIActionActiveOtherBrains : AIAction
    {
        [SerializeField]
        private List<AIBrain> brains = new List<AIBrain>();

        private AIBrain lastBrain;
        [SerializeField]
        private UnityEvent alterBrainEvent;
        [SerializeField]
        private UnityEvent nullBrainsEvent;

        [SerializeField]
        private string allFreezeTransitionState;
        private Character[] characters;
        private bool isAllFreeze = false;

        protected override void Initialization()
        {
            base.Initialization();
            characters = new Character[brains.Count];

            for (int i = 0; i < brains.Count; i++)
            {
                characters[i] = brains[i].GetComponent<Character>() ?? brains[i].GetComponentInParent<Character>();
            }
            
        }

        /// <summary>
        /// On PerformAction we face and aim if needed, and we shoot
        /// </summary>
        public override void PerformAction()
        {

        }

        /// <summary>
        /// 이전에 활성화한 뇌와 중복하지 않으며 실행
        /// </summary>
        public void ActiveRandomBrain()
        {
            DisableAllBrain();

            int randomIdx = -1;

            int nonFreezeIdx = -1;
            int freezeCount = 0;
            for (int i = 0; i < brains.Count; i++)
            {
                if (brains[i].CurrentState.StateName.Equals(brains[i].Freeze))
                {
                    freezeCount++;
                }
                else
                {
                    nonFreezeIdx = i;
                }
            }

            while (randomIdx < 0 || brains[randomIdx] == lastBrain || brains[randomIdx].CurrentState.StateName.Equals(brains[randomIdx].Freeze))
            {
                if (brains.Count == 1)
                {
                    randomIdx = 0;
                    break;
                }

               if(freezeCount == brains.Count)
                {
                    _brain.TransitionToState(allFreezeTransitionState);
                    isAllFreeze = true;
                    return;
                }
               else if(freezeCount == brains.Count - 1)
                {
                    randomIdx = nonFreezeIdx;
                    break;
                }

                randomIdx = Random.Range(0, brains.Count);
                brains[randomIdx].BrainActive = true;

                brains[randomIdx].BrainActive = false;
            }

            brains[randomIdx].BrainActive = true;
            brains[randomIdx].ResetBrain();
            brains[randomIdx].Target = _brain.Target;
            lastBrain = brains[randomIdx];

        }

        public void TryUnfreezeBrain()
        {
            if (isAllFreeze)
            {
                ActiveRandomBrain();
                isAllFreeze = false;
            }
        }

        public void ActiveRandomBrain(int i)
        {
            DisableAllBrain();

            brains[i].BrainActive = true;
            brains[i].ResetBrain();
            brains[i].Target = _brain.Target;
            lastBrain = brains[i];

        }


        public void RemoveBrain(AIBrain _brain)
        {
            brains.Remove(_brain);
            if (lastBrain == _brain)
            {
                lastBrain = null;
                alterBrainEvent.Invoke();
            }

            if (brains.Count == 0)
                nullBrainsEvent.Invoke();
        }

        public void DisableAllBrain()
        {
            for (int i = 0; i < brains.Count; i++)
            {
                brains[i].BrainActive = false;
            }
        }

        /// <summary>
        /// Faces the target if required
        /// </summary>

        public override void OnEnterState()
        {
            base.OnEnterState();
            ActiveRandomBrain();
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
