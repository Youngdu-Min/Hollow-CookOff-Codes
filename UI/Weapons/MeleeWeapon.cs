using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// A basic melee weapon class, that will activate a "hurt zone" when the weapon is used
    /// </summary>
    [AddComponentMenu("Corgi Engine/Weapons/Melee Weapon")]
    public class MeleeWeapon : Weapon
    {
        /// the possible shapes for the melee weapon's damage area
        public enum MeleeDamageAreaShapes { Rectangle, Circle }

        [MMInspectorGroup("Melee Damage Area", true, 65)]

        /// 데미지 구역의 모양 설정
        [Tooltip("데미지 구역의 모양 설정")]
        public MeleeDamageAreaShapes DamageAreaShape = MeleeDamageAreaShapes.Rectangle;
        /// 데미지 구역의 크기
        [Tooltip("데미지 구역의 크기")]
        public Vector2 AreaSize = new Vector2(1, 1);
        /// 데미지 구역의 오프셋
        [Tooltip("데미지 구역의 오프셋")]
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
        [Tooltip("The duration of the invincibility frames after the hit (in seconds)")]
        public float InvincibilityDuration = 0.5f;

        protected Collider2D _damageAreaCollider;
        [HideInInspector]
        public bool _attackInProgress = false;

        protected Color _gizmosColor;
        protected Vector3 _gizmoSize;

        protected CircleCollider2D _circleCollider2D;
        protected BoxCollider2D _boxCollider2D;
        protected Vector3 _gizmoOffset;
        [HideInInspector]
        public DamageOnTouch_Knife _damageOnTouch;
        protected GameObject _damageArea;

        protected bool _hitEventSent = false;
        protected bool _hitDamageableEventSent = false;
        protected bool _hitNonDamageableEventSent = false;
        protected bool _killEventSent = false;
        [HideInInspector]
        public bool isCharge;
        float btDownTime;
        [MMInspectorGroup("딜레이, 쿨다운", true, 76)]
        [Tooltip("쿨다운")]
        public float coolDown;
        [Tooltip("에어본 딜레이")]
        public float airDelay;
        [Tooltip("나이프 거리")]
        public float knifeDistance;
        // 벽 나이프 거리
        float wallKnifeDistance;

        Character _chara;
        [HideInInspector]
        public float currCoolDown;
        CharacterHorizontalMovement horizontalMovement;
        float _distanceTraveled;
        [HideInInspector]
        public Vector3 _initialPosition;
        float knifeSpeed;
        [HideInInspector]
        public bool isKeyUp;
        protected bool _shouldKeepKnifing = true;
        protected IEnumerator _meleeCoroutine;
        protected MMStateMachine<CharacterStates.MovementStates> _movement;
        protected MMStateMachine<CharacterStates.CharacterConditions> _condition;
        [HideInInspector]
        public Vector2 dir;

        // animation parameters
        protected Animator _animator;
        protected const string _knifingAnimationParameterName = "Knifing";
        protected int _knifingAnimationParameter;

        protected const string _airborningAnimationParameterName = "Airborning";
        protected int _airborningAnimationParameter;

        [MMInspectorGroup("ETC", true, 76)]
        public GameObject slashVisualEffect;
        public MMFeedbacks WeaponOnSlashFeedback;
        public MMFeedbacks WeaponOnSlashEndFeedback;
        public MMFeedbacks WeaponOnParryFeedback;
        public MMFeedbacks WeaponOnParryEndFeedback;
        private CharacterHandleWeapon characterHandleWeapon;

        /// <summary>
        /// Initialization
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
            DamageCaused = (int)HollowBalance.action.actionList[13].floatValue;
            if (_damageArea == null)
            {
                CreateDamageArea();
                DisableDamageArea();
                RegisterEvents();
            }
            _damageOnTouch.Owner = Owner.gameObject;
            horizontalMovement = Owner.GetComponent<CharacterHorizontalMovement>();
            isCharge = false;
            knifeDistance = HollowBalance.action.actionList[12].floatValue;
            coolDown = HollowBalance.action.actionList[14].floatValue;
            knifeSpeed = HollowBalance.action.actionList[23].floatValue;
            _chara = Owner.GetComponent<Character>();
            _movement = _chara.MovementState;
            _condition = _chara.ConditionState;
            characterHandleWeapon = _chara.GetComponent<CharacterHandleWeapon>();

            _animator = _chara._animator;
            if (_animator != null)
            {
                InitializeAnimatorParameters();
            }


        }

        protected override void Update()
        {
            ApplyOffset();

            if (Input.GetButton("Player1_SecondaryShoot"))
            {
                btDownTime += Time.deltaTime;
                AirBornePiUI.Instance().saveNum = btDownTime / HollowBalance.action.actionList[30].floatValue;
                // Debug.Log(btDownTime);
            }
            else if (Input.GetButtonUp("Player1_SecondaryShoot"))
            {
                if (currCoolDown > 0) // 공격하기 위해 버튼 뗀 직후 에어본 시간 초기화 방지
                    btDownTime = 0;
            }
            if (!_attackInProgress)
            {
                currCoolDown -= Time.deltaTime;
            }

            AirBornePiUI.Instance().ShowBlockImg(currCoolDown > 0);
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
                //print($"나이프 사이즈 {AreaSize} {AreaOffset}");
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

            _damageOnTouch = _damageArea.AddComponent<DamageOnTouch_Knife>();
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.DamageCaused = DamageCaused;
            _damageOnTouch.DamageCausedKnockbackType = Knockback;

            _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
            _damageArea.layer = gameObject.layer;
        }

        public virtual bool KnifeConditions()
        {
            if (_chara.MovementState.CurrentState == CharacterStates.MovementStates.KnifeExplosioning || _chara.MovementState.CurrentState == CharacterStates.MovementStates.Parry || _chara.MovementState.CurrentState == CharacterStates.MovementStates.Dodging || _chara.MovementState.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                return false;
            }


            return true;
        }

        protected override void TurnWeaponOn()
        {
            if (!KnifeConditions())
            {
                return;
            }

            WeaponState.ChangeState(WeaponStates.WeaponStart);
            if ((_characterHorizontalMovement != null) && (ModifyMovementWhileAttacking))
            {
                _movementMultiplierStorage = _characterHorizontalMovement.MovementSpeedMultiplier;
                _characterHorizontalMovement.MovementSpeedMultiplier = MovementMultiplier;
            }
            if (_comboWeapon != null)
            {
                _comboWeapon.WeaponStarted(this);
            }
            if (PreventHorizontalMovementWhileInUse && (_characterHorizontalMovement != null) && (_controller != null))
            {
                _characterHorizontalMovement.MovementForbidden = true;
            }

            if (currCoolDown <= 0)
            {

                // we set its dashing state to true
                _movement.ChangeState(CharacterStates.MovementStates.Meleeing);
                Debug.Log("coolDown " + currCoolDown);
                TriggerWeaponStartFeedback();

                TriggerWeaponUsedFeedback();
                _initialPosition = this.transform.position;

                dir = characterHandleWeapon.CurrentWeapon.transform.right * knifeSpeed;
                Debug.Log("weaponON, " + dir);

                _applyForceWhileInUse = true;
                _shouldKeepKnifing = true;

                RaycastHit2D hit;
                hit = Physics2D.Raycast(transform.position, dir, knifeDistance, 1 << LayerMask.NameToLayer("Platforms"));
                if (dir.y < transform.position.y)
                {
                    if (_chara.IsFacingRight)
                        hit = Physics2D.Raycast(transform.position, Vector2.right, knifeDistance, 1 << LayerMask.NameToLayer("Platforms"));
                    else if (!_chara.IsFacingRight)
                        hit = Physics2D.Raycast(transform.position, -Vector2.right, knifeDistance, 1 << LayerMask.NameToLayer("Platforms"));
                }
                Debug.DrawLine(transform.position, hit.point, Color.red);
                if (hit.collider != null)
                {

                    wallKnifeDistance = Mathf.Abs(hit.point.x - transform.position.x) - 1.5f;
                }
                else
                {
                    wallKnifeDistance = knifeDistance; // 임의의 값 사용

                }
                _meleeCoroutine = Melee();
                StartCoroutine(_meleeCoroutine);
                if (!isCharge)
                    StartCoroutine(MeleeWeaponAttack());
                currCoolDown = coolDown;
                // characterHandleWeapon.AbilityPermitted = false;
                // characterHandleWeapon.CurrentWeapon.enabled = false;
            }
        }

        protected override void WeaponUse()
        {
            ApplyRecoil(ApplyRecoilOnUse, RecoilOnUseProperties);
        }

        /// <summary>
        /// Triggers an attack, turning the damage area on and then off
        /// </summary>
        /// <returns>The weapon attack.</returns>

        public IEnumerator MeleeWeaponAttack()
        {
            Debug.Log("Attack");
            if (_attackInProgress) { yield break; }
            horizontalMovement.enabled = true;
            _attackInProgress = true;

            _controller.SetForce(Vector2.zero);
            _characterHorizontalMovement.SetHorizontalMove(0f);

            if (btDownTime >= HollowBalance.action.actionList[30].floatValue)
            {
                Debug.Log("Attack_Air");
                _damageOnTouch.DamageCausedKnockbackForce = HollowBalance.action.actionList[29].vectorValue;
                isCharge = true;
            }

            Debug.Log($"나이프 넉백 {_damageOnTouch.DamageCausedKnockbackForce} / {HollowBalance.action.actionList[29].vectorValue}");
            yield return new WaitForSeconds(InitialDelay);
            EnableDamageArea();
            yield return MMCoroutine.WaitForFrames(1);
            HandleMiss();

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
            Debug.Log("ON");
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

        /// <summary>
        /// Disables the damage area.
        /// </summary>
        protected virtual void DisableDamageArea()
        {
            _damageAreaCollider.enabled = false;
            Debug.Log("OFF");
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
            //_damageOnTouch._collidingCollider.GetComponent<DamageOnTouch>().onlyKnockback = false;
            // _delayBetweenUsesCounter = 0;

            if (!_hitEventSent)
            {
                WeaponHit();
                _hitEventSent = true;

            }
        }

        public override void WeaponHit()
        {
            Debug.Log(isCharge);
            TriggerWeaponOnHitFeedback();

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
            if (!_killEventSent)
            {
                //_attackInProgress = false;
                //StartCoroutine(MeleeWeaponAttack());
                WeaponKill();
                _killEventSent = true;
                _damageAreaCollider.enabled = true;


            }
        }

        protected virtual IEnumerator Melee()
        {
            // if the character is not in a position where it can move freely, we do nothing.
            /*
            if (!AbilityAuthorized
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                yield break;
            }
            */
            while (
            _shouldKeepKnifing
            && !_controller.State.TouchingLevelBounds
            && _movement.CurrentState == CharacterStates.MovementStates.Meleeing && _condition.CurrentState == CharacterStates.CharacterConditions.Normal)
            {
                _distanceTraveled = Vector3.Distance(_initialPosition, this.transform.position);

                if (!isCharge)
                    _damageOnTouch.DamageCausedKnockbackForce = new Vector2(Mathf.Abs((wallKnifeDistance - _distanceTraveled) * dir.x) / 17, 0);

                Debug.Log("나이프 " + _distanceTraveled + ", " + wallKnifeDistance);
                _controller.GravityActive(false);
                if ((_controller.State.IsCollidingLeft && dir.x < 0f)
                   || (_controller.State.IsCollidingRight && dir.x > 0f) || _distanceTraveled > wallKnifeDistance)
                    _shouldKeepKnifing = false;
                else
                {
                    if (!ForceToAim || (_controller.State.IsCollidingAbove && dir.y > 0f) || (_controller.State.IsCollidingBelow && dir.y < 0f))
                        dir.x = dir.x > 0f ? knifeSpeed : -knifeSpeed;

                    Debug.Log("weaponON After, " + dir);
                    _controller.SetForce(dir);

                }

                yield return null;
            }

            StartCoroutine(StopMelee());
        }

        public void CancelMelee()
        {
            _shouldKeepKnifing = false;
        }

        /// <summary>
        /// Stops the dash coroutine and resets all necessary parts of the character
        /// </summary>
        public IEnumerator StopMelee()
        {
            if (_meleeCoroutine == null)
            {
                yield break;
            }

            print($"나이프 끝 {_movement.CurrentState}");
            if (_meleeCoroutine != null)
            {
                StopCoroutine(_meleeCoroutine);
            }

            _controller.GravityActive(true);
            _controller.SetForce(Vector2.zero);

            // health.TemporaryInvulnerable = false;

            if (_movement.CurrentState == CharacterStates.MovementStates.Meleeing || _movement.CurrentState == CharacterStates.MovementStates.Parry)
            {
                yield return new WaitForSeconds(0.1f);
                print($"나이프 끝2");
                if (_controller.State.IsGrounded || _movement.PreviousState == CharacterStates.MovementStates.Meleeing)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                }
                else
                {
                    _movement.RestorePreviousState();
                }
            }
            ResetAbility();
            DisableDamageArea();
            _attackInProgress = false;
            isCharge = false;
            btDownTime = 0;
            horizontalMovement.enabled = true;
            yield return null;
            _damageOnTouch.hitEnemy.Clear();
            _damageOnTouch.EndDelayAttack();
            _meleeCoroutine = null;
            Owner._health.TemporaryInvulnerable = false;
            // characterHandleWeapon.CurrentWeapon.enabled = true;
            // sprite.enabled = true;
        }

        [ContextMenu("슬래시")]
        public virtual void TriggerWeaponSlashFeedback()
        {
            print($"슬래시 이펙트 시작");
            WeaponOnSlashFeedback?.PlayFeedbacks(this.transform.position);
            float rotAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            WeaponOnSlashFeedback?.transform.Rotate(Vector3.forward, rotAngle);
        }

        public virtual void TriggerWeaponSlashEndFeedback()
        {
            print($"슬래시 이펙트 끝");
            WeaponOnSlashEndFeedback?.PlayFeedbacks(this.transform.position);
        }

        [ContextMenu("패리")]
        public virtual void TriggerWeaponParryFeedback()
        {
            WeaponOnParryFeedback?.PlayFeedbacks(this.transform.position);
        }

        public virtual void TriggerWeaponParryEndFeedback()
        {
            WeaponOnParryEndFeedback?.PlayFeedbacks(this.transform.position);
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>

        public override void InitializeAnimatorParameters()
        {
            base.InitializeAnimatorParameters();
            RegisterAnimatorParameter(_knifingAnimationParameterName, AnimatorControllerParameterType.Bool, out _knifingAnimationParameter);
            RegisterAnimatorParameter(_airborningAnimationParameterName, AnimatorControllerParameterType.Bool, out _airborningAnimationParameter);
        }

        /// <summary>
        /// At the end of the cycle, we update our animator's Dashing state 
        /// </summary>
        public override void UpdateAnimator()
        {
            base.UpdateAnimator();
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _knifingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Meleeing), _chara._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _airborningAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Airborning), _chara._animatorParameters);

        }

        /// <summary>
        /// On reset ability, we cancel all the changes made
        /// </summary>
        protected void ResetAbility()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _knifingAnimationParameter, false, _chara._animatorParameters);
        }

        public void ResetAirborneAbility()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _airborningAnimationParameter, false, _chara._animatorParameters);
        }

        protected virtual void RegisterAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType, out int parameter)
        {
            parameter = Animator.StringToHash(parameterName);

            if (_animator == null)
            {
                _chara = Owner.GetComponent<Character>();
                _animator = _chara._animator;
            }
            if (_animator.MMHasParameterOfType(parameterName, parameterType))
            {
                _chara._animatorParameters.Add(parameter);
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
