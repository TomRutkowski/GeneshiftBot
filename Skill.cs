using System;
using System.Collections.Generic;

namespace draftbot
{
    class Skill
    {
        public string name;
        public UInt16 level;
        public bool isActive;
        public string prerequisite; //leaving as string in case of more complex prerequisites in future, unwise to use Skill
        public string description;
        public List<string> attributes;

        public bool doesSkillRequireActivesToBeDrafted()
        {
            return (this.name == "Mana Shield" || this.name == "Skill Shot") ? true : false;
        }
        
    }
}