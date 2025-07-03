using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
    [SerializeField] private List<GameObject> MapList;
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
    public void RenderMap(Vector3 position, int index)
    {
        Instantiate(MapList[index], position, Quaternion.identity);
    }
}
