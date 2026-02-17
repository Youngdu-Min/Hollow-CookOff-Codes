using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class ChatEventTriger : MonoBehaviour
{
    public string triggerID;
    private bool triggered = false;
    [SerializeField] private WhileChatEvent[] defineWhileChatEvents;
    [SerializeField] private UnityEvent enterTriggerEvent;
    [SerializeField] private UnityEvent triggerEvent;
    [SerializeField] private UnityEvent defineEndChatEvent;
    [SerializeField] private float startDelay = 0f;

    /// <summary>
    /// 대화 이벤트 발동
    /// </summary>
    public IEnumerator TriggerChatting()
    {
        yield return new WaitForSeconds(startDelay);
        ChatManager.Instance.SetDialogue(triggerID);
        ChatManager.Instance.StartChat(defineWhileChatEvents, defineEndChatEvent);

        StartCoroutine(InvokeTriggerEvent());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered)
            return;

        if (collision.tag == "Player")
        {
            triggered = true;
            enterTriggerEvent?.Invoke();
            StartCoroutine(TriggerChatting());
        }
    }

    IEnumerator InvokeTriggerEvent()
    {
        yield return new WaitUntil(() => !ChatManager.Instance.chatUI.activeInHierarchy);
        triggerEvent?.Invoke();
    }
}
