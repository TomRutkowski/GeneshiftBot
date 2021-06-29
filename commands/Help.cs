using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.WebSocket;

namespace draftbot
{
    class Help: Command
    {
        private static string _helpString;

        private static object _helpConcurrencyGuard = new object();

        public string Execute(Database database, SocketUser user, string state, string param)
        {
            lock(_helpConcurrencyGuard)
            {
                if (_helpString == null)
                {
                    _helpString = "Command List: \n\n";
                    foreach (Type t in Assembly.GetExecutingAssembly().GetTypes()) 
                    {
                        if (t.GetInterface("Command") != null)
                        {
                            Command command = (Command)Activator.CreateInstance(t);
                            _helpString += "#" + t.Name.ToLower() + " --- " + command.ShortDescription() + "\n";
                        }
                    }
                }
                return _helpString;
            }
        }

        public string ShortDescription()
        {
            return "Lists commands with a short description.";
        }
    }

}