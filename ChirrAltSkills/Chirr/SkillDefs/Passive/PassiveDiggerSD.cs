using ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef;
using RoR2;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.SkillDefs
{
    public class PassiveDiggerSD : AssignableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            if (!NetworkServer.active)
                return base.OnAssigned(skillSlot);

            var cloneComponent = skillSlot.characterBody.GetComponent<DiggerCloneComponent>();
            if (!cloneComponent)
                cloneComponent = skillSlot.characterBody.AddComponent<DiggerCloneComponent>();
            return new InstanceData
            {
                diggerComponent = cloneComponent,
            };
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            if (NetworkServer.active)
            {
                Destroy(((InstanceData)skillSlot.skillInstanceData).diggerComponent);
            }
            base.OnUnassigned(skillSlot);
        }
        protected class InstanceData : BaseSkillInstanceData
        {
            public DiggerCloneComponent diggerComponent;
        }
    }
}