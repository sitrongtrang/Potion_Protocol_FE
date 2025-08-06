using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private Transform _target;
    private Vector3 _offset;
    private RectTransform _rt;
    private Camera _cam;

    public void Initialize(Transform target, Vector3 offset)
    {
        _target = target;
        _offset = offset;
        _slider.value = 0.4f;
        _rt = GetComponent<RectTransform>();
        _cam = Camera.main;
        if (_target != null)
        {
            Vector3 worldPos = _target.position + _offset;
            Vector3 screenPos = _cam.WorldToScreenPoint(worldPos);
            _rt.position = screenPos;
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetProgress(float value)
    {
        if (_slider != null)
        {
            _slider.value = value;
        }
    }

    public void SetMaxValue(float value)
    {
        if (_slider != null)
        {
            _slider.maxValue = value;
        }
    }

    public void StartProgress(float duration)
    {
        _slider.maxValue = duration;
        StartCoroutine(AnimateProgress(duration));
    }

    public IEnumerator AnimateProgress(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetProgress(elapsed);
            yield return null;
        }
    }
}