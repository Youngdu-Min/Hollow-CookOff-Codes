using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu( fileName = "new DialogueDB", menuName = "Scriptable/new DialogueDB")]
public class ChatDB : ScriptableObject
{
    public List<TextAsset> dialogues = new List<TextAsset>();

    public TextAsset FindDialogueByName(string name)
    {

        TextAsset result = dialogues.FirstOrDefault(o => o.name == name);

        return result;

    }

}
