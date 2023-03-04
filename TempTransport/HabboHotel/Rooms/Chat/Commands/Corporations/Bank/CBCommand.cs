using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class CBCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 4 && Session.GetHabbo().Working == true)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<pseudonyme>"; }
        }

        public string Description
        {
            get { return "Pass through a citizen’s BANK CARD."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :cb <citizen name>");
                return;
            }

            if (Session.GetHabbo().getCooldown("bank_command"))
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User.ConnectedMetier == false)
            {
                Session.SendWhisper("You are not connected to the bank network.");
                return;
            }

            if(TargetClient.GetHabbo().Cb != "null")
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a bank card.");
                return;
            }
            
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser.isMakingCard == true)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already makes his card.");
                return;
            }

            Session.GetHabbo().addCooldown("bank_command", 2000);
            TargetUser.Frozen = true;
            TargetUser.isMakingCard = true;
            User.OnChat(User.LastBubble, "* Executes  " + TargetClient.GetHabbo().Username + "'s bank card *", true);
            TargetUser.OnChat(TargetUser.LastBubble, "* Dials their pin code *", true);
            TargetClient.SendWhisper("Please enter the pin code for your bank card.");
        }
    }
}