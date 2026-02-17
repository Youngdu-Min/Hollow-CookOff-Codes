using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSManager : MonoBehaviour
{
    private static GSManager _gs;
    public static GSManager Gs()
    {
        if (_gs ==null)
        {
            var obj = FindObjectOfType<GSManager>();
            if (obj == null)
            {
                _gs = obj;
                return obj;
            }

            var newObj = new GameObject("Google Sheet Manager");
            _gs = newObj.AddComponent<GSManager>();
        }
        return _gs;
    }
    public static WeaponTable.AbilityData Ability;
    public static SubweaponTable.Stim Stim;
    public static SubweaponTable.Hook Hook;
    public static SubweaponTable.Cloak Cloak;
    public static SubweaponTable.Grenade Grenade;

    private void Awake()
    {
        if (_gs == null)
        {
            _gs = this;
            DontDestroyOnLoad(this.gameObject);

        }
        if (_gs != this)
        {
            Destroy(this.gameObject);
            return;
        }
        UnityGoogleSheet.Load<WeaponTable.BaseData>();
        UnityGoogleSheet.Load<WeaponTable.AbilityData>();
        UnityGoogleSheet.Load<HollowBalance.action>();
        
        UnityGoogleSheet.Load<SubweaponTable.Cloak>();
        UnityGoogleSheet.Load<SubweaponTable.Stim>();
        UnityGoogleSheet.Load<SubweaponTable.Grenade>();
        UnityGoogleSheet.Load<SubweaponTable.Hook>();
        UnityGoogleSheet.Load<EnemyBalance.Data>();
        UnityGoogleSheet.Load<EnemyBalance.Damage>();
        UnityGoogleSheet.Load<EnemyBalance.etc>();

        UnityGoogleSheet.LoadAllData();

        Ability = WeaponTable.AbilityData.AbilityDataList[0];
    }
    // Start is called before the first frame update
    void Start()
    {
        Initiate();
    }

    private void Initiate()
    {
        Stim = SubweaponTable.Stim.StimList[0];
        Hook = SubweaponTable.Hook.HookList[0];
        Cloak = SubweaponTable.Cloak.CloakList[0];
        Grenade = SubweaponTable.Grenade.GrenadeList[0];
    }
}
