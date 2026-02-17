using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Garden
{
    public class WeaponGizmo : MonoBehaviour
    {
        private float blowbackRange;
        private float shotDegree;
        public float sin;
        public float z;
        public Transform muzzle;
        // Start is called before the first frame update
        void Start()
        {
            int _type = (int)GetComponent<MoreMountains.CorgiEngine.ProjectileWeapon>().weaponType;
            shotDegree = WeaponTable.BaseData.BaseDataList[_type].zSpread;

            blowbackRange = WeaponTable.AbilityData.AbilityDataList[0].blowbackRange;
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (muzzle.position.x > gameObject.transform.position.x ^ (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270))
            {
                z = transform.eulerAngles.z;
                sin = Mathf.Sin(z * Mathf.Deg2Rad);
                Debug.DrawLine(muzzle.position, muzzle.position + muzzle.right * blowbackRange, Color.blue);
            }
            else
            {
                z = transform.eulerAngles.z - 180;
                sin = Mathf.Sin(z * Mathf.Deg2Rad);
                Debug.DrawLine(muzzle.position, muzzle.position - muzzle.right * blowbackRange, Color.blue);
            }
            Debug.DrawLine(muzzle.position, muzzle.position + new Vector3(Mathf.Cos((shotDegree + z) * Mathf.Deg2Rad), Mathf.Sin((shotDegree+ z) * Mathf.Deg2Rad),0)* blowbackRange, Color.blue);

            Debug.DrawLine(muzzle.position, muzzle.position + new Vector3(Mathf.Cos((-shotDegree + z) * Mathf.Deg2Rad), Mathf.Sin((-shotDegree + z) * Mathf.Deg2Rad), 0) * blowbackRange, Color.blue);
#endif
        }
    }
}