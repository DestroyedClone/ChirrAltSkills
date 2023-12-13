using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.States.Passive
{
    internal class PassiveStageBuffES : EntityState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (currentScene == null)
                return;
            var info = ChirrStageBuffInfo.GetStageBuffInfo(currentScene.name);

            if (!NetworkServer.active) return;

            info.Apply(characterBody);
        }
    }
}
