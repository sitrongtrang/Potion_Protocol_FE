using System.Linq;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    [Header("Component")]
    public bool NodeNeedUpdate = false;
    [SerializeField] private bool _debug = false;
    [Header("Non Walkable Tags / Layers")]
    [SerializeField] private LayerMask _nonWalkableLayers;
    [SerializeField] private string[] _nonWalkableTags;
    [Header("Node Preference")]
    private int _x;
    private int _y;
    private float _cellSize;
    [Header("Cache")]
    private static Pathfinding _pathfindingInstance;
    private PathNode _cachedNode;

    void Update()
    {
        if (NodeNeedUpdate && Time.frameCount % 10 == 0)
        {
            UpdateWalkability();
        }
    }
    public void InitializeNode(int x, int y, float cellSize)
    {
        _x = x;
        _y = y;
        _cellSize = cellSize;

        _pathfindingInstance ??= Pathfinding.Instance;

        _cachedNode = _pathfindingInstance?.GetNode(_x, _y);

        UpdateWalkability();
    }

    private void UpdateWalkability()
    {
        if (_cachedNode == null) return;

        Collider2D hit = Physics2D.OverlapBox(
            transform.position,
            new(_cellSize, _cellSize),
            0,
            _nonWalkableLayers);

        _cachedNode.IsWalkable = hit == null || !_nonWalkableTags.Contains(hit.tag);
    }
    
    void OnDrawGizmos() {
        if (_debug)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new(_cellSize, _cellSize));
        }
    }
}