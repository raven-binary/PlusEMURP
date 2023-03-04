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
    class EatCommand : IChatCommand
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
            get { return "Eat something"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :eat <product>");
                return;
            }

            if (Session.GetRoleplay().Energy > 99)
            {
                Session.SendWhisper("You don't have to eat anything because your energy is already full.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User.isTradingItems)
            {
                Session.SendWhisper("You cannot eat while trading.");
                return;
            }

            string Produit = Params[1];
            int Energy;
            string Name;
            if (Produit == "brot")
            {
                if (Session.GetHabbo().Pain < 1)
                {
                    Session.SendWhisper("You don't have a wand.");
                    return;
                }

                Session.GetHabbo().Pain -= 1;
                Session.GetHabbo().updatePain();
                Energy = 50;
                Name = "das Brot";
            }
            else if (Produit == "lutscher")
            {
                if (Session.GetHabbo().Sucette < 1)
                {
                    Session.SendWhisper("You don't have a lollipop.");
                    return;
                }

                Session.GetHabbo().Sucette -= 1;
                Session.GetHabbo().updateSucette();
                Energy = 10;
                Name = "den Lutscher";
            }
            else if (Produit == "Snack")
            {
 
                Energy = 1;
                Name = "eine Snack";
            }
            else
            {
                Session.SendWhisper("The entered product is invalid.");
                return;
            }
            
            int NumberEnergy = 100 - Energy;
            User.GetClient().Shout("*Eats " + Name + " [+"+ Energy + "% Energy]*");
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