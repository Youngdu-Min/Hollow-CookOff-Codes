using UnityEngine;

public class Lookat : MonoBehaviour
{
    private Camera c;

    public SpriteRenderer sprite;

    public float ratio = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        c = Camera.main;
    }

    void OnDisable()
    {
        transform.localScale = Vector3.one;
        transform.eulerAngles = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0f)
        {
            Vector3 direction = c.ScreenToWorldPoint(Input.mousePosition) - transform.position;


            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (angle < 90 && angle > -90)
            {
                transform.localScale = Vector3.one;
                transform.eulerAngles = new Vector3(0, 0, angle * ratio);
            }
            else
            {

                transform.localScale = new Vector3(-1, -1, 1);
                if (angle > 0)
                {
                    transform.eulerAngles = new Vector3(0, 0, 180 + (angle - 180) * ratio);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, -180 + (angle + 180) * ratio);
                }
            }

        }

    }
}
