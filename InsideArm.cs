using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideArm : MonoBehaviour
{
    //안쪽 팔 움지이는 스크립트

    [SerializeField]
    private SpriteRenderer armSprite;

    [SerializeField]
    private Transform armTransform,flipTransform;

    [SerializeField]
    private Transform targetTransform;

    public float armAngle;

    [SerializeField]
    private bool flipped = true;
    //팔 스프라이트 끄고 켜기
    public bool SetSprite(bool active)
    {
        armSprite.gameObject.SetActive(active);
        return active;
    }

    //좌우반전 끄고 켜기
    public bool XFlipOn(bool active)
    {
        if (active)
            flipTransform.localScale = new Vector3(-1f, 1f, 1f);
        else
            flipTransform.localScale = new Vector3(1f, 1f, 1f);
        flipped = active;
        return active;
    }
    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetSprite(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (armTransform.gameObject.activeSelf && targetTransform != null)
        {
            armTransform.localRotation = targetTransform.rotation;

            armAngle = armTransform.eulerAngles.z;

            if (flipped)
            {
                
                if (armAngle <= 90f || armAngle >= 270f)
                {
                    armSprite.flipY = false;
                }
                else
                {
                    armSprite.flipY = true;
                }
            }
        }
    }
}
