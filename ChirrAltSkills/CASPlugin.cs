using BepInEx;
using ChirrAltSkills.Chirr;
using R2API.Networking;
using System.Security;
using System.Security.Permissions;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[module: UnverifiableCode]
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace ChirrAltSkills
{
    [BepInDependency("com.ChirrLover.Starstorm2Unofficial", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rob.DiggerUnearthed", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin("com.DestroyedClone.ChirrAltSkills", "ChirrSS2U Alt Skills", "0.0.2")]
    [BepInDependency(NetworkingAPI.PluginGUID)]
    public class CASPlugin : BaseUnityPlugin
    {
        internal static BepInEx.Logging.ManualLogSource _logger;
        internal static BepInEx.Configuration.ConfigFile _config;
        internal const string LastDllVersion = "0.16.4";
        public static PluginInfo PInfo { get; set; }

        public static bool modloaded_Digger = false;
        public static bool modloaded_ClassicItems = false;
        public static bool modloaded_Scepter = false;

        public void Awake()
        {
            PInfo = Info;
            _logger = Logger;
            _config = Config;

            modloaded_Digger = (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.DiggerUnearthed"));
            modloaded_ClassicItems = (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems"));
            modloaded_Scepter = (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter"));

            ConfigSetup.Init();
            Assets.Init();
            Buffs.Init();
            DamageTypes.Init();
            ChirrSetup.Init();
            ChirrCommands.AddChatCommands();
            Networking.Init();
        }
    }
}