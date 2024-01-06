using ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef;
using JetBrains.Annotations;
using RoR2;

namespace ChirrAltSkills.Chirr.SkillDefs.Special
{
    public class EatEnemySD : ChirrTargetableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            //if (NetworkServer.active)
            /*if (!skillSlot.GetComponent<EatRadiusBehavior>())
            {
                skillSlot.AddComponent<EatRadiusBehavior>();
            }*/
            return base.OnAssigned(skillSlot);
        }

        public override void OnUnassigned(GenericSkill skillSlot)
        {
            /*if (NetworkServer.active)
            if (skillSlot.GetComponent<EatRadiusBehavior>())
            {
                Destroy(skillSlot.GetComponent<EatRadiusBehavior>());
            }*/
            base.OnUnassigned(skillSlot);
        }
    }
}