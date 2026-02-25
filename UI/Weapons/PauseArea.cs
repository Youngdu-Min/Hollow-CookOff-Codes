using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;

public class PauseArea : MonoBehaviour
{
    [SerializeField]
    private LayerMask interactable;

    private float leftTime;
    private float bossLeftTime;
    private List<Collider2D> freezeObjects = new List<Collider2D>();
    [SerializeField] private MMFeedbacks freezeFeedback;
    //private AlphaCurve _alphaCurve;

    void PauseInitial()
    {
        leftTime = GSManager.Grenade.duration;
        freezeObjects.Clear();

        var collisions = Physics2D.OverlapCircleAll(transform.position, GSManager.Grenade.explosionRadius, interactable);
        foreach (var freezeObj in collisions)
        {
            if (freezeObj.tag == "Boss")
            {
                leftTime = 0.5f;
                break;
            }
        }
    }
    private void Awake()
    {
        if (TryGetComponent(out CircleCollider2D _circle))
        {
            _circle.radius = GSManager.Grenade.explosionRadius;
        }

        //_alphaCurve = FindObjectOfType<AlphaCurve>();
    }
    private void OnEnable()
    {
        PauseInitial();
    }

    void Update()
    {
        leftTime -= Time.deltaTime;
        if (leftTime <= 0)
        {
            EndPause();
            gameObject.SetActive(false);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        freezeObjects.Add(collision);
        if (collision.TryGetComponent(out Character character))
        {
            character.Freeze();
            if (character.gameObject.tag == "Boss" && leftTime > bossLeftTime)
            {
                leftTime = 0.5f;
            }
            else if (character.CharacterType == Character.CharacterTypes.Player)
            {
                freezeFeedback?.PlayFeedbacks();
                //_alphaCurve.SetColor((new Color(0f / 255f, 0f / 255f, 0f / 255f, 1f)));
            }
        }
        else if (collision.TryGetComponent(out Projectile obj))
        {
            obj.Speed = 0;
        }
        if (collision.TryGetComponent(out MovingPlatform moving))
        {
            moving.ScriptActivated = true;
            moving.ForbidMovement();
            //무빙플랫폼 이동 정지
        }
        if (collision.TryGetComponent(out SpinningObject _spin))
        {
            _spin.SetSpinable(false);
        }
        if (collision.TryGetComponent(out DamageOnTouch_BE _damage))
        {
            if (freezeObjects.Contains(FindObjectOfType<MainCharacter>().GetComponent<Collider2D>()))
            {
                EndPause();
                gameObject.SetActive(false);
            }
        }

    }

    private void EndPause()
    {
        foreach (var freezeObj in freezeObjects)
        {
            if (freezeObj.TryGetComponent(out Character character))
            {
                character.UnFreeze();
            }
            if (freezeObj.TryGetComponent(out Projectile projectile))
            {
                projectile.InitSpeed();
            }
            if (freezeObj.TryGetComponent(out MovingPlatform moving))
            {
                moving.AuthorizeMovement();//다시 움직임
            }
            if (freezeObj.TryGetComponent(out SpinningObject _spin))
            {
                _spin.SetSpinable(true);
            }
        }
    }
    private void OnDisable()
    {
        if (MainCharacter.instance.TryGetComponent(out SubWeapon _sub))
        {
            _sub.grenadeActive = false;
        }
    }
}

