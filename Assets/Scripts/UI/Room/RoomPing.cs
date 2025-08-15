using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class RoomPing : MonoBehaviour
{
    [SerializeField] private TMP_Text _ping;
    private void OnEnable()
    {
        NetworkTime.Instance.OnPingChanged += UpdatePing;
    }

    private void OnDisable()
    {
        NetworkTime.Instance.OnPingChanged -= UpdatePing;
    }

    private void UpdatePing(long newPing)
    {
        _ping.SetText("{0} ms", newPing);
        _ping.ForceMeshUpdate();
        Debug.Log("Ping: " + newPing);
    }
}
