using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Quản lý UI rebinding + ràng buộc không trùng phím theo rule game.
/// Chính sách: khi user rebinding 1 action, ta giữ action đó; mọi action khác vi phạm rule & dùng cùng phím sẽ bị clear.
/// Không cho đóng menu nếu còn action bắt buộc bị rỗng.
/// </summary>
public class SettingManager : MonoBehaviour
{
    [Header("UI Parents chứa các KeybindRowUI children")]
    [SerializeField] private List<Transform> _contentParents;

    [Header("Input Actions Asset")]
    [SerializeField] private InputActionAsset _inputActions;

    [Header("Tên Action Groups (phải trùng action names trong map Player)")]
    [Tooltip("Các action điều khiển di chuyển (WASD / Move composite parts...)")]
    [SerializeField] private string[] movementActions;      // ví dụ: "MoveUp","MoveDown","MoveLeft","MoveRight" hoặc nếu bạn dùng 1 action composite "Move" thì để "Move"
    [Tooltip("Dash action")]
    [SerializeField] private string dashAction;             // "Dash"
    [Tooltip("Attack + Skill actions (mutually unique nội bộ)")]
    [SerializeField] private string[] attackAndSkillActions; // "Attack","Skill1","Skill2","Skill3"
    [Tooltip("Pickup action (global unique)")]
    [SerializeField] private string pickupAction;           // "Pickup"
    [Tooltip("Drop action (global unique)")]
    [SerializeField] private string dropAction;             // "Drop"
    [Tooltip("Transfer action")]
    [SerializeField] private string transferAction;         // "Transfer"
    [Tooltip("Submit action")]
    [SerializeField] private string submitAction;           // "Submit"
    [Tooltip("Exploit action (để rule với transfer/submit)")]
    [SerializeField] private string exploitAction;          // "Exploit"
    [Tooltip("Craft action (để rule với transfer/submit)")]
    [SerializeField] private string craftAction;            // "Craft"
    [Tooltip("Các nút inventory (chỉ unique nội bộ)")]
    [SerializeField] private string[] inventoryActions;     // "Inv1","Inv2","Inv3"...

    // Runtime
    private InputActionRebindingExtensions.RebindingOperation _currentRebinding = null;
    private KeybindRowUI _currentRow = null;
    private readonly List<KeybindRowUI> _keybindRows = new(); // tất cả rows trong UI
    private string _path;

    // Cache action map
    private InputActionMap _playerMap;

    // ======================================================================

    public InputActionAsset GetRebindedAsset() => _inputActions;
    private bool _hasPendingChanges = false;
    [SerializeField] private UnityEngine.UI.Button _applyButton;
    [SerializeField] private UnityEngine.UI.Button _revertButton;
    [SerializeField] private UnityEngine.UI.Button _resetButton;

    void Start()
    {
        _applyButton.onClick.AddListener(ApplyChanges);
        _revertButton.onClick.AddListener(DiscardChanges);
        _resetButton.onClick.AddListener(ResetRebindsFromFile);

        _path = System.IO.Path.Combine(Application.persistentDataPath, "rebinds.json");
        _playerMap = _inputActions.FindActionMap("Player", throwIfNotFound: false);

        if (_playerMap == null)
        {
            Debug.LogError("KeybindMenuManager: Không tìm thấy ActionMap 'Player' trong asset!");
            return;
        }

        // 1) Build UI row list + init rows với binding mặc định
        BuildRowsFromUI();

        // 2) Load override từ file (nếu có) và refresh hiển thị
        LoadRebindsFromFile(); // sẽ gọi RefreshAllKeyDisplays sau khi load
    }

    // ----------------------------------------------------------------------

    private void BuildRowsFromUI()
    {
        _keybindRows.Clear();

        foreach (var parent in _contentParents)
        {
            if (parent == null) continue;

            foreach (Transform child in parent)
            {
                var row = child.GetComponent<KeybindRowUI>();
                if (row == null || string.IsNullOrEmpty(row.actionName))
                    continue;

                var action = _playerMap.FindAction(row.actionName, throwIfNotFound: false);
                if (action == null)
                {
                    Debug.LogWarning($"KeybindMenuManager: Không tìm thấy action '{row.actionName}' trong map Player.");
                    continue;
                }

                // Nếu row chưa set bindingIndex hợp lệ → tìm
                int bindingIdx = row.bindingIndex;
                if (bindingIdx < 0)
                {
                    if (!string.IsNullOrEmpty(row.compositePartName))
                        bindingIdx = FindCompositeBindingIndex(action, row.compositePartName);
                    else
                        bindingIdx = FindFirstNonCompositeBindingIndex(action);
                }

                if (bindingIdx < 0 || bindingIdx >= action.bindings.Count)
                {
                    Debug.LogWarning($"KeybindMenuManager: bindingIndex không hợp lệ cho action '{row.actionName}'.");
                    continue;
                }

                row.bindingIndex = bindingIdx;

                // Khởi tạo UI callback
                row.Init(StartRebinding);
                row.SetChangeButtonText("Change");

                // Update hiển thị theo binding hiện tại (gốc / override)
                row.UpdateKeyDisplay(GetBindingDisplaySafe(action, bindingIdx));

                // Lưu row
                _keybindRows.Add(row);
            }
        }
    }

    // ----------------------------------------------------------------------

    private int FindFirstNonCompositeBindingIndex(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!action.bindings[i].isComposite && !action.bindings[i].isPartOfComposite)
                return i;
        }
        // fallback 0
        return 0;
    }

    private string GetBindingDisplaySafe(InputAction action, int bindingIndex)
    {
        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            return "---";

        // Nếu binding path rỗng → hiển thị ---
        var b = action.bindings[bindingIndex];
        if (string.IsNullOrEmpty(b.effectivePath) && string.IsNullOrEmpty(b.path))
            return "---";

        return action.GetBindingDisplayString(bindingIndex);
    }

    // ----------------------------------------------------------------------

    private void RefreshAllKeyDisplays()
    {
        foreach (var row in _keybindRows)
        {
            var action = _playerMap.FindAction(row.actionName);
            if (action == null) continue;
            row.UpdateKeyDisplay(GetBindingDisplaySafe(action, row.bindingIndex));
        }
    }

    // ======================================================================
    //  REBIND
    // ======================================================================

    private void StartRebinding(KeybindRowUI row)
    {
        var action = _playerMap.FindAction(row.actionName);
        if (action == null)
            return;

        // Nếu đang rebinding chính cái này → dừng
        if (_currentRebinding != null && _currentRow == row)
        {
            _currentRebinding.Cancel();
            return;
        }

        // Nếu đang rebinding cái khác → hủy
        if (_currentRebinding != null)
        {
            _currentRebinding.Cancel();
        }

        action.Disable();

        _currentRebinding = action.PerformInteractiveRebinding(row.bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                op.Dispose();
                action.Enable();

                // Lấy phím mới
                string newDisplay = action.GetBindingDisplayString(row.bindingIndex);
                string newPath = GetEffectiveBindingPath(action, row.bindingIndex);

                Debug.Log($"🔁 Rebound {row.actionName} to {newDisplay} ({newPath})");

                // Cập nhật UI hàng hiện tại
                row.UpdateKeyDisplay(newDisplay);

                // Áp dụng rule xung đột → clear các action khác
                ResolveConflictsAfterRebind(row.actionName, row.bindingIndex, newPath);

                // Reset trạng thái rebinding
                _currentRebinding = null;
                _currentRow = null;
                _hasPendingChanges = true; // <-- ADD THIS
                // Gọi lại để cho phép đổi tiếp
                StartRebinding(row);
            })
            .OnCancel(op =>
            {
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

    // ----------------------------------------------------------------------

    private string GetEffectiveBindingPath(InputAction action, int bindingIndex)
    {
        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            return string.Empty;

        var b = action.bindings[bindingIndex];
        // effectivePath ưu tiên override; nếu rỗng dùng path gốc
        return string.IsNullOrEmpty(b.effectivePath) ? b.path : b.effectivePath;
    }

    // ======================================================================
    //  CONFLICT RULES
    // ======================================================================

    /// <summary>
    /// Được gọi sau khi 1 action đã được rebinding sang newPath.
    /// Giữ action vừa đổi; clear các action khác vi phạm rule dùng cùng newPath.
    /// </summary>
    private void ResolveConflictsAfterRebind(string changedActionName, int changedBindingIndex, string newPath)
    {
        // Nếu path rỗng (user cancel? hoặc clear) thì không cần check
        if (string.IsNullOrEmpty(newPath))
        {
            RefreshAllKeyDisplays();
            return;
        }

        // Build lookup
        var changedAction = _playerMap.FindAction(changedActionName);
        if (changedAction == null)
            return;

        // Chuẩn hoá so sánh
        string normNewPath = NormalizePath(newPath);

        // Lấy tên của part nếu có
        string changedPartName = _currentRow?.compositePartName?.ToLowerInvariant();
        if (changedPartName != null) Debug.Log(changedPartName);
        // Lặp toàn bộ row & action tương ứng
        foreach (var row in _keybindRows)
        {
            var otherAction = _playerMap.FindAction(row.actionName);
            if (otherAction == null)
                continue;

            // Nếu là cùng action + cùng part → bỏ qua chính nó
            bool isSameAction = row.actionName == changedActionName;
            string otherPart = row.compositePartName?.ToLowerInvariant();
            if (isSameAction && changedPartName == otherPart)
                continue;

            string otherPath = GetEffectiveBindingPath(otherAction, row.bindingIndex);
            string normOtherPath = NormalizePath(otherPath);

            if (string.IsNullOrEmpty(normOtherPath))
                continue;

            if (!PathsEqual(normNewPath, normOtherPath))
                continue;
            if (IsConflict(changedActionName, changedPartName, row.actionName, otherPart))
            {
                Debug.Log($"⚠️ Conflict: {changedActionName} [{newPath}] vs {row.actionName} [{otherPath}] → clearing {row.actionName}.");
                ClearBinding(otherAction, row.bindingIndex);
                row.UpdateKeyDisplay("---");
            }
        }

        // Sau khi xử lý: refresh UI hiển thị cho chắc
        RefreshAllKeyDisplays();
    }

    // ----------------------------------------------------------------------

    /// <summary>
    /// Quyết định 2 action có bị cấm dùng chung phím hay không theo rule.
    /// </summary>
    private bool IsConflict(string aName, string aPart, string bName, string bPart)
    {
        string Normalize(string s) => s?.Trim().ToLowerInvariant();

        aName = Normalize(aName);
        bName = Normalize(bName);
        aPart = Normalize(aPart);
        bPart = Normalize(bPart);

        bool In(string target, string[] group) =>
            group != null && group.Any(x => x.Equals(target, StringComparison.InvariantCultureIgnoreCase));
        bool Eq(string x, string y) =>
            string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);

        // -------------------------
        // Conflict trong nhóm Move
        // -------------------------
        bool aIsMove = Eq(aName, "move");
        bool bIsMove = Eq(bName, "move");

        if (aIsMove && bIsMove && aPart != null && bPart != null && aPart == bPart)
            return true;

        if ((aIsMove && bPart == null && In(bName, movementActions)) ||
            (bIsMove && aPart == null && In(aName, movementActions)))
            return true;

        // -------------------------
        // Movement hoặc Dash trùng với bất kỳ ai → conflict
        // -------------------------
        if (In(aName, movementActions) || In(bName, movementActions) || Eq(aName, dashAction) || Eq(bName, dashAction))
            return true;

        // -------------------------
        // Pickup / Drop: global unique
        // -------------------------
        if (Eq(aName, pickupAction) || Eq(bName, pickupAction))
            return true;

        if (Eq(aName, dropAction) || Eq(bName, dropAction))
            return true;

        // -------------------------
        // Attack + Skill: không trùng nhau trong nhóm
        // -------------------------
        bool aIsAtk = In(aName, attackAndSkillActions);
        bool bIsAtk = In(bName, attackAndSkillActions);
        if (aIsAtk && bIsAtk)
            return true;

        // -------------------------
        // Inventory: không trùng trong nhóm
        // -------------------------
        bool aIsInv = In(aName, inventoryActions);
        bool bIsInv = In(bName, inventoryActions);
        if (aIsInv && bIsInv)
            return true;

        // -------------------------
        // Transfer / Submit vs Attack / Exploit / Craft
        // -------------------------
        bool aIsTrans = Eq(aName, transferAction);
        bool bIsTrans = Eq(bName, transferAction);
        bool aIsSubmit = Eq(aName, submitAction);
        bool bIsSubmit = Eq(bName, submitAction);

        bool aIsAtkBase = Eq(aName, "attack") || Eq(aName, exploitAction) || Eq(aName, craftAction);
        bool bIsAtkBase = Eq(bName, "attack") || Eq(bName, exploitAction) || Eq(bName, craftAction);

        if ((aIsTrans && bIsAtkBase) || (bIsTrans && aIsAtkBase)) return true;
        if ((aIsSubmit && bIsAtkBase) || (bIsSubmit && aIsAtkBase)) return true;

        return false;
    }
    // ----------------------------------------------------------------------

    private void ClearBinding(InputAction action, int bindingIndex)
    {
        if (action == null) return;
        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count) return;

        action.ApplyBindingOverride(bindingIndex, string.Empty);
    }

    // ----------------------------------------------------------------------
    // PATH COMPARISON
    // ----------------------------------------------------------------------

    private string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        // Chuẩn lower
        path = path.ToLowerInvariant();

        // Bạn có thể thêm bước strip processors; ví dụ bỏ phím <keyboard> prefix nếu muốn so keyboard-only
        // Ở đây giữ nguyên để phân biệt device
        return path;
    }

    private bool PathsEqual(string a, string b) => a == b;

    // ======================================================================
    //  SAVE / LOAD
    // ======================================================================

    public void SaveRebindsToFile()
    {
        string json = _inputActions.SaveBindingOverridesAsJson();
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
            Debug.Log("📁 No rebinds file found at: " + _path + " → dùng binding mặc định trong asset.");
        }

        RefreshAllKeyDisplays();
    }

    public void ResetRebindsFromFile()
    {
        _inputActions.RemoveAllBindingOverrides();

        if (System.IO.File.Exists(_path))
        {
            System.IO.File.Delete(_path);
            Debug.Log("🗑️ Deleted rebind file: " + _path);
        }
        RefreshAllKeyDisplays();
        GetComponent<MiscSetting>().ResetToDefault();
    }

    // ======================================================================
    //  EXIT CHECK
    // ======================================================================

    /// <summary>
    /// Có action nào rỗng binding không? Nếu có → không được thoát khỏi menu.
    /// Bạn có thể giới hạn chỉ check các action bắt buộc (vd movement, attack, pickup,...).
    /// </summary>
    public bool HasAnyEmptyRequiredBinding()
    {
        foreach (var row in _keybindRows)
        {
            if (!IsRequiredAction(row.actionName))
                continue;

            var action = _playerMap.FindAction(row.actionName);
            if (action == null) continue;

            if (IsBindingEmpty(action, row.bindingIndex))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Định nghĩa action nào "bắt buộc". Tôi xem tất cả trừ inventory? hoặc bạn đổi theo ý.
    /// </summary>
    private bool IsRequiredAction(string actionName)
    {
        // Ở đây: tất cả action đều bắt buộc. Nếu muốn loại inventory:
        // return !inventoryActions.Contains(actionName);
        return true;
    }

    private bool IsBindingEmpty(InputAction action, int bindingIndex)
    {
        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            return true;

        var b = action.bindings[bindingIndex];
        return string.IsNullOrEmpty(b.effectivePath) && string.IsNullOrEmpty(b.path); // path gốc + override đều rỗng
    }

    // ======================================================================
    //  UTIL
    // ======================================================================

    private int FindCompositeBindingIndex(InputAction action, string partName)
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

    public void ApplyChanges()
    {
        if (_hasPendingChanges)
        {
            SaveRebindsToFile();
            _hasPendingChanges = false;
            Debug.Log("✅ Applied pending keybind changes.");
        }
        else
        {
            Debug.Log("⚠️ No changes to apply.");
        }
        PlayerPrefs.SetInt("IsAutoFocus", GetComponent<MiscSetting>().AutoFocusValue == true ? 1 : 0);
        PlayerPrefs.SetString("Language", GetComponent<MiscSetting>().LanguageValue);
    }
    public void DiscardChanges()
    {
        LoadRebindsFromFile(); // Revert lại
        _hasPendingChanges = false;
        GetComponent<MiscSetting>().LoadSettings();
    }
}
