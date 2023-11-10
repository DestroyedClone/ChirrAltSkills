using RoR2.Skills;
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

        public static SkillDef primaryThornSD;
        public static SteppedSkillDef primaryDoubleTapSD;

        public static SkillDef specialTransformSD;
        public static SkillDef specialEatSD;

        public static List<CharacterBody> chirrSoulmates = new List<CharacterBody>();
        public static List<CharacterBody> commandoSoulmates = new List<CharacterBody> ();

        public static void Init()
        {
            bodyPrefab = ChirrCore.chirrPrefab;
            cachedMass = bodyPrefab.GetComponent<CharacterMotor>().mass;
            skillLocator = bodyPrefab.GetComponent<SkillLocator>();
            SetupPassive();
            SetupPrimary();
            SetupSpecial();

            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
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
            if (body.bodyIndex == BodyCatalog.FindBodyIndex("CommandoBody"))
            {
                commandoSoulmates.Add(body);
                return;
            }
            else if (body.bodyIndex == ChirrCore.bodyIndex)
            {

            }
        }

        public static void CopySkillDefFields(SkillDef parent, SkillDef child, bool copyIdentifiers)
        {
            child.activationState = parent.activationState;
            child.activationStateMachineName = parent.activationStateMachineName;
            child.baseMaxStock = parent.baseMaxStock;
            child.baseRechargeInterval = parent.baseRechargeInterval;
            child.beginSkillCooldownOnSkillEnd = parent.beginSkillCooldownOnSkillEnd;
            child.canceledFromSprinting = parent.canceledFromSprinting;
            child.cancelSprintingOnActivation = parent.cancelSprintingOnActivation;
            child.dontAllowPastMaxStocks = parent.dontAllowPastMaxStocks;
            child.forceSprintDuringState = parent.forceSprintDuringState;
            child.fullRestockOnAssign = parent.fullRestockOnAssign;
            child.icon = parent.icon;
            child.interruptPriority = parent.interruptPriority;
            child.isCombatSkill = parent.isCombatSkill;
            child.keywordTokens = parent.keywordTokens;
            child.mustKeyPress = parent.mustKeyPress;
            child.rechargeStock = parent.rechargeStock;
            child.requiredStock = parent.requiredStock;
            child.resetCooldownTimerOnUse = parent.resetCooldownTimerOnUse;
            if (copyIdentifiers)
            {
                child.skillDescriptionToken = parent.skillDescriptionToken;
                child.skillName = parent.skillName;
                child.skillNameToken = parent.skillNameToken;
            }
            child.stockToConsume = parent.stockToConsume;
        }

        public static SkillDef CreateSkillDef(string token)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.skillName = token;
            (skillDef as ScriptableObject).name = token;
            skillDef.skillNameToken = token+"_NAME";
            skillDef.skillDescriptionToken = token + "_DESC";
            return skillDef;
        }

        public static SkillDef CreateSkillDef(string token, SkillFamily family, UnlockableDef unlockableDef = null)
        {
            var sd = CreateSkillDef(token);
            AddToSkillFamily(sd, family, unlockableDef);
            return sd;
        }

        public static void AddToSkillFamily(SkillDef skillDef, SkillFamily skillFamily, UnlockableDef unlockableDef = null)
        {
            HG.ArrayUtils.ArrayAppend(ref skillFamily.variants, new SkillFamily.Variant
            {
                skillDef = skillDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null),
                unlockableDef = unlockableDef
            });
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
            #region Creating Passive
            passiveSkill = bodyPrefab.AddComponent<GenericSkill>();
            passiveSkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (passiveSkillFamily as ScriptableObject).name = "DCSS2UChirrPassive";
            passiveSkill._skillFamily = passiveSkillFamily;
            ContentAddition.AddSkillFamily(passiveSkillFamily);

            var nsm = bodyPrefab.GetComponent<NetworkStateMachine>();
            passiveESM = bodyPrefab.AddComponent<EntityStateMachine>();
            passiveESM.commonComponents = nsm.stateMachines[0].commonComponents;
            passiveESM.customName = "Passive";
            //passiveESM.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EmptyES));

            HG.ArrayUtils.ArrayAppend(ref nsm.stateMachines, in passiveESM);
            #endregion
            #region None
            passiveDefaultSD = CreateSkillDef("Default");
            passiveDefaultSD.skillNameToken = skillLocator.passiveSkill.skillNameToken;
            passiveDefaultSD.skillName = passiveDefaultSD.skillNameToken;
            passiveDefaultSD.skillDescriptionToken = skillLocator.passiveSkill.skillDescriptionToken;
            passiveDefaultSD.icon = skillLocator.passiveSkill.icon;
            passiveDefaultSD.activationState = new EntityStates.SerializableEntityStateType(typeof(EmptyES));
            skillLocator.passiveSkill.enabled = false;
            AddPassive(passiveDefaultSD);
            #endregion

            passiveEcosystemSD = CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_STAGEBUFF");
            passiveEcosystemSD.icon = Assets.ChirrAssets.passiveEcosystemIcon;
            passiveEcosystemSD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveStageBuffES));
            AddPassive(passiveEcosystemSD);

            passiveSnackiesSD = CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_GLUTTON");
            passiveSnackiesSD.icon = Assets.ChirrAssets.passiveSnackiesIcon;
            passiveSnackiesSD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveSnackiesES));
            passiveSnackiesSD.keywordTokens = new string[]
            {
                "DESCLONE_SS2UCHIRR_SNACKY_KEYWORD"
            };
            AddPassive(passiveSnackiesSD);

            passiveBunnySD = CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_BUNNY");
            passiveBunnySD.icon = Assets.ChirrAssets.passiveLapinIcon;
            passiveBunnySD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveLapinES));
            AddPassive(passiveBunnySD);

            passiveSoulmateSD = CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_SOULMATE");
            passiveSoulmateSD.icon = Assets.ChirrAssets.passiveSoulmateIcon;
            passiveSoulmateSD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveSoulmateES));
            AddPassive(passiveSoulmateSD);

            passiveMinerSD = CreateSkillDef("DESCLONE_SS2UCHIRR_PASSIVE_MINER");
            passiveMinerSD.icon = Assets.ChirrAssets.passiveMinerIcon;
            passiveMinerSD.activationState = new EntityStates.SerializableEntityStateType(typeof(PassiveMinerES));
            AddPassive(passiveMinerSD);
        }
        private static void SetupPrimary()
        {
            var primarySF = skillLocator.primary.skillFamily;

            #region Needle
            primaryThornSD = CreateSkillDef("DESCLONE_SS2UCHIRR_PRIMARY_NEEDLE");
            CopySkillDefFields(primarySF.variants[0].skillDef, primaryThornSD, false);
            primaryThornSD.activationState = new EntityStates.SerializableEntityStateType(typeof(FireThorn));
            //primaryThornSD.icon = Assets.ChirrAssets.
            ContentAddition.AddSkillDef(primaryThornSD);
            AddToSkillFamily(primaryThornSD, primarySF);
            #endregion

            primaryDoubleTapSD = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDoubleTapSD.skillName = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP";
            (primaryDoubleTapSD as ScriptableObject).name = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP";
            primaryDoubleTapSD.skillNameToken = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP_NAME";
            primaryDoubleTapSD.skillDescriptionToken = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP_DESC";
            CopySkillDefFields(primarySF.variants[0].skillDef, primaryDoubleTapSD, false);
            primaryDoubleTapSD.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFirePistol.asset").WaitForCompletion().icon;
            primaryDoubleTapSD.activationState = new EntityStates.SerializableEntityStateType(typeof(FireDoubleTap));
            primaryDoubleTapSD.skillDescriptionToken = "COMMANDO_PRIMARY_DESCRIPTION";
            ContentAddition.AddSkillDef(primaryDoubleTapSD);
            AddToSkillFamily(primaryDoubleTapSD, primarySF);
        }
        private static void SetupSpecial()
        {
            specialTransformSD = CreateSkillDef("DESCLONE_SS2UCHIRR_TRANSFORM");
            var tame = skillLocator.special.skillFamily.variants[0].skillDef;
            CopySkillDefFields(tame, specialTransformSD, false);
            specialTransformSD.activationState = new EntityStates.SerializableEntityStateType(typeof(TransformEnemy));
            ContentAddition.AddSkillDef(specialTransformSD);

            HG.ArrayUtils.ArrayAppend(ref skillLocator.special.skillFamily.variants, new SkillFamily.Variant
            {
                skillDef = specialTransformSD,
                viewableNode = new ViewablesCatalog.Node(specialTransformSD.skillNameToken, false, null),
                unlockableDef = null
            });

            specialEatSD = CreateSkillDef("DESCLONE_SS2UCHIRR_VORE");
            CopySkillDefFields(tame, specialEatSD, false);
            specialEatSD.activationState = new EntityStates.SerializableEntityStateType(typeof(EatEnemy));
            specialEatSD.keywordTokens = new string[]
            {
                "DESCLONE_SS2UCHIRR_SNACKY_KEYWORD"
            };
            ContentAddition.AddSkillDef(specialEatSD);

            HG.ArrayUtils.ArrayAppend(ref skillLocator.special.skillFamily.variants, new SkillFamily.Variant
            {
                skillDef = specialEatSD,
                viewableNode = new ViewablesCatalog.Node(specialEatSD.skillNameToken, false, null),
                unlockableDef = null
            });
        }
    }
}
