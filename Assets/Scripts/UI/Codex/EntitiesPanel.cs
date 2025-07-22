using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EntitiesPanel : MonoBehaviour
{
    [SerializeField] private GameObject _monstersTab;
    [SerializeField] private GameObject _plantsTab;
    [SerializeField] private GameObject _metalsTab;
    [SerializeField] private GameObject _itemsTab;

    void Start()
    {
        ShowSelectedTab((int)CodexTabSelection.SelectedTab);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }

    public void ShowSelectedTab(int tabIndex)
    {
        CodexTabSelection.Tab tab = (CodexTabSelection.Tab)tabIndex;

        CodexTabSelection.SelectedTab = tab;

        _monstersTab.SetActive(tab == CodexTabSelection.Tab.Monsters);
        _plantsTab.SetActive(tab == CodexTabSelection.Tab.Plants);
        _metalsTab.SetActive(tab == CodexTabSelection.Tab.Metals);
        _itemsTab.SetActive(tab == CodexTabSelection.Tab.Items);
    }


    public void Return()
    {
        SceneManager.UnloadSceneAsync("MainMenu");
    }
}