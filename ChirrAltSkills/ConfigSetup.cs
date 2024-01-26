using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using static ChirrAltSkills.CASPlugin;

namespace ChirrAltSkills
{
    internal class ConfigSetup
    {
        public static ConfigEntry<bool> cfgEnablePassiveStageBuff;
        public static ConfigEntry<bool> cfgEnablePassiveSnackies;
        public static ConfigEntry<bool> cfgEnablePassiveBunny;
        public static ConfigEntry<bool> cfgEnablePassiveSoulmate;
        public static ConfigEntry<bool> cfgEnablePassiveDigger;

        public static ConfigEntry<bool> cfgEnablePrimaryDoubleTap;

        public static ConfigEntry<bool> cfgEnableSpecialTransform;
        public static ConfigEntry<bool> cfgEnableSpecialEat;

        private const string desc = "Enable this skill for usage.";

        public static void Init()
        {
            cfgEnablePassiveStageBuff = _config.Bind("Passive", "Stage Buff", true, desc);
            cfgEnablePassiveSnackies = _config.Bind("Passive", "Snackies", true, desc);
            cfgEnablePassiveBunny = _config.Bind("Passive", "Bunny", true, desc);
            cfgEnablePassiveSoulmate = _config.Bind("Passive", "Soulmate", true, desc);
            cfgEnablePassiveDigger = _config.Bind("Passive", "Digger", true, desc);

            cfgEnablePrimaryDoubleTap = _config.Bind("Primary", "Double Tap", true, desc);

            cfgEnableSpecialTransform = _config.Bind("Special", "Transform", true, desc);
            cfgEnableSpecialEat = _config.Bind("Special", "Eat", true, desc);
        }
    }
}
