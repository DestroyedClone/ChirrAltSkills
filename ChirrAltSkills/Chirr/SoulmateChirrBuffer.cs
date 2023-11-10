using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ChirrAltSkills.Chirr
{
    public class SoulmateChirrBuffer : MonoBehaviour
    {
        public CharacterBody body;

        public float stopwatch = 0;
        public float timer = 4f;

        public void FixedUpdate()
        {
            if (stopwatch <= 0)
            {
                stopwatch = timer;
                var buffCount = body.GetBuffCount(Buffs.soulmateBuff);
                var difference = ChirrMain.commandoSoulmates.Count - buffCount;
                if (difference > 0)
                {
                    for (int i = 0; i < difference; i++)
                    {
                        body.AddBuff(Buffs.soulmateBuff);
                    }
                }
                else if (difference < 0)
                {
                    for (int i = 0; i < -difference; i++)
                    {
                        body.RemoveBuff(Buffs.soulmateBuff);
                    }
                }
            }
        }
    }
}
