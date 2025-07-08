using UnityEngine;
using UnityEngine.Tilemaps;

public class GridLoader : MonoBehaviour
{
    public static GridLoader Instance { get; private set; }
    private static readonly float CELL_SCALE = 0.5f;
    private Pathfinding _pathfinding;
    [SerializeField] private GridObject _gridObject;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void LoadGrid(GameObject mapGameobject)
    {
        Grid grid = mapGameobject.GetComponent<Grid>();
        Vector2 tileSize = grid.cellSize;

        float cellSize = Mathf.Min(tileSize.x, tileSize.y) * CELL_SCALE;

        Tilemap[] tilemaps = mapGameobject.GetComponentsInChildren<Tilemap>();

        Vector3Int globalMinCell = new(int.MaxValue, int.MaxValue, 0);
        int maxXLength = 0, maxYLength = 0;
        foreach (Tilemap tilemap in tilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            Vector3Int min = bounds.min;

            maxXLength = Mathf.Max(maxXLength, bounds.size.x);
            maxYLength = Mathf.Max(maxYLength, bounds.size.y);

            if (min.x < globalMinCell.x) globalMinCell.x = min.x;
            if (min.y < globalMinCell.y) globalMinCell.y = min.y;
        }
        maxXLength = Mathf.CeilToInt((float)maxXLength / CELL_SCALE);
        maxYLength = Mathf.CeilToInt((float)maxYLength / CELL_SCALE);

        Vector2 bottomLeftWorldPos = grid.GetCellCenterWorld(globalMinCell);

        _pathfinding = new Pathfinding(maxXLength, maxYLength, cellSize, bottomLeftWorldPos);

        GameObject gridmap = new("Grid Map");
        gridmap.transform.SetParent(mapGameobject.transform);
        gridmap.transform.localPosition = Vector2.zero;
        for (int x = 0; x < maxXLength; x++)
        {
            for (int y = 0; y < maxYLength; y++)
            {
                GameObject gridGameObject = Instantiate(_gridObject.gameObject, gridmap.transform);
                gridGameObject.transform.localPosition = bottomLeftWorldPos + new Vector2(x, y) * cellSize;

                gridGameObject.GetComponent<GridObject>().InitializeNode(x, y, cellSize);
            }
        }

        
    }
}