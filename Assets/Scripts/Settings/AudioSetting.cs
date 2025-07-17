using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{

    
    private void OnEnable()
    {
        // Load volume từ setting hoặc PlayerPrefs (nếu có)
        float savedVolume = PlayerPrefs.GetFloat("bgm_volume", 0.8f);
        GetComponent<Slider>().value = savedVolume;
        AudioManager.Instance.SetGlobalVolume(savedVolume);

        GetComponent<Slider>().onValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(float newVolume)
    {
        AudioManager.Instance.SetGlobalVolume(newVolume);

        // Lưu lại (tuỳ hệ thống của bạn, có thể dùng SettingsManager)
        PlayerPrefs.SetFloat("bgm_volume", newVolume);
        PlayerPrefs.Save();
    }
}