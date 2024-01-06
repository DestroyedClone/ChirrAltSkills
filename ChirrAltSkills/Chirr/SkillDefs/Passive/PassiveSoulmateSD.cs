using RoR2;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.SkillDefs
{
    public class PassiveSoulmateSD : AssignableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            if (NetworkServer.active)
            if (!ChirrSetup.chirrSoulmates.Contains(skillSlot.characterBody))
            {
                ChirrSetup.chirrSoulmates.Add(skillSlot.characterBody);
                skillSlot.characterBody.AddComponent<SoulmateChirrBuffer>().body = skillSlot.characterBody;
            }
            return base.OnAssigned(skillSlot);
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            if (NetworkServer.active)
            if (ChirrSetup.chirrSoulmates.Contains(skillSlot.characterBody))
            {
                ChirrSetup.chirrSoulmates.Remove(skillSlot.characterBody);
                var comp = skillSlot.characterBody.GetComponent<SoulmateChirrBuffer>();
                if (comp)
                    Destroy(comp);
            }
            base.OnUnassigned(skillSlot);
        }
    }
}