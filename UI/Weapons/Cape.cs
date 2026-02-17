using MoreMountains.CorgiEngine;
using UnityEngine;

public class Cape : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    private Health hp;

    CharacterFlip flip;

    [SerializeField]
    private Transform parant;
    private PlayerHealth playerHealth;
    private Character character;

    WeaponAim aim;

    private void Awake()
    {
        parant = transform.parent;
        flip = GetComponentInParent<CharacterFlip>();
        playerHealth = parant.GetComponent<PlayerHealth>();
        character = playerHealth.GetComponent<Character>();
    }
    private void Start()
    {
        if (TryGetComponent(out Health _hp))
        {
            hp = _hp;
            _hp.MaximumHealth = (int)GSManager.Cloak.maxHp;
        }
        if (TryGetComponent(out BoxCollider2D _box))
        {
            _box.size = GSManager.Cloak.hitBox;
            _box.offset = GSManager.Cloak.hitPosition;
        }

        hp.InitialHealth = hp.MaximumHealth;
        print($"스프라이트 {hp.CurrentHealth} {hp.MaximumHealth} {(float)hp.CurrentHealth / hp.MaximumHealth}");

        playerHealth.IsBlock = (GameObject damageObj) =>
        {
            if (!gameObject.activeInHierarchy)
                return false;

            if ((damageObj.transform.position.x < transform.position.x && transform.localScale.x < 0)
            || (damageObj.transform.position.x > transform.position.x && transform.localScale.x > 0))
                return true;
            return false;
        };
    }
    // Update is called once per frame
    void Update()
    {

        if (OverHeatUI.isCookOff)
        {
            gameObject.SetActive(false);
        }

        if (SubWeapon.currentSubWp != SubWeaponType.Cloak)
        {
            SubWeapon.Instance.EndCloak();
            return;
        }

        print($"스프라이트 {hp.CurrentHealth} {hp.MaximumHealth} {(float)hp.CurrentHealth / hp.MaximumHealth}");
        sprite.color = Color.Lerp(Color.red, Color.white, (float)hp.CurrentHealth / hp.MaximumHealth);

        if (hp.CurrentHealth <= 0)
        {
            sprite.gameObject.SetActive(false);
            SubWeapon.Instance.EndCloak();
            return;
            //spriteGo.SetActive(false);
            //character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
        }
        //else
        //{
        //    spriteGo.SetActive(true);
        //    character.MovementState.ChangeState(CharacterStates.MovementStates.Blocking);
        //}




    }

    private void OnEnable()
    {
        if (this.transform.position.x < Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        if (flip != null)
        {
            flip.enabled = false;
        }

        aim = parant.GetComponentInChildren<WeaponAim>();

        aim.AimControl = WeaponAim.AimControls.Off;
        if (aim.transform.localScale.x > 0)
        {
            aim.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            aim.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
        //aim.transform.localScale = Vector3.one;
    }

    private void OnDisable()
    {
        if (flip != null)
        {
            flip.enabled = true;
        }
        aim = parant.GetComponentInChildren<WeaponAim>();
        aim.AimControl = WeaponAim.AimControls.Mouse;
    }
}
