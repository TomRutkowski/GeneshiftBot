using System;
using System.Collections.Generic;

namespace draftbot
{
    class DraftInfo
    {
        public HashSet<Skill> draftedSkills;

        //todo: ordered skills definition

        public UInt32 totalNoOfRerolls;
        public bool lastChoiceWasAReroll;
        public UInt32 numberOfRerollsInLastChain;
        public UInt32 numberOfDraftRowsInDb;
        public UInt32 totalPoints;
    }
}