using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviour
{
    public Canvas CreateRoomCanvas;
    public Canvas RoomListCanvas;
    public Canvas PrivateRoomCanvas;
    public Canvas PvPCanvas;
    private static List<SelectableImage> _allImages = new List<SelectableImage>();
    public Animator RefreshAnimator;
    public Button RefreshButton;
    private Coroutine _refreshRoom;
    private bool _doneRefresh = false;

    public static CreateRoomUI Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(SelectableImage img)
    {
        if (!_allImages.Contains(img))
        {
            _allImages.Add(img);
        }
    }

    public void Select(SelectableImage img)
    {
        var parent = img.transform.parent;

        foreach (var other in _allImages)
        {
            if (other != img && other.transform.parent == parent)
                other.ShowOutline(false);
        }

        img.ShowOutline(true);
        RoomScene.img = img.img;
    }

    public void OnMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public IEnumerator ShowError(GameObject Error)
    {
        Error.SetActive(true);
        yield return new WaitForSeconds(5f);
        Error.SetActive(false);
    }

    private void SwitchCanvases(Canvas canvasToShow)
    {
        List<Canvas> allCanvases = new List<Canvas> { CreateRoomCanvas, RoomListCanvas, PrivateRoomCanvas, PvPCanvas };

        foreach (var canvas in allCanvases)
        {
            if (canvas != null)
                canvas.gameObject.SetActive(canvas == canvasToShow);
        }
    }

    public void ShowCreateRoomCanvas()
    {
        SwitchCanvases(CreateRoomCanvas);
    }

    public void ShowRoomListCanvas()
    {
        SwitchCanvases(RoomListCanvas);
    }

    public void ShowPrivateRoomCanvas()
    {
        SwitchCanvases(PrivateRoomCanvas);
    }

    public void ShowPvPCanvas()
    {
        SwitchCanvases(PvPCanvas);
    }

    public void OnRefreshButtonClicked()
    {
        RefreshData();
        if (_refreshRoom != null) StopCoroutine(_refreshRoom);
        _refreshRoom = StartCoroutine(HandleRefresh());
    }

    private IEnumerator HandleRefresh()
    {
        RefreshAnimator.SetBool("isRefreshing", true);
        RefreshButton.interactable = false;

        yield return new WaitUntil(() => _doneRefresh);

        RefreshAnimator.SetBool("isRefreshing", false);
        RefreshButton.interactable = true;
        _doneRefresh = false;

        yield break;
    }

    private void RefreshData()
    {
        _doneRefresh = false;
        NetworkManager.Instance.SendMessage(new PlayerGetAllRoomRequest());
    }

    public void Refreshed()
    { 
        _doneRefresh = true; 
    }
}
