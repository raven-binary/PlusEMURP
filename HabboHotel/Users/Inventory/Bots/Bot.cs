namespace Plus.HabboHotel.Users.Inventory.Bots
{
    public class Bot
    {
        public Bot(int id, int ownerId, string name, string motto, string figure, string gender)
        {
            Id = id;
            OwnerId = ownerId;
            Name = name;
            Motto = motto;
            Figure = figure;
            Gender = gender;
        }

        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Name { get; set; }

        public string Motto { get; set; }

        public string Figure { get; set; }

        public string Gender { get; set; }
    }
}