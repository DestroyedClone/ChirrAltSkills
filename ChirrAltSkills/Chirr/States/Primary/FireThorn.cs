using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ChirrAltSkills.Chirr.States.Primary
{
    //lol
    public class FireThorn : BaseState
    {
        public static float damageCoefficient = 1.5f;
        public static float force = 100f;
        public static float baseDuration = 0.75f;
        public static float baseShotDuration = 0.2f;
        public static string attackSoundString = "SS2UChirrPrimary";
        public static int baseShotCount = 1;
        public static float spreadBloom = 0.4f;
        public static float recoil = 1f;

        public static GameObject projectilePrefab;
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/MuzzleflashCroco.prefab").WaitForCompletion();

        private int shotCount;
        private float duration;
        private float shotDuration;
        private float shotStopwatch;
        private bool crit;

        public override void OnEnter()
        {
            base.OnEnter();

            crit = RollCrit();
            shotCount = 0;
            shotStopwatch = 0f;
            duration = baseDuration / attackSpeedStat;
            shotDuration = baseShotDuration / attackSpeedStat;
            if (characterBody)
            {
                characterBody.SetAimTimer(2f);
            }

            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, "MuzzleWingL", false);
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, gameObject, "MuzzleWingR", false);

            PlayCrossfade("Gesture, Override", "Primary", "Primary.playbackRate", duration, 0.1f);
            PlayCrossfade("Gesture, Additive", "Primary", "Primary.playbackRate", duration, 0.1f);

            FireBullet();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (shotCount < baseShotCount)
            {
                shotStopwatch += Time.fixedDeltaTime;
                if (shotStopwatch >= shotDuration)
                {
                    FireBullet();
                }
            }
            else
            {
                if (isAuthority && fixedAge >= duration && shotCount >= baseShotCount)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (characterBody)
            {
                characterBody.SetSpreadBloom(0f, false);
            }
            base.OnExit();
        }

        private void FireBullet()
        {
            shotStopwatch = 0f;
            shotCount++;
            Util.PlaySound(attackSoundString, gameObject);

            if (isAuthority)
            {
                Ray aimRay = GetAimRay();

                ProjectileManager.instance.FireProjectile(projectilePrefab,
                    aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction),
                    gameObject, damageStat * damageCoefficient,
                    0f,
                    crit,
                    DamageColorIndex.Default, null, -1f);
            }
            AddRecoil(-0.4f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            if (characterBody) characterBody.AddSpreadBloom(spreadBloom); //Spread is cosmetic. Skill is always perfectly accurate.
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}