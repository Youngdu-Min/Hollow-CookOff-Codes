using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ArmorHealthDisplay : MonoBehaviour
{

    //적의 체력,갑옷의 체력바 표시해주는 스크립트
    // [SerializeField]
    public float curHealth;
    // [SerializeField]
    public float curArmor;
    [SerializeField]
    private GameObject hpBarObject;
    private RectTransform hpBarAnchor;

    private Image healthImage, armorImage;
    [HideInInspector]
    public HealthExpend corgiHealth;

    private float lastHp, lastArmor;
    [field: SerializeField]
    public bool ignoreKnife { get; private set; }

    private Collider2D coll;
    private Canvas canvas;
    private RectTransform canvasRect;
    private float currentAspectRatio = 0f;
    private Vector2 lastPos;
    private Vector2 lastCamPos;
    private bool isAspectChanged = false;

    private bool HpChanged()
    {
        if (corgiHealth.onlyArmor)
            curHealth = default;
        else
            curHealth = corgiHealth.CurrentHealth;
        curArmor = corgiHealth.currentArmor;
        if (lastHp == curHealth && lastArmor == curArmor)
        {
            return false;
        }
        lastHp = curHealth;
        lastArmor = curArmor;
        return true;

    }


    public void UpdateHealthBar()
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(coll.bounds.center + new Vector3(0.0f, coll.bounds.extents.y) + new Vector3(0, 0.6f));

        // Viewport에서의 위치를 실제 캔버스 위치로 변환
        screenPosition.x -= Camera.main.pixelRect.x;
        screenPosition.y -= Camera.main.pixelRect.y;

        Vector2 canvasPosition = new Vector2(
            screenPosition.x * canvasRect.rect.width / Camera.main.pixelRect.width,
            screenPosition.y * canvasRect.rect.height / Camera.main.pixelRect.height);

        hpBarAnchor.anchoredPosition = canvasPosition - (canvasRect.sizeDelta / 2f);

        if (HpChanged())//HP에 변화가 있으면
        {
            curArmor = corgiHealth.currentArmor;
            armorImage.fillAmount = curArmor / corgiHealth.maxArmor;
            healthImage.fillAmount = curHealth / corgiHealth.MaximumHealth;
            if (curHealth <= 0 && !corgiHealth.onlyArmor)
                hpBarObject.SetActive(false);
            else
                hpBarObject.SetActive(true);
        }
    }

    private void Awake()
    {

        corgiHealth = GetComponent<HealthExpend>();   //코기엔진 체력스크립트에서 체력 받아옴
        curHealth = corgiHealth.CurrentHealth;
        curArmor = corgiHealth.initialArmor;
        canvas = HPBarManager.instance.GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {

        hpBarObject = new GameObject(name + "HealthBar");
        hpBarObject.transform.SetParent(HPBarManager.instance.transform);
        hpBarObject.transform.localPosition = Vector3.zero;
        hpBarObject.transform.localScale = Vector3.one;
        hpBarObject.AddComponent<RectTransform>();
        hpBarAnchor = hpBarObject.GetComponent<RectTransform>();

        //체력UI 생성
        var hpObj = new GameObject(name + "Health");
        hpObj.transform.SetParent(hpBarObject.transform);
        hpObj.transform.localPosition = Vector3.zero;
        hpObj.transform.localScale = Vector3.one;
        healthImage = hpObj.AddComponent<Image>();
        healthImage.sprite = HPBarManager.instance.healthBar;
        healthImage.SetNativeSize();
        healthImage.type = Image.Type.Filled;
        healthImage.fillMethod = Image.FillMethod.Horizontal;

        //장갑UI 생성
        var armorObj = new GameObject(name + "Armor");
        armorObj.transform.SetParent(hpBarObject.transform);
        armorObj.transform.localPosition = Vector3.zero;
        armorObj.transform.localScale = Vector3.one;
        armorImage = armorObj.AddComponent<Image>();
        armorImage.sprite = HPBarManager.instance.armorBar;
        armorImage.SetNativeSize();
        armorImage.type = Image.Type.Filled;
        armorImage.fillMethod = Image.FillMethod.Horizontal;
        coll = GetComponent<Collider2D>();

        CheckAspectChange();
    }

    public void BarToggle(bool b)
    {
        if (healthImage && armorImage)
        {
            healthImage.enabled = b;
            armorImage.enabled = b;

        }
    }

    private async void CheckAspectChange()
    {
        while (true)
        {
            await UniTask.WaitUntil(() => Mathf.Approximately((float)Screen.width / Screen.height, currentAspectRatio) == false);
            isAspectChanged = true;
            currentAspectRatio = (float)Screen.width / Screen.height;
            await UniTask.Yield();

            // 갱신할 처리
        }
    }

    void OnEnable()
    {
        BarToggle(true);
    }

    void OnDisable()
    {
        BarToggle(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
    }
}