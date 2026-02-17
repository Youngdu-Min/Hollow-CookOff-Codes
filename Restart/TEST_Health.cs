using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.CorgiEngine;

public class TEST_Health : MonoBehaviour
{
    public static PlayerHealthBar instance;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Sprite heartIcon, emptyHeartIcon;
    [SerializeField]
    private Text text;

    private Health health;//플레이어 체력
    private float lastHP;
    private List<Image> heartsList = new List<Image>();
   //private List<Image> emptyHeartsList = new List<Image>();

    private GameObject filledHearts, emptyHearts;

    private GameObject FitContainer(string _name)
    {
        var obj = new GameObject(_name);    //옵젝 이름설정
        obj.transform.SetParent(this.transform);    //부모설정
        var rect = obj.AddComponent<RectTransform>();   //UI위치 초기화
        rect.anchoredPosition = Vector2.zero;   

        var layout = obj.AddComponent<HorizontalLayoutGroup>(); //그룹 레이아웃 설정
        layout.spacing = 10;
        layout.padding.top = 20;
        layout.padding.left = 20;
        layout.childAlignment = TextAnchor.MiddleLeft;
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

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
        emptyHearts = FitContainer("EmptyHearts");
        filledHearts = FitContainer("FilledHearts");
         
        }
    }
 
    void Start()
    {
        health = GameObject.Find("Player").GetComponent<Health>();

        //체력 100당 하트오브젝트 1개 생성
        for (int i = 0; i < Mathf.Ceil(health.MaximumHealth/100f); i++)
        {
            var heart = new GameObject("Heart_" + (i+1));
            heart.transform.SetParent(filledHearts.transform);

            Image img = heart.AddComponent<Image>();
            img.sprite = heartIcon;
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            heartsList.Add(img);
            

            var emptyH = new GameObject("EmptyHeart_" + (i+1));
            emptyH.transform.SetParent(emptyHearts.transform);

            Image emptyImg = emptyH.AddComponent<Image>();
            emptyImg.sprite = emptyHeartIcon;
            //emptyHeartsList.Add(emptyImg);

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
        text.text ="HP : " + (int)hp + " / " + health.MaximumHealth;
        int fullHeart = (int)hp / 100; //가득찬 하트 개수

        for (int i = 0; i <heartsList.Count; i++)
        {
            if (i < fullHeart)
            {
                heartsList[i].fillAmount = 1;
            }
            else if(i == fullHeart)
            {
                heartsList[i].fillAmount = (hp % 100)*0.01f;
            }
            else
            {
                heartsList[i].fillAmount = 0;
            }
        }
    }
}
