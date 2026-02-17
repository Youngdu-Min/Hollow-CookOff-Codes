using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class UnityEventString : UnityEvent<string>
{

}

public class BossHealthDisplay : Health
{
    int CurrHP
    {
        get
        {
            return CurrentHealth;
        }
        set
        {
            if (value > MaximumHealth)
                value = MaximumHealth;
            CurrentHealth = value;
            Debug.Log($"체력 {CurrentHealth} {divideValue * (divideCount - 1)} {MaximumHealth} {divideValue} {divideCount}");

            if (minimalHp >= CurrentHealth && !isBlockKilled)
            {
                Debug.Log("최소 체력");
                CurrentHealth = minimalHp;
                LaunchBlock();
            }
            else if (!isNonDivideHealth && divideValue * (divideCount - 1) > CurrentHealth)
            {
                CurrentHealth = divideValue * (divideCount - 1);
                LaunchBlock();
                Debug.Log("체력 고정");
            }

            RefreshHealthFill();

            //-------------------------------------------
            void LaunchBlock()
            {
                divideCount--;
                isBlocked = true;
                if (waitBlockState)
                    StartCoroutine(WaitBlockActive());
                else
                    BlockActive();
            }

            void BlockActive()
            {
                print($"블록 활성화 {isBlocked} | {gameObject}");
                if (!brain.BrainActive)
                {
                    brain.BrainActive = true;
                    brain.FreezeBrain();
                    brain.BrainActive = false;
                }
                else
                    brain.FreezeBrain();
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);
                slashHintUI?.gameObject.SetActive(true);
                blockEnter.Invoke();
            }

            IEnumerator WaitBlockActive()
            {
                yield return new WaitUntil(() => WaitBlockNameCheck());
                BlockActive();
            }

            bool WaitBlockNameCheck()
            {
                for (int i = 0; i < waitBlockStateNames.Length; i++)
                {
                    if (waitBlockStateNames[i] == brain.CurrentState.StateName)
                        return false;
                }
                return true;
            }
            //-------------------------------------------
        }
    }

    int minimalHp = 1;

    bool isBlocked;
    bool isBlockKilled;
    [Range(1, 3)]
    int currPhase = 1;
    public int CurrPhase // 1 ▰▰▰, 2 ▰▰▱, 3 ▰▱▱
    {
        get => currPhase;
        private set => currPhase = value;
    }

    float waitTime;

    [SerializeField]
    bool isNonDivideHealth;
    [SerializeField]
    int divideCount;
    int divideValue;

    [SerializeField]
    Image[] hpUI;

    [SerializeField]
    bool bossDeathChat;
    [SerializeField]
    string deathChatId;
    [SerializeField]
    bool bossPhaseChat;
    [SerializeField]
    string phaseChatId;
    [SerializeField]
    bool checkBossPhaseChat;
    [SerializeField, Range(1, 3)]
    int chatPhase; // 1 ▰▰▰, 2 ▰▰▱, 3 ▰▱▱
    [SerializeField]
    bool bossPhaseSpawn;
    [SerializeField]
    CamLock camlock;
    bool waitClearEnemies;


    AIBrain brain;

    [SerializeField]
    string nextPhaseState;

    [SerializeField]
    string recoveryState;
    [SerializeField]
    private bool waitBlockState;
    [SerializeField]
    private string[] waitBlockStateNames;

    [SerializeField]
    private UnityEvent blockEnter;
    [SerializeField]
    private UnityEvent blockExit;
    [SerializeField]
    private UnityEvent phaseEvent;
    [SerializeField]
    private float deathEventDelay;
    [SerializeField]
    private UnityEvent deathEvent;
    [SerializeField]
    private UnityEventString deathStringEvent;
    [SerializeField]
    private string deathStringEventParam;

    [SerializeField]
    private TextMeshPro slashHintUI;
    [SerializeField]
    private float flipOffset;
    private Vector2 originSlashUIPos;

    public void StartPhaseChat()
    {
        if (bossPhaseChat && CurrPhase == chatPhase && !checkBossPhaseChat)
        {
            ChatManager.Instance.StartChat(phaseChatId);
            checkBossPhaseChat = true;

            if (bossPhaseSpawn)
            {
                camlock.ManualAbleSpawning();
                waitClearEnemies = true;
                brain.BrainActive = false;
            }
        }

    }

    void StartDeathChat()
    {
        if (bossDeathChat)
        {
            ChatManager.Instance.StartChat(deathChatId);
            //TextAsset tmpAsset = ChatManager.chmana.chatDB.FindDialogueByName(deathChatId);
            //JsonMaker.Instance.ChangeGtxt(tmpAsset);
            //ChatManager.chmana.CallNewStart();
            Kill();
        }
        else
        {
            Kill();
        }
    }

    // Start is called before the first frame update
    protected override void Initialization()
    {
        base.Initialization();
        divideValue = MaximumHealth / divideCount;
        RefreshHealthFill();

        brain = GetComponentInChildren<AIBrain>();
        originSlashUIPos = slashHintUI.transform.localPosition;
    }

    /// <summary>
    /// Called when the object takes damage
    /// </summary>
    /// <param name="damage">The amount of health points that will get lost.</param>
    /// <param name="instigator">The object that caused the damage.</param>
    /// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
    /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
    public override void Damage(int damage, GameObject instigator, float flickerDuration,
        float invincibilityDuration, Vector3 damageDirection)
    {
        Debug.Log("데미지 " + damage + " " + instigator + " " + invincibilityDuration);
        if (damage <= 0)
        {
            Debug.Log("데미지 마이너스 " + instigator);
            OnHitZero?.Invoke();
            return;
        }

        // if the object is invulnerable, we do nothing and exit
        if (TemporaryInvulnerable || (Invulnerable && !instigator.CompareTag("Wall")))
        {
            Debug.Log("데미지 무적! " + instigator);
            OnHitZero?.Invoke();
            return;
        }
        else if (Invulnerable && instigator.CompareTag("Wall"))
        {
            Debug.Log("데미지 벽딜 " + instigator);
            _character.UnFreeze();
        }

        // if we're already below zero, we do nothing and exit
        if ((CurrentHealth <= 0) && (InitialHealth != 0))
        {
            Debug.Log("데미지 안줘도 체력 0 " + instigator);
            return;
        }

        if (isBlocked) return;

        // we decrease the character's health by the damage
        float previousHealth = CurrentHealth;
        CurrHP -= damage;

        Debug.Log("데미지 남은 체력 " + CurrentHealth + " " + instigator);

        LastDamage = damage;
        LastDamageDirection = damageDirection;
        lastInstigator = instigator;

        relativeX = this.gameObject.transform.position.x - instigator.transform.position.x > 0 ? 1 : -1;
        Debug.Log("X " + relativeX);
        OnHit?.Invoke();

        if (CurrentHealth < 0)
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
        if (CurrentHealth <= 0)
        {
            // we set its health to zero (useful for the healthbar)
            CurrentHealth = 0;
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

    void Update()
    {
        if (isBlocked)
        {
            Debug.Log("체력 회복 중");
            waitTime += Time.deltaTime;
            if (waitTime >= 10)
            {
                Debug.Log("체력 회복 완료");
                CurrHP += divideValue / 10;
                waitTime = 0;
                divideCount++;
                isBlocked = false;
                brain.TransitionToState(recoveryState);
                _character.UnFreeze();
                slashHintUI?.gameObject.SetActive(false);
                blockExit.Invoke();
            }
        }

        if (waitClearEnemies && camlock.clear)
        {
            Debug.Log("청소끝 리셋");
            brain.BrainActive = true;
            brain.ResetBrain();
            Invulnerable = false;
            waitClearEnemies = false;
        }

        slashHintUI.transform.localPosition = _character.IsFacingRight ? originSlashUIPos : originSlashUIPos + new Vector2(flipOffset, 0);
    }

    public bool IsCurrPhase(int phase)
    {
        return CurrPhase == phase;
    }

    public bool BreakBlock(int breakDamage)
    {
        if (!isBlocked)
            return false;

        slashHintUI?.gameObject.SetActive(false);
        CurrPhase++;
        isBlocked = false;
        waitTime = 0;
        print($"BreakBlock / phase{CurrPhase} / currHp {CurrHP} / breakDamage {breakDamage}");
        if (CurrHP - breakDamage <= 0)
        {
            isBlockKilled = true;
            StartDeathChat();
            return true;
        }

        phaseEvent.Invoke();
        _character.UnFreeze();
        //Damage(breakDamage, this.gameObject, 0, 0, Vector3.zero);
        if (nextPhaseState != null && nextPhaseState != "")
        {
            brain.TransitionToState(nextPhaseState);
        }
        else if (!waitClearEnemies)
        {
            brain.ResetBrain();
        }
        //CurrentHealth -= breakDamage;

        return true;
    }

    private void RefreshHealthFill()
    {
        if (hpUI.Length == 0)
            return;

        if (isNonDivideHealth)
        {
            for (int i = 0; i < hpUI.Length; i++)
                hpUI[i].fillAmount = (float)CurrentHealth / MaximumHealth;
            return;
        }
        int calculHealth = CurrentHealth;
        for (int i = 0; i < hpUI.Length; i++)
        {
            if (calculHealth >= divideValue)
            {
                calculHealth -= divideValue;
                hpUI[i].fillAmount = 1;
            }
            else
            {
                hpUI[i].fillAmount = (float)calculHealth / (float)divideValue;
                calculHealth = 0;
            }

        }
    }

    public bool Chatted()
    {
        if (bossPhaseChat && CurrPhase == chatPhase && !checkBossPhaseChat)
            return true;
        else
            return false;
    }

    protected override void OnEnable()
    {
        base.OnDisable();
    }

    public override void Kill()
    {
        StartCoroutine(WaitDeathEvent());
        base.Kill();
        for (int i = 0; i < hpUI.Length; i++)
            hpUI[i].fillAmount = 0;

        gameObject.layer = LayerMask.NameToLayer("NoCollision");
    }

    private IEnumerator WaitDeathEvent()
    {
        yield return new WaitUntil(() => !ChatManager.Instance.chatUI.activeInHierarchy);
        yield return new WaitForSeconds(deathEventDelay);
        deathEvent.Invoke();
        deathStringEvent.Invoke(deathStringEventParam);
    }

}