using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouseEnter : MonoBehaviour
{
    public Text txt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PointerEnter()
    {
        txt.color = Color.yellow;
    }
    public void PointerExit()
    {
        txt.color = Color.white;
    }
}
