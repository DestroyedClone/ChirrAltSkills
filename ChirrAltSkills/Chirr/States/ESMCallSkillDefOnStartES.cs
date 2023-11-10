using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChirrAltSkills.Chirr.States
{
    internal class ESMCallSkillDefOnStartES : EntityState
    {
        public override void OnEnter()
        {

            foreach (var gs in characterBody.GetComponents<GenericSkill>())
            {
                if (gs && gs.skillDef && gs.skillDef.activationStateMachineName == "Passive")
                {
                    gs.ExecuteIfReady();
                    return;
                }
            }
        }
    }
}
