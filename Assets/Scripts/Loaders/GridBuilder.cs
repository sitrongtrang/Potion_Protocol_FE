using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    
    [SerializeField] private GridCellObject _gridCellObjectPrefab;
    [SerializeField] private bool _isDebug = false;
    private bool _lastIsDebug = false;
    public event System.Action<bool> OnDebugChanged;
    [Header("Attributes")]
    private GridCellObject[,] _gridCellObjects;
    private Vector2 _originPosition;
    private float _cellSize;
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

        GameObject gridmap = new(objName);
        gridmap.transform.SetParent(parent);
        gridmap.transform.localPosition = Vector2.zero;

        _gridCellObjects = new GridCellObject[xDim, yDim];

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
}