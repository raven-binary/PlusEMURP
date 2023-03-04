using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class ItsMeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 5)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<pin>"; }
        }

        public string Description
        {
            get { return "change bubble color"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, you forgot something");
                return;
            }
            else if (Session.GetHabbo().isLoggedIn)
            {
                Session.SendWhisper("You are already verified!");
            }
            else if (Params[1].ToLower() != Session.GetHabbo().Username.ToLower())
            {
                Session.SendWhisper("You can only log into your own account");
            }
            else if (Session.GetHabbo().Username.ToLower() == Params[1].ToLower())
            {
                string thisPin = Params[2];
                string pin;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `pin` FROM users WHERE `id` = " + Session.GetHabbo().Id + " LIMIT 1");
                    dbClient.AddParameter("password", thisPin);
                    pin = dbClient.getString();
                }
                if (pin == Params[2])
                {
                    Session.GetHabbo().isLoggedIn = true;
                    PlusEnvironment.GetGame().GetClientManager().StaffWhisper(Session.GetHabbo().Username + " has successfully verified themselves");
                }
                else if (pin != Params[2])
                {
                    Session.SendWhisper("The entered PIN is invalid!");
                    PlusEnvironment.GetGame().GetClientManager().StaffWhisper(Session.GetHabbo().Username + " couldn't verify himself and was kicked from the client for security reasons!");
                    Task t = Task.Run(async delegate
                    {
                        await Task.Delay(1500);
                        Session.Disconnect();
                    });
                }
            }
        }
    }
}
