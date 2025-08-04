using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomScene : MonoBehaviour
{
    public Image TargetImage;
    public Sprite[] NewSprite;
    public static int img = 0;
    public TMP_Text RoomID;

    public void ChooseImage()
    {
        TargetImage.sprite = NewSprite[img];
        Debug.Log("Img: " + img);
    }

    public void SetPersonRoomName(string newName, TMP_Text Person)
    {
        Person.text = newName;
    }

    public void SetRoomID(string newID)
    {
        RoomID.text = "Room ID: " + newID;
    }
}
