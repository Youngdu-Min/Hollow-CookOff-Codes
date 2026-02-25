using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
public class RaycastBullet : MMPoolableObject
{
    private LineRenderer lr;
    [SerializeField]
    private float trailDuration;

    public int damage;
    public float range;
    public LayerMask Interactable;
    [SerializeField]
    private GameObject beBloodEffect;


    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public IEnumerator leaveTrail;
    protected IEnumerator LeaveTrail()
    {
        damage = (int)WeaponTable.BaseData.BaseDataMap[4].damage;
        Vector2 firePoint = this.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(firePoint, transform.right, range, Interactable);
        if (hit)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, hit.point + (hit.point - (Vector2)transform.position).normalized * 0.3f);
            var hp = hit.collider.transform.GetComponent<Health>();
            if (hp != null)
            {

                if (hp.TemporaryInvulnerable || hp.Invulnerable)
                {
                    DamageTextPool.GetObject(hit.point, "MISS");
                }
                else
                {
                    hp.Damage(damage, this.gameObject, 0, 0, Vector3.up);
                    InstantiateEffect(beBloodEffect, hp.transform.position);
                }
            }
        }
        else
        {
            //아무것도 안맞았을때
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position + transform.right * 100);
        }

        float _lifetime = trailDuration;
        while (_lifetime >= 0)
        {
            _lifetime -= Time.deltaTime;
            lr.widthMultiplier = (1 + _lifetime) * 0.5f;
            lr.startColor = SetAlphaColor(lr.startColor, _lifetime);
            lr.endColor = SetAlphaColor(lr.endColor, _lifetime);
            yield return null;
        }

        //yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
        yield break;
    }

    public void RayFire()
    {
        if (leaveTrail != null)
        {
            StopCoroutine(leaveTrail);
        }
        leaveTrail = LeaveTrail();
        StartCoroutine(leaveTrail);
    }

    private void InstantiateEffect(GameObject effect, Vector3 position)
    {
        var _effect = Instantiate(effect);
        _effect.transform.position = position;
        Destroy(_effect, 1f);
    }

    private Color SetAlphaColor(Color color, float alpha)
    {
        return color = new Color(color.r, color.g, color.b, alpha);
    }
}
