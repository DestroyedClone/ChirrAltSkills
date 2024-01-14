using RoR2;
using System.Collections.Generic;
using UnityEngine;
using
     System;
using UnityEngine.Networking;
using R2API.Networking;
using System.Security.Principal;
using R2API.Networking.Interfaces;
namespace ChirrAltSkills.Chirr
{
    internal class ChirrStageBuffInfo
    {
        /* Philosophy:
         * Grassy open stages are desirable, no armor but High Speed!
         * Sun lets me photosynthesize, Increase Regen! Increase Health!
         * Chirr is Chirr, so she'll at minimum get minStats for everything.
         * Boss stages get maxed.
         *
         * Armor:
         *  Tar and Corrosion is maxed, REALLY gotta prevent that.
         *  Cold is medium, biologically reasonable
         * ...
         */

        internal struct StageBuffInfo
        {
            public string[] stageIds;
            public float armor;
            public const float minArmor = 2;
            public const float medArmor = 4;
            public const float maxArmor = 6;

            public float movespeed;
            public const float minMS = 0.1f;
            public const float medMS = 0.3f;
            public const float maxMS = 0.5f;

            public float attackspeed;
            public const float minAS = 0.1f;
            public const float medAS = 0.3f;
            public const float maxAS = 0.5f;

            public float regen;
            public const float minRegen = 0.2f;
            public const float medRegen = 0.4f;
            public const float maxRegen = 0.6f;

            public float hover;
            public const float minHover = 0.25f;
            public const float medHover = 0.5f;
            public const float maxHover = 1.0f;

            public const int moveSpeedMaxStagesCleared = 10;
            public const float stageMultiplierStagesCleared = 0.2f;

            //this is NOT PERFORMANT
            //IM CALLING IT MULTIPLE TIMES
            public void GetResultingStatChanges(out float armorMult, out float asMult, out float msMult, out float regenMult, out string currentStageToken)
            {
                var stageClearCount = Mathf.Max(0, Run.instance.stageClearCount);

                var multiplier = 1 + stageClearCount * StageBuffInfo.stageMultiplierStagesCleared;
                var multiplierMS = 1 + Mathf.Min(20, stageClearCount) * StageBuffInfo.stageMultiplierStagesCleared;

                armorMult = armor * multiplier;
                asMult = attackspeed * multiplier;
                msMult = movespeed * multiplierMS;
                regenMult = regen * multiplier;
                currentStageToken = SceneCatalog.GetSceneDefForCurrentScene().nameToken;
            }

            public void Apply(CharacterBody characterBody)
            {
                GetResultingStatChanges(out float armorMult, out float asMult, out float msMult, out float regenMult, out string _);

                characterBody.baseArmor += armorMult;
                characterBody.baseAttackSpeed += asMult;
                characterBody.baseMoveSpeed += msMult;
                characterBody.baseRegen += regenMult;
                characterBody.MarkAllStatsDirty();
            }

            public void SendChatAnnouncementMessage(CharacterBody characterBody)
            {
                if (!NetworkServer.active) return;
                Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage()
                {
                    baseToken = "DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_0",
                    subjectCharacterBodyGameObject = characterBody.gameObject,
                    paramTokens = new string[]
                    {
                        SceneCatalog.GetSceneDefForCurrentScene().nameToken
                    }
                });
            }

            public string GetChangeMessage()
            {
                GetResultingStatChanges(out float armorMult, out float asMult, out float msMult, out float regenMult, out string _);

                string g(string token)
                {
                    return Language.GetString(token);
                }
                string formattedStatShow = Language.GetStringFormatted("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_1",
                    g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_ARMOR"), armorMult,
                    g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_MOVESPEED"), string.Format("{0:P}", asMult),
                    g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_ATTACKSPEED"), string.Format("{0:P}", msMult),
                    g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_REGEN"), regenMult,
                    g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_HOVER"), string.Format("{0:P}", hover));
                return formattedStatShow;
            }

            public void SendChatChangeMessage(NetworkInstanceId targetNetId)
            {
                if (!NetworkServer.active) return;
                GetResultingStatChanges(out float armorMult, out float asMult, out float msMult, out float regenMult, out string _);
                //new Networking.SendToAllSimpleChatMessage("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_1", combined).Send(NetworkDestination.Clients);
                new Networking.SendToClient_ChirrStatInfo(targetNetId, armorMult, asMult, msMult, regenMult, hover).Send(NetworkDestination.Clients);
            }

            public void LogChangeMessage()
            {
                GetResultingStatChanges(out float armorMult, out float asMult, out float msMult, out float regenMult, out string _);
                var array = new string[]
                {
                    Language.GetString("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_ARMOR"),
                    armorMult.ToString(),
                    Language.GetString("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_MOVESPEED"),
                    string.Format("{0:P}", asMult),
                    Language.GetString("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_ATTACKSPEED"),
                    string.Format("{0:P}", msMult),
                    Language.GetString("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_REGEN"),
                    regenMult.ToString(),
                    Language.GetString("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_HOVER"),
                    string.Format("{0:P}", hover),
                };
                Debug.Log(Language.GetStringFormatted("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_1", array));
            }
        }

        public static float GetStageHoverMultiplier()
        {
            return GetCurrentStageBuffInfo().hover;
        }

        public static StageBuffInfo GetCurrentStageBuffInfo()
        {
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var info = GetStageBuffInfo(currentScene.name);
            return info;
        }

        public static StageBuffInfo defaultInfo = new StageBuffInfo()
        {
            stageIds = new string[0],
            armor = StageBuffInfo.minArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.minRegen,
            hover = StageBuffInfo.medHover,
        };

        public static StageBuffInfo blackbeachInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "blackbeach",
                "blackbeach2",
                "itblackbeach"
            },
            armor = StageBuffInfo.minArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.medRegen,
            hover = StageBuffInfo.maxHover,
        };

        public static StageBuffInfo golemplainsInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "golemplains",
                "golemplains2",
                "itgolemplains"
            },
            armor = StageBuffInfo.medArmor,
            movespeed = StageBuffInfo.medMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.medRegen,
            hover = StageBuffInfo.maxHover,
        };

        public static StageBuffInfo snowyforestInfo = new StageBuffInfo()
        {
            stageIds = new string[] { "snowyforest" },
            armor = StageBuffInfo.medArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.minRegen,
            hover = StageBuffInfo.maxHover,
        };

        public static StageBuffInfo goolakeInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "goolake",
                "itgoolake"
            },
            armor = StageBuffInfo.maxArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.minRegen,
            hover = StageBuffInfo.minHover,
        };

        public static StageBuffInfo foggyswampInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "foggyswamp"
            },
            armor = StageBuffInfo.medArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.maxRegen,
            hover = StageBuffInfo.medHover,
        };

        public static StageBuffInfo ancientloftInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "ancientloft",
                "itancientloft"
            },
            armor = StageBuffInfo.maxArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.minRegen,
            hover = StageBuffInfo.minHover,
        };

        public static StageBuffInfo frozenwallInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "frozenwall",
                "itfrozenwall"
            },
            armor = StageBuffInfo.medArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.minRegen,
            hover = StageBuffInfo.medHover,
        };

        public static StageBuffInfo wispgraveyardInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "wispgraveyard"
            },
            armor = StageBuffInfo.medArmor,
            movespeed = StageBuffInfo.maxMS,
            attackspeed = StageBuffInfo.medAS,
            regen = StageBuffInfo.medRegen,
            hover = StageBuffInfo.maxHover,
        };

        public static StageBuffInfo sulfurpoolsInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "sulfurpools"
            },
            armor = StageBuffInfo.maxArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.minRegen,
            hover = StageBuffInfo.minHover,
        };

        public static StageBuffInfo dampcaveInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "dampcavesimple",
                "itdampcave"
            },
            armor = StageBuffInfo.minArmor,
            movespeed = StageBuffInfo.medMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.minRegen,
            hover = StageBuffInfo.medHover,
        };

        public static StageBuffInfo shipgraveyardInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "shipgraveyard"
            },
            armor = StageBuffInfo.minArmor,
            movespeed = StageBuffInfo.maxMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.maxRegen,
            hover = StageBuffInfo.maxHover,
        };

        public static StageBuffInfo rootjungleInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "rootjungle"
            },
            armor = StageBuffInfo.minArmor,
            movespeed = StageBuffInfo.maxMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.maxRegen,
            hover = StageBuffInfo.maxHover,
        };

        public static StageBuffInfo skymeadowInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "skymeadow",
                "itskymeadow"
            },
            armor = StageBuffInfo.minArmor,
            movespeed = StageBuffInfo.maxMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.maxRegen,
            hover = StageBuffInfo.maxHover,
        };

        public static StageBuffInfo bossInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "moon",
                "moon2",
                "itmoon",
                "limbo",
                "artifactworld",
                "goldshores",
                "voidraid",
                "arena"
            },
            armor = StageBuffInfo.maxArmor,
            movespeed = StageBuffInfo.maxMS,
            attackspeed = StageBuffInfo.maxAS,
            regen = StageBuffInfo.maxRegen,
            hover = StageBuffInfo.maxHover,
        };

        public static StageBuffInfo bazaarInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "bazaar"
            },
            //Left Intentionally Blank
        };

        public static StageBuffInfo voidstageInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "voidstage"
            },
            armor = StageBuffInfo.medArmor,
            movespeed = StageBuffInfo.medMS,
            attackspeed = StageBuffInfo.medAS,
            regen = StageBuffInfo.medRegen,
            hover = StageBuffInfo.medHover,
        };

        public static StageBuffInfo[] stageBuffInfos =
        {
            blackbeachInfo, golemplainsInfo, snowyforestInfo, goolakeInfo, foggyswampInfo, ancientloftInfo, frozenwallInfo, wispgraveyardInfo, sulfurpoolsInfo, dampcaveInfo, shipgraveyardInfo, rootjungleInfo, skymeadowInfo, bazaarInfo, voidstageInfo, bossInfo
        };

        public static Dictionary<string, StageBuffInfo> stageBuffInfoDict = new Dictionary<string, StageBuffInfo>();

        public static void Init()
        {
            GenerateDictionary();
        }

        public static void GenerateDictionary()
        {
            foreach (var info in stageBuffInfos)
            {
                foreach (var stageId in info.stageIds)
                {
                    stageBuffInfoDict.Add(stageId, info);
                }
            }
        }

        public static StageBuffInfo GetStageBuffInfo(string stageName)
        {
            return stageBuffInfoDict.TryGetValue(stageName, out var stageBuffInfo) ? stageBuffInfo : defaultInfo;
        }

        /*public static bool GetStageBuffInfo(string stageName, out StageBuffInfo stageBuffInfo)
        {
            stageBuffInfo = GetStageBuffInfo(stageName);
            return stageBuffInfo.stageIds.;
        }*/
    }
}