using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] private GridCellObject _gridCellObjectPrefab;
    [SerializeField] private bool _isDebug = false;
    private bool _lastIsDebug = false;
    public event System.Action<bool> OnDebugChanged;
    private GridCellObject[,] _gridCellObjects;
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
}