using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GridCellObject _gridCellObjectPrefab;
    [SerializeField] private float _defaultCellLifetime = 10f;
    [SerializeField] private bool _debug = false;
    
    [Header("Pooling")]
    [SerializeField] private int _initialPoolSize = 100;
    [SerializeField] private int _poolExpandSize = 50;

    // Public properties
    public float DefaultCellLifetime => _defaultCellLifetime;
    public HashSet<int> OverlappedCellIndices { get; } = new HashSet<int>();
    public HashSet<int> WalkableCellIndices { get; } = new HashSet<int>();

    // Private fields
    private GridCellObject[,] _grid;
    private int _width, _height;
    private float _cellSize;
    private Vector2 _origin;
    private string[] _overlapTags;
    private LayerMask _overlapLayers;
    private GameObject _gridContainer;
    private GridCellObject.OnOverlapBox _onOverlapBox;
    private Queue<GridCellObject> _cellPool = new Queue<GridCellObject>();
    private bool _lastDebugState;

    #region INITIALIZATION
    public void InitializeGrid(
        string gridName,
        int width, int height,
        float cellSize,
        Vector2 origin,
        string[] overlapTags,
        LayerMask overlapLayers,
        GridCellObject.OnOverlapBox onOverlapBox,
        Transform parent = null)
    {
        // Clear existing grid
        ClearGrid();

        // Store settings
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _origin = origin;
        _overlapTags = overlapTags;
        _overlapLayers = overlapLayers;
        if (_onOverlapBox == null)
        {
            _onOverlapBox = onOverlapBox;
            _onOverlapBox += HandleCellOverlapChanged;
        }

        // Create container
        _gridContainer = new GameObject(gridName);
        _gridContainer.transform.SetParent(parent);
        _gridContainer.transform.position = Vector3.zero;

        // Initialize grid and pool
        _grid = new GridCellObject[width, height];
        PrewarmPool(_initialPoolSize);

        // Create initial grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GetCell(x, y);
            }
        }
    }

    private void PrewarmPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreatePooledCell();
        }
    }

    private GridCellObject CreatePooledCell()
    {
        var cell = Instantiate(_gridCellObjectPrefab, _gridContainer.transform);
        cell.gameObject.SetActive(false);
        _cellPool.Enqueue(cell);
        return cell;
    }
    #endregion

    #region CELL MANAGEMENT
    public GridCellObject GetCell(int x, int y)
    {
        if (!IsValidGridPosition(x, y)) return null;

        // Return existing cell if available
        if (_grid[x, y] != null)
        {
            _grid[x, y].SetAccessed(true);
            return _grid[x, y];
        }

        // Get or create new cell
        var cell = GetCellFromPool();
        cell.Initialize(
            this, x, y, _cellSize,
            _overlapTags, _overlapLayers,
            _onOverlapBox,
            _defaultCellLifetime);
        
        _grid[x, y] = cell;
        return cell;
    }

    private GridCellObject GetCellFromPool()
    {
        if (_cellPool.Count == 0)
        {
            PrewarmPool(_poolExpandSize);
        }
        
        var cell = _cellPool.Dequeue();
        cell.gameObject.SetActive(true);
        return cell;
    }

    public void ReturnCellToPool(GridCellObject cell)
    {
        if (cell == null) return;

        var pos = cell.GridPosition;
        if (IsValidGridPosition(pos.x, pos.y))
        {
            _grid[pos.x, pos.y] = null;
        }

        cell.ResetCell();
        _cellPool.Enqueue(cell);
    }

    private void HandleCellOverlapChanged(int x, int y, bool isOverlapped)
    {
        int index = x * _height + y;
        
        if (isOverlapped)
        {
            OverlappedCellIndices.Add(index);
            WalkableCellIndices.Remove(index);
        }
        else
        {
            OverlappedCellIndices.Remove(index);
            WalkableCellIndices.Add(index);
        }
    }
    #endregion

    #region GRID OPERATIONS
    public Vector2 GetWorldPosition(int x, int y)
    {
        return _origin + new Vector2(x * _cellSize, y * _cellSize);
    }
    
    public Vector2 GetWorldPosition(GridCellObject gridCellObject)
    {
        return _origin + new Vector2(gridCellObject.GridPosition.x * _cellSize, gridCellObject.GridPosition.y * _cellSize);
    }

    public List<GridCellObject> GetCellsInArea(Vector2 center, Vector2 size)
    {
        var results = new List<GridCellObject>();

        int minX = Mathf.FloorToInt((center.x - size.x / 2 - _origin.x) / _cellSize);
        int maxX = Mathf.CeilToInt((center.x + size.x / 2 - _origin.x) / _cellSize);
        int minY = Mathf.FloorToInt((center.y - size.y / 2 - _origin.y) / _cellSize);
        int maxY = Mathf.CeilToInt((center.y + size.y / 2 - _origin.y) / _cellSize);

        minX = Mathf.Max(0, minX);
        maxX = Mathf.Min(_width - 1, maxX);
        minY = Mathf.Max(0, minY);
        maxY = Mathf.Min(_height - 1, maxY);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                results.Add(GetCell(x, y));
            }
        }

        return results;
    }

    public GridCellObject GetRandomNonoverlapCell()
    {
        if (WalkableCellIndices.Count == 0) return null;

        int randomIndex = Random.Range(0, WalkableCellIndices.Count);
        int i = 0;
        
        foreach (int index in WalkableCellIndices)
        {
            if (i++ == randomIndex)
            {
                int x = index / _height;
                int y = index % _height;
                return GetCell(x, y);
            }
        }

        return null;
    }

    public void ReleaseCell(GridCellObject cell)
    {
        if (cell != null)
        {
            cell.SetAccessed(false);
        }
    }

    public void ClearGrid()
    {
        if (_grid == null) return;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_grid[x, y] != null)
                {
                    ReturnCellToPool(_grid[x, y]);
                }
            }
        }

        if (_gridContainer != null)
        {
            Destroy(_gridContainer);
        }

        OverlappedCellIndices.Clear();
        WalkableCellIndices.Clear();
    }
    #endregion

    #region UTILITY
    private bool IsValidGridPosition(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }

    private void OnValidate()
    {
        if (_lastDebugState != _debug)
        {
            _lastDebugState = _debug;
            // Notify cells of debug state change
        }
    }
    #endregion
}