using System;
using System.Collections.Generic;
using System.Text;

namespace ChirrAltSkills.Chirr.States.Special
{
    internal class EatEnemyScepter : EatEnemy
    {
        public override bool AllowBoss => true;
    }
}
