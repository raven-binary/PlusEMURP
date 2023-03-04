using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SmokeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Hospital == 0)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "items"; }
        }

        public string Parameters
        {
            get { return "<product>"; }
        }

        public string Description
        {
            get { return "Smoke a product"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :smoke <product>");
                return;
            }

            if(Session.GetHabbo().getCooldown("fumer"))
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User.isTradingItems)
            {
                Session.SendWhisper("You cannot smoke while trading.");
                return;
            }

            string Produit = Params[1];
            if (Produit == "weed")
            {
                if (Session.GetHabbo().Weed < 2)
                {
                    Session.SendWhisper("You need 2g of weed to smoke one weed.");
                    return;
                }

                if (Session.GetHabbo().PhilipMo < 1)
                {
                    Session.SendWhisper("You need a cigarette to roll.");
                    return;
                }

                if (Session.GetHabbo().Clipper < 1)
                {
                    Session.SendWhisper("You don't have a lighter.");
                    return;
                }

                Session.GetHabbo().PhilipMo -= 1;
                Session.GetHabbo().updatePhilipMo();
                Session.GetHabbo().Weed -= 2;
                Session.GetHabbo().updateWeed();
                Session.GetHabbo().Clipper -= 1;
                Session.GetHabbo().updateClipper();
            }
            else if (Produit == "zigarette")
            {
                if (Session.GetHabbo().PhilipMo < 1)
                {
                    Session.SendWhisper("You don't have a cigarette.");
                    return;
                }

                if (Session.GetHabbo().Clipper < 1)
                {
                    Session.SendWhisper("You don't have a lighter.");
                    return;
                }

                if(Session.GetHabbo().Health < 10)
                {
                    Session.SendWhisper("Your health is not good, so you cannot smoke!");
                    return;
                }

                Session.GetHabbo().PhilipMo -= 1;
                Session.GetHabbo().updatePhilipMo();
                Session.GetHabbo().Clipper -= 1;
                Session.GetHabbo().updateClipper();
            }
            else
            {
                Session.SendWhisper("The entered product is invalid.");
                return;
            }

            Session.GetHabbo().addCooldown("fumer", 10000);
            if (Produit == "weed")
            {
                User.GetClient().Shout("*Takes a cigarette from the pack and empties the tobacco in the pack*");
                User.GetClient().Shout("*Add 2g of weed and roll the joint*");
                User.GetClient().Shout("*Light his weed with a lighter and smoke [+50% HEALTH]*");

                if (Session.GetHabbo().Health > 50)
                {
                    Session.GetHabbo().Health = Session.GetHabbo().HealthMax;
                    Session.GetHabbo().updateSante();

                }
                else
                {
                    Session.GetHabbo().Health += 50;
                    Session.GetHabbo().updateSante();
                }
                Session.GetHabbo().SmokeTimer += 1;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "smoke;start");
            }
            else
            {
                User.GetClient().Shout("*Takes a cigarette from the pack [-2% HEALTH]*");
                if (Session.GetHabbo().Health > 1)
                {
                    Session.GetHabbo().Health -= 2;
                    Session.GetHabbo().updateSante();

                }
            }
        }
    }
}