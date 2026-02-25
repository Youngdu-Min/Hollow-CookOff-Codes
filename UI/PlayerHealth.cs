using MoreMountains.CorgiEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct DamageData
{
    public int damage;
    public float damagedTime;
}

public class PlayerHealth : Health
{
    public int hpBuffer = 200;
    [HideInInspector]
    public GameObject catchedEnemy;
    private Queue<DamageData> damageQueue = new Queue<DamageData>();
    private readonly float damageRecoverTime = 0.05f;
    private float invincibleTime;
    public float InvincibleTime => invincibleTime;
    private int decreasePercent = 0;
    [SerializeField] private BlinkAnim blinkAnim;
    private float blinkDuration = 0.81f;
    public Func<GameObject, bool?> IsBlock;

    public override void Revive()
    {
        print("부활");
        //transform.position = 
        base.Revive();
        hpBuffer = 200;
        CurrentWeaponUI.ResetUI();
        WeaponSelectUI.Instance().Reset();
        WeaponSelectUI.Instance().SetPlayer(gameObject);
        WeaponSelectUI.Instance().FinalSelectSubWeapon(WeaponSelectUI.Instance().selectedSubWp);
    }

    protected override void Initialization()
    {
        base.Initialization();
        invincibleTime = HollowBalance.action.actionList[19].floatValue;
        hpBuffer = 200;

        if (SaveDataManager.Instance?.CurrDifficulty == Difficulty.Easy)
            decreasePercent = 60;
        else if (SaveDataManager.Instance?.CurrDifficulty == Difficulty.Normal)
            decreasePercent = 30;
        else if (SaveDataManager.Instance?.CurrDifficulty == Difficulty.Hard)
            decreasePercent = 0;

        OnHit += () => ScoreManager.Instance.score.damageCount++;
    }

    public override void Damage(int damage, GameObject instigator, float flickerDuration,
            float invincibilityDuration, Vector3 damageDirection)
    {
        if (catchedEnemy != null && instigator != catchedEnemy) // 잡혔고 이름이 같으면
        {
            Debug.Log("잡혔으니 리턴 / " + instigator + " / " + catchedEnemy);
            return;

        }

        bool? blockResult = IsBlock?.Invoke(instigator);
        if (blockResult != null && (bool)blockResult)
        {
            print("방어로 인한 데미지 무시");
            return;
        }

        float prevHealth = CurrentHealth;
        damage = damage - (damage * decreasePercent / 100);
        print($"데미지 {damage} {CurrentHealth} {hpBuffer}");

        if (CurrentHealth - damage < hpBuffer)
        {
            damage = CurrentHealth - hpBuffer;
            hpBuffer -= 100;
        }

        DamageOnTouch damageOnTouch = instigator.GetComponent<DamageOnTouch>();
        if (damageOnTouch && damageOnTouch.damageType == DamageOnTouch.DamageType.Grind)
        {
            base.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection);
            DamagedEvent(invincibilityDuration);
        }
        else
        {
            base.Damage(damage, instigator, flickerDuration, invincibleTime, damageDirection);
            DamagedEvent(invincibleTime);
        }

        void DamagedEvent(float time)
        {
            if (prevHealth <= CurrentHealth)
                return;

            if (time < blinkDuration)
                return;

            damageQueue.Enqueue(new DamageData { damage = damage, damagedTime = Time.time });
            blinkAnim.StartBlinking(blinkDuration, (int)(time / blinkDuration));

        }

    }

    public void ParryRecoverHealth()
    {
        if (damageQueue.Count == 0)
            return;

        if (damageRecoverTime > Time.time - damageQueue.Peek().damagedTime)
        {
            print($"체력회복 {CurrentHealth} {damageQueue.Peek().damage}");
            SetHealth(CurrentHealth + damageQueue.Dequeue().damage, gameObject);
        }
        else
            damageQueue.Dequeue();

        if (damageQueue.Count > 0)
            ParryRecoverHealth();
    }

    public override void Kill()
    {
        LevelManager.Instance.RemoveTmpCheckPoint();
        HealthExpend[] tempEnemies = GameObject.FindGameObjectsWithTag("TempEnemy")
    .Select(go => go.GetComponent<HealthExpend>())
    .Where(healthExpend => healthExpend != null)
    .ToArray();

        foreach (HealthExpend healthExpend in tempEnemies)
            Destroy(healthExpend.GetCorgi().gameObject);

        print($"플레이어 사망 {gameObject}");

        if (CamLock.camLock == null)
            return;

        CamLock.camLock.AbleSpawnReset();
        ScoreManager.Instance.score.deathCount += 1;
        AIActionSpawnCharater[] spawnCharacters = FindObjectsOfType<AIActionSpawnCharater>();
        foreach (AIActionSpawnCharater spawnCharacter in spawnCharacters)
            spawnCharacter.KillSpawnedCharacter();

        base.Kill();
    }

}
