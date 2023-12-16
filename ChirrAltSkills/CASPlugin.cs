using System;
using System.Security.Permissions;
using System.Security;
using BepInEx;
using ChirrAltSkills.Chirr;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[module: UnverifiableCode]
namespace ChirrAltSkills
{
    [BepInDependency("com.ChirrLover.Starstorm2Unofficial", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rob.DiggerUnearthed", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin("com.DestroyedClone.ChirrAltSkills", "ChirrSS2U Alt Skills", "0.0.1")]
    public class CASPlugin : BaseUnityPlugin
    {
        internal static BepInEx.Logging.ManualLogSource _logger;
        internal static BepInEx.Configuration.ConfigFile _config;
        const string LastDllVersion = "0.16.4";
        public static PluginInfo PInfo { get; set; }

        public static bool modloaded_Miner = false;

        public void Awake()
        {
            PInfo = Info;
            _logger = Logger;
            _config = Config;

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.DiggerUnearthed"))
                modloaded_Miner = true;

            Assets.Init();
            Buffs.Init();
            ChirrMain.Init();
        }
    }
}
