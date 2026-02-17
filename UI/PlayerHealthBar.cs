using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private static PlayerHealthBar instance;
    public static PlayerHealthBar Instance()
    {
        if (instance == null)
        {
            var obj = FindObjectOfType<PlayerHealthBar>();
            if (obj != null)
            {
                instance = obj;
                return obj;
            }
            var ins = new GameObject("Player Healthbar");
            instance = ins.AddComponent<PlayerHealthBar>();
        }
        return instance;
    }
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Sprite heartIcon, emptyHeartIcon;
    [SerializeField]
    private Text text;

    private PlayerHealth health;//플레이어 체력
    private float lastHP;
    private List<Image> heartsList = new List<Image>();
    //private List<Image> emptyHeartsList = new List<Image>();

    private GameObject filledHearts, emptyHearts;

    private GameObject FitContainer(string _name)
    {
        var obj = new GameObject(_name);    //옵젝 이름설정
        obj.transform.SetParent(this.transform);    //부모설정
        obj.transform.localScale = Vector3.one;
        obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y);
        obj.transform.localPosition = Vector3.zero;

        var layout = obj.AddComponent<HorizontalLayoutGroup>(); //그룹 레이아웃 설정
        layout.spacing = 10;
        layout.padding.top = 20;
        layout.padding.left = 20;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        return obj;
    }

    private bool HPChanged()
    {
        if (lastHP == health.CurrentHealth)
        {
            return false;
        }
        lastHP = health.CurrentHealth;
        return true;
    }

    public void SetPlayer(GameObject _player)
    {
        player = _player;
    }

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this);
            }
            else
            {
                emptyHearts = FitContainer("EmptyHearts");
                filledHearts = FitContainer("FilledHearts");
            }
        }
        else
        {

            instance = this;
            emptyHearts = FitContainer("EmptyHearts");
            filledHearts = FitContainer("FilledHearts");

        }
    }

    void Start()
    {
        health = player.GetComponent<PlayerHealth>();

        //체력 100당 하트오브젝트 1개 생성
        GenerateHeart();

    }

    void GenerateHeart()
    {
        for (int i = 0; i < Mathf.Ceil(health.MaximumHealth / HollowBalance.action.actionList[0].intValue); i++)
        {
            var heart = new GameObject("Heart_" + (i + 1));
            heart.transform.SetParent(filledHearts.transform);
            heart.transform.localPosition = new Vector2(heart.transform.localPosition.x, heart.transform.localPosition.y);
            heart.transform.localScale = Vector3.one;

            Image img = heart.AddComponent<Image>();
            img.sprite = heartIcon;
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            heartsList.Add(img);
            var emptyH = new GameObject("EmptyHeart_" + (i + 1));
            emptyH.transform.SetParent(emptyHearts.transform);
            emptyH.transform.localPosition = new Vector2(emptyH.transform.localPosition.x, emptyH.transform.localPosition.y);
            emptyH.transform.localScale = Vector3.one;

            Image emptyImg = emptyH.AddComponent<Image>();
            emptyImg.sprite = emptyHeartIcon;
            //emptyHeartsList.Add(emptyImg);

            var heartRect = heart.GetComponent<RectTransform>();
            heartRect.sizeDelta = new Vector2(50, 50);
            var emptyHeartRect = emptyH.GetComponent<RectTransform>();
            emptyHeartRect.sizeDelta = new Vector2(50, 50);

        }
    }

    void Update()
    {
        if (HPChanged())
        {
            UpdateHP(health.CurrentHealth);
        }
    }

    public void UpdateHP(float hp)
    {
        text.text = "HP : " + (int)hp + " / " + health.MaximumHealth;
        int fullHeart = (int)hp / HollowBalance.action.actionList[0].intValue; //가득찬 하트 개수

        for (int i = 0; i < heartsList.Count; i++)
        {
            if (i < fullHeart)
            {
                heartsList[i].fillAmount = 1;
            }
            else if (i == fullHeart)
            {
                heartsList[i].fillAmount = (hp % HollowBalance.action.actionList[0].intValue) * 0.01f;
            }
            else
            {
                heartsList[i].fillAmount = 0;
            }
        }
    }
}
