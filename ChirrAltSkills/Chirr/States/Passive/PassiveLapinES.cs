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
            characterBody.baseJumpCount++;
            characterBody.baseJumpPower *= 3 / 4;
            characterBody.MarkAllStatsDirty();
        }
    }
}
