using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapLoader : MonoBehaviour
{
    public static MapLoader Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        RenderMap(Vector3.zero, "Dungeon");
    }

    private void RenderMap(Vector3 position, string mapName)
    {
        GameObject mapRes = Resources.Load<GameObject>("Maps/" + mapName);
        GameObject map = Instantiate(mapRes, position, Quaternion.identity);

        GridLoader.Instance.LoadGrid(map);
    }
}
