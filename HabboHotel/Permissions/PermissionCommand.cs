namespace Plus.HabboHotel.Permissions
{
    internal class PermissionCommand
    {
        public string Command { get; }
        public int GroupId { get; }
        public int SubscriptionId { get; }

        public PermissionCommand(string command, int groupId, int subscriptionId)
        {
            Command = command;
            GroupId = groupId;
            SubscriptionId = subscriptionId;
        }
    }
}