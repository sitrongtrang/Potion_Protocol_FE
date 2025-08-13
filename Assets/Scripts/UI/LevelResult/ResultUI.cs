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
        int score = LoadingScreenUI.Instance.GetData<int>("Score"); 
        int star = LoadingScreenUI.Instance.GetData<int>("Stars"); 
        StartCoroutine(PlayResultSequence(score, star));
    }

    private IEnumerator PlayResultSequence(int score, int star)
    {
        // int[] thresholds = _levelConfig.ScoreThresholds;
        // int starsEarned = 3;

        // for (int i = thresholds.Length - 1; i >= 0; i--)
        // {
        //     if (score >= thresholds[i]) break;
        //     starsEarned--;
        // }

        bool scoreDone = false, starDone = false;

        StartCoroutine(Wrap(_scoreAnim.AnimateScore(score), () => scoreDone = true));
        StartCoroutine(Wrap(_starAnim.AnimateStar(star), () => starDone = true));

        yield return new WaitUntil(() => scoreDone && starDone);

        if (star == 3)
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
