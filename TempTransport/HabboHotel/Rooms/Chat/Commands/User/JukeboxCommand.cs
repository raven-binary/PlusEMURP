using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.Database.Interfaces;
using System.Data;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Notifications;
namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class JukeboxCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Shows the songs in queue"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            StringBuilder Songs = new StringBuilder();

            for (int i = 0; i < 100; i++)
            {
                DataRow UpNext = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `jukebox_requests` WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", i);
                    UpNext = dbClient.getRow();
                }

                i += 1;

                if (UpNext != null)
                {
                    Songs.Append(PlusEnvironment.UpNextReq(Convert.ToString(UpNext["video_id"]) + "\n"));
                }
            }

            Session.SendMessage(new MOTDNotificationComposer("Songs in queue\n\n" + Songs.ToString()));
        }
    }
}
