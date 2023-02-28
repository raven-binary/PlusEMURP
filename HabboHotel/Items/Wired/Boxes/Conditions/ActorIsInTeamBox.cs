﻿using System;
using System.Collections.Concurrent;
using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Items.Wired.Boxes.Conditions
{
    internal class ActorIsInTeamBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.ConditionActorIsInTeamBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public ActorIsInTeamBox(Room instance, Item item)
        {
            Instance = instance;
            Item = item;

            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket packet)
        {
            int unknown = packet.PopInt();
            int unknown2 = packet.PopInt();

            StringData = unknown2.ToString();
        }

        public bool Execute(params object[] @params)
        {
            if (@params.Length == 0 || Instance == null || string.IsNullOrEmpty(StringData))
                return false;

            Habbo player = (Habbo) @params[0];
            if (player == null)
                return false;

            RoomUser user = Instance.GetRoomUserManager().GetRoomUserByHabbo(player.Id);
            if (user == null)
                return false;

            if (int.Parse(StringData) == 1 && user.Team == Team.Red)
                return true;
            if (int.Parse(StringData) == 2 && user.Team == Team.Green)
                return true;
            if (int.Parse(StringData) == 3 && user.Team == Team.Blue)
                return true;
            if (int.Parse(StringData) == 4 && user.Team == Team.Yellow)
                return true;
            return false;
        }
    }
}