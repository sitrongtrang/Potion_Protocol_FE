using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public int PlayedLevels = 3;
    public int CurrentLevel;
    public LevelConfig CurrentLevelConfig;
    public int Score;
    public int Star;

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
    public Action<Sprite, float> InitSkillUI;
    public Func<int, IEnumerator> StartCoolDown;
}
