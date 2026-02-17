using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float initAngle = 0;
    private Vector3 rotationAxis = Vector3.back;

    void Start()
    {
        if (initAngle != default)
        {
            transform.RotateAround(target.position, rotationAxis, initAngle);
            transform.rotation = Quaternion.identity;
        }

    }

    void FixedUpdate()
    {
        transform.RotateAround(target.position, rotationAxis, rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.identity;
    }
}