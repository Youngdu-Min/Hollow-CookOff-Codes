using MoreMountains.Tools;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class TargetTransform
{
    public Transform[] target;
}


/// <summary>
/// 옵젝 위치 자동으로 섞어주는 액션
/// </summary>
[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Random Generate")]
public class AIActionRandomGenerate : AIAction
{

    [SerializeField] TargetTransform[] targetTransforms;
    [SerializeField] float localPosX;
    [SerializeField] float lastPosX;
    [SerializeField] GameObject baseObj;

    /// <summary>
    /// On PerformAction we face and aim if needed, and we shoot
    /// </summary>
    public override void PerformAction()
    {

    }

    /// <summary>
    /// Faces the target if required
    /// </summary>

    public override void OnEnterState()
    {
        base.OnEnterState();
        baseObj.SetActive(true);
        Shuffle();


    }

    /// <summary>
    /// When exiting the state we make sure we're not shooting anymore
    /// </summary>
    public override void OnExitState()
    {
        base.OnExitState();
        baseObj.SetActive(false);
    }

    [ContextMenu("Shuffle")]
    // Random Shuffle GameObject Index
    void Shuffle()
    {
        lastPosX = 0;

        for (int i = targetTransforms.Length - 1; i >= 0; i--)
        {
            int r = Random.Range(0, i);
            TargetTransform tmp = targetTransforms[i];
            targetTransforms[i] = targetTransforms[r];
            targetTransforms[r] = tmp;
        }

        for (int poo = 0; poo < targetTransforms.Length; poo++)
        {
            if (lastPosX == 0)
                lastPosX = transform.position.x + localPosX;
            else
                lastPosX = lastPosX + localPosX;

            for (int bar = 0; bar < targetTransforms[poo].target.Length; bar++)
            {
                Transform target = targetTransforms[poo].target[bar];
                target.position = new Vector2(lastPosX, target.position.y);
            }
            Debug.Log($"지난 위치 {lastPosX}");
        }
    }
}
