using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class StyleCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 5 && Session.GetHabbo().Working == true)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Changes players requested hair"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :style <username>");
                return;
            }

            string Username = Params[1];
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found");
                return;
            }

            if (User.GoalX != TargetUser.GoalX && User.GoalY != TargetUser.GoalY - 1)
            {
                Session.SendWhisper("You must behind to the target");
                return;
            }

            if (TargetUser.Item_On != 3451 || TargetClient.GetHabbo().Hair == null)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " did not requested for a new hairstyle");
                return;
            }

            User.Freezed = true;
            TargetUser.Freezed = true;

            User.Say("begins cutting " + TargetClient.GetHabbo().Username + " hair");
            User.ApplyEffect(4);

            System.Timers.Timer StyleTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(5));
            StyleTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(5);
            StyleTimer.Elapsed += delegate
            {
                if (TargetClient != null)
                {
                    User.Freezed = false;
                    TargetUser.Freezed = false;
                    Session.GetHabbo().resetEffectEvent();
                    User.Say("finishes cutting " + TargetClient.GetHabbo().Username + " hair giving the desired hairstyle");

                    Session.SendWhisper("You have received a $15 tip for completing a hairstyle");
                    Session.GetHabbo().Credits += 15;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    Session.GetHabbo().RPCache(3);

                    TargetClient.SendWhisper("You have been charged $50 for this hairstyle");
                    TargetClient.GetHabbo().Credits -= 50;
                    TargetClient.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    TargetClient.GetHabbo().RPCache(3);

                    TargetClient.GetHabbo().Look = TargetClient.GetHabbo().Hair;
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE users SET look = @look WHERE `id` = '" + TargetClient.GetHabbo().Id + "' LIMIT 1");
                        dbClient.AddParameter("look", TargetClient.GetHabbo().Look);
                        dbClient.RunQuery();
                    }
                    TargetClient.GetHabbo().Hair = null;
                    TargetClient.SendMessage(new UserChangeComposer(TargetUser, true));
                    TargetClient.GetHabbo().CurrentRoom.SendMessage(new UserChangeComposer(TargetUser, false));
                    TargetClient.GetHabbo().resetAvatarEvent();
                }
                else
                {
                    Session.SendWhisper("Player left the game");
                    User.Freezed = false;
                    Session.GetHabbo().resetEffectEvent();
                }
                StyleTimer.Stop();
            };
            StyleTimer.Start();
        }
    }
}