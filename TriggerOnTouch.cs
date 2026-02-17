using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnTouch : MonoBehaviour
{

    public List<GameObject> activeObjects = new List<GameObject>();

    public List<GameObject> inactiveObjects = new List<GameObject>();


    public void Activate()
    {
        foreach (var item in activeObjects)
        {
            item.SetActive(true);
        }

        foreach (var item in inactiveObjects)
        {
            item.SetActive(false);
        }
    }


    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Activate();
        }
    }
}
