using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("충돌");
            GameObject.Find("playerGM").GetComponent<PlayerC>().Checkpoint = transform; // playerGM오브젝트를 찾아 체크포인트 위치 설정( 나중에 public으로 게임오브젝트 받아서 해도됨)
        }
    }
}
