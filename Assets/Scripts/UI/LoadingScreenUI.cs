using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private Scrollbar _progressBar;
    [SerializeField] private Text _progressText;
    private float _speed = 0.5f;

    void OnEnable()
    {
        StartCoroutine(RenderLoadingScene());
    }

    private IEnumerator RenderLoadingScene()
    {
        gameObject.SetActive(true);
        _progressBar.value = 0f;
        _progressText.text = "0%";

        float displayProgress = 0f;

        while (displayProgress < 1f)
        {
            displayProgress = Mathf.MoveTowards(displayProgress, 1f, _speed * Time.deltaTime);
            _progressBar.value = displayProgress;
            _progressText.text = Mathf.RoundToInt(displayProgress * 100f) + "%";
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
