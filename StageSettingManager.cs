using UnityEngine;
//스테이지 입장시

//이 스테이지에서
//어떤 무기 사용 가능한지,
//무기 인벤토리 열수 있는지, 등등 한꺼번에 모아놓은 곳
public class StageSettingManager : MonoBehaviour
{
    public static StageSetting Setting { get; private set; }

    [SerializeField] private StageSetting _setting;

    private void Awake()
    {
        Setting = _setting;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initiate();
    }

    [ContextMenu("Initiate")]
    private void Initiate()
    {
        SaveDataManager.Instance.SetEnable(WeaponType.Rifle, _setting.Rifle);
        SaveDataManager.Instance.SetEnable(WeaponType.Shotgun, _setting.Shotgun);
        SaveDataManager.Instance.SetEnable(WeaponType.RevolverRifle, _setting.SniperRifle);
        SaveDataManager.Instance.SetEnable(WeaponType.Machinegun, _setting.MachineGun);
        SaveDataManager.Instance.SetEnable(WeaponType.Stim, _setting.Stim);
        SaveDataManager.Instance.SetEnable(WeaponType.Hook, _setting.Hook);
        SaveDataManager.Instance.SetEnable(WeaponType.Cape, _setting.Cape);
        SaveDataManager.Instance.SetEnable(WeaponType.Grenade, _setting.Grenade);
        SaveDataManager.Instance.SetEnable(WeaponType.Behemoth, _setting.BE_gun);
        SaveDataManager.Instance.SetEnable(ContentsEnable.BE_Explosion, _setting.BE_gun);
    }


}

[System.Serializable]
public class StageSetting
{
    [Header("Weapon Accessible")]
    public bool Rifle;   //1
    public bool Shotgun;     //2
    public bool SniperRifle; //3
    public bool MachineGun;  //4
    [Space]
    public bool Stim;        //5
    public bool Hook;        //6
    public bool Cape;        //7
    public bool Grenade;     //8

    [Header("UI Accessible")]
    public bool Enable_WeaponInventory;

    [Header("BE Accessible")]
    public bool BE_gun;    //9(R)
    public bool BE_Explosion;
    public bool BE_Heal;



}