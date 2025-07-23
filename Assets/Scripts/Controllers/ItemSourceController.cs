using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemSourceController : MonoBehaviour
{
    [SerializeField] private ItemSourceConfig _config;
    public ItemSourceConfig Config => _config;
    private ItemSourceSpawner _itemSourceSpawner;
    [SerializeField] private Collider2D _collider;

    public void Initialize(ItemSourceSpawner spawner)
    {
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

            var cells = grid.GetCellsInArea(transform.position, _collider.bounds.size);
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
        _itemSourceSpawner.NotifyItemSourceRemoved(this);
        Destroy(gameObject);
    }
}