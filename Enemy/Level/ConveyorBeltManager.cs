using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltManager : MonoBehaviour
{
    public GameObject beltPrefab;

    public static ConveyorBeltManager instance;


    public static Dictionary<Vector2, GameObject> Belts = new Dictionary<Vector2, GameObject>();

    private void Awake()
    {
        instance = this;
    }

    public static void ContainUnit(GameObject newBeltParts, Vector2 _direction)
    {
        //이속이 같은 벨트끼리 그룹으로 묶음
        if (Belts.ContainsKey(_direction))
        {
            newBeltParts.transform.SetParent(Belts[_direction].transform);
        }
        else
        {
            // Debug.Log("벨트 그룹 생성");
            var belt = Instantiate(instance.beltPrefab, instance.transform);
            belt.transform.position = Vector3.zero;
            belt.GetComponent<ConveyorBelt>().SetDirection(_direction);
            Belts.Add(_direction, belt);

            newBeltParts.transform.SetParent(belt.transform);
        }
    }
}
