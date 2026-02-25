using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// A basic melee weapon class, that will activate a "hurt zone" when the weapon is used
    /// </summary>
    [AddComponentMenu("Corgi Engine/Weapons/Melee Weapon_AI")]
    public class MeleeWeapon_AI : Weapon
    {
        /// the possible shapes for the melee weapon's damage area
        public enum MeleeDamageAreaShapes { Rectangle, Circle }

        [MMInspectorGroup("Melee Damage Area", true, 65)]

        /// the shape of the damage area (rectangle or circle)
        [Tooltip("the shape of the damage area (rectangle or circle)")]
        public MeleeDamageAreaShapes DamageAreaShape = MeleeDamageAreaShapes.Rectangle;
        /// the size of the damage area
        [Tooltip("the size of the damage area")]
        public Vector2 AreaSize = new Vector2(1, 1);
        /// the offset to apply to the damage area (from the weapon's attachment position
        [Tooltip("the offset to apply to the damage area (from the weapon's attachment position")]
        public Vector2 AreaOffset = new Vector2(1, 0);

        [MMInspectorGroup("Melee Damage Area Timing", true, 70)]

        /// the initial delay to apply before triggering the damage area
        [Tooltip("the initial delay to apply before triggering the damage area")]
        public float InitialDelay = 0f;
        /// the duration during which the damage area is active
        [Tooltip("the duration during which the damage area is active")]
        public float ActiveDuration = 1f;

        [MMInspectorGroup("Melee Damage Caused", true, 75)]

        /// the layers that will be damaged by this object
        [Tooltip("the layers that will be damaged by this object")]
        public LayerMask TargetLayerMask;
        /// The amount of health to remove from the player's health
        [Tooltip("The amount of health to remove from the player's health")]
        public int DamageCaused = 10;
        /// the kind of knockback to apply
        [Tooltip("the kind of knockback to apply")]
        public DamageOnTouch.KnockbackStyles Knockback;
        /// The force to apply to the object that gets damaged
        [Tooltip("The force to apply to the object that gets damaged")]
        public Vector2 KnockbackForce = new Vector2(10, 2);
        /// The duration of the invincibility frames after the hit (in seconds)
        [Tooltip("The duration of the invincibility frames after the hit (in seconds)")]
        public float InvincibilityDuration = 0.5f;

        protected Collider2D _damageAreaCollider;
        protected bool _attackInProgress = false;

        protected Color _gizmosColor;
        protected Vector3 _gizmoSize;

        protected CircleCollider2D _circleCollider2D;
        protected BoxCollider2D _boxCollider2D;
        protected Vector3 _gizmoOffset;
        public DamageOnTouch _damageOnTouch;
        public bool isStun;
        public bool stunInfinite;
        protected GameObject _damageArea;

        protected bool _hitEventSent = false;
        protected bool _hitDamageableEventSent = false;
        protected bool _hitNonDamageableEventSent = false;
        protected bool _killEventSent = false;
        protected Animator animator;
        protected AIBrain _brain;

        public DamageOnTouch.DamageType normalDamage;
        public DamageOnTouch.DamageType normalDamage_Air;
        public DamageOnTouch.DamageType powerDamage;

        [MMInspectorGroup("Brain", true, 80)]
        public bool onHitChangeState;
        public string changeStateStr;


        IEnumerator coroutine;

        /// <summary>
        /// Initialization
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();

            animator = Owner.GetComponent<Animator>();
            if (animator == null)
            {
                animator = Owner.GetComponentInChildren<Animator>();
            }

            if (_damageArea == null)
            {
                CreateDamageArea();
                DisableDamageArea();
                RegisterEvents();
            }

            _brain = Owner.CharacterBrain;

        }

        /// <summary>
        /// Creates the damage area.
        /// </summary>
        protected virtual void CreateDamageArea()
        {
            _damageArea = new GameObject
            {
                name = this.name + "DamageArea"
            };
            _damageArea.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            _damageArea.transform.SetParent(this.transform);

            if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
            {
                _boxCollider2D = _damageArea.AddComponent<BoxCollider2D>();
                _boxCollider2D.offset = AreaOffset;
                _boxCollider2D.size = AreaSize;
                _damageAreaCollider = _boxCollider2D;
            }
            if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
            {
                _circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
                _circleCollider2D.transform.position = this.transform.position + this.transform.rotation * AreaOffset;
                _circleCollider2D.radius = AreaSize.x / 2;
                _damageAreaCollider = _circleCollider2D;
            }

            _damageAreaCollider.isTrigger = true;

            Rigidbody2D rigidBody = _damageArea.AddComponent<Rigidbody2D>();
            rigidBody.isKinematic = true;

            _damageArea.layer = 2;
            _damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.useEnumDamage = true;
            _damageOnTouch.DamageCausedKnockbackType = Knockback;
            _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
            _damageOnTouch.DamageCausedKnockbackForce.x = HollowBalance.action.actionList[24].floatValue;
            _damageOnTouch.damageType = DamageOnTouch.DamageType.FistPunch;
            _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
            _damageOnTouch.isStun = isStun;
            _damageOnTouch.stunInfinite = stunInfinite;


        }

        /// <summary>
        /// When the weapon is used, we trigger our attack routine
        /// </summary>
        protected override void WeaponUse()
        {
            _brain = Owner.CharacterBrain;
            base.WeaponUse();
            coroutine = MeleeWeaponAttack();
            StartCoroutine(coroutine);
        }

        /// <summary>
        /// Triggers an attack, turning the damage area on and then off
        /// </summary>
        /// <returns>The weapon attack.</returns>
        protected virtual IEnumerator MeleeWeaponAttack()
        {
            if (_attackInProgress) { yield break; }

            _attackInProgress = true;
            if (_brain.CurrentState.initialCanParry)
            {
                yield return new WaitForSeconds(InitialDelay);
                _damageOnTouch.damageType = powerDamage;
                _brain._weaponState = AIBrain.WeaponState.Special;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);

                if (!_controller.State.IsGrounded)
                    _damageOnTouch.damageType = normalDamage_Air;
                else
                    _damageOnTouch.damageType = normalDamage;

                _brain._weaponState = AIBrain.WeaponState.Normal;
            }
            OnDisable();
            RegisterEvents();
            EnableDamageArea();
            _damageOnTouch.SetDamage();
            yield return MMCoroutine.WaitForFrames(1);
            HandleMiss();
            yield return new WaitForSeconds(ActiveDuration);

            DisableDamageArea();

        }

        /// <summary>
        /// Enables the damage area.
        /// </summary>
        protected virtual void EnableDamageArea()
        {
            _hitEventSent = false;
            _hitDamageableEventSent = false;
            _hitNonDamageableEventSent = false;
            _killEventSent = false;
            _damageAreaCollider.enabled = true;
            animator?.SetBool("DamageArea", true);
        }

        /// <summary>
        /// Triggers a weapon miss if no hit was detected last frame
        /// </summary>
        protected virtual void HandleMiss()
        {
            if (!_hitEventSent)
            {
                WeaponMiss();
            }
        }

        public override void TurnWeaponOff()
        {
            base.TurnWeaponOff();
            OnDisable();
            DisableDamageArea();
        }

        /// <summary>
        /// Disables the damage area.
        /// </summary>
        protected virtual void DisableDamageArea()
        {
            _damageAreaCollider.enabled = false;
            if (_brain == null)
            {
                Initialization();
            }
            _brain._weaponState = AIBrain.WeaponState.None;
            animator?.SetBool("DamageArea", false);
            _attackInProgress = false;
        }

        /// <summary>
        /// Draws the melee weapon's range
        /// </summary>
        protected virtual void DrawGizmos()
        {
            _gizmoOffset = AreaOffset;

            Gizmos.color = Color.red;
            if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
            {
                Gizmos.DrawWireSphere(this.transform.position + _gizmoOffset, AreaSize.x / 2);
            }
            if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
            {
                MMDebug.DrawGizmoRectangle(this.transform.position + _gizmoOffset, AreaSize, Color.red);
            }
        }

        /// <summary>
        /// Draws gizmos on selected if the app is not playing
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                DrawGizmos();
            }
        }

        /// <summary>
        /// When we get a hit, we trigger one on the main class
        /// </summary>
        protected virtual void OnHit()
        {
            WeaponHit();
            _brain.CurrentState.SetCanParry(false);
            _hitEventSent = true;

            if (onHitChangeState)
            {
                TurnWeaponOff();
                _brain.TransitionToState(changeStateStr);
            }
        }

        /// <summary>
        /// When we get a HitDamageable, we trigger one on the main class
        /// </summary>
        protected virtual void OnHitDamageable()
        {
            if (!_hitDamageableEventSent)
            {
                WeaponHitDamageable();
                _hitDamageableEventSent = true;
            }

        }


        /// <summary>
        /// When we get a HitNonDamageable, we trigger one on the main class
        /// </summary>
        protected virtual void OnHitNonDamageable()
        {
            if (!_hitNonDamageableEventSent)
            {
                WeaponHitNonDamageable();
                _hitNonDamageableEventSent = true;
            }
        }

        /// <summary>
        /// When we get a Kill, we trigger one on the main class
        /// </summary>
        protected virtual void OnKill()
        {
            StopCoroutine(coroutine);
            DisableDamageArea();
            if (!_killEventSent)
            {
                WeaponKill();
                _killEventSent = true;
            }
        }

        /// <summary>
        /// On enable, we reset the object's speed
        /// </summary>
        protected virtual void RegisterEvents()
        {
            if (_damageOnTouch != null)
            {
                _damageOnTouch.OnKill += OnKill;
                _damageOnTouch.OnHit += OnHit;
                _damageOnTouch.OnHitDamageable += OnHitDamageable;
                _damageOnTouch.OnHitNonDamageable += OnHitNonDamageable;
            }
        }

        /// <summary>
        /// On Disable we unsubscribe from our delegates
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_damageOnTouch != null)
            {
                _damageOnTouch.OnKill -= OnKill;
                _damageOnTouch.OnHit -= OnHit;
                _damageOnTouch.OnHitDamageable -= OnHitDamageable;
                _damageOnTouch.OnHitNonDamageable -= OnHitNonDamageable;

            }
        }
    }
}
