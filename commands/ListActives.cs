using System;
using System.Collections.Generic;
using Discord.WebSocket;

namespace draftbot
{
    class ListActives: Command
    {
        public string Execute(Database database, SocketUser user, string state, string param)
        {
            string outputString = "";
            LocalMemoryWrapper skillSetReferences = new LocalMemoryWrapper(database);
            foreach (Skill skillEntry in skillSetReferences.allSkills)
            {
                if (skillEntry.isActive == true)
                {
                    outputString = outputString + skillEntry.name + ", ";
                }
            }
            return outputString.Substring(0, outputString.Length - 2);
        }

        public string ShortDescription()
        {
            return "Lists all active skills.";
        }
    }

}