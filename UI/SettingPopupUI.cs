using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopupUI : MonoBehaviour
{
    public static SettingPopupUI Instance;


    private string settingsFilePath;

    private SettingOptions originSettings;
    private SettingOptions currentSettings;


    public Slider musicVolumeBar, SFXVolumeBar;
    public Toggle fullscreenToggle;


    public TMP_Dropdown resolutionDropdown;


    private Resolution[] resolutions;

    [SerializeField]
    private GameObject display;
    [SerializeField]
    private Image blockImg;
    [SerializeField]
    private AudioClip sampleSfx;
    private float lastPlayedSfxTime;

    public void OpenSettingUI(bool isActiveBlockImg)
    {
        display?.SetActive(true);
        blockImg.enabled = isActiveBlockImg;

        OpenDisplay();
    }

    public void CloseSettingUI()
    {
        display?.SetActive(false);
        blockImg.enabled = false;
        MMSoundManager.Instance.SetVolumeMusic(originSettings.musicVolume);
        MMSoundManager.Instance.SetVolumeSfx(originSettings.SfxVolume);
        LanguageSelector.Instance.OnLanguageChanged(originSettings.languageIndex);
    }
    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            currentSettings = new SettingOptions();
            settingsFilePath = Application.persistentDataPath + "/settings.json";
            LoadSettings();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        currentSettings = new SettingOptions();
        LoadSettings();

        if (resolutionDropdown != null)
        {
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            // 각 해상도를 드롭다운에 추가
            foreach (var res in resolutions)
            {
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(res.width + "x" + res.height));
            }
            resolutionDropdown.captionText.text = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
            // 드롭다운의 옵션에 대한 이벤트 리스너 추가
            resolutionDropdown.onValueChanged.AddListener(delegate
            {
                ChangeResolution(resolutionDropdown);
            });
        }

    }

    public void SetMasterVolume(float volume)
    {
        MMSoundManager.Instance.SetVolumeMaster(volume);
    }
    public void SetMusicVolume(float volume)
    {
        currentSettings.musicVolume = volume;
        MMSoundManager.Instance.SetVolumeMusic(volume);
    }
    public void SetSfxVolume(float volume)
    {
        currentSettings.SfxVolume = volume;
        MMSoundManager.Instance.SetVolumeSfx(volume);
        if(lastPlayedSfxTime + sampleSfx.length < Time.realtimeSinceStartup)
        {
            MMSoundManager.Instance.OnMMSfxEvent(sampleSfx);
            lastPlayedSfxTime = Time.realtimeSinceStartup;
        }
    }


    public void SetFullScreen(bool check)
    {
        currentSettings.fullscreen = check;
    }


    public void SaveSettings()
    {
        //현재 세팅정보 저장
        string json = JsonUtility.ToJson(currentSettings);
        System.IO.File.WriteAllText(settingsFilePath, json);
    }

    void LoadSettings()
    {
        //세팅 불러오기
        if (System.IO.File.Exists(settingsFilePath) && JsonUtility.FromJson<SettingOptions>(System.IO.File.ReadAllText(settingsFilePath)) != null)
        {
            string json = System.IO.File.ReadAllText(settingsFilePath);
            originSettings = JsonUtility.FromJson<SettingOptions>(json);
            if (currentSettings.fullscreen)
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, originSettings.fullscreen);
            MMSoundManager.Instance.SetVolumeMusic(originSettings.musicVolume);
            MMSoundManager.Instance.SetVolumeSfx(originSettings.SfxVolume);
        }
        else
        {
            // 파일이 없을 경우 기본 설정으로 초기화
            originSettings = new SettingOptions();
            SaveSettings();
        }

        Cursor.lockState = Screen.fullScreen ? CursorLockMode.Confined : CursorLockMode.None;

    }

    public void ApplySettings()
    {
        //소리
        MMSoundManager.Instance.SetVolumeMusic(currentSettings.musicVolume);

        MMSoundManager.Instance.SetVolumeSfx(currentSettings.SfxVolume);

        //풀스크린
        Screen.fullScreen = currentSettings.fullscreen;
        if (currentSettings.fullscreen)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, currentSettings.fullscreen);
        Cursor.lockState = Screen.fullScreen ? CursorLockMode.None : CursorLockMode.Confined;

        //언어

        originSettings.musicVolume = currentSettings.musicVolume;
        originSettings.SfxVolume = currentSettings.SfxVolume;
        originSettings.fullscreen = currentSettings.fullscreen;
        originSettings.languageIndex = (int)LanguageSelector.SelectedLanguage;
        SaveSettings();
    }


    public void ChangeResolution(TMP_Dropdown dropdown)
    {
        int resolutionIndex = dropdown.value;
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }

    public void OpenDisplay()
    {
        display.SetActive(true);
        UINavigation[] uINavigations = FindObjectsOfType<UINavigation>();
        uINavigations.ForEach(u => u.AddNavigationBlockObject(display));

        musicVolumeBar.value = originSettings.musicVolume;
        SFXVolumeBar.value = originSettings.SfxVolume;
        fullscreenToggle.isOn = originSettings.fullscreen;
    }

    public bool IsOpen()
        => display.activeSelf;
}


public class SettingOptions
{
    //음악
    public float musicVolume = 1f;
    public float SfxVolume = 1f;


    //풀스크린

    public bool fullscreen = true;

    //언어
    public int languageIndex;

}