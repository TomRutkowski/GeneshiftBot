using System;
using System.Collections.Generic;
using Discord.WebSocket;

namespace draftbot
{
    class Draft: Command
    {

        public static List<Skill> DraftAlgorithm(HashSet<Skill> currentDraft, Database database, List<Skill> previousChoices)
        {
            LocalMemoryWrapper gameSkills = new LocalMemoryWrapper(database);
            Dictionary<string, Skill> draftedSkillDictionary = new Dictionary<string, Skill>(); //allows skills to be found by key more easily
            Dictionary<string, Skill> previousChoiceDictionary = new Dictionary<string, Skill>();
            Random rng = new Random();

            UInt32 totalPointsSpent = 0;
            byte totalActivesDrafted = 0;

            if (currentDraft == null)
            {
                currentDraft = new HashSet<Skill>();
            }

            foreach (Skill draftedSkill in currentDraft)
            {
                totalPointsSpent += draftedSkill.level;
                totalActivesDrafted += draftedSkill.isActive ? (Byte)1 : (Byte)0;
                draftedSkillDictionary.Add(draftedSkill.name , draftedSkill);
            }

            foreach (Skill previousChoice in previousChoices)
            {
                previousChoiceDictionary.Add(previousChoice.name, previousChoice);
            }

            float maxUniqueSkills = 10; //the maximum amount of unique skills you can have in your current draft before new skills are penalised
            float tooHighSkillLevel = 4; //the maximum skill level of a draft offering at a given point allocation

            switch (totalPointsSpent)
            {
                case 0:
                case 1:
                    maxUniqueSkills = 3;
                    tooHighSkillLevel = 1;
                    break;
                case 2:
                case 3:
                case 4:
                    maxUniqueSkills = 4;
                    tooHighSkillLevel = 2;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    maxUniqueSkills = 5;
                    tooHighSkillLevel = 3;
                    break;
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    maxUniqueSkills = 6;
                    tooHighSkillLevel = 4;
                    break;
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                    maxUniqueSkills = 7;
                    tooHighSkillLevel = 4;
                    break;
            }

            byte activesInDraftSlots = 0;
            List<Skill> draftChoices = new List<Skill>(); //ordered choice of what skills show up in the draft, slot matters in algorithm
            Dictionary<string, Skill> draftChoicesDictionary = new Dictionary<string, Skill>();
            
            for (byte draftIndex = 0; draftIndex < 3; draftIndex++)
            {
                List<SkillWeight> skillPreferencesForThisSlot = new List<SkillWeight>();

                foreach (Skill skill in gameSkills.allSkills)
                {

                    float previouslyDraftedPenalty = 1.0f; //penalty for having been offered this in the previous set of choices, whether rerolling or not.
                    float newActivePenalty = 1.0f; //penalty for being offered new actives if you already have one
                    float highLevelPenalty = 1.0f; //penalty for a skill that's already levelled high
			        float tooManySkillsPenalty = 1.0f; //penalty for having too many skills at a given range

                    Skill skillValue = new Skill();

                    if (draftChoicesDictionary.TryGetValue(skill.name, out skillValue))
                    {
                        continue; //the same skill will never appear multiple times in a choice group
                    }

                    if (totalActivesDrafted == 0)
                    {
                        if (skill.doesSkillRequireActivesToBeDrafted())
                        {
                            continue; //skills that require actives to be drafted do not appear if you don't have an active
                        }
                    }

                    if (skill.prerequisite != null)
                    {
                        if (!draftedSkillDictionary.TryGetValue(skill.prerequisite, out skillValue))
                        {
                            continue; //skill must have its prerequisite to be drafted
                        }
                    }

                    if (draftedSkillDictionary.TryGetValue(skill.name, out skillValue))
                    {
                        if (draftedSkillDictionary[skill.name].level == 0)
                        {
                            throw new Exception("A drafted skill should have a level.");
                        }

                        if (draftedSkillDictionary[skill.name].level == 4)
                        {
                            continue; //skills cannot go above level 4
                        }
                        else if (draftedSkillDictionary[skill.name].level >= tooHighSkillLevel)
                        {
                            highLevelPenalty = 50.0f; //skills that are outside of the penalty range
                        }
                        else
                        {
                            highLevelPenalty = 1 + ((float)draftedSkillDictionary[skill.name].level)/tooHighSkillLevel;
                        }

                        skill.level = (UInt16)(draftedSkillDictionary[skill.name].level + 1); //set the level for the choice;
                    }
                    else
                    {
                        if (totalActivesDrafted == 3 && skill.isActive)
                        {
                            continue; //you cannot draft active skills if you already have three.
                        }

                        tooManySkillsPenalty = 2.0f; //natural penalty for new skills

                        if (currentDraft.Count >= maxUniqueSkills)
                        {
                            //5x, 20x, 45x, 80x penalty for new skill when at skill cap
                            tooManySkillsPenalty = 1 + currentDraft.Count - maxUniqueSkills;
					        tooManySkillsPenalty *= tooManySkillsPenalty;
					        tooManySkillsPenalty *= 5;
                        }

                        if (skill.isActive == true)
                        {
                            if(activesInDraftSlots >=2)
                            {
                                newActivePenalty = 200.0f;
                            }
                            else if (activesInDraftSlots == 1)
                            {
                                newActivePenalty = 1.3f;
                            }
                        }

                        skill.level = 1; //this is a new skill
                    }

                    if (previousChoiceDictionary.TryGetValue(skill.name, out skillValue))
                    {
                        previouslyDraftedPenalty = 4.0f;
                    }

                    int randVal = rng.Next(0, 10000);
                    float randScore = 0.1f + 0.9f*(randVal/10000.0f);
                    randScore *= previouslyDraftedPenalty;
                    randScore *= newActivePenalty;
                    randScore *= highLevelPenalty;
                    randScore *= tooManySkillsPenalty;
                    skillPreferencesForThisSlot.Add(new SkillWeight(skill, randScore));

                }

                skillPreferencesForThisSlot.Sort();

                /*
                foreach(SkillWeight sw in skillPreferencesForThisSlot)
                {
                    Console.WriteLine(sw.skill.name + " : " + sw.weight);
                }
                */

                draftChoices.Add(skillPreferencesForThisSlot[0].skill);
                draftChoicesDictionary.Add(skillPreferencesForThisSlot[0].skill.name, skillPreferencesForThisSlot[0].skill);

                if (skillPreferencesForThisSlot[0].skill.isActive)
                {
                    activesInDraftSlots++;
                }
            }

            return draftChoices;
        }

        public string Execute(Database database, SocketUser user, string state, string param)
        {
            string outString = "";

            if (state == "Start")
            {
                List<Skill> draftChoices = DraftAlgorithm(new HashSet<Skill>(), database, new List<Skill>());
                database.StorePreviousDraftChoices(user.Id.ToString(), draftChoices);
                database.SetState(user.Id.ToString(), "Draft");
                outString += BuildCurrentString(new HashSet<Skill>(), 0, 1);
                outString += BuildDraftString(draftChoices);
                database.SetRemind(user.Id.ToString(), outString);
                return outString;
            }
            
            if (state == "Draft")
            {
                LocalMemoryWrapper localMemory = new LocalMemoryWrapper(database);
                if (param == null)
                {
                    DraftInfo draftInfo = database.GetDraft(user.Id.ToString(), localMemory);
                    outString += BuildCurrentString(draftInfo.draftedSkills, draftInfo.totalNoOfRerolls, draftInfo.totalPoints + 1);
                    List<Skill> previousDraftChoices = database.GetPreviousDraftChoices(user.Id.ToString(), new LocalMemoryWrapper(database));
                    outString += BuildDraftString(previousDraftChoices);
                    return outString;
                }

                UInt32 readNumber = 0;
                if (UInt32.TryParse(param.Substring(0,1), out readNumber))
                {
                    if (readNumber > 0 && readNumber < 4)
                    {
                        List<Skill> previousDraftChoices = database.GetPreviousDraftChoices(user.Id.ToString(), localMemory);
                        Int64 draftCount = database.GetDraftCount(user.Id.ToString());
                        database.DraftSkill(user.Id.ToString(), draftCount, previousDraftChoices[Convert.ToInt32(readNumber) - 1].name);
                        DraftInfo draftInfo = database.GetDraft(user.Id.ToString(), localMemory);
                        List<Skill> draftChoices = DraftAlgorithm(draftInfo.draftedSkills, database, database.GetPreviousDraftChoices(user.Id.ToString(), localMemory));
                        database.StorePreviousDraftChoices(user.Id.ToString(), draftChoices);
                        if (draftInfo.totalPoints > 29)
                        {
                            database.SetState(user.Id.ToString(), "Finish");
                            outString += BuildCurrentString(draftInfo.draftedSkills, draftInfo.totalNoOfRerolls, draftInfo.totalPoints);
                            outString += "Finished Draft!";
                            database.SetRemind(user.Id.ToString(), outString);
                            return outString;
                        }
                        else
                        {
                            outString += BuildCurrentString(draftInfo.draftedSkills, draftInfo.totalNoOfRerolls, draftInfo.totalPoints + 1);
                            outString += BuildDraftString(draftChoices);
                            database.SetRemind(user.Id.ToString(), outString);
                            return outString;
                        }

                    }
                    else if (readNumber == 4)
                    {
                        Int64 draftCount = database.GetDraftCount(user.Id.ToString());
                        DraftInfo draftInfo = database.GetDraft(user.Id.ToString(), localMemory);
                        database.Reroll(user.Id.ToString(), draftInfo.numberOfDraftRowsInDb, draftInfo.lastChoiceWasAReroll, draftInfo.numberOfRerollsInLastChain);
                        List<Skill> draftChoices = DraftAlgorithm(draftInfo.draftedSkills, database, database.GetPreviousDraftChoices(user.Id.ToString(), localMemory));
                        database.StorePreviousDraftChoices(user.Id.ToString(), draftChoices);
                        outString += BuildCurrentString(draftInfo.draftedSkills, draftInfo.totalNoOfRerolls + 1, draftInfo.totalPoints + 1);
                        outString += BuildDraftString(draftChoices);
                        database.SetRemind(user.Id.ToString(), outString);
                        return outString;
                    }
                    else
                    {
                        return "Invalid choice, please select one of the options.";
                    }
                }
                else
                {
                    return "Invalid parameter; must be a number between 1 to 4.";
                }
            }

            return "Type #new to draft again.";
        }

        private string BuildCurrentString(HashSet<Skill> currentDraft, UInt64 totalNoOfRerolls, UInt64 currentLevel)
        {
            string outString = "Current Draft: ";
            foreach (Skill draftedSkill in currentDraft)
            {
                outString += " " + draftedSkill.name + " " + draftedSkill.level + ",";
            }

            outString = outString.Substring(0, outString.Length - 1);
            outString += ".\n";
            outString += "Current Level: ";
            outString += (currentLevel);
            outString += ".\n";
            outString += "Total Cash Spent on Rerolls So Far: $";
            outString += (totalNoOfRerolls * 1000);
            outString += ".\n";

            return outString;
        }

        private string BuildDraftString(List<Skill> draftChoices)
        {
                string outString = "";
                foreach (Skill choice in draftChoices)
                {
                    outString = outString + "[" + (draftChoices.IndexOf(choice) + 1) + "] " + choice.name + " " + choice.level + "\n";
                }
                outString = outString + "[4] Reroll";
                return outString;
        }

        public string ShortDescription()
        {
            return "Without parameter: Shows the current draft. With parameter: Makes a choice.";
        }

    }

}