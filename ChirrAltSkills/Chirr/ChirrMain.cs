﻿using RoR2.Skills;
using RoR2;
using Starstorm2Unofficial.Survivors.Chirr;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2.SkillLocator;
using R2API;
using ChirrAltSkills.Chirr.States;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using ChirrAltSkills.Chirr.States.Passive;
using ChirrAltSkills.Chirr.States.Special;
using ChirrAltSkills.Chirr.States.Primary;
using ChirrAltSkills.Chirr.States.CharacterMains;
using ChirrAltSkills.Chirr.States.Passive.Hover;

namespace ChirrAltSkills.Chirr
{
    public class ChirrMain
    {
        public static float cachedMass;

        public static GameObject bodyPrefab;
        public static SkillLocator skillLocator;

        public static EntityStateMachine passiveESM;
        public static GenericSkill passiveSkill;
        public static SkillFamily passiveSkillFamily;

        public static SkillDef passiveEcosystemSD;
        public static SkillDef passiveSnackiesSD;
        public static SkillDef passiveBunnySD;
        public static SkillDef passiveSoulmateSD;
        public static SkillDef passiveMinerSD;
        public static SkillDef passiveDefaultSD;

        public static SteppedSkillDef primaryDoubleTapSD;

        public static SkillDef specialTransformSD;
        public static SkillDef specialEatSD;

        public static List<CharacterBody> chirrSoulmates = new List<CharacterBody>();
        public static List<CharacterBody> commandoSoulmates = new List<CharacterBody> ();

        public static BodyIndex commandoBodyIndex;

        public static void Init()
        {
            bodyPrefab = ChirrCore.chirrPrefab;
            cachedMass = bodyPrefab.GetComponent<CharacterMotor>().mass;
            skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            RevertBaseChanges();
            SetupPassive();
            SetupPrimary();
            SetupSpecial();

            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            Run.onServerGameOver += Run_onServerGameOver;
            On.RoR2.BodyCatalog.Init += CacheBodyIndex;

            ChirrStageBuffInfo.Init();
        }

        private static void CacheBodyIndex(On.RoR2.BodyCatalog.orig_Init orig)
        {
            orig();
            commandoBodyIndex = BodyCatalog.FindBodyIndex("CommandoBody");
        }

        private static void RevertBaseChanges()
        {
            bodyPrefab.GetComponent<CharacterBody>().baseJumpPower = 15f;
            bodyPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(typeof(ChirrCharMainVariableFly));
        }

        private static void Run_onServerGameOver(Run arg1, GameEndingDef arg2)
        {
            chirrSoulmates.Clear();
            commandoSoulmates.Clear();
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport obj)
        {
            if (!NetworkServer.active) return;
            if (obj == null) return;
            if (!obj.victim) return;
            if (obj.victimBodyIndex == BodyCatalog.FindBodyIndex("CommandoBody"))
            {
                foreach (var soulmate in chirrSoulmates)
                {
                    if (!soulmate) continue;
                    var dur = 15f;
                    for (int i = 0; i < 5; i++)
                        soulmate.AddTimedBuff(RoR2Content.Buffs.BeetleJuice, dur);
                }
                commandoSoulmates.Remove(obj.victimBody);
            }
            else if (obj.victimBodyIndex == ChirrCore.bodyIndex)
            {
                chirrSoulmates.Remove(obj.victimBody);
            }
        }

        private static void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (!NetworkServer.active) return;
            if (!body || !body.master) return;
            if (body.bodyIndex == commandoBodyIndex && !commandoSoulmates.Contains(body))
            {
                commandoSoulmates.Add(body);
                return;
            }
            else if (body.bodyIndex == ChirrCore.bodyIndex)
            {
                var locPassiveESM = EntityStateMachine.FindByCustomName(body.gameObject, "Passive");
                var nextState = EntityStateCatalog.InstantiateState(locPassiveESM.mainStateType);
                locPassiveESM.SetInterruptState(nextState, EntityStates.InterruptPriority.Death);
            }
        }

        private static void SetupPassive()
        {
            void AddPassive(SkillDef skillDef)
            {
                skillDef.activationStateMachineName = "Passive";
                HG.ArrayUtils.ArrayAppend(ref passiveSkillFamily.variants, new SkillFamily.Variant
                {
                    skillDef = skillDef,
                    viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null),
                    unlockableDef = null
                });
                ContentAddition.AddSkillDef(skillDef);
            }
            ContentAddition.AddEntityState<ChirrCharMainVariableFly>(out _);
            ContentAddition.AddEntityState<VariableHoverOn>(out _);

            #region Creating Passive
            passiveSkill = bodyPrefab.AddComponent<GenericSkill>();
            passiveSkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (passiveSkillFamily as ScriptableObject).name = "DCSS2UChirrPassive";
            passiveSkill._skillFamily = passiveSkillFamily;
            ContentAddition.AddSkillFamily(passiveSkillFamily);

            var nsm = bodyPrefab.GetComponent<NetworkStateMachine>();
            passiveESM = bodyPrefab.AddComponent<EntityStateMachine>();
            //passiveESM.commonComponents = nsm.stateMachines[0].commonComponents;
            passiveESM.customName = "Passive";
            passiveESM.mainStateType = new EntityStates.SerializableEntityStateType(typeof(ESMCallSkillDefOnStartES));
            ContentAddition.AddEntityState<ESMCallSkillDefOnStartES>(out _);

            HG.ArrayUtils.ArrayAppend(ref nsm.stateMachines, in passiveESM);
            #endregion
            #region None
            passiveDefaultSD = ChirrMainHelpers.CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_TAKEFLIGHT");
            passiveDefaultSD.skillNameToken = skillLocator.passiveSkill.skillNameToken;
            passiveDefaultSD.skillName = passiveDefaultSD.skillNameToken;
            passiveDefaultSD.skillDescriptionToken = skillLocator.passiveSkill.skillDescriptionToken;
            passiveDefaultSD.icon = skillLocator.passiveSkill.icon;
            passiveDefaultSD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveTakeFlightES));
            skillLocator.passiveSkill.enabled = false;
            AddPassive(passiveDefaultSD);
            ContentAddition.AddEntityState<PassiveTakeFlightES>(out _);
            #endregion

            passiveEcosystemSD = ChirrMainHelpers.CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_STAGEBUFF");
            passiveEcosystemSD.icon = Assets.ChirrAssets.passiveEcosystemIcon;
            passiveEcosystemSD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveStageBuffES));
            AddPassive(passiveEcosystemSD);
            ContentAddition.AddEntityState<PassiveStageBuffES>(out _);

            passiveSnackiesSD = ChirrMainHelpers.CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_GLUTTON");
            passiveSnackiesSD.icon = Assets.ChirrAssets.passiveSnackiesIcon;
            passiveSnackiesSD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveSnackiesES));
            passiveSnackiesSD.keywordTokens = new string[]
            {
                "DESCLONE_SS2UCHIRR_SNACKY_KEYWORD"
            };
            AddPassive(passiveSnackiesSD);
            ContentAddition.AddEntityState<PassiveSnackiesES>(out _);

            passiveBunnySD = ChirrMainHelpers.CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_BUNNY");
            passiveBunnySD.icon = Assets.ChirrAssets.passiveLapinIcon;
            passiveBunnySD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveLapinES));
            AddPassive(passiveBunnySD);
            ContentAddition.AddEntityState<PassiveLapinES>(out _);

            passiveSoulmateSD = ChirrMainHelpers.CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_SOULMATE");
            passiveSoulmateSD.icon = Assets.ChirrAssets.passiveSoulmateIcon;
            passiveSoulmateSD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveSoulmateES));
            AddPassive(passiveSoulmateSD);
            ContentAddition.AddEntityState<PassiveSoulmateES>(out _);

            passiveMinerSD = ChirrMainHelpers.CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_MINER");
            passiveMinerSD.icon = Assets.ChirrAssets.passiveMinerIcon;
            passiveMinerSD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveMinerES));
            AddPassive(passiveMinerSD);
            ContentAddition.AddEntityState<PassiveMinerES>(out _);
        }
        private static void SetupPrimary()
        {
            var primarySF = skillLocator.primary.skillFamily;

            primaryDoubleTapSD = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDoubleTapSD.skillName = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP";
            (primaryDoubleTapSD as ScriptableObject).name = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP";
            primaryDoubleTapSD.skillNameToken = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP_NAME";
            primaryDoubleTapSD.skillDescriptionToken = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP_DESC";
            ChirrMainHelpers.CopySkillDefFields(primarySF.variants[0].skillDef, primaryDoubleTapSD, false);
            primaryDoubleTapSD.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFirePistol.asset").WaitForCompletion().icon;
            primaryDoubleTapSD.activationState = new EntityStates.SerializableEntityStateType(typeof(FireDoubleTap));
            primaryDoubleTapSD.skillDescriptionToken = "COMMANDO_PRIMARY_DESCRIPTION";
            primaryDoubleTapSD.stepCount = 2;
            primaryDoubleTapSD.stepGraceDuration = 0.1f;
            ContentAddition.AddSkillDef(primaryDoubleTapSD);
            ChirrMainHelpers.AddToSkillFamily(primaryDoubleTapSD, primarySF);
            ContentAddition.AddEntityState<FireDoubleTap>(out _);
        }
        private static void SetupSpecial()
        {
            specialTransformSD = ChirrMainHelpers.CreateSkillDef("DESCLONE_SS2UCHIRR_TRANSFORM");
            var tame = skillLocator.special.skillFamily.variants[0].skillDef;
            ChirrMainHelpers.CopySkillDefFields(tame, specialTransformSD, false);
            specialTransformSD.activationState = new EntityStates.SerializableEntityStateType(typeof(TransformEnemy));
            specialTransformSD.icon = Assets.ChirrAssets.specialTFIcon;
            ContentAddition.AddSkillDef(specialTransformSD);
            ChirrMainHelpers.AddToSkillFamily(specialTransformSD, skillLocator.special.skillFamily);
            ContentAddition.AddEntityState<TransformEnemy>(out _);

            specialEatSD = ChirrMainHelpers.CreateSkillDef("DESCLONE_SS2UCHIRR_VORE");
            ChirrMainHelpers.CopySkillDefFields(tame, specialEatSD, false);
            specialEatSD.activationState = new EntityStates.SerializableEntityStateType(typeof(EatEnemy));
            specialEatSD.icon = Assets.ChirrAssets.specialEatIcon;
            specialEatSD.keywordTokens = new string[]
            {
                "DESCLONE_SS2UCHIRR_SNACKY_KEYWORD"
            };
            ContentAddition.AddSkillDef(specialEatSD);
            ChirrMainHelpers.AddToSkillFamily(specialEatSD, skillLocator.special.skillFamily);
            ContentAddition.AddEntityState<EatEnemy>(out _);
        }
    }
}
