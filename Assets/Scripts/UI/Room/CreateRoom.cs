using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    private static List<SelectableImage> allImages = new List<SelectableImage>();
    private static SelectableImage currentSelected = null;
    public TMP_InputField RoomName;
    public TMP_InputField Password;
    public GameObject error;
    private short GameMode;
    private short RoomType;

    public static void Register(SelectableImage img)
    {
        if (!allImages.Contains(img))
        {
            allImages.Add(img);
        }
    }

    public static void Select(SelectableImage img)
    {
        if (currentSelected != null && currentSelected != img)
        {
            currentSelected.ShowOutline(false);
        }

        currentSelected = img;
        currentSelected.ShowOutline(true);
        RoomScene.img = currentSelected.img;
    }

    public void OnMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void OnRoomScene()
    {
        if (string.IsNullOrEmpty(RoomName.text))
        {
            StartCoroutine(ShowError());
        }
        else
        {
            if (Password.text != null) RoomType = 0;
            else RoomType = 1;
            Debug.Log("hihi");
            NetworkManager.Instance.SendMessage(
                new PlayerCreateRoomRequest
                {
                    RoomName = RoomName.text,
                    GameMode = GameMode,
                    RoomType = RoomType,
                    Password = Password.text,
                }
            );
        }
    }

    public IEnumerator ShowError()
    {
        error.SetActive(true);
        yield return new WaitForSeconds(5f);
        error.SetActive(false);
    }

    public void OnPvP()
    {
        GameMode = 1;
    }

    public void OnCoop()
    {
        GameMode = 0;
    }
}
