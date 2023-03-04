using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboRoleplay.Misc;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
using Plus.HabboRoleplay.RoleplayUsers;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboRoleplay.Timer
{
    public class RoleplayTimer
    {
        /// <summary>
        /// The users roleplay instance
        /// </summary>
        RoleplayUser RP;

        int CycleCount;

        public RoleplayTimer(RoleplayUser RoleplayUser)
        {
            this.RP = RoleplayUser;
        }

        public void OnCycle()
        {
            try
            {
                CycleCount++;
                if (CycleCount == 1)
                {
                    #region Taxi
                    if (RP.usingTaxiRide && !RP.roomUser.IsWalking)
                    {
                        RP.TaxiRideTimer++;
                        if (RP.TaxiRideTimer >= 10)
                        {
                            RP.GetClient().SendWhisper("Looks likes you got stuck here, we reset you taxi ride");
                            Taxi.Reset(RP.GetClient());
                        }
                        else
                            Taxi.Route(RP.GetClient());
                    }
                    #endregion

                    #region Medkit Cooldown
                    if (RP.MedkitCooldown > 0)
                    {
                        RP.MedkitCooldown--;
                        if (RP.MedkitCooldown == 1)
                            RP.MedkitCooldown = 0;
                    }
                    #endregion
                }
                else if (CycleCount >= 2)
                {
                    CycleCount = 0;

                    #region Medkit
                    if (RP.UsingMedkit && RP.Health < RP.HealthMax)
                    {
                        RP.Health += 2;
                        if (RP.Health > RP.HealthMax)
                            RP.Health = RP.HealthMax;

                        RP.RPCache(1);
                    }
                    else if (RP.UsingMedkit)
                        RP.UsingMedkit = false;
                    #endregion

                    #region Snack
                    if (RP.UsingSnack && RP.Energy < 100)
                    {
                        RP.Energy += 3;
                        if (RP.Energy > 100)
                            RP.Energy = 100;

                        RP.RPCache(1);
                    }
                    else if (RP.UsingSnack)
                        RP.UsingSnack = false;
                    RP.roomUser.CarryItem(0);
                    #endregion

                    if (RP.Aggression > 0)
                    {
                        RP.Aggression--;
                        if (RP.Aggression == 1)
                            RP.Aggression = 0;

                        RP.RPCache(1);
                    }

                    if (RP.Prison && RP.Timer > 0)
                    {
                        RP.PrisonTimer++;
                        if (RP.PrisonTimer >= 60)
                        {
                            RP.PrisonTimer = 0;
                            RP.Timer--;
                            if (RP.Timer <= 0)
                            {
                                RP.EndPrison();
                            }
                            else
                            {
                                RP.GetClient().SendWhisper("You have " + RP.Timer + " minutes until you are released");
                                RP.UpdateValue("timer", Convert.ToString(RP.Timer));
                            }
                        }
                    }

                   



                    if (RP.roomUser.RockId > 0)
                    {
                        RP.roomUser.RockTimer++;

                        if (RP.Prison && RP.roomUser.RockTimer >= 40)
                        {
                            foreach (Item item in RP.Room.GetRoomItemHandler().GetFloor.ToList())
                                if (item.Id == RP.roomUser.RockId)
                                    RP.Room.GetRoomItemHandler().RemoveRoomItem(item);

                            RP.Timer--;
                            RP.UpdateValue("timer", Convert.ToString(RP.Timer));

                            RP.roomUser.Say("smashes the rock to pieces");
                            RP.GetClient().SendWhisper("Your prison sentence has been reduced by 1 minute - Timeleft: " + RP.Timer + " minutes");

                            if (RP.Timer <= 0)
                                RP.EndPrison();

                            foreach (RoomUser UserInRoom in RP.Habbo.CurrentRoom.GetRoomUserManager().GetUserList().ToList())
                            {
                                if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null || UserInRoom == RP.roomUser)
                                    continue;

                                if (UserInRoom.RockId == RP.roomUser.RockId)
                                {
                                    UserInRoom.Say("stops smashing the rock");
                                    UserInRoom.RockId = 0;
                                    UserInRoom.RockTimer = 0;
                                    UserInRoom.GetClient().GetRoleplay().ResetEffect();
                                }
                            }

                            RP.roomUser.RockId = 0;
                            RP.roomUser.RockTimer = 0;
                            RP.ResetEffect();
                        }
                    }
                }
            }
            catch { }
        }
    }
}
