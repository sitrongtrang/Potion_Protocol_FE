using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class KeybindMenuManager : MonoBehaviour
{
    public List<Transform> contentParents;
    public InputActionAsset inputActions;

    private InputActionRebindingExtensions.RebindingOperation currentRebinding = null;
    private KeybindRowUI currentRow = null;
    private List<KeybindRowUI> keybindRows = new(); // danh sách tất cả rows
    string path;
    string json;
    void Start()
    {
        path = Application.persistentDataPath + "/rebinds.json";
        LoadRebindsFromFile();
        // Debug.Log("🔁 Loaded rebinds from file: " + path);
        
        var map = inputActions.FindActionMap("Player");

        foreach (var parent in contentParents)
        {
            foreach (Transform child in parent)
            {
                var row = child.GetComponent<KeybindRowUI>();
                if (row == null || string.IsNullOrEmpty(row.actionName)) continue;
                row.Init(StartRebinding);
                row.SetChangeButtonText("Change");
                keybindRows.Add(row); // ⬅️ thêm dòng này để sau còn dùng
                var action = map.FindAction(row.actionName);
                if (action == null) continue;

                int bindingIdx = row.bindingIndex;

                if (bindingIdx < 0 && !string.IsNullOrEmpty(row.compositePartName))
                    bindingIdx = FindCompositeBindingIndex(action, row.compositePartName);
                else
                    bindingIdx = 0;

                if (bindingIdx < 0 || bindingIdx >= action.bindings.Count) continue;

                row.bindingIndex = bindingIdx;
                row.UpdateKeyDisplay(action.GetBindingDisplayString(bindingIdx));
                row.Init(StartRebinding);
                row.SetChangeButtonText("Change");
            }
        }
    }
    private void RefreshAllKeyDisplays()
    {
        Debug.Log(keybindRows.Count);
        foreach (var row in keybindRows)
        {
            var action = inputActions.FindActionMap("Player").FindAction(row.actionName);
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
        var action = inputActions.FindActionMap("Player").FindAction(row.actionName);
        if (action == null) return;

        // Nếu đang rebinding chính cái này → dừng lại
        if (currentRebinding != null && currentRow == row)
        {
            Debug.Log($"🛑 Stop rebinding for {row.actionName}");
            currentRebinding.Cancel(); // Gọi OnCancel
            return;
        }

        // Nếu đang rebinding cái khác → hủy luôn
        if (currentRebinding != null)
        {
            currentRebinding.Cancel(); // Gọi OnCancel bên dưới
        }

        action.Disable();

        currentRebinding = action.PerformInteractiveRebinding(row.bindingIndex)
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
                currentRebinding = null;
                currentRow = null;

                // Gọi lại để cho phép đổi tiếp
                StartRebinding(row);
            })
            .OnCancel(op =>
            {
                Debug.Log($"❌ Cancelled rebinding: {row.actionName}");
                op.Dispose();
                action.Enable();
                currentRebinding = null;
                currentRow = null;
                row.SetChangeButtonText("Change");
            })
            .Start();

        currentRow = row;
        row.SetChangeButtonText("Stop");
    }
    public void SaveRebindsToFile()
    {
        string json = inputActions.SaveBindingOverridesAsJson(); // 🔁 gọi lại mỗi lần lưu
        System.IO.File.WriteAllText(path, json);
        Debug.Log("💾 Saved rebinds to file: " + path);
    }
    public void LoadRebindsFromFile()
    {
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            inputActions.LoadBindingOverridesFromJson(json);
            Debug.Log("🔁 Loaded rebinds from file: " + path);
        }
        else
        {
            Debug.Log("📁 No rebinds file found at: " + path);
        }
        RefreshAllKeyDisplays(); // ⬅️ Cập nhật UI // gọi khi bắt đầu game
    }
    public void ResetRebindsFromFile()
    {
        inputActions.RemoveAllBindingOverrides();

        string path = Application.persistentDataPath + "/rebinds.json";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("🗑️ Deleted rebind file: " + path);
        }
        RefreshAllKeyDisplays(); // ⬅️ Cập nhật lại hiển thị
    }
}
