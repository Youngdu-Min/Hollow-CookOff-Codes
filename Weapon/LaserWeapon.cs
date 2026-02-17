using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// A weapon class aimed specifically at allowing the creation of various projectile weapons, from shotgun to machine gun, via plasma gun or rocket launcher
    /// </summary>
    [MMHiddenProperties("WeaponOnMissFeedback", "ApplyRecoilOnHitDamageable", "ApplyRecoilOnHitNonDamageable", "ApplyRecoilOnHitNothing", "ApplyRecoilOnKill")]
    [AddComponentMenu("Corgi Engine/Weapons/Laser Weapon")]
    public class LaserWeapon : Weapon
    {
        [MMInspectorGroup("Projectile Spawn", true, 65)]

        /// the transform to use as the center reference point of the spawn
        [Tooltip("the transform to use as the center reference point of the spawn")]
        public Transform ProjectileSpawnTransform;
        /// the offset position at which the projectile will spawn
        [Tooltip("the offset position at which the projectile will spawn")]
        public Vector3 ProjectileSpawnOffset = Vector3.zero;
        /// the local position at which this projectile weapon should spawn projectiles
        [MMReadOnly]
        [Tooltip("the local position at which this projectile weapon should spawn projectiles")]
        public Vector3 SpawnPosition = Vector3.zero;
        protected Vector3 _spawnPositionCenter;
        protected DamageOnTouch _damageOnTouch;
        protected WeaponLaserSight _laserSight;

        /// <summary>
        /// Initialize this weapon
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
            _damageOnTouch = GetComponent<DamageOnTouch>();
            _laserSight = GetComponent<WeaponLaserSight>();
            DelayBeforeUse = EnemyBalance.etc.etcList[11].floatValue;
        }

        /// <summary>
        /// Called everytime the weapon is used
        /// </summary>
        protected override void WeaponUse()
        {
            base.WeaponUse();

            //회전하고있으면 공격시마다 조준값 변경
            if (_aimableWeapon != null && _aimableWeapon.AimControl == WeaponAim.AimControls.Spinning)
            {
                _aimableWeapon.spinFlip = !_aimableWeapon.spinFlip;
            }

            _damageOnTouch.OnTriggerEnter2D(_laserSight.RaycastHit2D.collider);
            print($"레이저 {_laserSight.RaycastHit2D.collider.gameObject}");
        }

        /// <summary>
        /// When the weapon is selected, draws a circle at the spawn's position
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(SpawnPosition, 0.2f);
        }
    }
}
