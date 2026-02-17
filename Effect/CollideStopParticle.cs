using UnityEngine;

public class CollideStopParticle : MonoBehaviour
{
    private ParticleSystem particle;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject hurricane)
    {
        particle.time = 0;
        particle.Stop();
    }
}
