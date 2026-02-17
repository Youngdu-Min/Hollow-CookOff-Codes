using MoreMountains.CorgiEngine;
using UnityEngine;

public class ForceStopOnCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        print("Get zero 0");
        if (col.TryGetComponent(out CorgiController corgiController))
        {
            corgiController.SetForce(Vector2.zero);
            print("Get zero");
        }
    }
}
