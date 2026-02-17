using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using UnityEngine.U2D;

[RequireComponent(typeof (DamageOnTouch), typeof(CompositeCollider2D))]
public class ElectricWallManager : MonoBehaviour
{
    private static ElectricWallManager instance;
    public static ElectricWallManager Instance()
    {
        if (instance == null)
        {
            var ins = new GameObject("ElectricWallManager");
            instance = ins.AddComponent<ElectricWallManager>();
        }
        return instance;
    }
    public GameObject wallPrefab;
    [SerializeField]
    private List<ElectricWall> walls = new List<ElectricWall>();
    [SerializeField]
    private List<Collider2D> wallColliders = new List<Collider2D>();

    private Rigidbody2D rb;
    private CompositeCollider2D composite;
    private DamageOnTouch dot;




    public void AddWall(ElectricWall wall)
    {
        walls.Add(wall);
        wallColliders.Add(wall.GetComponentInChildren<Collider2D>());
    }
    public static ElectricWall CreateNewWall(Vector2 position)
    {
        for (int i = 0; i < instance.walls.Count; i++)
        {
            if (instance.walls[i].gameObject.activeSelf == false)
            {
                instance.walls[i].transform.position = position;
                instance.walls[i].gameObject.SetActive(true);
                return instance.walls[i];
            }
        }
       var _newWall = Instantiate(instance.wallPrefab, instance.transform);
        _newWall.transform.position = position;
        return _newWall.GetComponent<ElectricWall>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (instance == this)
        {
            Initiate();
        }
        else
        {

            Destroy(this);
        }

        //풀링
    }

    private void Initiate()
    {
        rb = instance.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        composite = instance.GetComponent<CompositeCollider2D>();
        composite.isTrigger = true;
        dot = instance.GetComponent<DamageOnTouch>();
        dot.TargetLayerMask = LayerMask.GetMask("Player");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {

        float shortestDist = 9999;
        float currDist;
        GameObject shortestObj = null;
        for (int i = 0; i < wallColliders.Count; i++)
        {
            currDist = Mathf.Abs(wallColliders[i].transform.position.x - col.gameObject.transform.position.x);
            if(shortestDist > currDist)
            {
                shortestObj = wallColliders[i].gameObject;
                shortestDist = currDist;
            }
        }
        dot.Owner = shortestObj;
    }

    

}
