using System;
using System.Linq;
using System.Text;
using System.Data;
using Fleck;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ViewGangCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "gang"; }
        }

        public string Parameters
        {
            get { return "<gang>"; }
        }

        public string Description
        {
            get { return "Shows a gang"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :gangview <gang>");
                return;
            }

            DataRow GetGang = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `gang` WHERE `name` = @gang LIMIT 1");
                dbClient.AddParameter("gang", Params[1]);
                GetGang = dbClient.getRow();
            }

            if (GetGang == null)
            {
                Session.SendWhisper("This gang does not exist");
                return;
            }

            if (Convert.ToInt32(GetGang["id"]) == Session.GetHabbo().Gang)
            {
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "gang;toggle;" + Session.GetHabbo().Gang + ";" + Session.GetHabbo().getNameOfGang());
                return;
            }

            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "gang-view;" + GetGang["id"]);
        }
    }
}