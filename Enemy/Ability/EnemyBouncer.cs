using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.U2D;

public class EnemyBouncer : MonoBehaviour
{[SerializeField]
    [Tooltip("Just for debugging, adds some velocity during OnEnable")]
    private Vector3 initialVelocity;

    [SerializeField]
    private float minVelocity = 10f;

    private Vector3 lastFrameVelocity;
    private Rigidbody2D rb;
    [SerializeField]
    private CorgiController corgi;
    private GameObject player;
    private int bounceCt;
    [SerializeField]
    private int endBounceCt;

    private void OnEnable()
    {
        corgi.GravityActive(false);
        
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(WaitPlayer());
    }

    private void OnDisable()
    {
        corgi.GravityActive(true);
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        corgi.SetForce(Vector2.zero);
        rb.constraints = RigidbodyConstraints2D.None;
        this.GetComponent<AIBrain>().enabled = true;
    }

    IEnumerator WaitPlayer()
    {
        yield return new WaitUntil(() => GameObject.Find("Hollow") != null);
        player = GameObject.Find("Hollow");
        rb.velocity = (player.transform.position - transform.position).normalized * minVelocity;

        Debug.Log("속도 " + rb.velocity);
    }


    private void Update()
    {
        lastFrameVelocity = rb.velocity;
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        Bounce(coll.contacts[0].normal);
    }
    /*
    void OnTriggerEnter2D(Collider2D col)
    {
    }
    */
    private void Bounce(Vector3 collisionNormal)
    {
        bounceCt++;
        if (bounceCt >= endBounceCt)
        {
            this.enabled = false;
        }
        var speed = lastFrameVelocity.magnitude;
        Debug.Log($"튕긴 속도 {rb.velocity.magnitude}");
        var direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);

        Debug.Log("Out Direction: " + direction);
        rb.velocity = direction * Mathf.Max(speed, minVelocity);
    }
}
