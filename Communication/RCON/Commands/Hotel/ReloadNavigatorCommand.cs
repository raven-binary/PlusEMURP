namespace Plus.Communication.Rcon.Commands.Hotel
{
    internal class ReloadNavigatorCommand : IRconCommand
    {
        public string Description => "This command is used to reload the navigator.";

        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetNavigator().Init();

            return true;
        }
    }
}