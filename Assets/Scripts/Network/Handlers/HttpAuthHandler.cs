using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // optional, if using TextMeshPro UI

public class HttpAuthHandler : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;

    [Header("Register")]
    [SerializeField] private TMP_InputField registerusernameField;
    [SerializeField] private TMP_InputField registerpasswordField;
    [SerializeField] private TMP_InputField confirmpasswordField;
    [SerializeField] private TMP_InputField displaynameField;

    [Header("URLs")]
    [SerializeField] private StaticURLSO _loginUrl;
    [SerializeField] private StaticURLSO _registerUrl;

    [Header("UI")]
    [SerializeField] private GameObject _loginError;
    [SerializeField] private GameObject _registerError;
    [SerializeField] private float _disableAfterSeconds;
    [SerializeField] private Canvas _loginCanvas;
    [SerializeField] private Canvas _signupCanvas;

    private Coroutine _loginErrorDisable;
    private Coroutine _registerErrorDisable;

    void Start()
    {
        if (NetworkManager.Instance.IsAuthenticated)
        {
            NetworkManager.Instance.Authenticate();
            StartCoroutine(LoadMainMenu(true));
        }
    }

    public void OnLoginButtonPressed()
    {
        StartCoroutine(SendLoginRequest(usernameField.text, passwordField.text));
    }

    IEnumerator SendLoginRequest(string username, string password)
    {
        LoginData loginData = new LoginData
        {
            Username = username,
            Password = password
        };

        string json = JsonConvert.SerializeObject(loginData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(_loginUrl.StaticURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log("Login successful: " + request.downloadHandler.text);
            // Parse token or user info if needed

            LoginSuccess loginSuccess = JsonConvert.DeserializeObject<LoginSuccess>(request.downloadHandler.text);
            // GameObject networkManager = new GameObject("Network Manager");
            // networkManager.AddComponent<NetworkManager>();
            NetworkManager.Instance.SetAuthenToken(loginSuccess.LoginSuccessDat.Token);
            NetworkManager.Instance.Authenticate();

            StartCoroutine(LoadMainMenu(true));
            // LoadTestScene();
        }
        else
        {
            Debug.LogError("Login failed: " + request.error);
            if (_loginErrorDisable != null) StopCoroutine(_loginErrorDisable);
            _loginError.SetActive(true);
            _loginErrorDisable = StartCoroutine(DisableLoginError());
            // LoadTestScene();
        }
    }

    private void LoadTestScene()
    {
        SceneManager.LoadSceneAsync("FriendListScene");
    }

    public void OnPlayAsGuest()
    {
        StartCoroutine(LoadMainMenu(false));
    }

    private IEnumerator LoadMainMenu(bool isOnline)
    {
        LoadingScreenUI.Instance.SetData("Online", isOnline);
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

    private IEnumerator DisableLoginError()
    {
        yield return new WaitForSeconds(_disableAfterSeconds);
        _loginError.SetActive(false);
    }

    private IEnumerator DisableRegisterError()
    {
        yield return new WaitForSeconds(_disableAfterSeconds);
        _registerError.SetActive(false);
    }

    public void OnRegisterButtonPressed()
    {
        StartCoroutine(SendRegisterRequest(registerusernameField.text, registerpasswordField.text, confirmpasswordField.text, displaynameField.text));
    }

    IEnumerator SendRegisterRequest(string username, string password, string confirmPassword, string displayName)
    {
        RegisterData registerData = new RegisterData
        {
            Username = username,
            Password = password,
            ConfirmPassword = confirmPassword,
            DisplayName = displayName
        };

        string json = JsonConvert.SerializeObject(registerData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(_registerUrl.StaticURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("UserName: " + username);
        Debug.Log("Password: " + password);
        Debug.Log("Display Name: " +  displayName);
        Debug.Log("Confirm Password: " + confirmPassword);

        if (request.result == UnityWebRequest.Result.Success)
        {
            RegisterSuccess resp = JsonConvert.DeserializeObject<RegisterSuccess>(
                request.downloadHandler.text
            );
            Debug.Log("Register successful! Message: " + resp.Message);
            showLogin();
        }
        else
        {
            Debug.LogError("Register failed: " + request.error);
            if (_registerErrorDisable != null) StopCoroutine(_registerErrorDisable);
            _registerError.SetActive(true);
            _registerErrorDisable = StartCoroutine(DisableRegisterError());
        }
    }

    public void showSignup()
    {
        _loginCanvas.gameObject.SetActive(false);
        Reset();
        _signupCanvas.gameObject.SetActive(true);
    }

    public void showLogin()
    {
        _signupCanvas.gameObject.SetActive(false);
        Reset();
        _loginCanvas.gameObject.SetActive(true);
    }

    private void Reset()
    {
        usernameField.text = string.Empty;
        passwordField.text = string.Empty;
        registerusernameField.text = string.Empty;
        registerpasswordField.text = string.Empty;
        confirmpasswordField.text = string.Empty;
        displaynameField.text = string.Empty;
        _registerError.SetActive(false);
        _loginError.SetActive(false);
    }
}
