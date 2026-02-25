using MoreMountains.CorgiEngine;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private GameObject beltLeft;
    [SerializeField] private GameObject beltMiddle;
    [SerializeField] private GameObject beltRight;
    [SerializeField] private int generateCount;

    //컨베이어 벨트에 닿은 물체 이동
    [SerializeField] private Vector2 direction;

    private void Awake()
    {
        SetGridBelt();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out CorgiController controller))
        {
            if (controller.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            if (direction.x < 0)
            {
                if (!controller.State.IsCollidingAbove)
                {
                    collision.transform.Translate(direction * Time.deltaTime);
                }
            }
            else if (direction.x > 0)
            {
                if (!controller.State.IsCollidingAbove)
                    collision.transform.Translate(direction * Time.deltaTime);
            }
        }
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }

    [ContextMenu("SetGridBelt")]
    public void SetGridBelt()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        for (int i = 0; i < generateCount; i++)
        {
            if (i == 0)
                Instantiate(beltLeft, transform);
            else if (i == generateCount - 1)
                Instantiate(beltRight, transform);
            else
                Instantiate(beltMiddle, transform);

            print(transform.GetChild(i).name);
            transform.GetChild(i).localPosition = new Vector3(i, 0, 0);
        }

    }
}
