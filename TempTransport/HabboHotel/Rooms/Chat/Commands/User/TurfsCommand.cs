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
    class TurfsCommand : IChatCommand
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
            get { return "Shows the turfs"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            StringBuilder Rooms = new StringBuilder();

            Room Turf1 = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(47);
            Room Turf2 = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(140);
            Room Turf3 = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(145);

            if (Turf1.Capture > 0)
            {
                if (Turf1.Assault > 0)
                {
                    Rooms.Append(Turf1.Name + "\n- " + Session.GetHabbo().getNameOfThisGang(Turf1.Capture) + " ASSAULTED\n");
                }
                else
                {
                    Rooms.Append(Turf1.Name + "\n- Controlled by: " + Session.GetHabbo().getNameOfThisGang(Turf1.Capture) + "\n");
                }
            }
            else
            {
                if (Turf1.Assault > 0)
                {
                    Rooms.Append(Turf1.Name + "\n - ASSAULTED\n");
                }
                else
                {
                    Rooms.Append(Turf1.Name + "\n - Uncontrolled\n");
                }
            }

            if (Turf2.Capture > 0)
            {
                if (Turf2.Assault > 0)
                {
                    Rooms.Append(Turf2.Name + "\n - " + Session.GetHabbo().getNameOfThisGang(Turf2.Capture) + " ASSAULTED\n");
                }
                else
                {
                    Rooms.Append(Turf2.Name + "\n - Controlled by: " + Session.GetHabbo().getNameOfThisGang(Turf2.Capture) + "\n");
                }
            }
            else
            {
                if (Turf2.Assault > 0)
                {
                    Rooms.Append(Turf2.Name + "\n - ASSAULTED\n");
                }
                else
                {
                    Rooms.Append(Turf2.Name + "\n - Uncontrolled\n");
                }
            }

            if (Turf3.Capture > 0)
            {
                if (Turf3.Assault > 0)
                {
                    Rooms.Append(Turf3.Name + "\n - " + Session.GetHabbo().getNameOfThisGang(Turf3.Capture) + " ASSAULTED\n");
                }
                else
                {
                    Rooms.Append(Turf3.Name + "\n - Controlled by: " + Session.GetHabbo().getNameOfThisGang(Turf3.Capture) + "\n");
                }
            }
            else
            {
                if (Turf3.Assault > 0)
                {
                    Rooms.Append(Turf3.Name + "\n - ASSAULTED\n");
                }
                else
                {
                    Rooms.Append(Turf3.Name + "\n - Uncontrolled\n");
                }
            }

            Session.SendMessage(new MOTDNotificationComposer("Turfs\n\n" + Rooms.ToString()));
        }
    }
}
