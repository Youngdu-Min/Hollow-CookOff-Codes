using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// A basic melee weapon class, that will activate a "hurt zone" when the weapon is used
    /// </summary>
    [AddComponentMenu("Corgi Engine/Weapons/Multiple Melee Weapon_AI")]
    public class MultipleMeleeWeapon_AI : MeleeWeapon_AI
    {

        [MMInspectorGroup("Attack", true, 80)]
        public bool isFreeze;
        private int attackCount;
        private float attackInterval;
        private bool isForceEnd;
        [SerializeField] private int startFrame;

        protected void Start()
        {
            attackCount = (int)EnemyBalance.etc.etcList[3].floatValue;
            attackInterval = EnemyBalance.etc.etcList[2].floatValue;
            Debug.Log(attackCount + " " + attackInterval + " 어택");
        }

        protected override void CaseWeaponStart()
        {
            base.CaseWeaponStart();
            animator.SetBool("DamageArea", true);
        }


        protected override IEnumerator MeleeWeaponAttack()
        {
            Debug.Log("공격 실행 전 " + _attackInProgress);

            if (_attackInProgress) { yield break; }

            _attackInProgress = true;
            isForceEnd = false;
            OnDisable();
            RegisterEvents();


            Character character = GameObject.FindWithTag("Player").GetComponent<Character>();
            Health cape = character.GetComponentInChildren<Cape>(true).GetComponent<Health>();
            PlayerHealth playerHealth = character.GetComponent<PlayerHealth>();
            int targetLayer = character.gameObject.layer;

            //yield return new WaitForSeconds(attackInterval);
            EnableDamageArea();
            yield return new WaitForSeconds(0.05f);
            HandleMiss();

            if (!_hitEventSent)
                yield break;

            //if (Owner.MovementState.CurrentState == CharacterStates.MovementStates.Falling)
            //    Owner.Freeze();

            playerHealth.catchedEnemy = _damageArea;
            SetDefinedPosition();
            character.gameObject.layer = LayerMask.NameToLayer("NoCollision");


            DisableDamageArea();

            for (int i = 0; i < attackCount - 1; i++)
            {
                yield return MMCoroutine.WaitFor(attackInterval);
                if (character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Normal)
                {
                    WeaponEndManually();
                    character.gameObject.layer = targetLayer;
                    yield break;
                }
                if (i == attackCount - 2)
                {
                    playerHealth.Damage(_damageOnTouch.DamageCaused, _damageOnTouch.gameObject, playerHealth.InvincibleTime, playerHealth.InvincibleTime, Vector3.up);
                    TryDamageTarget(cape, _damageOnTouch.DamageCaused, _damageOnTouch.gameObject, playerHealth.InvincibleTime, playerHealth.InvincibleTime, Vector3.up);
                }
                else
                {
                    playerHealth.Damage(_damageOnTouch.DamageCaused, _damageOnTouch.gameObject, _damageOnTouch.InvincibilityDuration, _damageOnTouch.InvincibilityDuration, Vector3.up);
                    TryDamageTarget(cape, _damageOnTouch.DamageCaused, _damageOnTouch.gameObject, _damageOnTouch.InvincibilityDuration, _damageOnTouch.InvincibilityDuration, Vector3.up);
                }
                WeaponOnHitFeedback?.PlayFeedbacks();
            }
            yield return MMCoroutine.WaitFor(attackInterval);

            if (character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
            {
                if (character.transform.position.x < Owner.transform.position.x)
                    character.GetComponent<CorgiController>().AddForce(new Vector2(-KnockbackForce.x, KnockbackForce.y));
                else
                    character.GetComponent<CorgiController>().AddForce(KnockbackForce);
                character.UnFreeze();
            }

            playerHealth.catchedEnemy = null;
            character.gameObject.layer = targetLayer;
            animator.SetBool("DamageArea", false);
            animator.SetBool("Hit", _hitEventSent);

            TurnWeaponOff();
            Owner.UnFreeze();
            Debug.Log("종료");

            yield return MMCoroutine.WaitFor(attackInterval);
            _attackInProgress = false;

            void SetDefinedPosition()
            {
                float xOffset = 1;
                if (character.transform.position.x < Owner.transform.position.x)
                    xOffset *= -1;

                CorgiController targetCorgi = playerHealth.GetCorgi();
                if (Owner.MovementState.CurrentState == CharacterStates.MovementStates.Falling)
                    targetCorgi.SetTransformPosition(new Vector2(_damageArea.transform.position.y + xOffset, _damageArea.transform.position.y));
            }

            void TryDamageTarget(Health health, int damage, GameObject instigator, float flickerDuration,
            float invincibilityDuration, Vector3 damageDirection)
            {
                if (health == null || !health.gameObject.activeInHierarchy)
                    return;

                cape.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection);
            }
        }

        protected override void DisableDamageArea()
        {
            Debug.Log("콜라이더 끔");
            _damageAreaCollider.enabled = false;
            if (_brain == null)
            {
                Initialization();
            }
            _brain._weaponState = AIBrain.WeaponState.None;
            _attackInProgress = false;
        }

        protected override void CreateDamageArea()
        {
            base.CreateDamageArea();
            _damageOnTouch.InvincibilityDuration = 0;
            _damageOnTouch.isFreeze = isFreeze;
            _damageOnTouch.useEnumDamage = true;
            _damageOnTouch.damageType = DamageOnTouch.DamageType.Grind;
            //_damageOnTouch.InvincibilityDuration = EnemyBalance.etc.etcList[2].floatValue;
        }

        protected override void HandleMiss()
        {
            animator.SetBool("Hit", _hitEventSent);
            if (_hitEventSent)
                return;

            Debug.Log("스턴 미스");
            WeaponMiss();
            TurnWeaponOff();
            Owner.UnFreeze();
            //_brain.ResetBrain();

            animator.SetBool("DamageArea", false);
            _brain.TransitionToState("MissDelay");
            _attackInProgress = false;
        }

        public void SetForceEnd()
        {
            isForceEnd = true;
        }

        public void WeaponEndManually()
        {
            print("강제 종료");
            animator.SetBool("Hit", false);
            WeaponMiss();
            TurnWeaponOff();
            Owner.UnFreeze();

            animator.SetBool("DamageArea", false);
            _brain.TransitionToState("MissDelay");
            _attackInProgress = false;
        }
    }
}
