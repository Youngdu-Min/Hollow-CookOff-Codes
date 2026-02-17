using MoreMountains.CorgiEngine;
using System.Collections;
using UnityEngine;

public class SpecialAbility : MonoBehaviour
{
    public InputManager input;
    public InsideArm inArm;
    public ProjectileWeapon weapon;
    public ProjectileWeapon Weapon()
    {
        weapon = (ProjectileWeapon)GetComponent<CharacterHandleWeapon>().CurrentWeapon;
        return (ProjectileWeapon)GetComponent<CharacterHandleWeapon>().CurrentWeapon;
    }
    private Character chara;
    private bool panningUsable;

    private CorgiController _controller;

    public float spinGage;

    private bool pirouetteUsable;
    private bool isSpinning;
    public bool IsSpinning => isSpinning;
    [SerializeField] private Animator animator;
    private IEnumerator panning;
    private PlayerHealth playerHealth;
    private CharacterJump jump;
    private float pirouetteGravityDivide = 5f;

    // Start is called before the first frame update
    void Start()
    {
        weapon = (ProjectileWeapon)GetComponent<CharacterHandleWeapon>().CurrentWeapon;
        _controller = GetComponent<CorgiController>();
        chara = GetComponent<Character>();
        playerHealth = GetComponent<PlayerHealth>();
        jump = GetComponent<CharacterJump>();
        panningUsable = true;
        pirouetteUsable = true;
        playerHealth.OnRevive += () =>
        {
            weapon = (ProjectileWeapon)GetComponent<CharacterHandleWeapon>().CurrentWeapon;
            panningUsable = true;
            pirouetteUsable = true;
            animator.SetBool("Spinning", false);
            StopPirouette(false);
            spinGage = 0;
            PirouetteGage.Instance()?.gameObject.SetActive(false);
        };
        PirouetteGage.Instance()?.gameObject.SetActive(false);

        Lookat lookat = GetComponentInChildren<Lookat>();
        chara.onStun += () =>
        {
            lookat.enabled = false;
        };
        chara.onFreeze += () =>
        {
            lookat.enabled = false;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (chara.ConditionState.CurrentState != CharacterStates.CharacterConditions.Normal)
        {
            return;
        }

        //패닝
        if (Input.GetMouseButtonDown(1) && Input.GetMouseButton(0))
        {
            if (Weapon().weaponType == ProjectileWeapon.WeaponType.Revolver && panningUsable)
            {
                panningUsable = false;
                panning = Panning();
                StartCoroutine(panning);
            }
        }

        //머신건 피루엣
        if (pirouetteUsable && weapon != null && weapon.weaponType == ProjectileWeapon.WeaponType.Machinegun)
        {
            WeaponAim weaponAim = weapon.GetComponent<WeaponAim>();
            if (Input.GetMouseButtonDown(1) && Input.GetMouseButton(0))
            {
                PirouetteGage.Instance();
                weaponAim.AimControl = WeaponAim.AimControls.Spinning;
                isSpinning = true;

                if (weapon.TryGetComponent(out SpriteRenderer _sp))
                    _sp.enabled = false;

                var _sprite = weapon.GetComponentsInChildren<SpriteRenderer>();
                _sprite.ForEach(x => x.enabled = false);

                if (inArm != null)
                {
                    inArm.SetTarget(weapon.transform);
                    inArm.SetSprite(true);
                    inArm.XFlipOn(true);
                }

                if (pirouetteCool != null)
                    StopCoroutine(pirouetteCool);
            }
            if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && isSpinning)
            {
                if (chara.ConditionState.CurrentState == CharacterStates.CharacterConditions.Normal)
                {
                    if (_controller.State.IsGrounded && weaponAim.AimControl == WeaponAim.AimControls.Off)
                    {
                        StopPirouette();
                        return;
                    }

                    if (!_controller.State.IsGrounded)
                    {
                        weaponAim.AimControl = WeaponAim.AimControls.Off;
                        _controller.Parameters.Gravity = chara.OriginalGravity / pirouetteGravityDivide;
                    }
                    else
                    {
                        jump.AbilityPermitted = false;
                        spinGage += Time.deltaTime;
                        weaponAim.AimControl = WeaponAim.AimControls.Spinning;
                        PirouetteGage.Instance()?.gameObject.SetActive(true);
                    }
                    animator.SetBool("Spinning", true);
                }

                if (spinGage > GSManager.Ability.pirouetteStunMax)
                {
                    pirouetteUsable = false;
                    StopPirouette();
                    pirouetteStun = PirouetteStun();
                    StartCoroutine(pirouetteStun);

                }
            }

            //한쪽을 때면
            if (isSpinning && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
            {
                StopPirouette();
                PirouetteGage.Instance()?.gameObject.SetActive(false);
            }
        }

    }

    IEnumerator Panning()
    {
        playerHealth.OnHit += () => CancelPanning();

        chara.Freeze();
        yield return new WaitForSeconds(GSManager.Ability.panningDelay);
        for (int i = 0; i < 5; i++)
        {
            if (weapon?.weaponType == ProjectileWeapon.WeaponType.Revolver)
            {
                weapon.ManualUse();
                yield return new WaitForSeconds(GSManager.Ability.panningRate);
            }
        }
        chara.UnFreeze();
        panning = PanningCool();
        StartCoroutine(panning);
        playerHealth.OnHit -= () => CancelPanning();



    }

    private void CancelPanning(bool isUnfreeze = true)
    {
        if (isUnfreeze)
            chara.UnFreeze();
        if (panning != null)
            StopCoroutine(panning);
        StartCoroutine(PanningCool());
    }

    IEnumerator PanningCool()
    {
        yield return new WaitForSeconds(GSManager.Ability.panningCool);
        panningUsable = true;
    }

    IEnumerator pirouetteCool;
    IEnumerator PirouetteCool()
    {
        yield return new WaitForSeconds(GSManager.Ability.pirouetteStunReset);

        pirouetteUsable = true;
        spinGage = 0;
        pirouetteCool = null;
        yield break;
    }

    IEnumerator pirouetteStun;
    IEnumerator PirouetteStun()
    {
        chara.Stun();
        yield return new WaitForSeconds(GSManager.Ability.pirouetteStunReset);

        chara.UnFreeze();
        pirouetteUsable = true;
        spinGage = 0;

        PirouetteGage.Instance()?.gameObject.SetActive(false);
        yield break;
    }

    public void StopPirouette(bool isCool = true)
    {
        animator.SetBool("Spinning", false);
        weapon.GetComponent<WeaponAim>().AimControl = WeaponAim.AimControls.Mouse;
        isSpinning = false;
        jump.AbilityPermitted = true;
        _controller.Parameters.Gravity = chara.OriginalGravity;

        if (weapon.TryGetComponent(out SpriteRenderer _sp))
            _sp.enabled = true;
        var _sprite = weapon.GetComponentsInChildren<SpriteRenderer>();
        foreach (var item in _sprite)
            item.enabled = true;

        inArm?.XFlipOn(false);

        if (isCool)
        {
            pirouetteCool = PirouetteCool();
            StartCoroutine(pirouetteCool);

        }
    }
}
