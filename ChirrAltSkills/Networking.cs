using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using R2API.Networking;
using RoR2;
using UnityEngine;
using R2API.Networking.Interfaces;
using UnityEngine.Networking;
using BepInEx;

namespace ChirrAltSkills
{
    public class Networking
    {
        public static void Init()
        {
            NetworkingAPI.RegisterMessageType<SendToClient_ChirrStatInfo>();
        }
        public class SendToClient_ChirrStatInfo : INetMessage
        {
            public NetworkInstanceId targetNetId;

            private const string formatToken = "DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_1";
            private const string armorToken = "DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_ARMOR";
            public float armor;
            private const string attackspeedToken = "DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_ATTACKSPEED";
            public float attackspeed;
            private const string movespeedToken = "DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_MOVESPEED";
            public float movespeed;
            private const string regenToken = "DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_REGEN";
            public float regen;
            private const string hoverToken = "DESCLONE_SS2UCHIRR_STAGEBUFF_MESSAGE_HOVER";
            public float hover;

            public SendToClient_ChirrStatInfo() { }

            public SendToClient_ChirrStatInfo(NetworkInstanceId targetNetId, float armor, float movespeed, float attackspeed, float regen, float hover)
            {
                this.targetNetId = targetNetId;
                this.armor = armor;
                this.movespeed = movespeed;
                this.attackspeed = attackspeed;
                this.regen = regen;
                this.hover = hover;
            }

            public void Deserialize(NetworkReader reader)
            {
                targetNetId = reader.ReadNetworkId();
                armor = reader.ReadSingle();
                movespeed = reader.ReadSingle();
                attackspeed = reader.ReadSingle();
                regen = reader.ReadSingle();
                hover = reader.ReadSingle();
            }

            public void OnReceived()
            {
                if (LocalUserManager.GetFirstLocalUser().currentNetworkUser.netId != targetNetId)
                    return;
                var message = Language.GetStringFormatted(formatToken, 
                    Language.GetString(armorToken),
                    armor.ToString(),
                    Language.GetString(attackspeedToken),
                    string.Format("{0:P}", attackspeed),
                    Language.GetString(movespeedToken),
                    string.Format("{0:P}", movespeed),
                    Language.GetString(regenToken),
                    regen.ToString(),
                    Language.GetString(hoverToken),
                    string.Format("{0:P}", hover)
                    );
                Chat.AddMessage(message);
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(targetNetId);
                writer.Write(armor);
                writer.Write(attackspeed);
                writer.Write(movespeed);
                writer.Write(regen);
                writer.Write(hover);
            }
        }

        public class SendToAllSimpleChatMessage : INetMessage
        {
            public string formatToken;
            public string paramTokens;

            public SendToAllSimpleChatMessage() { }

            public SendToAllSimpleChatMessage(string formatToken, string paramTokens)
            {
                this.formatToken = formatToken;
                this.paramTokens = paramTokens;
            }

            public void Deserialize(NetworkReader reader)
            {
                formatToken = reader.ReadString();
                paramTokens = reader.ReadString();
            }

            public void OnReceived()
            {
                List<string> delimitFormatted = new List<string>();
                foreach (var token in paramTokens.Split(','))
                {
                    delimitFormatted.Add(Language.GetString(token));
                }
                Chat.AddMessage(Language.GetStringFormatted(formatToken, delimitFormatted));
            }

            public void Serialize(NetworkWriter writer)
            {
                writer.Write(formatToken);
                writer.Write(paramTokens);
            }
        }
    }
}
