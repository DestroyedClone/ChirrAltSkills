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

        }

        public static StageBuffInfo defaultInfo = new StageBuffInfo()
        {
            stageIds = new string[0]
        };

        public static StageBuffInfo blackbeachInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "golemplains",
                "golemplains2",
                "itgolemplains"
            }
        };

        public static StageBuffInfo golemplainsInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "golemplains",
                "golemplains2",
                "itgolemplains"
            }
        };

        public static StageBuffInfo snowyforestInfo = new StageBuffInfo()
        {
            stageIds = new string[] { "snowyforst" }
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
            }
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
            }
        };
        public static StageBuffInfo rootjungleInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "rootjungle"
            }
        };
        public static StageBuffInfo skymeadowInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "skymeadow",
                "itskymeadow"
            }
        };
        public static StageBuffInfo moonInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "moon",
                "moon2",
                "itmoon"
            }
        };
        public static StageBuffInfo limboInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "limbo"
            }
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
            }
        };
        public static StageBuffInfo goldshoresInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "goldshores"
            }
        };
        public static StageBuffInfo arenaInfo = new StageBuffInfo()
        {
            stageIds = new string[]
            {
                "arena"
            }
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
            }
        };



        public static StageBuffInfo[] stageBuffInfos =
        {
            blackbeachInfo, golemplainsInfo
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
    }
}
