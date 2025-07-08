using UnityEngine;
using UnityEngine.Tilemaps;

public class GridLoader : MonoBehaviour
{
    public static GridLoader Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void LoadGrid(GameObject map)
    {
        Grid grid = map.GetComponent<Grid>();
        Vector3 tileSize = grid.cellSize;
        // Vector2 tileSize = tilemap.cellSize;

        // BoundsInt bounds = tilemap.cellBounds;
        // int tilesX = bounds.size.x;
        // int tilesY = bounds.size.y;

        // Vector3Int bottomLeftCell = tilemap.cellBounds.min; // Bottom-left corner cell in grid space
        // Vector2 worldPos = tilemap.CellToWorld(bottomLeftCell);
    }
}