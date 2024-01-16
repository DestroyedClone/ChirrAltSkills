using RoR2;
using System;
using UnityEngine;
using static ChirrAltSkills.Chirr.ChirrStageBuffInfo;

namespace ChirrAltSkills.Chirr
{
    internal class ChirrCommands
    {
        public static void AddChatCommands()
        {
            On.RoR2.Chat.SendBroadcastChat_ChatMessageBase += ActivateChatCommand;
        }

        private static void ActivateChatCommand(On.RoR2.Chat.orig_SendBroadcastChat_ChatMessageBase orig, ChatMessageBase message)
        {
            if (!(message is Chat.UserChatMessage chatMsg)) return;
            if (chatMsg.text.ToLower() != "/chirrstage") return;
            var bop = ChirrStageBuffInfo.GetCurrentStageBuffInfo();
            bop.SendChatChangeMessage(chatMsg.sender.GetComponent<NetworkUser>().netId);
            chatMsg.text += $" [!]";
            orig(message);
        }

        [ConCommand(commandName = "cas_printstagebuff", flags = ConVarFlags.None, helpText = "Prints the current stagebuffinfo")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Registered and used by assembly")]
        private static void CCPrintStageBuffInfo(ConCommandArgs _)
        {
            if (!Run.instance)
            {
                UnityEngine.Debug.LogWarning("Can't run cas_printstagebuff without being in a run!");
                return;
            }
            var bop = ChirrStageBuffInfo.GetCurrentStageBuffInfo();
            bop.LogChangeMessage();
        }
    }
}