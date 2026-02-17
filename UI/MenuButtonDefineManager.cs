using UnityEngine;
using UnityEngine.UI;

public class MenuButtonDefineManager : MonoBehaviour
{
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button levelsBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button quitBtn;

    private void Start()
    {
        settingsBtn?.onClick.AddListener(() => SettingPopupUI.Instance.OpenSettingUI(true));
        if (continueBtn)
            continueBtn.interactable = SaveDataManager.Instance.TopStage() > 0;
    }

}
