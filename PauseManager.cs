using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance;
    public static PauseManager Instance()
    {
        if (instance == null)
        {

        }
        return instance;
    }

    private bool isPaused;

    [SerializeField] private GameObject pausedmenu;
    [SerializeField] private GameObject player;

    private Dictionary<object, bool> pauseCalled = new Dictionary<object, bool>();
    private float lastTimeScale;
    [SerializeField] private GameObject[] playSceneOnlyObjects;
    [SerializeField] private GameObject[] exceptBossSceneObjects;
    [SerializeField] private TextMeshProUGUI bestTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscClick();
        }
    }



    public void EscClick()
    {
        if (SettingPopupUI.Instance.IsOpen())
            return;
        //메인메뉴에서는 일시정지 기능 안함
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("main") || sceneName.Contains("Main") || sceneName.Contains("Loading"))
            return;

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (instance.pauseCalled.ContainsValue(true))
            return;

        isPaused = true;
        lastTimeScale = Time.timeScale;
        PauseCall(this, true);
        pausedmenu.gameObject.SetActive(true);
        StageSelectUI.Instance?.SetBestTime(ref bestTime);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = lastTimeScale;
        PauseCall(this, false);
        pausedmenu.gameObject.SetActive(false);
    }

    private static void _restart()
    {
        //현재 씬 재시작
        instance.ResumeGame();
        print("현재 씬 재시작");
        MMSceneLoadingManager.LoadScene(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Restart()
    {
        _restart();
    }

    public void RestartAtCheckPoint()
    {
        ResumeGame();

        pausedmenu.gameObject.SetActive(false);
        if (player == null)
        {
            var characters = FindObjectsOfType<Character>();
            foreach (var character in characters)
            {
                if (character.CharacterType == Character.CharacterTypes.Player)
                    player = character.gameObject;
            }
        }

        pausedmenu.gameObject.SetActive(false);//메뉴를 닫음
        player.GetComponent<PlayerHealth>().CurrentHealth = (int)mobjson.mj.Data.ps.hpsave; // 현재 체력을 체크포인트때의 체력으로 돌아감
        player.GetComponent<BioEnerge>().currentBE = (int)mobjson.mj.Data.ps.bpsave; // 현재 bp포인트를 체크포인트때의 포인트로 돌아감
        player.transform.position = LevelManager.Instance.CurrentCheckPoint.transform.position; // 플레이어를 체크포인트 위치로 보냄

        MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing); // 카메라가 플레이어 따라 이동하게함
        CamLock[] camLocks = FindObjectsOfType<CamLock>();
        camLocks.ForEach(x => x.AbleSpawnReset());

    }
    public void ChangeScene(string sceneName)
    {
        MMSceneLoadingManager.LoadScene(sceneName);
    }

    public void OpenSettings()
    {
        SettingPopupUI.Instance.OpenSettingUI(false);
    }

    public void PauseCall(object source, bool pause)
    {
        if (instance.pauseCalled.ContainsKey(source))
        {
            Debug.Log(source + " wants " + pause);
            instance.pauseCalled[source] = pause;
        }
        else
        {
            Debug.Log(source + " wants " + pause);
            instance.pauseCalled.Add(source, pause);
        }

        bool isPauseCalled = instance.pauseCalled.ContainsValue(true);
        if (Time.timeScale > 0 && isPauseCalled)
            lastTimeScale = Time.timeScale;
        Time.timeScale = (isPauseCalled) ? 0 : lastTimeScale;

        if (isPauseCalled)
            MainCharacter.DisableControl();
        else
            MainCharacter.AllowControl();

        DefineWeaponSelectActive();

        void DefineWeaponSelectActive()
        {
            bool isActive = true;
            foreach (KeyValuePair<object, bool> pair in instance.pauseCalled)
            {
                if (!pair.Value)
                    continue;
                if (pair.Key is WeaponSelectUI)
                    continue;
                isActive = false;
                break;
            }
            WeaponSelectUI.Instance()?.SetActivate(isActive);
        }
    }

    public bool IsPauseCalled()
        => instance.pauseCalled.ContainsValue(true);

    public bool IsPauseSourceCalled(object source)
    {
        if (source == null)
        {
            print("source is null");
            return false;
        }

        bool b = instance.pauseCalled.ContainsKey(source);
        print($"{source} {b}");
        return b ? instance.pauseCalled[source] : false;
    }

    public void SetActiveTimeObjectParent(bool isTrue)
        => playSceneOnlyObjects.ForEach(x => x.SetActive(isTrue));

    public void SetActiveExceptBossSceneObjects(bool isTrue)
        => exceptBossSceneObjects.ForEach(x => x.SetActive(isTrue));
}
