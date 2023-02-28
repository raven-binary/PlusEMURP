namespace Plus.HabboHotel.Users.Messenger
{
    public class MessengerRequest
    {
        public MessengerRequest(int toUser, int fromUser, string username)
        {
            To = toUser;
            From = fromUser;
            Username = username;
        }

        public string Username { get; }

        public int To { get; }

        public int From { get; }
    }
}