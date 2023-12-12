using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChirrAltSkills.Chirr
{
    internal class ChirrStageBuffInfo
    {
        internal struct StageBuffInfo
        {
            public string[] stageIds;
            public float armor;
            public const float maxArmor = 5;
            public const float medArmor = 3;
            public const float minArmor = 1;

            public float movespeed;
            public const float minMS = 0.5f;
            public const float medMS = 0.3f;
            public const float maxMS = 0.1f;

            public float attackspeed;
            



            public void Apply(CharacterBody characterBody)
            {
                characterBody.baseArmor += armor;
                characterBody.baseAttackSpeed += attackspeed;
                characterBody.baseMoveSpeed += movespeed;
            }
        }

        public static StageBuffInfo defaultInfo = new StageBuffInfo()
        {
            stageIds = new string[0]
        };

        public static StageBuffInfo blackbeachInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "blackbeach",
                "blackbeach2",
                "itblackbeach"
            },
            movespeed = 0.5f,
            attackspeed = 0.1f,
        };

        public static StageBuffInfo golemplainsInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "golemplains",
                "golemplains2",
                "itgolemplains"
            },
            movespeed = 0.1f,
            attackspeed = 0.1f,
            armor = StageBuffInfo.medArmor
        };

        public static StageBuffInfo snowyforestInfo = new StageBuffInfo()
        {
            stageIds = new string[] { "snowyforest" },
        };

        public static StageBuffInfo goolakeInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "goolake",
                "itgoolake"
            }
        };

        public static StageBuffInfo foggyswampInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "foggyswamp"
            },
            armor = StageBuffInfo.maxArmor
        };

        public static StageBuffInfo ancientloftInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "ancientloft",
                "itancientloft"
            }
        };

        public static StageBuffInfo frozenwallInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "frozenwall",
                "itfrozenwall"
            }
        };

        public static StageBuffInfo wispgraveyardInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "wispgraveyard"
            },
            armor = StageBuffInfo.medArmor
        };

        public static StageBuffInfo sulfurpoolsInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "sulfurpools"
            }
        };
        public static StageBuffInfo dampcaveInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "dampcavesimple",
                "itdampcave"
            }
        };
        public static StageBuffInfo shipgraveyardInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "shipgraveyard"
            },
            armor = StageBuffInfo.medArmor
        };
        public static StageBuffInfo rootjungleInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "rootjungle"
            },
            armor = StageBuffInfo.maxArmor,
        };
        public static StageBuffInfo skymeadowInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "skymeadow",
                "itskymeadow"
            },
            armor = StageBuffInfo.maxArmor,
        };
        public static StageBuffInfo moonInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "moon",
                "moon2",
                "itmoon"
            },
            armor = StageBuffInfo.maxArmor,
        };
        public static StageBuffInfo limboInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "limbo"
            },
            armor = StageBuffInfo.maxArmor,
        };
        public static StageBuffInfo bazaarInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "bazaar"
            }
        };
        public static StageBuffInfo artifactworldInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "artifactworld"
            },
            armor = StageBuffInfo.maxArmor,
        };
        public static StageBuffInfo goldshoresInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "goldshores"
            },
            armor = StageBuffInfo.maxArmor,
        };
        public static StageBuffInfo arenaInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "arena"
            },
            armor = StageBuffInfo.maxArmor,
        };
        public static StageBuffInfo voidstageInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "voidstage"
            }
        };
        public static StageBuffInfo voidraidInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "voidraid"
            },
            armor = StageBuffInfo.maxArmor,
        };



        public static StageBuffInfo[] stageBuffInfos =
        {
            blackbeachInfo, golemplainsInfo, snowyforestInfo, goolakeInfo, foggyswampInfo, ancientloftInfo, frozenwallInfo, wispgraveyardInfo, sulfurpoolsInfo, dampcaveInfo, shipgraveyardInfo, rootjungleInfo, skymeadowInfo, moonInfo, limboInfo, bazaarInfo, artifactworldInfo, goldshoresInfo, arenaInfo, voidstageInfo, voidraidInfo
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
