using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GridObject : MonoBehaviour
{
    [Header("Non Walkable Tags")]
    [SerializeField] private string[] _nonWalkableTags;
    [Header("Node Preference")]
    private int _x;
    private int _y;
    [Header("Cache")]
    private static Pathfinding _pathfindingInstance;
    private PathNode _cachedNode;
    public void SetXY(int x, int y)
    {
        _x = x;
        _y = y;

        _pathfindingInstance ??= Pathfinding.Instance;

        _cachedNode = _pathfindingInstance?.GetNode(_x, _y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_cachedNode == null) return;

        foreach (var tag in _nonWalkableTags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                _cachedNode.IsWalkable = false;
                return;
            }
        }
    }
    
    // private void OnTriggerExit2D(Collision2D collision)
    // {
    //     if (_cachedNode == null) return;
        
    //     foreach (var tag in _nonWalkableTags)
    //     {
    //         if (collision.gameObject.CompareTag(tag))
    //         {
    //             _cachedNode.IsWalkable = true;
    //             return;
    //         }
    //     }
    // }
}