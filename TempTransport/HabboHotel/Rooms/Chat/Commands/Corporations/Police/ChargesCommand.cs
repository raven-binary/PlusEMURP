using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;


using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ChargesCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<charges>"; }
        }

        public string Description
        {
            get { return "Shows the charges of a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Charges = string.Empty;
            if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().Working && Params.Length == 2)
            {
                string Username = Params[1];
                GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                if (TargetClient == null)
                {
                    Session.SendWhisper("Player not found");
                    return;
                }

                if (!TargetClient.GetRoleplay().Wan.IsWanted)
                {
                    Session.SendWhisper(TargetClient.GetHabbo().Username + " has no charges");
                    return;
                }

                foreach (var Charge in TargetClient.GetRoleplay().Wan.Charges.OrderBy(p => p.Key))
                {
                    if (String.IsNullOrWhiteSpace(Charges))
                    {
                        Charges = TargetClient.GetRoleplay().Wan.GetArrestTimeByCharge(Charge.Key) + " " + Charge.Key;
                    }
                    else
                    {
                        Charges += ", " + TargetClient.GetRoleplay().Wan.GetArrestTimeByCharge(Charge.Key) + " " + Charge.Key;
                    }
                }

                Session.SendWhisper("Charges for " + TargetClient.GetHabbo().Username + ": " + Charges);
                Session.SendWhisper("Arrest time: " + TargetClient.GetRoleplay().Wan.ArrestTime() + " minutes");
            }
            else
            {
                if (!Session.GetRoleplay().Wan.IsWanted)
                {
                    Session.SendWhisper("You don't have any charges");
                    return;
                }

                foreach (var Charge in Session.GetRoleplay().Wan.Charges.OrderBy(p => p.Key))
                {
                    if (String.IsNullOrWhiteSpace(Charges))
                    {
                        Charges = Session.GetRoleplay().Wan.GetArrestTimeByCharge(Charge.Key) + " " + Charge.Key;
                    }
                    else
                    {
                        Charges += ", " + Session.GetRoleplay().Wan.GetArrestTimeByCharge(Charge.Key) + " " + Charge.Key;
                    }
                }

                Session.SendWhisper("Charges: " + Charges);
                Session.SendWhisper("Arrest time: " + Session.GetRoleplay().Wan.ArrestTime() + " minutes, ticket cost: $" + Session.GetRoleplay().Wan.ArrestTime() * 7);
            }
        }
    }
}