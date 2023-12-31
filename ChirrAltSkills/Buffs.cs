﻿using ChirrAltSkills.Chirr;
using R2API;
using RoR2;
using UnityEngine;

namespace ChirrAltSkills
{
    internal class Buffs
    {
        public static BuffDef indulgenceBuff;
        public static BuffDef snackiesBuff;
        public static BuffDef soulmateBuff;
        public static BuffDef adrenalineBuff;
        public static BuffDef hoverDurationIndicatorBuff;
        public static BuffDef bunnyJumpBuff;

        public static void Init()
        {
            CreateBuffs();

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var snackyBuffCount = sender.GetBuffCount(snackiesBuff);
            var gluttonyBuffCount = sender.GetBuffCount(indulgenceBuff);
            var snackerBuffCountTotal = snackyBuffCount + gluttonyBuffCount;
            if (snackerBuffCountTotal > 0)
            {
                args.armorAdd += 0.005f * snackerBuffCountTotal;
                args.attackSpeedMultAdd -= 0.003f * snackerBuffCountTotal;
                args.baseHealthAdd += 15 * snackerBuffCountTotal;
                args.jumpPowerMultAdd -= 0.001f * snackerBuffCountTotal;
                args.moveSpeedReductionMultAdd += 0.005f * snackerBuffCountTotal;
                //args.utilityCooldownMultAdd += 0.1f * buffCount;
                //Relicofmass
                //sender.acceleration = sender.baseAcceleration / (snackyBuffCount * StaticValues.massFactor / 2);
                sender.characterMotor.mass = ChirrSetup.cachedMass + (ChirrSetup.cachedMass / 10 * snackerBuffCountTotal);
            }
            var soulmateBuffCount = sender.GetBuffCount(soulmateBuff);
            if (soulmateBuffCount > 0)
            {
                args.armorAdd += 5f * soulmateBuffCount;
                args.baseRegenAdd += 0.5f * soulmateBuffCount;
                //Get level health
                args.healthMultAdd += 0.1f * soulmateBuffCount;
            }
            var lapinCount = sender.GetBuffCount(bunnyJumpBuff);
            if (lapinCount > 0)
            {
                args.jumpPowerMultAdd += 0.1f * lapinCount;
            }
        }

        public static void CreateBuffs()
        {
            snackiesBuff = ScriptableObject.CreateInstance<BuffDef>();
            snackiesBuff.name = "DCSS2UChirrSnackies";
            snackiesBuff.buffColor = new Color(0.980f, 0.376f, 0.00f); //orange ish
            snackiesBuff.canStack = true;
            snackiesBuff.iconSprite = Assets.ChirrAssets.buffSnackiesIcon;
            snackiesBuff.isCooldown = false;
            snackiesBuff.isDebuff = false;
            snackiesBuff.isHidden = false;
            ContentAddition.AddBuffDef(snackiesBuff);

            indulgenceBuff = ScriptableObject.CreateInstance<BuffDef>();
            indulgenceBuff.name = "DCSS2UChirrIndulgence";
            indulgenceBuff.buffColor = new Color(0.690f, 0.104f, 0.104f); //gross dark red
            indulgenceBuff.iconSprite = Assets.ChirrAssets.buffIndulgenceIcon;
            indulgenceBuff.canStack = true;
            indulgenceBuff.isCooldown = false;
            indulgenceBuff.isDebuff = false;
            indulgenceBuff.isHidden = false;
            ContentAddition.AddBuffDef(indulgenceBuff);

            soulmateBuff = ScriptableObject.CreateInstance<BuffDef>();
            soulmateBuff.name = "DCSS2UChirrComfortingPresence";
            soulmateBuff.buffColor = new Color(0.351f, 0.770f, 0.246f); //chirr-ish green
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
                adrenalineBuff = ScriptableObject.CreateInstance<BuffDef>();
                adrenalineBuff.name = "DCSS2UChirrAdrenaline";
                adrenalineBuff.buffColor = new Color(0.753f, 0.770f, 0.246f); //chirr-ish green + yelloish
                adrenalineBuff.iconSprite = Assets.ChirrAssets.buffAdrenalineIcon;
                adrenalineBuff.canStack = true;
                adrenalineBuff.isCooldown = false;
                adrenalineBuff.isDebuff = false;
                adrenalineBuff.isHidden = false;
                RecalculateStatsAPI.GetStatCoefficients += ChirrGoldRushRecalcStats;
            }

            hoverDurationIndicatorBuff = ScriptableObject.CreateInstance<BuffDef>();
            hoverDurationIndicatorBuff.name = "DCSS2UChirrHoverDurationRemaining";
            hoverDurationIndicatorBuff.buffColor = new Color(0.351f, 0.770f, 0.246f); //chirr-ish green
            hoverDurationIndicatorBuff.iconSprite = Assets.ChirrAssets.buffHoverDurationIcon;
            hoverDurationIndicatorBuff.canStack = true;
            hoverDurationIndicatorBuff.isCooldown = false;
            hoverDurationIndicatorBuff.isDebuff = false;
            hoverDurationIndicatorBuff.isHidden = false;
            ContentAddition.AddBuffDef(hoverDurationIndicatorBuff);

            bunnyJumpBuff = ScriptableObject.CreateInstance<BuffDef>();
            bunnyJumpBuff.name = "DCSS2UChirrBunnyJumpBoost";
            bunnyJumpBuff.buffColor = new Color(0.351f, 0.770f, 0.246f); //chirr-ish green
            bunnyJumpBuff.iconSprite = Assets.ChirrAssets.buffBunnyJumpIcon;
            bunnyJumpBuff.canStack = true;
            bunnyJumpBuff.isCooldown = false;
            bunnyJumpBuff.isDebuff = false;
            bunnyJumpBuff.isHidden = false;
            ContentAddition.AddBuffDef(bunnyJumpBuff);
        }

        private static void ChirrGoldRushRecalcStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int goldRushCount = sender.GetBuffCount(Buffs.adrenalineBuff);
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
            adrenalineBuff = BuffCatalog.GetBuffDef(BuffCatalog.FindBuffIndex("GoldRush"));
        }
    }
}