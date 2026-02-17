using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverHeatUI : MonoBehaviour
{
    public static OverHeatUI instance;

    private Character chara;

    private int subWeaponIndex, mainWpIndex;
    public float gaugeDuration;
    public float reduceSpeed;


    public static bool isCookOff = false;

    [SerializeField]
    private Color normal, danger, cookoff;
    [SerializeField]
    private TextMeshProUGUI warinng;
    private SubWeapon sub;
    [SerializeField]
    private Image gauge;
    [SerializeField]
    private List<float> heat = new List<float>();

    public static float GetHeat(int index)
    {
        return instance.heat[index];
    }

    [SerializeField]
    private List<float> lastTime = new List<float>();
    private bool updated = true;
    public bool ignoreHeat;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            return;
        }

        instance = this;
        Initiate();
    }

    private void Initiate()
    {
        for (int i = 0; i < 5; i++)
        {
            heat.Add(0);
            lastTime.Add(0);
        }
    }

    public static void Heat(int index)
    {
        if (instance.ignoreHeat)
            return;
        //Debug.Log("Weapon IDX " + index);
        instance.mainWpIndex = index;
        instance.lastTime[index] = Time.time;

        if (instance.heat[index] >= 100)
        {
            return;
        }
        instance.heat[index] += WeaponTable.BaseData.BaseDataMap[index].overHeat;
        print($"Heat : {instance.heat[index]} {WeaponTable.BaseData.BaseDataMap[index].overHeat}");
        if (instance.heat[index] >= 100)    //과열수치가 100 이상이 되면
        {
            instance.heat[index] = 100;
            instance.StartCoroutine(instance.IECookOff(index));
        }
        instance.gauge.fillAmount = instance.heat[index] / 100;
        instance.UpdateMainWeapon();
    }

    private IEnumerator IECookOff(int _index)
    {
        isCookOff = true;
        float duration = 3;
        var weapon = (ProjectileWeapon)MainCharacter.instance.GetComponent<CharacterHandleWeapon>().CurrentWeapon;
        float tick = weapon.TimeBetweenUses;
        var _weaponType = weapon.weaponType;
        MainCharacter.instance.GetComponent<CharacterHandleWeapon>().AbilityPermitted = false;

        while (duration > 0)
        {
            yield return new WaitForSeconds(tick);
            if (weapon == null)
            {
                weapon = (ProjectileWeapon)MainCharacter.instance.GetComponent<CharacterHandleWeapon>().CurrentWeapon;
            }
            if (weapon != null && weapon.weaponType == _weaponType)
            {

                weapon.ManualUse();
                //마우스 랜덤이동
                MouseMovement.Move(Vector2.up * Random.Range(50, -30));
            }
            duration -= tick;
        }
        heat[_index] = 0;
        instance.UpdateMainWeapon();
        if (weapon != null && weapon.weaponType == _weaponType)
        {

            MainCharacter.instance.GetComponent<CharacterHandleWeapon>().AbilityPermitted = true;
        }
        isCookOff = false;
    }

    public static void ChangeMainWeapon(int index)
    {
        float interval = Time.time - instance.lastTime[index] - instance.gaugeDuration;
        if (interval > 0 && !isCookOff)
        {
            instance.heat[index] -= interval * instance.reduceSpeed;
            instance.heat[index] = Mathf.Max(0, instance.heat[index]);
        }
        instance.mainWpIndex = index;
        instance.UpdateMainWeapon();
    }

    public static void ChangeSubWeapon(int index)
    {
        instance.subWeaponIndex = index;
        instance.updated = true;
        //instance.subGauge.fillAmount = instance.sub.Percentage(index);
    }

    private void UpdateMainWeapon()
    {
        gauge.fillAmount = heat[mainWpIndex] / 100;
        if (gauge.fillAmount < 0.8f)    //게이지 80%미만
        {
            warinng.text = null;
            gauge.color = normal;
        }
        else if (gauge.fillAmount < 1f)//게이지 80~99
        {
            warinng.text = "<b>D A N G E R<b>";
            warinng.color = danger;
            gauge.color = danger;
        }
        else    //게이지 100%
        {
            instance.warinng.text = "<b>C O O K O F F<b>";
            warinng.color = cookoff;
            gauge.color = cookoff;
        }

    }
    private void UpdateSubWeapon(int index)
    {
        float _gauge = sub.Percentage(index);
        // subGauge.fillAmount = _gauge;
        if (_gauge == 1f)
        {
            instance.updated = false;
        }
        // Debug.Log(instance.sub.Percentage(index));
        //미완성
    }
    private void Start()
    {
        sub = MainCharacter.instance.GetComponent<SubWeapon>();
        chara = MainCharacter.instance.GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            updated = true;
        }

        if (updated)
        {
            UpdateSubWeapon(subWeaponIndex);
            UpdateMainWeapon();
        }
    }
    private void FixedUpdate()
    {
        if (chara.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
        {
            for (int i = 0; i < lastTime.Count; i++)
            {
                lastTime[i] += Time.fixedDeltaTime;
            }
        }
        if (heat[mainWpIndex] > 0 && lastTime[mainWpIndex] + gaugeDuration < Time.time)
        {
            heat[mainWpIndex] -= Time.fixedDeltaTime * reduceSpeed;
            UpdateMainWeapon();
        }
    }
}
