using System;
using System.Collections.Generic;

namespace draftbot
{
    class SkillWeight : IComparable<SkillWeight>
    {
        public Skill skill
        {
            get;
            private set;
        }
        public float weight
        {
            get;
            private set;
        }

        public SkillWeight(Skill skillIn, float weightIn)
        {
            skill = skillIn;
            weight = weightIn;
        }

        public int CompareTo(SkillWeight otherSkillWeight)
        {
            if (otherSkillWeight == null)
            {
                return 1;
            }
            return Comparer<float>.Default.Compare(this.weight, otherSkillWeight.weight);
        }
    }
}