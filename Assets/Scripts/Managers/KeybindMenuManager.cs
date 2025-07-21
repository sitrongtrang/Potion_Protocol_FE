using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class KeybindMenuManager : MonoBehaviour
{
    [SerializeField] private List<Transform> _contentParents;
    [SerializeField] private InputActionAsset _inputActions;

    private InputActionRebindingExtensions.RebindingOperation _currentRebinding = null;
    private KeybindRowUI _currentRow = null;
    private List<KeybindRowUI> _keybindRows = new(); // danh sách tất cả rows
    string _path;
    public InputActionAsset GetRebindedAsset()
    {
        return _inputActions;
    }
    void Start()
    {
        _path = Application.persistentDataPath + "/rebinds.json";
        LoadRebindsFromFile();
        // Debug.Log("🔁 Loaded rebinds from file: " + path);
        
        var map = _inputActions.FindActionMap("Player");

        foreach (var parent in _contentParents)
        {
            foreach (Transform child in parent)
            {
                var row = child.GetComponent<KeybindRowUI>();
                if (row == null || string.IsNullOrEmpty(row.actionName)) continue;
                row.Init(StartRebinding);
                row.SetChangeButtonText("Change");
                _keybindRows.Add(row); // ⬅️ thêm dòng này để sau còn dùng
                var action = map.FindAction(row.actionName);
                if (action == null) continue;

                int bindingIdx = row.bindingIndex;

                if (bindingIdx < 0 && !string.IsNullOrEmpty(row.compositePartName))
                    bindingIdx = FindCompositeBindingIndex(action, row.compositePartName);
                else
                    bindingIdx = 0;

                if (bindingIdx < 0 || bindingIdx >= action.bindings.Count) continue;

                row.bindingIndex = bindingIdx;
                Debug.Log(action.GetBindingDisplayString(bindingIdx));
                row.UpdateKeyDisplay(action.GetBindingDisplayString(bindingIdx));
                row.Init(StartRebinding);
                row.SetChangeButtonText("Change");
            }
        }
    }
    private void RefreshAllKeyDisplays()
    {
        Debug.Log(_keybindRows.Count);
        foreach (var row in _keybindRows)
        {
            var action = _inputActions.FindActionMap("Player").FindAction(row.actionName);
            Debug.Log($"🔃 Refreshing {row.actionName}");
            if (action != null && row.bindingIndex >= 0 && row.bindingIndex < action.bindings.Count)
            {
                string newDisplay = action.GetBindingDisplayString(row.bindingIndex);
                Debug.Log($"🔃 Refreshing {row.actionName} → {newDisplay}");
                row.UpdateKeyDisplay(newDisplay);
            }
        }
    }

    int FindCompositeBindingIndex(InputAction action, string partName)
    {
        string partLower = partName.ToLowerInvariant();
        for (int i = 0; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];
            if (b.isPartOfComposite && b.name == partLower && b.path.StartsWith("<Keyboard>"))
                return i;
        }
        return -1;
    }

    void StartRebinding(KeybindRowUI row)
    {
        var action = _inputActions.FindActionMap("Player").FindAction(row.actionName);
        if (action == null) return;

        // Nếu đang rebinding chính cái này → dừng lại
        if (_currentRebinding != null && _currentRow == row)
        {
            Debug.Log($"🛑 Stop rebinding for {row.actionName}");
            _currentRebinding.Cancel(); // Gọi OnCancel
            return;
        }

        // Nếu đang rebinding cái khác → hủy luôn
        if (_currentRebinding != null)
        {
            _currentRebinding.Cancel(); // Gọi OnCancel bên dưới
        }

        action.Disable();

        _currentRebinding = action.PerformInteractiveRebinding(row.bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                string newKey = action.GetBindingDisplayString(row.bindingIndex);
                Debug.Log($"🔁 Rebound {row.actionName} to {newKey}");
                op.Dispose();
                action.Enable();

                row.UpdateKeyDisplay(newKey);
                SaveRebindsToFile();
                // 👉 Cập nhật lại trạng thái trước khi gọi lại StartRebinding
                _currentRebinding = null;
                _currentRow = null;

                // Gọi lại để cho phép đổi tiếp
                StartRebinding(row);
            })
            .OnCancel(op =>
            {
                Debug.Log($"❌ Cancelled rebinding: {row.actionName}");
                op.Dispose();
                action.Enable();
                _currentRebinding = null;
                _currentRow = null;
                row.SetChangeButtonText("Change");
            })
            .Start();

        _currentRow = row;
        row.SetChangeButtonText("Stop");
    }
    public void SaveRebindsToFile()
    {
        string json = _inputActions.SaveBindingOverridesAsJson(); // 🔁 gọi lại mỗi lần lưu
        System.IO.File.WriteAllText(_path, json);
        Debug.Log("💾 Saved rebinds to file: " + _path);
    }
    public void LoadRebindsFromFile()
    {
        if (System.IO.File.Exists(_path))
        {
            string json = System.IO.File.ReadAllText(_path);
            _inputActions.LoadBindingOverridesFromJson(json);
            Debug.Log("🔁 Loaded rebinds from file: " + _path);
        }
        else
        {
            Debug.Log("📁 No rebinds file found at: " + _path);
        }
        RefreshAllKeyDisplays(); // ⬅️ Cập nhật UI // gọi khi bắt đầu game
    }
    public void ResetRebindsFromFile()
    {
        _inputActions.RemoveAllBindingOverrides();

        string path = Application.persistentDataPath + "/rebinds.json";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("🗑️ Deleted rebind file: " + path);
        }
        RefreshAllKeyDisplays(); // ⬅️ Cập nhật lại hiển thị
    }
}
