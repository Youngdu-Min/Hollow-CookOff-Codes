using MoreMountains.CorgiEngine;
using System.Collections;
using UnityEngine;

public class AIGrenade : MonoBehaviour
{
    [SerializeField]
    Transform owner;
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField]
    private float flightSpeed = 3;
    public float hoverHeight;
    [SerializeField]
    private GameObject explosionObj;
    [SerializeField]
    private string targetTag;
    [SerializeField]
    private bool isIgnoreInvulnerable;
    private KillTimer killTimer;

    private void Awake()
    {
        if (TryGetComponent(out CircleCollider2D _circle))
        {
            _circle.radius = GSManager.Grenade.projectileRadius;
        }

        killTimer = GetComponent<KillTimer>();
    }
    private void OnEnable()
    {
        if (explosionObj)
            explosionObj.SetActive(false);

        float angle = transform.eulerAngles.z;
        transform.parent = owner;
        transform.localScale = (angle < 90 || angle > 270) ? new Vector3(1, 1, 1) : new Vector3(1, -1, 1);
        transform.position = owner.transform.position;
        StartCoroutine(ThrowProjectile());
    }

    private IEnumerator ThrowProjectile()
    {
        transform.parent = null;
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);
        Health targetHealth = target.GetComponent<Health>();
        bool lastInvulnerable = default;
        if (targetHealth && isIgnoreInvulnerable)
        {
            lastInvulnerable = targetHealth.Invulnerable;
            targetHealth.SetInvulnerable(false);
        }

        killTimer?.SetChainHealthes(target.GetComponents<Health>());

        Vector2 start = transform.position;
        Vector2 end = target.transform.position;

        float time = 0f;
        float duration = flightSpeed;

        Vector2 lastPos = Vector2.zero;
        Vector2 position = Vector2.zero;
        Vector2 posMag = Vector2.zero;

        while (time < duration)
        {
            time += Time.deltaTime * 3;
            float linearT = time / duration;
            float heightT = curve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, hoverHeight, heightT);
            lastPos = transform.position;
            position = Vector2.Lerp(start, end, linearT) + new Vector2(0f, height);
            posMag += position;
            transform.position = position;
            yield return null;
        }

        while (gameObject.activeInHierarchy)
        {
            transform.position += (Vector3)(position - lastPos) * 1.5f;
            yield return null;
        }

        if (targetHealth && isIgnoreInvulnerable)
            targetHealth.SetInvulnerable(lastInvulnerable);
    }

    public void StartExplosion()
    {
        explosionObj.transform.position = transform.position;
        explosionObj.SetActive(true);
        Invoke(nameof(EndExplosion), 0.1f);
    }

    public void EndExplosion()
    {
        explosionObj.SetActive(false);
    }
}
