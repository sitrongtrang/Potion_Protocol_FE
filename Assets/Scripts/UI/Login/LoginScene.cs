using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    [SerializeField] private Button _mainButton;
    [SerializeField] private InputField _passwordField;
    [SerializeField] private Button _toggleButton;
    [SerializeField] private Sprite _eyeClosedSprite;
    [SerializeField] private Sprite _eyeOpenSprite;           

    private bool _isPasswordVisible = false;
    private Image _buttonImage;

    void Awake()
    {
        _buttonImage = _toggleButton.GetComponent<Image>();
        _toggleButton.onClick.AddListener(TogglePasswordVisibility);
    }

    void TogglePasswordVisibility()
    {
        _isPasswordVisible = !_isPasswordVisible;

        if (_isPasswordVisible)
        {
            _passwordField.contentType = InputField.ContentType.Standard;
            _buttonImage.sprite = _eyeOpenSprite;
        }
        else
        {
            _passwordField.contentType = InputField.ContentType.Password;
            _buttonImage.sprite = _eyeClosedSprite;
        }
        _passwordField.ForceLabelUpdate();
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
