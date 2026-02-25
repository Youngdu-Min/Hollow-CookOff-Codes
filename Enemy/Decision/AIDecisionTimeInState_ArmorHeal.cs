using System.Collections;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{

    public class AIDecisionTimeInState_ArmorHeal : AIDecisionTimeInState
    {
        HealthExpend health;
        AIBrain_ArmorFind _brain_armor;
        bool timeEnd = true; // 시간 초과로 종료 됐는지 확인
        float saveTime; // 초과로 종료되지 않았으면 남은 시간 저장

        public override void Initialization()
        {
            base.Initialization();
            _brain_armor = GetComponent<AIBrain_ArmorFind>();
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _brain.FindTarget_OrderDistance();
        }


        protected override bool EvaluateTime()
        {
            if (_brain == null) { return false; }

            if (_brain.TimeInThisState >= _randomTime)
            {
                timeEnd = true;
                return true;
            }
            else
            {
                timeEnd = false;
                saveTime = _randomTime - _brain.TimeInThisState;
                return false;
            }
        }

        protected override void RandomizeTime()
        {
            if (!timeEnd)
                _randomTime = saveTime;
            else
                _randomTime = EnemyBalance.etc.etcList[1].floatValue;
            /*
            if(_brain.Target != null)
            {
                Debug.Log("뇌 " + _brain.Target);
                line.points[1] = _brain.Target;

                health = _brain.Target.GetComponent<HealthExpend>();
                if (health == null)
                    health = _brain.Target.parent.GetComponent<HealthExpend>();
                if (health == null)
                    health = _brain.Target.parent.parent.GetComponent<HealthExpend>();

                health.currentArmor = health.inintialArmor;

            }
            else
            {
                _randomTime = 0;
                Debug.Log("랜덤 타임. " + _randomTime);
            }*/
        }

        public override void OnExitState()
        {
            DecisionInProgress = false;
            if (_brain_armor.armorTarget != null && !IsInvoking(nameof(ArmorHeal)))
            {
                Invoke(nameof(ArmorHeal), saveTime);
            }
        }

        IEnumerator waitHeal(Transform obj, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            health = obj.GetComponent<HealthExpend>();
            if (health == null)
                health = obj.parent.GetComponent<HealthExpend>();
            if (health == null)
                health = obj.parent.parent.GetComponent<HealthExpend>();

            health.currentArmor = health.initialArmor;
        }

        void ArmorHeal()
        {
            if (_brain_armor.armorTarget == null)
                return;

            health = _brain_armor.armorTarget.GetComponent<HealthExpend>();
            if (health == null)
                health = _brain_armor.armorTarget.parent.GetComponent<HealthExpend>();
            if (health == null)
                health = _brain_armor.armorTarget.parent.parent.GetComponent<HealthExpend>();

            health.currentArmor = health.initialArmor;
        }
    }
}
