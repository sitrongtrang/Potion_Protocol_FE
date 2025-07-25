using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MiscSetting : MonoBehaviour
{
    string[] supportedLanguages = { "en", "vi", "ja", "fr" }; // thứ tự khớp với Dropdown
    [SerializeField] Toggle _isAutoFocus;
    [SerializeField] TMP_Dropdown _language;

    public bool AutoFocusValue;
    public string LanguageValue;

    void OnEnable()
    {
        LoadSettings();

        _isAutoFocus.onValueChanged.AddListener(OnAutoFocusChanged);
        _language.onValueChanged.AddListener(OnLanguageChanged);
    }

    public void LoadSettings()
    {
        AutoFocusValue = PlayerPrefs.GetInt("IsAutoFocus", 1) == 1;
        string LanguageValue = PlayerPrefs.GetString("Language", "en");

        _isAutoFocus.isOn = AutoFocusValue;

        int langIndex = System.Array.IndexOf(supportedLanguages, LanguageValue);
        _language.value = langIndex >= 0 ? langIndex : 0;
    }

    void OnAutoFocusChanged(bool value)
    {
        AutoFocusValue = value ? true : false;
    }

    void OnLanguageChanged(int index)
    {
        if (index >= 0 && index < supportedLanguages.Length)
        {
            LanguageValue = supportedLanguages[index];
        }
    }

    public void ResetToDefault()
    {
        // Xóa key hoặc đặt lại giá trị mặc định
        PlayerPrefs.SetInt("IsAutoFocus", 1);
        PlayerPrefs.SetString("Language", "en");
        PlayerPrefs.Save();

        LoadSettings(); // cập nhật lại UI
    }
}