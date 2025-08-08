public static class NetworkMessageTypes
{
    public static class Client
    {
        public static class Authentication
        {
            public const short TryAuth = 100;
            public const short TryReconnect = 101;
            public const short TryDisconnect = 102;
            public const short TryHeartBeat = 103;
        }

        public static class FriendSystem
        {
            public const short GetFriendList = 505;
            public const short RemoveFriend = 503;
            // public const short InviteFriend = 
            public const short SendFriendRequest = 500;
            public const short AcceptFriendRequest = 501;
            public const short DeclineFriendRequest = 502;
            public const short GetFriendRequests = 504;
            public const short GetMyRequests = 506;
        }
        public static class Pregame
        {
            public const short CreateRoom = 400;
            public const short StartGame = 405;
            public const short JoinRoom = 401;
            public const short LeaveRoom = 402;
            public const short Ready = 403;
            public const short Unready = 404;
            public const short GetRoomInfo = 406;
            public const short GetRoomByName = 410;
            public const short GetAllRoom = 408;
        }

        public static class Ingame
        {
            public const short Input = 600;
        }

        public static class System
        {
            public const short GetUserInfo = 200;
            public const short GetUserByID = 201;
            public const short Ping = 104;
        }
    }

    public static class Server
    {
        public static class FriendSystem
        {
            public const short GetFriendList = 601;
            public const short RemoveFriend = 605;
            public const short SendFriendRequest = 602;
            public const short AcceptFriendRequest = 603;
            public const short DeclineFriendRequest = 604;
            public const short GetFriendRequests = 600;
            public const short GetMyRequests = 606;
        }
        public static class System
        {
            public const short AuthSuccess = 100;
            public const short Pong = 800;
            public const short GetUserInfo = 200;
        }

        public static class Room
        {
            public const short CreateRoom = 700;
            public const short PlayerJoined = 701;
            public const short PlayerLeft = 702;
            public const short Ready = 703;
            public const short UnReady = 704;

            public const short GetRoomInfo = 707;
            public const short GetRoomByName = 711;
            public const short GetAllRoom = 709;

            public const short RoomFull = 750;
            public const short InRoom = 752;
            public const short NotInRoom = 753;
            public const short OnlyLeader = 754;
            public const short RoomInvalidPassword = 755;
            public const short RoomNotExist = 756;
        }

        public static class Pregame
        {
            public const short StartGame = 705;
            public const short MatchMaking = 706;
            public const short GetUserByID = 201;
        }

        public static class Player
        {
            public const short Connected = 1001;
            public const short Disconnected = 1002;
        }

        public static class GameState
        {
            public const short StateUpdate = 301;
            public const short ScoreUpdate = 1501;
            public const short TimeUpdate = 1502;
        }
    }
}