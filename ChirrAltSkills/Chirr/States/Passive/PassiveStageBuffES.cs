using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using ChirrAltSkills.Chirr.States.CharacterMains;

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
