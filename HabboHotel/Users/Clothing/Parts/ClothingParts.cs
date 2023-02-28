namespace Plus.HabboHotel.Users.Clothing.Parts
{
    public sealed class ClothingParts
    {
        public ClothingParts(int id, int partId, string part)
        {
            Id = id;
            PartId = partId;
            Part = part;
        }

        public int Id { get; set; }

        public int PartId { get; set; }

        public string Part { get; set; }
    }
}