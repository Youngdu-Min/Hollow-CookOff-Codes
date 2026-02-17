using UnityEngine;

public class RebindAnimation : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private Animator animator;

    private void Start()
    {
        animator = target.GetComponent<Animator>();
        target.SetActive(false);
        animator.enabled = false;
    }

    public void SetRebind()
    {

        target.SetActive(true);
        animator.enabled = true;
        animator.Rebind();
    }


}
