using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGrenade : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject bomb;

    private GameObject _bomb;

    private Vector2 startposition;
    public void Bomb()
    {
        if (_bomb == null)
        {
            _bomb = Instantiate(bomb, transform.position, Quaternion.identity);
        }
        else
        {
            _bomb.transform.position = this.transform.position;
            _bomb.SetActive(true);
        }

    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        if (TryGetComponent(out CircleCollider2D _circle))
        {
            _circle.radius = GSManager.Grenade.projectileRadius;
        }
    }
    private void OnEnable()
    {
        startposition = transform.position;
        rb.velocity = transform.right * GSManager.Grenade.velocity;
        rb.gravityScale = 0;
        float angle = transform.eulerAngles.z;
        if (angle < 90 || angle > 270)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, -1, 1);
        }

    }

    private void FixedUpdate()
    {
        if (rb.gravityScale == 0)
        {
            if (Vector2.SqrMagnitude(startposition - (Vector2)transform.position) > GSManager.Grenade.straightRange * GSManager.Grenade.straightRange)
            {
                rb.gravityScale = GSManager.Grenade.gravityScale;
            }

        }

    }
}
