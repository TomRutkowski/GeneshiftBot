using System;
using Discord.WebSocket;

namespace draftbot
{
    class W: Command
    {
        public string Execute(Database database, SocketUser user, string state, string param)
        {
            DescWeapon descCommand = new DescWeapon();
            return descCommand.Execute(database, user, state, param);
        }

        public string ShortDescription()
        {
            return "Alias for #DescWeapon x.";
        }
    }

}