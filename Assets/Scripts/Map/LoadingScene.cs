using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class LoadingScene : MonoBehaviour
{
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private Scrollbar progressBar;
    [SerializeField] private Text progressText;
    private float fakeSpeed = 0.5f;
    public static LoadingScene Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public IEnumerator RenderLoadingScene()
    {
        loadingUI.SetActive(true);
        progressBar.value = 0f;
        progressText.text = "0%";

        float displayProgress = 0f;

        while (displayProgress < 1f)
        {
            displayProgress = Mathf.MoveTowards(displayProgress, 1f, fakeSpeed * Time.deltaTime);
            progressBar.value = displayProgress;
            progressText.text = Mathf.RoundToInt(displayProgress * 100f) + "%";
            yield return null;
        }
        loadingUI.SetActive(false);
    }
}
