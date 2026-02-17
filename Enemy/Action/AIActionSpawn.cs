using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// An Action that shoots using the currently equipped weapon. If your weapon is in auto mode, will shoot until you exit the state, and will only shoot once in SemiAuto mode. You can optionnally have the character face (left/right) the target, and aim at it (if the weapon has a WeaponAim component).
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/AI/Actions/AI Action Spawn")]
    public class AIActionSpawn : AIAction
    {
        [SerializeField]
        private Transform[] preset;
        [SerializeField]
        private GameObject spawnPrefab;
        [SerializeField]
        private UnityEvent spawnEvent;

        [SerializeField]
        private bool isLerpMove = false;
        [SerializeField]
        private bool isLerpEndCollider = false;
        [SerializeField]
        private Transform initTarget;

        /// <summary>
        /// On PerformAction we face and aim if needed, and we shoot
        /// </summary>
        public override void PerformAction()
        {

        }

        public void Spawn()
        {
            int random = Random.Range(0, preset.Length);
            for (int i = 0; i < preset[random].childCount; i++)
            {
                GameObject obj = Instantiate(spawnPrefab, preset[random].GetChild(i));
                if (isLerpMove)
                    LerpMove(obj);
                else
                    obj.transform.localPosition = Vector3.zero;
            }
            spawnEvent?.Invoke();
        }

        private void LerpMove(GameObject obj)
        {
            if (isLerpEndCollider)
            {
                var coll = obj.GetComponent<Collider2D>();
                coll.enabled = false;
                obj.transform.position = initTarget.position;
                obj.transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => coll.enabled = true);
            }
            else
            {
                obj.transform.position = initTarget.position;
                obj.transform.DOLocalMove(Vector3.zero, 0.5f);
            }

        }

        /// <summary>
        /// Faces the target if required
        /// </summary>

        public override void OnEnterState()
        {
            base.OnEnterState();
            Spawn();
        }

        /// <summary>
        /// When exiting the state we make sure we're not shooting anymore
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
        }
    }
}
