using System;
using Discord.WebSocket;

namespace draftbot
{
    class Desc : Command
    {

        public string Execute(Database database, SocketUser user, string state, string param)
        {
            if (param == null)
            {
                return "Please write a skill you wish described.";
            }

            LocalMemoryWrapper localMemory = new LocalMemoryWrapper(database);
            Skill foundSkill = new Skill();
            Alias foundAlias = new Alias();

            if (localMemory.allSkillsDictionary.TryGetValue(param, out foundSkill))
            {
                return BuildString(param, localMemory);
            }
            else
            {
                if (localMemory.allAliasesDictionary.TryGetValue(param, out foundAlias))
                {
                    if (localMemory.allSkillsDictionary.TryGetValue(localMemory.allAliasesDictionary[param].skillName, out foundSkill))
                    {
                        return BuildString(localMemory.allAliasesDictionary[param].skillName, localMemory);
                    }
                }
            }

            return "Could not find the skill. It may be ambiguous (SS is Swift Stalker OR Skill Shot for example). Try another phrase or write the skill name fully.";
        }

        public string ShortDescription()
        {
            return "Describes a skill's description.";
        }

        public string BuildString(string skillName, LocalMemoryWrapper localMemory)
        {
            Attribute foundAttribute = new Attribute();

            String outString = "";
            outString += localMemory.allSkillsDictionary[skillName].name.ToUpper();
            outString += "\n";
            outString += localMemory.allSkillsDictionary[skillName].description;
            outString += "\n";
            foreach (string attributeString in localMemory.allSkillsDictionary[skillName].attributes)
            {
                if (attributeString != null)
                {
                    if (localMemory.allAttributesDictionary.TryGetValue(attributeString, out foundAttribute))
                    {
                        //ignores unfound attributes so typos don't just ruin this command
                        outString += foundAttribute.name;
                        outString += " : ";
                        foreach (Decimal val in foundAttribute.levelVal)
                        {
                            Console.WriteLine(val);
                            outString += val.ToString();
                            outString += "/";
                        }
                        outString = outString.Substring(0, outString.Length - 1);
                        outString += "\n";
                    }
                }
            }
            return outString;
        }

    }

}