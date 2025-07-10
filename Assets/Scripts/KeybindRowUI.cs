using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeybindRowUI : MonoBehaviour
{
    public string actionName;
    public string compositePartName; // để trống nếu không phải composite
    public int bindingIndex = -1;
    
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private Button changeButton;
    [SerializeField] private TextMeshProUGUI labelText;
    

    private System.Action<KeybindRowUI> onChangeRequested;

    public void Init(System.Action<KeybindRowUI> onChangeCallback)
    {
        onChangeRequested = onChangeCallback;

        if (keyText == null)
            keyText = transform.Find("Key")?.GetComponent<TextMeshProUGUI>();
        if (changeButton == null)
            changeButton = transform.Find("Change")?.GetComponent<Button>();
        if (labelText == null)
            labelText = transform.Find("Label")?.GetComponent<TextMeshProUGUI>();

        if (keyText == null || changeButton == null)
        {
            Debug.LogError($"❌ Missing components in {gameObject.name}");
            return;
        }

        // if (labelText != null)
        //     labelText.text = actionName;

        changeButton.onClick.RemoveAllListeners();
        changeButton.onClick.AddListener(() =>
        {
            onChangeRequested?.Invoke(this);
        });
    }

    public void UpdateKeyDisplay(string newKey)
    {
        if (keyText != null)
            keyText.text = newKey;
    }

    public void SetChangeButtonText(string text)
    {
        var btnLabel = changeButton.GetComponentInChildren<TextMeshProUGUI>();
        if (btnLabel != null)
            btnLabel.text = text;
    }
}
