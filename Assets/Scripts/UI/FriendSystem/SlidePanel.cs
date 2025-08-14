using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SlidePanel : MonoBehaviour
{
    public RectTransform panel;
    public float duration = 0.5f;

    void Start()
    {
        panel.anchoredPosition = new Vector2(-Screen.width, 0);
        StartCoroutine(SlideIn());
    }

    IEnumerator SlideIn()
    {
        Vector2 start = new Vector2(-1920, 0);
        Vector2 end = new Vector2(0, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            panel.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = end;
    }
    public void OnButtonClicked()
    {
        StartCoroutine(SlideOut());
    }
    IEnumerator SlideOut()
    {
        Vector2 start = new Vector2(0, 0);
        Vector2 end = new Vector2(-1920, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            panel.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = end;
        SceneManager.UnloadSceneAsync("FriendListScene");
    }
}
