using UnityEngine;

public class WeaponBox : MonoBehaviour
{
    //접촉 시 무기 해금
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private ContentsEnable unlock;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (unlock != ContentsEnable.None)
            {
                //무기관련 시스템 해금
                switch (unlock)
                {
                    case ContentsEnable.BE_Explosion:
                        SaveDataManager.Instance.SetEnable(unlock, true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //기타 시스템 해금

                SaveDataManager.Instance.SetEnable(weaponType, true);
            }

            gameObject.SetActive(false);
        }
    }


}

public enum ContentsEnable
{
    None = 0,
    BE_Explosion = 1,

}