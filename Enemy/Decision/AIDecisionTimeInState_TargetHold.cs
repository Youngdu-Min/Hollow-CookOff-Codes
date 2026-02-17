using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class AIDecisionTimeInState_TargetHold : AIDecisionTimeInState
    {
        AIDecisionDetectTargetLine line;
        Character character;
        Character targetCharacter;
        PlayerHealth health;
        DamageOnTouch damageArea;

        protected override void Start()
        {
            base.Initialization();

            Invoke(nameof(WaitTarget), 2);


        }

        public override void Initialization()
        {
            base.Initialization();
            RandomizeTime();
        }

        void WaitTarget()
        {
            line = GetComponent<AIDecisionDetectTargetLine>();
            character = GetComponent<Character>();
            if (character == null)
                character = GetComponentInParent<Character>();

            Debug.Log("health target " + _brain.Target);
            targetCharacter = _brain.Target.GetComponent<Character>();
            if (targetCharacter == null)
                targetCharacter = _brain.Target.GetComponentInParent<Character>();

            health = targetCharacter._health as PlayerHealth;
            Debug.Log("health " + health.name);
            var currWeapon = character.FindAbility<CharacterHandleWeapon>().CurrentWeapon as MeleeWeapon_AI;
            damageArea = currWeapon._damageOnTouch;
            Debug.Log("damage " + damageArea.name);
        }

        /// <summary>
        /// On enter state we randomize our next delay
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();

            if (line != null && line.holded) // 잡았으면
            {
                health.catchedEnemy = damageArea.gameObject;
                targetCharacter.Freeze();
            }
        }

        public override void OnExitState()
        {
            base.OnExitState();
            if (line != null && line.holded)
            {
                targetCharacter.UnFreeze();
                line.holded = false;
                health.catchedEnemy = null;
            }
        }
    }
}