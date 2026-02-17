using DG.Tweening;
using MoreMountains.CorgiEngine;
using UnityEngine;

public class TargetMoveTo : MonoBehaviour
{
    private Character _character;
    private CorgiController _controller;
    [SerializeField]
    private string _targetName;

    // Start is called before the first frame update
    void Start()
    {
        _character = GameObject.FindGameObjectWithTag(_targetName).GetComponent<Character>();
        _controller = _character.GetComponent<CorgiController>();
    }

    public void MoveTo(GameObject target)
    {
        _character.Freeze();
        _controller.transform.DOMove(target.transform.position, 1.0f).OnComplete(() => { _character.UnFreeze(); });
    }
}
