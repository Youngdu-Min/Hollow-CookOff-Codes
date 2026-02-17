using UnityEngine;

[System.Serializable]
public class PortraitsDictionary : SerializableDictionary<string, Sprite> { }

[CreateAssetMenu(fileName = "new ChatDisplaryData", menuName = "Scriptable/ChatDisplaryData")]
public class ChatDisplayData : ScriptableObject
{
    [SerializeField]
    public PortraitsDictionary portriats;

}
