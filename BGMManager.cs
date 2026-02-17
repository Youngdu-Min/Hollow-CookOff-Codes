using MoreMountains.Tools;
using System.Collections;
using UnityEngine;


[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip clip;
}

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    [SerializeField] Sound[] bgmSounds;

    [SerializeField] AudioSource bgmSource;

    private void Awake()
    {

        if (Instance != null)
        {
            Instance.bgmSource?.Stop();
        }
        Instance = this;
        Initiate();

    }

    private void Initiate()
    {
        if (bgmSource == null)
        {
            bgmSource = GetComponent<AudioSource>();
        }

        bgmSource.outputAudioMixerGroup = MMSoundManager.Instance.settingsSo.MusicAudioMixerGroup;
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayRandomBGM();
    }

    public void PlayRandomBGM()
    {
        if (bgmSounds.Length > 0)
        {
            ChangeMusic(bgmSounds[0].clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSoundSpeed()
    {
        bgmSource.pitch = 0.7f;
    }

    public void SetSoundSpeedNomal()
    {
        bgmSource.pitch = 1f;
    }

    public void ChangeMusic(AudioClip newClip)
    {
        if (newClip == Instance.bgmSource.clip)
        {
            //return;
        }
        bgmSource.clip = newClip;
        bgmSource.Play();
    }

    public void FadeOutMusic(float fadeDuration)
    {
        StartCoroutine(FadeAudioSource(0, fadeDuration));
    }

    private IEnumerator FadeAudioSource(float targetVolume, float fadeDuration)
    {
        float startVolume = bgmSource.volume;
        float timer = 0;

        while (bgmSource.volume != targetVolume)
        {
            timer += Time.unscaledDeltaTime;

            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);

            yield return null;
        }

        if (bgmSource.volume <= 0)
            bgmSource.Stop();
    }

}