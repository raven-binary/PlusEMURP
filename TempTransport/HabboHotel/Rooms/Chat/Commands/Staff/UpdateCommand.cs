using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.GameClients;


using Plus.HabboHotel.Moderation;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Catalog;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class UpdateCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }
        public string Parameters
        {
            get { return "%variable%"; }
        }

        public string Description
        {
            get { return "Reload a specific part of the hotel."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("You must inculde a thing to update, e.g. :update catalog");
                return;
            }

            string UpdateVariable = Params[1];
            switch (UpdateVariable.ToLower())
            {
                case "cata":
                case "catalog":
                case "catalogue":
                    {
                        if (Session.GetHabbo().Rank != 8)
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_catalog' permission.");
                            break;
                        }

                        PlusEnvironment.GetGame().GetCatalog().Init(PlusEnvironment.GetGame().GetItemManager());
                        PlusEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        Session.SendWhisper("Catalogue successfully updated.");
                        break;
                    }

                case "items":
                case "furni":
                case "furniture":
                    {
                        if (Session.GetHabbo().Rank != 8)
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_furni' permission.");
                            break;
                        }

                        PlusEnvironment.GetGame().GetItemManager().Init();
                        Session.SendWhisper("Items successfully updated.");
                        break;
                    }

                case "models":
                    {
                        if (Session.GetHabbo().Rank != 8)
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_models' permission.");
                            break;
                        }

                        PlusEnvironment.GetGame().GetRoomManager().LoadModels();
                        Session.SendWhisper("Room models successfully updated.");
                        break;
                    }

               

                case "ranks":
                case "rights":
                case "permissions":
                    {
                        if (Session.GetHabbo().Rank != 8)
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_rights' permission.");
                            break;
                        }

                        PlusEnvironment.GetGame().GetPermissionManager().Init();

                        foreach (GameClient Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
                        {
                            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().GetPermissions() == null)
                                continue;

                            Client.GetHabbo().GetPermissions().Init(Client.GetHabbo());
                        }

                        Session.SendWhisper("Rank definitions successfully updated.");
                        break;
                    }

                

                case "bans":
                    {
                        if (Session.GetHabbo().Rank != 8)
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_bans' permission.");
                            break;
                        }

                        PlusEnvironment.GetGame().GetModerationManager().ReCacheBans();
                        Session.SendWhisper("Ban cache re-loaded.");
                        break;
                    }

               


                default:
                    Session.SendWhisper("'" + UpdateVariable + "' is not a valid thing to reload.");
                    break;
            }
        }
    }
}