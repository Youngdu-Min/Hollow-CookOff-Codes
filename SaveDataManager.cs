using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
public class SaveDataManager : MonoBehaviour
{
    public const int MAX_STAGE = 18;
    private int currentStage;
    private Difficulty currentDifficulty;

    public Difficulty CurrDifficulty
        => currentDifficulty;

    private bool isWatchedEnding;
    public bool IsWatchedEnding => isWatchedEnding;

    private SaveData saveData;

    public static SaveDataManager Instance { get; private set; }

    private bool isTestMode = false;
    public bool IsTestMode => isTestMode;

    private void Update()
    {
        if (IsTestMode && Input.GetKey(KeyCode.F2))
            PlayerPrefs.DeleteAll();
    }

    public void SetWatchedEnding(bool isWatched)
    {
        isWatchedEnding = isWatched;
        Instance.Save();
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        Instance.saveData.difficulty = difficulty;
    }

    public void OverrideDifficulty()
        => currentDifficulty = Instance.saveData.difficulty;

    public int TopStage()
    {
        if (Instance.saveData != null)
        {
            return Instance.saveData.TopStage;

        }
        else
        {
            return 0;
        }
    }

    public string StageToString(int stage)
    {
        return (TopStage() / 4 + 1) + "-" + TopStage() % 4;

    }


    // Start is called before the first frame update
    void Start()
    {
        //SaveHighscore(1, Difficulty.Easy, 1234, 56.78f);
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void SetClearStage(int stage)
    {
        stage = Mathf.Min(stage, MAX_STAGE);

        Instance.saveData.TopStage = stage;
    }

    /// <summary>
    /// 스테이지, 난이도 설정하고 점수 초기화
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="difficulty"></param>
    public void FormatScore(int stage, Difficulty difficulty)
    {
        currentStage = stage;
        currentDifficulty = difficulty;
    }

    /// <summary>
    /// 현재 스테이지의 점수 저장
    /// </summary>
    public void SaveCurrentHighScore()
    {
        SaveHighscore(currentStage, currentDifficulty, ScoreManager.Instance.score.totalScore, ScoreManager.Instance.score.clearTime);

    }


    /// <summary>
    ///   // 해당 스테이지,난이도에 하이스코어 저장
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="difficulty"></param>
    /// <param name="score"></param>
    public void SaveHighscore(int stage, Difficulty difficulty, int score, float clearTime)
    {


        ScoreData existingHighscore = Instance.saveData.highscores.Find(h => h.stage == stage && h.difficulty == difficulty);
        if (existingHighscore != null)
        {
            if (score > existingHighscore.score)
            {
                existingHighscore.score = score;
            }
            if (clearTime < existingHighscore.bestTime)
            {
                existingHighscore.bestTime = clearTime;
            }
        }
        else
        {
            // Add a new highscore if it doesn't exist
            Instance.saveData.highscores.Add(new ScoreData(stage, difficulty, score, clearTime));
        }

        // Save the data
        Instance.Save();
    }

    /// <summary>
    /// 해당 난이도,스테이지의 하이스코어값 가져오기
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="difficulty"></param>
    /// <returns></returns>
    public int GetHighscore(int stage, Difficulty difficulty)
    {

        ScoreData highscore = Instance.saveData.highscores.Find(h => h.stage == stage && h.difficulty == difficulty);
        if (highscore != null)
        {
            return highscore.score;
        }
        else
        {
            return 0;
        }
    }

    public float GetBestTime(int stage, Difficulty difficulty)
    {
        ScoreData highscore = Instance.saveData.highscores.Find(h => h.stage == stage && h.difficulty == difficulty);
        if (highscore != null)
        {
            return highscore.bestTime;
        }
        else
        {
            return int.MaxValue;
        }
    }

    public void SetLastStage(int stage)
    {
        Instance.saveData.lastStage = stage;
        SetWatchedEnding(false);
    }

    public int GetLastStage()
        => Instance.saveData.lastStage;

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("SaveData", jsonData);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("SaveData"))
        {
            string jsonData = PlayerPrefs.GetString("SaveData");
            saveData = JsonUtility.FromJson<SaveData>(jsonData);
        }
        else
        {
            saveData = new SaveData();
        }

    }

    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        saveData = new SaveData();
    }

    public void StageClear(int stage)
    {

        SaveHighscore(stage, currentDifficulty, ScoreManager.Instance.score.totalScore, ScoreManager.Instance.clearTime);

        Instance.saveData.TopStage = Mathf.Max(stage + 1, Instance.saveData.TopStage);
        Instance.saveData.lastStage = Mathf.Min(stage + 1, Instance.saveData.TopStage);

        Instance.Save();

    }
    public void SetEnable(WeaponType weaponType, bool enable)
    {
        bool isGetMainWeapon = isMainWeapon(weaponType) && enable;
        bool isGetSubWeapon = IsSubWeapon(weaponType) && enable;

        switch (weaponType)
        {
            case WeaponType.Rifle:
                Instance.saveData.AR_Enable = enable;
                break;
            case WeaponType.Shotgun:
                Instance.saveData.SG_Enable = enable;
                break;
            case WeaponType.RevolverRifle:
                Instance.saveData.SR_Enable = enable;
                break;
            case WeaponType.Machinegun:
                Instance.saveData.MG_Enable = enable;
                break;

            case WeaponType.Stim:
                Instance.saveData.Stim_Enable = enable;
                break;
            case WeaponType.Hook:
                Instance.saveData.Hook_Enable = enable;
                break;
            case WeaponType.Cape:
                Instance.saveData.Cape_Enable = enable;
                break;
            case WeaponType.Grenade:
                Instance.saveData.Grenade_Enable = enable;
                break;
            case WeaponType.Behemoth:
                Instance.saveData.Behemoth_Enable = enable;
                break;

            default:
                break;
        }

        if (enable)
            WeaponSelectUI.Instance().Select((int)weaponType);
        if (isGetMainWeapon)
            WeaponSelectUI.Instance().FinalSelectMainWeapon((int)weaponType);
        else if (isGetSubWeapon)
            WeaponSelectUI.Instance().FinalSelectSubWeapon((int)weaponType);

        SetEnableInventory();
        Instance.Save();

    }
    public void SetEnable(ContentsEnable contentsType, bool enable)
    {
        switch (contentsType)
        {
            case ContentsEnable.None:
                break;
            case ContentsEnable.BE_Explosion:
                Instance.saveData.BE_Explosion_Enable = enable;
                break;
            default:
                break;
        }
        Instance.Save();
    }

    public void SetEnableEveryWeapon()
    {
        Instance.saveData.AR_Enable = true;
        Instance.saveData.SG_Enable = true;
        Instance.saveData.SR_Enable = true;
        Instance.saveData.MG_Enable = true;
        Instance.saveData.Stim_Enable = true;
        Instance.saveData.Hook_Enable = true;
        Instance.saveData.Cape_Enable = true;
        Instance.saveData.Grenade_Enable = true;
        Instance.saveData.Behemoth_Enable = true;
        Instance.saveData.BE_Explosion_Enable = true;

        SetEnableInventory();
        Instance.Save();
    }

    public void SetEnableInventory()
        => Instance.saveData.Inventory_Enable = true;

    public bool IsEnable(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Rifle:
                return Instance.saveData.AR_Enable;

            case WeaponType.Shotgun:
                return Instance.saveData.SG_Enable;

            case WeaponType.RevolverRifle:
                return Instance.saveData.SR_Enable;

            case WeaponType.Machinegun:
                return Instance.saveData.MG_Enable;

            case WeaponType.Stim:
                return Instance.saveData.Stim_Enable;

            case WeaponType.Hook:
                return Instance.saveData.Hook_Enable;

            case WeaponType.Cape:
                return Instance.saveData.Cape_Enable;

            case WeaponType.Grenade:
                return Instance.saveData.Grenade_Enable;

            case WeaponType.Behemoth:
                return Instance.saveData.Behemoth_Enable;

            default:
                break;
        }
        return false;
    }

    public bool IsEnable(ContentsEnable contentsType)
    {
        switch (contentsType)
        {
            case ContentsEnable.BE_Explosion:
                return Instance.saveData.BE_Explosion_Enable;
            default:
                break;
        }
        return false;
    }

    public bool IsEnableInventory()
        => Instance.saveData.Inventory_Enable;

    private bool IsEnableAnySubWeapon()
        => Instance.saveData.Stim_Enable || Instance.saveData.Hook_Enable || Instance.saveData.Cape_Enable || Instance.saveData.Grenade_Enable;

    public int? GetEnableSubWeaponIndex()
    {
        if (Instance.saveData.Stim_Enable)
            return 4;
        if (Instance.saveData.Hook_Enable)
            return 5;
        if (Instance.saveData.Cape_Enable)
            return 6;
        if (Instance.saveData.Grenade_Enable)
            return 7;

        return null;
    }

    private bool isMainWeapon(WeaponType weaponType)
        => weaponType == WeaponType.Rifle || weaponType == WeaponType.Shotgun || weaponType == WeaponType.RevolverRifle || weaponType == WeaponType.Machinegun;

    private bool IsSubWeapon(WeaponType weaponType)
        => weaponType == WeaponType.Stim || weaponType == WeaponType.Hook || weaponType == WeaponType.Cape || weaponType == WeaponType.Grenade;
}

[System.Serializable]
class ScoreData
{
    public int stage;
    public Difficulty difficulty;
    public int score;
    public float bestTime = 1800;

    public ScoreData(int stage, Difficulty difficulty, int score, float bestTime)
    {
        this.stage = stage;
        this.difficulty = difficulty;
        this.score = score;
        this.bestTime = bestTime;
    }
}

[System.Serializable]
class SaveData
{
    public List<ScoreData> highscores;
    public Difficulty difficulty;
    public int lastStage;

    public bool AR_Enable = true;
    public bool SG_Enable = false;
    public bool SR_Enable = false;
    public bool MG_Enable = false;


    public bool Stim_Enable = false;
    public bool Hook_Enable = false;
    public bool Cape_Enable = false;
    public bool Grenade_Enable = false;
    public bool Behemoth_Enable = false;

    public bool BE_Explosion_Enable = false;
    public bool Inventory_Enable = false;

    public int TopStage = 0;    //도달한 최고 스테이지

    public SaveData()
    {
        highscores = new List<ScoreData>();
    }
}


public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Insane,
}

public enum ScoreRank
{
    S = 0,
    A = 1,
    B = 2,
    C = 3,
    D = 4,
    Zero = 5,
}

public enum WeaponType
{
    Rifle = 0,
    Shotgun = 1, RevolverRifle = 2, Machinegun = 3,
    Stim = 4,
    Hook = 5,
    Cape = 6,
    Grenade = 7,
    Behemoth = 8,
}


