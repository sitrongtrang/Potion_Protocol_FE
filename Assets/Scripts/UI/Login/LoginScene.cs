using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    [SerializeField] private Button _mainButton;
    [SerializeField] private TMP_InputField _passwordField;
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
            _passwordField.contentType = TMP_InputField.ContentType.Standard;
            _buttonImage.sprite = _eyeOpenSprite;
        }
        else
        {
            _passwordField.contentType = TMP_InputField.ContentType.Password;
            _buttonImage.sprite = _eyeClosedSprite;
        }
        _passwordField.ForceLabelUpdate();
    }

    public void OnMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    private IEnumerator LoadMainMenu()
    {
        AsyncOperation request = SceneManager.LoadSceneAsync("MainMenu");
        request.completed += async (op) => 
        {
            await LoadingScreenUI.Instance.RenderFinish();
        }; 
        LoadingScreenUI.Instance.gameObject.SetActive(true);
        List<AsyncOperation> opList = new List<AsyncOperation>();
        opList.Add(request);
        yield return StartCoroutine(LoadingScreenUI.Instance.RenderLoadingScene(opList));
    }
}
