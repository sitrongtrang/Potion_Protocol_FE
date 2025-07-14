using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OreController : MonoBehaviour
{
    private OreConfig _config;
    public OreConfig Config => _config;
    [SerializeField] private Collider2D _collider;
    public void Initialize(OreConfig config)
    {
        _config = config;

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
        Destroy(gameObject);
    }
}