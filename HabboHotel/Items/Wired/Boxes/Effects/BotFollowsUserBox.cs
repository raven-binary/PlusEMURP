﻿using System;
using System.Collections.Concurrent;
using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class BotFollowsUserBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectBotFollowsUserBox;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotFollowsUserBox(Room instance, Item item)
        {
            Instance = instance;
            Item = item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket packet)
        {
            int unknown = packet.PopInt();
            int followMode = packet.PopInt(); //1 = follow, 0 = don't.
            string botConfiguration = packet.PopString();

            if (SetItems.Count > 0)
                SetItems.Clear();

            StringData = followMode + ";" + botConfiguration;
        }

        public bool Execute(params object[] @params)
        {
            if (@params == null || @params.Length == 0)
                return false;

            if (string.IsNullOrEmpty(StringData))
                return false;

            Habbo player = (Habbo) @params[0];
            if (player == null)
                return false;

            RoomUser human = Instance.GetRoomUserManager().GetRoomUserByHabbo(player.Id);
            if (human == null)
                return false;

            string[] stuff = StringData.Split(';');
            if (stuff.Length != 2)
                return false; //This is important, incase a cunt scripts.

            string username = stuff[1];

            RoomUser user = Instance.GetRoomUserManager().GetBotByName(username);
            if (user == null)
                return false;

            int followMode = 0;
            if (!int.TryParse(stuff[0], out followMode))
                return false;

            if (followMode == 0)
            {
                user.BotData.ForcedUserTargetMovement = 0;

                if (user.IsWalking)
                    user.ClearMovement(true);
            }
            else if (followMode == 1)
            {
                user.BotData.ForcedUserTargetMovement = player.Id;

                if (user.IsWalking)
                    user.ClearMovement(true);
                user.MoveTo(human.X, human.Y);
            }

            return true;
        }
    }
}