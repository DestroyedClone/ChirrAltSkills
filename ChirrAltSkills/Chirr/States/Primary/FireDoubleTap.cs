using System;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.SS2UStates.Chirr;

namespace ChirrAltSkills.Chirr.States.Primary
{
    public class FireDoubleTap : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        void SteppedSkillDef.IStepSetter.SetStep(int i)
        {
            pistol = i;
        }

        private void FireBullet(string targetMuzzle)
        {
            Util.PlaySound(FirePistol2.firePistolSoundString, gameObject);
            if (ChirrPrimary.muzzleflashEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(ChirrPrimary.muzzleflashEffectPrefab, gameObject, targetMuzzle, false);

            }
            AddRecoil(-0.4f * FirePistol2.recoilAmplitude, -0.8f * FirePistol2.recoilAmplitude, -0.3f * FirePistol2.recoilAmplitude, 0.3f * FirePistol2.recoilAmplitude);
            if (isAuthority)
            {
                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = characterBody.spreadBloomAngle,
                    damage = FirePistol2.damageCoefficient * damageStat,
                    force = FirePistol2.force,
                    tracerEffectPrefab = FirePistol2.tracerEffectPrefab,
                    muzzleName = targetMuzzle,
                    hitEffectPrefab = FirePistol2.hitEffectPrefab,
                    isCrit = Util.CheckRoll(critStat, characterBody.master),
                    radius = 0.1f,
                    smartCollision = true
                }.Fire();
            }
            characterBody.AddSpreadBloom(FirePistol2.spreadBloomValue);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            duration = FirePistol2.baseDuration / attackSpeedStat;
            aimRay = GetAimRay();
            StartAimMode(aimRay, 3f, false);
            /*if (this.pistol % 2 == 0)
            {
                this.PlayAnimation("Gesture Additive, Left", "FirePistol, Left");
                this.FireBullet("MuzzleLeft");
                return;
            }
            this.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
            this.FireBullet("MuzzleRight");*/
            PlayCrossfade("Gesture, Override", "Primary", "Primary.playbackRate", duration, 0.1f);
            PlayCrossfade("Gesture, Additive", "Primary", "Primary.playbackRate", duration, 0.1f);
            if (pistol % 2 == 0)
            {
                FireBullet("MuzzleWingL");
                return;
            }
            FireBullet("MuzzleWingR");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge < duration || !isAuthority)
            {
                return;
            }
            /*if (base.activatorSkillSlot.stock <= 0)
            {
                this.outer.SetNextState(new ReloadPistols());
                return;
            }*/
            outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private int pistol;

        private Ray aimRay;

        private float duration;
    }
}
