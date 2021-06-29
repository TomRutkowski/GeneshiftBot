using System;
using System.Collections.Generic;

namespace draftbot
{
    class LocalMemoryWrapper
    {
        private static HashSet<Skill> _allSkills;
        private static HashSet<Attribute> _allAttributes;
        private static HashSet<Alias> _allAliases;
        private static Dictionary<string, Skill> _allSkillsDictionary; //use to reference specific skill entries by name
        private static Dictionary<string, Attribute> _allAttributesDictionary;
        private static Dictionary<string, Alias> _allAliasesDictionary;
        private static object _allSkillsConcurrencyGuard = new object();
        private static object _allAttributesConcurrencyGuard = new object();
        private static object _allAliasesConcurrencyGuard = new object();
        private static object _skillsDictionaryConcurrencyGuard = new object();
        private static object _attrsDictionaryConcurrencyGuard = new object();
        private static object _aliasesDictionaryConcurrencyGuard = new object();

        private Database _database;

        public LocalMemoryWrapper(Database dbIn)
        {
            _database = dbIn;
        }

        public Dictionary<string, Skill> allSkillsDictionary
        {
            get
            {
                lock(_skillsDictionaryConcurrencyGuard)
                {
                    if(_allSkillsDictionary == null)
                    {
                        _allSkillsDictionary = new Dictionary<string, Skill>(StringComparer.OrdinalIgnoreCase);
                        foreach (Skill skill in allSkills)
                        {
                            _allSkillsDictionary.Add(skill.name, skill);
                        }
                    }
                }
                return _allSkillsDictionary;
            }
        }

        public HashSet<Skill> allSkills
        {
            get
            {
                lock(_allSkillsConcurrencyGuard)
                {
                    if(_allSkills == null)
                    {
                        _allSkills = _database.GetAllSkills();
                    }
                }
                return _allSkills;
            }
        }

        public Dictionary<string, Attribute> allAttributesDictionary
        {
            get
            {
                lock(_attrsDictionaryConcurrencyGuard)
                {
                    if(_allAttributesDictionary == null)
                    {
                        _allAttributesDictionary = new Dictionary<string, Attribute>(StringComparer.OrdinalIgnoreCase);
                        foreach (Attribute attr in allAttributes)
                        {
                            _allAttributesDictionary.Add(attr.name, attr);
                        }
                    }
                }
                return _allAttributesDictionary;
            }
        }

        public HashSet<Attribute> allAttributes
        {
            get
            {
                lock(_allAttributesConcurrencyGuard)
                {
                    if(_allAttributes == null)
                    {
                        _allAttributes = _database.GetAllAttributes();
                    }
                }
                return _allAttributes;
            }
        }

        public Dictionary<string, Alias> allAliasesDictionary
        {
            get
            {
                lock(_aliasesDictionaryConcurrencyGuard)
                {
                    if(_allAliasesDictionary == null)
                    {
                        _allAliasesDictionary = new Dictionary<string, Alias>(StringComparer.OrdinalIgnoreCase);
                        foreach (Alias alias in allAliases)
                        {
                            _allAliasesDictionary.Add(alias.aliasName, alias);
                        }
                    }
                }
                return _allAliasesDictionary;
            }
        }

        public HashSet<Alias> allAliases
        {
            get
            {
                lock(_allAliasesConcurrencyGuard)
                {
                    if(_allAliases == null)
                    {
                        _allAliases = _database.GetAllAliases();
                    }
                }
                return _allAliases;
            }
        }

    }
}