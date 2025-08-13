using UnityEngine;
using UnityEngine.EventSystems;

public class CursorSetting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Texture2D _hoverTexture;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance.SetCursorTexture(_hoverTexture); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.ResetToInitialCursor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CursorManager.Instance.ResetToInitialCursor();
    }
}
