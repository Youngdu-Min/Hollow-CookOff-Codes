using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningObject : MonoBehaviour
{
    public float spinSpeed = 60;//초당 회전속도
    public bool canSpin =true;
    

    void Update()
    {
        if (Time.timeScale !=0 && spinSpeed != 0 && canSpin)
        {
            transform.Rotate( new Vector3(0, 0, spinSpeed) * Time.deltaTime);
        }
    }

    public void SetSpinable(bool spin)
    {
        canSpin = spin;
    }

}
