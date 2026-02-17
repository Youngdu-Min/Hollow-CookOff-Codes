using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{

    private Rigidbody2D rb;
    private TextMeshPro textmesh;

    private IEnumerator life;
    private IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(1.4f);
        DestroyText();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        textmesh = GetComponent<TextMeshPro>();

    }
    void OnEnable()
    {
       // rb.velocity = new Vector2(Random.Range(-1, 1f), 3); //무작위로 튐

        life = LifeTime();
        StartCoroutine(life);
    }

    public void SetSpeed(Vector2 _speed)
    {
        rb.velocity = new Vector2(Random.Range(-1, 1f), 2) + _speed;
    }

    public void Print(Vector2 pos, float _value)
    {
        textmesh.text = _value.ToString();
        transform.position = pos;
    }
    private void DestroyText()
    {
        DamageTextPool.ReturnObject(this);
    }
}
