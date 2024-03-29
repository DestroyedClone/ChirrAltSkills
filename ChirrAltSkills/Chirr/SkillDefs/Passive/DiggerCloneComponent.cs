﻿using RoR2;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.SkillDefs
{
    public class DiggerCloneComponent : MonoBehaviour
    {
        private float adrenalineGainBuffer;
        private float adrenalineCap;
        private int moneyTracker;
        private float residue;
        private int buffCounter;

        public CharacterBody characterBody;

        public void Awake()
        {
            this.characterBody = GetComponent<CharacterBody>();
        }

        public void Start()
        {
            this.adrenalineGainBuffer = 0.3f;
            this.moneyTracker = (int)characterBody.master.money;
            this.adrenalineCap = CASPlugin.modloaded_Digger ? GetModdedAdrenalineCap() : 49;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public float GetModdedAdrenalineCap()
        {
            return DiggerPlugin.DiggerPlugin.adrenalineCap;
        }

        public void FixedUpdate()
        {
            this.adrenalineGainBuffer -= Time.fixedDeltaTime;
            if (this.adrenalineGainBuffer <= 0 && NetworkServer.active) this.UpdatePassiveBuff();
            else this.buffCounter = characterBody.GetBuffCount(Buffs.adrenalineBuff);
        }

        private void UpdatePassiveBuff()
        {
            int currentCount = characterBody.GetBuffCount(Buffs.adrenalineBuff);
            int newMoney = (int)characterBody.master.money;

            if (this.moneyTracker < newMoney)
            {
                this.RefreshExistingStacks(currentCount);
                this.GiveNewStacks(newMoney);
            }
            this.moneyTracker = newMoney;

            this.HandleStackDecay(currentCount);
        }

        private void RefreshExistingStacks(int currentCount)
        {
            characterBody.ClearTimedBuffs(Buffs.adrenalineBuff);
            for (int i = 0; i < currentCount; i++)
            {
                if (characterBody.GetBuffCount(Buffs.adrenalineBuff) <= this.adrenalineCap) characterBody.AddTimedBuff(Buffs.adrenalineBuff, 5);
            }
        }

        private void GiveNewStacks(int newMoney)
        {
            float baseReward = (newMoney - this.moneyTracker) / Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f);
            this.residue = (baseReward + this.residue) % 5;
            float numStacks = (baseReward + this.residue) / 5;

            for (int i = 1; i <= numStacks; i++)
            {
                if (characterBody.GetBuffCount(Buffs.adrenalineBuff) <= this.adrenalineCap) characterBody.AddTimedBuff(Buffs.adrenalineBuff, 5);
            }
        }

        private void HandleStackDecay(int currentCount)
        {
            if (this.buffCounter != 0 && currentCount == 0)
            {
                for (int i = 1; i < buffCounter * .5; i++)
                {
                    if (characterBody.GetBuffCount(Buffs.adrenalineBuff) <= this.adrenalineCap) characterBody.AddTimedBuff(Buffs.adrenalineBuff, 1);
                }
            }

            this.buffCounter = characterBody.GetBuffCount(Buffs.adrenalineBuff);
        }

        //new
        private void OnDestroy()
        {
            characterBody.ClearTimedBuffs(Buffs.adrenalineBuff);
        }
    }
}