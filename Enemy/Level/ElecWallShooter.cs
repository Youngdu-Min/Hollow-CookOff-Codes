using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecWallShooter : MonoBehaviour
{
    //전기벽 테스트용 스크립트

     IEnumerator Cycle()
    {
        int count = 0;
        while (true)
        {
            yield return new WaitForSeconds(1f);
           var newObj = ElectricWallManager.CreateNewWall(transform.position+ transform.up * count);
            newObj.SetSpeed(transform.right);
            newObj.SetLifeTime(4);
            count++;

        }
    }
    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(Cycle());
    }
    
}
