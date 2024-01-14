using RoR2;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.SkillDefs
{
    public class PassiveStageBuffSD : AssignableSkillDef
    {
        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var info = ChirrStageBuffInfo.GetStageBuffInfo(currentScene.name);
            info.Apply(skillSlot.characterBody);
            info.SendChatAnnouncementMessage(skillSlot.characterBody);
            return base.OnAssigned(skillSlot);
        }
    }
}