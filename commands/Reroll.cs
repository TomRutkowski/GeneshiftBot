using System;
using Discord.WebSocket;

namespace draftbot
{
    class Reroll: Command
    {
        public string Execute(Database database, SocketUser user, string state, string param)
        {
            Draft draftCommand = new Draft();
            return draftCommand.Execute(database, user, state, "4");
        }

        public string ShortDescription()
        {
            return "Alias for #draft 4.";
        }
    }

}