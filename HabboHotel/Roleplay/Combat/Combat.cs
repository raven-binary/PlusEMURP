using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.HabboHotel.Rooms.PathFinding;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Zstandard.Net.ZstandardInterop;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;
using MySqlX.XDevAPI;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Utilities;
using static System.Collections.Specialized.BitVector32;

namespace Plus.HabboHotel.Roleplay.Combat
{
    public class Combat
    {
        public Habbo Habbo { get; set; }

        public Combat(Habbo habbo)
        {
            this.Habbo = habbo;
        }

        public void Hit(RoomUser roomUser, RoomUser targetRoomUser, GameClient targetClient)
        {
            try
            {
                #region Null checks and shit

                if (roomUser == null || targetRoomUser == null)
                    return;

                Room room = Habbo.CurrentRoom;
                if (room == null || targetClient?.GetHabbo() == null)
                    return;

                if (targetClient.GetHabbo().Health <= 0)
                    return;

                #endregion

                // Randomize damage + hit chance
                int value = 0;
                if (Habbo.GetStats().CombatLevel == 2)
                    value = RandomNumber.GenerateNewRandom(-1, 4);
                else if (Habbo.GetStats().CombatLevel == 3)
                    value = RandomNumber.GenerateNewRandom(-1, 5);
                else if (Habbo.GetStats().CombatLevel == 4)
                    value = RandomNumber.GenerateNewRandom(-1, 6);
                else if (Habbo.GetStats().CombatLevel >= 5)
                    value = RandomNumber.GenerateNewRandom(-1, 7);
                else
                {
                    value = RandomNumber.GenerateNewRandom(-1, 3);
                }

                // Check for health
                int health = this.CheckHealth(targetClient.GetHabbo().Health - value);
                targetClient.GetHabbo().Health = health;

                /*
                 * if bodyguard / bot
                 * if hit bot
                 * if passive mode?
                 * if too far
                 * if farming
                 * if working
                 * if dead/healing
                 * if combat disabled
                 */

                if (value <= 0)
                {
                    room.SendPacket(new ChatComposer(roomUser.VirtualId,
                        "*swings at " + targetClient.GetHabbo().Username + ", but misses*", 0,
                        roomUser.LastBubble));
                } else {
                    room.SendPacket(new ChatComposer(roomUser.VirtualId,
                        "*hits " + targetClient.GetHabbo().Username + " causing " + value + " damage*", 0,
                        roomUser.LastBubble));

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `users` SET `health` = " + health + " WHERE `id` = '" +
                                          targetClient.GetHabbo().Id + "'");
                        dbClient.RunQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exception that may occur
                Console.WriteLine($"An error occurred in Hit method: {ex.Message}");
            }
        }

        private int CheckHealth(int value)
        {
            if (value < 0)
                return 0;

            if (value > 100)
                return 100;

            return value;
        }
    }
}