using DG.Tweening;
using MoreMountains.CorgiEngine;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(StratchableStick))]
public class Presser : MonoBehaviour
{
    public SpriteRenderer warnigSprite;
    [SerializeField]
    public BoxCollider2D presserHitbox;

    private StratchableStick stratch;
    private float minY;

    private Vector2 initialPosition;
    private float initialLength;


    [SerializeField]
    [Tooltip("최초 실행까지 대기시간")]
    private float initTime;

    private int _flag;
    private bool crushed = false;
    /*
     * 0 = 경고표시
     * 1 = 내려옴
     * 2 = 올라옴
     * 3 = 대기
     */
    public LayerMask interactable;

    [Header("Time Cycle")]
    [Tooltip("warning: 경고등 깜빡이는 시간")]
    public float warning;
    [Tooltip("holdPress: 완전히 내려온 프레스가 대기하는 시간")]
    public float holdPress;
    [Tooltip("rollbackDelay: 프레스가 다시 올라가는데 걸리는 시간")]
    public float rollbackDelay;
    [Tooltip("waiting: 대기시간")]
    public float waiting;
    private float warningTime = 1f;// 경고등 깜빡이
    private Color warningColor = Color.white;
    private Collider2D[] lastColliders;
    [SerializeField]
    private float pullUpPower = 5f;

    IEnumerator PressCycle(float _wait)
    {
        yield return new WaitForSeconds(_wait);
        //경고등 표시 -> 내려오면서 대미지 -> 원래자리로 돌아감 -> 잠시 대기 -> 반복

        while (true)
        {
            _flag = 0;
            presserHitbox.enabled = false;
            yield return new WaitForSeconds(warning);
            _flag = 1;
            yield return new WaitForSeconds(holdPress);
            _flag = 2;
            PullUp();
            yield return new WaitForSeconds(rollbackDelay);
            _flag = 3;
            presserHitbox.enabled = false;
            presserHitbox.transform.position = initialPosition;
            stratch.SetLength(initialLength);
            crushed = false;
            yield return new WaitForSeconds(waiting);
            _flag = 0;
            warningTime = 1f;
        }
    }

    [ContextMenu("PullUp")]
    public void PullUp()
    {
        if (lastColliders != null)
        {
            foreach (var item in lastColliders)
            {
                float targetY = item.transform.position.y + item.bounds.extents.y;
                print($"pull up {item.name} {targetY}");
                item.transform.DOMove(new Vector3(item.transform.position.x, targetY, item.transform.position.z), 0.1f);
            }
            lastColliders = null;
        }
    }


    private void Awake()
    {
        stratch = GetComponent<StratchableStick>();
        initialPosition = presserHitbox.transform.position;
        initialLength = presserHitbox.size.y;
        warnigSprite.color = new Color(1, 1, 1, 0);
    }
    // Start is called before the first frame update
    void Start()
    {
        minY = transform.position.y;
        _flag = 3;
        StartCoroutine(PressCycle(initTime));
    }

    // Update is called once per frame
    void Update()
    {
        switch (_flag)
        {
            case 0:
                warningColor.a = Mathf.Abs(warningTime % 2 - 1) * 0.7f;
                warnigSprite.color = warningColor;
                warningTime += Time.deltaTime;
                break;
            case 1:
                if (warningColor.a > 0f)
                {
                    warningColor.a -= 3 * Time.deltaTime;
                    warnigSprite.color = warningColor;
                }
                if (crushed)
                {
                    break;
                }


                float distanceBottom = presserHitbox.transform.position.y + presserHitbox.offset.y - presserHitbox.size.y / 2f - minY;

                float pushPower = 100 * Time.deltaTime;
                // Debug.Log(distanceBottom);

                if (pushPower > distanceBottom)
                {
                    var L = presserHitbox.transform.position.y - minY;
                    stratch.SetLength(L * 2);
                    //쾅!!
                    crushed = true;
                    lastColliders = Physics2D.OverlapBoxAll(presserHitbox.transform.position, presserHitbox.size, 0, interactable);
                    foreach (var collision in lastColliders)
                    {
                        if (collision.TryGetComponent(out Health health))
                        {
                            health.Damage(100, this.gameObject, 1, 1, Vector3.down);
                            collision.transform.position = new Vector3(collision.transform.position.x, transform.position.y + collision.transform.localScale.y / 2f, collision.transform.position.z);
                        }
                        if (collision.TryGetComponent(out Character character))
                        {
                            if (health != null && health.CurrentHealth > 0)
                            {
                                character.Stun();
                                character.StartCoroutine(character.UnFreezeTimer(holdPress));
                            }
                        }
                    }
                    presserHitbox.enabled = true;

                }
                else
                {
                    stratch.Stratch(pushPower, false);

                    var collisions = Physics2D.OverlapBoxAll(presserHitbox.transform.position, presserHitbox.size, 0, interactable);
                    foreach (var _col in collisions)
                    {
                        if (!_col.TryGetComponent(out Health health))
                            break;
                        var distance = _col.transform.position.y - _col.transform.localScale.y * 0.5f - minY;
                        _col.transform.Translate(Vector3.down * Mathf.Min(pushPower, distance));
                    }
                }
                break;
            case 2:
                stratch.Stratch(-(initialPosition.y - initialLength * 0.5f - minY) / rollbackDelay * Time.deltaTime, false);
                break;
            default:
                break;
        }
    }
}
