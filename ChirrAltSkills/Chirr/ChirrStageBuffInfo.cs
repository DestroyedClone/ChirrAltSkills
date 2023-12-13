using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
            public const float minArmor = 1;
            public const float medArmor = 3;
            public const float maxArmor = 5;

            public float movespeed;
            public const float minMS = 0.1f;
            public const float medMS = 0.3f;
            public const float maxMS = 0.5f;

            public float attackspeed;
            public const float minAS = 0.1f;
            public const float medAS = 0.3f;
            public const float maxAS = 0.5f;

            public float regen;
            public const float minRegen = 0.1f;
            public const float medRegen = 0.3f;
            public const float maxRegen = 0.5f;

            public const int moveSpeedMaxStagesCleared = 10;
            public const float stageMultiplierStagesCleared = 0.1f;

            public void Apply(CharacterBody characterBody)
            {
                var stageClearCount = Run.instance.stageClearCount;
                if (stageClearCount < 0) return;

                var multiplier = 1 + stageClearCount * stageMultiplierStagesCleared;
                var multiplierMS = 1 + Mathf.Min(20, stageClearCount) * stageMultiplierStagesCleared;

                characterBody.baseArmor += armor * multiplier;
                characterBody.baseAttackSpeed += attackspeed * multiplier;
                characterBody.baseMoveSpeed += movespeed * multiplierMS;
                characterBody.baseRegen += regen * multiplier;
                characterBody.MarkAllStatsDirty();
            }
        }

        public static StageBuffInfo defaultInfo = new StageBuffInfo()
        {
            stageIds = new string[0],
            armor = StageBuffInfo.minArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.minRegen,
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
        };

        public static StageBuffInfo snowyforestInfo = new StageBuffInfo()
        {
            stageIds = new string[] { "snowyforest" },
            armor = StageBuffInfo.medArmor,
            movespeed = StageBuffInfo.minMS,
            attackspeed = StageBuffInfo.minAS,
            regen = StageBuffInfo.minRegen,
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
