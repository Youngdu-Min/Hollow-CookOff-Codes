using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointDatasave : MonoBehaviour
{
    public float hp; //체력를 저장할 변수
    public float bio;//바이오에너지를 저장할 변수
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.tag == "Player")
        {
            this.gameObject.SetActive(false);

            hp = collider.GetComponent<PlayerHealth>().CurrentHealth; //  플레이어의 체력을 저장
            bio = collider.GetComponent<BioEnerge>().currentBE; // 플레이어의 바이오에너지를 저장

            mobjson.mj.Data.ps.hpsave = hp; // json에 데이터 저장
            mobjson.mj.Data.ps.bpsave = bio;
        }

    }
}
