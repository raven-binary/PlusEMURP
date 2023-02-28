namespace Plus.Communication.Rcon.Commands.Hotel
{
    internal class ReloadItemsCommand : IRconCommand
    {
        public string Description => "This command is used to reload the game items.";

        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetItemManager().Init();

            return true;
        }
    }
}