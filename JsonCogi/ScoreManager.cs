using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public static int stageIndex;

    public float clearTime = 0;
    public Score score = new Score();
    private BioEnerge bioEnerge;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        clearTime = 0;
        bioEnerge = FindObjectOfType<BioEnerge>();
    }

    void FixedUpdate()
    {
        if (PauseManager.Instance().IsPauseSourceCalled(bioEnerge))
            clearTime += Time.unscaledDeltaTime;
        else
            clearTime += Time.deltaTime;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        clearTime = 0;
        score.Clear();

    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        // 이벤트 핸들러 등록 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ShowScore()
    {

    }

}

[Serializable]
public class Score
{
    public int stageIndex;

    public int totalScore;

    public float clearTime;
    public int clearTimeScore = 0; //빠른 클리어 시간 점수
    public int slowcoachScore = 0; // 늦은 클리어 시간
    public int deathsScore = 0;// 사망수 점수
    public int slashScore = 0; //슬래시점수

    public int slashCount = 0;
    public int parryCount = 0;

    public int parryScore = 0; //패리점수

    public int airshootScore = 0; // 공중에서 사격 점수
    public int flyingTarget = 0; // 공중 적 사격 점수

    public int perfectBonus = 0;
    public string perfectBonusString; // 노 데미지, 노 힐, 노 사망
    public int damageCount = 0; // 데미지 x 점수
    public int healCount = 0; // 힐 x 점수
    public int deathCount = 0; // 사망x 점수


    public void Clear()
    {
        totalScore = 0;
        clearTime = 0;
        clearTimeScore = 0;
        slowcoachScore = 0;
        deathsScore = 0;
        slashScore = 0;

        slashCount = 0;
        parryCount = 0;

        parryScore = 0;
        airshootScore = 0;
        flyingTarget = 0;

        perfectBonus = 0;
        damageCount = 0;
        healCount = 0;
        deathCount = 0;

    }


    public void Calculate()//스코어 점수 계산
    {
        clearTime = ScoreManager.Instance.clearTime;
        var evaluation = Result.Evaluation.EvaluationList;
        //스피드러너 보너스만큼 점수 증가
        if (clearTime < Result.Time.TimeList[ScoreManager.stageIndex].SpeedRunner) // 스테이지를 빨리 클리어했을경우
        {
            clearTimeScore += evaluation[0].score;

            totalScore += clearTimeScore;
        }
        else if (clearTime < Result.Time.TimeList[ScoreManager.stageIndex].SlowCoach) // 늦게 클리어했을경우
        {
            //슬로우코치만큼 점수 감소
            clearTimeScore += evaluation[1].score;

            totalScore += clearTimeScore;
        }

        deathsScore = deathCount * evaluation[2].score;
        totalScore += deathsScore;

        slashScore = slashCount * evaluation[3].score;
        totalScore += slashScore;

        parryScore = parryCount * evaluation[4].score;
        totalScore += parryScore;

        Debug.Log($"ScoreTest {airshootScore} {flyingTarget}");
        airshootScore = Mathf.Min(airshootScore, evaluation[5].score);
        totalScore += airshootScore;

        flyingTarget = Mathf.Min(flyingTarget, evaluation[6].score);
        totalScore += flyingTarget;

        if (damageCount == 0)//노 데미지로 클리어
        {
            perfectBonus = evaluation[7].score;
            perfectBonusString = "NO DAMAGE";
        }
        else if (healCount == 0) //노 힐로 클리어
        {
            perfectBonus = evaluation[8].score;
            perfectBonusString = "NO HEALING";
        }
        else if (deathCount == 0) // 안죽고 클리어
        {
            perfectBonus = evaluation[9].score;
            perfectBonusString = "NO DEATH";
        }
        else
        {
            perfectBonus = 0;
            perfectBonusString = "NO DEATH";
        }

        totalScore += perfectBonus;

    }




}