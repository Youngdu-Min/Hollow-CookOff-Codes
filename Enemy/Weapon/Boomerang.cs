using UnityEngine;

public class Boomerang : MonoBehaviour
{
    Vector3 endPos;
    [SerializeField]
    Transform centerPos;
    [SerializeField]
    Transform owner;
    [SerializeField]
    float distance;
    [SerializeField, Range(0, 1)]
    float decSpeedRatio;
    [SerializeField]
    float distRatio;
    [SerializeField]
    float speedDivide;
    float currSpeed;
    float initialCurrSpeed;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    float rotSpeed;

    float DistRatio
    {
        get
        {
            return distRatio;
        }
        set
        {
            distRatio = value;
            if (decSpeedRatio < distRatio)
            {
                currSpeed = initialCurrSpeed / 2;
            }
            else
            {
                currSpeed = initialCurrSpeed;
            }

            if (distRatio > 1)
                isReturn = true;
            if (isReturn && distRatio < 0)
                gameObject.SetActive(false);
        }
    }

    bool isReturn;

    private void OnEnable()
    {
        isReturn = false;
        DistRatio = 0;
        spriteRenderer.transform.rotation = Quaternion.identity;
        transform.position = owner.position;
        SetEndPos();
    }

    // Start is called before the first frame update
    void Start()
    {

        initialCurrSpeed = Time.deltaTime / speedDivide;
        currSpeed = initialCurrSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime == 0)
            return;

        if (!isReturn)
        {
            DistRatio += currSpeed;
            spriteRenderer.transform.Rotate(new Vector3(0, 0, 360 * currSpeed * rotSpeed) * Time.deltaTime, Space.Self);
        }
        else
        {
            DistRatio -= currSpeed;
            spriteRenderer.transform.Rotate(new Vector3(0, 0, -(360 * currSpeed * rotSpeed)) * Time.deltaTime, Space.Self);
        }

        transform.position = Vector3.Lerp(owner.position, endPos, distRatio);
    }

    void SetEndPos()
    {
        if (centerPos.position.x > owner.position.x)
        {
            endPos = owner.position + new Vector3(distance, 0, 0);
        }
        else
        {

            endPos = owner.position + new Vector3(-distance, 0, 0);
        }
    }
}
