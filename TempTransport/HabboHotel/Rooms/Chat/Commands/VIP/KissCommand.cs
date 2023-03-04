using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class KissCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 1)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "vip"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Kisses a player"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a targe");
                return;
            }

            if (PlusEnvironment.getCooldown("kiss" + Session.GetHabbo().Id))
            {
                Session.SendWhisper("You must wait before you can perform this action again");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Session.GetHabbo().usingArena)
            {
                Session.SendWhisper("You cannot perform this action while fighting in the arena");
                return;
            }

            if (Session.GetHabbo().Cuffed)
            {
                Session.SendWhisper("You cannot perform this action while cuffed");
                return;
            }

            User.Say("kisses " + TargetClient.GetHabbo().Username, 16);
            PlusEnvironment.addCooldown("kiss" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertSecondsToMilliseconds(5)));
        }
    }
}
