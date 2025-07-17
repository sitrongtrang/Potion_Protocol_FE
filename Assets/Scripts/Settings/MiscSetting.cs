using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MiscSetting : MonoBehaviour
{
    string[] supportedLanguages = { "en", "vi", "ja", "fr" }; // thứ tự khớp với Dropdown
    [SerializeField] Toggle _isAutoFocus;
    [SerializeField] TMP_Dropdown _language;

    private const string AutoFocusKey = "IsAutoFocus";
    private const string LanguageKey = "Language";

    void OnEnable()
    {
        LoadSettings();

        _isAutoFocus.onValueChanged.AddListener(OnAutoFocusChanged);
        _language.onValueChanged.AddListener(OnLanguageChanged);
    }

    void LoadSettings()
    {
        bool isAutoFocus = PlayerPrefs.GetInt(AutoFocusKey, 1) == 1;
        string lang = PlayerPrefs.GetString(LanguageKey, "en");

        _isAutoFocus.isOn = isAutoFocus;

        int langIndex = System.Array.IndexOf(supportedLanguages, lang);
        _language.value = langIndex >= 0 ? langIndex : 0;
    }

    void OnAutoFocusChanged(bool value)
    {
        PlayerPrefs.SetInt(AutoFocusKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnLanguageChanged(int index)
    {
        if (index >= 0 && index < supportedLanguages.Length)
        {
            string selectedLang = supportedLanguages[index];
            PlayerPrefs.SetString(LanguageKey, selectedLang);
            PlayerPrefs.Save();
        }
    }

    public void ResetToDefault()
    {
        // Xóa key hoặc đặt lại giá trị mặc định
        PlayerPrefs.SetInt(AutoFocusKey, 1);
        PlayerPrefs.SetString(LanguageKey, "en");
        PlayerPrefs.Save();

        LoadSettings(); // cập nhật lại UI
    }
}