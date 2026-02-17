using MoreMountains.Tools;
using UnityEngine;

/// <summary>
/// 옵젝 위치 자동으로 섞어주는 액션
/// </summary>
[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Set Target")]
public class AIActionSetTarget : AIAction
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
    }

    /// <summary>
    /// When exiting the state we make sure we're not shooting anymore
    /// </summary>
    public override void OnExitState()
    {
        base.OnExitState();
        baseObj.SetActive(false);
    }
}
