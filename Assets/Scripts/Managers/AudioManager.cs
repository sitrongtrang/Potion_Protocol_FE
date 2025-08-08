using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public bool loop;
        [Range(0f, 1f)] public float volume = 1f;

        [HideInInspector] public AudioSource source;
    }

    public Sound[] sounds;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Tạo AudioSource cho từng sound
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
        }
    }

    public void Play(string name)
    {
        Sound s = FindSound(name);
        if (s != null)
            s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = FindSound(name);
        if (s != null)
            s.source.Stop();
    }

    public void SetVolume(string name, float volume)
    {
        Sound s = FindSound(name);
        if (s != null)
        {
            s.volume = Mathf.Clamp01(volume);
            s.source.volume = Mathf.Clamp01(volume);
        }
    }
    public void SetGlobalVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        foreach (var s in sounds)
        {
            s.volume = volume;          // lưu lại volume gốc
            s.source.volume = volume;   // set cho AudioSource đang phát
        }
    }

    private Sound FindSound(string name)
    {
        return System.Array.Find(sounds, s => s.name == name);
    }
    public void FadeOut(string name, float duration)
    {
        StartCoroutine(FadeSoundOut(FindSound(name), duration));
    }

    public void FadeIn(string name, float duration)
    {
        Sound s = FindSound(name);
        if (s != null)
        {
            s.source.volume = 0;
            s.source.Play();
            StartCoroutine(FadeSoundIn(s, duration));
        }
    }

    private IEnumerator<WaitForSeconds> FadeSoundOut(Sound s, float duration)
    {
        if (s == null)
        {
            Debug.Log("FadeOut failed: sound null");
            yield break;
        }

        float startVolume = s.source.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            s.source.volume = Mathf.Lerp(startVolume, 0, time / duration);
            yield return null;
        }

        s.source.Stop();
        s.source.volume = startVolume;
    }

    private IEnumerator<WaitForSeconds> FadeSoundIn(Sound s, float duration)
    {
        if (s == null) yield break;
        float targetVolume = s.volume;
        s.source.volume = 0f;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            s.source.volume = Mathf.Lerp(0f, targetVolume, time / duration);
            yield return null;
        }

        s.source.volume = targetVolume;
    }
}
