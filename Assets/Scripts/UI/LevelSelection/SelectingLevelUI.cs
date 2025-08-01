using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectingLevelUI : MonoBehaviour
{
    [SerializeField] private int _numLevelPerRow = 2;
    [SerializeField] private int _numLevelRows = 2;
    [SerializeField] private int _numLevels = 10;
    [SerializeField] private GameObject _levelPanel;
    [SerializeField] private GameObject _levelRowPrefab;
    [SerializeField] private GameObject _levelButtonPrefab;
    [SerializeField] private Button _nextPageButton;
    [SerializeField] private Button _previousPageButton;

    private int _currentPage = 0;

    private int _numLevelPerPage;
    private int _totalPages;

    void Start()
    {
        _numLevelPerPage = _numLevelPerRow * _numLevelRows;
        _totalPages = Mathf.CeilToInt((float)_numLevels / _numLevelPerPage);
        UpdateLevelButtons();
        _nextPageButton.onClick.AddListener(OnNextPage);
        _previousPageButton.onClick.AddListener(OnPreviousPage);
    }

    public void OnReturn()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnLevelSelected(int level)
    {
        GameManager.Instance.CurrentLevel = level;
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        List<AsyncOperation> opList = new List<AsyncOperation>();
        AsyncOperation loadSceneRequest = SceneManager.LoadSceneAsync("GameScene");
        opList.Add(loadSceneRequest);

        string levelPath = $"ScriptableObjects/Levels/Level{GameManager.Instance.CurrentLevel + 1}";
        ResourceRequest loadLevelRequest = Resources.LoadAsync<LevelConfig>(levelPath);
        opList.Add(loadLevelRequest);

        loadSceneRequest.completed += async (op) => 
        {
            Scene loadedScene = SceneManager.GetSceneByName("GameScene");

            while (!loadLevelRequest.isDone)
            {
                await Task.Yield();
            }

            GameObject[] rootObjects = loadedScene.GetRootGameObjects();
            LevelManager levelManager = null;
            ItemPool itemPool = null;
            for (int i = 0; i < rootObjects.Length; i++)
            {
                levelManager = rootObjects[i].GetComponentInChildren<LevelManager>();
                itemPool = rootObjects[i].GetComponentInChildren<ItemPool>();
                if (levelManager)
                {
                    levelManager.Initialize(loadLevelRequest.asset as LevelConfig);
                }
                if (itemPool)
                {
                    itemPool.Initialize(loadLevelRequest.asset as LevelConfig);
                    break;
                }
            }

            await LoadingScreenUI.Instance.RenderFinish();
        };

        LoadingScreenUI.Instance.gameObject.SetActive(true);
        yield return StartCoroutine(LoadingScreenUI.Instance.RenderLoadingScene(opList));
    }

    public void OnNextPage()
    {
        int oldPage = _currentPage;
        _currentPage = Mathf.Clamp(_currentPage + 1, 0, _totalPages - 1);
        if (oldPage != _currentPage) UpdateLevelButtons();
    }

    public void OnPreviousPage()
    {
        int oldPage = _currentPage;
        _currentPage = Mathf.Clamp(_currentPage - 1, 0, _totalPages - 1);
        if (oldPage != _currentPage) UpdateLevelButtons();
    }

    private void UpdateLevelButtons()
    {
        int startLevel = _currentPage * _numLevelPerPage;
        int endLevel = Mathf.Min(startLevel + _numLevelPerPage, _numLevels); // Exclusive end
        int numRows = Mathf.Min(_numLevelRows, Mathf.CeilToInt((float)(endLevel - startLevel) / _numLevelPerRow));

        for (int i = 0; i < numRows; i++)
        {
            if (i < _levelPanel.transform.childCount)
            {   // Reuse existing row
                Transform row = _levelPanel.transform.GetChild(i);
                row.gameObject.SetActive(true);
                for (int j = 0; j < _numLevelPerRow; j++)
                {
                    GameObject buttonObj;
                    if (j >= row.childCount)
                    {   // Create new button if it doesn't exist
                        buttonObj = Instantiate(_levelButtonPrefab, row);
                        buttonObj.transform.SetParent(row);
                    }

                    // Add the correct listener
                    buttonObj = row.GetChild(j).gameObject;
                    Button button = buttonObj.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    int rowNum = i;
                    int levelNum = j;
                    button.onClick.AddListener(() => OnLevelSelected(startLevel + rowNum * _numLevelPerRow + levelNum));

                    // Hide button if it exceeds the end level 
                    row.GetChild(j).gameObject.SetActive(startLevel + i * _numLevelPerRow + j < endLevel);

                    // Check if the level is locked
                    bool isLocked = startLevel + i * _numLevelPerRow + j > GameManager.Instance.PlayedLevels;
                    ToggleLockLevel(row.GetChild(j).gameObject, isLocked);
                }
            }
            else
            {   // Create new row
                GameObject row = Instantiate(_levelRowPrefab, transform);
                row.transform.SetParent(_levelPanel.transform);
                for (int j = 0; j < _numLevelPerRow; j++)
                {
                    // Add the correct listener
                    GameObject buttonObj = Instantiate(_levelButtonPrefab, row.transform);
                    buttonObj.transform.SetParent(row.transform);
                    Button button = buttonObj.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    int rowNum = i;
                    int levelNum = j;
                    button.onClick.AddListener(() => OnLevelSelected(startLevel + rowNum * _numLevelPerRow + levelNum));

                    // Hide button if it exceeds the end level 
                    row.transform.GetChild(j).gameObject.SetActive(startLevel + i * _numLevelPerRow + j < endLevel);

                    bool isLocked = startLevel + i * _numLevelPerRow + j > GameManager.Instance.PlayedLevels;
                    ToggleLockLevel(buttonObj, isLocked);
                }
            }  
        }

        for (int i = numRows; i < _levelPanel.transform.childCount; i++)
        {   // Hide row if exceed number of rows
            Transform row = _levelPanel.transform.GetChild(i);
            row.gameObject.SetActive(false);
        }

        _nextPageButton.gameObject.SetActive(_currentPage < _totalPages - 1);
        _previousPageButton.gameObject.SetActive(_currentPage > 0);
    }
    
    private void ToggleLockLevel(GameObject levelButton, bool isLocked)
    {
        GameObject lockImage = levelButton.transform.Find("LockImage")?.gameObject;
        if (lockImage != null)
        {
            lockImage.SetActive(isLocked);
        }
        Button button = levelButton.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = !isLocked;
        }
    }
}
