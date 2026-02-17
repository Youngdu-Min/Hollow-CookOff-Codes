using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System;
using UnityEngine;

public class BEhemoth : ProjectileWeapon
{
    protected BioEnerge BioEnerge;

    public override void Initialization()
    {
        base.Initialization();
        BioEnerge = MainCharacter.instance.GetComponent<BioEnerge>();
    }
    public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation = true)
    {
        /// we get the next object in the pool and make sure it's not null
        GameObject nextGameObject = ObjectPooler.GetPooledGameObject();

        // mandatory checks
        if (nextGameObject == null) { return null; }
        if (nextGameObject.GetComponent<MMPoolableObject>() == null)
        {
            throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
        }
        // we position the object
        nextGameObject.transform.position = spawnPosition;
        // we set its direction

        Projectile projectile = nextGameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetWeapon(this);
            if (Owner != null)
                projectile.SetOwner(Owner.gameObject);
        }

        // we activate the object
        nextGameObject.SetActive(true);

        nextGameObject.transform.rotation = transform.rotation;
        if (Flipped)
            nextGameObject.transform.Rotate(new Vector3(0, 0, 180));

        RaycastBullet trail = nextGameObject.GetComponent<RaycastBullet>();
        trail?.RayFire();

        if (projectile != null)
        {
            if (RandomSpread)
            {
                _randomSpreadDirection.x = UnityEngine.Random.Range(-Spread.x, Spread.x);
                _randomSpreadDirection.y = UnityEngine.Random.Range(-Spread.y, Spread.y);
                _randomSpreadDirection.z = UnityEngine.Random.Range(-Spread.z, Spread.z);
            }
            else
            {
                if (totalProjectiles > 1)
                {
                    _randomSpreadDirection.x = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.x, Spread.x);
                    _randomSpreadDirection.y = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.y, Spread.y);
                    _randomSpreadDirection.z = MMMaths.Remap(projectileIndex, 0, totalProjectiles - 1, -Spread.z, Spread.z);
                }
                else
                {
                    _randomSpreadDirection = Vector3.zero;
                }
            }

            Quaternion spread = Quaternion.Euler(_randomSpreadDirection);
            projectile.SetDirection(spread * transform.right * (Flipped ? -1 : 1), transform.rotation, Owner.IsFacingRight);
            if (RotateWeaponOnSpread)
                this.transform.rotation = this.transform.rotation * spread;
        }

        if (triggerObjectActivation)
            nextGameObject.GetComponent<MMPoolableObject>()?.TriggerOnSpawnComplete();

        return (nextGameObject);
    }

    protected override void ShootRequest()
    {
        if (Time.timeScale == 0)
            return;

        if (!BioEnerge.UseBE(100))
        {
            WeaponState.ChangeState(WeaponStates.WeaponIdle);
            return;
        }
        base.ShootRequest();
    }
}
