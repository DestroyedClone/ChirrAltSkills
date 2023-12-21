using EntityStates;
using RoR2;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.States.Passive
{
    internal class PassiveStageBuffES : EntityState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (!NetworkServer.active) return;
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var info = ChirrStageBuffInfo.GetStageBuffInfo(currentScene.name);
            info.Apply(characterBody);
            if (characterBody.hasEffectiveAuthority)
            {
                Chat.AddMessage($"");
            }
        }
    }
}