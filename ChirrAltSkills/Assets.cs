using Starstorm2Unofficial.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;
using R2API;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using RoR2;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using BepInEx;

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

            public static Sprite LoadSprite(string path)
            {
                if (path.IsNullOrWhiteSpace()) return null;
                return mainAssetBundle.LoadAsset<Sprite>(path);
            }

            public static void Init()
            {
                passiveEcosystemIcon = LoadSprite("Assets/StageBuffIcon");
                passiveSnackiesIcon = LoadSprite("Assets/GluttonyIcon");
                passiveLapinIcon = LoadSprite("Assets/LapinIcon");
                passiveSoulmateIcon = LoadSprite("Assets/SoulmatePassiveIcon");
                passiveMinerIcon = LoadSprite("Assets/MinerIcon");
                //Assets/NeedleIcon
                specialTFIcon = LoadSprite("Assets/ChirrTFIcon");
                specialTFIconScepter = LoadSprite("Assets/ChirrTFScepterIcon");
                specialEatIcon = LoadSprite("Assets/ChirrEatIcon");
                specialEatIconScepter = LoadSprite("Assets/ChirrEatScepterIcon");

                buffSnackiesIcon = LoadSprite("");
                buffSoulmateIcon = LoadSprite("Assets/SoulmateBuffIcon");
            }
        }
    }
}
