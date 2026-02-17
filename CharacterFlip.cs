using MoreMountains.CorgiEngine;
using UnityEngine;

public class CharacterFlip : MonoBehaviour
{
    private int flip;
    private int lastFlip;

    public Transform body;
    public GameObject filpObj;
    private Character chara;
    private Vector3 mousePosition;
    private ProjectileWeapon projectileWeapon;

    // Start is called before the first frame update
    void Start()
    {
        if (!filpObj)
        {
            // Debug.Log("부가 오브젝트 찾지 못 함");
        }
        if (TryGetComponent(out Character _chara))
        {
            chara = _chara;
        }

        projectileWeapon = GetComponentInChildren<CharacterHandleWeapon>().CurrentWeapon as ProjectileWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        if (chara.ConditionState.CurrentState == CharacterStates.CharacterConditions.Normal &&
            chara.MovementState.CurrentState != CharacterStates.MovementStates.Dashing &&
            chara.MovementState.CurrentState != CharacterStates.MovementStates.Dodging &&
            chara.MovementState.CurrentState != CharacterStates.MovementStates.Meleeing &&
            chara.MovementState.CurrentState != CharacterStates.MovementStates.Airborning)
        {

            mousePosition = MainCamera.Main().ScreenToWorldPoint(Input.mousePosition);

            flip = mousePosition.x > transform.position.x ? 1 : -1;
            projectileWeapon.SetFlipped(flip != lastFlip);
            body.localScale = new Vector3(flip, 1, 1);
            FlipOtherObj();
            lastFlip = flip;
            //print("flip " + flip + " " + gameObject);
        }
    }

    void FlipOtherObj()
    {
        if (!filpObj)
            return;
        float x = Mathf.Abs(filpObj.transform.position.x) * flip;
        filpObj.transform.position = new Vector3(x, filpObj.transform.position.y, filpObj.transform.position.z);
        // Debug.Log("x " + x);]
    }
}
