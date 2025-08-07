using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RoomType
{
    Public,
    Private
}

public enum GameMode
{
    Coop,
    PvP
}

public enum PlayerRole
{
    Leader,
    Member
}

public class CreateRoom : MonoBehaviour
{
    [SerializeField] private GameObject Error;
    [SerializeField] private TMP_InputField RoomName;
    [SerializeField] private TMP_InputField Password;
    [SerializeField] private SelectableImage _gameModeOutline;
    [SerializeField] private SelectableImage _roomTypeOutline;
    private GameMode _gameMode;
    private RoomType _roomType;
    private Coroutine _roomNameError;

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
                MapID = RoomScene.img,
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

    public void ResetRoom()
    {
        Debug.Log("aaaaaaaaa");
        RoomName.text = null;
        Password.text = null;
        Error.SetActive(false);
        _gameModeOutline.Click();
        _roomTypeOutline.Click();
    }
}
