using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class CustomActionEvent : MonoBehaviour
    {
        [SerializeField]
        AIBrain brain;

        [SerializeField]
        Transform originTarget;

        public void ChangeTarget(Transform target)
        {
            originTarget = brain.transform;
            brain.Target = target;
        }

        public void RollBackTarget()
        {
            brain.Target = originTarget;
        }
    }
}
