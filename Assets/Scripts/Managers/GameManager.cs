using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public int PlayedLevels = 3;
    public int CurrentLevel;

    [SerializeField] private int _playerScore;
    [SerializeField] private int _playerStar;

    public int Score
    {
        get
        {
            return _playerScore;
        }
        set
        { 
            _playerScore = value;
        }
    }

    public int Star
    {
        get
        {
            return _playerStar;
        }
        set
        {
            _playerStar = value;
        }
    }
    

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
}
