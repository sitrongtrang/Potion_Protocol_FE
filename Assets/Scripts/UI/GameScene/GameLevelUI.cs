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
    [SerializeField] private GameStateHandler _gameStateHandler;
    [SerializeField] private StartGameHandler _startGameHandler;
    [SerializeField] Animator _timeImageAnimator;
    [SerializeField] Animator _timeTextAnimator;

    void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            LevelManager.Instance.OnScoreChanged += UpdateScoreText;
            LevelManager.Instance.OnTimeChanged += UpdateTimeText;
            LevelManager.Instance.OnPauseToggled += TogglePause;
        }
        else if (SceneManager.GetActiveScene().name == "OnlineGameScene")
        {
            _gameStateHandler.OnScoreChanged += UpdateScoreText;
            _gameStateHandler.OnTimeChanged += UpdateTimeText;
        }
    }

    void OnDisable()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            LevelManager.Instance.OnScoreChanged -= UpdateScoreText;
            LevelManager.Instance.OnTimeChanged -= UpdateTimeText;
            LevelManager.Instance.OnPauseToggled -= TogglePause;
        }
        else if (SceneManager.GetActiveScene().name == "OnlineGameScene")
        {
            _gameStateHandler.OnScoreChanged -= UpdateScoreText;
            _gameStateHandler.OnTimeChanged -= UpdateTimeText;
        }
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = score.ToString();
    }

    public void UpdateTimeText(float time)
    {
        if (time <= 30f && time >= 0.5f)
        {
            _timeImageAnimator.SetBool("isTimeUp", true);
            _timeTextAnimator.SetBool("isTimeUp", true);
        }
        if (time <= 0.5f)
        {
            _timeImageAnimator.SetBool("isTimeOut", true);
            _timeTextAnimator.SetBool("isTimeOut", true);
        }
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