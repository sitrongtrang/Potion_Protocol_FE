using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject _controlsTab;
    [SerializeField] private GameObject _soundTab;
    [SerializeField] private GameObject _miscTab;

    void Start()
    {
        ShowSelectedTab((int)SettingsSelection.SelectedTab);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }

    public void ShowSelectedTab(int tabIndex)
    {
        SettingsSelection.Tab tab = (SettingsSelection.Tab)tabIndex;

        SettingsSelection.SelectedTab = tab;

        _controlsTab.SetActive(tab == SettingsSelection.Tab.Controls);
        _soundTab.SetActive(tab == SettingsSelection.Tab.Sound);
        _miscTab.SetActive(tab == SettingsSelection.Tab.Miscellaneous);
    }

    public void Return()
    {
        SceneManager.UnloadSceneAsync("SettingsScene");
    }
}