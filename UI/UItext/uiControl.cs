using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.CorgiEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea]
    public string dialogue;

}
public class uiControl : MonoBehaviour
{
    [SerializeField] public GameObject player;

    [SerializeField] public GameObject txt_Dialogue;
    [SerializeField] public Text txt;
    [SerializeField] public Camera playercm;
    [SerializeField] public Camera eventcm;

    [SerializeField] public GameObject eventpos;
    private bool isDialogue = false;
    private bool isback = false;
    public Transform startpos;
    private int count = 0;
    public GameObject dgm;


    public Dialogue[] dialogue;
    void Start()
    {

    }
    private void Update()
    {
        if (isDialogue == true)
        {
            eventcm.transform.position = Vector3.Lerp(eventcm.transform.position, eventpos.transform.position, Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                count = dialogue.Length;
                onButtonClick();
            }

        }
        if (isDialogue == false && isback == true)
        {
            Debug.Log("시작");
            eventcm.transform.position = Vector3.Lerp(eventcm.transform.position, startpos.transform.position, Time.deltaTime);

        }

    }
    public void ShowDialogue()
    {


        GameObject.Find("Corgi").GetComponent<CorgiController>().enabled = false;
        txt_Dialogue.gameObject.SetActive(true);
        count = 0;
        isDialogue = true;
        NextDialogue();

    }
    public void HideDialogue()
    {
        GameObject.Find("Corgi").GetComponent<CorgiController>().enabled = true;


        isback = true;
        txt_Dialogue.gameObject.SetActive(false);
        count = 0;
        isDialogue = false;
        Destroy(dgm);
        


    }



    public void NextDialogue()
    {

        txt.text = dialogue[count].dialogue;
        count++;
    }

    public void onButtonClick()
    {
        if (count < dialogue.Length )
        {
            Debug.Log("클릭");
            Debug.Log(count);
            NextDialogue();

        }
        else
        {

            HideDialogue();

        }
    }
  

 
}
