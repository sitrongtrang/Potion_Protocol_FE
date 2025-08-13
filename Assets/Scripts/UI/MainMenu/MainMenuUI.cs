using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _soloPlayButton;
    [SerializeField] private Button _onlinePlayButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _codexButton;


    public void OnSoloPlay()
    {
        StartCoroutine(LoadSelectLevel("LevelSelectionScene"));
    }

    private IEnumerator LoadSelectLevel(string SceneName)
    {
        AsyncOperation request = SceneManager.LoadSceneAsync(SceneName);
        request.completed += async (op) =>
        {
            await LoadingScreenUI.Instance.RenderFinish();
        };
        LoadingScreenUI.Instance.gameObject.SetActive(true);
        List<AsyncOperation> opList = new List<AsyncOperation>();
        opList.Add(request);
        yield return StartCoroutine(LoadingScreenUI.Instance.RenderLoadingScene(opList));
    }

    public void OnOnlinePlay()
    {
        StartCoroutine(LoadSelectLevel("CreateRoomScene"));
    }

    public void OnSettings()
    {
        SettingsSelection.SelectedTab = SettingsSelection.Tab.Controls;
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
    }

    public void OnCodex()
    {
        SceneManager.LoadScene("CodexScene", LoadSceneMode.Additive);
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
