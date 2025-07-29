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
        CreateRoom.Register(this);
    }

    public void Click()
    {
        CreateRoom.Select(this);
    }

    public void ShowOutline(bool show)
    {
        if (outline != null)
        {
            outline.enabled = show;
        }
    }
}
