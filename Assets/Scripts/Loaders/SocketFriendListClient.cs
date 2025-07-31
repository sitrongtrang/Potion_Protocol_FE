using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class SocketFriendListClient : MonoBehaviour
{
    private const string ServerIP = "10.7.1.166";
    private const int ServerPort = 9000;
    private Socket socket;
    Guid userId;

    void Start()
    {
        StartCoroutine(Login("john_doe", "JohnDoe123"));
    }

    public IEnumerator Login(string username, string password)
    {
        string url = "http://10.7.1.166:8080/users/login";
        var loginData = new LoginData { Username = username, Password = password };
        string jsonData = JsonConvert.SerializeObject(loginData);

        UnityWebRequest request = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Login failed: " + request.error);
        }
        else
        {
            var response = JsonConvert.DeserializeObject<LoginSuccess>(request.downloadHandler.text);
            userId = JwtDecoder.ExtractUserIdFromToken(response.LoginSuccessDat.Token);
            Debug.Log("✅ Logged in. UserId: " + userId);

            ConnectToSocketServer();
            bool isAuthed = SendAuthMessage(response.LoginSuccessDat.Token); // truyền token JWT
            // SendGetFriendList(userId);
        }
    }

    bool SendAuthMessage(string token)
    {
        var authMessage = new AuthMessage { Token = token };
        byte[] authData = Serialization.SerializeMessage(authMessage);
        socket.Send(authData);
        Debug.Log("📤 Sent AuthMessage");

        byte[] buffer = new byte[4096];
        int received = socket.Receive(buffer);
        byte[] actualData = new byte[received];
        Array.Copy(buffer, 0, actualData, 0, received);

        ServerMessage response = Serialization.DeserializeMessage(actualData);
        // Debug.Log(response.GetType().Name);
        if (response is AuthSuccessMessage authSuccess)
        {
            Debug.Log("✅ Auth success. ReconnectToken: " + authSuccess.ReconnectToken);
            return true;
        }
        else
        {
            Debug.LogWarning("❌ Auth failed or unexpected response.");
            return false;
        }
    }

    public void OnButtonClicked()
    {
        SendGetFriendList(userId);
    }
    void ConnectToSocketServer()
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort));
            Debug.Log("✅ Connected to socket server");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Socket error: " + ex.Message);
        }
    }

    void SendGetFriendList(Guid userId)
    {
        try
        {
            var message = new FriendListClientMessage(userId);
            byte[] data = Serialization.SerializeMessage(message);
            Debug.Log(data);
            socket.Send(data);
            Debug.Log("📤 Sent GetFriendListMessage");

            byte[] buffer = new byte[4096];
            int received = socket.Receive(buffer);

            byte[] actualData = new byte[received];
            Array.Copy(buffer, 0, actualData, 0, received);

            ServerMessage response = Serialization.DeserializeMessage(actualData);

            if (response is FriendListServerMessage friendListMsg)
            {
                Debug.Log($"👥 Received {friendListMsg.friendList.Count} friends:");
                foreach (var friend in friendListMsg.friendList)
                {
                    Debug.Log($"👤 {friend.friendDisplayName} ({friend.friendId})");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Unexpected server message type.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ GET_FRIEND_LIST error: " + ex.Message);
        }
        finally
        {
            socket?.Close();
        }
    }

    [Serializable]
    public class LoginData
    {
        [JsonProperty("username")]
        public string Username;
        [JsonProperty("password")]
        public string Password;
    }

    [Serializable]
    public class LoginSuccess
    {
        [JsonProperty("data")]
        public LoginDataWrapper LoginSuccessDat;
    }

    [Serializable]
    public class LoginDataWrapper
    {
        [JsonProperty("token")]
        public string Token;
    }

    public static class JwtDecoder
    {
        public static Guid ExtractUserIdFromToken(string token)
        {
            string[] parts = token.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid JWT format");

            string payload = parts[1];
            int padding = 4 - (payload.Length % 4);
            if (padding < 4) payload += new string('=', padding);
            byte[] data = Convert.FromBase64String(payload);
            string json = Encoding.UTF8.GetString(data);

            var payloadObj = JsonConvert.DeserializeObject<JwtPayload>(json);
            return Guid.Parse(payloadObj.UserId);
        }
    }

    public class JwtPayload
    {
        [JsonProperty("id")]
        public string UserId;
    }
}