using MoreMountains.CorgiEngine;
using UnityEngine;

public class ParticleDamageOnTouch : DamageOnTouch
{
    ParticleSystem particle;

    protected override void Awake()
    {
        base.Awake();
        particle = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {

        print($"충돌 particle {other}");
        Colliding(other.GetComponent<Collider2D>());

        particle.Stop();

    }
}