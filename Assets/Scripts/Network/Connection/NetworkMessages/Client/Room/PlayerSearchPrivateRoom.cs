using TMPro;
using UnityEngine;

public class PlayerSearchPrivateRoom : MonoBehaviour
{
    public TMP_InputField searchInput;
    public GameObject Error;
    private Coroutine _roomIDError;

    public void OnSearchPrivateRoom()
    {
        if (string.IsNullOrEmpty(searchInput.text))
        {
            if (_roomIDError != null) StopCoroutine(_roomIDError);
            _roomIDError = StartCoroutine(CreateRoomUI.Instance.ShowError(Error));
        }
        else
        {
            NetworkManager.Instance.SendMessage(new PlayerGetRoomByIDRequest
            {
                roomID = searchInput.text
            });
        }
    }
}
