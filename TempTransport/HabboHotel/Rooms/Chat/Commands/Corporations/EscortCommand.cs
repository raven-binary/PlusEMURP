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
    class EscortCommand : IChatCommand
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
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Escort"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :escort <username>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);


            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (Session.GetHabbo().Escorting && TargetClient.GetHabbo().EscortBy == Session.GetHabbo().Username)
            {
                User.Say("lets go of " + TargetClient.GetHabbo().Username);
                TargetUser.UltraFastWalking = false;
                TargetClient.GetHabbo().EscortBy = null;
                Session.GetHabbo().Escorting = false;
                Session.GetHabbo().EscortUsername = null;
                return;
            }
            else if (TargetClient.GetHabbo().Cuffed && TargetClient.GetHabbo().EscortBy == null)
            {
                User.Say("wraps their cuffs around " + TargetClient.GetHabbo().Username + "'s wrist");
                User.Say("grabs " + TargetClient.GetHabbo().Username + " by the cuffs and starts escorting them");

                TargetClient.GetHabbo().Escort = true;
                TargetClient.GetHabbo().EscortBy = Session.GetHabbo().Username;
                Session.GetHabbo().Escorting = true;
                Session.GetHabbo().EscortUsername = TargetClient.GetHabbo().Username;
                return;
            }

            if (Math.Abs(User.X - TargetUser.X) > 2 || Math.Abs(User.Y - TargetUser.Y) > 2)
            {
                Session.SendWhisper("You must next to the target");
                return;
            }

            if (Session.GetHabbo().UsingParamedic && TargetClient.GetHabbo().IsWaitingForParamedic)
            {
                if (TargetClient.GetHabbo().WaitingForParamedicFrom != Session.GetHabbo().Username)
                {
                    Session.SendWhisper("A worker already accepted the paramedic call for " + TargetClient.GetHabbo().Username);
                    return;
                }

                TargetUser.Statusses.Remove("lay");
                TargetUser.Z += 0.35;
                TargetUser.isLying = false;
                TargetUser.Freezed = false;
                TargetUser.UpdateNeeded = true;

                User.Say("grabs " + TargetClient.GetHabbo().Username + " and begins escorting them to hospital");
                Session.GetHabbo().ParamedicUsername = TargetClient.GetHabbo().Username;
                Session.GetHabbo().resetEffectEvent();
                TargetClient.GetHabbo().resetEffectEvent();

                if (User.RotBody == 0)
                    TargetUser.MoveTo(User.SetX, User.SetY - 1);
                else if (User.RotBody == 1)
                    TargetUser.MoveTo(User.SetX + 1, User.SetY - 1);
                else if (User.RotBody == 2)
                    TargetUser.MoveTo(User.SetX + 1, User.SetY);
                else if (User.RotBody == 3)
                    TargetUser.MoveTo(User.SetX + 1, User.SetY + 1);
                else if (User.RotBody == 4)
                    TargetUser.MoveTo(User.SetX, User.SetY + 1);
                else if (User.RotBody == 5)
                    TargetUser.MoveTo(User.SetX - 1, User.SetY + 1);
                else if (User.RotBody == 6)
                    TargetUser.MoveTo(User.SetX - 1, User.SetY);
                else if (User.RotBody == 7)
                    TargetUser.MoveTo(User.SetX - 1, User.SetY - 1);
                if (TargetUser.RotBody != User.RotBody)
                    TargetClient.GetHabbo().SetRot(User.RotBody);
            }
        }
    }
}