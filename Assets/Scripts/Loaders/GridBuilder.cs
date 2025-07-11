using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{

    [SerializeField] private GridCellObject _gridCellObjectPrefab;

    [SerializeField] private bool _isDebug = false;
    private bool _lastIsDebug = false;
    public event System.Action<bool> OnDebugChanged;

    [Header("Attributes")]
    private Vector2 _originPosition;
    private float _cellSize;
    private int _xDim, _yDim;
    private GridCellObject[,] _gridCellObjects;
    private HashSet<int> _gridCellObjectIndicesOverlap = new();
    public HashSet<int> GridCellObjectIndicesOverlap => _gridCellObjectIndicesOverlap;
    private HashSet<int> _gridCellObjectIndicesNotOverlap = new();
    public HashSet<int> GridCellObjectIndicesNotOverlap => _gridCellObjectIndicesNotOverlap;

    private void OnValidate()
    {
        if (_lastIsDebug != _isDebug)
        {
            _lastIsDebug = _isDebug;
            OnDebugChanged?.Invoke(_isDebug);
        }
    }
    public void BuildGrid(
        string objName,
        int xDim,
        int yDim,
        float cellSize,
        Vector2 originPosition,
        string[] overlapTags,
        LayerMask overlapLayerMasks,
        GridCellObject.OnOverlapBox onOverlapBox = null,
        Transform parent = null
        )
    {
        _originPosition = originPosition;
        _cellSize = cellSize;
        _xDim = xDim;
        _yDim = yDim;

        onOverlapBox += (ix, iy, isoverlap) =>
        {
            int index = ix * yDim + iy;
            if (isoverlap)
            {
                _gridCellObjectIndicesOverlap.Add(index);
                _gridCellObjectIndicesNotOverlap.Remove(index);
            }
            else
            {
                _gridCellObjectIndicesOverlap.Remove(index);
                _gridCellObjectIndicesNotOverlap.Add(index);
            }
        };

        GameObject gridmap = new(objName);
        gridmap.transform.SetParent(parent);
        gridmap.transform.localPosition = Vector2.zero;

        _gridCellObjects = new GridCellObject[xDim, yDim];
        OnDebugChanged = null;
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GridCellObject gridGameObject = Instantiate(_gridCellObjectPrefab, gridmap.transform);
                gridGameObject.transform.position = originPosition + new Vector2(x, y) * cellSize;

                gridGameObject.Initialize(x, y, cellSize, overlapTags, overlapLayerMasks, onOverlapBox);

                _gridCellObjects[x, y] = gridGameObject;

                OnDebugChanged += gridGameObject.SetDebug;
            }
        }
    }

    public List<GridCellObject> GetOverlapGridCellObjects(Vector2 centerWorldPosition, float xSizeInFloat = 0, float ySizeInFloat = 0)
    {
        int xCenter = Mathf.FloorToInt((centerWorldPosition.x - _originPosition.x) / _cellSize);
        int yCenter = Mathf.FloorToInt((centerWorldPosition.y - _originPosition.y) / _cellSize);

        int xRadius = Mathf.CeilToInt(xSizeInFloat / (_cellSize * 2));
        int yRadius = Mathf.CeilToInt(ySizeInFloat / (_cellSize * 2));

        int xMin = Mathf.Max(0, xCenter - xRadius);
        int xMax = Mathf.Min(_gridCellObjects.GetLength(0) - 1, xCenter + xRadius);
        int yMin = Mathf.Max(0, yCenter - yRadius);
        int yMax = Mathf.Min(_gridCellObjects.GetLength(1) - 1, yCenter + yRadius);

        var result = new List<GridCellObject>();
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                result.Add(_gridCellObjects[x, y]);
            }
        }
        return result;
    }

    public GridCellObject GetCellObject(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _xDim || y >= _yDim) return null;
        return _gridCellObjects[x, y];
    }

    public GridCellObject GetRandomNotOverlapCell()
    {
        int randIndex = Random.Range(0, _gridCellObjectIndicesNotOverlap.Count);
        int i = 0;
        foreach (int index in _gridCellObjectIndicesNotOverlap)
        {
            if (i == randIndex)
            {
                int x = index / _yDim;
                int y = index % _yDim;
                return GetCellObject(x, y);
            }
            i++;
        }
        return null;
    }

    // public GridCellObject GetRandomGridCellObject(System.Predicate<GridCellObject> condition)
    // {
    //     if (_gridCellObjects == null) return null;

    //     List<GridCellObject> candidates = new();

    //     int width = _gridCellObjects.GetLength(0);
    //     int height = _gridCellObjects.GetLength(1);

    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int y = 0; y < height; y++)
    //         {
    //             GridCellObject cell = _gridCellObjects[x, y];
    //             if (condition == null || condition(cell))
    //             {
    //                 candidates.Add(cell);
    //             }
    //         }
    //     }

    //     if (candidates.Count == 0)
    //         return null;

    //     return candidates[Random.Range(0, candidates.Count)];
    // }
}