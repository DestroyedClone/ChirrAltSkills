using System;
using System.Collections.Generic;
using System.Text;
using ChirrAltSkills.Chirr.States.CharacterMains;
using EntityStates;
using RoR2;

namespace ChirrAltSkills.Chirr.States.Passive
{
    internal class PassiveLapinES : EntityState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            characterBody.baseJumpCount += 2;
            characterBody.baseJumpPower *= 0.75f;
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            characterBody.MarkAllStatsDirty();
        }
    }
}
