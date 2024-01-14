using ChirrAltSkills.Chirr.States.Special;
using R2API;
using RoR2;
using UnityEngine;

namespace ChirrAltSkills
{
    internal class DamageTypes
    {
        public static R2API.DamageAPI.ModdedDamageType chirrChompDT;

        public static void Init()
        {
            chirrChompDT = R2API.DamageAPI.ReserveDamageType();

            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport dr)
        {
            if (!dr.attacker
                || dr.attackerBodyIndex != Starstorm2Unofficial.Survivors.Chirr.ChirrCore.bodyIndex) return;
            if (DamageAPI.HasModdedDamageType(dr.damageInfo, chirrChompDT))
            {
                EatEnemy(dr);
            }
        }

        private static void EatEnemy(DamageReport damageReport)
        {
            float remainingHealthFraction = damageReport.combinedHealthBeforeDamage / damageReport.victimBody.healthComponent.fullCombinedHealth;
            float remainingHealthPercentage = remainingHealthFraction * 100;

            float durationEating = EatEnemyES.baseBuffDuration + EatEnemyES.stackBuffDuration * remainingHealthPercentage * EatEnemyES.secondsPerHealthPercentage;
            int buffsToGive = Mathf.CeilToInt(remainingHealthPercentage * EatEnemyES.stacksPerHealthPercentage);
            for (int i = 0; i < buffsToGive; i++)
                damageReport.attackerBody.AddTimedBuff(Buffs.snackiesBuff, durationEating);

            int victimBuffCount = damageReport.victimBody.GetBuffCount(Buffs.snackiesBuff);
            victimBuffCount += damageReport.victimBody.GetBuffCount(Buffs.indulgenceBuff);
            float durationStealing = 10f;
            for (int i = 0; i < victimBuffCount; i++)
                damageReport.attackerBody.AddTimedBuff(Buffs.snackiesBuff, durationStealing);

            EffectData effectData = new EffectData
            {
                origin = damageReport.attackerBody.corePosition
            };
            effectData.SetNetworkedObjectReference(damageReport.attacker);
            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FruitHealEffect"), effectData, true);
            damageReport.attackerBody.healthComponent.HealFraction(remainingHealthFraction, default);
        }
    }
}