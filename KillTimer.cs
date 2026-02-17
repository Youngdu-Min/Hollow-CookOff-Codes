using MoreMountains.CorgiEngine;
using System.Collections;
using UnityEngine;

public class KillTimer : MonoBehaviour
{
    private Health health;
    [SerializeField] private Health[] chainHealthes;
    [SerializeField] private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        StartCoroutine(KillAfterTime());
    }

    IEnumerator KillAfterTime()
    {
        yield return new WaitForSeconds(timer);
        health?.Kill();
        chainHealthes.ForEach(h => h.Kill());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetChainHealthes(Health[] healthes)
    {
        chainHealthes = healthes;
    }


}
