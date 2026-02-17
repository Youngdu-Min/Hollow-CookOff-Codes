using UnityEngine;
using UnityEngine.UI;

public class SlashCombo : MonoBehaviour
{
    public static SlashCombo instance;

    public int comboCount;
    public float comboTime = 5;
    public float curComboTime;


    [SerializeField]
    private Image maskImage;

    [SerializeField]
    private Text redText, darkText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
        //brightText = GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetCombo();
    }

    // Update is called once per frame
    void Update()
    {
        if (comboCount > 1)
        {
            redText.text = "X" + comboCount.ToString();
            darkText.text = "X" + comboCount.ToString();

            maskImage.rectTransform.sizeDelta = new Vector2(redText.rectTransform.sizeDelta.x * curComboTime / comboTime,
                maskImage.rectTransform.sizeDelta.y);
        }
        else
        {
            darkText.enabled = false;
            redText.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (comboCount > 0)
        {
            curComboTime -= Time.fixedDeltaTime;
            if (curComboTime <= 0)
            {
                ResetCombo();
            }
        }
    }


    public static void ComboUp()
    {
        instance.comboCount++;
        instance.curComboTime = instance.comboTime;

        if (instance.comboCount >= 2)
        {
            instance.redText.enabled = true;
            instance.darkText.enabled = true;
        }
    }

    public static void ResetCombo()
    {
        instance.comboCount = 0;
        instance.curComboTime = 0;

        instance.redText.text = "X0";
        instance.darkText.text = "X0";
        instance.maskImage.rectTransform.sizeDelta = new Vector2(0, instance.maskImage.rectTransform.sizeDelta.y);
    }

}
