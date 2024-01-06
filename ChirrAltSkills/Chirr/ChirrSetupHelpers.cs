using ChirrAltSkills;
using ChirrAltSkills.Chirr.SkillDefs.TargetableSkillDef;
using RoR2;
using RoR2.Skills;
using UnityEngine;

internal static class ChirrSetupHelpers
{
    public static void AddToSkillFamily(SkillDef skillDef, SkillFamily skillFamily, UnlockableDef unlockableDef = null)
    {
        HG.ArrayUtils.ArrayAppend(ref skillFamily.variants, new SkillFamily.Variant
        {
            skillDef = skillDef,
            viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null),
            unlockableDef = unlockableDef
        });
    }

    public static void CopySkillDefFields(SkillDef parent, SkillDef child, bool copyIdentifiers)
    {
        if (!parent && child) Debug.LogError($"Attempted to call CopySkillDefFields with no parent! Child: {child.skillName}");
        if (!child && parent) Debug.LogError($"Attempted to call CopySkillDefFields with no child! Parent: {parent.skillName}");
        if (!child && !parent) Debug.LogError($"Attempted to call CopySkillDefFields with no child or parent!!!");
        child.activationState = parent.activationState;
        child.activationStateMachineName = parent.activationStateMachineName;
        child.baseMaxStock = parent.baseMaxStock;
        child.baseRechargeInterval = parent.baseRechargeInterval;
        child.beginSkillCooldownOnSkillEnd = parent.beginSkillCooldownOnSkillEnd;
        child.canceledFromSprinting = parent.canceledFromSprinting;
        child.cancelSprintingOnActivation = parent.cancelSprintingOnActivation;
        child.dontAllowPastMaxStocks = parent.dontAllowPastMaxStocks;
        child.forceSprintDuringState = parent.forceSprintDuringState;
        child.fullRestockOnAssign = parent.fullRestockOnAssign;
        child.icon = parent.icon;
        child.interruptPriority = parent.interruptPriority;
        child.isCombatSkill = parent.isCombatSkill;
        child.keywordTokens = parent.keywordTokens;
        child.mustKeyPress = parent.mustKeyPress;
        child.rechargeStock = parent.rechargeStock;
        child.requiredStock = parent.requiredStock;
        child.resetCooldownTimerOnUse = parent.resetCooldownTimerOnUse;
        if (copyIdentifiers)
        {
            child.skillDescriptionToken = parent.skillDescriptionToken;
            child.skillName = parent.skillName;
            child.skillNameToken = parent.skillNameToken;
        }
        child.stockToConsume = parent.stockToConsume;
    }

    public static T CreateSkillDef<T>(string token, bool setDefaultStateToIdle = true) where T : AssignableSkillDef
    {
        Debug.Log(token);
        T skillDef = ScriptableObject.CreateInstance<T>();
        skillDef.skillName = token;
        (skillDef as ScriptableObject).name = token;
        skillDef.skillNameToken = token + "_NAME";
        skillDef.skillDescriptionToken = token + "_DESC";
        if (setDefaultStateToIdle)
            skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
        return skillDef;
    }

    public static ChirrTargetableSkillDef CreateTargetableSkillDef(string token)
    {
        return CreateSkillDef(token) as ChirrTargetableSkillDef;
    }

    public static SkillDef CreateSkillDef(string token)
    {
        SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
        skillDef.skillName = token;
        (skillDef as ScriptableObject).name = token;
        skillDef.skillNameToken = token + "_NAME";
        skillDef.skillDescriptionToken = token + "_DESC";
        return skillDef;
    }

    public static SkillDef CreateSkillDef(string token, SkillFamily family, UnlockableDef unlockableDef = null)
    {
        var sd = CreateSkillDef(token);
        AddToSkillFamily(sd, family, unlockableDef);
        return sd;
    }
}