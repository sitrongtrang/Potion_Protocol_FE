using System.Collections.Generic;
using UnityEngine;

public class GridBuilderFactory : MonoBehaviour
{
    public static GridBuilderFactory Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    [SerializeField] private GridBuilder _gridBuilder;
    private List<GridBuilder> _gridBuilders = new();

    public void BuildGrid(
        int xDim,
        int yDim,
        float cellSize,
        Vector2 originPosition,
        string[] overlapTags = null,
        LayerMask overlapLayerMasks = default,
        GridCellObject.OnOverlapBox onOverlapBox = null,
        Transform parent = null,
        string objName = "Grid"
    )
    {
        GridBuilder gridBuilder = Instantiate(_gridBuilder, transform);
        _gridBuilders.Add(gridBuilder);
        gridBuilder.BuildGrid(xDim, yDim, cellSize, originPosition, overlapTags, overlapLayerMasks, onOverlapBox, parent, objName);
    }
}