using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Threading;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class MedkitCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Use a Medkit"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Session.GetHabbo().Stunned)
            {
                Session.SendWhisper("You can not perform this action while stunned");
                return;
            }

            if (Session.GetHabbo().Hospital == 1 || Session.GetRoleplay().Dead)
            {
                Session.SendWhisper("You can not perform this action while dead");
                return;
            }

            if (Session.GetHabbo().isBleeding)
            {
                Session.SendWhisper("You can not use a medkit while bleeding");
                return;
            }

            if (Params.Length == 1)
            {
                if (Session.GetHabbo().InventorySlot1 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot1");
                }
                else if (Session.GetHabbo().InventorySlot2 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot2");
                }
                else if (Session.GetHabbo().InventorySlot3 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot3");
                }
                else if (Session.GetHabbo().InventorySlot4 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot4");
                }
                else if (Session.GetHabbo().InventorySlot5 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot5");
                }
                else if (Session.GetHabbo().InventorySlot6 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot6");
                }
                else if (Session.GetHabbo().InventorySlot7 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot7");
                }
                else if (Session.GetHabbo().InventorySlot8 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot8");
                }
                else if (Session.GetHabbo().InventorySlot9 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot9");
                }
                else if (Session.GetHabbo().InventorySlot10 == "medkit")
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "medkit,slot10");
                }
                else
                {
                    Session.SendWhisper("You don't have any medkit");
                    return;
                }
            }
            else
            {
                string Slot = string.Empty;
                if (Session.GetHabbo().InventorySlot1 == "medkit")
                {
                    Slot = "slot1";
                }
                else if (Session.GetHabbo().InventorySlot2 == "medkit")
                {
                    Slot = "slot2";
                }
                else if (Session.GetHabbo().InventorySlot3 == "medkit")
                {
                    Slot = "slot3";
                }
                else if (Session.GetHabbo().InventorySlot4 == "medkit")
                {
                    Slot = "slot4";
                }
                else if (Session.GetHabbo().InventorySlot5 == "medkit")
                {
                    Slot = "slot5";
                }
                else if (Session.GetHabbo().InventorySlot6 == "medkit")
                {
                    Slot = "slot6";
                }
                else if (Session.GetHabbo().InventorySlot7 == "medkit")
                {
                    Slot = "slot7";
                }
                else if (Session.GetHabbo().InventorySlot8 == "medkit")
                {
                    Slot = "slot8";
                }
                else if (Session.GetHabbo().InventorySlot9 == "medkit")
                {
                    Slot = "slot9";
                }
                else if (Session.GetHabbo().InventorySlot10 == "medkit")
                {
                    Slot = "slot10";
                }
                else
                {
                    Session.SendWhisper("You don't have any medkit");
                    return;
                }

                GameClient Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (Target == null)
                {
                    Session.SendWhisper(Params[1] + " could not be found");
                    return;
                }

                if (Session.GetHabbo().getCooldown("medkit") == true)
                {
                    Session.SendWhisper("Please wait " + Session.GetHabbo().MedkitCooldownTimer + " seconds before using another medkit");
                    return;
                }

                if (Target.GetRoleplay().Health == Target.GetRoleplay().HealthMax)
                {
                    Target.SendWhisper(Target.GetHabbo().Username + " don't need a medkit, their health is already replenished!");
                    return;
                }

                if (Target.GetHabbo().Hospital > 0 || Target.GetHabbo().IsWaitingForParamedic)
                    return;

                User.Say("uses medkit on " + Target.GetHabbo().Username);
                Target.GetHabbo().UsingMedkit = true;
                Target.GetRoleplay().Health += 25;
                Session.GetHabbo().addCooldown("medkit", 180000);
                Session.GetHabbo().MedkitCooldownTimer = 180;

                System.Timers.Timer MedkitCooldownTimer = new System.Timers.Timer(500);
                MedkitCooldownTimer.Interval = 500;
                MedkitCooldownTimer.Elapsed += delegate
                {
                    if (Session.GetHabbo().MedkitCooldownTimer > 0)
                    {
                        Session.GetHabbo().MedkitCooldownTimer -= 1;
                        MedkitCooldownTimer.Start();
                    }
                    else
                    {
                        MedkitCooldownTimer.Stop();
                    }
                };
                MedkitCooldownTimer.Start();

                if (Target.GetRoleplay().Health > Target.GetRoleplay().HealthMax)
                {
                    Target.GetRoleplay().Health = Target.GetRoleplay().HealthMax;
                    Target.GetHabbo().RPCache(1);
                    return;
                }
                Target.GetHabbo().RPCache(1);

                System.Timers.Timer MedkitTimer = new System.Timers.Timer(500);
                MedkitTimer.Interval = 500;
                MedkitTimer.Elapsed += delegate
                {
                    if (Target.GetHabbo().UsingMedkit)
                    {
                        if (Target.GetRoleplay().Health == Target.GetRoleplay().HealthMax || Target.GetRoleplay().Health > Target.GetRoleplay().HealthMax)
                        {
                            Target.GetRoleplay().Health = Target.GetRoleplay().HealthMax;
                            Target.GetHabbo().RPCache(1);
                            Target.GetHabbo().UsingMedkit = false;
                            MedkitTimer.Stop();
                        }
                        else
                        {
                            Target.GetRoleplay().Health += 3;
                            if (Target.GetRoleplay().Health == Target.GetRoleplay().HealthMax || Target.GetRoleplay().Health > Target.GetRoleplay().HealthMax)
                            {
                                Target.GetRoleplay().Health = Target.GetRoleplay().HealthMax;
                                Target.GetHabbo().RPCache(1);
                                Target.GetHabbo().UsingMedkit = false;
                                MedkitTimer.Stop();
                                return;
                            }
                            Target.GetHabbo().RPCache(1);
                            MedkitTimer.Start();
                        }
                    }
                    else
                    {
                        MedkitTimer.Stop();
                    }
                };
                MedkitTimer.Start();

                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "inventory", "update_quantity," + Slot + ",-,1");
            }
        }
    }
}