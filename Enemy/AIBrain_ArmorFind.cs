using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// the AI brain is responsible from going from one state to the other based on the defined transitions. It's basically just a collection of states, and it's where you'll link all the actions, decisions, states and transitions together.
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/AI/AIBrain Armor Find")]
    public class AIBrain_ArmorFind : AIBrain
    {
        DrawLine line;
        [HideInInspector]
        public Transform armorTarget;

        protected override void Start()
        {
            line = this.gameObject.GetComponent<DrawLine>();
            line.enabled = false;
            ResetBrain();
            FindTarget_OrderDistance();

        }

        public override void FindTarget_OrderDistance()
        {
            Target = null;
            float dist = 100;
            GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
            foreach (GameObject go in gos)
            {
                HealthExpend armor = go.GetComponent<HealthExpend>();
                if (armor == null || armor.maxArmor <= 0)
                {
                    continue;
                }
                if (go.layer == LayerMask.NameToLayer(TargetLayer) && go.gameObject != this.gameObject && armor.currentArmor < armor.maxArmor)
                {
                    float currdist = Vector3.Distance(this.gameObject.transform.position, go.gameObject.transform.position);
                    if (currdist < dist)
                    {
                        Target = go.transform;
                        armorTarget = go.transform;
                        dist = currdist;
                    }
                }
            }

            if (Target == null)
                FindTarget();
            else
            {
                line.enabled = true;
                line.points[1] = Target;
            }
        }

        public override void FindTarget()
        {
            line.enabled = false;
            GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
            foreach (GameObject go in gos)
            {
                if (go.layer == LayerMask.NameToLayer("Player") && go.gameObject != this.gameObject)
                {
                    Target = go.transform;
                    break;
                }
            }
            TransitionToState("AlterFollowing");
        }

        public override void SetBrainActive(bool status)
        {
            base.SetBrainActive(status);
            if (status == false)
            {
                line.enabled = false;
                armorTarget = null;
            }
        }
    }
}
