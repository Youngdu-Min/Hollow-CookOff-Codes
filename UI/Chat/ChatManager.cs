using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public struct WhileChatEvent
{
    [SerializeField] private int chatIndex;
    [SerializeField] private UnityEvent chatEvent;
    public int ChatIndex => chatIndex;
    public UnityEvent ChatEvent => chatEvent;
}

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    public TextMeshProUGUI speakerTxt;
    public TextMeshProUGUI contentTxt;
    public Image portraitImage;
    public GameObject chatUI;

    private List<string> curDialogue = new List<string>();
    private int index = 0;

    public ChatDB chatDB;
    int lineSize, rowSize;

    public ChatDisplayData portsData; //초상화 데이터 주소

    public CharacterDatabase CharacterDB;

    public float timeForCharacter = 0.05f;

    public float timeForCharacterFast = 0.03f;

    private float characterTime = 0.05f;

    string typingDialog; //입력중인 대화내용의 완성본


    public bool isDialogEnd;

    public bool isTypeingEnd = false;
    private float timer = 0.05f;
    private Canvas[] hideCanvases;
    [SerializeField] private Canvas[] ignoreHideCanvases;
    private WhileChatEvent[] whileChatEvents;
    private UnityEvent endChatEvent;
    [SerializeField] private MMFeedbacks transitionFeedbacks;

    public void SetDialogue(string name)
    {
        //Debug.Log(name);
        var textAsset = Instance.chatDB.FindDialogueByName(name);

        curDialogue.Clear();
        curDialogue = StringToDialogues(textAsset.text);
    }


    public List<string> StringToDialogues(string text)
    {
        List<string> dialogues = new List<string>();

        string[] dialogueEntities = text.Split('\n');

        for (int i = 0; i < dialogueEntities.Length; i++)
        {
            dialogues.Add(dialogueEntities[i]);
        }

        return dialogues;


    }

    public void StartChat(string chatID)
    {
        SetDialogue(chatID);
        StartChat();

    }

    public void StartChat(WhileChatEvent[] defineWhileChatEvents = null, UnityEvent defineEndChatEvent = null)
    {
        whileChatEvents = defineWhileChatEvents;
        endChatEvent = defineEndChatEvent;
        hideCanvases = FindObjectsOfType<Canvas>();
        SetActiveOtherCanvas(false);
        if (MainCharacter.instance != null)
            MainCharacter.DisableControl();

        index = 0;

        PauseManager.Instance().PauseCall(Instance, true);
        //PauseManager.Instance().PauseGame();
        Instance.chatUI.SetActive(true);

        NextChat();
    }

    private void ExecuteWhileChatEvents()
    {
        if (whileChatEvents == null)
            return;

        for (int i = 0; i < whileChatEvents.Length; i++)
        {
            if (index == whileChatEvents[i].ChatIndex)
                whileChatEvents[i].ChatEvent.Invoke();
        }
    }

    private void SetActiveOtherCanvas(bool isTrue)
    {
        foreach (var target in hideCanvases)
        {
            if (target == null || ignoreHideCanvases.Contain(target))
                continue;

            target.enabled = isTrue;
        }
    }

    IEnumerator _typer;

    IEnumerator Typer(string chars, TextMeshProUGUI textObj)
    {
        int currentChar = 0;
        int charLength = chars.Length;
        typingDialog = chars;

        isTypeingEnd = false;
        textObj.text = ""; //기존 대화내용 초기화

        //print("대화 시작");
        while (currentChar < charLength)
        {
            yield return new WaitForSecondsRealtime(timeForCharacter);
            //html문장부호면 즉시 완성
            if (chars[currentChar] == '<')
            {
                do
                {
                    textObj.text += chars[currentChar].ToString();
                    currentChar++;
                    if (currentChar >= charLength)
                    {
                        isTypeingEnd = true;
                        yield break;
                    }
                } while (chars[currentChar] != '>');

            }
            textObj.text += chars[currentChar].ToString();
            currentChar++;

        }
        if (currentChar >= charLength)
        {
            isTypeingEnd = true;
            yield break;
        }

    }

    /// <summary>
    /// 입력중인 문장 즉시 완성
    /// </summary>
    public void EndTyping()
    {
        if (Instance._typer != null)
            Instance.StopCoroutine(Instance._typer);

        Instance.contentTxt.text = Instance.typingDialog;
        isTypeingEnd = true;
        transitionFeedbacks?.PlayFeedbacks();
    }


    public void ChangeLanguage(LanguageData language)
    {
        //        언어가 바뀌면 할 일
        // 1.대사 DB 갈아끼우기
        Instance.chatDB = language.dialogDB;

        // 2.해당언어로 폰트 바꾸기
        Instance.contentTxt.font = language.font;
        Instance.speakerTxt.font = language.font;

    }


    public void NextChat()
    {
        if (index < curDialogue.Count)
        {
            Instance.UpdateChat(curDialogue[index]);
            ExecuteWhileChatEvents();
            index++;
            if (index > 0)
                transitionFeedbacks?.PlayFeedbacks();
        }
        else
        {
            index = 0;
            EndChat();
        }

    }

    private void UpdateChat(string entity)
    {
        string[] _e = entity.Split('\t');
        string _speaker = _e[0].Trim();
        CharacterData speakerData;

        if (CharacterDB.characters.TryGetValue(_speaker, out speakerData))
        {
            speakerTxt.text = speakerData.GetNameByLanguage();
            portraitImage.sprite = speakerData.portrait;
        }
        else
        {
            speakerTxt.text = _speaker;
            portraitImage.sprite = null;
        }

        if (_typer != null)
        {
            StopCoroutine(_typer);
        }

        if (_e.Length < 2)
            return;
        string _content = _e[1];

        _typer = Typer(_content, contentTxt);
        StartCoroutine(_typer); //대화 한글자씩 출력
    }

    public void EndChat()
    {
        endChatEvent?.Invoke();
        SetActiveOtherCanvas(true);
        PauseManager.Instance().PauseCall(Instance, false);
        Instance.chatUI.SetActive(false);
        MainCharacter.AllowControl();

    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (Instance.chatUI.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            EndChat();
        }
    }
}
