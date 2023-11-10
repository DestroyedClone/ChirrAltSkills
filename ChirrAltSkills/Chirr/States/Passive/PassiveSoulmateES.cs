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

            if (ChirrMain.chirrSoulmates.Contains(characterBody))
                return;

            ChirrMain.chirrSoulmates.Add(characterBody);
            characterBody.AddComponent<SoulmateChirrBuffer>().body = characterBody;
        }
    }
}