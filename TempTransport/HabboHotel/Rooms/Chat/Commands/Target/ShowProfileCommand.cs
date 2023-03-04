using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using System.Data;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class ShowProfileCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "target"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Shows the profile from a specific target"; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            string Username = Params[1];
            DataRow User = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id,username FROM `users` WHERE `username` = @username LIMIT 1;");
                dbClient.AddParameter("username", Username);
                User = dbClient.getRow();
            }

            if (User == null)
            {
                Session.SendWhisper("Player not found");
                return;
            }

            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "profile", "open," + User["id"] + ",false");
        }
    }
}
