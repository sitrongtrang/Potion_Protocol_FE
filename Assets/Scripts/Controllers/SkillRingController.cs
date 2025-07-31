using UnityEngine;

public class SkillRingController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_spriteRenderer.sprite) transform.Rotate(0f, 0f, 360f * Time.deltaTime);
    }
}
