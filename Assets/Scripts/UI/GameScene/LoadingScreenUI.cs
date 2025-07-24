using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private Scrollbar _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;
    private float _speed = 0.5f;
    private float _currentProgress = 0f;

    public IEnumerator RenderLoadingScene(AsyncOperation loadingOperation)
    {
        while (!loadingOperation.isDone)
        {
            float targetProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            _currentProgress = Mathf.MoveTowards(_currentProgress, targetProgress, _speed * Time.deltaTime);
            _progressBar.value = _currentProgress;
            _progressText.text = Mathf.RoundToInt(_currentProgress * 100f) + "%";
            yield return null;
        }

        _progressBar.value = 1f;
        _progressText.text = "100%";
    }
}
