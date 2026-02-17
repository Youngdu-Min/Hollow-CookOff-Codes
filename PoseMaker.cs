using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
public class PoseMaker : MonoBehaviour
{
    //무기 각도에따라 팔 자세 바꿔주는 스크립트
    public GameObject body, armAngle,bodyAngle;
    [Header("Left Arm")]
    public GameObject shoulderL;
    public GameObject handL, elbowL, upperArmL, foreArmL;
    [Header("Right Arm")]
    public GameObject shoulderR;
    public GameObject handR, elbowR, upperArmR, foreArmR;

    public Vector2 handPos_L, handPos_R;
    public float ArmLength;

    private SpriteRenderer characterSprite;
    private int flip;

    public MoreMountains.CorgiEngine.CharacterHandleWeapon weaponHandler;

    public void UpdatePose()
    {

        //팔 각도 산출
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(
            mousePosition.x - body.transform.position.x,
            mousePosition.y - body.transform.position.y
            );

        if (weaponHandler.CurrentWeapon.GetComponent<WeaponAim>() != null)
        {
            Debug.Log(weaponHandler.CurrentWeapon.GetComponent<WeaponAim>().AimControl);
        }
        armAngle.transform.right = direction;
        //좌우반전확인
        if (mousePosition.x > body.transform.position.x)
        {
            characterSprite.flipX = false;
            flip = 1;
        }
        else
        {
            characterSprite.flipX = true;
            flip = -1;
        }

        //몸통 각도 계산
       //bodyAngle.transform.rotation = Quaternion.Euler(0, 0, armAngle.transform.rotation.z * 20f);
        //어깨위치 계산. 위치는 임의로넣은거
        shoulderL.transform.localPosition = new Vector2(0.5f * flip, 0.28f);
        shoulderR.transform.localPosition = new Vector2(-0.5f * flip, 0.31f);

        //손 위치 계산
        handL.transform.localPosition = new Vector2(handPos_L.x, handPos_L.y * flip);
        if (handPos_L.x > ArmLength*2)
        {
            handPos_L.x = ArmLength * 2;
        }
        handR.transform.localPosition = new Vector2(handPos_R.x,handPos_R.y*flip);
        
        //팔꿈치 위치 계산
        elbowL.transform.localPosition = new Vector2(
            handPos_L.x*0.5f,
            - Mathf.Sqrt((ArmLength * ArmLength) - (handPos_L.x * handPos_L.x * 0.25f) - handPos_L.y*handPos_L.y) * flip
            );
        elbowR.transform.localPosition = new Vector2(
            handPos_R.x * 0.5f,
            -Mathf.Sqrt((ArmLength * ArmLength) - (handPos_R.x * handPos_R.x * 0.25f) - handPos_L.y * handPos_L.y) * flip
            );

        //팔뚝 위치 계산

        upperArmR.transform.position = shoulderR.transform.position;
        upperArmR.transform.right = elbowR.transform.position - upperArmR.transform.position;//오른팔 위

        foreArmR.transform.position = elbowR.transform.position;
        foreArmR.transform.right = handR.transform.position - foreArmR.transform.position;  //오른팔 아래

        upperArmL.transform.position = shoulderL.transform.position;
        upperArmL.transform.right = elbowL.transform.position - upperArmL.transform.position; //왼팔 위

        foreArmL.transform.position = elbowL.transform.position;
        foreArmL.transform.right = handL.transform.position - foreArmL.transform.position; //왼팔 아래


    
    }

    private void Awake()
    {
        characterSprite = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {

        body = gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
            UpdatePose();
    }
}
