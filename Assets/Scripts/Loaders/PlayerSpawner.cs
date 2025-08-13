using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject SpawnPlayer(GameObject playerPrefab)
    {
        GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        return player;
    }
}