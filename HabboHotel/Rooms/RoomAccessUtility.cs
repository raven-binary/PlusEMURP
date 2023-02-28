﻿namespace Plus.HabboHotel.Rooms
{
    public static class RoomAccessUtility
    {
        public static int GetRoomAccessPacketNum(RoomAccess access)
        {
            switch (access)
            {
                default:
                case RoomAccess.Open:
                    return 0;

                case RoomAccess.Doorbell:
                    return 1;

                case RoomAccess.Password:
                    return 2;

                case RoomAccess.Invisible:
                    return 3;
            }
        }

        public static RoomAccess ToRoomAccess(string id)
        {
            switch (id)
            {
                default:
                case "open":
                    return RoomAccess.Open;

                case "locked":
                    return RoomAccess.Doorbell;

                case "password":
                    return RoomAccess.Password;

                case "invisible":
                    return RoomAccess.Invisible;
            }
        }

        public static RoomAccess ToRoomAccess(int id)
        {
            switch (id)
            {
                default:
                case 0:
                    return RoomAccess.Open;

                case 1:
                    return RoomAccess.Doorbell;

                case 2:
                    return RoomAccess.Password;

                case 3:
                    return RoomAccess.Invisible;
            }
        }
    }
}