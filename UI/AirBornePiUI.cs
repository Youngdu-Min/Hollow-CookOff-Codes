using MoreMountains.CorgiEngine;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AirBornePiUI : MonoBehaviour
{

    private static AirBornePiUI instance;

    public static AirBornePiUI Instance()
    {
        if (instance == null)
        {
            new GameObject("Air Borne Pi UI").AddComponent<AirBornePiUI>();
        }
        return instance;
    }

    [SerializeField] private GameObject airBorneObject = null;
    private Image airBorneImg;
    private Image blockImg;
    public GameObject BarTr { get; private set; }
    private AsyncOperationHandle handle;
    private LevelManager levelManager;

    public float saveNum = 0;
    [SerializeField]
    private string knifeAddress = "Prefabs/KnifeTime.prefab";
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = transform.GetComponentInParent<Canvas>();
        //캔버스 있는지 확인
        if (!_canvas)
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        levelManager = FindObjectOfType<LevelManager>();

        if (instance == null)
            instance = this;


    }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == this)
            Initiate();
        else
            Destroy(this);
        /*
        if (m_goPrefab == null)
        {
            //프리팹 없으면 어드레서블에서 찾아옴
            Addressables.LoadAssetAsync<GameObject>("pirouetteBar").Completed +=
            (AsyncOperationHandle<GameObject> obj) =>
            {
                handle = obj;
                m_goPrefab = obj.Result;
                
                
            };
        }
        else
        {
            Initiate();
        }
        */

    }

    private void Initiate()
    {
        GameObject targetObj = levelManager.Players[0].gameObject;


        if (!airBorneObject)
        {
            //프리팹 없으면 어드레서블에서 찾아옴
            Addressables.LoadAssetAsync<GameObject>(knifeAddress).Completed +=
            (AsyncOperationHandle<GameObject> obj) =>
            {
                handle = obj;
                airBorneObject = obj.Result;

                BarTr = Instantiate(obj.Result, targetObj.transform.position, Quaternion.identity, transform);
                BarTr.gameObject.SetActive(false);
                Addressables.Release(handle);
                Image[] images = BarTr.GetComponentsInChildren<Image>();
                airBorneImg = images.Skip(1).First();
                blockImg = images.Last();
            };
        }
        else
        {
            if (Instance() != null)
            {
                BarTr = Instantiate(airBorneObject, targetObj.transform.position, Quaternion.identity, transform);
                BarTr.gameObject.SetActive(false);
                Image[] images = BarTr.GetComponentsInChildren<Image>();
                airBorneImg = images.Skip(1).First();
                blockImg = images.Last();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!BarTr)
            return;

        if (Input.GetButtonUp("Player1_SecondaryShoot"))
        {
            BarTr.gameObject.SetActive(false);
            airBorneImg.fillAmount = 0;
        }

        if (Input.GetButton("Player1_SecondaryShoot"))
        {
            Vector2 screenPoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out screenPoint);
            BarTr.transform.localPosition = screenPoint;

            if (PauseManager.Instance().IsPauseCalled())
                return;

            BarTr.gameObject.SetActive(true);
            airBorneImg.fillAmount = saveNum;
        }
    }

    public void ShowBlockImg(bool isShow)
    {
        blockImg?.gameObject.SetActive(isShow);
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR

#else
        Addressables.Release(handle);
#endif

    }
}