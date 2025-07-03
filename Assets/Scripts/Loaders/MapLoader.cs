using UnityEngine;

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
        GameObject map = Resources.Load<GameObject>("Maps/" + mapName);
        Instantiate(map, position, Quaternion.identity);
    }
}
