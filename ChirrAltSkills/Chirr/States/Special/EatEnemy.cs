using EntityStates;
using Starstorm2Unofficial.Survivors.Chirr.Components;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Skills;
using UnityEngine.Networking;
using UnityEngine;

namespace ChirrAltSkills.Chirr.States.Special
{
    internal class EatEnemy : BaseState
    {
        public float radius = 5;
        public float radiusPerStack = 1;
        public float duration = 60;
        public virtual bool AllowBoss => false;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("SS2UChirrSpecial", gameObject);

            if (!characterBody.hasEffectiveAuthority) return;
            var buffCount = characterBody.GetBuffCount(Buffs.snackyBuff);
            var calculatedRadius = radius + radiusPerStack * buffCount;
            var cachedPosition = characterBody.corePosition;
            foreach (var enemy in CharacterMaster.readOnlyInstancesList)
            {
                if (enemy.teamIndex == teamComponent.teamIndex)
                    continue;
                var enemyCB = enemy.GetBody();
                if (!enemyCB)
                    continue;
                if (enemyCB.isBoss && !AllowBoss)
                    continue;
                if (Vector3.Distance(enemyCB.corePosition, cachedPosition) > calculatedRadius)
                    continue; 
                var enemyHC = enemyCB.healthComponent;
                if (!enemyHC || !enemyHC.alive || enemyHC.health <= 0 || enemyHC.combinedHealthFraction > 0.5f)
                    continue;

                var remainingHealthFraction = enemyHC.combinedHealthFraction;
                int buffsToGive = Mathf.CeilToInt(remainingHealthFraction * 10);
                for (int i = 0; i < buffsToGive; i++)
                    characterBody.AddTimedBuff(Buffs.snackyBuff, duration);

                EffectData effectData = new EffectData();
                effectData.origin = transform.position;
                effectData.SetNetworkedObjectReference(gameObject);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FruitHealEffect"), effectData, true);
                healthComponent.HealFraction(remainingHealthFraction, default);

                enemyHC.Suicide(characterBody.gameObject);
                break;
            }
            /*if (!foundTarget)
            {
                skillLocator.special.AddOneStock();
            }*/
            outer.SetNextStateToMain();
        }
    }
}
