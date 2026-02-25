using System.Collections;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This action directs the CharacterHorizontalMovement ability to move in the direction of the target.
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Move Towards Target Point")]
    // [RequireComponent(typeof(CharacterHorizontalMovement))]
    public class AIActionMoveTowardsTarget_Point : AIActionMoveTowardsTarget
    {
        enum MoveMode
        {
            Walk,
            Swing
        }
        [Header("-----------------------------")]
        [SerializeField]
        MoveMode moveMode = MoveMode.Walk;

        [Header("포인트 이동")]
        [SerializeField] private bool isSameInitPoint;
        [SerializeField] private int initPointIdx;
        [SerializeField] private GameObject[] points;
        private GameObject currPoint;
        private DamageOnTouch charaColl;
        private Transform originTarget;
        private BossHealthDisplay bossHealth;
        private bool manualMove;
        private int currIdx = -1;

        [Header("스윙 이동")]
        [SerializeField][Range(0f, 10f)] private float speed = 2;
        [SerializeField][Range(0f, 10f)] private float jumpSpeed = 2;
        [SerializeField][Range(0f, 10f)] private float radius = 1;

        private float runningTime = 0;
        private Vector2 swingPos = Vector2.zero;
        [SerializeField] private float endPosX;
        private float lastPosDiff;

        private float ratio = 2;
        private Vector2 initialPos;

        private bool isJump = true;
        private bool isSpin = false;

        private LineRenderer line;
        [SerializeField] private GameObject linePointObj;

        /// <summary>
        /// On init we grab our CharacterHorizontalMovement ability
        /// </summary>
        protected override void Initialization()
        {
            _characterHorizontalMovement = this.gameObject.GetComponent<Character>()?.FindAbility<CharacterHorizontalMovement>();
            if (_characterHorizontalMovement == null)
            {
                _characterHorizontalMovement = transform.parent.parent.GetComponent<Character>()?.FindAbility<CharacterHorizontalMovement>();
            }

            _controller = GetComponent<CorgiController>();
            if (_controller == null)
                _controller = transform.parent.parent.GetComponent<CorgiController>();

            bossHealth = _controller.GetComponent<BossHealthDisplay>();
            charaColl = _controller.GetComponent<DamageOnTouch>();
            line = _controller.GetComponent<LineRenderer>();

        }

        protected void SetPoint()
        {
            StartPointValidate();
            int randomIdx = default;

            if (bossHealth.Chatted())
            {
                randomIdx = points.Length - 1;
                manualMove = true;
                bossHealth.Invulnerable = true;
            }
            else
            {
                if (moveMode == MoveMode.Walk)
                    SetIdx(out randomIdx);
                else if (moveMode == MoveMode.Swing)
                    isMoveAir = true;
            }

            if (line)
                line.positionCount = 2;
            currIdx = randomIdx;
            currPoint = points[randomIdx];
        }

        private void StartPointValidate()
        {
            if (currIdx >= 0) return;
            for (int i = 0; i < points.Length; i++)
            {
                if (Vector2.Distance(points[i].transform.position, _brain.transform.position) < 1.5f)
                    _brain.CurrentState.EvaluateTransitions(true);
            }
        }

        private void SetIdx(out int randomIdx)
        {
            if (currIdx < 0 && isSameInitPoint)
            {
                randomIdx = initPointIdx;
                return;
            }
            while (true) // 다른 포인트로 될 때까지 반복
            {
                randomIdx = Random.Range(0, points.Length);
                if (currIdx != randomIdx || currIdx < 0)
                    break;

            }
        }

        /// <summary>
        /// Moves the character in the decided direction
        /// </summary>
        protected override void Move()
        {
            if (moveMode == MoveMode.Walk)
                WalkPoint();
            else if (moveMode == MoveMode.Swing)
                SwingPoint();
        }

        private void WalkPoint()
        {
            if (Mathf.Abs(transform.position.x - currPoint.transform.position.x) <= MinimumDistance)
                return;

            if (transform.position.x < currPoint.transform.position.x)
                _characterHorizontalMovement.SetHorizontalMove(1f);
            else
                _characterHorizontalMovement.SetHorizontalMove(-1f);
        }

        private void SwingPoint()
        {
            if (isJump && !isSpin)
            {
                if (_brain.Target.position.y > _controller.transform.position.y)
                {
                    Vector2 sumVector = _controller.transform.position + _controller.transform.up * jumpSpeed * Time.deltaTime;
                    _controller.GravityActive(false);
                    _controller.SetTransformPosition(sumVector);
                }
                else
                {
                    isJump = false;
                    isSpin = true;
                    radius = Vector3.Distance(new Vector3(_brain.Target.position.x, 0, 0), new Vector3(_controller.transform.position.x, 0, 0));
                    initialPos = _controller.transform.position;
                    endPosX = initialPos.x < _brain.Target.position.x ? initialPos.x + radius * 2 : initialPos.x - radius * 2;
                    line.enabled = true;
                    linePointObj.gameObject.SetActive(true);
                    RefreshLine();
                    RefreshLinePointRotation();
                }

            }

            if (isSpin)
            {
                float currPosDiff;
                lastPosDiff = Mathf.Abs((float)((System.Math.Truncate(endPosX * 10) / 10) - (System.Math.Truncate(_controller.transform.position.x * 10) / 10)));
                RefreshLine();

                runningTime += Time.deltaTime * speed;
                float x = initialPos.x < _brain.Target.position.x ? radius * -Mathf.Cos(runningTime) : radius * Mathf.Cos(runningTime);
                float y = radius * -Mathf.Sin(runningTime) / ratio;
                swingPos = new Vector2(x + _brain.Target.position.x, y + _brain.Target.position.y);
                _controller.SetTransformPosition(swingPos);
                currPosDiff = Mathf.Abs((float)((System.Math.Truncate(endPosX * 10) / 10) - (System.Math.Truncate(_controller.transform.position.x * 10) / 10)));

                if ((System.Math.Truncate(endPosX * 10) / 10) == (System.Math.Truncate(_controller.transform.position.x * 10) / 10) || lastPosDiff < currPosDiff)
                {
                    _controller.transform.position = new Vector3(endPosX, _controller.transform.position.y, _controller.transform.position.z);
                    SafeExit();

                }
            }

            void RefreshLine()
            {
                Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, Camera.main.nearClipPlane));

                line.SetPosition(0, _controller.transform.position);
                line.SetPosition(1, new Vector3(centerPos.x, centerPos.y, 0));
                linePointObj.transform.position = line.GetPosition(1);
            }

            void RefreshLinePointRotation()
            {
                Vector2 direction = line.GetPosition(0) - line.GetPosition(1);
                float rotAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
                Quaternion rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);
                linePointObj.transform.rotation = rotation;
            }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            charaColl.DamageCausedKnockbackType = DamageOnTouch.KnockbackStyles.NoKnockback;

            if (originTarget)
            {
                SetPoint();
                _brain.Target = currPoint.transform;

            }
            else
                StartCoroutine(TargetWait());
        }

        IEnumerator TargetWait()
        {
            _brain.FindTarget();
            yield return new WaitUntil(() => _brain.Target != null);
            originTarget = _brain.Target;
            SetPoint();
            _brain.Target = currPoint.transform;
        }

        /// <summary>
        /// When exiting the state we reset our movement.
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            charaColl.DamageCausedKnockbackType = DamageOnTouch.KnockbackStyles.AddForce;
            _characterHorizontalMovement?.SetHorizontalMove(0f);

            _brain.Target = originTarget;
            if (manualMove)
                bossHealth.StartPhaseChat();

            if (moveMode == MoveMode.Swing)
                SafeExit();
        }

        void SafeExit()
        {
            isJump = true;
            isSpin = false;
            radius = default;
            initialPos = default;
            endPosX = default;
            isMoveAir = false;
            runningTime = 0;
            line.enabled = false;
            line.positionCount = 0;
            linePointObj.gameObject.SetActive(false);
            _controller.GravityActive(true);
        }
    }
}
