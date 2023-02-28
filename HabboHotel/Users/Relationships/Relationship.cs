namespace Plus.HabboHotel.Users.Relationships
{
    public class Relationship
    {
        public int Id;
        public int Type;
        public int UserId;

        public Relationship(int id, int user, int type)
        {
            Id = id;
            UserId = user;
            Type = type;
        }
    }
}