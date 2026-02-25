using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public UIStyle.StageUIStyle style;

    [SerializeField]
    private Image rank;
    [SerializeField]
    private GameObject display;

    [SerializeField]
    private MoreMountains.CorgiEngine.StartScreen startScreen;

    [SerializeField]
    private TextMeshProUGUI speedrunTxt, deathTxt, slashComboTxt, parryTxt, airShootTxt, flyingTargetTxt, perfectionTxt;
    [SerializeField] private TextMeshProUGUI perfectionTypeTxt;

    [Space]
    [SerializeField] private TextMeshProUGUI clearScore;
    [SerializeField] private TextMeshProUGUI clearTime;


    public void OpenDisplay()
    {
        UpdateDisplay(ScoreManager.Instance.score);
        PauseManager.Instance().PauseCall(this, true);

        MainCharacter.DisableControl();
        display.gameObject.SetActive(true);

    }


    public void CloseDisplay()
    {
        PauseManager.Instance().PauseCall(this, false);
        MainCharacter.AllowControl();
        display.gameObject.SetActive(false);
    }

    public void NextLevel()
    {
        int _index = StageSelectUI.Instance.StageIndex();
        StageSelectUI.Instance.SetStage(_index + 1);

        CloseDisplay();
        startScreen.NextLevel = StageSelectUI.Instance.CurStageName();
        startScreen.ButtonPressed();

    }
    public void TryAgain()
    {
        ScoreManager.Instance.score.Clear();
        CloseDisplay();
        MoreMountains.Tools.MMSceneLoadingManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateDisplay(Score score)//스코어 점수 계산
    {
        score.Calculate();
        Difficulty diff = (Difficulty)StageSelectUI.Instance.curDifficulty;

        int bestScore = SaveDataManager.Instance.GetHighscore(StageSelectUI.Instance.StageIndex(), diff);
        if (score.totalScore > bestScore)
            bestScore = score.totalScore;
        float bestTime = SaveDataManager.Instance.GetBestTime(StageSelectUI.Instance.StageIndex(), diff);
        if (score.clearTime < bestTime)
            bestTime = score.clearTime;

        rank.sprite = style.GetRankSprite(StageSelectUI.Instance.GetRank(score.stageIndex, score.totalScore));
        speedrunTxt.text = score.clearTimeScore.ToString();// 스피드런 점수
        deathTxt.text = score.deathsScore.ToString(); // 죽은횟수 점수
        slashComboTxt.text = score.slashScore.ToString(); // 슬래시 점수
        parryTxt.text = score.parryScore.ToString(); // 패리 점수
        airShootTxt.text = score.airshootScore.ToString();
        flyingTargetTxt.text = score.flyingTarget.ToString();
        perfectionTxt.text = score.perfectBonus.ToString();
        perfectionTypeTxt.text = score.perfectBonusString;

        clearScore.text = score.totalScore.ToString() + " / " + bestScore.ToString();
        clearTime.text = StageSelectUI.Instance.TimeFormat(score.clearTime) + " / " + StageSelectUI.Instance.TimeFormat(bestTime);
    }

}
