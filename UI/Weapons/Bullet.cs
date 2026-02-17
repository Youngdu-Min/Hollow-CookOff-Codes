using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int weaponIndex;
    private Vector2 firePosition;
    private DamageOnTouch damageOnTouch;
    private Projectile projectile;
    private CorgiController ownerCorgi;


    private float SqrDistance()
    {
        return ((Vector2)transform.position - firePosition).sqrMagnitude;
    }

    public int CalDamage(float origin, Vector2 firePos, Vector2 hitPos, float range)
    {
        float sqrDistance = (hitPos - firePos).sqrMagnitude;
        if (sqrDistance < range * range)
        {
            return (int)origin;
        }
        else
        {
            return (int)(origin * range / Mathf.Sqrt(sqrDistance));
        }

        /*
         * 사거리가 3m일때
         * 3m 내에서 맞추면 데미지 그대로
         * 거리 6m에서 맞추면 데미지 0.5배
         * 9m에서 맞추면 데미지0.33배
         */
    }

    private void OnEnable()
    {
        firePosition = transform.position;

        if (damageOnTouch == null)
            damageOnTouch = GetComponent<DamageOnTouch>();
        if (projectile == null)
            projectile = GetComponent<Projectile>();
        if (ownerCorgi == null)
            ownerCorgi = projectile.GetOwner()?.GetComponent<CorgiController>();

        if (weaponIndex == 1)
        {
            damageOnTouch.DamageCausedKnockbackForce = new Vector2(GSManager.Ability.blowbackPower, GSManager.Ability.blowbackPower * 0.1f) * 2;
            StartCoroutine(RevertKnockBack());
        }

        IEnumerator RevertKnockBack()
        {
            yield return new WaitForSeconds(0.1f);
            damageOnTouch.DamageCausedKnockbackForce = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int dmg = CalDamage(WeaponTable.BaseData.BaseDataList[weaponIndex].damage, firePosition, transform.position,
                   WeaponTable.BaseData.BaseDataList[weaponIndex].range);
        damageOnTouch.DamageCaused = dmg;
        if (collision.gameObject.TryGetComponent(out PauseArea pause))
        {
            projectile.Speed = 0;
            return;
        }

        if (collision.gameObject.TryGetComponent(out Health hp))
        {
            var _controller = collision.GetComponent<CorgiController>();

            if (hp.TemporaryInvulnerable || hp.Invulnerable)
                return;

            if (_controller && _controller.State.IsFalling)
                ScoreManager.Instance.score.flyingTarget += Mathf.Min(hp.CurrentHealth, (int)WeaponTable.BaseData.BaseDataMap[weaponIndex].damage);

            if (ownerCorgi && ownerCorgi.gameObject.layer == LayerMask.NameToLayer("Player") && !ownerCorgi.State.IsGrounded)
                ScoreManager.Instance.score.airshootScore += Mathf.Min(hp.CurrentHealth, (int)WeaponTable.BaseData.BaseDataMap[weaponIndex].damage);

            if (gameObject.CompareTag("isParry"))
                ScoreManager.Instance.score.parryScore += Mathf.Min(hp.CurrentHealth, (int)WeaponTable.BaseData.BaseDataMap[weaponIndex].damage);

            //라이플이면
            if (weaponIndex == 0 && !hp.ImmuneToKnockback)
            {
                if (collision.gameObject.tag != "Boss" && _controller?.State.IsFalling == true)
                {
                    MMSimpleObjectPooler[] pooler = FindObjectsOfType<MMSimpleObjectPooler>(); // 오브젝트 풀러 찾아서
                    char sp = '-';
                    string[] spStr = this.gameObject.name.Split(sp); // 이 총알의 이름을 스폰하는지 비교('-' 이전 부분은 동일함)
                    Debug.Log("split " + spStr[0]);
                    for (int i = 0; i < pooler.Length; i++)
                    {
                        if (pooler[i].GameObjectToPool.name.Equals(spStr[0])) // 오브젝트 풀러가 생성하는 오브젝트와 이 총알의 '-' 앞부분이 같다면
                        {
                            // 트리거 해피 상승량 - (이 옵젝 위치 - 풀러 무기의 주인(플레이어)의 위치로 계산한 값)으로 포스 적용
                            float y = transform.position.y - pooler[i].GetComponent<ProjectileWeapon>().Owner.transform.position.y;
                            _controller?.SetVerticalForce(GSManager.Ability.enemyTriggerHappy - y);
                            Debug.Log("splitSp " + (GSManager.Ability.enemyTriggerHappy - y));
                        }

                    }

                    //if(_character.MovementState.CurrentState == CharacterStates.MovementStates.Flying)
                    // _character.Stun();
                    //_character.MovementState.ChangeState(CharacterStates.MovementStates.Falling);
                }
            }
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        projectile.Speed = WeaponTable.BaseData.BaseDataMap[weaponIndex].Velocity;
    }
}
