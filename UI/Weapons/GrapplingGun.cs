using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
namespace MoreMountains.CorgiEngine
{
    public class GrapplingGun : MonoBehaviour
    {
        [Header("Scripts Ref:")]
        public GrapplingRope grappleRope;

        [Header("Layers Settings:")]
        [SerializeField] private LayerMask layerMask;

        private CameraController _camera;

        [Header("Transform Ref:")]
        public Transform gunHolder;
        public Transform gunPivot;
        public Transform firePoint;

        [Header("Physics Ref:")]
        public SpringJoint2D m_springJoint2D;
        public DistanceJoint2D m_distanceJoint2D;
        public Rigidbody2D m_rigidbody;

        [Header("Rotation:")]
        [Range(0, 60)][SerializeField] private float swingSpeed = 4;

        [Header("Distance:")]
        [SerializeField] private bool hasMaxDistance = false;
        [SerializeField] private float maxDistance = 20;

        [Header("Launching:")]
        [SerializeField] private bool launchToPoint = true;

        [Header("No Launch To Point")]
        [SerializeField] private bool autoConfigureDistance = false;
        [SerializeField] private float targetDistance = 3;
        [SerializeField] private float targetFrequncy = 1;

        [SerializeField] private float enemyPullTime = 0.5f;

        [HideInInspector] public Vector2 grapplePoint;
        [HideInInspector] public Vector2 grappleDistanceVector;

        private Character _character;
        private CorgiController _controller;
        private Health health;
        private float startHealth;
        private float grabBeforeWait;
        [HideInInspector]
        public bool enemyGrab;
        private Transform enemyTr;

        [HideInInspector]
        public bool endOff;
        [HideInInspector]
        public bool onlyShowRope;
        private Vector2 launchPos;

        [SerializeField] private GameObject grappleHead;
        [SerializeField] private MMFeedbacks grappleShootFeedback;
        [SerializeField] private MMFeedbacks grappleHitFeedback;
        private Collider2D coll;
        public bool IsGrappling => grappleRope.isGrappling;

        private void Awake()
        {
            coll = GetComponent<Collider2D>();
        }

        void Start()
        {
            _camera = FindObjectOfType<CameraController>();
            _character = gunHolder.GetComponent<Character>();
            _controller = gunHolder.GetComponent<CorgiController>();
            health = gunHolder.GetComponent<Health>();

            swingSpeed = GSManager.Hook.swing;
            StartCoroutine(GrappleOff());
            grappleHead.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && !grappleRope.isGrappling && !grappleRope.enabled)
                SetGrapplePoint();
            else if (grappleRope.enabled && !onlyShowRope)
                RotateGun();
            else if (SubWeapon.currentSubWp != SubWeaponType.Grapple && endOff)
            {
                StartCoroutine(GrappleOff());
                gameObject.SetActive(false);
                Debug.Log("subweapon 그래플 3 " + endOff);
            }

            grappleHead.transform.position = grapplePoint;
        }

        void ControllerToggle(bool b)
        {
            if (_controller != null)
                _controller.enabled = b;
            // _controller.GravityActive(b);
        }


        public IEnumerator GrappleOff()
        {
            print($"GrappleOff");

            m_rigidbody.gravityScale = 1;
            ControllerToggle(true);
            m_rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            if (onlyShowRope)
                yield return new WaitForSeconds(grappleRope.waveSize);

            m_springJoint2D.enabled = false;
            m_distanceJoint2D.enabled = false;
            enemyGrab = false;

            grappleRope.enabled = false;
            grappleHead.gameObject.SetActive(false);
            grabBeforeWait = 0;
            onlyShowRope = false;
            coll.enabled = false;
            enemyTr = null;
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayCancelStopFeedback());

            IEnumerator DelayCancelStopFeedback()
            {
                yield return new WaitForSeconds(0.2f);
                grappleHitFeedback.StopFeedbacks();
            }
        }

        void RotateGun()
        {
            if (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead || _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Stunned || _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
            {
                StartCoroutine(GrappleOff());
                return;
            }

            Vector2 currDistance = grapplePoint - (Vector2)gunHolder.position;
            float deceleration = launchPos.x > 0 ? -0.4f : 0.4f;

            if (grappleHitFeedback?.IsPlaying == false)
                grappleHitFeedback?.PlayFeedbacks();

            Debug.Log(launchPos + "launchPos");
            if (!enemyGrab && (_controller.State.IsGrounded || (launchPos.x > 0 && (currDistance.x + deceleration) * -1 >= launchPos.x) || (launchPos.x < 0 && (currDistance.x + deceleration) * -1 <= launchPos.x) || health.CurrentHealth < startHealth))
            {
                StartCoroutine(GrappleOff());
                return;
            }

            if (enemyGrab && grabBeforeWait > 0.2f)
            {
                int direction = gunHolder.position.x > enemyTr.position.x ? 1 : -1;
                enemyTr.DOMoveX(gunHolder.position.x, enemyPullTime).OnUpdate(() =>
                {
                    print($"enemyTr.position.x : {enemyTr.position.x}, gunHolder.position.x : {gunHolder.position.x} / {Mathf.Abs(enemyTr.position.x - gunHolder.position.x)}");
                    if (Mathf.Abs(enemyTr.position.x - gunHolder.position.x) < 1f)
                        StartCoroutine(GrappleOff());
                });
                grapplePoint = enemyTr.position;
            }

            if (grappleRope.waveSize <= 0)
                m_rigidbody.gravityScale = swingSpeed;

            grabBeforeWait += Time.deltaTime;
        }

        void SetGrapplePoint()
        {
            Vector2 distanceVector = MainCamera.Main().ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
            float x = distanceVector.x < 0 ? Mathf.Abs(firePoint.localPosition.x) * -1 : Mathf.Abs(firePoint.localPosition.x) * 1;
            firePoint.transform.localPosition = new Vector3(x, firePoint.transform.localPosition.y, firePoint.transform.localPosition.z);

            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized, Mathf.Infinity, layerMask);
            if (!_hit || !IsPointInsideCamera(_hit.point))
                return;

            print($"_hit.transform.gameObject.layer : {_hit.transform.gameObject} {_hit.transform.gameObject.layer}");

            grappleShootFeedback?.PlayFeedbacks();

            if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistance || !hasMaxDistance)
            {
                grapplePoint = _hit.point;
                grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                grappleRope.enabled = true;
                grappleHead.transform.position = grapplePoint;
                grappleHead.gameObject.SetActive(true);
                Vector2 direction = grapplePoint - (Vector2)gunHolder.position;
                float rotAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                Quaternion rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);
                grappleHead.transform.rotation = rotation;
            }

            if ((_hit.transform.gameObject.layer == 13 || _hit.transform.gameObject.layer == 29) && !_hit.transform.gameObject.CompareTag("Boss") && !_hit.transform.gameObject.CompareTag("ExplosionEnemy") && !_hit.transform.GetComponent<Health>().ImmuneToKnockback)
            {
                enemyGrab = true;
                enemyTr = _hit.transform;
                m_rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else if ((_hit.point.y < gunHolder.position.y) || _controller.State.IsGrounded || (_hit.transform.gameObject.layer == 30 && _camera.edgePoints[1].y + MainCamera.Main().transform.position.y > _hit.point.y + 0.1f))
            {
                onlyShowRope = true;
                StartCoroutine(GrappleOff());
                return;
            }
            else
            {
                m_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            ControllerToggle(false);
            startHealth = health.CurrentHealth;

            coll.enabled = !enemyGrab;
            launchPos = grapplePoint - (Vector2)gunHolder.position;

            bool IsPointInsideCamera(Vector2 point)
            {
                if (MainCamera.Main() == null)
                {
                    Debug.LogError("Main camera not assigned!");
                    return false;
                }

                // Convert the world point to viewport point
                Vector3 viewportPoint = MainCamera.Main().WorldToViewportPoint(new Vector3(point.x, point.y, 0f));

                // Check if the point is inside the camera's viewport
                return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
            }
        }

        public void Grapple()
        {
            m_springJoint2D.autoConfigureDistance = false;

            if (launchToPoint)
            {
                m_distanceJoint2D.connectedAnchor = grapplePoint;
                m_distanceJoint2D.enabled = true;
                m_distanceJoint2D.distance = targetDistance;
                return;
            }

            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }
            else
            {
                m_springJoint2D.distance = targetDistance;
                m_springJoint2D.frequency = targetFrequncy;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }

        private void OnDrawGizmosSelected()
        {
            if (firePoint != null && hasMaxDistance)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(firePoint.position, maxDistance);
            }
        }
    }
}
