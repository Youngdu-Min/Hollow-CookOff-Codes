using System;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
    [Serializable]
    public struct LanguageImage
    {
        [SerializeField] private LanguageSelector.Language language;
        [SerializeField] private Sprite[] images;

        public bool IsSameLanguage(LanguageSelector.Language _language)
            => language == _language;

        public Sprite[] GetImages()
            => images;
    }
    [SerializeField] private LanguageImage[] languageImages;
    [SerializeField] private UnityEvent triggerEvent;

    //튜토리얼 트리거 박스
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        for (int i = 0; i < languageImages.Length; i++)
        {
            if (languageImages[i].IsSameLanguage(LanguageSelector.SelectedLanguage))
            {
                Tutorial.Instance.StartTutorial(languageImages[i].GetImages(), triggerEvent);
                break;
            }
        }

        this.gameObject.SetActive(false);
    }

}
