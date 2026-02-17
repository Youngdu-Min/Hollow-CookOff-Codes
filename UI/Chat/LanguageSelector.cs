using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public static LanguageSelector Instance;
    public static Language SelectedLanguage = 0;
    public enum Language
    {
        English,
        Korean,
    }

    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private LanguageData[] languages; // 언어 목록
    public LanguageData currentLanguageData => languages[(int)SelectedLanguage];
    public Action languageChangeAction;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        PopulateDropdown();
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        Initialize();
    }

    void PopulateDropdown()
    {
        List<string> _languages = new List<string>();

        for (int i = 0; i < languages.Length; i++)
        {
            _languages.Add(languages[i].nation);
        }
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(_languages);


    }

    public void OnLanguageChanged(int index)
    {
        print($"언어변경 {(Language)index}으로");
        SelectedLanguage = (Language)index;
        //챗매니저의 참조 DB,폰트 변경
        ChatManager.Instance.ChangeLanguage(Instance.languages[index]);
        //변경된 언어설정 저장
        PlayerPrefs.SetInt("LanguageIndex", index);
        Apply();
    }

    private void Initialize()
    {
        Apply();
    }

    public void Apply()
    {

        if (PlayerPrefs.HasKey("LanguageIndex"))
        {

            int languageIndex = PlayerPrefs.GetInt("LanguageIndex");

            //드롭다운 디폴트선택지 변경
            if (languageIndex != -1)
            {
                languageDropdown.value = languageIndex;
                languageDropdown.RefreshShownValue();
            }

            languageChangeAction?.Invoke();
        }
        else
        {
            // 저장된 언어가 없을 경우
        }
    }
}


