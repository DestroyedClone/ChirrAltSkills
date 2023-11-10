using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;

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
            if (info.stageIds.Length == 0) return;

        }
    }
}
