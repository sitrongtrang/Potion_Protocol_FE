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
    public bool pvp;
    public TMP_InputField inputField;
    public GameObject error;

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
        if (string.IsNullOrEmpty(inputField.text))
        {
            StartCoroutine(ShowError());
        }
        else SceneManager.LoadSceneAsync("RoomScene");
    }

    public IEnumerator ShowError()
    {
        error.SetActive(true);
        yield return new WaitForSeconds(5f);
        error.SetActive(false);
    }

    public void OnPvP()
    {
        pvp = true;

    }

    public void OnCoop()
    {
        pvp = false;
    }
}
