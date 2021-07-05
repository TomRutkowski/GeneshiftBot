using System;
using System.Collections.Generic;

namespace draftbot
{
    class LocalMemoryWrapper
    {
        private static HashSet<Skill> _allSkills;
        private static HashSet<Attribute> _allAttributes;
        private static HashSet<SkillAlias> _allSkillAliases;
        private static HashSet<Weapon> _allWeapons;
        private static HashSet<WeaponAlias> _allWeaponAliases;

        private static Dictionary<string, Skill> _allSkillsDictionary; //use to reference specific skill entries by name
        private static Dictionary<string, Attribute> _allAttributesDictionary;
        private static Dictionary<string, SkillAlias> _allSkillAliasesDictionary;
        private static Dictionary<string, Weapon> _allWeaponsDictionary;
        private static Dictionary<string, WeaponAlias> _allWeaponAliasesDictionary;

        private static object _allSkillsConcurrencyGuard = new object();
        private static object _allAttributesConcurrencyGuard = new object();
        private static object _allSkillAliasesConcurrencyGuard = new object();
        private static object _allWeaponsConcurrencyGuard = new object();
        private static object _allWeaponAliasesConcurrencyGuard = new object();
        private static object _skillsDictionaryConcurrencyGuard = new object();
        private static object _attrsDictionaryConcurrencyGuard = new object();
        private static object _skillAliasesDictionaryConcurrencyGuard = new object();
        private static object _weaponsDictionaryConcurrencyGuard = new object();
        private static object _weaponAliasesDictionaryConcurrencyGuard = new object();

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

        public Dictionary<string, SkillAlias> allSkillAliasesDictionary
        {
            get
            {
                lock(_skillAliasesDictionaryConcurrencyGuard)
                {
                    if(_allSkillAliasesDictionary == null)
                    {
                        _allSkillAliasesDictionary = new Dictionary<string, SkillAlias>(StringComparer.OrdinalIgnoreCase);
                        foreach (SkillAlias skillAlias in allSkillAliases)
                        {
                            _allSkillAliasesDictionary.Add(skillAlias.aliasName, skillAlias);
                        }
                    }
                }
                return _allSkillAliasesDictionary;
            }
        }

        public HashSet<SkillAlias> allSkillAliases
        {
            get
            {
                lock(_allSkillAliasesConcurrencyGuard)
                {
                    if(_allSkillAliases == null)
                    {
                        _allSkillAliases = _database.GetAllSkillAliases();
                    }
                }
                return _allSkillAliases;
            }
        }

        public Dictionary<string, Weapon> allWeaponsDictionary
        {
            get
            {
                lock(_weaponsDictionaryConcurrencyGuard)
                {
                    if(_allWeaponsDictionary == null)
                    {
                        _allWeaponsDictionary = new Dictionary<string, Weapon>(StringComparer.OrdinalIgnoreCase);
                        foreach (Weapon weapon in allWeapons)
                        {
                            _allWeaponsDictionary.Add(weapon.name, weapon);
                        }
                    }
                }
                return _allWeaponsDictionary;
            }
        }

        public HashSet<Weapon> allWeapons
        {
            get
            {
                lock(_allWeaponsConcurrencyGuard)
                {
                    if(_allWeapons == null)
                    {
                        _allWeapons = _database.GetAllWeapons();
                    }
                }
                return _allWeapons;
            }
        }

        public Dictionary<string, WeaponAlias> allWeaponAliasesDictionary
        {
            get
            {
                lock(_weaponAliasesDictionaryConcurrencyGuard)
                {
                    if(_allWeaponAliasesDictionary == null)
                    {
                        _allWeaponAliasesDictionary = new Dictionary<string, WeaponAlias>(StringComparer.OrdinalIgnoreCase);
                        foreach (WeaponAlias weaponAlias in allWeaponAliases)
                        {
                            _allWeaponAliasesDictionary.Add(weaponAlias.aliasName, weaponAlias);
                        }
                    }
                }
                return _allWeaponAliasesDictionary;
            }
        }

        public HashSet<WeaponAlias> allWeaponAliases
        {
            get
            {
                lock(_allWeaponAliasesConcurrencyGuard)
                {
                    if(_allWeaponAliases == null)
                    {
                        _allWeaponAliases = _database.GetAllWeaponAliases();
                    }
                }
                return _allWeaponAliases;
            }
        }

    }
}