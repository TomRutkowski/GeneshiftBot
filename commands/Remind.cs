using System;
using Discord.WebSocket;

namespace draftbot
{
    class Remind: Command
    {
        public string Execute(Database database, SocketUser user, string state, string param)
        {
            return database.GetRemind(user.Id.ToString());
        }

        public string ShortDescription()
        {
            return "Reminds you of the last important message.";
        }
    }

}