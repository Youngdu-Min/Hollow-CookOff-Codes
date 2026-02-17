using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOnclickFeedbacks : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private MMFeedbacks feedbacks;

    // Start is called before the first frame update
    void Start()
    {
        buttons.ForEach(button => button.onClick.AddListener(() => feedbacks.PlayFeedbacks()));
    }
}
