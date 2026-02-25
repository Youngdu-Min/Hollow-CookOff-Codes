using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using Toctoc;
using UnityEngine;
using static MoreMountains.CorgiEngine.CharacterStates;

public class BioEnerge : MonoBehaviour
{
    Health health;
    Character character;
    public float currentBE, MaxBE;
    public float BeRegen;

    public GameObject ExplosionObj;
    float expTime;
    DamageOnTouch damageOnTouch;
    CircleCollider2D circle;
    public float DelayBeforeUse;
    float _delayBeforeUseCounter;
    public bool infinBe;
    protected int _knifeExplosionAnimationParameter;
    protected const string _knifeExplosionAnimationParameterName = "KnifeExplosion";
    [SerializeField]
    private MMFeedbacks _healFeedbacks;
    [SerializeField]
    private MMFeedbacks _beExplosionFeedbacks;
    private AlphaCurve _alphaCurve;

    public bool UseBE(float amount)
    {//BE 충분한지 확인
        if (infinBe)
        {
            return true;
        }
        else if (currentBE >= amount)
        {
            currentBE -= amount;
            return true;
        }

        return false;
    }

    public bool RestoreBE(float amount)
    {
        if (currentBE < MaxBE) //be가 최대치보다 적을때만 회복
        {
            currentBE = Mathf.Min(MaxBE, currentBE + amount);
            return true;
        }

        return false;
    }

    private void Awake()
    {
        health = GetComponent<Health>();
        character = GetComponent<Character>();
        var alphaCurves = FindObjectsOfType<AlphaCurve>();
        foreach (var alphaCurve in alphaCurves)
        {
            if (alphaCurve.gameObject.layer == 5)
                _alphaCurve = alphaCurve;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentBE = MaxBE / 3;
        ExplosionObj.SetActive(false);
        damageOnTouch = ExplosionObj.GetComponent<DamageOnTouch>();
        circle = ExplosionObj.GetComponent<CircleCollider2D>();
        expTime = HollowBalance.action.actionList[15].floatValue; // 시전시간 
        DelayBeforeUse = HollowBalance.action.actionList[16].floatValue; // 쿨타임
        damageOnTouch.DamageCausedKnockbackForce.y = HollowBalance.action.actionList[17].floatValue; // 띄우는 높이
        circle.radius = HollowBalance.action.actionList[18].floatValue; // 범위
        MMAnimatorExtensions.AddAnimatorParameterIfExists(character._animator, _knifeExplosionAnimationParameterName, out _knifeExplosionAnimationParameter, AnimatorControllerParameterType.Bool, character._animatorParameters);

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E)) //e누르면 힐
        {
            if (UseBE(100))
            {
                health.CurrentHealth = Mathf.Min(health.MaximumHealth, health.CurrentHealth + 100);
                _healFeedbacks?.PlayFeedbacks();
                _alphaCurve?.SetColor((new Color(142f / 255f, 202f / 255f, 230f / 255f, 1f)));
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (SaveDataManager.Instance.IsEnable(ContentsEnable.BE_Explosion) && character.MovementState.CurrentState != MovementStates.KnifeExplosioning && _delayBeforeUseCounter <= 0)
            {
                if (UseBE(100))
                {
                    StartCoroutine(BE_Explosion());
                }

            }

        }
        _delayBeforeUseCounter -= Time.deltaTime;
        MMAnimatorExtensions.UpdateAnimatorBool(character._animator, _knifeExplosionAnimationParameter, character.MovementState.CurrentState == MovementStates.KnifeExplosioning, character._animatorParameters, character.PerformAnimatorSanityChecks);

    }
    IEnumerator BE_Explosion()
    {
        PauseManager.Instance().PauseCall(this, true);
        if (character._animator.enabled == false)
            character._animator.enabled = true;
        character._animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        character.MovementState.ChangeState(MovementStates.KnifeExplosioning);
        yield return new WaitForSecondsRealtime(expTime / 2);
        _beExplosionFeedbacks?.PlayFeedbacks();
        yield return new WaitForSecondsRealtime(expTime / 2);
        ExplosionObj.SetActive(true);
        PauseManager.Instance().PauseCall(this, false);
        yield return new WaitForSeconds(0.1f);
        ExplosionObj.SetActive(false);
        character.MovementState.ChangeState(MovementStates.Idle);
        character._animator.updateMode = AnimatorUpdateMode.Normal;

        _delayBeforeUseCounter = DelayBeforeUse;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, HollowBalance.action.actionList[18].floatValue);

    }
}
