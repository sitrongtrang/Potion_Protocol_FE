using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button _mainButton;
    [SerializeField] private Button _playButton;

    [Header("Animation Controllers")]
    [SerializeField] private ScoreAnim _scoreAnim;
    [SerializeField] private StarAnim _starAnim;
    [SerializeField] private StarDropper _starDrop;


    [Header("Level Config")]
    [SerializeField] private LevelConfig _levelConfig;

    private void Start()
    {
        StartCoroutine(PlayResultSequence());
    }

    private IEnumerator PlayResultSequence()
    {
        int score = GameManager.Instance != null
            ? GameManager.Instance.Score
            : 0;
        int[] thresholds = _levelConfig.ScoreThresholds;
        int starsEarned = 3;

        for (int i = thresholds.Length - 1; i >= 0; i--)
        {
            if (score >= thresholds[i]) break;
            starsEarned--;
        }

        bool scoreDone = false, starDone = false;

        StartCoroutine(Wrap(_scoreAnim.AnimateScore(score), () => scoreDone = true));
        StartCoroutine(Wrap(_starAnim.AnimateStar(starsEarned), () => starDone = true));

        yield return new WaitUntil(() => scoreDone && starDone);

        if (starsEarned == 3)
        {
            _starDrop.SpawnStars();
        }
    }

    private IEnumerator Wrap(IEnumerator inner, Action onComplete)
    {
        yield return StartCoroutine(inner);
        onComplete?.Invoke();
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
        
    public void OnSelectLevel()
    {
        SceneManager.LoadScene("LevelSelectionScene");
    }
}
