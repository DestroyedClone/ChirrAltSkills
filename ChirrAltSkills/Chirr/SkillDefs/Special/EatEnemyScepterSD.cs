using ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef;
using JetBrains.Annotations;
using RoR2;

namespace ChirrAltSkills.Chirr.SkillDefs.Special
{
    public class EatEnemyScepterSD : ChirrTargetableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            var original = base.OnAssigned(skillSlot);
            var tracker = ((ChirrTargetableSkillDef.InstanceData)original).chirrTracker;
            tracker.maxTrackingDistance = 10;
            tracker.targetNeedsHealthThreshold = false;
            tracker.targetCanBeNonMaster = true;
            tracker.targetCanBeBoss = true;
            return original;
        }
    }
}