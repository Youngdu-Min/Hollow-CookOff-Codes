using UnityEngine;
using UnityEngine.UI;

public class PirouetteGage : MonoBehaviour
{
    private static PirouetteGage instance;

    public static PirouetteGage Instance()
    {
        if (instance == null)
        {
            new GameObject("Pirouette Gage").AddComponent<PirouetteGage>();
        }
        return instance;
    }
    public Image image;
    [SerializeField]
    private Color stunYellow, stunRed;

    public SpecialAbility ability;

    public GameObject player;

    void Awake()
    {
        if (instance == null)
            instance = this;

        if (instance != this)
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {

        player = MainCharacter.instance.gameObject;

        image = GetComponent<Image>();
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        ability = player.GetComponent<SpecialAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = ability.spinGage / GSManager.Ability.pirouetteStunMax;
        if (image.fillAmount == 1)
            image.color = stunRed;
        else
            image.color = stunYellow;

        transform.position = Camera.main.WorldToScreenPoint(player.transform.position + Vector3.up);
    }
}
