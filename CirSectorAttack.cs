using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class CirSectorAttack : MonoBehaviour
    {
        [SerializeField] private bool m_bDebugMode = false;

        [Header("View Config")]
        [Range(0f, 360f)]
        [SerializeField] private float m_horizontalViewAngle = 0f; // 시야각
        [SerializeField] private float m_viewRadius = 1f; // 시야 범위
        [Range(-180f, 180f)]
        [SerializeField] private float m_viewRotateZ = 0f; // 시야각의 회전값

        [SerializeField] private LayerMask m_viewTargetMask;       // 인식 가능한 타켓의 마스크
        [SerializeField] private LayerMask m_viewObstacleMask;     // 인식 방해물의 마스크 

        private List<Collider2D> hitedTargetContainer = new List<Collider2D>(); // 인식한 물체들을 보관할 컨테이너

        private float m_horizontalViewHalfAngle = 0f; // 시야각의 절반 값

        public AIDecisionDetectTargetRadius circle;
        Character character;
        float currViewRotateZ;

        private void Start()
        {
            m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;
            character = GetComponent<Character>();

            if (character == null)
            {
                character = transform.parent.GetComponent<Character>();
            }

            circle.onHit += () => FindViewTargets();
        }

        private void OnDrawGizmos()
        {
            if (m_bDebugMode)
            {
                m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;

                Vector3 originPos = transform.position;

                Gizmos.DrawWireSphere(originPos, m_viewRadius);

                Vector3 horizontalRightDir = AngleToDirZ(m_horizontalViewHalfAngle + currViewRotateZ);
                Vector3 horizontalLeftDir = AngleToDirZ(-m_horizontalViewHalfAngle + currViewRotateZ);
                Vector3 lookDir = AngleToDirZ(currViewRotateZ);

                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(originPos, horizontalLeftDir * m_viewRadius);
                Gizmos.DrawRay(originPos, lookDir * m_viewRadius);
                Gizmos.DrawRay(originPos, horizontalRightDir * m_viewRadius);

                FindViewTargets();
            }
        }

        public Collider2D[] FindViewTargets()
        {
            hitedTargetContainer.Clear();

            Vector2 originPos = transform.position;
            Collider2D[] hitedTargets = Physics2D.OverlapCircleAll(originPos, m_viewRadius, m_viewTargetMask);
            currViewRotateZ = m_viewRotateZ;
            if (Application.isPlaying && character.IsFacingRight)
            {
                currViewRotateZ *= -1;
            }

            foreach (Collider2D hitedTarget in hitedTargets)
            {
                Vector2 targetPos = hitedTarget.transform.position;
                Vector2 dir = (targetPos - originPos).normalized;
                Vector2 lookDir = AngleToDirZ(currViewRotateZ);

                // float angle = Vector3.Angle(lookDir, dir)
                // 아래 두 줄은 위의 코드와 동일하게 동작함. 내부 구현도 동일
                float dot = Vector2.Dot(lookDir, dir);
                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (angle <= m_horizontalViewHalfAngle)
                {
                    RaycastHit2D rayHitedTarget = Physics2D.Raycast(originPos, dir, m_viewRadius, m_viewObstacleMask);
                    if (rayHitedTarget)
                    {
                        if (m_bDebugMode)
                            Debug.DrawLine(originPos, rayHitedTarget.point, Color.yellow);
                        circle.isInSector = false;
                    }
                    else
                    {
                        hitedTargetContainer.Add(hitedTarget);
                        if (m_bDebugMode)
                            Debug.DrawLine(originPos, targetPos, Color.red);
                        circle.isInSector = true;
                    }
                }
            }

            //Debug.Log("cir " + circle.isInSector);

            if (hitedTargetContainer.Count > 0)
                return hitedTargetContainer.ToArray();
            else
                return null;
        }

        // -180~180의 값을 Up Vector 기준 Local Direction으로 변환시켜줌.
        private Vector2 AngleToDirZ(float angleInDegree)
        {
            float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian));
        }
    }
}
