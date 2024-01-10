using JetBrains.Annotations;
using RoR2;

namespace ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef
{
    public class ChirrTargetableSkillDef : AssignableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            var tracker = skillSlot.GetComponent<ChirrTracker>();
            tracker.enabled = true;
            return new ChirrTargetableSkillDef.InstanceData
            {
                chirrTracker = tracker
            };
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            ((ChirrTargetableSkillDef.InstanceData)skillSlot.skillInstanceData).chirrTracker.enabled = false;
            base.OnUnassigned(skillSlot);
        }

        private static bool HasTarget([NotNull] GenericSkill skillSlot)
        {
            ChirrTracker chirrTracker = ((ChirrTargetableSkillDef.InstanceData)skillSlot.skillInstanceData).chirrTracker;
            return chirrTracker != null ? chirrTracker.GetTrackingTarget() : null;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return ChirrTargetableSkillDef.HasTarget(skillSlot) && base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && ChirrTargetableSkillDef.HasTarget(skillSlot);
        }

        protected class InstanceData : BaseSkillInstanceData
        {
            public ChirrTracker chirrTracker;
            public bool healthThreshold = false;
            public float healthThresholdPercentage;
        }
    }
}