using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Area Data", menuName = "Scriptable/Area Data")]
public class AreaData : ScriptableObject
{
    [Serializable]
    public struct StageData
    {
        [SerializeField] private string sceneName;
        public string SceneName => sceneName;
        [SerializeField] private bool isCutscene;
        public bool IsCutscene => isCutscene;
    }

    [field: SerializeField] private StageData[] stages = null;
    public StageData[] Stages => stages;
}
