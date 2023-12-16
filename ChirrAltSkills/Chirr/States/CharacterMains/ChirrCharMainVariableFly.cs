using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.CharacterAI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using Starstorm2Unofficial.Survivors.Chirr;
using Starstorm2Unofficial.Survivors.Chirr.Components;
using ChirrAltSkills.Chirr.States.Passive.Hover;

namespace ChirrAltSkills.Chirr.States.CharacterMains
{
    //not inherenting from EntityStates.SS2UStates.Chirr
    //cause worried about base.OnEnter calls redundancy
    internal class ChirrCharMainVariableFly : GenericCharacterMain
    {
        public static string wingSoundStart = "SS2UChirrSprintStart";
        public static string wingSoundLoop = "SS2UChirrSprintLoop";
        public static string wingSoundStop = "SS2UChirrSprintStop";

        private ChirrFriendController friendController;
        private uint wingSoundID;
        private bool playingWingSound = false;
        private bool inJetpackState = false;
        private EntityStateMachine jetpackStateMachine;

        private float hoverVelocityMultiplier = 1;
        private float hoverAccelerationMultiplier = 1;

        //Set, no toggle.
        private bool canHover = true;
        private bool hoverHasDuration = false;
        private float hoverDuration = -1;
        private float hoverStopwatch = 0;
        private bool hoverOnCooldown = false;

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
                    return;
                if (skill.skillDef == ChirrMain.passiveEcosystemSD)
                {
                    var mult = ChirrStageBuffInfo.GetStageHoverMultiplier();
                    var accelMult = Mathf.Max(1f, 1 / mult);

                    ChangeHoverMultiplier(1, accelMult);
                }
                else if (skill.skillDef == ChirrMain.passiveBunnySD) {
                    canHover = false;
                }
                else if (skill.skillDef == ChirrMain.passiveMinerSD)
                {
                    hoverHasDuration = true;
                    hoverDuration = 5f;
                }
                return;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            jetpackStateMachine = EntityStateMachine.FindByCustomName(gameObject, "Jetpack");

            friendController = GetComponent<ChirrFriendController>();
            if (NetworkServer.active && friendController)
            {
                friendController.TryGetSavedMaster();
            }
            ChangeMainBasedOnSkillDef();

            //Set ending text.
            //Very bad way to do this, this is a mess.
            if (characterBody && isAuthority)
            {
                CharacterMaster ownerMaster = characterBody.master;
                if (ownerMaster && ownerMaster.loadout != null)
                {
                    int skinIndex = (int)ownerMaster.loadout.bodyLoadoutManager.GetSkinIndex(ChirrCore.bodyIndex);
                    SkinDef equippedSkin = HG.ArrayUtils.GetSafe(BodyCatalog.GetBodySkins(ChirrCore.bodyIndex), skinIndex);
                    bool isMaid = equippedSkin == ChirrSkins.maidSkin;

                    if (ChirrCore.survivorDef && ChirrCore.survivorDef.outroFlavorToken != "SS2UCHIRR_OUTRO_BROTHER_EASTEREGG")
                    {
                        ChirrCore.survivorDef.outroFlavorToken = "SS2UCHIRR_OUTRO_FLAVOR";
                        if (isAuthority && isMaid) ChirrCore.survivorDef.outroFlavorToken = "SS2UCHIRR_OUTRO_MAID_FLAVOR";
                    }
                }
            }

            if (characterMotor && hoverHasDuration)
            {
                characterMotor.onHitGroundAuthority += ResetHoverCooldown;
            }
        }

        private void ResetHoverCooldown(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            hoverStopwatch = hoverDuration;
            hoverOnCooldown = false;
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
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

            inJetpackState = jetpackStateMachine && jetpackStateMachine.state.GetType() == typeof(VariableHoverOn);
            bool shouldPlayWingSound = inJetpackState;//base.characterBody.isSprinting || 
            if (shouldPlayWingSound != playingWingSound)
            {
                if (!playingWingSound)
                {
                    playingWingSound = true;
                    //Util.PlaySound(wingSoundStart, base.gameObject);
                    wingSoundID = Util.PlaySound(wingSoundLoop, gameObject);
                }
                else
                {
                    playingWingSound = false;
                    AkSoundEngine.StopPlayingID(wingSoundID);
                    Util.PlaySound(wingSoundStop, gameObject);
                }
            }

            if (hoverHasDuration && inJetpackState && !hoverOnCooldown)
            {
                hoverStopwatch -= Time.fixedDeltaTime;
                hoverOnCooldown = hoverStopwatch <= 0;
            }

            //Technically don't need a network check.
            if (NetworkServer.active)
            {
                if (friendController && skillLocator && skillLocator.special && skillLocator.special.skillDef == ChirrCore.specialScepterDef)
                {
                    friendController.canBefriendChampion = true;
                }

                //Dont set it to false, in case Mithrix steals scepter (I think it's already blacklisted from stealing)
                /*else
                {
                    friendController.canBefriendChampion = false;
                }*/
            }
        }

        public override void OnExit()
        {
            Util.PlaySound(wingSoundStop, gameObject);

            if (characterMotor && hoverHasDuration)
            {
                characterMotor.onHitGroundAuthority -= ResetHoverCooldown;
            }
            base.OnExit();
        }
    }
}
