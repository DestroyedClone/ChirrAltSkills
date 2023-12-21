using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.States.Passive
{
    internal class PassiveSnackiesPerStageES : EntityState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (!NetworkServer.active || !RoR2.Run.instance) return;
            int stageClearCount = Mathf.Abs(RoR2.Run.instance.stageClearCount);
            var buffCount = 2 + stageClearCount * 2;
            for (int i = 0; i < buffCount; i++)
            {
                characterBody.AddBuff(Buffs.snackiesBuff);
            }
        }
    }
}