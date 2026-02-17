using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomButtonShape : MonoBehaviour
{
    private void Start()
    {
        //투명도가 0.1보다 낮으면 클릭안됨
        //소스이미지의 Read/Write Enabled을 활성화할것
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}
