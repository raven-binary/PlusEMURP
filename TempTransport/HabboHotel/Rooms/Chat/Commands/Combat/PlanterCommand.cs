/*sing System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class PlanterCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "arme"; }
        }

        public string Parameters
        {
            get { return "<bürgername>"; }
        }

        public string Description
        {
            get { return "Stelle einen Bürger auf"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Ungültige Syntax :plan <bürgername>");
                return;
            }

            if (Session.GetHabbo().ArmeEquiped != "sabre")
            {
                Session.SendWhisper("Du musst einen Schwert haben, um einen Bürger anlegen zu können.");
                return;
            }

            if (Session.GetHabbo().Hospital == 1)
                return;

            if (Session.GetHabbo().CurrentRoomId == 3 || Session.GetHabbo().CurrentRoomId == 18 || Session.GetHabbo().CurrentRoomId == 20)
            {
                Session.SendWhisper("Hier kannst du keine Waffe ausrüsten!");
                return;
            }

            if (Session.GetHabbo().ArmeEquiped != null && Session.GetHabbo().CurrentRoom.Description.Contains("INDOOR") && PlusEnvironment.Salade != Session.GetHabbo().CurrentRoomId && PlusEnvironment.Purge == false)
            {
                Session.SendWhisper("Du kannst hier nicht kämpfen.");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " könnte nicht in diesem Raum gefunden werden.");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
            {
                Session.SendWhisper("Du kannst dich nicht selber pflanzen.");
                return;
            }

            if (TargetClient.GetHabbo().Hospital == 1)
                return;
            
            if(TargetClient.GetHabbo().Rank == 8 && PlusEnvironment.Salade == Session.GetHabbo().CurrentRoomId)
            {
                Session.SendWhisper("Dieser Bürger konnte während des Salats nicht abstürzen.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Session.GetHabbo().Menotted == true || User.Tased == true)
            {
                Session.SendWhisper("Du kannst nicht zuschlagen, wenn du dich nicht bewegen kannst.");
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (User.Immunised == true)
            {
                Session.SendWhisper("Du kannst keinen Bürger schlagen, solang du Immun bist.");
                return;
            }

            if (TargetUser.Immunised == true)
            {
                Session.SendWhisper("Du kannst " + TargetClient.GetHabbo().Username + " nicht schlagen, weil er Immunität geniesst");
                return;
            }

            if (User.DuelUser != null && TargetClient.GetHabbo().Username != User.DuelUser)
            {
                Session.SendWhisper("Du kannst " + TargetClient.GetHabbo().Username + " nicht schlagen, solange du dich mit " + User.DuelUser + " duellierst.");
                return;
            }

            if (TargetUser.DuelUser != null && TargetUser.DuelUser != Session.GetHabbo().Username)
            {
                Session.SendWhisper("Du kannst " + TargetClient.GetHabbo().Username + " nicht schlagen, weil er sich gerade duelliert.");
                return;
            }

            int Range;
            int DegatMin;
            int DegatMax;
            string Name;

            if (Session.GetHabbo().ArmeEquiped == "sabre")
            {
                Range = 1;
                DegatMin = 12;
                DegatMax = 21;
                Name = "Schwert";
            }
            else
            {
                return;
            }
            
            if (Math.Abs(User.Y - TargetUser.Y) > Range || Math.Abs(User.X - TargetUser.X) > Range)
            {
                User.GetClient().Shout("*Versucht " + TargetClient.GetHabbo().Username + " mit den " + Name + " zu verletzen, verfehlt dennoch [-2% Energy]*");
                Session.GetHabbo().Chargeur = Session.GetHabbo().Chargeur - 1;
                Session.GetHabbo().EnergyFaintness(2);
                return;
            }

            Random degatTaked = new Random();
            int degatTakedNumber = degatTaked.Next(DegatMin, DegatMax);
            if(TargetClient.GetHabbo().Health > degatTakedNumber)
            {
                User.GetClient().Shout("*Rammt das " + Name + " gegen " + TargetClient.GetHabbo().Username + "'s bauch und verliert " + degatTakedNumber + " Gesundheit [-2% Energy] *");
                TargetClient.GetHabbo().Health -= degatTakedNumber;
                TargetClient.GetHabbo().updateSante();
            }
            else
            {
                TargetClient.GetHabbo().Hospital = 1;
                int Voler = 0;
                if (TargetClient.GetHabbo().Rank == 1 && PlusEnvironment.Purge == false && Session.GetHabbo().Hospital == 0 && PlusEnvironment.Salade != Session.GetHabbo().CurrentRoomId)
                {
                    decimal decimalCredits = Convert.ToInt32(TargetClient.GetHabbo().Credits);
                    Voler = Convert.ToInt32((decimalCredits / 100m) * 50m);
                }
                User.GetClient().Shout("*Hat " + TargetClient.GetHabbo().Username + " mit seinem " + Name + " getötet  [-2% Energy]*");
                TargetUser.GetClient().Shout("*Ist kurz vor dem Tod und wird ins Krankenhaus gebracht*");
                if (Voler > 0)
                {
                    User.GetClient().Shout("*Klaut " + Voler + " Euro von " + TargetClient.GetHabbo().Username + "*");
                    TargetClient.GetHabbo().Credits -= Voler;
                    TargetClient.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "my_stats;" + TargetClient.GetHabbo().Credits + ";" + TargetClient.GetHabbo().Duckets + ";" + TargetClient.GetHabbo().EventPoints);
                    Session.GetHabbo().Credits += Voler;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "my_stats;" + Session.GetHabbo().Credits + ";" + Session.GetHabbo().Duckets + ";" + Session.GetHabbo().EventPoints);
                }
                TargetClient.GetHabbo().updateHospitalEtat(TargetUser, 10);
                TargetClient.GetHabbo().Health = 0;
                TargetClient.GetHabbo().updateSante();
                Session.GetHabbo().Kills += 1;
                Session.GetHabbo().updateKill();
                Session.GetHabbo().updateGangKill();
                Session.GetHabbo().updateXpGang(10);
                TargetClient.GetHabbo().Deaths += 1;
                TargetClient.GetHabbo().updateDeaths();
                TargetClient.GetHabbo().updateGangMort();
                if (PlusEnvironment.Purge == true && Session.GetHabbo().Gang != 0)
                {
                    PlusEnvironment.updateGangPurgeKill(Session.GetHabbo().Gang);
                }
                else if (PlusEnvironment.Salade == Session.GetHabbo().CurrentRoomId)
                {
                    PlusEnvironment.GetGame().GetClientManager().checkIfWinSalade();
                }

                if (User.DuelUser != null)
                {
                    User.DuelUser = null;
                    User.DuelToken = null;
                    User.GetClient().Shout("*Gewinnt den Duell*");
                }
                Session.GetHabbo().insertLastAction(TargetClient.GetHabbo().Username + " hat " + Name + " getötet.");
                PlusEnvironment.GetGame().GetClientManager().sendLastActionMsg("<span class=\"blue\">" + Session.GetHabbo().Username + "</span> hat <span class=\"red\">" + TargetClient.GetHabbo().Username + "</span> mit seinem " + Name + " getötet.");
                TargetUser.GetClient().SendNotification(Session.GetHabbo().Username + " hat dich getötet. Du wirst jetzt ins Krankenhaus gebracht und in 10 Minuten komplett geheilt!");
            }
        }
    }
}*/