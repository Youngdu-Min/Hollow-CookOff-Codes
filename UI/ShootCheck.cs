using MoreMountains.Feedbacks;
using UnityEngine;

public class ShootCheck : MonoBehaviour
{
    [SerializeField] private MMFeedbacks shootTailFeedbacks;

    public void Shoot(int index)
    {
        OverHeatUI.Heat(index);
    }

    public void PlayShootTailFeedbacks()
    {
        if (shootTailFeedbacks == null)
            return;

        if (!OverHeatUI.isCookOff)
        {
            shootTailFeedbacks.PlayFeedbacks();
        }
    }
}
