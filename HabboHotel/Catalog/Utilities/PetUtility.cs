﻿using System;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.AI;

namespace Plus.HabboHotel.Catalog.Utilities
{
    public static class PetUtility
    {
        public static bool CheckPetName(string name)
        {
            if (name.Length < 1 || name.Length > 16)
                return false;

            return PlusEnvironment.IsValidAlphaNumeric(name);
        }

        public static Pet CreatePet(int userId, string name, int type, string race, string colour)
        {
            Pet pet = new(0, userId, 0, name, type, race, colour, 0, 100, 100, 0, PlusEnvironment.GetUnixTimestamp(), 0, 0, 0.0, 0, 0, 0, -1, "-1");

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO bots (user_id,name, ai_type) VALUES (" + pet.OwnerId + ",@" + pet.PetId + "name, 'pet')");
                dbClient.AddParameter(pet.PetId + "name", pet.Name);
                pet.PetId = Convert.ToInt32(dbClient.InsertQuery());

                dbClient.SetQuery("INSERT INTO bots_petdata (id,type,race,color,experience,energy,createstamp) VALUES (" + pet.PetId + ", " + pet.Type + ",@" + pet.PetId + "race,@" + pet.PetId + "color,0,100,UNIX_TIMESTAMP())");
                dbClient.AddParameter(pet.PetId + "race", pet.Race);
                dbClient.AddParameter(pet.PetId + "color", pet.Color);
                dbClient.RunQuery();
            }

            return pet;
        }
    }
}