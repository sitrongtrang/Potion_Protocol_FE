using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private RectTransform _rt;
    private Transform _target;
    private Vector3 _offset;
    private Camera _cam;

    public void Initialize(Transform target, float maxHp, Vector3 offset, Action<float> onHpChanged, Action onDie)
    {
        _target = target;
        _offset = offset;
        _slider.maxValue = maxHp;
        _slider.value = maxHp;
        _rt = GetComponent<RectTransform>();
        _cam = Camera.main;

        // Subscribe vào event
        onHpChanged += SetHp;
        onDie += DestroySelf;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 worldPos = _target.position + _offset;
        Vector3 screenPos = _cam.WorldToScreenPoint(worldPos);
        _rt.position = screenPos;
    }

    public void SetHp(float hp)
    {
        _slider.value = hp;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}