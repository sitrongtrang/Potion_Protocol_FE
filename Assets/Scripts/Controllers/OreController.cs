using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OreController : MonoBehaviour
{
    private OreConfig _config;
    public OreConfig Config => _config;
    private OreSpawner _oreSpawner;
    [SerializeField] private Collider2D _collider;

    public void Initialize(OreSpawner spawner, OreConfig config)
    {
        _oreSpawner = spawner;
        _config = config;

        CheckOverlapGrid();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.otherCollider);
        if (collision.otherCollider.CompareTag("Weapon"))
        {
            Debug.Log("Colliding weapon");
            PlayerController player = collision.transform.GetComponentInParent<PlayerController>();
            if (player != null && player.Attack.IsAttacking)
            {
                OnFarmed();
            }
        }
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
        _oreSpawner.NotifyOreRemoved(_config);
        Destroy(gameObject);
    }
}