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
    class MedicamentCommand : IChatCommand
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
            get { return "Take medicine"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :medicament <product>");
                return;
            }

            if (Session.GetHabbo().Health > 199)
            {
                Session.SendWhisper("You don't need to take any medication as your health is already full.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User.isTradingItems)
            {
                Session.SendWhisper("You cannot take medication while trading.");
                return;
            }

            if (Session.GetHabbo().getCooldown("medicament"))
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            string Produit = Params[1];
            int Sante;
            string Name;
            if (Produit == "doliprane")
            {
                if (Session.GetHabbo().Doliprane < 1)
                {
                    Session.SendWhisper("You don't have any medication.");
                    return;
                }

                Session.GetHabbo().Doliprane -= 1;
                Session.GetHabbo().updateDoliprane();
                Sante = 50;
                Name = "medikit";
            }
            else
            {
                Session.SendWhisper("The entered product is invalid.");
                return;
            }

            Session.GetHabbo().addCooldown("medicament", 6000);
            int NumberSante = Session.GetHabbo().HealthMax - Sante;
            User.GetClient().Shout("*uses a " + Name + "*");
            if (Session.GetHabbo().Health >= NumberSante)
            {
                Session.GetHabbo().Health = Session.GetHabbo().HealthMax;
                Session.GetHabbo().updateSante();

            }
            else
            {
                Session.GetHabbo().Health += Sante;
                Session.GetHabbo().updateSante();
            }
        }
    }
}