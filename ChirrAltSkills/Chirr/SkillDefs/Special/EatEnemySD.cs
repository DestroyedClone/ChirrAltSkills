using ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef;
using JetBrains.Annotations;
using RoR2;

namespace ChirrAltSkills.Chirr.SkillDefs.Special
{
    public class EatEnemySD : ChirrTargetableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            var original = base.OnAssigned(skillSlot);
            var tracker = ((ChirrTargetableSkillDef.InstanceData)original).chirrTracker;
            tracker.targetHealthThreshold = false;
            tracker.targetNeedsMaster = false;
            tracker.targetCanBeBoss = false;
            return original;
        }
    }
}