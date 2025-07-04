using System.Collections.Generic;
using UnityEngine;
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

        foreach (var pos in positions)
        {
            GameObject fire = Instantiate(Fire, Vector3.zero, Quaternion.identity, map.transform);
            fire.transform.localPosition = pos;
        }
    }
    public Vector3[] positions;
    public GameObject Fire;
}
