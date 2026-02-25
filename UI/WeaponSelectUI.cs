using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MoreMountains.CorgiEngine.CharacterStates;


public class WeaponSelectUI : MonoBehaviour
{
    private static WeaponSelectUI instance;
    public static WeaponSelectUI Instance()
    {
        if (instance == null && StageSelectUI.Instance && !StageSelectUI.Instance.IsCutscene())
        {
            new GameObject("WeaponSelectUI").AddComponent<WeaponSelectUI>();
        }
        return instance;
    }

    public GameObject player;

    public Color baseColor = new Color(1, 1, 1, 1);
    public Color redHover, redSelected;
    public Color blueHover, blueSelected;

    //각도계산용
    private Vector2 normalisedMousePoition;
    private float currentAngle;
    private float tableRadius = 400;

    [SerializeField] private int currentHover = -1;
    public int selectedMainWp = 0, selectedSubWp = 4;
    private bool isMouseSelect;

    public List<GameObject> weapons = new List<GameObject>();
    [SerializeField] private GameObject behemoth;

    public List<Sprite> weaponSprites = new List<Sprite>();

    [SerializeField] private List<GameObject> bullets = new List<GameObject>();
    [SerializeField] private List<Image> pizzaImage = new List<Image>();
    [SerializeField] private GameObject[] blockImages;

    private CharacterHandleWeapon mainWeapon;
    private SubWeapon subWeapon;
    private Character chara;
    private SpecialAbility specialAbility;
    [SerializeField] private GameObject display;
    private int currentMainWeapon = -1;

    private GrapplingGun grapple;
    [SerializeField] private MMFeedbacks weaponSelectFeedbacks;
    [SerializeField] private MMFeedbacks behemothSelectFeedbacks;
    private bool isActivate;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initiate();
            return;
        }

        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    void Initiate()
    {
        instance = this;
        LoadTable();
    }

    // Start is called before the first frame update
    void Start()
    {
        Select(0);
        int? index = SaveDataManager.Instance.GetEnableSubWeaponIndex();
        if (index != null)
        {
            Select((int)index);
            CurrentWeaponUI.instance.ChangeSubWeapon((int)index);
        }
        else
            CurrentWeaponUI.instance.SetActiveSubWeaponImage(false);
        SetPlayer(MainCharacter.instance.gameObject);
        grapple = player.GetComponentInChildren<GrapplingGun>(true);
        grapple.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        CurrentWeaponUI.instance.RefreshLeftTime(SubWeapon.Instance.RefreshLeftTimeImage());
        if (SubWeapon.currentSubWp == SubWeaponType.Cloak)
            CurrentWeaponUI.instance.ChangeSubWeaponColor(Color.Lerp(Color.red, Color.white, (float)SubWeapon.Instance.ClockHealthRatio()));

        if (!isActivate)
            return;

        if (chara.ConditionState.CurrentState == CharacterConditions.Stunned || chara.ConditionState.CurrentState == CharacterConditions.Frozen)
            return;

        if (subWeapon.IsUsingCape())
            return;

        if (specialAbility.IsSpinning)
            return;

        normalisedMousePoition = new Vector2(Input.mousePosition.x - Screen.width / 2, -Input.mousePosition.y + Screen.height / 2);
        if (display.activeSelf && normalisedMousePoition.sqrMagnitude < tableRadius * tableRadius)  //무기창 켜져있고 마우스가 그위에있을때
        {
            Debug.Log("마우스호버");
            //각도 계산
            int tmpHover;
            currentAngle = Mathf.Atan2(normalisedMousePoition.y, normalisedMousePoition.x) * Mathf.Rad2Deg;
            currentAngle = (currentAngle + 540) % 360;
            tmpHover = (int)currentAngle / 45;

            if (tmpHover != currentHover) //호버 바꼈으면
            {
                UnHover(currentHover);
                Hover(tmpHover);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                isMouseSelect = true;
                Select(tmpHover);
            }
        }

        //normalisedMousePoition.sqrMagnitude 

        if (Input.GetKeyDown(KeyCode.R))    //베히모스 장착
        {
            if (!SaveDataManager.Instance.IsEnable(WeaponType.Behemoth))
                return;

            if (behemoth == null)
            {
                Debug.Log("Gameobject Behemoth is not set.");
                return;
            }

            if (chara.ConditionState.CurrentState != CharacterStates.CharacterConditions.Normal)
                return;

            if (BEUI.IsBehemoth)
            {
                mainWeapon.ChangeWeapon(weapons[selectedMainWp].GetComponent<Weapon>(), "1", false);
                BEUI.InactiveBehemoth();
                CurrentWeaponUI.ChangeMainWeapon(selectedMainWp);
                CurrentWeaponUI.instance.SetOriginMainWpImage();
                RefreshOverHeatUI(selectedMainWp);
            }
            else
            {
                mainWeapon.ChangeWeapon(behemoth.GetComponent<BEhemoth>(), "1", false);
                BEUI.ActiveBehemoth();
                behemothSelectFeedbacks?.PlayFeedbacks();
                CurrentWeaponUI.instance.mainWpImage.sprite = weaponSprites[8];
                CurrentWeaponUI.instance.mainWpImage.SetNativeSize();
                RefreshOverHeatUI(4); 
            }
            //슬래시,패리 비활성화
        }

        int? inputNumber = GetInputNumber();

        if (inputNumber != null)
        {
            Select((int)inputNumber);
            if (inputNumber <= 3)
                FinalSelectMainWeapon((int)inputNumber);
            else
                FinalSelectSubWeapon((int)inputNumber);
        }

        if (!SaveDataManager.Instance.IsEnableInventory())
            return;

        if (Input.GetKeyDown(KeyCode.Q)) //q누르면 창 나옴
        {
            display.SetActive(true);
            RefreshBlockImage();
            mainWeapon.enabled = false;
            PauseManager.Instance().PauseCall(this, true);
            Select(selectedMainWp);
            Select(selectedSubWp);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            FinalSelectMainWeapon(selectedMainWp);
            FinalSelectSubWeapon(selectedSubWp);
            display.SetActive(false);
            mainWeapon.AbilityPermitted = true;
            mainWeapon.enabled = true;
            PauseManager.Instance().PauseCall(this, false);
        }

        int? GetInputNumber()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                return 0;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                return 1;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                return 2;
            if (Input.GetKeyDown(KeyCode.Alpha4))
                return 3;
            if (Input.GetKeyDown(KeyCode.Alpha5))
                return 4;
            if (Input.GetKeyDown(KeyCode.Alpha6))
                return 5;
            if (Input.GetKeyDown(KeyCode.Alpha7))
                return 6;
            if (Input.GetKeyDown(KeyCode.Alpha8))
                return 7;
            return null;
        }
    }
    public void LoadTable()
    {
        //구글시트의 데이터테이블대로 무기 스탯 조정
        for (int i = 0; i < weapons.Count; i++)
        {
            var baseData = WeaponTable.BaseData.BaseDataList[i];
            var _weapon = weapons[i].GetComponent<ProjectileWeapon>();

            _weapon.TriggerMode = (Weapon.TriggerModes)baseData.autoFire;
            _weapon.TimeBetweenUses = baseData.useTime;
            _weapon.ProjectilesPerShot = baseData.ProjectilePerShot;
            _weapon.Spread = new Vector3(0, 0, baseData.zSpread);
            _weapon.RandomSpread = (baseData.randomSpread == 1) ? true : false;

            if ((WeaponType)i == WeaponType.Shotgun)
            {
                _weapon.ApplyRecoilOnUse = true;
                _weapon.RecoilOnUseProperties.RecoilForceAirborne = (float)GSManager.Ability.stuntFlying;
                _weapon.RecoilOnUseProperties.RecoilStyle = DamageOnTouch.KnockbackStyles.StuntFlying;
            }

            if ((WeaponType)i == WeaponType.Rifle)
            {
                _weapon.ApplyRecoilOnUse = true;
                _weapon.RecoilOnUseProperties.RecoilForceAirborne = GSManager.Ability.playerTriggerHappy;
                _weapon.RecoilOnUseProperties.RecoilStyle = DamageOnTouch.KnockbackStyles.Vertical;

            }
        }
        for (int i = 0; i < bullets.Count; i++)
        {
            var baseData = WeaponTable.BaseData.BaseDataList[i];
            var _bullet = bullets[i];
            _bullet.GetComponent<Projectile>().Speed = baseData.Velocity;
        }
    }


    public void Hover(int _num)
    {
        if (!SaveDataManager.Instance.IsEnable((WeaponType)_num))
            return;

        if (_num <= 3)
        {
            if (_num != selectedMainWp)
                pizzaImage[_num].color = redHover;
        }
        else
        {
            if (_num != selectedSubWp)
                pizzaImage[_num].color = blueHover;
        }
        currentHover = _num;
    }

    public void UnHover(int _num)
    {
        if (!SaveDataManager.Instance.IsEnable((WeaponType)_num))
            return;

        if (_num < 0)
            return;
        if (_num == selectedMainWp)
            pizzaImage[_num].color = redSelected;
        else if (_num == selectedSubWp)
            pizzaImage[_num].color = blueSelected;
        else
            pizzaImage[_num].color = baseColor;
    }

    private void RefreshBlockImage()
    {
        for (int i = 0; i < blockImages.Length; i++)
            blockImages[i].SetActive(!SaveDataManager.Instance.IsEnable((WeaponType)i));
    }

    public void Select(int _num)
    {
        //해금 안되있으면 패스
        if (SaveDataManager.Instance?.IsEnable((WeaponType)_num) == false)
        {
            print($"{(WeaponType)_num} 해금 안됨");
            return;
        }


        if (_num <= 3) //선택된게 주무기일때
        {
            if (pizzaImage[_num] != null)
            {
                //디셀렉트
                if (currentHover == selectedMainWp)
                    pizzaImage[selectedMainWp].color = redHover;
                else
                    pizzaImage[selectedMainWp].color = baseColor;

                if (selectedMainWp != _num)
                    weaponSelectFeedbacks?.PlayFeedbacks();

                //셀렉트
                pizzaImage[_num].color = redSelected;
                selectedMainWp = _num;
            }
            return;
        }
        else if (4 <= _num && _num <= 7)//선택된게 보조무기일때
        {

            if (pizzaImage[_num] != null)
            {
                //디셀렉트
                if (currentHover == selectedSubWp)
                    pizzaImage[selectedSubWp].color = blueHover;
                else
                    pizzaImage[selectedSubWp].color = baseColor;

                if (selectedSubWp != _num)
                    weaponSelectFeedbacks?.PlayFeedbacks();

                //셀렉트
                pizzaImage[_num].color = blueSelected;
                selectedSubWp = _num;
            }
        }
    }

    public void FinalSelectMainWeapon(int i)
    {
        //해금 안되있으면 패스
        if (SaveDataManager.Instance.IsEnable((WeaponType)i) == false) return;

        if (chara.ConditionState.CurrentState != CharacterStates.CharacterConditions.Normal)
        {
            chara.ConditionState.OnStateChange -= () => RememberSelectWeapon(() => FinalSelectMainWeapon(i));
            chara.ConditionState.OnStateChange += () => RememberSelectWeapon(() => FinalSelectMainWeapon(i));
            //상태이상 상태면 무기변경 불가
            return;
        }
        if (currentMainWeapon == i && !BEUI.IsBehemoth)
            return;

        //망토 활성화중엔 변경불가
        if (subWeapon.Cloak.activeSelf)
            return;

        currentMainWeapon = i;
        mainWeapon.ChangeWeapon(weapons[i].GetComponent<Weapon>(), "1", false);
        isMouseSelect = false;

        //아킴보일 경우 팔 하나 추가하는 코드
        if (player.TryGetComponent(out InsideArm inArm))
        {
            inArm.SetTarget(mainWeapon.CurrentWeapon.transform);
            inArm.SetSprite(i == 3);

        }

        if (PauseManager.Instance().IsPauseCalled())
            mainWeapon.CurrentWeapon.GetComponent<WeaponAim>().enabled = false;

        BEUI.InactiveBehemoth();
        CurrentWeaponUI.instance.SetOriginMainWpImage();
        CurrentWeaponUI.ChangeMainWeapon(i);
        RefreshOverHeatUI(i);
    }

    private void RefreshOverHeatUI(int i)
    {
        OverHeatUI.ChangeMainWeapon(i);
        if (OverHeatUI.GetHeat(i) >= 100)
            mainWeapon.PermitAbility(false);
        else
            mainWeapon.PermitAbility(true);
    }


    public void FinalSelectSubWeapon(int i)
    {
        if (SaveDataManager.Instance?.IsEnable((WeaponType)i) == false)
            return;

        if (chara.ConditionState.CurrentState != CharacterStates.CharacterConditions.Normal)
        {
            chara.ConditionState.OnStateChange -= () => RememberSelectWeapon(() => FinalSelectSubWeapon(i));
            chara.ConditionState.OnStateChange += () => RememberSelectWeapon(() => FinalSelectSubWeapon(i));
            //상태이상 상태면 무기변경 불가
            return;
        }

        if (SubWeapon.currentSubWp == (SubWeaponType)i)
            return;

        isMouseSelect = false;
        SubWeapon.currentSubWp = (SubWeaponType)i;
        if (SubWeapon.currentSubWp == SubWeaponType.Grapple)
        {
            grapple.endOff = false;
            grapple.gameObject.SetActive(true);
        }
        else if (grapple)
            grapple.endOff = true;

        if (4 <= i && i <= 7)
        {
            CurrentWeaponUI.instance.ChangeSubWeapon(i);
            OverHeatUI.ChangeSubWeapon(i);
        }
    }
    void RememberSelectWeapon(Action action)
    {
        if (isMouseSelect && chara.ConditionState.CurrentState == CharacterStates.CharacterConditions.Normal)
        {
            print($"적용 {GetInstanceID()}");
            action.Invoke();
        }
    }

    public void SetPlayer(GameObject _player)
    {
        mainWeapon = _player.GetComponent<CharacterHandleWeapon>();
        subWeapon = _player.GetComponent<SubWeapon>();
        chara = _player.GetComponent<Character>();
        specialAbility = _player.GetComponent<SpecialAbility>();
        player = _player;

        print($"{GetInstanceID()} {player}");
    }

    public void SetActivate(bool _isActivate)
    {
        isActivate = _isActivate;
    }

    public void Reset()
    {
        Select(0);
        Select(4);
    }
}
