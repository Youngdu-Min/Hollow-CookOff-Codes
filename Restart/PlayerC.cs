using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerC : MonoBehaviour
{
    public Transform Checkpoint;
   // public GameObject Hero;
    private Health health;
    public GameObject start;
    public GameObject restartUI;
    public Transform tr;


    void Start()
    {
        health = GameObject.Find("Player").GetComponent<Health>(); // public 게임오브젝트로 받아 Find사용안하고도 가능

        Checkpoint= start.transform; // 초기 체크포인트 위치 설정


       tr = GameObject.Find("Player").GetComponent<Transform>(); 

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.T)) //리스폰 테스트를 위한 코드 T를 누르면 체력이 감소함
        {
            health.CurrentHealth -= 50;
        }
        if (health.CurrentHealth <= 0)
        {
           
            GameObject.Find("Player").GetComponent<Character>().enabled = false; //스크립트 비활성화
            GameObject.Find("Player").GetComponent<CorgiController>().enabled = false; //스크립트 비활성화

            restartUI.SetActive(true); //R키를 누르라는 UI 활성화
            if (Input.GetKeyDown(KeyCode.R)) //R키를 누르면 캐릭터 위치를 체크포인트 위치로 이동시킴
            {
                tr.position=Checkpoint.position;
                restartUI.SetActive(false);
                GameObject.Find("Player").SetActive(true);
                GameObject.Find("Player").GetComponent<Character>().enabled = true; //스크립트 비활성화
                GameObject.Find("Player").GetComponent<CorgiController>().enabled = true; //스크립트 비활성화
                Debug.Log("클릭");
                //Instantiate(Hero, Checkpoint, Quaternion.identity);
                health.CurrentHealth = 300; //체력 초기화

            }
        }
        
    }
    /*
    void Death()
    {
        spawn();
    }

    void spawn()
    {
       
        SceneManager.LoadScene("restartTEST");
    }*/
}
