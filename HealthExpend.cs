using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthExpend : Health
{
    public int currentArmor;
    public int initialArmor;
    public int maxArmor;

    [SerializeField]
    private MMFeedbacks armorBreakFeedback;
    public bool armorBreakAction;
    public AIActionSwapBrain swapBrain;
    public AIActionSpawnCharater spawnCharater;
    public CharacterAbility[] lastAbility;

    public bool stunOnDied;
    private LayerMask initLayer;

    public bool AlreadyDead()
    {
        if (currentArmor + CurrentHealth < stackedDamage)
        {
            return true;
        }
        return false;
    }
    private bool reaper = false;

    private int stackedDamage = 0;

    public bool dodgeBullet;
    [SerializeField]
    private float dodgeAmt = 10;

    public bool onlyArmor;

    public bool hasNonArmor;
    [SerializeField]
    private HealthExpend separateCore;
    [SerializeField]
    private MMFeedbacks reviveFeedbacks;

    private struct DelayedDamage
    {
        public int damage;
        public GameObject instigator;
        public float flickerDuration;
        public float invincibilityDuration;

        public DelayedDamage(int _damage, GameObject _instigator, float _flickerDuration, float _invincibilityDuration)
        {
            damage = _damage;
            instigator = _instigator;
            flickerDuration = _flickerDuration;
            invincibilityDuration = _invincibilityDuration;
        }
    }


    private Queue<DelayedDamage> delayedDamages = new Queue<DelayedDamage>();

    protected override void Start()
    {
        base.Start();
        initLayer = gameObject.layer;
    }


    public bool HasArmor()
    {
        if (currentArmor > 0)
        {
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (Time.timeScale != 0 && delayedDamages.Count != 0 &&
            (_character == null || (_character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Paused && _character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Frozen)))
        {
            reaper = true;
            var _damage = delayedDamages.Dequeue();
            Damage(_damage.damage, _damage.instigator, _damage.flickerDuration, 0, Vector3.up);
            stackedDamage -= _damage.damage;
            if (delayedDamages.Count == 0)
            {
                stackedDamage = 0;
            }
        }
    }

    /// <summary>
    /// Grabs useful components, enables damage and gets the inital color
    /// </summary>
    protected override void Initialization()
    {
        _character = this.gameObject.GetComponent<Character>();
        _characterPersistence = this.gameObject.GetComponent<CharacterPersistence>();

        if (this.gameObject.MMGetComponentNoAlloc<SpriteRenderer>() != null)
        {
            _renderer = this.gameObject.GetComponent<SpriteRenderer>();
        }

        if (_character != null)
        {
            if (_character.CharacterModel != null)
            {
                if (_character.CharacterModel.GetComponentInChildren<Renderer>() != null)
                {
                    _renderer = _character.CharacterModel.GetComponentInChildren<Renderer>();
                }
            }
        }

        // we grab our animator
        if (_character != null)
        {
            if (_character.CharacterAnimator != null)
            {
                _animator = _character.CharacterAnimator;
            }
            else
            {
                _animator = this.gameObject.GetComponent<Animator>();
            }
        }
        else
        {
            _animator = this.gameObject.GetComponent<Animator>();
        }

        if (_animator != null)
        {
            _animator.logWarnings = false;
        }


        _autoRespawn = this.gameObject.GetComponent<AutoRespawn>();
        _controller = this.gameObject.GetComponent<CorgiController>();
        if (!_controller)
            _controller = manualCorgi;
        _healthBar = this.gameObject.GetComponent<MMHealthBar>();
        _collider2D = this.gameObject.GetComponent<Collider2D>();

        StoreInitialPosition();
        if (gameObject.active)
        {
            StartCoroutine(DataSync());
        }
        _initialized = true;
        initDamage?.Invoke();
        DamageEnabled();

    }

    IEnumerator DataSync()
    {
        yield return null;
        for (int i = 0; i < EnemyBalance.Data.DataList.Count; i++)
        {
            if (_character.CharacterBrain?._aiType.ToString() == EnemyBalance.Data.DataList[i].strValue)
            {
                initialArmor = hasNonArmor ? 0 : EnemyBalance.Data.DataList[i].armor;
                maxArmor = initialArmor;
                InitialHealth = EnemyBalance.Data.DataList[i].hp;
                MaximumHealth = InitialHealth;
                break;
            }
        }


        currentArmor = initialArmor;
        if (initialArmor > maxArmor)
        {
            maxArmor = initialArmor;
        }


        CurrentHealth = InitialHealth;
        initDamage?.Invoke();
        UpdateHealthBar(false);
    }

    protected override void OnEnable()
    {
        if ((_characterPersistence != null) && (_characterPersistence.Initialized))
        {
            UpdateHealthBar(false);
            return;
        }

        CurrentHealth = InitialHealth;
        currentArmor = initialArmor;
        DamageEnabled();
        UpdateHealthBar(false);
    }

    public void RestoreArmor(int amount) //아머회복
    {
        currentArmor = Mathf.Min(maxArmor, currentArmor + amount);
    }

    public IEnumerator RemoveArmor()
    {
        yield return new WaitUntil(() => currentArmor > 0);
        Damage(currentArmor, gameObject, 0, 0, Vector3.zero);
    }

    /// <summary>
    /// Called when the object takes damage
    /// </summary>
    /// <param name="damage">The amount of health points that will get lost.</param>
    /// <param name="instigator">The object that caused the damage.</param>
    /// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
    /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
    public override void Damage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration, Vector3 damageDirection)
    {
        if (_character != null
            && (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Paused ||
            _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen))
        {
            if (!reaper)
            {
                //데미지 누적 및 지연피해
                delayedDamages.Enqueue(new DelayedDamage(damage, instigator, flickerDuration, invincibilityDuration));
                stackedDamage += damage;
                if (AlreadyDead())
                {
                    gameObject.layer = 21;
                }
                return;
            }
        }
        if (damage <= 0)
        {
            OnHitZero?.Invoke();
            return;
        }

        // if the object is invulnerable, we do nothing and exit
        if (TemporaryInvulnerable || Invulnerable || (_character.onlyBackStab && gameObject.layer == 29 && !instigator.CompareTag("Bio")))
        {
            OnHitZero?.Invoke();
            return;
        }

        // if we're already below zero, we do nothing and exit
        if ((CurrentHealth <= 0) && (InitialHealth != 0))
        {
            return;
        }

        if (dodgeBullet && instigator.layer == 16 && !instigator.CompareTag("isParry") && _character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Stunned)
        {
            Dodge(instigator);
            return;
        }

        if (separateCore != null && instigator.CompareTag("Bio"))
            separateCore.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection);


        LastDamage = damage;
        lastInstigator = instigator;

        if (currentArmor > 0) //아머가 있을경우
        {
            if (currentArmor > damage) //아머가 데미지보다 많으면
            {
                currentArmor -= damage;
                damage = 0;
            }
            else
            {
                damage -= currentArmor;
                currentArmor = 0;
                armorBreakFeedback?.PlayFeedbacks();
                if (armorBreakAction)
                {
                    swapBrain?.PerformAction();
                    if (spawnCharater != null)
                        StartCoroutine(spawnCharater.DamageSpawnObj(damage, instigator, flickerDuration, invincibilityDuration, damageDirection));

                    for (int i = 0; i < lastAbility.Length; i++)
                        lastAbility[i].enabled = false;
                }
            }
        }

        // we decrease the character's health by the damage
        float previousHealth = CurrentHealth;
        CurrentHealth -= damage;

        LastDamageDirection = damageDirection;
        relativeX = this.gameObject.transform.position.x - instigator.transform.position.x > 0 ? 1 : -1;
        OnHit?.Invoke();

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
        }

        // we prevent the character from colliding with Projectiles, Player and Enemies
        if (invincibilityDuration > 0)
        {
            DamageDisabled();
            StartCoroutine(DamageEnabled(invincibilityDuration));
        }

        // we trigger a damage taken event
        MMDamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

        if (_animator != null)
        {
            _animator.SetTrigger("Damage");
        }

        // we play the damage feedback
        DamageFeedbacks?.PlayFeedbacks();

        if (FlickerSpriteOnHit)
        {
            // We make the character's sprite flicker
            if (_renderer != null)
            {
                StartCoroutine(MMImage.Flicker(_renderer, _initialColor, FlickerColor, 0.05f, flickerDuration));
            }
        }


        // we update the health bar
        UpdateHealthBar(true);

        // if health has reached zero
        if ((onlyArmor && currentArmor <= 0) || (!onlyArmor && CurrentHealth <= 0))
        {
            // we set its health to zero (useful for the healthbar)
            CurrentHealth = 0;
            currentArmor = 0;

            if (_character != null)
            {
                if (_character.CharacterType == Character.CharacterTypes.Player)
                {
                    LevelManager.Instance.KillPlayer(_character);
                    return;
                }
            }

            Kill();

        }

    }

    public override void Kill()
    {
        if (_character.onlyBackStab)
            _character.ArmorBreakBackstab();

        if (stunOnDied)
        {

            _collider2D.enabled = false;
            Character chara = transform.parent.gameObject.GetComponent<Character>();
            chara.Stun();
            StartCoroutine(chara.UnFreezeTimer(HollowBalance.action.actionList[20].floatValue));
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("NoCollision");
            base.Kill();

        }

        if (CamLock.camLock == null || (spawnCharater != null && spawnCharater.IsKillSelf) || separateCore)
            return;


        CamLock.camLock.ableCount -= 1;
        CamLock.camLock.AbleSpawning();
    }
    public override void Revive()
    {
        base.Revive();
        if (MainCharacter.instance.gameObject == this.gameObject)
        {
            CurrentWeaponUI.ResetUI();
        }
        reviveFeedbacks?.PlayFeedbacks();
        gameObject.layer = initLayer;
        separateCore?.Revive();
    }

    private void Dodge(GameObject target)
    {
        if (this.transform.position.x < target.transform.position.x)
            this.transform.position = new Vector2(this.transform.position.x + dodgeAmt, this.transform.position.y);
        else
            this.transform.position = new Vector2(this.transform.position.x - dodgeAmt, this.transform.position.y);

    }
}
