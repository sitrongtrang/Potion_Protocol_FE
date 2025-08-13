using System.Collections.Generic;
using UnityEngine;

public class StationSpawner : MonoBehaviour
{
    public GameObject Spawn(GameObject stationPrefab)
    {
        GameObject station = Instantiate(stationPrefab, transform.position, Quaternion.identity);
        return station;
    }
}