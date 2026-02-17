using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class BulletParry : MonoBehaviour
    {
        private CharacterHandleWeapon mainWeapon;
        private WeaponSelectUI selectUI;
        private MeleeWeapon knife;
        [SerializeField] private Weapon ParryWeapon;
        private bool isInit = false;
        // private Weapon LastWeapon;

        private void Init()
        {
            mainWeapon = this.gameObject.GetComponent<CharacterHandleWeapon>();
            knife = this.gameObject.GetComponent<CharacterHandleSecondaryWeapon>().InitialWeapon.GetComponent<MeleeWeapon>();
            selectUI = FindObjectOfType<WeaponSelectUI>();
        }

        public void ParryStart(float timeScale, GameObject parryObj)
        {
            if (!isInit)
                Init();


            mainWeapon.CurrentWeapon.Owner.CharacterAnimator.enabled = false;
            print($"패리 시작 {mainWeapon.WeaponAttachment.gameObject} {mainWeapon.WeaponAttachment.gameObject.activeSelf}");

            if (!mainWeapon.WeaponAttachment.gameObject.activeSelf)
                mainWeapon.WeaponAttachment.gameObject.SetActive(true);
            mainWeapon.ChangeWeapon(ParryWeapon, "1", false);

            GameObject parryBullet = mainWeapon.CurrentWeapon.GetComponent<MMSimpleObjectPooler>().ReturnActivePool();
            SpriteRenderer parryBulletSprite = parryBullet.GetComponentInChildren<SpriteRenderer>();
            SpriteRenderer parryObjSpriteRenderer = parryObj.GetComponentInChildren<SpriteRenderer>();

            parryBulletSprite.sprite = parryObjSpriteRenderer.sprite;
            parryBulletSprite.transform.localEulerAngles = parryObjSpriteRenderer.transform.localEulerAngles;
            parryBullet.GetComponent<Projectile>().SetShowLine(parryObj.GetComponentInChildren<Projectile>().ShowLine);
            Destroy(parryBullet.GetComponent<BoxCollider2D>());
            parryBullet.AddComponent<BoxCollider2D>();
            parryBulletSprite.transform.localScale = parryObjSpriteRenderer.transform.localScale;
            parryObj.GetComponent<Health>().Kill();
            Time.timeScale = timeScale;
            // LastWeapon = mainWeapon.CurrentWeapon;
        }

        public IEnumerator ParryEnd()
        {
            if (selectUI == null)
                yield break;
            Time.timeScale = 1;
            yield return null;
            mainWeapon.ChangeWeapon(selectUI.weapons[selectUI.selectedMainWp].GetComponent<Weapon>(), "1", false);
            mainWeapon.CurrentWeapon.Owner.CharacterAnimator.enabled = true;
            knife.currCoolDown = 0;
            enabled = false;
        }

        private void Update()
        {

            if (Input.GetButton("Player1_Shoot") || Input.GetButton("Player1_SecondaryShoot"))
            {
                mainWeapon.ShootStart();

            }

            if (mainWeapon.CurrentWeapon._delayBetweenUsesCounter > 0)
            {
                StartCoroutine(ParryEnd());
            }
        }
    }
}
