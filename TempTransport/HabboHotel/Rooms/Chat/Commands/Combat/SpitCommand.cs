using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class SpitCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 1)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "vip"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Spits a player, dealing 1 damage"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo().getCooldown("spit_command") == true)
            {
                Session.SendWhisper("You must wait before you can spit again");
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :spit <username>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Params[1] + " not found in this room");
                return;
            }

            if (Session.GetHabbo().CheckSafezone(Username) || Session.GetHabbo().CheckWarnings())
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2 || User.X != TargetUser.X && User.Y != TargetUser.Y)
            {
                Session.SendWhisper("You must be next to the target");
                return;
            }

            Room CurrentRoom = Session.GetHabbo().CurrentRoom;
            Random TokenRand = new Random();
            int randomId = TokenRand.Next(590000, 590090095);
            Session.GetHabbo().addCooldown("spit_command", 5000);

            Item BloodBlue = new Item(randomId, CurrentRoom.Id, 5501, "", TargetUser.X, TargetUser.Y, 0, 0, Session.GetHabbo().Id, 0, 0, 0, string.Empty, Room);
            if (CurrentRoom.GetRoomItemHandler().SetFloorItemByForce(null, BloodBlue, TargetUser.X, TargetUser.Y, 0, true, false, true))
            {
                CurrentRoom.SendMessage(new ObjectUpdateComposer(BloodBlue, 0));
                BloodBlue.ExtraData = "5";
                BloodBlue.UpdateState(false, true);

                User.GetClient().Shout("*spits at " + TargetClient.GetHabbo().Username + ", dealing 1 damage*", 4);
                Session.GetHabbo().CreateAggression(100);
                TargetClient.GetRoleplay().Health -= 1;
                TargetClient.GetHabbo().RPCache(1);
                TargetUser.GetClient().GetHabbo().HPBarley();

                System.Timers.Timer timer1 = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(2));
                timer1.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(2);
                timer1.Elapsed += delegate
                {
                    CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, BloodBlue.Id);
                    timer1.Stop();
                };
                timer1.Start();
            }
        }
    }
}
