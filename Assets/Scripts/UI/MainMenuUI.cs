using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _soloPlayButton;
    [SerializeField] private Button _coopPlayButton;
    [SerializeField] private Button _pvpPlayButton;
    [SerializeField] private Button _controlOptionButton;
    [SerializeField] private Button _soundOptionsButton;
    [SerializeField] private Button _miscOptionsButton;
    [SerializeField] private Button _quitButton;


    public void OnSoloPlay()
    {
        // SceneManager.LoadScene("LevelSelectionScene");
        SceneManager.LoadScene("GameScene");
    }

    public void OnCoopPlay()
    {
        ShowNotImplemented();
    }

    public void OnPvpPlay()
    {
        ShowNotImplemented();
    }

    public void OnSettings(int tab)
    {
        SettingsSelection.SelectedTab = (SettingsSelection.Tab)tab;
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
    }

    public void OnQuit()
    {
        Debug.Log("Quit game");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void ShowNotImplemented()
    {
        Debug.Log("Feature not implemented yet.");
    }
}
