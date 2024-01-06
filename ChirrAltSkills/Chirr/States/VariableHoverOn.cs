using EntityStates;
using EntityStates.Mage;
using UnityEngine;

namespace ChirrAltSkills.Chirr.States
{
    internal class VariableHoverOn : JetpackOn
    {
        //public new float hoverVelocity;
        //public new float hoverAcceleration;
        public float hoverVelocityMultiplier = 1;

        public float hoverAccelerationMultiplier = 1;

        public override void OnEnter()
        {
            base.OnEnter();

            //RoR2.Chat.AddMessage($"Vel {hoverVelocityMultiplier} Accel {hoverAccelerationMultiplier}");
            //hoverVelocity = JetpackOn.hoverVelocity * 1;// * hoverVelocityMultiplier;
            hoverAcceleration = JetpackOn.hoverAcceleration * hoverAccelerationMultiplier;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                float num = characterMotor.velocity.y;
                num = Mathf.MoveTowards(num, hoverVelocity, hoverAcceleration * Time.fixedDeltaTime);
                characterMotor.velocity = new Vector3(characterMotor.velocity.x, num, characterMotor.velocity.z);
            }
            if (base.characterBody && !base.characterBody.isSprinting && base.inputBank)
            {
                Ray aimRay = base.GetAimRay();
                Vector2 moveDirectionFlat = new Vector2(base.inputBank.moveVector.x, base.inputBank.moveVector.z);
                Vector2 forwardDirectionFlat = new Vector2(aimRay.direction.x, aimRay.direction.z);

                float angle = Vector2.Angle(moveDirectionFlat, forwardDirectionFlat);
                if (angle <= 90f) base.characterBody.isSprinting = true;
            }
        }
    }
}