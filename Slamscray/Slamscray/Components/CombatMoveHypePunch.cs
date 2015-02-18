using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slamscray;

//@Author: J. Brown / DrMelon
//@Purpose: This is the CombatMove for the Punch movement.

namespace Slamscray.Components
{
    public class CombatMoveHypePunch : Slamscray.Components.CombatMove
    {
        public static float HYPE_PUNCH_DAMAGE = 1.0f;
        public static float HYPE_PUNCH_INVTIME = 10.0f;
        public static float HYPE_PUNCH_PUSHAMT = 100.0f;
        public static float HYPE_PUNCH_FREEZEAMT = 0.0f;
        public static int HYPE_PUNCH_TIME = 10;

        public CombatMoveHypePunch() : base()
        {
            animationToPlay = "punch";
            moveLength = HYPE_PUNCH_TIME;
            isInterruptable = false;
            moveTime = moveLength;
            moveState = Entities.Stormdark.MoveState.PUNCH;
        }

        public override void Startup()
        {
            thePlayer.myMoveState = moveState;
        }

        public override void Update()
        {
            // Update shoryuken state
            if (moveTime > 0)
            {
                // Speed down
                thePlayer.myPlatforming.Speed.X = Util.Approach(thePlayer.myPlatforming.Speed.X, 0, thePlayer.myPlatforming.CurrentAccel / 1.3f);

                // Damage anything we hit with a healthdamage component, and shove it around!
                List<Entity> collisionList = thePlayer.myCollider.CollideEntities(thePlayer.X, thePlayer.Y, thePlayer.myCollider.Tags);
                foreach (Entity ent in collisionList)
                {
                    if (ent == thePlayer)
                    {
                        // Don't collide with yourself!
                        continue;
                    }
                    Slamscray.Components.HealthDamageComponent dam = ent.GetComponent<Slamscray.Components.HealthDamageComponent>();
                    if (dam != null && dam.Invulnerable == false && dam.Dead == false)
                    {
                        // Set damage accordingly.
                        // Set up attack info
                        Slamscray.Components.HealthDamageComponent.AttackInfo atk = new Components.HealthDamageComponent.AttackInfo();
                        atk.facingLeft = thePlayer.spriteSheet.FlippedX;
                        atk.impulseAmt = HYPE_PUNCH_PUSHAMT;
                        dam.Attacked(HYPE_PUNCH_DAMAGE, atk);
                        dam.Invulnerable = true;
                        dam.InvulnTime = HYPE_PUNCH_INVTIME;

                        // Freeze game a sec
                        Global.paused = true;
                        Global.theCameraShaker.ShakeCamera(10);
                        Global.pauseTime = HYPE_PUNCH_FREEZEAMT;

                        // Play sound
                        thePlayer.hypePunchSound.Play();

                        thePlayer.Scene.PauseGroup(Global.GROUP_ACTIVEOBJECTS);
                        thePlayer.hypeAmt += 5.0f;
                    }
                }

                moveTime--;
            }
            if (moveTime <= 0)
            {
                moveTime = 0;
            }
        }
    }
}
