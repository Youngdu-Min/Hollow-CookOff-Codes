using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageSelectButton : MonoBehaviour,ISelectHandler
{



    public int stage = 0;

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        StageSelectUI.Instance.curStage = stage;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
