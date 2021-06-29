using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace draftbot
{
    partial class Database
    {
        public DraftInfo GetDraft(string userId, LocalMemoryWrapper localMemory)
        {
            //skillset is here to make sure you fetch data into local memory before using it
            //will be called multiple times regularly, holding a flag for the user so that this is not recalled if no changes have been made
            DraftInfo draftInfoOut = new DraftInfo();
            draftInfoOut.draftedSkills = new HashSet<Skill>();

            Dictionary<string, Skill> draftedSkillsDictionary = new Dictionary<string, Skill>();

            SQLiteCommand getDraftCommand = new SQLiteCommand(_dbConnection);
            getDraftCommand.CommandText = "SELECT Skill, RerollCount FROM Draft WHERE UserId = '" + userId + "' ORDER BY Number ASC";
            using (SQLiteDataReader reader = getDraftCommand.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        draftInfoOut.numberOfDraftRowsInDb++;
                        if (reader.IsDBNull(reader.GetOrdinal("Skill")))
                        {
                            draftInfoOut.lastChoiceWasAReroll = true;
                            draftInfoOut.numberOfRerollsInLastChain = UInt32.Parse(reader["RerollCount"].ToString());
                            draftInfoOut.totalNoOfRerolls += draftInfoOut.numberOfRerollsInLastChain;
                        }
                        else
                        {
                            draftInfoOut.lastChoiceWasAReroll = false;
                            Skill skillValue = new Skill();
                            string currentlyReadSkill = reader["Skill"].ToString();

                            if (draftedSkillsDictionary.TryGetValue(currentlyReadSkill, out skillValue))
                            {
                                draftedSkillsDictionary[currentlyReadSkill].level++;
                                draftInfoOut.totalPoints++;
                            }
                            else
                            {
                                if (localMemory.allSkillsDictionary.TryGetValue(currentlyReadSkill, out skillValue))
                                {
                                    Skill newDraftedSkill = new Skill();
                                    newDraftedSkill.name = skillValue.name;
                                    newDraftedSkill.level = 1;
                                    newDraftedSkill.isActive = skillValue.isActive;
                                    newDraftedSkill.prerequisite = skillValue.prerequisite;
                                    draftedSkillsDictionary.Add(currentlyReadSkill, newDraftedSkill);
                                    draftInfoOut.totalPoints++;
                                }
                                else
                                {
                                    Console.WriteLine("Warning, invalid skill in database, ignoring.");
                                }
                            }
                        }
                    }

                    foreach (KeyValuePair<string, Skill> draftedSkillEntry in draftedSkillsDictionary)
                    {
                        draftInfoOut.draftedSkills.Add(draftedSkillEntry.Value);
                    }

                }
            }

            return draftInfoOut;
        }

        public void Reroll(string userId, UInt32 lastNumber, bool lastChoiceWasAReroll, UInt32 previousRerollChainNumber)
        {
            SQLiteCommand rerollCommand = new SQLiteCommand(_dbConnection);
            if (lastChoiceWasAReroll)
            {
                rerollCommand.CommandText = "UPDATE Draft SET RerollCount='" + (++previousRerollChainNumber) + "' WHERE UserId = '" + userId + "' AND Number = '" + lastNumber + "';";
            }
            else
            {
                rerollCommand.CommandText = "INSERT INTO Draft (UserId, Number, RerollCount) VALUES ('" + userId + "', '" + (++lastNumber) + "', '1')";
                Console.WriteLine(rerollCommand.CommandText);
            }
            rerollCommand.ExecuteNonQuery();
        }

        public void DraftSkill(string userId, Int64 lastNumber, string skillName)
        {
            SQLiteCommand draftSkillCommand = new SQLiteCommand(_dbConnection);
            draftSkillCommand.CommandText = "INSERT INTO Draft (UserId, Number, Skill) VALUES ('" + userId + "', '" + (++lastNumber) + "', '" + skillName + "')";
            draftSkillCommand.ExecuteNonQuery();
        }

        public void StorePreviousDraftChoices(string userId, List<Skill> draftChoices)
        {
            ClearPreviousDraftChoices(userId);

            SQLiteCommand storepdcCommand = new SQLiteCommand(_dbConnection);
            storepdcCommand.CommandText = "INSERT INTO PreviousChoices (UserId, Number, Skill, Level) VALUES ";
            foreach (Skill draftChoice in draftChoices)
            {
                storepdcCommand.CommandText += "('" + userId + "', '" + (draftChoices.IndexOf(draftChoice) + 1) + "', '" + draftChoice.name + "', '" + draftChoice.level + "'),";
            }
            storepdcCommand.CommandText = storepdcCommand.CommandText.Substring(0, storepdcCommand.CommandText.Length - 1);
            storepdcCommand.CommandText += ";";
            storepdcCommand.ExecuteNonQuery();
        }

        public List<Skill> GetPreviousDraftChoices(string userId, LocalMemoryWrapper localMemory)
        {
            SQLiteCommand getpdcCommand = new SQLiteCommand(_dbConnection);
            List<Skill> previousDraftChoices = new List<Skill>();
            getpdcCommand.CommandText = "SELECT Skill, Level FROM PreviousChoices WHERE UserId = '" + userId + "'";
            using (SQLiteDataReader reader = getpdcCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Skill draftChoice = new Skill();
                        if (localMemory.allSkillsDictionary.TryGetValue(reader["Skill"].ToString(), out draftChoice))
                        {
                            draftChoice.level = Byte.Parse(reader["Level"].ToString());
                        }
                        else
                        {
                            throw new Exception("Old skill is in the database.");
                        }
                        previousDraftChoices.Add(draftChoice);
                    }
                    return previousDraftChoices;
                }
                else
                {
                    throw new Exception("Invalid state reached.");
                }
            }
        }

        public void ClearDraft(string userId)
        {
            SQLiteCommand deleteDraftCommand = new SQLiteCommand(_dbConnection);
            deleteDraftCommand.CommandText = "DELETE FROM Draft WHERE UserId = '" + userId + "'";
            deleteDraftCommand.ExecuteNonQuery();
        }

        public void ClearPreviousDraftChoices(string userId)
        {
            SQLiteCommand deletePreviousChoicesCommand = new SQLiteCommand(_dbConnection);
            deletePreviousChoicesCommand.CommandText = "DELETE FROM PreviousChoices WHERE UserId = '" + userId + "'";
            deletePreviousChoicesCommand.ExecuteNonQuery();
        }

        public Int64 GetDraftCount(string userId)
        {
            SQLiteCommand countCommand = new SQLiteCommand(_dbConnection);
            countCommand.CommandText = "SELECT COUNT(Number) FROM Draft WHERE UserId = '" + userId + "'";
            return (Int64)countCommand.ExecuteScalar();
        }
    }

}