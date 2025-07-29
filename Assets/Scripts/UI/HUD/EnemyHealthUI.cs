using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private float _timeHealth;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Gradient _healthGradient;
    private RectTransform _rt;
    private Transform _target;
    private Vector3 _offset;
    private Camera _cam;
    private CanvasGroup _canvasGroup;
    private Coroutine _hideCoroutine;

    private void Awake()
    {
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    public void Initialize(Transform target, float maxHp, Vector3 offset)
    {
        _target = target;
        _offset = offset;
        _slider.maxValue = maxHp;
        _slider.value = maxHp;
        _rt = GetComponent<RectTransform>();
        _cam = Camera.main;
    }

    void LateUpdate()
    {
        if (_target == null) return;

        Vector3 worldPos = _target.position + _offset;
        Vector3 screenPos = _cam.WorldToScreenPoint(worldPos);
        _rt.position = screenPos;
    }

    public void SetHp(float hp)
    {
        _slider.value = hp;
        float percent = hp / _slider.maxValue;
        Color col = _healthGradient.Evaluate(percent);
        _fillImage.color = col;
        ShowAndScheduleHide();
    }

    private void ShowAndScheduleHide()
    {
        _canvasGroup.alpha = 1f;

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);
        //_hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_timeHealth);
        _canvasGroup.alpha = 0f;
        _hideCoroutine = null;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}