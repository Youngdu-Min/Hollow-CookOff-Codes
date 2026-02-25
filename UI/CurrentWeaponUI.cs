using UnityEngine;
using UnityEngine.UI;

public class CurrentWeaponUI : MonoBehaviour
{
    public static CurrentWeaponUI instance;
    [SerializeField]
    private Sprite mainWeaponBg, subWeaponBg;
    public Image mainWpImage, subWpImage;
    [SerializeField] private Image leftTimeImage;
    private Vector2 originWeaponSize;

    private void Awake()
    {

        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            return;
        }

        instance = this;
        originWeaponSize = mainWpImage.GetComponent<RectTransform>().sizeDelta;
        Initiate();
    }

    private void Initiate()
    {
        /*
        var mainBg = new GameObject("MainWeaponBg");

        mainBg.transform.SetParent(this.gameObject.transform);
        RectTransform mainrect = mainBg.AddComponent<RectTransform>();
        //mainrect.anchoredPosition = Vector2.zero;
        mainrect.pivot = Vector2.zero;
        mainBg.transform.localPosition = Vector2.zero;

        var mainsprite = mainBg.AddComponent<Image>();
        mainsprite.sprite = mainWeaponBg;
        mainsprite.SetNativeSize();
        
        var mainObj = new GameObject("MainWeaponSprite");
        mainWpImage = mainObj.AddComponent<Image>();
        mainObj.transform.SetParent(this.gameObject.transform);
        var rect3 = mainObj.GetComponent<RectTransform>();
        mainObj.transform.localPosition = mainrect.sizeDelta * 0.5f;
        
        var subBg = new GameObject("SubWeaponBg");
        subBg.transform.SetParent(this.gameObject.transform);
        RectTransform subrect = subBg.AddComponent<RectTransform>();
        // subrect.anchoredPosition = Vector2.zero;
        subrect.pivot = Vector2.zero;
        subBg.transform.localPosition = new Vector2(96, 0);
        var subSprite = subBg.AddComponent<Image>();
        subSprite.sprite = subWeaponBg;
        subSprite.SetNativeSize();
        

        var obj4 = new GameObject("SubWeaponSprite");
        subWpImage = obj4.AddComponent<Image>();
        obj4.transform.SetParent(this.gameObject.transform);
        var rect4 = obj4.GetComponent<RectTransform>();
        obj4.transform.localPosition = (Vector2)subrect.localPosition + subrect.sizeDelta * 0.5f;
        rect4.localScale = new Vector2(0.75f, 0.75f);
        */
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeMainWeapon(0);
    }

    public static void ChangeMainWeapon(int index)
    {

        instance.mainWpImage.sprite = WeaponSelectUI.Instance().weaponSprites[index];
        //instance.mainWpImage.SetNativeSize();
    }

    public void ChangeSubWeapon(int index)//index = 4~7
    {
        SetActiveSubWeaponImage(true);
        if (instance.subWpImage != null)
        {
            instance.subWpImage.sprite = WeaponSelectUI.Instance().weaponSprites[index];
            instance.subWpImage.color = Color.white;
        }

    }

    public void SetActiveSubWeaponImage(bool active)
    {
        subWpImage.gameObject.SetActive(active);
    }

    public void ChangeSubWeaponColor(Color color)
    {
        subWpImage.color = color;
    }

    public void RefreshLeftTime(float leftTime)
    {
        leftTimeImage.fillAmount = leftTime;
    }

    public void SetOriginMainWpImage()
    {
        mainWpImage.GetComponent<RectTransform>().sizeDelta = originWeaponSize;
    }

    public static void ResetUI()
    {
        if (instance.mainWpImage != null)
        {
            instance.mainWpImage.sprite = WeaponSelectUI.Instance().weaponSprites[0];
            //instance.mainWpImage.SetNativeSize();
            // instance.subWpImage.sprite = WeaponSelectUI.Instance().weaponSprites[4];
        }
    }
}
