using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum RoomType
{
    Public,
    Private
}

public enum GameMode
{
    PvP,
    Coop
}

public class CreateRoom : MonoBehaviour
{
    public TMP_InputField RoomName;
    public TMP_InputField Password;
    private GameMode _gameMode;
    private RoomType _roomType;
    private Coroutine _roomNameError;
    public GameObject Error;

    public void OnRoomScene()
    {
        if (string.IsNullOrEmpty(RoomName.text))
        {
            if (_roomNameError != null) StopCoroutine(_roomNameError);
            _roomNameError = StartCoroutine(CreateRoomUI.Instance.ShowError(Error));
        }
        else
        {
            _roomType = string.IsNullOrEmpty(Password.text) ? RoomType.Public : RoomType.Private;
            PlayerCreateRoomRequest playerCreateRoomRequest = new PlayerCreateRoomRequest
            {
                RoomName = RoomName.text,
                GameMode = (short)_gameMode,
                RoomType = (short)_roomType,
                Password = Password.text,
            };
            NetworkManager.Instance.SendMessage(playerCreateRoomRequest);
        }
    }

    public void OnPvP()
    {
        _gameMode = GameMode.PvP;
    }

    public void OnCoop()
    {
        _gameMode = GameMode.Coop;
    }
}
