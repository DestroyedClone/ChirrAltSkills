using System;
using System.Collections.Generic;
using System.Text;
using EntityStates;

namespace ChirrAltSkills.Chirr.States.Passive
{
    internal class PassiveSoulmateES : EntityState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            if (ChirrSetup.chirrSoulmates.Contains(characterBody))
                return;

            ChirrSetup.chirrSoulmates.Add(characterBody);
            characterBody.AddComponent<SoulmateChirrBuffer>().body = characterBody;
        }
    }
}