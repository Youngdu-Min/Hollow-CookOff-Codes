using System;
using UnityEngine;

public class SceneSetter : MonoBehaviour
{
    [SerializeField][Range(0, 10)] private int setIdx;
    [field: SerializeField] private ObjectGroup[] objectGroups;
    private int lastIdx;

    [ContextMenu("Set Scene")]
    public void SetScene()
    {
        SetScene(setIdx);
    }

    public void SetScene(int idx)
    {
        if (idx > objectGroups.Length - 1)
        {
            Debug.LogError($"Out of range. Max value is {objectGroups.Length - 1}");
            return;
        }

        for (int i = 0; i < objectGroups.Length; i++)
        {
            for (int j = 0; j < objectGroups[i].objects.Length; j++)
            {
                objectGroups[i].objects[j].SetActive(false);
            }
        }
        for (int i = 0; i < objectGroups[idx].objects.Length; i++)
        {
            objectGroups[idx].objects[i].SetActive(true);
        }
        lastIdx = idx;
    }

}

[Serializable]
public class ObjectGroup
{
    public string groupName;
    public GameObject[] objects;
}
