using TMPro;
using UnityEngine;

public class DialogDisplayControler : MonoBehaviour
{
    //대화이벤트 도중 컨트롤하는 스크립트

    private string fullText;
    private bool isTyping = false;

    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f; // 글자가 출력되는 속도


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ChatManager.Instance.isTypeingEnd)
            {
                ChatManager.Instance.NextChat();
            }
            else
            {
                ChatManager.Instance.EndTyping();
            }
        }

    }
}
