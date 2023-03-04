using Plus.HabboHotel.GameClients;
using WebHook;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ChargeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().Working)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<username> <charge>"; }
        }

        public string Description
        {
            get { return "Charges a player with a specific charge"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);

            if (TargetClient == null)
            {
                Session.SendWhisper("Player not found");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            string Charge = Params[2];
            if (Charge == "assault")
            {
                TargetClient.GetRoleplay().Wan.Assault += 1;
            }
            else if (Charge == "murder")
            {
                TargetClient.GetRoleplay().Wan.Murder += 1;
            }
            else if (Charge == "copassault")
            {
                TargetClient.GetRoleplay().Wan.Copassault += 1;
            }
            else if (Charge == "copmurder")
            {
                TargetClient.GetRoleplay().Wan.Copmurder += 1;
            }
            else if (Charge == "ganghomicide")
            {
                if (TargetClient.GetRoleplay().Wan.Ganghomicide == 1)
                {
                    Session.SendWhisper("This player is already charged for ganghomicide");
                    return;
                }
                TargetClient.GetRoleplay().Wan.Ganghomicide = 1;
            }
            else if (Charge == "obstruction")
            {
                TargetClient.GetRoleplay().Wan.Obstruction += 1;
            }
            else if (Charge == "hacking")
            {
                TargetClient.GetRoleplay().Wan.Hacking += 1;
            }
            else if (Charge == "trespassing")
            {
                TargetClient.GetRoleplay().Wan.Trespassing += 1;
            }
            else if (Charge == "robbery")
            {
                TargetClient.GetRoleplay().Wan.Robbery += 1;
            }
            else if (Charge == "illegalarea")
            {
                TargetClient.GetRoleplay().Wan.Illegalarea += 1;
            }
            else if (Charge == "jailbreak")
            {
                TargetClient.GetRoleplay().Wan.Jailbreak += 1;
            }
            else if (Charge == "terrorism")
            {
                TargetClient.GetRoleplay().Wan.Terrorism += 1;
            }
            else if (Charge == "drugs")
            {
                TargetClient.GetRoleplay().Wan.Drugs += 1;
            }
            else if (Charge == "execution")
            {
                TargetClient.GetRoleplay().Wan.Execution += 1;
            }
            else if (Charge == "escaping")
            {
                TargetClient.GetRoleplay().Wan.Escaping += 1;
            }
            else if (Charge == "non-compliance")
            {
                TargetClient.GetRoleplay().Wan.NonCompliance += 1;
            }
            else if (Charge == "911abuse")
            {
                TargetClient.GetRoleplay().Wan.CallAbuse += 1;
            }
            else
            {
                Session.SendWhisper("The charge is invalid");
                return;
            }

            if (!TargetClient.GetRoleplay().Wan.Charges.ContainsKey(Charge))
            {
                TargetClient.GetRoleplay().Wan.Charges.Add(Charge, 0);
            }
            else
            {
                TargetClient.GetRoleplay().Wan.Charges.Remove(Charge);
                TargetClient.GetRoleplay().Wan.Charges.Add(Charge, 0);
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            User.Say("charges " + TargetClient.GetHabbo().Username + " with " + Charge);
            TargetClient.SendWhisper("You have been charged " + Charge + " by " + Session.GetHabbo().Username, 7);
            TargetClient.GetRoleplay().Wan.Add();
            PlusEnvironment.GetGame().GetClientManager().PoliceRadio(Session.GetHabbo().Username + " charged " + TargetClient.GetHabbo().Username + " with " + Charge + " @ " + Session.GetHabbo().CurrentRoom.Name);

            //Webhook.SendCopFeed(Session.GetHabbo().RankInfo.Name + " **" + Session.GetHabbo().Username + "** charged **" + TargetClient.GetHabbo().Username + "** with " + Charge + " @ __" + Session.GetHabbo().CurrentRoom.Name + "__");
        }
    }
}