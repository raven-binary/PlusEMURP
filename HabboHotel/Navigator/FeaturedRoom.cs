namespace Plus.HabboHotel.Navigator
{
    public class FeaturedRoom
    {
        public int RoomId { get; }
        public string Caption { get; }
        public string Description { get; }
        public string Image { get; }

        public FeaturedRoom(int roomId, string caption, string description, string images)
        {
            RoomId = roomId;
            Caption = caption;
            Description = description;
            Image = images;
        }
    }
}