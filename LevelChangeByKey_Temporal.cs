using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class LevelChangeByKey_Temporal : MonoBehaviour
    {
        public string str;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.J))
            {
                LevelManager.Instance.GotoLevel(str);
            }
        }
    }
}
