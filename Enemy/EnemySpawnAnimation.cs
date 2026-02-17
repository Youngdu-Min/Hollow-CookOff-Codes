using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class EnemySpawnAnimation : MonoBehaviour
{

    public float timer = 0f;
    private AIBrain brain;
    private Animator animator;


    public bool hasSpawnAnimation = true;

    [SerializeField]
    private SpriteRenderer[] sprites;

    public float colorGradiantTime = 0.5f;
    public float opacityGradiantTime = 0.5f;

    private IEnumerator _spawnAnim;
    IEnumerator SpawnAnimation()
    {
        if (brain != null)
        {

            brain.BrainActive = false;
        }
        if (animator != null)
        {
            animator.SetBool("Spawning", true);
            animator.enabled = false;
        }


        sprites = GetComponentsInChildren<SpriteRenderer>();
        timer = 0f;

        float colorRatio = 0f;
        float opacityRatio = 0f;
        Color color = new Color();

        while (opacityRatio < 1f)
        {
            print("opacityRatio : " + opacityRatio);
            opacityRatio = timer / opacityGradiantTime;
            color.a = opacityRatio;

            foreach (var item in sprites)
            {
                item.color = color;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (colorRatio < 1f)
        {
            print("colorRatio : " + colorRatio);
            colorRatio = timer / colorGradiantTime;

            color.r = colorRatio;
            color.g = colorRatio;
            color.b = colorRatio;

            foreach (var item in sprites)
            {
                item.color = color;
            }
            timer += Time.deltaTime;
            yield return null;
        }


        if (brain != null)
        {
            brain.BrainActive = true;

        }
        if (animator != null)
        {
            animator.SetBool("Spawning", false);
            animator.enabled = true;
        }
        yield break;
    }

    private void Awake()
    {
        brain = GetComponentInChildren<AIBrain>();

        animator = GetComponentInChildren<Animator>();
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        Init();
    }

    [ContextMenu("Init")]
    public void Init()
    {
        if (hasSpawnAnimation)
        {

            if (_spawnAnim != null) StopCoroutine(_spawnAnim);
            _spawnAnim = SpawnAnimation();
            StartCoroutine(_spawnAnim);
        }
        else
        {
            animator.SetBool("Spawning", false);
        }
    }
}
