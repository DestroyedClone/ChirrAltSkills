using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using ChirrAltSkills.Chirr.States.CharacterMains;

namespace ChirrAltSkills.Chirr.States.Passive
{
    internal class PassiveTakeFlightES : EntityState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            //https://github.com/Moffein/Starstorm2Unofficial/blob/546dec44fd5d967f2f66dfd261bad7dbbb55cdcc/Starstorm%202/Survivors/Chirr/ChirrCore.cs#L595C56-L595C56
            characterBody.baseJumpPower = 22.5f;
        }
    }
}
