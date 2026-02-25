using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BEUI : MonoBehaviour
{

    private static BEUI instance;
    public static BEUI Instance()
    {
        if (instance == null)
        {
            var obj = new GameObject("BE UI");
            instance = obj.AddComponent<BEUI>();
        }
        return instance;
    }
    public static bool IsBehemoth = false;

    [SerializeField]
    GameObject player;
    [SerializeField]
    private Sprite filledBeSprite, emptyBeSprite;
    [SerializeField]
    private Sprite behemothSprite;

    private BioEnerge be;//플레이어 체력
    private float lastBe = 0f;
    private List<Image> BeImages = new List<Image>();

    private GameObject filledGauge, emptyGauge;

    private GameObject FitContainer(string _name)
    {

        var obj = new GameObject(_name);    //옵젝 이름설정
        obj.transform.SetParent(this.transform);    //부모설정
        obj.transform.localScale = Vector3.one;
        obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y);
        var rect = obj.AddComponent<RectTransform>();   //UI위치 초기화
        rect.anchoredPosition = Vector2.zero;

        var layout = obj.AddComponent<HorizontalLayoutGroup>(); //그룹 레이아웃 설정
        layout.spacing = 15;
        layout.padding.top = 20;
        layout.padding.left = 22;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        return obj;
    }

    private bool HPChanged()
    {
        if (lastBe == be.currentBE)
        {
            return false;
        }
        lastBe = be.currentBE;
        return true;
    }

    public void SetPlayer(GameObject _player)
    {
        player = _player;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (instance != this)
        {
            Destroy(this);
        }
        else
        {
            emptyGauge = FitContainer("EmptyGauges");
            filledGauge = FitContainer("FilledGauges");

        }
    }

    void Start()
    {
        be = player.GetComponent<BioEnerge>();

        Invoke(nameof(GenerateBE), 0.2f);

    }

    void GenerateBE()
    {
        for (int i = 0; i < Mathf.Ceil(be.MaxBE / HollowBalance.action.actionList[2].intValue); i++)
        {
            var beGauge = new GameObject("BE_" + (i + 1));
            beGauge.transform.SetParent(filledGauge.transform);
            beGauge.transform.localPosition = new Vector2(beGauge.transform.localPosition.x, beGauge.transform.localPosition.y);
            beGauge.transform.localScale = Vector3.one;

            Image img = beGauge.AddComponent<Image>();
            img.sprite = filledBeSprite;
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Vertical;
            BeImages.Add(img);


            var emptyBe = new GameObject("EmptyBE_" + (i + 1));
            emptyBe.transform.SetParent(emptyGauge.transform);
            emptyBe.transform.localPosition = new Vector2(emptyBe.transform.localPosition.x, emptyBe.transform.localPosition.y);
            emptyBe.transform.localScale = Vector3.one;

            Image emptyImg = emptyBe.AddComponent<Image>();
            emptyImg.sprite = emptyBeSprite;

            var beGaugeRect = beGauge.GetComponent<RectTransform>();
            beGaugeRect.sizeDelta = new Vector2(45, 100);
            var emptyBeRect = emptyBe.GetComponent<RectTransform>();
            emptyBeRect.sizeDelta = new Vector2(45, 100);
        }
        UpdateBE(be.currentBE);
    }


    void Update()
    {
        if (HPChanged())
        {
            UpdateBE(be.currentBE);
        }
    }

    public static void ActiveBehemoth()
    {
        IsBehemoth = true;
        foreach (var item in Instance().BeImages)
        {
            item.sprite = Instance().behemothSprite;
        }
    }

    public static void InactiveBehemoth()
    {
        IsBehemoth = false;

        foreach (var item in Instance().BeImages)
        {
            item.sprite = Instance().filledBeSprite;
        }
    }

    public void UpdateBE(float _amout)
    {
        int fullHeart = (int)_amout / HollowBalance.action.actionList[2].intValue; //가득찬 하트 개수

        for (int i = 0; i < BeImages.Count; i++)
        {
            if (i < fullHeart)
            {
                BeImages[i].fillAmount = 1;
            }
            else if (i == fullHeart)
            {
                BeImages[i].fillAmount = (_amout % HollowBalance.action.actionList[2].intValue) * 0.01f;
            }
            else
            {
                BeImages[i].fillAmount = 0;
            }
        }
    }
}
