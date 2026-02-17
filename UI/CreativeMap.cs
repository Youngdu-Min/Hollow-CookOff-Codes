using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.CorgiEngine;
using TMPro;
using System;

public class CreativeMap : MonoBehaviour
{
    // public AudioMixer audioMixer;

    public TMP_Dropdown spawnDropdown;
    public Transform spawnPos;
    public GameObject[] enemyList;
    Health playerHealth;
    public Text playerHealthTxt;
    public Text enemyHealthTxt;
    public Text beInfinTxt;
    public Text heatTxt;
    bool enemyInvincible;
    BioEnerge bio;

    private void Start()
    {
        spawnDropdown.ClearOptions();

        List<string> options = new List<string>();
        // int Idx;
        /*
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIdx = i;
            }

        }
        */

        for (int i = 0; i < enemyList.Length; i++)
        {
            string enemyStr = enemyList[i].gameObject.name;
            options.Add(enemyStr);
        }
        spawnDropdown.AddOptions(options);
        // spawnDropdown.value = Idx;
        spawnDropdown.RefreshShownValue();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<Health>();
        bio = MainCharacter.instance.GetComponent<BioEnerge>();
    }

    public void SpawnEnemy()
    {
        GameObject enemy = enemyList[spawnDropdown.value];
        GameObject obj = Instantiate(enemy, spawnPos.position, Quaternion.identity);
        obj.GetComponent<Health>().Invulnerable = enemyInvincible;
        Debug.Log("무적? " + obj.GetComponent<Health>().Invulnerable);
    }

    public void TogglePlayerInvincible()
    {
        playerHealth.Invulnerable = !playerHealth.Invulnerable;
        if (playerHealth.Invulnerable)
            playerHealth.CurrentHealth = playerHealth.MaximumHealth;
        playerHealthTxt.text = "플레이어 무적: " + playerHealth.Invulnerable.ToString();
    }
    public void ToggleEnemyInvincible()
    {
        enemyInvincible = !enemyInvincible;
        enemyHealthTxt.text = "적 무적: " + enemyInvincible.ToString();
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
        foreach (GameObject go in gos)
        {
            if (go.layer == 13 || go.layer == 29)
            {
                try
                {
                    Health hp = go.GetComponent<Health>();
                    if(hp == null)
                    {
                        hp = go.transform.parent.GetComponent<Health>();
                    }
                    Debug.Log("적 " + go.gameObject.name);
                    hp.Invulnerable = enemyInvincible;

                }
                catch (NullReferenceException) { }
                
            }
        }

    }
    
    public void ToggleBE()
    {
        bio.infinBe = !bio.infinBe;
        if (bio.infinBe)
            bio.RestoreBE(1000);
        beInfinTxt.text = "BE 무한: " + bio.infinBe.ToString();
    }
    public void ToggleHeat()
    {
        OverHeatUI.instance.ignoreHeat = !OverHeatUI.instance.ignoreHeat;
        heatTxt.text = "과열 무시: " + OverHeatUI.instance.ignoreHeat.ToString();
    }
}
