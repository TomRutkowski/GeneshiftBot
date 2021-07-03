using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace draftbot
{
    partial class Database
    {
        private SQLiteConnection _dbConnection;

        public Database(SQLiteConnection connectionIn)
        {
            _dbConnection = connectionIn;
        }

        public HashSet<Skill> GetAllSkills()
        {
            //should only be called once per program lifespan and cached
            HashSet<Skill> allSkillsSet = new HashSet<Skill>();

            SQLiteCommand getAllSkillsCommand = _dbConnection.CreateCommand(); 

            getAllSkillsCommand.CommandText = "SELECT Name, Active, Prerequisite, Description, Attribute1, Attribute2, Attribute3, Attribute4 FROM Skill";
            using (SQLiteDataReader reader = getAllSkillsCommand.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Skill currentlyReadSkill = new Skill();
                        currentlyReadSkill.attributes = new List<string>();

                        currentlyReadSkill.name = reader["Name"].ToString();
                        currentlyReadSkill.isActive = bool.Parse(reader["Active"].ToString());
                        currentlyReadSkill.prerequisite = reader.IsDBNull(reader.GetOrdinal("Prerequisite")) ? null : reader["Prerequisite"].ToString();
                        currentlyReadSkill.description = reader["Description"].ToString();
                        for (int i = 1; i < 5; i++)
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("Attribute" + i)))
                            {
                                currentlyReadSkill.attributes.Add(reader["Attribute" + i].ToString());
                            }
                        }

                        allSkillsSet.Add(currentlyReadSkill);
                    }
                }

            }

            if (allSkillsSet.Count == 0)
            {
                throw new Exception("There should be skills in the database.");
            }

            return allSkillsSet;
        }

        public HashSet<Attribute> GetAllAttributes()
        {
            //should only be called once per program lifespan and cached
            HashSet<Attribute> allAttributesSet = new HashSet<Attribute>();

            SQLiteCommand getAllAttributesCommand = _dbConnection.CreateCommand();

            getAllAttributesCommand.CommandText = "SELECT Name, Level1, Level2, Level3, Level4 FROM Attribute";

            using (SQLiteDataReader reader = getAllAttributesCommand.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Attribute currentlyReadAttribute = new Attribute();
                        currentlyReadAttribute.levelVal = new List<decimal>();

                        currentlyReadAttribute.name = reader["Name"].ToString();

                        for (int i = 1; i < 5; i++)
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("Level" + i)))
                            {
                                currentlyReadAttribute.levelVal.Add(Decimal.Parse(reader["Level" + i].ToString()));
                            }
                        }

                        allAttributesSet.Add(currentlyReadAttribute);
                    }
                }

            }

            if (allAttributesSet.Count == 0)
            {
                throw new Exception("There should be attributes in the database.");
            }

            return allAttributesSet;
        }

        public HashSet<Alias> GetAllAliases()
        {
            //should only be called once per program lifespan and cached
            HashSet<Alias> allAliasSet = new HashSet<Alias>();

            SQLiteCommand getAllAliasesCommand = _dbConnection.CreateCommand();

            getAllAliasesCommand.CommandText = "SELECT Alias, Skill FROM Alias";

            using (SQLiteDataReader reader = getAllAliasesCommand.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Alias currentlyReadAlias = new Alias();
                        currentlyReadAlias.aliasName = reader["Alias"].ToString();
                        currentlyReadAlias.skillName = reader["Skill"].ToString();
                        allAliasSet.Add(currentlyReadAlias);
                    }
                }

            }

            return allAliasSet;
        }

    }

}