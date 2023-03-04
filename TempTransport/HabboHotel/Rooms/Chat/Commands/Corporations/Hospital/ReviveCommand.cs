using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using WebHook;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ReviveCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 2 && Session.GetHabbo().Working == true)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Escort your patient here to revive them"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Session.GetHabbo().ParamedicUsername);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (Session.GetHabbo().CurrentRoomId != 63)
            {
                Session.SendWhisper("Escort your patient to the Hospital Patient Room to revive them");
                return;
            }

            if (User.Item_On != 500013)
            {
                Session.SendWhisper("Escort your patient to the Paramedic Point to revive them");
                return;
            }

            if (User.Item_On == 500013)
            {
                TargetClient.GetHabbo().Hospital = 1;
                TargetClient.GetHabbo().updateHospitalEtat(TargetUser, 3);
                User.FastWalking = false;
                User.Say("unloads " + TargetClient.GetHabbo().Username + " out of their ambulance and transports them to a bed");
                Session.SendWhisper("You have received a $20 tip for reviving "+ TargetClient.GetHabbo().Username);
                Session.GetHabbo().Credits += 20;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                Session.GetHabbo().RPCache(3);
                TargetClient.GetHabbo().WaitingForParamedicFrom = null;
                TargetClient.GetHabbo().IsWaitingForParamedic = false;
                TargetUser.SuperFastWalking = false;
                TargetUser.AllowOverride = false;
                TargetClient.GetHabbo().resetEffectEvent();

                Session.GetHabbo().UsingParamedic = false;
                Session.GetHabbo().ParamedicUsername = null;
                User.FastWalking = false;
                User.AllowOverride = false;
                Session.GetHabbo().resetEffectEvent();

                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Session.GetHabbo().Username + "</span> revived <span class=\"red\">" + TargetClient.GetHabbo().Username);
                Webhook.SendWebhook(":ambulance: " + Session.GetHabbo().Username + " revived " + TargetClient.GetHabbo().Username);
            }
        }
    }
}