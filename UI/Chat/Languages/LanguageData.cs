using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LanguageData", menuName = "Scriptable/LanguageData")]
public class LanguageData : ScriptableObject
{
    public string nation;
    public TMPro.TMP_FontAsset font;
    public ChatDB dialogDB;

}
