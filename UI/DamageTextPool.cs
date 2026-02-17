using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextPool : MonoBehaviour
{
    public static DamageTextPool Instance;
    [SerializeField]
    private GameObject damageTextPrefab;

    private Queue<DamageText> PoolingDamageText = new Queue<DamageText>();

    private void Awake()
    {
        Instance = this;
    }

    private DamageText CreateNewObject()
    {
        var newObj = Instantiate(damageTextPrefab, transform).GetComponent<DamageText>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private void Initialize(int count)
    {
        for (int i = 0; i < count; i++)
        {
            PoolingDamageText.Enqueue(CreateNewObject());
        }
    }

    public static DamageText GetObject() //옵젝 불러오기
    {
        if (Instance.PoolingDamageText.Count > 0)//풀에 옵젝 있으면
        {
            var obj = Instance.PoolingDamageText.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else //없으면
        {
            var newObj = Instance.CreateNewObject();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(true);
            return newObj;

        }
    }

    public static DamageText GetObject(Vector2 _pos, string _value) //옵젝 불러오기
    {
        if (Instance.PoolingDamageText.Count > 0)//풀에 옵젝 있으면
        {
            var obj = Instance.PoolingDamageText.Dequeue();
            obj.transform.SetParent(null);
            obj.GetComponent<TextMeshPro>().text = _value.ToString();
            obj.transform.position = _pos;
            obj.gameObject.SetActive(true);
            return obj;
        }
        else //없으면
        {
            var newObj = Instance.CreateNewObject();
            newObj.transform.SetParent(null);
            newObj.GetComponent<TextMeshPro>().text = _value.ToString();
            newObj.transform.position = _pos;
            newObj.gameObject.SetActive(true);
            return newObj;

        }
    }

    public static void ReturnObject(DamageText obj)//옵젝 반납
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.PoolingDamageText.Enqueue(obj);
    }
}
