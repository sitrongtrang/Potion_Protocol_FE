using UnityEngine;

[SerializeField] class AudioController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Tên bản nhạc muốn phát")]
    [SerializeField] string musicName;
    private bool isNewMusicName = true;
    [SerializeField] float fadeTime = 1f;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("bgm_volume", 0.8f);
        Time.timeScale = 1;
        AudioManager.Instance.SetVolume(musicName, savedVolume);
        if (!string.IsNullOrEmpty(musicName))
        {
            // Tắt tất cả các bản nhạc đang chạy
            StopAllOtherMusicExcept(musicName);

            // Phát nhạc mới với fade in
            if (isNewMusicName) AudioManager.Instance.FadeIn(musicName, fadeTime);
        }
    }

    void StopAllOtherMusicExcept(string keepName)
    {
        foreach (var sound in AudioManager.Instance.sounds)
        {
            if (sound.name == keepName && sound.source.isPlaying)
            {
                isNewMusicName = false;
                return;
            }
            if (sound.name != keepName && sound.source.isPlaying)
            {
                AudioManager.Instance.FadeOut(sound.name, fadeTime);
            }
        }
    }
}
