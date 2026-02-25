using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneManager : MonoBehaviour
{
    [Serializable]
    public struct CutsceneInfo
    {
        [SerializeField] private UnityEvent[] cutsceneEvent;
        [SerializeField] private float duration;
        public UnityEvent[] CutsceneEvent => cutsceneEvent;
        public float Duration => duration;
    }

    [SerializeField] private CutsceneInfo[] cutsceneSequence;

    public void StartCutscene()
    {
        StartCoroutine(PlayCutscene());
    }

    public IEnumerator PlayCutscene(int index = 0)
    {
        foreach (var unityEvent in cutsceneSequence[index].CutsceneEvent)
            unityEvent.Invoke();

        yield return new WaitForSeconds(cutsceneSequence[index].Duration);

        if (index + 1 < cutsceneSequence.Length)
            StartCoroutine(PlayCutscene(index + 1));
    }
}
