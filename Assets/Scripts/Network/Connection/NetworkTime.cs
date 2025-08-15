using System;
using System.Collections;
using TMPro;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkTime : MonoBehaviour
{
    public static NetworkTime Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            NetworkManager.OnDisconnectedComplete += Reconnect;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [Header("Constants")]
    public const int AVG_RTT = 150;
    [Header("Ping Settings")]
    private const double SmoothingFactor = 0.1;
    [SerializeField] private float _pingIntervalInSeconds = 1f;
    private Coroutine _pingRoutine;

    public long EstimatedServerTime => TimeSyncUtils.GetUnixTimeMilliseconds() + ClockOffset;
    public long RoundTripTime { get; private set; }
    public long ClockOffset { get; private set; }
    private bool _awaitingPong;
    public Action<long> OnPingChanged;

    private float _lastPongTime;
    private float _lastMessageTime;
    private float _timeoutThreshold = 5000f;
    [SerializeField] private int _maxMissedPongs = 2; // cho phép mất tối đa 2 pong liên tiếp
    private int _missedPongCount;
    [SerializeField] private StaticURLSO _loginUrl;
    private void Start()
    {
        ResetTimeoutTimers();
        _pingRoutine = StartCoroutine(PingLoop());
    }

    void Update()
    {
        if (TimeSyncUtils.GetUnixTimeMilliseconds() - _lastPongTime > _timeoutThreshold &&
            TimeSyncUtils.GetUnixTimeMilliseconds() - _lastMessageTime > _timeoutThreshold && NetworkManager.Instance.IsAuthenticated)
        {
            _missedPongCount++;
            if (_missedPongCount >= _maxMissedPongs)
            {
                Debug.LogWarning("Mất kết nối, bắt đầu reconnect...");
                _missedPongCount = 0; // reset counter

                // Chỉ NetworkTime gọi reconnect
                NetworkManager.Instance.Disconnect();
                
            }
        }
    }

    void Reconnect()
    {
        StartCoroutine(StartReconnect());
    }

    IEnumerator StartReconnect()
    {
        Debug.Log("Starting reconnect");
        NetworkManager.Instance.ScheduleReconnect();
        if (PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password"))
        {
            if (!NetworkManager.Instance.IsInGame)
            {
                LoginData loginData = new LoginData
                {
                    Username = PlayerPrefs.GetString("Username"),
                    Password = PlayerPrefs.GetString("Password")
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
                    LoginSuccess loginSuccess = JsonConvert.DeserializeObject<LoginSuccess>(request.downloadHandler.text);
                    // GameObject networkManager = new GameObject("Network Manager");
                    // networkManager.AddComponent<NetworkManager>();
                    Debug.Log("AuthToken: ++" + loginSuccess.LoginSuccessDat.Token);
                    NetworkManager.Instance.SetAuthenToken(loginSuccess.LoginSuccessDat.Token);
                    PlayerPrefs.SetString("AuthToken", loginSuccess.LoginSuccessDat.Token);

                }
            }
            
        }
        else
        {
            SceneManager.LoadScene("LoginScene");
        }
        // NetworkManager.Instance.Authenticate();
    }

    public void ResetTimeoutTimers()
    {
        _lastPongTime = TimeSyncUtils.GetUnixTimeMilliseconds();
        _lastMessageTime = TimeSyncUtils.GetUnixTimeMilliseconds();
        _missedPongCount = 0;
    }

    void OnDestroy()
    {
        if (_pingRoutine != null)
        {
            StopCoroutine(_pingRoutine);
            _pingRoutine = null;
        }
    }

    private IEnumerator PingLoop()
    {
        while (true)
        {
            if (NetworkManager.Instance.IsAuthenticated)
                SendPing();
            Debug.Log("Ping");
            yield return new WaitForSeconds(_pingIntervalInSeconds);
        }
    }

    private void SendPing()
    {
        _awaitingPong = true;
        NetworkManager.Instance.SendMessage(new PingMessage());
    }

    public void OnAnyMessageReceived(ServerMessage serverMessage)
    {
        _lastMessageTime = TimeSyncUtils.GetUnixTimeMilliseconds();
    }
    public void HandlePong(PongMessage pong)
    {
        if (!_awaitingPong) return;

        long now = TimeSyncUtils.GetUnixTimeMilliseconds();
        RoundTripTime = now - pong.ClientSendTime;

        // if (RoundTripTime > 300) return; // Ignore bad sample

        long estimatedServerTime = (long)(pong.ServerReceiveTime + (RoundTripTime / 2.0));
        ClockOffset = (long)((1 - SmoothingFactor) * ClockOffset + SmoothingFactor * (estimatedServerTime - now));

        _awaitingPong = false;

        OnPingChanged?.Invoke(RoundTripTime);

        _lastPongTime = TimeSyncUtils.GetUnixTimeMilliseconds();
        _missedPongCount = 0; // reset khi có pong
        Debug.Log($"[TimeSync] RTT: {RoundTripTime:F4}ms, Offset: {ClockOffset:F4}ms, ServerTime: {EstimatedServerTime:F4}ms");
    }
}