using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FunctionUntil : MonoBehaviour
{
    public void SetPosition(Transform tr)
    {
        transform.position = tr.position;
    }

    public void LevelCharacterWeaponFlip()
    {
        Transform weaponAttachmentTr = LevelManager.Instance.Players[0].GetComponent<CharacterHandleWeapon>().WeaponAttachment.gameObject.transform;
        weaponAttachmentTr.localScale = new Vector3(weaponAttachmentTr.localScale.x * -1, 1, 1);
    }

    public void SetLevelCharacterBrainReset()
    {
        LevelManager.Instance.Players[0].GetComponent<AIBrain>().ResetBrain();
    }

    public void SetLevelCharacterPosition(Transform tr)
    {
        LevelManager.Instance.Players[0].transform.position = tr.position;
    }

    public void SetLevelCharacterActive(bool active)
    {
        LevelManager.Instance.Players[0].gameObject.SetActive(active);
    }

    public void ChangeTargetSpriteRendererColorBlack(SpriteRenderer sprite)
    {
        sprite.color = Color.black;
    }

    public void ChangeTargetSpriteRendererColorWhite(SpriteRenderer sprite)
    {
        sprite.color = Color.white;
    }

    public void LerpLight2DIntensity(Light2D light)
    {
        StartCoroutine(LightLerp());

        // Ensure that the light intensity is exactly 100 at the end
        light.intensity = 100;

        IEnumerator LightLerp()
        {

            float currentTime = 0;

            while (currentTime <= 1)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / 1; // Calculate the interpolation factor
                light.intensity = Mathf.Lerp(0, 100, t);
                // Wait for the end of frame before continuing the loop
                yield return null;
            }
        }

    }

    public void ChangeScene(string sceneName)
    {
        StartScreen startScreen = FindObjectOfType<StartScreen>();
        startScreen.NextLevel = sceneName;
        startScreen.ButtonPressed();
    }

    public void SetSaveManagerWatchedEnding()
    {
        SaveDataManager.Instance.SetWatchedEnding(true);
    }

}
