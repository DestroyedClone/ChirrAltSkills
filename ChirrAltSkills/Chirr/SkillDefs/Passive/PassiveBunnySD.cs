using RoR2;
using RoR2.Skills;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.SkillDefs.Passive
{
    public class PassiveBunnySD : AssignableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            PassiveBunnySD.InstanceData instanceData = new PassiveBunnySD.InstanceData();
            instanceData.skillSlot = skillSlot;
            skillSlot.characterBody.baseJumpCount += 2;
            skillSlot.characterBody.baseJumpPower -= 3.75f; //default 15, -25% = 11.25
            skillSlot.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            skillSlot.characterBody.MarkAllStatsDirty();
            skillSlot.characterBody.characterMotor.onHitGroundAuthority += instanceData.BunnyBoostOnLand_Server;
            return instanceData;
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            skillSlot.characterBody.baseJumpCount -= 2;
            skillSlot.characterBody.baseJumpPower += 3.75f;
            if (NetworkServer.active)
            {
                while (skillSlot.characterBody.HasBuff(Buffs.bunnyJumpBuff))
                {
                    skillSlot.characterBody.RemoveBuff(Buffs.bunnyJumpBuff);
                }
            }
            skillSlot.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            skillSlot.characterBody.MarkAllStatsDirty();
            skillSlot.characterBody.characterMotor.onHitGroundAuthority -= ((PassiveBunnySD.InstanceData)skillSlot.skillInstanceData).BunnyBoostOnLand_Server;
            base.OnUnassigned(skillSlot);
        }

        public class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public void BunnyBoostOnLand_Server(ref CharacterMotor.HitGroundInfo hitGroundInfo)
            {
                skillSlot.characterBody.AddTimedBuffAuthority(Buffs.bunnyJumpBuff.buffIndex, 10f);
            }

            public GenericSkill skillSlot;
        }
    }
}