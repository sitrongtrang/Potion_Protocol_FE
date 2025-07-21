using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private GameObject _pauseMenu;

    void Start()
    {
        LevelManager.Instance.OnScoreChanged += UpdateScoreText;
        LevelManager.Instance.OnTimeChanged += UpdateTimeText;
        LevelManager.Instance.OnPauseToggled += TogglePause;
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = score.ToString();
    }

    public void UpdateTimeText(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        _timeText.text = string.Format("{0:mm}:{0:ss}", timeSpan);
    }

    public void TogglePause(bool isPaused)
    {
        _pauseMenu.SetActive(isPaused);
    }

    public void OnSettings()
    {
        SettingsSelection.SelectedTab = SettingsSelection.Tab.Controls;
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Additive);
    }

    public void OnQuit()
    {
        SceneManager.LoadScene("LevelSelectionScene");
    }
}