using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class ShootPrevent : MonoBehaviour
{
    public CharacterHandleWeapon handle;
    // Start is called before the first frame update
    void Start()
    {
        handle = gameObject.GetComponent<CharacterHandleWeapon>();
    }

    // Update is called once per frame
    void Update()
    {   //시간이 멈춰있을땐 총 안쏨
        if (Time.timeScale ==0)
        {
            handle.AbilityPermitted = false;
        }
        else
        {
            handle.AbilityPermitted = true;
        }
    }
}
