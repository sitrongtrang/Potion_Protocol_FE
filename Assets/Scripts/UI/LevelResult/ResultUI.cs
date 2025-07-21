using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private Button _mainButton;
    [SerializeField] private Button _playButton;

    public void OnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
        
    public void OnSelectLevel()
    {
        SceneManager.LoadScene("LevelSelectionScene");
    }
}
