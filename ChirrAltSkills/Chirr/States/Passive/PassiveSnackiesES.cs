﻿using System;
using System.Collections.Generic;
using System.Text;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.States.Passive
{
    internal class PassiveSnackiesES : EntityState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (!NetworkServer.active || !RoR2.Run.instance) return;
            int stageClearCount = Mathf.Abs(RoR2.Run.instance.stageClearCount);
            var buffCount = 2 + stageClearCount * 2;
            for (int i = 0; i < buffCount; i++)
            {
                characterBody.AddBuff(Buffs.snackyBuff);
            }
        }
    }
}
