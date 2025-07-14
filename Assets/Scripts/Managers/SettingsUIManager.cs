using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ButtonPanelPair
{
    public GameObject button;
    public GameObject panel;
}
public class SettingsUIManager : MonoBehaviour
{
    [SerializeField] List<ButtonPanelPair> _buttonSettingAndPanelList;
     private Dictionary<GameObject, GameObject> _buttonSettingAndPanel;

    private void Awake()
    {
        _buttonSettingAndPanel = new Dictionary<GameObject, GameObject>();
        foreach (var pair in _buttonSettingAndPanelList)
        {
            _buttonSettingAndPanel[pair.button] = pair.panel;

            // Gán sự kiện cho button
            var btn = pair.button.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnClickButton(pair.button));
            }
        }
    }

    private void OnClickButton(GameObject clickedButton)
    {
        foreach (var kvp in _buttonSettingAndPanel)
        {
            bool isActive = kvp.Key == clickedButton;
            kvp.Value.SetActive(isActive);
        }
    }
}
