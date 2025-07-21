using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

    private void Start()
    {
        RenderScore();
    }

    private void RenderScore()
    {
        int score = GameManager.Instance != null
                    ? GameManager.Instance.Score
                    : 0;
        _scoreText.text = "Score: " + score.ToString();
    }
}
