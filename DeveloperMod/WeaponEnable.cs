using UnityEngine;

public class WeaponEnable : MonoBehaviour
{

    public WeaponType weaponType;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetWeaponEnable(bool enable)
    {
        SaveDataManager.Instance.SetEnable(weaponType, enable);
    }
}
