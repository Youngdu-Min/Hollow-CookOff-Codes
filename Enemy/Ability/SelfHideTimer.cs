using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfHideTimer : MonoBehaviour
{
    [SerializeField] private float waitHideTime;
    
    private void OnEnable()
    {
        StartCoroutine(HideTimer());
    }

    IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(waitHideTime);
        gameObject.SetActive(false);
    }
}
