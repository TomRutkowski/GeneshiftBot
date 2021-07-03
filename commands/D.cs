using System;
using Discord.WebSocket;

namespace draftbot
{
    class D: Command
    {
        public string Execute(Database database, SocketUser user, string state, string param)
        {
            Draft draftCommand = new Draft();
            return draftCommand.Execute(database, user, state, param);
        }

        public string ShortDescription()
        {
            return "Alias for #draft x.";
        }
    }

}