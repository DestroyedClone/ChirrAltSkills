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
            public static Sprite passiveStageBuffIcon;
            public static Sprite passiveSnackiesPerStageIcon;
            public static Sprite passiveBunnyIcon;
            public static Sprite passiveSoulmateIcon;
            public static Sprite passiveDiggerIcon;

            public static Sprite primaryLeafBladeIcon;
            public static Sprite secondaryHeadbuttWeakenIcon;

            public static Sprite specialTFIcon;
            public static Sprite specialTFIconScepter;
            public static Sprite specialEatIcon;
            public static Sprite specialEatIconScepter;

            public static Sprite buffIndulgenceIcon;
            public static Sprite buffSnackiesIcon;
            public static Sprite buffSoulmateIcon;
            public static Sprite buffAdrenalineIcon;
            public static Sprite buffHoverDurationIcon;
            public static Sprite buffBunnyJumpIcon;

            public static Sprite LoadSprite(string path)
            {
                return mainAssetBundle.LoadAsset<Sprite>(path);
            }

            public static void Init()
            {
                passiveStageBuffIcon = LoadSprite("PassiveStageBuffIcon");
                passiveSnackiesPerStageIcon = LoadSprite("PassiveSnackiesPerStageIcon");
                passiveBunnyIcon = LoadSprite("PassiveBunnyIcon");
                passiveSoulmateIcon = LoadSprite("PassiveSoulmateIcon");
                passiveDiggerIcon = LoadSprite("PassiveDiggerIcon");
                //NeedleIcon

                specialTFIcon = LoadSprite("SpecialTransformEnemyIcon");
                specialTFIconScepter = LoadSprite("SpecialTransformEnemyScepterIcon");
                specialEatIcon = LoadSprite("SpecialEatEnemyIcon");
                specialEatIconScepter = LoadSprite("SpecialEatEnemyScepterIcon");

                buffSnackiesIcon = LoadSprite("BuffSnackiesIcon");
                buffIndulgenceIcon = buffSnackiesIcon;
                buffSoulmateIcon = LoadSprite("BuffSoulmateIcon");
                buffAdrenalineIcon = LoadSprite("BuffAdrenalineIcon");
                buffHoverDurationIcon = LoadSprite("BuffHoverDurationIcon");
                buffBunnyJumpIcon = LoadSprite("BuffBunnyJumpIcon");
            }
        }
    }
}