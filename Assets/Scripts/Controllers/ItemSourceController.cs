using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemSourceController : MonoBehaviour
{
    private ItemSourceConfig _config;
    private Collider2D _collider2D;
    private ItemSourceSpawner _itemSourceSpawner;
    [Header("Collision")]
    private SpriteRenderer _spriteRenderer;
    private AABBCollider _collider;
    private Vector2 _size = Vector2.zero;

    public ItemSourceConfig Config => _config;
    public AABBCollider Collider => _collider;
    public Vector2 Size => _size;

    public void Initialize(EntityConfig entityConfig, ItemSourceSpawner spawner)
    {
        if (entityConfig is ItemSourceConfig itemSourceConfig)
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
        _collider2D = GetComponent<Collider2D>();
        _itemSourceSpawner = spawner;

        CheckOverlapGrid();
    }

    public void OnFarmed()
    {
        ItemPool.Instance.SpawnItem(_config.DroppedItem, transform.position);
        TrueDie();
    }

    private void CheckOverlapGrid()
    {
        void CheckGrid(string builderName)
        {
            var grid = GridBuilderFactory.Instance.GetBuilder(builderName);
            if (grid == null) return;

            var cells = grid.GetCellsInArea(transform.position, _collider2D.bounds.size);
            foreach (var cell in cells)
            {
                cell.CheckOverlap();
            }
        }
        CheckGrid(GridBuilderFactory.BuilderNames[0]);
        CheckGrid(GridBuilderFactory.BuilderNames[1]);
    }

    private void TrueDie()
    {
        CheckOverlapGrid();
        CollisionSystem.RemoveStaticCollider(_collider);
        _itemSourceSpawner.NotifyItemSourceRemoved(this);
        Destroy(gameObject);
    }
    
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