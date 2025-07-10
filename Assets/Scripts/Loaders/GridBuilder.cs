using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] private GridCellObject _gridCellObjectPrefab;
    [SerializeField] private bool _isDebug = false;
    private bool _lastIsDebug = false;
    public event System.Action<bool> OnDebugChanged;
    private void OnValidate()
    {
        if (_lastIsDebug != _isDebug)
        {
            _lastIsDebug = _isDebug;
            OnDebugChanged?.Invoke(_isDebug);
        }
    }
    public void BuildGrid(
        int xDim,
        int yDim,
        float cellSize,
        Vector2 originPosition,
        string[] overlapTags,
        LayerMask overlapLayerMasks,
        GridCellObject.OnOverlapBox onOverlapBox = null,
        Transform parent = null,
        string objName = "Grid")
    {
        GameObject gridmap = new(objName);
        gridmap.transform.SetParent(parent);
        gridmap.transform.localPosition = Vector2.zero;
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject gridGameObject = Instantiate(_gridCellObjectPrefab.gameObject, gridmap.transform);
                gridGameObject.transform.localPosition = originPosition + new Vector2(x, y) * cellSize;

                GridCellObject gridObject = gridGameObject.GetComponent<GridCellObject>();
                gridObject.Initialize(x, y, cellSize, overlapTags, overlapLayerMasks, onOverlapBox);

                OnDebugChanged += gridObject.SetDebug;
            }
        }
    }
}