using EntityStates;
using EntityStates.SS2UStates.Chirr;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ChirrAltSkills.Chirr.States.CharacterMains
{
    internal class ChirrCharMainVariableFly : ChirrMain
    {
        private float hoverVelocityMultiplier = 1;
        private float hoverAccelerationMultiplier = 1;

        //Set by ChangeHoverMultiplier, do not set manually.
        private bool canHover = true;

        private bool hoverHasDuration = false;
        private float hoverDuration = -1;
        private float hoverStopwatch = 0;
        private bool hoverOnCooldown = false;

        private static BuffDef IndicatorBuff => Buffs.hoverDurationIndicatorBuff;

        public bool isBunny = false;

        public void ChangeHoverMultiplier(float hoverVelocityMultiplier, float hoverAccelerationMultiplier)
        {
            this.hoverVelocityMultiplier = hoverVelocityMultiplier;
            this.hoverAccelerationMultiplier = hoverAccelerationMultiplier;
            if (hoverAccelerationMultiplier < 0)
            {
                canHover = false;
            };
        }

        public void ChangeMainBasedOnSkillDef()
        {
            foreach (var skill in gameObject.GetComponents<GenericSkill>())
            {
                if (!skill || !skill.skillFamily || (skill.skillFamily as ScriptableObject).name != "DCSS2UChirrPassive")
                    continue;
                if (skill.skillDef == ChirrSetup.passiveStageBuffSD)
                {
                    var mult = ChirrStageBuffInfo.GetStageHoverMultiplier();
                    var accelMult = Mathf.Max(1f, 1 / mult);

                    ChangeHoverMultiplier(1, accelMult);
                }
                else if (skill.skillDef == ChirrSetup.passiveBunnySD)
                {
                    isBunny = true;
                    hoverHasDuration = true;
                    hoverDuration = 2f;
                    //ChangeHoverMultiplier(1, -1);
                }
                else if (skill.skillDef == ChirrSetup.passiveDiggerSD)
                {
                    hoverHasDuration = true;
                    hoverDuration = 5f;
                }
                break;
            }
            if (hoverHasDuration)
            {
                characterMotor.onHitGroundServer += ClearIndicatorBuffOnLandingServer;
            }
        }

        private void ClearIndicatorBuffOnLandingServer(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            characterBody.ClearTimedBuffs(IndicatorBuff);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            ChangeMainBasedOnSkillDef();

            if (characterMotor && hoverHasDuration)
            {
                characterMotor.onHitGroundAuthority += ResetHoverCooldown;
            }

            var speSD = skillLocator.special.skillDef;
            //var chirrTracker = GetComponent<ChirrTracker>();
            //if (speSD == Starstorm2Unofficial.Survivors.Chirr.ChirrCore.specialDef)
            if (speSD == ChirrSetup.specialEatSD)
            {
                friendController.maxTrackingDistance = 0;
            }
            else if (speSD == ChirrSetup.specialTransformSD)
            {
                friendController.maxTrackingDistance = 0;
            }
        }

        private void ResetHoverCooldown(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            hoverStopwatch = hoverDuration;
            hoverOnCooldown = false;
            if (NetworkServer.active)
                while (characterBody.HasBuff(IndicatorBuff))
                {
                    characterBody.RemoveBuff(IndicatorBuff);
                }
        }

        public void GenericCharacterMain_ProcessJump()
        {
            if (this.hasCharacterMotor)
            {
                bool flag = false;
                bool flag2 = false;
                if (this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
                {
                    int itemCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
                    float horizontalBonus = 1f;
                    float verticalBonus = 1f;
                    if (base.characterMotor.jumpCount >= base.characterBody.baseJumpCount)
                    {
                        flag = true;
                        horizontalBonus = 1.5f;
                        verticalBonus = 1.5f;
                    }
                    else if ((float)itemCount > 0f && base.characterBody.isSprinting)
                    {
                        float num = base.characterBody.acceleration * base.characterMotor.airControl;
                        if (base.characterBody.moveSpeed > 0f && num > 0f)
                        {
                            flag2 = true;
                            float num2 = Mathf.Sqrt(10f * (float)itemCount / num);
                            float num3 = base.characterBody.moveSpeed / num;
                            horizontalBonus = (num2 + num3) / num3;
                        }
                    }
                    GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus, false);
                    if (this.hasModelAnimator)
                    {
                        int layerIndex = base.modelAnimator.GetLayerIndex("Body");
                        if (layerIndex >= 0)
                        {
                            if (base.characterMotor.jumpCount == 0 || base.characterBody.baseJumpCount == 1)
                            {
                                base.modelAnimator.CrossFadeInFixedTime("Jump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
                            }
                            else
                            {
                                base.modelAnimator.CrossFadeInFixedTime("BonusJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
                            }
                        }
                    }
                    if (flag)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
                        {
                            origin = base.characterBody.footPosition
                        }, true);
                    }
                    else if (base.characterMotor.jumpCount > 0)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
                        {
                            origin = base.characterBody.footPosition,
                            scale = base.characterBody.radius
                        }, true);
                    }
                    if (flag2)
                    {
                        EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
                        {
                            origin = base.characterBody.footPosition,
                            rotation = Util.QuaternionSafeLookRotation(base.characterMotor.velocity)
                        }, true);
                    }
                    base.characterMotor.jumpCount++;
                }
            }
        }

        public override void ProcessJump()
        {
            GenericCharacterMain_ProcessJump(); //base.ProcessJump();
            inJetpackState = jetpackStateMachine.state.GetType() == typeof(VariableHoverOn);
            if (hasCharacterMotor && hasInputBank && isAuthority)
            {
                bool inputPressed = inputBank.jump.down && characterMotor.velocity.y < 0f && !characterMotor.isGrounded;
                if (canHover
                    && inputPressed
                    && !inJetpackState
                    && (!hoverHasDuration || !hoverOnCooldown)
                    )
                {
                    jetpackStateMachine.SetNextState(new VariableHoverOn()
                    {
                        hoverVelocityMultiplier = hoverVelocityMultiplier,
                        hoverAccelerationMultiplier = hoverAccelerationMultiplier
                    });
                    //if NetworkServer.active of course it wont run under an authority check
                    for (int i = 0; i < hoverDuration + 1; i++)
                    {
                        characterBody.AddTimedBuffAuthority(IndicatorBuff.buffIndex, hoverDuration - i);
                    }
                }
                if (inJetpackState && (!inputPressed || !canHover || hoverOnCooldown))
                {
                    jetpackStateMachine.SetNextState(new Idle());
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            //gets set then set by this
            inJetpackState = jetpackStateMachine && jetpackStateMachine.state.GetType() == typeof(VariableHoverOn);

            if (hoverHasDuration && inJetpackState && !hoverOnCooldown)
            {
                hoverStopwatch -= Time.fixedDeltaTime;
                /*if (NetworkServer.active)
                {
                    var buffCount = characterBody.GetBuffCount(IndicatorBuff);
                    if (buffCount > hoverStopwatch + 1 && buffCount > 0)
                    {
                        characterBody.RemoveBuff(IndicatorBuff);
                    }
                }*/
                hoverOnCooldown = hoverStopwatch <= 0;
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                while (characterBody.HasBuff(IndicatorBuff))
                {
                    characterBody.RemoveBuff(IndicatorBuff);
                }
            }
            if (characterMotor && hoverHasDuration)
            {
                characterMotor.onHitGroundAuthority -= ResetHoverCooldown;
            }
            base.OnExit();
        }
    }
}