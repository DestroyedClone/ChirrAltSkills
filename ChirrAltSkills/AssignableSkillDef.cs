using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace ChirrAltSkills
{
    public class AssignableSkillDef : SkillDef
    {
        public System.Func<GenericSkill, SkillDef.BaseSkillInstanceData> onAssign;
        public System.Action<GenericSkill> onUnassign;

        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            return onAssign?.Invoke(skillSlot);
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            base.OnUnassigned(skillSlot);
            onUnassign?.Invoke(skillSlot);
        }
    }
}
