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
            tracker.maxTrackingDistance = 10;
            tracker.targetCanBeBoss = false;
            tracker.targetCanBeNonMaster = true;
            tracker.targetNeedsHealthThreshold = true;
            tracker.targetHealthThresholdPercentage = 0.5f;
            return original;
        }
    }
}