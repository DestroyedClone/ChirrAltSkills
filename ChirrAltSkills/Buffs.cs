using ChirrAltSkills.Chirr;
using R2API;
using RoR2;
using Starstorm2Unofficial;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ChirrAltSkills
{
    internal class Buffs
    {
        public static BuffDef snackyBuff;
        public static BuffDef soulmateBuff;
        public static BuffDef goldRushBuff;
        public static BuffDef hoverDurationIndicatorBuff;
        public static BuffDef lapinBuff;

        public static void Init()
        {
            CreateBuffs();

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var snackyBuffCount = sender.GetBuffCount(snackyBuff);
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
                sender.characterMotor.mass = ChirrMain.cachedMass + (ChirrMain.cachedMass / 4 * snackyBuffCount);
            }
            var soulmateBuffCount = sender.GetBuffCount(soulmateBuff);
            if (soulmateBuffCount > 0)
            {
                args.armorAdd += 5f * soulmateBuffCount;
                args.baseRegenAdd += 0.5f * soulmateBuffCount;
                //Get level health
                args.healthMultAdd += 0.1f * soulmateBuffCount;
            }
            var lapinCount = sender.GetBuffCount(lapinBuff);
            if (lapinCount > 0)
            {
                args.jumpPowerMultAdd += 0.1f * lapinCount;
            }
        }

        public static void CreateBuffs()
        {
            snackyBuff = ScriptableObject.CreateInstance<BuffDef>();
            snackyBuff.name = "DCSS2UChirrSnackies";
            snackyBuff.buffColor = Color.white;
            snackyBuff.canStack = true;
            snackyBuff.iconSprite = Assets.ChirrAssets.buffSnackiesIcon;
            snackyBuff.isCooldown = false;
            snackyBuff.isDebuff = false;
            snackyBuff.isHidden = false;
            ContentAddition.AddBuffDef(snackyBuff);

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


            lapinBuff = ScriptableObject.CreateInstance<BuffDef>();
            lapinBuff.name = "DCSS2UChirrBoostJumpPower";
            lapinBuff.buffColor = Color.white;
            //lapinBuff.iconSprite = null;
            lapinBuff.canStack = true;
            lapinBuff.isCooldown = false;
            lapinBuff.isDebuff = false;
            lapinBuff.isHidden = false;
            ContentAddition.AddBuffDef(lapinBuff);
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
