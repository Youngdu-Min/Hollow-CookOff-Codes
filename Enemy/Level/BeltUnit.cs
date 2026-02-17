using UnityEngine;

public class BeltUnit : MonoBehaviour
{

    //컨베이어 벨트에 닿은 물체 이동
    public Vector2 direction;
    private void Awake()
    {
        //ConveyorBeltManager.ContainUnit(this.gameObject,direction);
        //Debug.Log(transform.parent);
    }
    private void Start()
    {

        //ConveyorBeltManager.ContainUnit(this.gameObject, direction);

    }
}
