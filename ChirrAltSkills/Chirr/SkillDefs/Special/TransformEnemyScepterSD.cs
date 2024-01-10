using ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef;
using JetBrains.Annotations;
using RoR2;

namespace ChirrAltSkills.Chirr.SkillDefs.Special
{
    public class TransformEnemyScepterSD : ChirrTargetableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            var original = base.OnAssigned(skillSlot);
            ChirrTracker tracker = ((ChirrTargetableSkillDef.InstanceData)original).chirrTracker;
            tracker.targetCanBeBoss = false;
            tracker.targetNeedsMaster = true;
            tracker.targetHealthThreshold = true;
            tracker.targetHealthThresholdPercentage = 0.5f;
            return original;
        }
    }
}