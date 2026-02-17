using UnityEngine;


public class ElectricWall : MonoBehaviour
{

    public bool hasLifeTime = false;
    public float lifeTime = 0;
    /// <summary>
    /// 길이
    /// </summary>
    public float length = 5;
    /// <summary>
    /// 오프셋
    /// </summary>
    public float offset = 0f;

    public float rotationSpd;
    public Vector2 speed;

    [Space]

    [SerializeField]
    private Transform[] wallTransform;

    [SerializeField]
    private BoxCollider2D platformBox;
    [SerializeField]
    private BoxCollider2D damageBox;
    [SerializeField]
    private SpriteRenderer sr;
    [SerializeField]
    private bool isSetParentManager = true;
    private bool isRotate = true;


    [ContextMenu("UpdateLength")]
    public void UpdateLength()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        UpdateOffset();
    }

    private void Awake()
    {
        if (isSetParentManager)
            transform.SetParent(ElectricWallManager.Instance().transform);
        ElectricWallManager.Instance().AddWall(this);

        // platformBox = GetComponent<BoxCollider2D>();

        platformBox.isTrigger = true;
        platformBox.usedByComposite = false;
        damageBox.isTrigger = true;
        damageBox.usedByComposite = false;


        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
    }

    /*
    public void SetLength(float _length)
    {
        sr.size = new Vector2(_length, 1);
        platformBox.size = new Vector2(_length, 1);
    }
    }*/

    public void SetLifeTime(float _time)
    {
        lifeTime = _time;
        hasLifeTime = true;
    }

    public void SetSpeed(Vector2 _speed)
    {
        speed = _speed;
    }

    public void SetDirection(Vector3 _dir)
    {
        transform.right = _dir;
    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateOffset();
        sr.drawMode = SpriteDrawMode.Tiled;
    }

    public void SetLength(float length, float offset)
    {
        this.length = length;
        this.offset = offset;
        UpdateOffset();
    }
    public void UpdateOffset()
    {
        for (int i = 0; i < wallTransform.Length; i++)
        {
            wallTransform[i].localPosition = Vector3.right * (offset / 2f);

        }
        sr.size = new Vector2(length, 1);
        platformBox.size = new Vector2(length, 1f);
        damageBox.size = new Vector2(length + 0.4f, 1.4f);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        UpdateOffset();
#endif
        //전기벽 회전

        if (rotationSpd != 0f && isRotate)
        {
            transform.Rotate(Vector3.back * Time.deltaTime * rotationSpd);
        }

        if (Mathf.Abs(speed.x) + Mathf.Abs(speed.y) > 0.01f)
        {
            transform.Translate(speed * Time.deltaTime);
        }

        if (hasLifeTime && lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                this.gameObject.SetActive(false);
                //지속시간 끝나면 사라짐
            }
        }

    }

    public void SetActiveBox_N_Sprite(bool _active)
    {
        platformBox.enabled = _active;
        damageBox.enabled = _active;
        sr.enabled = _active;
    }

    public void SetRotate(bool _rotate)
    {
        isRotate = _rotate;
    }
}
