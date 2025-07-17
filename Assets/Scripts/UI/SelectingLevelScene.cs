using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectingLevelScene : MonoBehaviour
{
    [SerializeField] private Button _mainButton;

    public void OnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
