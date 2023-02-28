namespace Plus.HabboHotel.Catalog.Pets
{
    public class PetRace
    {
        public PetRace(int raceId, int primaryColour, int secondaryColour, bool hasPrimaryColour, bool hasSecondaryColour)
        {
            RaceId = raceId;
            PrimaryColour = primaryColour;
            SecondaryColour = secondaryColour;
            HasPrimaryColour = hasPrimaryColour;
            HasSecondaryColour = hasSecondaryColour;
        }

        public int RaceId { get; set; }

        public int PrimaryColour { get; set; }

        public int SecondaryColour { get; set; }

        public bool HasPrimaryColour { get; set; }

        public bool HasSecondaryColour { get; set; }
    }
}