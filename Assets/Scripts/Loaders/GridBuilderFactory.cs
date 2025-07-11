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
    public Dictionary<string, GridBuilder> GridBuilders { get; private set; }

    public void BuildGrid(
        string objName,
        int xDim,
        int yDim,
        float cellSize,
        Vector2 originPosition,
        string[] overlapTags = null,
        LayerMask overlapLayerMasks = default,
        GridCellObject.OnOverlapBox onOverlapBox = null,
        Transform parent = null
        
    )
    {
        GridBuilder gridBuilder = Instantiate(_gridBuilder, transform);
        gridBuilder.gameObject.name = objName + " Builder";
        GridBuilders ??= new();
        GridBuilders.Add(objName, gridBuilder);
        gridBuilder.InitializeGrid(objName, xDim, yDim, cellSize, originPosition, overlapTags, overlapLayerMasks, onOverlapBox, parent);
    }
}