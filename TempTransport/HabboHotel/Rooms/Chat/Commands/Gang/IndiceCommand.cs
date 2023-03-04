using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class IndiceCommande : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().EventCount == 7 && Session.GetHabbo().Rank == 8)
                return true;

            return false;
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
            get { return "Check the index."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "eventMission;alert;Indice;https://i.imgur.com/zbUNVD7.gif;Document texte;Here is the plan:<br /><i>When Santa Claus is in the city center of " + PlusEnvironment.Hotelname + " we will intercept.<br />We'll have to be discreet and quick, nobody should see us.<br /><br />You have to know that the HQ is the attic of the inhabitants, the plant as for it, it will be entrance.<br/><br/><b>Malutin</b></i>");
        }
    }
}