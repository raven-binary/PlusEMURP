/*using System;
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
    class ShootCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "weapon"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Erschieße einen Bürger"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Ungültige Syntax :shoot <bürgername>");
                return;
            }

            if (Session.GetHabbo().ArmeEquiped == null || Session.GetHabbo().ArmeEquiped == "bat" || Session.GetHabbo().ArmeEquiped == "sabre")
            {
                Session.SendWhisper("Du musst mit einer Schusswaffe ausstatten, um einen Bürger zu erschießen.");
                return;
            }

            if (Session.GetHabbo().Hospital == 1)
                return;

            if (Session.GetHabbo().getCooldown("shoot_command") == true)
            {
                Session.SendWhisper("You're shooting too fast, wait a litte bit!");
                return;
            }

            if (Room.RoomData.Description.Contains("SAFEZONE"))
            {
                Session.SendWhisper("You cannot equip weapons here.");
                return;
            }

            if (Session.GetHabbo().CurrentRoomId == 3 || Session.GetHabbo().CurrentRoomId == 18 || Session.GetHabbo().CurrentRoomId == 20)
            {
                Session.SendWhisper("You cannot equip weapons here.");
                return;
            }

            if (Session.GetHabbo().ArmeEquiped != null && Session.GetHabbo().CurrentRoom.Description.Contains("INDOOR") && PlusEnvironment.Purge == false)
            {
                Session.SendWhisper("You can't shoot here.");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room.");
                return;
            }

            if (Session.GetHabbo().Recharge == true)
            {
                Session.SendWhisper("Du kannst nicht feuern, während du deine Waffe nachlädst.");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
            {
                Session.SendWhisper("You can't shoot yourself.");
                return;
            }

            if (TargetClient.GetHabbo().Hospital == 1)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Session.GetHabbo().Menotted == true || User.Tased == true)
            {
                Session.SendWhisper("Du kannst nicht schießen, wenn du bewegungsunfähig bist.");
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (User.Immunised == true)
            {
                Session.SendWhisper("Du kannst keinen Bürger erschießen, wenn du immun bist.");
                return;
            }

            if (TargetUser.Immunised == true)
            {
                Session.SendWhisper("Du kannst " + TargetClient.GetHabbo().Username + " nicht schießen, weil er gerade immun ist.");
                return;
            }

            if (TargetUser.IsAsleep)
            {
                Session.SendWhisper("Du kannst " + TargetClient.GetHabbo().Username + " nicht schießen, weil er gerade abwesend ist.");
                return;
            }

            if (TargetUser.OnDuty == true)
            {
                Session.SendWhisper("You can't shoot this staff right now.");
                return;
            }

            if (TargetUser.usingFish == true)
            {
                TargetUser.usingFish = false;
                TargetUser.GetClient().Shout("*Hört auf zu angeln*");
                TargetUser.ApplyEffect(0);
                return;
            }

            if (TargetUser.usingFarm == true)
            {
                TargetUser.usingFarm = false;
                TargetUser.GetClient().Shout("*Hört auf Karotten zu Ertnen*");
                TargetUser.ApplyEffect(0);
                return;
            }

            if (User.DuelUser != null && TargetClient.GetHabbo().Username != User.DuelUser)
            {
                Session.SendWhisper("Du kannst " + TargetClient.GetHabbo().Username + " nicht erschießen, weil er gerade mit " + User.DuelUser + " im Zweikampf ist.");
                return;
            }

            if (TargetUser.DuelUser != null && TargetUser.DuelUser != Session.GetHabbo().Username)
            {
                Session.SendWhisper("Du kannst " + TargetClient.GetHabbo().Username + " nicht erschießen, weil es sich um ein Duell befindet.");
                return;
            }

            int Range;
            int DegatMin;
            int DegatMax;
            string Name;
            int Chargeur;

            Session.GetHabbo().addCooldown("shoot_command", 2000);
            if (Session.GetHabbo().ArmeEquiped == "ak47")
            {
                if (Session.GetHabbo().AK47_Munitions == 0)
                {
                    Session.SendWhisper("Du hast keine Munition mehr für deine AK47.");
                    return;
                }

                if (Session.GetHabbo().AK47_Munitions > 4)
                {
                    Chargeur = 5;
                }
                else
                {
                    Chargeur = Session.GetHabbo().AK47_Munitions;
                }

                if (Session.GetHabbo().Chargeur < 1)
                {
                    Session.GetHabbo().Recharge = true;
                    User.GetClient().Shout("*Lädt seine AK47 nach*");
                    System.Timers.Timer timer2 = new System.Timers.Timer(2500);
                    timer2.Interval = 2500;
                    timer2.Elapsed += delegate
                    {
                        User.GetClient().Shout("*Hat sein AK47 voll nachladen*");
                        Session.GetHabbo().Chargeur = Chargeur;
                        Session.GetHabbo().Recharge = false;
                        timer2.Stop();
                    };
                    timer2.Start();
                    return;
                }

                Range = 3;
                DegatMin = 11;
                DegatMax = 16;
                Name = "sa AK47";
                Session.GetHabbo().AK47_Munitions -= 1;
                Session.GetHabbo().updateAK47Munitions();
            }
            else if (Session.GetHabbo().ArmeEquiped == "uzi")
            {
                if (Session.GetHabbo().Uzi_Munitions == 0)
                {
                    Session.SendWhisper("Du hast keine Munition mehr für deine Uzi.");
                    return;
                }

                if (Session.GetHabbo().Uzi_Munitions > 6)
                {
                    Chargeur = 7;
                }
                else
                {
                    Chargeur = Session.GetHabbo().Uzi_Munitions;
                }

                if (Session.GetHabbo().Chargeur < 1)
                {
                    Session.GetHabbo().Recharge = true;
                    User.GetClient().Shout("*Lädt seine Uzi nach*");
                    System.Timers.Timer timer2 = new System.Timers.Timer(3000);
                    timer2.Interval = 3000;
                    timer2.Elapsed += delegate
                    {
                        User.GetClient().Shout("*Hat seine Uzi voll nachladen*");
                        Session.GetHabbo().Chargeur = Chargeur;
                        Session.GetHabbo().Recharge = false;
                        timer2.Stop();
                    };
                    timer2.Start();
                    return;
                }

                Range = 3;
                DegatMin = 15;
                DegatMax = 10;
                Name = "Uzi";
                Session.GetHabbo().Uzi_Munitions -= 1;
                Session.GetHabbo().updateUziMunitions();
            }
            else
            {
                return;
            }

            if (Math.Abs(User.Y - TargetUser.Y) > Range || Math.Abs(User.X - TargetUser.X) > Range || User.X != TargetUser.X && User.Y != TargetUser.Y)
            {
                User.GetClient().Shout("*Schießt auf " + TargetClient.GetHabbo().Username + " aber berührt ihn nicht [-2% Energy]*");
                Session.GetHabbo().Chargeur = Session.GetHabbo().Chargeur - 1;
                Session.GetHabbo().EnergyFaintness(2);
                return;
            }

            Random degatTaked = new Random();
            int degatTakedNumber = degatTaked.Next(DegatMin, DegatMax);
            if (TargetClient.GetHabbo().Health > degatTakedNumber)
            {
                User.GetClient().Shout("*Schießt auf " + TargetClient.GetHabbo().Username + " und verursacht 3 " + degatTakedNumber + " Schaden [-2% Energy]*");
                TargetClient.GetHabbo().Health -= degatTakedNumber;
                TargetClient.GetHabbo().updateSante();
                TargetUser.OnChat(TargetUser.LastBubble, "[GESUNDHEIT] " + TargetClient.GetHabbo().Health + "/" + TargetClient.GetHabbo().HealthMax + "", true);
            }
            else
            {
                TargetClient.GetHabbo().Hospital = 1;
                int Voler = 0;
                if (TargetClient.GetHabbo().Rank == 1 && PlusEnvironment.Purge == false && Session.GetHabbo().Hospital == 0)
                {
                    decimal decimalCredits = Convert.ToInt32(TargetClient.GetHabbo().Credits);
                    Voler = Convert.ToInt32((decimalCredits / 100m) * 50m);
                }
                User.GetClient().Shout("*Schießt auf " + TargetClient.GetHabbo().Username + " und tötet ihn [-2% Energy]*");
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

                if (User.DuelUser != null)
                {
                    User.DuelUser = null;
                    User.DuelToken = null;
                    User.GetClient().Shout("*Gewwint den Duell*");

                }
                PlusEnvironment.GetGame().GetClientManager().sendLastActionMsg("<span class=\"blue\">" + Session.GetHabbo().Username + "</span> hat <span class=\"red\">" + TargetClient.GetHabbo().Username + "</span> getötet");
                Session.GetHabbo().insertLastAction("A tué " + TargetClient.GetHabbo().Username + " avec " + Name + ".");
                TargetUser.GetClient().SendNotification(Session.GetHabbo().Username + " hat dich getötet. Du wirst jetzt ins Krankenhaus gebracht und in 10 Minuten komplett geheilt!");
            }
            Session.GetHabbo().Chargeur = Session.GetHabbo().Chargeur - 1;
            Session.GetHabbo().EnergyFaintness(2);
        }
    }
}*/