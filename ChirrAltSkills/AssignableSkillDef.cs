using RoR2;
using RoR2.Skills;

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