namespace Plus.Communication.Rcon.Commands.Hotel
{
    internal class ReloadFilterCommand : IRconCommand
    {
        public string Description => "This command is used to reload the chatting filter manager.";

        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetChatManager().GetFilter().Init();
            return true;
        }
    }
}