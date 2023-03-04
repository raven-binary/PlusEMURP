using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class GiveStockCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 5 && Session.GetHabbo().isLoggedIn)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<amount>"; }
        }

        public string Description
        {
            get { return "Add an amount to a deal transaction."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :cipher <montant>");
                return;
            }

            if (Session.GetHabbo().CurrentRoom.Group == null)
                return;

            int num;
            if (!Int32.TryParse(Params[1], out num) || Params[1].StartsWith("0") || 0 > Convert.ToInt32(Params[1]))
            {
                Session.SendWhisper("The amount is invalid.");
                return;
            }

            if(Session.GetHabbo().CurrentRoom.Group.Id == 1 || Session.GetHabbo().CurrentRoom.Group.Usine == 1 || Session.GetHabbo().CurrentRoom.Group.PayedByEtat == 1 || Session.GetHabbo().CurrentRoom.Group.Id == 18)
            {
                Session.SendWhisper("This business does not need to be expanded or cannot be expanded.");
                return;
            }

            Session.GetHabbo().CurrentRoom.Group.ChiffreAffaire += Convert.ToInt32(Params[1]);
            Session.GetHabbo().CurrentRoom.Group.updateChiffre();

            if (Session.GetHabbo().Rank == 8 || Session.GetHabbo().Rank == 7 || Session.GetHabbo().Rank == 6 || Session.GetHabbo().Working == false)
            {
                Group Gouvernement = null;
                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(6, out Gouvernement))
                {
                    Gouvernement.ChiffreAffaire -= Convert.ToInt32(Params[1]);
                    Gouvernement.updateChiffre();
                }
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            User.OnChat(User.LastBubble, "* Increases the economy of this business " + Params[1] + " euros$ * ", true);
        }
    }
}