using ChirrAltSkills.Chirr;
using R2API;
using RoR2;
using UnityEngine;

namespace ChirrAltSkills
{
    internal class Buffs
    {
        public static BuffDef snackiesBuff;
        public static BuffDef soulmateBuff;
        public static BuffDef goldRushBuff;
        public static BuffDef hoverDurationIndicatorBuff;
        public static BuffDef bunnyBuff;

        public static void Init()
        {
            CreateBuffs();

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var snackyBuffCount = sender.GetBuffCount(snackiesBuff);
            if (snackyBuffCount > 0)
            {
                args.armorAdd += 0.005f * snackyBuffCount;
                args.attackSpeedMultAdd -= 0.005f * snackyBuffCount;
                args.baseHealthAdd += 15 * snackyBuffCount;
                args.jumpPowerMultAdd -= 0.005f * snackyBuffCount;
                args.moveSpeedReductionMultAdd += 0.05f * snackyBuffCount;
                //args.utilityCooldownMultAdd += 0.1f * buffCount;
                //Relicofmass
                //sender.acceleration = sender.baseAcceleration / (snackyBuffCount * StaticValues.massFactor / 2);
                sender.characterMotor.mass = ChirrSetup.cachedMass + (ChirrSetup.cachedMass / 4 * snackyBuffCount);
            }
            var soulmateBuffCount = sender.GetBuffCount(soulmateBuff);
            if (soulmateBuffCount > 0)
            {
                args.armorAdd += 5f * soulmateBuffCount;
                args.baseRegenAdd += 0.5f * soulmateBuffCount;
                //Get level health
                args.healthMultAdd += 0.1f * soulmateBuffCount;
            }
            var lapinCount = sender.GetBuffCount(bunnyBuff);
            if (lapinCount > 0)
            {
                args.jumpPowerMultAdd += 0.1f * lapinCount;
            }
        }

        public static void CreateBuffs()
        {
            snackiesBuff = ScriptableObject.CreateInstance<BuffDef>();
            snackiesBuff.name = "DCSS2UChirrSnackies";
            snackiesBuff.buffColor = Color.white;
            snackiesBuff.canStack = true;
            snackiesBuff.iconSprite = Assets.ChirrAssets.buffSnackiesIcon;
            snackiesBuff.isCooldown = false;
            snackiesBuff.isDebuff = false;
            snackiesBuff.isHidden = false;
            ContentAddition.AddBuffDef(snackiesBuff);

            soulmateBuff = ScriptableObject.CreateInstance<BuffDef>();
            soulmateBuff.name = "DCSS2UChirrSoulmate";
            soulmateBuff.buffColor = Color.white;
            soulmateBuff.iconSprite = Assets.ChirrAssets.buffSoulmateIcon;
            soulmateBuff.canStack = true;
            soulmateBuff.isCooldown = false;
            soulmateBuff.isDebuff = false;
            soulmateBuff.isHidden = false;
            ContentAddition.AddBuffDef(soulmateBuff);

            if (CASPlugin.modloaded_Miner)
            {
                On.RoR2.BuffCatalog.Init += GetDiggerGoldRush;
            }
            else
            {
                goldRushBuff = ScriptableObject.CreateInstance<BuffDef>();
                goldRushBuff.name = "DCSS2UChirrGoldRush";
                goldRushBuff.buffColor = Color.yellow;
                goldRushBuff.iconSprite = Assets.ChirrAssets.buffGoldrushIcon;
                goldRushBuff.canStack = true;
                goldRushBuff.isCooldown = false;
                goldRushBuff.isDebuff = false;
                goldRushBuff.isHidden = false;
                RecalculateStatsAPI.GetStatCoefficients += ChirrGoldRushRecalcStats;
            }

            hoverDurationIndicatorBuff = ScriptableObject.CreateInstance<BuffDef>();
            hoverDurationIndicatorBuff.name = "DCSS2UChirrHoverDurationRemaining";
            hoverDurationIndicatorBuff.buffColor = Color.white;
            hoverDurationIndicatorBuff.iconSprite = null;
            hoverDurationIndicatorBuff.canStack = true;
            hoverDurationIndicatorBuff.isCooldown = false;
            hoverDurationIndicatorBuff.isDebuff = false;
            hoverDurationIndicatorBuff.isHidden = false;
            ContentAddition.AddBuffDef(hoverDurationIndicatorBuff);

            bunnyBuff = ScriptableObject.CreateInstance<BuffDef>();
            bunnyBuff.name = "DCSS2UChirrBunny";
            bunnyBuff.buffColor = Color.white;
            //lapinBuff.iconSprite = null;
            bunnyBuff.canStack = true;
            bunnyBuff.isCooldown = false;
            bunnyBuff.isDebuff = false;
            bunnyBuff.isHidden = false;
            ContentAddition.AddBuffDef(bunnyBuff);
        }

        private static void ChirrGoldRushRecalcStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int goldRushCount = sender.GetBuffCount(Buffs.goldRushBuff);
            if (goldRushCount > 0)
            {
                args.attackSpeedMultAdd += 0.1f * goldRushCount;
                args.baseMoveSpeedAdd += 0.15f * goldRushCount;
                args.baseRegenAdd += 0.25f * goldRushCount;
            }
        }

        private static void GetDiggerGoldRush(On.RoR2.BuffCatalog.orig_Init orig)
        {
            orig();
            goldRushBuff = BuffCatalog.GetBuffDef(BuffCatalog.FindBuffIndex("GoldRush"));
        }
    }
}