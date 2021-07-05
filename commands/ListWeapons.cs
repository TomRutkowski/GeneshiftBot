using System.Collections.Generic;
using Discord.WebSocket;

namespace draftbot
{
    class ListWeapons: Command
    {
        public string Execute(Database database, SocketUser user, string state, string param)
        {
            string outputString = "";
            LocalMemoryWrapper weaponSetReferences = new LocalMemoryWrapper(database);
            foreach (Weapon weaponEntry in weaponSetReferences.allWeapons)
            {
                    outputString = outputString + weaponEntry.name + ", ";
            }
            return outputString.Substring(0, outputString.Length - 2);
        }

        public string ShortDescription()
        {
            return "Lists all weapons.";
        }
    }

}