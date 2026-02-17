using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class HomingProjectile : Projectile
    {
        public Transform target;
        public float rotateSpeed = 200f;
        private bool isStartMove;
        private Vector2 startPos;
        private float endPosDist;
        [SerializeField] private float posDistRandom;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            startPos = transform.position;
            isStartMove = true;
            endPosDist = EnemyBalance.etc.etcList[12].floatValue;
            Speed = EnemyBalance.etc.etcList[13].floatValue;
        }

        public override void SetOwner(GameObject newOwner)
        {
            base.SetOwner(newOwner);
            AIBrain brain = _owner.GetComponent<AIBrain>();
            if (brain && brain.Target != null)
            {
                target = brain.Target.transform;
            }
        }

        public override void Movement()
        {
            print($"{gameObject} Speed {Speed} {Direction} {Acceleration * Time.deltaTime}");
            if (Speed <= 0)
                return;

            print($"호밍 거리 {Vector2.Distance(transform.position, startPos)}");
            if (!isStartMove)
            {
                Direction = (Vector2)target.position - (Vector2)transform.position;
                Direction.Normalize();
            }
            else
            {
                Direction = transform.up;
                var calculEndPosDist = Random.Range(endPosDist - posDistRandom, endPosDist + posDistRandom);
                if (Vector2.Distance(transform.position, startPos) >= calculEndPosDist)
                    isStartMove = false;
            }


            float rotAngle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg - 90f;
            transform.DORotate(new Vector3(0, 0, rotAngle), rotateSpeed).SetSpeedBased(true);

            _movement = Direction * (Speed) * Time.deltaTime;
            transform.Translate(_movement, Space.World);

            Speed += Acceleration * Time.deltaTime;


            if (showLine && line)
                DrawLine();
        }
    }
}