using EntityStates;
using EntityStates.Mage;
using UnityEngine;

namespace ChirrAltSkills.Chirr.States.Passive.Hover
{
    internal class VariableHoverOn : BaseState
    {
        //public static float hoverVelocity;

        //public static float hoverAcceleration;

        public float hoverVelocity;
        public float hoverAcceleration;
        public float hoverVelocityMultiplier = 1;
        public float hoverAccelerationMultiplier = 1;

        private Transform jetOnEffect;

        public override void OnEnter()
        {
            base.OnEnter();

            //RoR2.Chat.AddMessage($"Vel {hoverVelocityMultiplier} Accel {hoverAccelerationMultiplier}");
            hoverVelocity = JetpackOn.hoverVelocity;// * hoverVelocityMultiplier;
            hoverAcceleration = JetpackOn.hoverAcceleration * hoverAccelerationMultiplier;

            this.jetOnEffect = base.FindModelChild("JetOn");
            if (this.jetOnEffect)
            {
                this.jetOnEffect.gameObject.SetActive(true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                float num = base.characterMotor.velocity.y;
                num = Mathf.MoveTowards(num, hoverVelocity, hoverAcceleration * Time.fixedDeltaTime);
                base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, num, base.characterMotor.velocity.z);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.jetOnEffect)
            {
                this.jetOnEffect.gameObject.SetActive(false);
            }
        }
    }
}