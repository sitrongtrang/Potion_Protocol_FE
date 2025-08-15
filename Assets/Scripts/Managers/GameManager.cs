using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public int PlayedLevels = 3;
    public int CurrentLevel;
    public int Star;
    public Action<FriendViewMode, int> LoadFriendList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}
