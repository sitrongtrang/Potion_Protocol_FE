using TMPro;
using UnityEngine;

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
            //Debug.Log(playerCreateRoomRequest.RoomName);
            //Debug.Log(playerCreateRoomRequest.GameMode);
            //Debug.Log(playerCreateRoomRequest.RoomType);
            //Debug.Log(playerCreateRoomRequest.Password);
            NetworkManager.Instance.SendMessage(playerCreateRoomRequest);
            CreateRoomUI.Instance.ShowPvPCanvas();
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
