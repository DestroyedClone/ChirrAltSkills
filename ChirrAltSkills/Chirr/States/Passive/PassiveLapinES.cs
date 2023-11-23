using System;
using System.Collections.Generic;
using System.Text;
using EntityStates;
using RoR2;

namespace ChirrAltSkills.Chirr.States.Passive
{
    internal class PassiveLapinES : EntityState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage($"Lapin!");
            characterBody.baseJumpCount += 1;
            characterBody.baseJumpPower *= 0.75f;
            characterBody.MarkAllStatsDirty();
        }
    }
}
