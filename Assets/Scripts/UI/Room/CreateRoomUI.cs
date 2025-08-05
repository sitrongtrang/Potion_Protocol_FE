using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas _createRoomCanvas;
    [SerializeField] private Canvas _roomListCanvas;
    [SerializeField] private Canvas _pvpCanvas;
    [Header("Refresh")]
    [SerializeField] private Animator _refreshAnimator;
    [SerializeField] private Button _refreshButton;
    private Coroutine _refreshRoom;
    private bool _doneRefresh = false;

    private static List<SelectableImage> _allImages = new List<SelectableImage>();
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
        List<Canvas> allCanvases = new List<Canvas> { _createRoomCanvas, _roomListCanvas, _pvpCanvas };

        foreach (var canvas in allCanvases)
        {
            if (canvas != null)
                canvas.gameObject.SetActive(canvas == canvasToShow);
        }
    }

    public void ShowCreateRoomCanvas()
    {
        SwitchCanvases(_createRoomCanvas);
    }

    public void ShowRoomListCanvas()
    {
        SwitchCanvases(_roomListCanvas);
    }

    public void ShowPvPCanvas()
    {
        SwitchCanvases(_pvpCanvas);
    }

    public void OnRefreshButtonClicked()
    {
        RefreshData();
        if (_refreshRoom != null) StopCoroutine(_refreshRoom);
        _refreshRoom = StartCoroutine(HandleRefresh());
    }

    public IEnumerator HandleRefresh()
    {
        _refreshAnimator.SetBool("IsRefresh", true);
        _refreshButton.interactable = false;

        yield return new WaitUntil(() => _doneRefresh);

        _refreshAnimator.SetBool("IsRefresh", false);
        _refreshButton.interactable = true;
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

    public void SetImageAlpha(float alphaValue, Image targetImage)
    {
        if (targetImage != null) {
            Color currentColor = targetImage.color;
            currentColor.a = alphaValue;
            targetImage.color = currentColor;
        }
    }
}
