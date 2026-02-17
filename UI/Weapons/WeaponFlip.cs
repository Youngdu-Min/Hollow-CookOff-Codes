using MoreMountains.CorgiEngine;
using UnityEngine;

public class WeaponFlip : MonoBehaviour
{
    public float angle;
    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private GameObject front;

    private Character chara;
    private WeaponAim aim;
    private float lastLocalScaleX;
    private int waitFrame = 2;
    private int currentFrame = 0;
    private bool isWait;
    private SpriteRenderer recticleSprite;

    private void Start()
    {
        if (transform.parent.TryGetComponent(out Character _chara))
        {
            chara = _chara;
        }
        if (TryGetComponent(out WeaponAim _aim))
        {
            aim = _aim;
            recticleSprite = aim.Reticle.GetComponent<SpriteRenderer>();
        }

    }
    // Update is called once per frame
    void Update()
    {
    }
    private void LateUpdate()
    {
        if (chara != null && chara.ConditionState.CurrentState != CharacterStates.CharacterConditions.Normal)
        {
            //캐릭터 상태가 정상인 때만 에이밍 가능
            return;
        }
        if (aim != null)
        {
            if (aim.AimControl == WeaponAim.AimControls.Mouse || aim.AimControl == WeaponAim.AimControls.Spinning || aim.AimControl == WeaponAim.AimControls.Script)
            {
                angle = front.transform.eulerAngles.z;
                if (angle < 90 || angle > 270)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    if (aim.AimControl == WeaponAim.AimControls.Spinning)
                    {
                        transform.localScale = new Vector3(1, -1, 1);
                    }
                    else
                    {
                        transform.localScale = new Vector3(-1, -1, 1);
                    }
                }

                if (transform.localScale.x != lastLocalScaleX)
                {
                    isWait = true;
                    currentFrame = 0;
                }

                if (isWait)
                {
                    currentFrame++;
                    if (currentFrame >= waitFrame)
                    {
                        isWait = false;
                    }
                }

                aim.SetReticleActive(!isWait);
                //print(recticleSprite.enabled);

                lastLocalScaleX = transform.localScale.x;
            }
        }

    }
}
