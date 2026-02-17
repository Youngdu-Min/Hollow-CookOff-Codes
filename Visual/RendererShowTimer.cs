using System.Collections;
using UnityEngine;

public class RendererShowTimer : MonoBehaviour
{
    [SerializeField] private float timer = 0.0f;
    private Renderer rend;
    // Start is called before the first frame update
    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private IEnumerator ShowAfterTime()
    {
        yield return new WaitForSeconds(timer);
        rend.enabled = true;
    }

    void OnEnable()
    {
        rend.enabled = false;
        StartCoroutine(ShowAfterTime());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
