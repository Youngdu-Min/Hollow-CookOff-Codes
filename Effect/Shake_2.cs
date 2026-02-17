using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;


public class Shake_2 : MonoBehaviour
{
    public Transform shakeCamera;
    public bool shakeRotate = false;
    public Vector3 originPos;
    private Quaternion originRot;
    public Vector3 shakePos;
    // Start is called before the first frame update
    void Start()
    {
        /*
         * 
         * 저번에 말한 화면 흔들림
할로우가 데미지를 입었을때
슬래시 성공
에어본 타격
베히모스 발사 시
B.E 익스플로전 사용 시
         * */
    }


    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float magnitudeRot = 0.01f)
    {
        float passTime = 0.0f;
        while (passTime < duration)
        {
            shakePos = new Vector3(originPos.x + 0.5f, originPos.y + 0.5f, originPos.z + 0.5f);
            shakeCamera.localPosition = shakePos;
            if (shakeRotate)
            {
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            passTime += Time.deltaTime;

            yield return null;
        }

        shakeCamera.localRotation = originRot;
        shakeCamera.localPosition = originPos;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            originPos = shakeCamera.localPosition;
            originRot = shakeCamera.localRotation;
            StartCoroutine(ShakeCamera());
        }
    }

    public void shakeStart()
    {
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
        StartCoroutine(ShakeCamera());
    }

}
