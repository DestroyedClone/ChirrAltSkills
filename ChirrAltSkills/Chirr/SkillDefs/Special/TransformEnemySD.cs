using ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef;
using JetBrains.Annotations;
using RoR2;

namespace ChirrAltSkills.Chirr.SkillDefs.Special
{
    public class TransformEnemySD : ChirrTargetableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            var original = base.OnAssigned(skillSlot);
            ((ChirrTargetableSkillDef.InstanceData)original).chirrTracker.targetNeedsMaster = true;
            return original;
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            ((ChirrTargetableSkillDef.InstanceData)skillSlot.skillInstanceData).chirrTracker.targetNeedsMaster = false;
            base.OnUnassigned(skillSlot);
        }
    }
}