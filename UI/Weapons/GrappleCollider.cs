using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class GrappleCollider : MonoBehaviour
    {
        public GrapplingGun grapplingGun;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // grapplingGun.enemyGrab = false;
            StartCoroutine(grapplingGun.GrappleOff()); 
        }
    }
}
