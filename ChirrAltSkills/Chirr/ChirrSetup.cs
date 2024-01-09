using ChirrAltSkills.Chirr.SkillDefs;
using ChirrAltSkills.Chirr.SkillDefs.Passive;
using ChirrAltSkills.Chirr.SkillDefs.Special;
using ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef;
using ChirrAltSkills.Chirr.States;
using ChirrAltSkills.Chirr.States.CharacterMains;
using ChirrAltSkills.Chirr.States.Primary;
using ChirrAltSkills.Chirr.States.Special;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using Starstorm2Unofficial.Survivors.Chirr;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr
{
    public class ChirrSetup
    {
        public static float cachedMass;

        public static GameObject bodyPrefab;
        public static SkillLocator skillLocator;

        public static EntityStateMachine passiveESM;
        public static GenericSkill passiveSkill;
        public static SkillFamily passiveSkillFamily;

        public static PassiveStageBuffSD passiveStageBuffSD;
        public static PassiveSnackiesPerStageSD passiveSnackiesSD;
        public static PassiveBunnySD passiveBunnySD;
        public static PassiveSoulmateSD passiveSoulmateSD;
        public static PassiveDiggerSD passiveDiggerSD;
        public static PassiveTakeFlightSD passiveDefaultSD;

        public static SteppedSkillDef primaryDoubleTapSD;

        public static TransformEnemySD specialTransformSD;
        public static TransformEnemySD specialTransformScepterSD;
        public static EatEnemySD specialEatSD;
        public static EatEnemySD specialEatScepterSD;

        public static List<CharacterBody> chirrSoulmates = new List<CharacterBody>();
        public static List<CharacterBody> commandoSoulmates = new List<CharacterBody>();

        public static BodyIndex commandoBodyIndex;

        public static void Init()
        {
            bodyPrefab = ChirrCore.chirrPrefab;
            cachedMass = bodyPrefab.GetComponent<CharacterMotor>().mass;
            skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            var chirrTracker = bodyPrefab.AddComponent<ChirrTracker>();
            chirrTracker.enabled = false;

            RevertBaseChanges();
            SetupPassive();
            SetupPrimary();
            SetupSpecial();

            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            GlobalEventManager.onCharacterDeathGlobal += Server_onCharacterDeath;
            Run.onServerGameOver += Run_ResetSoulmates;
            Stage.onServerStageComplete += Stage_ResetSoulmates;
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                commandoBodyIndex = BodyCatalog.FindBodyIndex("CommandoBody");
            };

            ChirrStageBuffInfo.Init();

            if (CASPlugin.modloaded_Scepter) ScepterSetup();
            //if (CASPlugin.modloaded_ClassicItems) ClassicScepterSetup();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void ScepterSetup()
        {
            ContentAddition.AddEntityState<TransformEnemyScepterES>(out _);
            specialTransformScepterSD = ChirrSetupHelpers.CreateSkillDef<TransformEnemySD>("DESCLONE_SS2UCHIRR_TRANSFORM_SCEPTER");
            ChirrSetupHelpers.CopySkillDefFields(specialTransformSD, specialTransformScepterSD, false);
            specialTransformScepterSD.activationState = new EntityStates.SerializableEntityStateType(typeof(TransformEnemyScepterES));
            specialTransformScepterSD.icon = Assets.ChirrAssets.specialTransformScepterIcon;
            ContentAddition.AddSkillDef(specialTransformScepterSD);
            //ChirrSetupHelpers.AddToSkillFamily(specialTransformScepterSD, skillLocator.special.skillFamily);

            ContentAddition.AddEntityState<EatEnemyScepterES>(out _);
            specialEatScepterSD = ChirrSetupHelpers.CreateSkillDef<EatEnemySD>("DESCLONE_SS2UCHIRR_EAT_SCEPTER");
            ChirrSetupHelpers.CopySkillDefFields(specialEatSD, specialEatScepterSD, false);
            specialEatScepterSD.activationState = new EntityStates.SerializableEntityStateType(typeof(EatEnemyScepterES));
            specialEatScepterSD.icon = Assets.ChirrAssets.specialEatScepterIcon;
            specialEatScepterSD.keywordTokens = new string[]
            {
                "DESCLONE_SS2UCHIRR_SNACKIES_KEYWORD"
            };
            ContentAddition.AddSkillDef(specialEatScepterSD);
            //ChirrSetupHelpers.AddToSkillFamily(specialEatSD, skillLocator.special.skillFamily);

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(specialTransformScepterSD, "SS2UChirrBody", specialTransformSD);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(specialEatScepterSD, "SS2UChirrBody", specialEatSD);
        }

        private static void RevertBaseChanges()
        {
            bodyPrefab.GetComponent<CharacterBody>().baseJumpPower = 15f;
            var esmList = bodyPrefab.GetComponents<EntityStateMachine>();
            foreach (var esm in esmList)
            {
                if (esm.customName == "Body")
                {
                    esm.mainStateType = new EntityStates.SerializableEntityStateType(typeof(ChirrCharMainVariableFly));
                    break;
                }
                //Debug.Log($"{esm.customName} : {esm.initialStateType} : {esm.mainStateType}");
            }
        }

        private static void ResetSoulmates()
        {
            chirrSoulmates.Clear();
            commandoSoulmates.Clear();
        }

        private static void Stage_ResetSoulmates(Stage obj)
        {
            ResetSoulmates();
        }

        private static void Run_ResetSoulmates(Run arg1, GameEndingDef arg2)
        {
            ResetSoulmates();
        }

        private static void Server_onCharacterDeath(DamageReport obj)
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
            passiveESM.initialStateType = new EntityStates.SerializableEntityStateType(typeof(Idle));
            passiveESM.mainStateType = new EntityStates.SerializableEntityStateType(typeof(Idle));

            HG.ArrayUtils.ArrayAppend(ref nsm.stateMachines, in passiveESM);

            #endregion Creating Passive

            #region Default

            passiveDefaultSD = ChirrSetupHelpers.CreateSkillDef<PassiveTakeFlightSD>("DESCLONE_SS2UCHIRR_PASSIVE_TAKEFLIGHT");
            passiveDefaultSD.skillNameToken = skillLocator.passiveSkill.skillNameToken;
            passiveDefaultSD.skillName = passiveDefaultSD.skillNameToken;
            passiveDefaultSD.skillDescriptionToken = skillLocator.passiveSkill.skillDescriptionToken;
            passiveDefaultSD.icon = skillLocator.passiveSkill.icon;
            skillLocator.passiveSkill.enabled = false;
            AddPassive(passiveDefaultSD);

            #endregion Default

            passiveStageBuffSD = ChirrSetupHelpers.CreateSkillDef<PassiveStageBuffSD>("DESCLONE_SS2UCHIRR_PASSIVE_STAGEBUFF");
            passiveStageBuffSD.icon = Assets.ChirrAssets.passiveStageBuffIcon;
            AddPassive(passiveStageBuffSD);

            passiveSnackiesSD = ChirrSetupHelpers.CreateSkillDef<PassiveSnackiesPerStageSD>("DESCLONE_SS2UCHIRR_PASSIVE_SNACKIESPERSTAGE");
            passiveSnackiesSD.icon = Assets.ChirrAssets.passiveSnackiesPerStageIcon;
            passiveSnackiesSD.keywordTokens = new string[]
            {
                "DESCLONE_SS2UCHIRR_SNACKIES_KEYWORD"
            };
            AddPassive(passiveSnackiesSD);

            passiveBunnySD = ChirrSetupHelpers.CreateSkillDef<PassiveBunnySD>("DESCLONE_SS2UCHIRR_PASSIVE_BUNNY");
            passiveBunnySD.icon = Assets.ChirrAssets.passiveBunnyIcon;
            AddPassive(passiveBunnySD);

            passiveSoulmateSD = ChirrSetupHelpers.CreateSkillDef<PassiveSoulmateSD>("DESCLONE_SS2UCHIRR_PASSIVE_SOULMATE");
            passiveSoulmateSD.icon = Assets.ChirrAssets.passiveSoulmateIcon;
            AddPassive(passiveSoulmateSD);

            passiveDiggerSD = ChirrSetupHelpers.CreateSkillDef<PassiveDiggerSD>("DESCLONE_SS2UCHIRR_PASSIVE_DIGGER");
            passiveDiggerSD.icon = Assets.ChirrAssets.passiveDiggerIcon;
            AddPassive(passiveDiggerSD);
        }

        private static void SetupPrimary()
        {
            var primarySF = skillLocator.primary.skillFamily;

            ContentAddition.AddEntityState<FireDoubleTapES>(out _);
            primaryDoubleTapSD = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDoubleTapSD.skillName = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP";
            (primaryDoubleTapSD as ScriptableObject).name = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP";
            primaryDoubleTapSD.skillNameToken = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP_NAME";
            primaryDoubleTapSD.skillDescriptionToken = "DESCLONE_SS2UCHIRR_PRIMARY_DOUBLETAP_DESC";
            ChirrSetupHelpers.CopySkillDefFields(primarySF.variants[0].skillDef, primaryDoubleTapSD, false);
            primaryDoubleTapSD.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyFirePistol.asset").WaitForCompletion().icon;
            primaryDoubleTapSD.activationState = new EntityStates.SerializableEntityStateType(typeof(FireDoubleTapES));
            primaryDoubleTapSD.skillDescriptionToken = "COMMANDO_PRIMARY_DESCRIPTION";
            primaryDoubleTapSD.stepCount = 2;
            primaryDoubleTapSD.stepGraceDuration = 0.1f;
            ContentAddition.AddSkillDef(primaryDoubleTapSD);
            ChirrSetupHelpers.AddToSkillFamily(primaryDoubleTapSD, primarySF);
        }

        private static void SetupSpecial()
        {
            ContentAddition.AddEntityState<TransformEnemyES>(out _);
            specialTransformSD = ChirrSetupHelpers.CreateSkillDef<TransformEnemySD>("DESCLONE_SS2UCHIRR_TRANSFORM");
            var tame = skillLocator.special.skillFamily.variants[0].skillDef;
            ChirrSetupHelpers.CopySkillDefFields(tame, specialTransformSD, false);
            specialTransformSD.activationState = new EntityStates.SerializableEntityStateType(typeof(TransformEnemyES));
            specialTransformSD.icon = Assets.ChirrAssets.specialTFIcon;
            ContentAddition.AddSkillDef(specialTransformSD);
            ChirrSetupHelpers.AddToSkillFamily(specialTransformSD, skillLocator.special.skillFamily);

            ContentAddition.AddEntityState<EatEnemyES>(out _);
            specialEatSD = ChirrSetupHelpers.CreateSkillDef<EatEnemySD>("DESCLONE_SS2UCHIRR_EAT");
            ChirrSetupHelpers.CopySkillDefFields(tame, specialEatSD, false);
            specialEatSD.activationState = new EntityStates.SerializableEntityStateType(typeof(EatEnemyES));
            specialEatSD.icon = Assets.ChirrAssets.specialEatIcon;
            specialEatSD.keywordTokens = new string[]
            {
                "DESCLONE_SS2UCHIRR_SNACKIES_KEYWORD"
            };
            ContentAddition.AddSkillDef(specialEatSD);
            ChirrSetupHelpers.AddToSkillFamily(specialEatSD, skillLocator.special.skillFamily);
        }
    }
}