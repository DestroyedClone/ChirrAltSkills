using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.SkillDefs
{
    public class PassiveSnackiesPerStageSD : AssignableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            if (NetworkServer.active)
            {
                int stageClearCount = Mathf.Abs(RoR2.Run.instance.stageClearCount);
                var buffCount = 2 + stageClearCount * 2;
                for (int i = 0; i < buffCount; i++)
                {
                    skillSlot.characterBody.AddBuff(Buffs.indulgenceBuff);
                }
            }
            return base.OnAssigned(skillSlot);
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            if (NetworkServer.active)
            {
                while (skillSlot.characterBody.HasBuff(Buffs.indulgenceBuff))
                {
                    skillSlot.characterBody.RemoveBuff(Buffs.indulgenceBuff);
                }
            }
            base.OnUnassigned(skillSlot);
        }
    }
}