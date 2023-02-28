using System;
using System.Text;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    internal class StatsCommand : IChatCommand
    {
        public string PermissionRequired => "command_stats";

        public string Parameters => "";

        public string Description => "View your current statistics.";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            double minutes = session.GetHabbo().GetStats().OnlineTime / 60;
            double hours = minutes / 60;
            int onlineTime = Convert.ToInt32(hours);
            string s = onlineTime == 1 ? "" : "s";

            StringBuilder habboInfo = new();
            habboInfo.Append("Your account stats:\r\r");

            habboInfo.Append("Currency Info:\r");
            habboInfo.Append("Credits: " + session.GetHabbo().Credits + "\r");
            habboInfo.Append("Duckets: " + session.GetHabbo().Duckets + "\r");
            habboInfo.Append("Diamonds: " + session.GetHabbo().Diamonds + "\r");
            habboInfo.Append("Online Time: " + onlineTime + " Hour" + s + "\r");
            habboInfo.Append("Respects: " + session.GetHabbo().GetStats().Respect + "\r");
            habboInfo.Append("GOTW Points: " + session.GetHabbo().GotwPoints + "\r\r");

            session.SendNotification(habboInfo.ToString());
        }
    }
}