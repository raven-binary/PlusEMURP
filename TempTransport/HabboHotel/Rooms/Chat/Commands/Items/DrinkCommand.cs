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
    class DrinkCommand : IChatCommand
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
            get { return "Drink something"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :drink <product>");
                return;
            }

            if (Session.GetRoleplay().Energy > 99)
            {
                Session.SendWhisper("You don't have to drink anything because your energy is already full.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if(User.isTradingItems)
            {
                Session.SendWhisper("You cannot drink while trading.");
                return;
            }

            string Produit = Params[1];
            int Energy;
            string Name;
            if (Produit == "cola" || Produit == "coca")
            {
                if (Session.GetHabbo().Coca < 1)
                {
                    Session.SendWhisper("You don't have any cola.");
                    return;
                }

                Session.GetHabbo().Coca -= 1;
                Session.GetHabbo().updateCoca();
                Energy = 30;
                Name = "eine Coca Cola";
            }
            else if (Produit == "fanta")
            {
                if (Session.GetHabbo().Fanta < 1)
                {
                    Session.SendWhisper("You don't have a Fanta.");
                    return;
                }

                Session.GetHabbo().Fanta -= 1;
                Session.GetHabbo().updateFanta();
                Energy = 20;
                Name = "eine Fanta";
            }
            else
            {
                Session.SendWhisper("The returned product is invalid.");
                return;
            }
            
            int NumberEnergy = 100 - Energy;
            User.GetClient().Shout("*Drinks "+ Name + " [+"+ Energy + "% Energy]*");
            if (Session.GetRoleplay().Energy >= NumberEnergy)
            {
                Session.GetRoleplay().Energy = 100;
                Session.GetHabbo().updateEnergy();
                
            }
            else
            {
                Session.GetRoleplay().Energy += Energy;
                Session.GetHabbo().updateEnergy();
            }

            Session.SendMessage(new WhisperComposer(User.VirtualId, "Energy : "+ Session.GetRoleplay().Energy  + "/100", 0, 34));
        }
    }
}