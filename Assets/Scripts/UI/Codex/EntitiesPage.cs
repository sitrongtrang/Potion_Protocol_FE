using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntitiesPage : MonoBehaviour
{
    [SerializeField] private string _entitiesFolderPath;
    [SerializeField] private int _numEntitiesPerRow = 4;
    [SerializeField] private int _numEntityRows = 4;
    [SerializeField] private GameObject _entityPage;
    [SerializeField] private GameObject _entityRowPrefab;
    [SerializeField] private GameObject _entityButtonPrefab;
    [SerializeField] private Button _nextPageButton;
    [SerializeField] private Button _previousPageButton;
    private int _currentPage = 0;
    private List<EntityConfig> _entityConfigs = new();
    private int _numEntities = 0;
    private int _numEntitiesPerPage;
    private int _totalPages;

    void Start()
    {
        EntityConfig[] _entites = Resources.LoadAll<EntityConfig>(_entitiesFolderPath);
        for (int i = 0; i < _entites.Length; i++)
        {
            _entityConfigs.Add(_entites[i]);

        }
        _numEntitiesPerPage = _numEntitiesPerRow * _numEntityRows;
        _numEntities = _entityConfigs.Count;
        _totalPages = Mathf.CeilToInt((float)_numEntities / _numEntitiesPerPage);
        UpdateEntityButtons();
    }

    void OnEnable()
    {
        _nextPageButton.onClick.AddListener(OnNextPage);
        _previousPageButton.onClick.AddListener(OnPreviousPage);
    }

    void OnDisable()
    {
        _nextPageButton.onClick.RemoveListener(OnNextPage);
        _previousPageButton.onClick.RemoveListener(OnPreviousPage); 
    }

    public void OnNextPage()
    {
        int oldPage = _currentPage;
        _currentPage = Mathf.Clamp(_currentPage + 1, 0, _totalPages - 1);
        if (oldPage != _currentPage) UpdateEntityButtons();
    }

    public void OnPreviousPage()
    {
        int oldPage = _currentPage;
        _currentPage = Mathf.Clamp(_currentPage - 1, 0, _totalPages - 1);
        if (oldPage != _currentPage) UpdateEntityButtons();
    }

    private void UpdateEntityButtons()
    {
        int startEntity = _currentPage * _numEntitiesPerPage;
        int endEntity = Mathf.Min(startEntity + _numEntitiesPerPage, _numEntities); // Exclusive end
        int numRows = Mathf.Min(_numEntityRows, Mathf.CeilToInt((float)(endEntity - startEntity) / _numEntitiesPerRow));

        for (int i = 0; i < numRows; i++)
        {
            if (i < _entityPage.transform.childCount)
            {   // Reuse existing row
                Transform row = _entityPage.transform.GetChild(i);
                row.gameObject.SetActive(true);
                for (int j = 0; j < _numEntitiesPerRow; j++)
                {
                    GameObject buttonObj;
                    if (j >= row.childCount)
                    {   // Create new button if it doesn't exist
                        buttonObj = Instantiate(_entityButtonPrefab, row);
                        buttonObj.transform.SetParent(row);
                    }

                    // Add the correct listener
                    buttonObj = row.GetChild(j).gameObject;
                    Button button = buttonObj.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    int rowNum = i;
                    int entityNum = j;
                    EntityConfig entityConfig = _entityConfigs[startEntity + rowNum * _numEntitiesPerRow + entityNum];
                    button.onClick.AddListener(() => OnEntitySelected(entityConfig));
                    
                    // Set the icon for the button
                    Image Icon = buttonObj.transform.Find("Icon").GetComponent<Image>();
                    Icon.sprite = entityConfig.Icon;

                    // Hide button if it exceeds the end entity 
                    row.GetChild(j).gameObject.SetActive(startEntity + i * _numEntitiesPerRow + j < endEntity);
                }
            }
            else
            {   // Create new row
                GameObject row = Instantiate(_entityRowPrefab, transform);
                row.transform.SetParent(_entityPage.transform);
                for (int j = 0; j < _numEntitiesPerRow; j++)
                {
                    // Add the correct listener
                    GameObject buttonObj = Instantiate(_entityButtonPrefab, row.transform);
                    buttonObj.transform.SetParent(row.transform);
                    Button button = buttonObj.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    int rowNum = i;
                    int entityNum = j;
                    EntityConfig entityConfig = _entityConfigs[startEntity + rowNum * _numEntitiesPerRow + entityNum];
                    button.onClick.AddListener(() => OnEntitySelected(entityConfig));

                    // Set the icon for the button
                    Image Icon = buttonObj.transform.Find("Icon").GetComponent<Image>();
                    Icon.sprite = entityConfig.Icon;

                    // Hide button if it exceeds the end entity 
                    row.transform.GetChild(j).gameObject.SetActive(startEntity + i * _numEntitiesPerRow + j < endEntity);
                }
            }
        }

        for (int i = numRows; i < _entityPage.transform.childCount; i++)
        {   // Hide row if exceed number of rows
            Transform row = _entityPage.transform.GetChild(i);
            row.gameObject.SetActive(false);
        }

        _nextPageButton.gameObject.SetActive(_currentPage < _totalPages - 1);
        _previousPageButton.gameObject.SetActive(_currentPage > 0);
    }

    private void OnEntitySelected(EntityConfig entityConfig)
    {
        // Handle entity selection logic here
        Debug.Log($"Selected Entity");
        // For example, you could load a scene or display entity details
    }
}