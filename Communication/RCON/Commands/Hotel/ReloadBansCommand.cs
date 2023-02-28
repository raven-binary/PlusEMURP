namespace Plus.Communication.Rcon.Commands.Hotel
{
    internal class ReloadBansCommand : IRconCommand
    {
        public string Description => "This command is used to re-cache the bans.";

        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetModerationManager().ReCacheBans();

            return true;
        }
    }
}