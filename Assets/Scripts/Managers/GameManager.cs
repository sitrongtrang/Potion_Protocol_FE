using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsAutoFocus; // placeholder for auto focus state of player inventory;

    [SerializeField] private int _playerScore;
    [SerializeField] private int _playerStart;

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
            return _playerStart;
        }
        set
        {
            _playerStart = value;
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
        IsAutoFocus = PlayerPrefs.GetInt("IsAutoFocus") == 1; // load here
        DontDestroyOnLoad(this);
    }
}
