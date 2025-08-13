using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

    private void Start()
    {
        int score = LoadingScreenUI.Instance.GetData<int>("Score");
        RenderScore(score);
    }

    private void RenderScore(int score)
    {
        _scoreText.text = "Score: " + score.ToString();
    }
}
