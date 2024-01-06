using RoR2;

namespace ChirrAltSkills.Chirr.SkillDefs
{
    public class PassiveStageBuffSD : AssignableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var info = ChirrStageBuffInfo.GetStageBuffInfo(currentScene.name);
            info.Apply(skillSlot.characterBody);
            return base.OnAssigned(skillSlot);
        }
    }
}