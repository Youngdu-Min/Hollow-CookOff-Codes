using System;
using TMPro;
using UnityEngine;

public class LanguageText : MonoBehaviour
{
    [Serializable]
    private struct LanguageTextString
    {
        [SerializeField] private LanguageSelector.Language language;
        [SerializeField] private string textStr;

        public bool IsSameLanguage(LanguageSelector.Language _language)
            => language == _language;

        public string GetTextString()
            => textStr;
    }
    [SerializeField] private LanguageTextString[] languageTextStrings;
    private TMP_Text tmp;

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TMP_Text>();
        RefreshText();
        LanguageSelector.Instance.languageChangeAction += RefreshText;
    }
    void OnEnable()
    {
        RefreshText();
    }

    private void SetTextFont(TMP_FontAsset font)
        => tmp.font = font;

    private void RefreshText()
    {
        if (tmp == null)
            return;

        for (int i = 0; i < languageTextStrings.Length; i++)
        {
            if (languageTextStrings[i].IsSameLanguage(LanguageSelector.SelectedLanguage))
            {
                tmp.text = languageTextStrings[i].GetTextString().Replace("\\n", "\n");
                break;
            }
        }

        SetTextFont(LanguageSelector.Instance.currentLanguageData.font);
    }
}
