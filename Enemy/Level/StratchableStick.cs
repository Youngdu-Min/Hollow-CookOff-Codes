using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StratchableStick : MonoBehaviour
{
    public GameObject target;

    [SerializeField]
    private SpriteRenderer sr;
    private BoxCollider2D box2d;
    // Start is called before the first frame update
    void Start()
    {
        sr = target.GetComponent<SpriteRenderer>();
        box2d = target.GetComponent<BoxCollider2D>();
    }

    public void Stratch(float amount, bool fixedCenter)
    {
        if (sr !=null)
        {
            sr.size += Vector2.up * amount;
        }
        if (box2d !=null)
        {
            box2d.size += Vector2.up * amount;
        }
        if (!fixedCenter)
        {
            target.transform.Translate(Vector3.down * amount/ 2f);

        }
    }

    public void SetLength(float _length)
    {

        sr.size = new Vector2(sr.size.x, _length);
        box2d.size = new Vector2(box2d.size.x, _length);

    }
}
