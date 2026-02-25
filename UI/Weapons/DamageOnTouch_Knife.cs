using DG.Tweening;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Corgi Engine/Character/Damage/DamageOnTouch_Knife")]

public class DamageOnTouch_Knife : DamageOnTouch
{
    ArmorHealthDisplay armor;
    MeleeWeapon melee;
    BulletParry bulletParry;
    BioEnerge bioEnerge;
    Character _chara;
    public List<GameObject> hitEnemy { get; private set; } = new List<GameObject>();
    /// <summary>
    /// Describes what happens when colliding with a damageable object
    /// </summary>
    /// <param name="health">Health.</param>
    private bool isPaused;
    private bool delayAttack;
    private readonly float parryTimescale = 0.01f;
    private readonly float bulletParryWaitTime = 3f;
    private readonly float parryWaitTime = 0.6f;
    private readonly float slashWaitTime = 0.6f;

    private Tweener moveTween;

    protected override void Awake()
    {
        base.Awake();
        melee = transform.parent.GetComponent<MeleeWeapon>();
        bulletParry = transform.parent.transform.parent.GetComponent<BulletParry>();
        bulletParry.enabled = false;
        bioEnerge = transform.parent.transform.parent.GetComponent<BioEnerge>();

    }

    private void Start()
    {
        _chara = Owner.gameObject.GetComponent<Character>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        delayAttack = false;
    }

    protected override void OnCollideWithDamageable(Health health)
    {
        if (hitEnemy.Contains(health.gameObject))
            return;
        hitEnemy.Add(health.gameObject);

        Debug.Log($"������ ������ {_collidingCollider.gameObject}");
        if (_collidingCollider.gameObject.layer == 27)
        {
            StartCoroutine(BulletParry());
            return;
        }
        armor = _collidingCollider.GetComponent<ArmorHealthDisplay>();

        // if what we're colliding with is a CorgiController, we apply a knockback force
        _colliderCorgiController = health.GetCorgi();

        if (_colliderCorgiController && (DamageCausedKnockbackForce != Vector2.zero) && _colliderHealth && (!_colliderHealth.Invulnerable) && (_colliderHealth.gameObject.CompareTag("Boss") || !armor || (armor && !armor.ignoreKnife)))
        {
            StartCoroutine(AttackDelay());

        }
        else
        {
            Debug.Log("������ ���õ� " + "_colliderCorgiController " + _colliderCorgiController + "DamageCausedKnockbackForce " + DamageCausedKnockbackForce + "_colliderHealth.TemporaryInvulnerable " + _colliderHealth.TemporaryInvulnerable + "_colliderHealth.Invulnerable " + _colliderHealth.Invulnerable);
            StartCoroutine(melee.StopMelee());
        }

        if (delayAttack)
            StartCoroutine(melee.StopMelee());
    }

    IEnumerator BulletParry()
    {
        _chara.MovementState.ChangeState(CharacterStates.MovementStates.Parry);
        melee.TriggerWeaponParryFeedback();
        ScoreManager.Instance.score.parryCount += 1; // �и� ���� ����
        if (_health is PlayerHealth)
        {
            PlayerHealth playerHealth = _health as PlayerHealth;
            playerHealth.ParryRecoverHealth();
        }
        _health.TemporaryInvulnerable = true;
        bulletParry.enabled = true;
        bulletParry.ParryStart(parryTimescale, _collidingCollider.gameObject);
        _colliderHealth.Kill();
        yield return new WaitForSeconds(bulletParryWaitTime * parryTimescale);
        StartCoroutine(bulletParry.ParryEnd());
        yield return null;
        StartCoroutine(melee.StopMelee());

    }

    private readonly int EnemyLayer = 13;
    IEnumerator AttackDelay()
    {
        Character chara = _colliderHealth.GetCorgi().GetComponent<Character>();
        BossHealthDisplay bossHealth = _colliderHealth as BossHealthDisplay;
        HealthExpend armor = _colliderHealth as HealthExpend;

        AIBrain _brain = chara.CharacterBrain;

        AttackOnHit();
        // �齺�� �������� Ȯ��
        bool isRight = chara.IsFacingRight;
        Debug.Log("Right??" + isRight);

        if (chara.onlyBackStab)
        {
            if ((isRight && melee.Owner.transform.position.x > _collidingCollider.transform.position.x) || (!isRight && melee.Owner.transform.position.x < _collidingCollider.transform.position.x))
            {
                melee.CancelMelee();
            }
            else
            {
                chara.ArmorBreakBackstab();
                _collidingCollider.gameObject.layer = EnemyLayer;
            }
        }

        if (armor?.currentArmor <= 0 && !armor.gameObject.CompareTag("Block"))
        {
            if (_collidingCollider.gameObject.CompareTag("Bio"))
                bioEnerge.RestoreBE(bioEnerge.MaxBE / 3);
            else
                bioEnerge.RestoreBE(30 + SlashCombo.instance.comboCount * 15);
            TryDamage(armor.MaximumHealth, gameObject, InvincibilityDuration, InvincibilityDuration, Vector3.up);
            StartCoroutine(SlashFeedback());
            Slash();
        }
        else
        {

            if (_brain && _brain.CurrentState.canParry)
            {
                print($"�и�");
                chara.ParryStun();
                StartCoroutine(ParryFeedback());
                StartCoroutine(chara.UnFreezeTimer(5));
                melee.currCoolDown = 0f;
                _brain.CurrentState.SetCanParry(false);

            }
            else if (melee.isCharge)
            {
                StartCoroutine(melee.StopMelee());
                _chara.MovementState.ChangeState(CharacterStates.MovementStates.Airborning);
                Debug.Log("airborn " + CharacterStates.MovementStates.Airborning);
                yield return new WaitForSeconds(melee.airDelay);
                Debug.Log("airborn2 " + CharacterStates.MovementStates.Airborning);
                melee.ResetAirborneAbility();
                delayAttack = true;
                ApplyDamageCausedKnockback();
                TryDamage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration, Vector3.up);

            }

            if (!delayAttack)
            {
                _knockbackForce = DamageCausedKnockbackForce;
                Vector2 relativePosition = _colliderCorgiController.transform.position - Owner.transform.position;
                _knockbackForce.x = relativePosition.x > 0 ? _knockbackForce.x : -_knockbackForce.x;

                float colliderHalfX = relativePosition.x > 0 ? -_colliderHealth.GetCorgi().Bounds.x : _colliderHealth.GetCorgi().Bounds.x;
                moveTween = chara.transform.DOMove(chara.transform.position + new Vector3(_knockbackForce.x, _knockbackForce.y), 0.7f).SetEase(Ease.OutCirc).OnUpdate(() =>
                {
                    RaycastHit2D hit = Physics2D.Raycast(chara.transform.position, _knockbackForce.x > 0 ? Vector2.right : Vector2.left, Mathf.Abs(colliderHalfX), _colliderHealth.GetCorgi().PlatformMask);
                    if (hit.collider != null)
                    {
                        // Kill the tween if raycast hits another collider
                        moveTween.Kill();
                        Debug.Log($"Tween killed during movement due to raycast hit. {chara} {colliderHalfX}");
                        chara.transform.position = new Vector2(chara.transform.position.x + colliderHalfX, chara.transform.position.y);
                    }
                }).OnComplete(() =>
                {
                    DOVirtual.DelayedCall(Time.deltaTime, () =>
                    {

                        var pos = Camera.main.WorldToScreenPoint(chara.transform.position);
                        bool outOfBounds = !Screen.safeArea.Contains(pos);
                        print($"outOfBounds {outOfBounds} {pos} {chara}");
                        if (outOfBounds)
                            chara.transform.position = new Vector2(chara.transform.position.x + colliderHalfX, chara.transform.position.y);
                    });
                     
                });
                TryDamage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration, Vector3.up);
                if (_colliderHealth.ImmuneToKnockback)
                    StartCoroutine(melee.StopMelee());
            }

            IEnumerator IgnoreGravityApplyDamageCausedKnockback()
            {
                _colliderCorgiController.GravityActive(false);
                yield return null;
                ApplyDamageCausedKnockback();
                TryDamage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration, Vector3.up);
                if (_colliderHealth.ImmuneToKnockback)
                    StartCoroutine(melee.StopMelee());
                yield return new WaitForSeconds(0.2f);
                _colliderCorgiController.GravityActive(true);
            }
        }

        if (bossHealth && bossHealth.BreakBlock(DamageCaused))
        {
            StartCoroutine(SlashFeedback());
            bioEnerge.RestoreBE(bioEnerge.MaxBE / 3);
            Slash();
        }

        if (_colliderHealth.CurrentHealth <= 0)
        {
            OnKill?.Invoke();
        }
        SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
        if (_chara.MovementState.CurrentState == CharacterStates.MovementStates.Airborning)
            _chara.MovementState.ChangeState(CharacterStates.MovementStates.Idle);

        void Slash()
        {
            SlashCombo.ComboUp();
            melee._initialPosition = transform.position;
            melee.currCoolDown = 0;
        }

        void AttackOnHit()
        {

            HitDamageableFeedback?.PlayFeedbacks(this.transform.position);
            OnHitDamageable?.Invoke();

            if ((FreezeFramesOnHitDuration > 0) && (Time.timeScale > 0))
            {
                MMFreezeFrameEvent.Trigger(Mathf.Abs(FreezeFramesOnHitDuration));
            }

        }

        void TryDamage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration, Vector3 damageDirection)
        {
            if (_collidingCollider.gameObject.layer != EnemyLayer)
                return;

            _colliderHealth.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection);
        }
    }


    protected override void ApplyDamageCausedKnockback()
    {
        if (_colliderHealth.ImmuneToKnockback)
            return;

        if (_colliderCorgiController != null)
        {
            _knockbackForce.x = DamageCausedKnockbackForce.x;
            if (DamageCausedKnockbackDirection == CausedKnockbackDirections.BasedOnSpeed)
            {
                Vector2 totalVelocity = _colliderCorgiController.Speed + _velocity;
                _knockbackForce.x *= -1 * Mathf.Sign(totalVelocity.x);
            }
            if (DamageCausedKnockbackDirection == CausedKnockbackDirections.BasedOnOwnerPosition)
            {
                if (Owner == null) { Owner = this.gameObject; }
                Vector2 relativePosition = _colliderCorgiController.transform.position - Owner.transform.position;
                _knockbackForce.x *= Mathf.Sign(relativePosition.x);
            }

            _knockbackForce.y = DamageCausedKnockbackForce.y;

            if (DamageCausedKnockbackType == KnockbackStyles.SetForce)
            {
                print($"���� ���� {_knockbackForce}");
                _colliderCorgiController.SetForce(_knockbackForce);
            }
            if (DamageCausedKnockbackType == KnockbackStyles.AddForce)
            {
                _colliderCorgiController.AddForce(_knockbackForce);
            }
        }
    }
    //�Ͻ�����
    IEnumerator SlashFeedback()
    {
        melee.Owner._health.TemporaryInvulnerable = true;
        InstantiateSlashEffect(_colliderHealth.transform);
        print($"������ ���� {Quaternion.FromToRotation(Vector3.up, (Vector3)melee.dir).eulerAngles}");
        melee.TriggerWeaponSlashFeedback();
        ScoreManager.Instance.score.slashCount += 1; //������ ���� ����
        OnPauseClick_little(true);
        yield return new WaitForSeconds(slashWaitTime * Time.timeScale);
        OnPauseClick_little(false);
        melee.TriggerWeaponSlashEndFeedback();

    }

    public void InstantiateSlashEffect(Transform InstantiateTransform)
    {

        GameObject effectObj = Instantiate(melee.slashVisualEffect, InstantiateTransform, false);
        float rotAngle = Mathf.Atan2(melee.dir.y, melee.dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);
        effectObj.transform.eulerAngles = effectObj.transform.eulerAngles + rotation.eulerAngles;
        Destroy(effectObj, 3f);
    }

    IEnumerator ParryFeedback()
    {
        _chara.MovementState.ChangeState(CharacterStates.MovementStates.Parry);
        melee.TriggerWeaponParryFeedback();
        ScoreManager.Instance.score.parryCount++; // �и� ���� ����
        if (_health is PlayerHealth)
        {
            PlayerHealth playerHealth = _health as PlayerHealth;
            playerHealth.ParryRecoverHealth();
        }
        _health.TemporaryInvulnerable = true;
        OnPauseClick_little(true);
        yield return new WaitForSeconds(parryWaitTime * Time.timeScale);
        OnPauseClick_little(false);
        melee.TriggerWeaponParryEndFeedback();
        _chara.MovementState.RestorePreviousState();
    }

    public void OnPauseClick_little(bool isTrue)
    {
        if (ChatManager.Instance.chatUI.activeInHierarchy)
        {
            isPaused = true;
            Time.timeScale = 0;
            return;
        }

        isPaused = isTrue;
        Time.timeScale = (isPaused) ? 0.001f : 1.0f;
    }

    public void EndDelayAttack()
    {
        delayAttack = false;
    }
}