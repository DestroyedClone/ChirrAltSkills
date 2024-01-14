using ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef;
using EntityStates;
using R2API;
using RoR2;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.States.Special
{
    internal class EatEnemyES : BaseState
    {
        public const float baseBuffDuration = 10;
        public const float stackBuffDuration = 1f;
        public const float damageCoefficient = 5f; //1000%
        public const float stacksPerHealthPercentage = 0.5f;
        public const float secondsPerHealthPercentage = 0.5f;

        public virtual bool AllowBoss => false;

        private ChirrTracker chirrTracker;

        private readonly float duration = 0.25f;

        public override void OnEnter()
        {
            base.OnEnter();
            this.chirrTracker = base.GetComponent<ChirrTracker>();
            Util.PlaySound("SS2UChirrSpecial", gameObject);

            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(duration + 1f);
            }
            if (NetworkServer.active)
                ChompEnemy(chirrTracker.GetTrackingTarget().healthComponent);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > this.duration)
            {
                if (base.isAuthority)
                    this.outer.SetNextStateToMain();
                return;
            }
        }

        public void ChompEnemy(HealthComponent enemyHC)
        {
            DamageInfo damageInfo = new DamageInfo()
            {
                attacker = gameObject,
                inflictor = gameObject,
                crit = characterBody.RollCrit(),
                damage = characterBody.damage * damageCoefficient,
                damageType = DamageType.Generic,
                damageColorIndex = DamageColorIndex.WeakPoint,
                position = enemyHC.body.corePosition,
                procCoefficient = 0.1f
            };
            damageInfo.AddModdedDamageType(DamageTypes.chirrChompDT);
            enemyHC.TakeDamage(damageInfo);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}