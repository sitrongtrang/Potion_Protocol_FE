using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableImage : MonoBehaviour
{
    private Outline outline;
    public int img;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        CreateRoomUI.Instance.Register(this);
    }

    public void Click()
    {
        CreateRoomUI.Instance.Select(this);
    }

    public void ShowOutline(bool show)
    {
        if (outline != null)
        {
            outline.enabled = show;
        }
    }
}
