using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterDictionary : SerializableDictionary<string, CharacterData> { }

[CreateAssetMenu(fileName = "Character Database", menuName = "Scriptable/Character Database")]
public class CharacterDatabase : ScriptableObject
{

    [SerializeField]
    public CharacterDictionary characters;

}

[System.Serializable]
public class CharacterData
{
    public List<string> names = new List<string>();
    public Sprite portrait;

    public string GetNameByLanguage()
    {
        return names[(int)LanguageSelector.SelectedLanguage];
    }

}
