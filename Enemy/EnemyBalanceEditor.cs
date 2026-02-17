using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MoreMountains.CorgiEngine;

public class EnemyBalanceEditor : MonoBehaviour
{
    public List<GameObject> enemys = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        SetStats();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStats()
    {
        int i = 0;
        foreach (var item in enemys)
        {
            if (item != null)
            {
                var data = EnemyBalance.Data.DataMap[i];
                if (data !=null)
                {
                    var controller = item.GetComponent<CorgiController>();
                    controller.Parameters.MaxVelocity = new Vector2(data.speed, data.speed);
                    controller.Parameters.SpeedAccelerationOnGround = data.speed * 0.2f;
                    controller.Parameters.SpeedAccelerationInAir = data.speed * 0.1f;

                    var health = item.GetComponent<HealthExpend>();
                    health.maxArmor = (int)data.armor;
                    health.MaximumHealth = (int)data.hp;
                }
            }
            i++;
        }
    }
}
/*
[CustomEditor(typeof(EnemyBalanceEditor))]
public class EnemyEditor : Editor
{
    EnemyBalanceEditor balance;

    private void OnEnable()
    {
        UnityGoogleSheet.Load<EnemyBalance.Data>();
        balance = target as EnemyBalanceEditor;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

            if (GUILayout.Button("스탯 적용"))
        {
            balance.SetStats();
        }
    }
}
*/
