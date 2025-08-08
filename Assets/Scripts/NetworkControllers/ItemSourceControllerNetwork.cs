using UnityEngine;

public class ItemSourceControllerNetwork : NetworkBehaviour
{
    private ItemSourceConfig _config;
    private SpriteRenderer _spriteRenderer;
    private AABBCollider _collider;
    private Vector2 _size = Vector2.zero;

    public ItemSourceConfig Config => _config;

    void OnDestroy()
    {
        CollisionSystem.RemoveStaticCollider(_collider);
    }

    #region Initialization
    public override void Initialize(string entityId, ScriptableObject scriptableObject)
    {
        EntityId = entityId;
        if (scriptableObject is ItemSourceConfig itemSourceConfig)
        {
            _config = itemSourceConfig;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer)
            {
                _spriteRenderer.sprite = itemSourceConfig.Icon;
                SetCollider();
                CollisionSystem.InsertStaticCollider(_collider);
            }
        }
    }
    #endregion

    public void SetCollider()
    {
        if (_collider == null)
        {
            AABBCollider temp = AABBCollider.GetColliderBaseOnSprite(_spriteRenderer, transform);
            _collider = new AABBCollider(temp)
            {
                Layer = (int)EntityLayer.ItemSource,
                Owner = gameObject
            };
        }
        else
        {
            _collider.SetSize(_size);
            Vector2 center = transform.position;
            _collider.SetBottomLeft(center - _size / 2f);
        }
    }
}