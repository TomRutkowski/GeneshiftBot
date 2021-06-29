using System;
using Discord.WebSocket;

namespace draftbot
{
    class S: Command
    {
        public string Execute(Database database, SocketUser user, string state, string param)
        {
            Desc descCommand = new Desc();
            return descCommand.Execute(database, user, state, param);
        }

        public string ShortDescription()
        {
            return "Alias for $desc x.";
        }
    }

}