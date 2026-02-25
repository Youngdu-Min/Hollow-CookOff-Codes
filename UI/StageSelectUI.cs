using MoreMountains.Feedbacks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    public static StageSelectUI Instance;

    [SerializeField] private AreaData[] areaDatas;
    [SerializeField] private MoreMountains.CorgiEngine.StartScreen startButton, virtualStartButton;
    [SerializeField] private UIStyle.StageUIStyle m_style;
    [SerializeField] private List<Button> uiButtons = new List<Button>();
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Image basePanal;
    [SerializeField] private GameObject clearInfoParent;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI stageText, clearTimeText;
    [SerializeField] private Image rankImage;
    [SerializeField] private Image stageImage;

    [Header("Difficulty Buttons")]
    [SerializeField] private Image ParabellumBtn;
    [SerializeField] private Image FullMetalBtn;
    [SerializeField] private Image HollowPointBtn;

    //스테이지 선택 UI
    [Header("arrows")]
    public CustomButtonKey leftArrow;
    public CustomButtonKey rightArrow;
    public int curDifficulty = 0;

    private bool dirty = false;
    [SerializeField] private MMFeedbacks changeDiffFeedbacks;
    [SerializeField] private MMFeedbacks changeStageFeedbacks;

    public int StageIndex()
    {
        int index = 0;
        for (int i = 0; i < _curArea; i++)
        {
            index += areaDatas[i].Stages.Length;
        }
        index += _curStage;
        return index;
    }

    public void SetStage(int _index)
    {
        for (int i = 0; i < areaDatas.Length; i++)
        {
            if (_index < areaDatas[i].Stages.Length)
            {
                curArea = i;
                curStage = _index;
                break;
            }
            else
            {
                _index -= areaDatas[i].Stages.Length;
            }
            curStage = i;
        }

    }

    public int curStage
    {
        get { return _curStage; }
        set
        {
            dirty = _curStage != value;
            _curStage = value;
            if (changeStageFeedbacks && changeStageFeedbacks.gameObject.activeInHierarchy)
                changeStageFeedbacks?.PlayFeedbacks();
        }
    }
    private int _curStage = 0;

    public int curArea
    {
        get { return _curArea; }
        set
        {
            dirty = _curArea != value;
            _curArea = value;
            _curArea = Mathf.Clamp(_curArea, 0, areaDatas.Length - 1);

            _curStage = 0;
            /*
            if (areaDatas[curArea].Stages.Length - 1 < curStage)
                curStage = areaDatas[curArea].Stages.Length - 1;
            */
        }
    }
    private int _curArea = 0;

    private bool IsFirstStage()
        => curStage == 0;
    private bool IsLastStage()
        => curStage == areaDatas[curArea].Stages.Length - 1;

    public void CheckInput()
    {
        int _dif = curDifficulty;

        if (Input.GetKeyDown(KeyCode.E))
            _dif++;
        else if (Input.GetKeyDown(KeyCode.Q))
            _dif--;

        _dif = Mathf.Clamp(_dif, 0, 2);

        if (curDifficulty != _dif)
        {
            dirty = true;
            SetDifficulty((Difficulty)_dif);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            curArea--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            curArea++;
        }

    }

    public void SetDifficulty(int difficulty)
        => SetDifficulty((Difficulty)difficulty);

    private void SetDifficulty(Difficulty difficulty)
    {
        if (changeDiffFeedbacks.gameObject.activeInHierarchy && curDifficulty != (int)difficulty)
            changeDiffFeedbacks?.PlayFeedbacks();
        curDifficulty = (int)difficulty;
        dirty = true;
    }

    public void RefreshDifficultyUI(Image image)
    {
        switch ((Difficulty)curDifficulty)
        {
            case Difficulty.Easy:
                image.color = m_style.parabellumColor;
                break;
            case Difficulty.Normal:
                image.color = m_style.fullMetalColor;
                break;
            case Difficulty.Hard:
                image.color = m_style.hollowPointColor;
                break;
            case Difficulty.Insane:
                break;
            default:
                break;
        }
    }

    public void OnRightArrowClick()
    {
        if (IsLastStage()) return;

        curStage++;
    }

    public void OnLeftArrowClick()
    {
        if (IsFirstStage()) return;

        curStage--;
    }


    public void UpdateUI()
    {
        ScoreManager.stageIndex = StageIndex();

        int _score = SaveDataManager.Instance.GetHighscore(StageIndex(), (Difficulty)curDifficulty);
        scoreText.text = _score.ToString("00000");

        stageText.text = areaDatas[curArea].Stages[curStage].SceneName;


        SetBestTime(ref clearTimeText);

        stageImage.sprite = m_style.stageImg[StageIndex()];
        SetRank(GetRank(StageIndex(), _score));
        SetDifficulty(curDifficulty);
        RefreshDifficultyUI(basePanal);

        startButton.gameObject.SetActive(EcessableStage(StageIndex()));
        if (SaveDataManager.Instance.IsWatchedEnding)
            backGroundImage.sprite = m_style.afterEndingImage;
        else
            backGroundImage.sprite = m_style.stageImg[SaveDataManager.Instance.GetLastStage()];
        leftArrow.gameObject.SetActive(!IsFirstStage());
        rightArrow.gameObject.SetActive(!IsLastStage());
        clearInfoParent?.SetActive(!areaDatas[curArea].Stages[curStage].IsCutscene);

    }

    public void SetBestTime(ref TextMeshProUGUI text)
    {
        float time = SaveDataManager.Instance.GetBestTime(StageIndex(), (Difficulty)curDifficulty);
        text.text = TimeFormat(time);
    }

    public string TimeFormat(float time)
    {
        if (time > Mathf.Pow(60, 3) || time <= 0)
            return "--:--:--";

        int minute = Mathf.FloorToInt(time / 60);
        int second = Mathf.FloorToInt(time % 60);
        int millisecond = Mathf.FloorToInt(time * 100) % 100;
        return string.Format("{0:00}:{1:00}.{2:00}", minute, second, millisecond);

    }

    public bool IsCutscene()
        => areaDatas[curArea].Stages[curStage].IsCutscene;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;

            Initiate();
        }
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetStyle();
        SetStage(SaveDataManager.Instance.TopStage());
        UpdateUI();
        SceneManager.activeSceneChanged += ChangeActiveScene;

        void ChangeActiveScene(Scene current, Scene next)
        {
            AreaData area;
            for (int i = 0; i < areaDatas.Length; i++)
            {
                area = areaDatas[i];
                for (int j = 0; j < area.Stages.Length; j++)
                {
                    if (area.Stages[j].SceneName == next.name)
                    {
                        curArea = i;
                        curStage = j;
                        break;
                    }
                }
            }

            if (FindObjectOfType<BossHealthDisplay>())
                PauseManager.Instance().SetActiveExceptBossSceneObjects(false);
            else
                PauseManager.Instance().SetActiveTimeObjectParent(!areaDatas[curArea].Stages[curStage].IsCutscene);
        }
    }


    private void Initiate()
    {

        ParabellumBtn.color = m_style.parabellumColor;
        FullMetalBtn.color = m_style.fullMetalColor;
        HollowPointBtn.color = m_style.hollowPointColor;
        backGroundImage.sprite = m_style.stageImg[StageIndex()];

    }
    // Update is called once per frame
    void Update()
    {
        CheckInput();

        if (dirty)
        {
            UpdateUI();
            dirty = false;
        }

    }

    public void SetStyle()
    {
        foreach (var item in uiButtons)
            item.colors = m_style.buttonColors;
    }

    public bool EcessableStage(int stage)
        => stage <= SaveDataManager.Instance.TopStage();

    public void SelectArea(int area)
    {
        curArea = area;

        if (changeStageFeedbacks && changeStageFeedbacks.gameObject.activeInHierarchy && dirty)
            changeStageFeedbacks?.PlayFeedbacks();
    }

    public void AddStageNumber(int amount)
        => curStage += amount;

    public ScoreRank GetRank(int stage, int score)
    {
        Result.Rank table = Result.Rank.RankMap[stage];

        if (score == 0)
            return ScoreRank.Zero;
        if (score >= table.Srank)
            return ScoreRank.S;
        else if (score >= table.Arank)
            return ScoreRank.A;
        else if (score >= table.Brank)
            return ScoreRank.B;
        else if (score >= table.Crank)
            return ScoreRank.C;

        return ScoreRank.D;

    }

    private void SetRank(ScoreRank rank)
    {
        switch (rank)
        {
            case ScoreRank.S:
                rankImage.sprite = m_style.rankS;
                break;
            case ScoreRank.A:
                rankImage.sprite = m_style.rankA;
                break;
            case ScoreRank.B:
                rankImage.sprite = m_style.rankB;
                break;
            case ScoreRank.C:
                rankImage.sprite = m_style.rankC;
                break;
            case ScoreRank.D:
                rankImage.sprite = m_style.rankD;
                break;
            case ScoreRank.Zero:
                rankImage.sprite = m_style.rankZero;
                break;
            default:
                break;
        }
    }


    public void EnterStage()
    {
        if (EcessableStage(StageIndex()))
        {

            startButton.NextLevel = CurStageName();
            startButton.ButtonPressed();
            SaveDataManager.Instance.SetDifficulty((Difficulty)curDifficulty);
            SaveDataManager.Instance.SetLastStage(StageIndex());
            SaveDataManager.Instance.Save();

        }
        else
        {
        }
    }

    public void EnterFirstStage()
    {
        curArea = 0;
        curStage = 0;
        virtualStartButton.NextLevel = "1-1";
        virtualStartButton.ButtonPressed();
        SaveDataManager.Instance.SetLastStage(0);
        SaveDataManager.Instance.SetDifficulty((Difficulty)curDifficulty);
        SaveDataManager.Instance.Save();

    }

    public void ContinueStage()
    {
        SetStage(SaveDataManager.Instance.GetLastStage());
        virtualStartButton.NextLevel = CurStageName();
        virtualStartButton.ButtonPressed();
        SaveDataManager.Instance.OverrideDifficulty();
    }

    public string CurStageName()
    {
        string result = areaDatas[curArea].Stages[curStage].SceneName;
        return result.Trim();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

