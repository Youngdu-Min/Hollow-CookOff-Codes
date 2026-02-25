using MoreMountains.CorgiEngine;
using System.Collections;
using UnityEngine;

public enum SubWeaponType
{
    None,
    Stim = 4,
    Grapple = 5,
    Cloak = 6,
    PauseGrenade = 7,
}

public class SubWeapon : MonoBehaviour
{
    public static SubWeapon Instance;

    //플레이어가 가진 보조장비들을 발동시키는 스크립트
    public static SubWeaponType currentSubWp = SubWeaponType.None;


    private bool usingCape = false;
    [SerializeField] private float curStimCoolDown;
    [SerializeField] private float curStimDuration;

    public GameObject Cloak;

    private float lastCloakActivated;
    [SerializeField]
    private int lastCloakHp = 100;
    private Health cloakHP;
    [SerializeField] float capeDivideSpeed = 2;
    CharacterHorizontalMovement horizontalMovement;

    public GameObject pauseGrenade;
    public bool grenadeActive = false;
    private GameObject _grenade;
    [SerializeField] private float curGrenadeTime = 0f;
    [SerializeField] private GrapplingRope grapplingRope;

    private Camera _camera;

    private Character character;
    private PlayerHealth characterHealth;
    private Lookat lookat;
    private bool isUsed;

    public float Percentage(int index)
    {
        switch (index)
        {
            case 4: //스팀
                return Mathf.Max(curStimDuration / GSManager.Stim.duration, curStimCoolDown / GSManager.Stim.coolTime);
            case 5: //밧줄
                return 1f;
            case 6: //망토
                return cloakHP.CurrentHealth * 0.01f;
            case 7: //폭탄
                return curGrenadeTime / GSManager.Grenade.coolTime;
            default:
                return 0;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        curStimCoolDown = GSManager.Stim.coolTime;

        Cloak = Instantiate(Cloak, this.transform);
        Cloak.SetActive(false);
        Cloak.transform.localPosition = Vector3.zero;
        Cloak.name = "cloak";
        cloakHP = Cloak.GetComponent<Health>();
        cloakHP.ResetHealthToMaxHealth();
        cloakHP.SetBlockInitHealthOnEnable(true);
        cloakHP.OnDeath += () =>
        {
            character.Stun();
            StartCoroutine(character.UnFreezeTimer(5));

        };

        horizontalMovement = GetComponent<CharacterHorizontalMovement>();
        curGrenadeTime = GSManager.Grenade.coolTime;

        _camera = Camera.main;
        character = GetComponent<Character>();
        characterHealth = GetComponent<PlayerHealth>();
        lookat = GetComponentInChildren<Lookat>();
        character.onFreeze += () => lookat.enabled = false;
        character.onUnFreeze += () => lookat.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse1) && !Input.GetMouseButton(0))
            UseSubWeapon();
        if (isUsed && !Input.GetKey(KeyCode.Mouse1) && character.FlipModelOnDirectionChange)
            EndSubWeapon();
    }

    public void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;

        //스팀 관련
        if (curStimDuration > 0)
        {
            curStimDuration -= Time.fixedDeltaTime;
            if (curStimDuration <= 0) //스팀 끝났으면 쿨돌기 시작
            {
                BGMManager.Instance.SetSoundSpeedNomal();
                EndStim();
            }
        }
        else if (curStimCoolDown < GSManager.Stim.coolTime)
            curStimCoolDown += Time.fixedDeltaTime;

        if (Cloak.activeSelf)
            lastCloakActivated = Time.time;
        lastCloakHp = cloakHP.CurrentHealth;

        if (!grenadeActive && curGrenadeTime < GSManager.Grenade.coolTime)
            curGrenadeTime += Time.fixedDeltaTime;
    }


    public void UseSubWeapon()
    {
        if (Time.timeScale == 0)
            return;

        if (character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen || character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Stunned)
            return;

        switch (currentSubWp)
        {
            case SubWeaponType.Stim:
                UseStim();
                break;
            case SubWeaponType.Grapple:
                break;
            case SubWeaponType.Cloak:
                UseCloak();
                break;
            case SubWeaponType.PauseGrenade:
                UseGrenade();
                break;
            default:
                break;
        }
        isUsed = true;
    }

    public void EndSubWeapon()
    {
        isUsed = false;
    }

    //1번 스팀팩
    public void UseStim()
    {
        if (!SaveDataManager.Instance.IsEnable(WeaponType.Stim))
            return;

        if (curStimDuration > 0) //재사용 가능할 때
            EndStim();
        else if (curStimCoolDown >= GSManager.Stim.coolTime)//사용가능할때
        {
            BGMManager.Instance.SetSoundSpeed();
            curStimCoolDown = 0;
            curStimDuration = GSManager.Stim.duration;
            Time.timeScale = GSManager.Stim.timeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

    }
    public void EndStim()
    {
        BGMManager.Instance.SetSoundSpeedNomal();
        curStimDuration = 0;
        Time.timeScale = 1;
        curStimCoolDown = 0;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void UseCloak()
    {
        if (usingCape)
        {
            EndCloak();
            return;
        }
        usingCape = true;
        lookat.enabled = false;
        character.onUnFreeze += () =>
        {
            if (usingCape)
                character.TryChangeAimControls(WeaponAim.AimControls.Off);
        };
        float _timePassed = Mathf.Max(0, Time.time - lastCloakActivated - GSManager.Cloak.regenCool);

        if (lastCloakHp + _timePassed * GSManager.Cloak.hpRegen > 0)
        {
            cloakHP.SetHealth(lastCloakHp + (int)(_timePassed * GSManager.Cloak.hpRegen), this.gameObject);
            Cloak.SetActive(true);
            characterHealth.OnHit += EndCloak;
            horizontalMovement.MovementSpeed /= capeDivideSpeed;
            character.MovementState.ChangeState(CharacterStates.MovementStates.Blocking);
        }
    }

    public void EndCloak()
    {
        usingCape = false;
        lookat.enabled = true;
        Cloak.GetComponent<Collider2D>().enabled = true;
        Cloak.SetActive(false);
        horizontalMovement.MovementSpeed *= capeDivideSpeed;
        character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
        character.onUnFreeze -= () =>
        {
            if (usingCape)
                character.TryChangeAimControls(WeaponAim.AimControls.Off);
        };
        lastCloakActivated = Time.time;
        lastCloakHp = cloakHP.CurrentHealth;
        characterHealth.OnHit -= EndCloak;
    }

    public bool IsUsingCape()
        => usingCape;

    public IEnumerator CloakHeal()
    {
        yield return new WaitForSeconds(1);
    }
    //4번 퍼즈 그레네이드 
    public void UseGrenade()
    {
        if (curGrenadeTime < GSManager.Grenade.coolTime)//아직 쿨 안돌았으면
            return;

        if (usingCape)
            return;

        Vector2 pos, mouse;
        pos = transform.position;
        mouse = _camera.ScreenToWorldPoint(Input.mousePosition);

        float angle = Mathf.Atan2(mouse.y - pos.y, mouse.x - pos.x) * Mathf.Rad2Deg;
        //수류탄 던지는 코드
        if (_grenade == null)
            _grenade = Instantiate(pauseGrenade, transform.position + (Vector3)(mouse - pos).normalized * GSManager.Grenade.projectileRadius, Quaternion.AngleAxis(angle, Vector3.forward));
        else
        {
            _grenade.transform.SetPositionAndRotation(transform.position + (Vector3)(mouse - pos).normalized * GSManager.Grenade.projectileRadius, Quaternion.AngleAxis(angle, Vector3.forward));
            _grenade.SetActive(true);
        }

        grenadeActive = true;
        curGrenadeTime = 0;
    }

    public float RefreshLeftTimeImage()
    {
        float fillValue = 0f;
        switch (currentSubWp)
        {
            case SubWeaponType.Stim:
                fillValue = 1 - curStimCoolDown / GSManager.Stim.coolTime;
                break;
            case SubWeaponType.Grapple:
                fillValue = grapplingRope.isGrappling ? 1 : 0;
                break;
            case SubWeaponType.Cloak:
                if (usingCape)
                    fillValue = 1;
                else
                {
                    float _timePassed = Mathf.Max(0, Time.time - lastCloakActivated - GSManager.Cloak.regenCool);
                    fillValue = lastCloakHp + _timePassed * GSManager.Cloak.hpRegen > 0 ? 0 : 1;
                }
                break;
            case SubWeaponType.PauseGrenade:
                fillValue = 1 - curGrenadeTime / GSManager.Grenade.coolTime;
                break;
            default:
                break;
        }

        return fillValue;
    }

    public float ClockHealthRatio()
    {
        float _timePassed = Mathf.Max(0, Time.time - lastCloakActivated - GSManager.Cloak.regenCool);
        return (float)(lastCloakHp + _timePassed * GSManager.Cloak.hpRegen) / cloakHP.MaximumHealth;
    }
}
