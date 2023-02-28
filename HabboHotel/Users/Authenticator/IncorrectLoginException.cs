using System;

namespace Plus.HabboHotel.Users.Authenticator
{
    [Serializable]
    public class IncorrectLoginException : Exception
    {
        public IncorrectLoginException(string reason) : base(reason)
        {
        }
    }
}