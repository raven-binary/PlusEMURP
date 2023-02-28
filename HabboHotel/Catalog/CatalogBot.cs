namespace Plus.HabboHotel.Catalog
{
    public class CatalogBot
    {
        public int Id { get; }
        public string Figure { get; }
        public string Gender { get; }
        public string Motto { get; }
        public string Name { get; }
        public string AIType { get; }

        public CatalogBot(int id, string name, string figure, string motto, string gender, string type)
        {
            Id = id;
            Name = name;
            Figure = figure;
            Motto = motto;
            Gender = gender;
            AIType = type;
        }
    }
}