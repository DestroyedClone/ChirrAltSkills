using System;
using System.Collections.Generic;
using System.Text;

namespace ChirrAltSkills.Chirr.States.Secondary
{
    internal class HeadbuttWeaken : EntityStates.SS2UStates.Chirr.Headbutt
    {
        public override void OnEnter()
        {
            base.OnEnter();
            damageType = RoR2.DamageType.WeakOnHit;
        }
    }
}
