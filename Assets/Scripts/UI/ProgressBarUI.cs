using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private Transform _target;
    private Vector3 _offset;
    private RectTransform _rt;
    private Camera _cam;

    public void Initialize(Transform target, float finishTime, Vector3 offset)
    {
        _target = target;
        _offset = offset;
        _slider.maxValue = finishTime;
        _slider.value = 0.4f;
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

    public void SetProgress(float value)
    {
        if (_slider != null)
        {
            _slider.value = value;
        }
    }
}