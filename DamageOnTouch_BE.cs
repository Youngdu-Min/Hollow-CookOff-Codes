using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to an object and it will cause damage to objects that collide with it. 
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Damage/DamageOnTouch_BE")]
    public class DamageOnTouch_BE : DamageOnTouch
    {
        new Character character;

        protected override void Awake()
        {
            base.Awake();
            character = GetComponentInParent<Character>();
        }
        protected override void Colliding(Collider2D collider)
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            // if the object we're colliding with is part of our ignore list, we do nothing and exit
            if (_ignoredGameObjects.Contains(collider.gameObject))
            {
                return;
            }

            // if what we're colliding with isn't part of the target layers, we do nothing and exit
            if (!MMLayers.LayerInLayerMask(collider.gameObject.layer, TargetLayerMask))
            {
                return;
            }

            _collidingCollider = collider;
            _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();
            var _colliderChara = collider.gameObject.GetComponent<Character>();
            var _colliderMelee = collider.gameObject.GetComponentInChildren<MultipleMeleeWeapon_AI>();
            print($"BE collide {_collidingCollider}");
            if (_colliderMelee != null)
            {
                print($"BE collide melee {_colliderMelee}");
            }


            OnHit?.Invoke();

            if (isStun)
            {
                character.Stun();
            }

            _colliderMelee?.SetForceEnd();

            if (_colliderChara?.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
                _colliderChara?.UnFreeze();

            // if what we're colliding with is damageable
            if ((_colliderHealth != null) && (_colliderHealth.enabled))
            {
                if (_colliderHealth.CurrentHealth > 0)
                {
                    OnCollideWithDamageable(_colliderHealth);
                }
            }
            // if what we're colliding with can't be damaged
            else
            {
                OnCollideWithNonDamageable();
            }

        }

        private void OnDisable()
        {
            if (isStun)
            {
                if (character != null)
                {
                    character.UnFreeze();
                }

            }
        }
    }
}