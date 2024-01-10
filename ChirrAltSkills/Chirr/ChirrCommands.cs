using RoR2;
using System;
using UnityEngine;
using static ChirrAltSkills.Chirr.ChirrStageBuffInfo;

namespace ChirrAltSkills.Chirr
{
    internal class ChirrCommands
    {
        [ConCommand(commandName = "cas_printstagebuff", flags = ConVarFlags.None, helpText = "Prints the current stagebuffinfo")]
        private static void CCPrintStageBuffInfo(ConCommandArgs args)
        {
            if (!Run.instance)
            {
                UnityEngine.Debug.LogWarning("Can't run cas_printstagebuff without being in a run!");
                return;
            }
            var info = ChirrStageBuffInfo.GetCurrentStageBuffInfo();

            var stageClearCount = Mathf.Max(0, Run.instance.stageClearCount);

            var multiplier = 1 + stageClearCount * StageBuffInfo.stageMultiplierStagesCleared;
            var multiplierMS = 1 + Mathf.Min(20, stageClearCount) * StageBuffInfo.stageMultiplierStagesCleared;

            float armorMult = info.armor * multiplier;
            float asMult = info.attackspeed * multiplier;
            float msMult = info.movespeed * multiplierMS;
            float regenMult = info.regen * multiplier;

            string g(string token)
            {
                return Language.GetString(token);
            }
            string stageName = Language.GetString(SceneCatalog.GetSceneDefForCurrentScene().nameToken);
            string formattedAnnouncement = Language.GetStringFormatted("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_0", stageName);
            string formattedStatShow = Language.GetStringFormatted("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_1",
                g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_ARMOR"), armorMult,
                g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_MOVESPEED"), string.Format("{0:P}", asMult),
                g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_ATTACKSPEED"), string.Format("{0:P}", msMult),
                g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_REGEN"), regenMult,
                g("DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_HOVER"), string.Format("{0:P}", info.hover));
            Debug.Log(formattedAnnouncement + Environment.NewLine + formattedStatShow);
        }
    }
}