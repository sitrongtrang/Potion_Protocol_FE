using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private Scrollbar _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;
    private float _speed = 0.5f;
    private float _currentProgress = 0f;
    public Action OnSceneExit;
    public Action OnSceneEnter;
    private readonly Dictionary<string, object> _sceneData = new Dictionary<string, object>();
    public static LoadingScreenUI Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
    }

    #region Data Transfer API
    public void SetData(string key, object value)
    {
        _sceneData[key] = value;
    }

    public T GetData<T>(string key)
    {
        if (_sceneData.TryGetValue(key, out object value) && value is T typedValue)
            return typedValue;
        return default;
    }

    public bool HasData(string key) => _sceneData.ContainsKey(key);

    public void ClearData(string key)
    {
        _sceneData.Remove(key);
    }

    public void ClearAllData()
    {
        _sceneData.Clear();
    }
    #endregion

    public IEnumerator RenderLoadingScene(List<AsyncOperation> loadingOperations)
    {
        _currentProgress = 0f;
        OnSceneExit?.Invoke();
        // Loop until all operations are done
        while (loadingOperations.Exists(op => !op.isDone))
        {
            float total = 0f;
            foreach (var op in loadingOperations)
                total += Mathf.Clamp01(op.progress / 0.9f);

            float avgProgress = total / loadingOperations.Count;
            _currentProgress = Mathf.MoveTowards(_currentProgress, avgProgress, _speed * Time.deltaTime);

            _progressBar.value = _currentProgress;
            _progressText.text = Mathf.RoundToInt(_currentProgress * 100f) + "%";

            yield return null;
        }

        _progressBar.value = 1f;
        _progressText.text = "100%";
        yield return null;
    }

    public async Task RenderFinish()
    {
        while (_currentProgress < 1f)
        {
            _currentProgress = Mathf.MoveTowards(_currentProgress, 1f, _speed * Time.deltaTime);
            _progressBar.value = _currentProgress;
            _progressText.text = Mathf.RoundToInt(_currentProgress * 100f) + "%";

            await Task.Yield(); // Wait for next frame :contentReference[oaicite:2]{index=2}
        }

        _progressBar.value = 1f;
        _progressText.text = "100%";
        OnSceneEnter?.Invoke();
        ClearAllData();
        gameObject.SetActive(false);
    }
}
