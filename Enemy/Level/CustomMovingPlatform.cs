using UnityEngine;
using static System.Math;

public class CustomMovingPlatform : MonoBehaviour
{
    ElectricWall electricWall;
    [SerializeField] private float startRot;
    [SerializeField] private float endRot;
    [SerializeField] private bool permitBlink;
    [SerializeField] private float perHideAngle;
    [SerializeField] private float afterHideEnableAngle;
    private float lastHideAngle;
    private bool isHide;
    private float currLength;
    [SerializeField]
    private float lerpTime = 1f;
    [SerializeField]
    private float currentTime = 0;
    private float endLength;

    enum State
    {
        Enable,
        Disable
    }
    State state;

    void Awake()
    {
        electricWall = GetComponent<ElectricWall>();
        endLength = electricWall.length;
        electricWall.SetActiveBox_N_Sprite(false);
        electricWall.SetLength(0, 0);
    }


    [ContextMenu("SetStartRot")]
    void OnEnable()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, startRot);
        lastHideAngle = startRot;
        state = State.Enable;
        currentTime = 0;
        electricWall.SetActiveBox_N_Sprite(true);
    }

    // Update is called once per frame
    void Update()
    {
        LerpPlatform();
        //Debug.Log($"{gameObject} | {transform.eulerAngles.z} | {endRot}");
        //Debug.Log($"{gameObject} Angle {Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, lastHideAngle))} | {perHideAngle}");

        if (permitBlink)
        {
            if (!isHide && Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, lastHideAngle)) > perHideAngle)
            {
                lastHideAngle = transform.eulerAngles.z;
                electricWall.SetActiveBox_N_Sprite(false);
                isHide = true;

            }
            else if (isHide && Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, lastHideAngle)) > afterHideEnableAngle)
            {
                lastHideAngle = transform.eulerAngles.z;
                electricWall.SetActiveBox_N_Sprite(true);
                isHide = false;
            }

        }


        if (Truncate(transform.eulerAngles.z) == Truncate(endRot))
        {
            state = State.Disable;
        }
    }

    void LerpPlatform()
    {
        if (state == State.Enable)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= lerpTime)
            {
                currentTime = lerpTime;
                electricWall.SetRotate(true);
            }
            else
            {
                electricWall.SetRotate(false);
            }

        }
        else
        {
            electricWall.SetRotate(false);
            currentTime -= Time.deltaTime;

            if (currentTime < 0)
            {
                currentTime = 0;
            }
        }

        currLength = Mathf.Lerp(0, endLength, currentTime / lerpTime);
        electricWall.SetLength(currLength, currLength);

        if (currLength == 0)
            gameObject.SetActive(false);

        //Debug.Log($"길이 {currLength} {electricWall.leftOffset}, {currRightOffset} {electricWall.rightOffset} | {currentTime / lerpTime}");
    }
}
