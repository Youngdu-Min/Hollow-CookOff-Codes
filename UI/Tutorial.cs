using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class Tutorial : MonoBehaviour
{

    public static Tutorial Instance;

    private Sprite[] images;

    [SerializeField] private int currentPage;
    [SerializeField] private Image display;
    [SerializeField] private GameObject displayGo;
    [SerializeField] private GameObject NextArrow, BackArrow;
    [SerializeField] private Button closeBtn;
    [SerializeField] private MMFeedbacks transitionFeedbacks;
    private int endPageIdx;
    private int lastPageIdx;
    private UnityEvent tutorialEndEvent;

    public void StartTutorial()
    {
        BackArrow.SetActive(false);
        NextArrow.SetActive(images.Length > 1);
        closeBtn.gameObject.SetActive(images.Length == 1);

        PauseManager.Instance().PauseCall(this, true);

        endPageIdx = images.Length - 1;
        currentPage = 0;
        display.sprite = images[0];
        displayGo.SetActive(true);
    }

    public void StartTutorial(Sprite[] _images, UnityEvent endEvent = null)
    {
        images = _images;
        StartTutorial();
        tutorialEndEvent = endEvent;
    }


    public void MovePage(bool next)
    {
        currentPage = next ? currentPage + 1 : currentPage - 1;
        currentPage = Mathf.Clamp(currentPage, 0, endPageIdx);

        if (endPageIdx <= currentPage)
            closeBtn.gameObject.SetActive(true);

        display.sprite = images[currentPage];
        NextArrow.SetActive(currentPage < endPageIdx);
        BackArrow.SetActive(currentPage > 0);
        if (currentPage != lastPageIdx)
            transitionFeedbacks?.PlayFeedbacks();
        lastPageIdx = currentPage;
    }


    public void EndTutorial()
    {
        transitionFeedbacks?.PlayFeedbacks();
        PauseManager.Instance().PauseCall(this, false);
        //Time.timeScale = 1f;
        NextArrow.SetActive(false);
        BackArrow.SetActive(false);
        closeBtn.gameObject.SetActive(false);
        displayGo.SetActive(false);
        tutorialEndEvent?.Invoke();
        tutorialEndEvent = null;


    }

    private void Awake()
    {

        DontDestroyOnLoad(gameObject);
        Instance = this;

    }

    private void Start()
    {
        closeBtn.onClick.AddListener(EndTutorial);
    }

    private void Update()
    {
        if (displayGo.activeSelf == false)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            MovePage(false);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MovePage(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndTutorial();
        }
    }
}
