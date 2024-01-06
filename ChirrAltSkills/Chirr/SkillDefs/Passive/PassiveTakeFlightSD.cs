using RoR2;

namespace ChirrAltSkills.Chirr.SkillDefs.Passive
{
    public class PassiveTakeFlightSD : AssignableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            //https://github.com/Moffein/Starstorm2Unofficial/blob/546dec44fd5d967f2f66dfd261bad7dbbb55cdcc/Starstorm%202/Survivors/Chirr/ChirrCore.cs#L595C56-L595C56
            skillSlot.characterBody.baseJumpPower = 22.5f;
            return base.OnAssigned(skillSlot);
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            skillSlot.characterBody.baseJumpPower = 15;
            base.OnUnassigned(skillSlot);
        }
    }
}