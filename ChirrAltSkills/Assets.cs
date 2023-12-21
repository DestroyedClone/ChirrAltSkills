using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ChirrAltSkills
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle = null;

        public const string bundleName = "chirraltskillsassetbundle";

        public static string AssetBundlePath
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(CASPlugin.PInfo.Location), bundleName);
            }
        }

        public static T LoadAddressable<T>(string path)
        {
            return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
        }

        public static void Init()
        {
            PopulateAssets();
            ChirrAssets.Init();
        }

        public static void PopulateAssets()
        {
            mainAssetBundle = AssetBundle.LoadFromFile(AssetBundlePath);
        }

        public static class ChirrAssets
        {
            public static Sprite passiveEcosystemIcon;
            public static Sprite passiveSnackiesIcon;
            public static Sprite passiveLapinIcon;
            public static Sprite passiveSoulmateIcon;
            public static Sprite passiveMinerIcon;

            public static Sprite primaryLeafBladeIcon;
            public static Sprite secondaryHeadbuttWeakenIcon;

            public static Sprite specialTFIcon;
            public static Sprite specialTFIconScepter;
            public static Sprite specialEatIcon;
            public static Sprite specialEatIconScepter;

            public static Sprite buffSnackiesIcon;
            public static Sprite buffSoulmateIcon;
            public static Sprite buffGoldrushIcon;

            public static Sprite LoadSprite(string path)
            {
                return mainAssetBundle.LoadAsset<Sprite>(path);
            }

            public static void Init()
            {
                passiveEcosystemIcon = LoadSprite("StageBuffIcon");
                passiveSnackiesIcon = LoadSprite("GluttonyIcon");
                passiveLapinIcon = LoadSprite("LapinIcon");
                passiveSoulmateIcon = LoadSprite("SoulmatePassiveIcon");
                passiveMinerIcon = LoadSprite("MinerIcon");
                //NeedleIcon
                specialTFIcon = LoadSprite("ChirrTFIcon");
                specialTFIconScepter = LoadSprite("ChirrTFScepterIcon");
                specialEatIcon = LoadSprite("ChirrEatIcon");
                specialEatIconScepter = LoadSprite("ChirrEatScepterIcon");

                buffSnackiesIcon = passiveSnackiesIcon;
                buffSoulmateIcon = LoadSprite("SoulmateBuffIcon");
                buffGoldrushIcon = passiveMinerIcon;
            }
        }
    }
}