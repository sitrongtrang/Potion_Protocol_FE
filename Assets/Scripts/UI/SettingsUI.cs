using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public GameObject controlsTab;
    public GameObject soundTab;
    public GameObject miscTab;

    void Start()
    {
        controlsTab.SetActive(false);
        soundTab.SetActive(false);
        miscTab.SetActive(false);

        switch (SettingsSelection.SelectedTab)
        {
            case SettingsSelection.Tab.Controls:
                controlsTab.SetActive(true);
                break;
            case SettingsSelection.Tab.Sound:
                soundTab.SetActive(true);
                break;
            case SettingsSelection.Tab.Miscellaneous:
                miscTab.SetActive(true);
                break;
        }
    }

    public void ShowControlsTab()
    {
        SettingsSelection.SelectedTab = SettingsSelection.Tab.Controls;
        controlsTab.SetActive(true);
        soundTab.SetActive(false);
        miscTab.SetActive(false);
    }
    public void ShowSoundTab()
    {
        SettingsSelection.SelectedTab = SettingsSelection.Tab.Sound;
        controlsTab.SetActive(false);
        soundTab.SetActive(true);
        miscTab.SetActive(false);
    }
    public void ShowMiscTab()
    {
        SettingsSelection.SelectedTab = SettingsSelection.Tab.Miscellaneous;
        controlsTab.SetActive(false);
        soundTab.SetActive(false);
        miscTab.SetActive(true);
    }
}