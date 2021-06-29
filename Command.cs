using System.Collections.Generic;
using Discord.WebSocket;

namespace draftbot
{
    interface Command
    {
        public string Execute(Database database, SocketUser user, string state, string param);

        public string ShortDescription();
    }
}