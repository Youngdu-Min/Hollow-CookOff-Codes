using MoreMountains.CorgiEngine;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    //주인공 구분하기쉽게 하려고 만든 스크립트. 쥔공만 가지고 있어야함

    public static MainCharacter instance;

    void SetMainCharacter(GameObject _c)
    {

        //UI 집중
        PlayerHealthBar.Instance().SetPlayer(_c);
        BEUI.Instance().SetPlayer(_c);
    }

    static public void DisableControl()
    {
        if (instance == null)
            return;

        print("컨트롤 불가");
        instance.GetComponent<CharacterFlip>().enabled = false;
        instance.GetComponentsInChildren<WeaponAim>(true).ForEach(x => x.enabled = false);
        Cursor.visible = true;

        CharacterHandleWeapon[] weapons = instance.GetComponentsInChildren<CharacterHandleWeapon>();
        CharacterAbility[] abilities = instance.GetComponentsInChildren<CharacterAbility>();

        weapons.ForEach((x) => x.enabled = false);
        abilities.ForEach((x) => x.enabled = false);
    }

    static public void AllowControl()
    {
        if (instance == null)
            return;

        print("컨트롤 가능");
        instance.GetComponent<CharacterFlip>().enabled = true;
        instance.GetComponentsInChildren<WeaponAim>(true).ForEach(x => x.enabled = true);
        CharacterHandleWeapon[] weapons = instance.GetComponentsInChildren<CharacterHandleWeapon>();
        CharacterAbility[] abilities = instance.GetComponentsInChildren<CharacterAbility>();

        weapons.ForEach((x) => x.enabled = true);
        abilities.ForEach((x) => x.enabled = true);
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("주인공 스크립트가 중복됨");
        }
        else
        {
            instance = this;
            SetMainCharacter(this.gameObject);
        }
    }

    private void Start()
    {
        WeaponSelectUI.Instance().SetPlayer(this.gameObject);
    }
}
