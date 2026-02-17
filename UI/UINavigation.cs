using MoreMountains.Feedbacks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UINavigation : MonoBehaviour
{
    [SerializeField] private Selectable[] uiElements; // 이동할 UI 요소들을 여기에 할당합니다.
    private List<Selectable> selectables = new List<Selectable>();
    private int curIndex = 0;
    private int lastIndex = 0;

    [SerializeField] private bool canScroll = true;

    [SerializeField] private KeyCode[] positiveKeys, negativeKeys;

    private static UINavigation CurrentGroup; //현재 UI 그룹

    private static UINavigation DefaultGroup;
    [SerializeField] private bool isDefault = false; //최초 기본선택상태
    [SerializeField] private MMFeedbacks selectFeedbacks;
    [SerializeField] private GameObject[] changeNavigationBlockObjects;

    private async void OnEnable()
    {
        await Init();
        if (isDefault)
        {
            DefaultGroup = this;
            ChangeGroup(this);
        }
    }

    private async Task Init()
    {
        await Task.Yield();
        if (selectables.Count > 0)
            return;

        for (int i = 0; i < uiElements.Length; i++)
        {
            int index = i;
            if (uiElements[index].TryGetComponent(out Button btn))
            {
                if (!btn.interactable)
                    continue;
                selectables.Add(uiElements[index]);
            }
        }
    }

    public void AddNavigationBlockObject(GameObject obj)
    {
        if (changeNavigationBlockObjects == null)
            changeNavigationBlockObjects = new GameObject[1];
        else
            System.Array.Resize(ref changeNavigationBlockObjects, changeNavigationBlockObjects.Length + 1);

        for (int i = 0; i < changeNavigationBlockObjects.Length - 1; i++)
        {
            if (changeNavigationBlockObjects[i] == obj)
                return;
        }

        changeNavigationBlockObjects[changeNavigationBlockObjects.Length - 1] = obj;
    }

    public async void ChangeCurrentToThis()
    {
        if (selectables.Count == 0)
            await Init();
        ChangeGroup(this);
    }

    private void ChangeGroup(UINavigation uiNav)
    {
        CurrentGroup = uiNav;
        curIndex = 0;
        SelectElement();
    }

    void Update()
    {
        if (CurrentGroup == this)
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        for (int i = 0; i < changeNavigationBlockObjects.Length; i++)
        {
            if (!changeNavigationBlockObjects[i] || changeNavigationBlockObjects[i].activeInHierarchy)
                return;
        }

        // WASD 키 입력을 감지합니다.
        if (CheckContainKeyCodes(positiveKeys))
        {
            curIndex = GetNextEnabledIndex(curIndex, -1, canScroll);
            SelectElement();
        }
        else if (CheckContainKeyCodes(negativeKeys))
        {
            curIndex = GetNextEnabledIndex(curIndex, 1, canScroll);
            SelectElement();
        }

        bool CheckContainKeyCodes(KeyCode[] keyCodes)
        {
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(keyCodes[i]))
                    return true;
            }
            return false;
        }

        int GetNextEnabledIndex(int startIndex, int direction, bool canScroll)
        {
            int index = startIndex;
            int count = selectables.Count;
            do
            {
                if (canScroll)
                    index = (index + direction + count) % count;
                else
                    index = Mathf.Clamp(index + direction, 0, count - 1);
            } while (!selectables[index].gameObject.activeSelf); // Skip disabled game objects

            return index;
        }
    }

    void SelectElement()
    {
        selectables[curIndex].Select();
        if (selectFeedbacks != null && selectFeedbacks.gameObject.activeInHierarchy && curIndex != lastIndex)
            selectFeedbacks.PlayFeedbacks();
        lastIndex = curIndex;
        print($"{curIndex} {canScroll} 선택됨");

    }

    private void OnDisable()
    {
        ChangeGroup(DefaultGroup);
    }
}
