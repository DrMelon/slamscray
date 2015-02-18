using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slamscray;

//@Author: J. Brown / DrMelon
//@Purpose: This is the CombatMove for the Hype version of the Shoryuken movement.

namespace Slamscray.Components
{
    public class CombatMoveHypeShoryuken : Slamscray.Components.CombatMove
    {
        public static float HYPE_SHORYUKEN_DAMAGE = 10.0f;
        public static float HYPE_SHORYUKEN_INVTIME = 7.0f;
        public static float HYPE_SHORYUKEN_PUSHAMT = 250.0f;
        public static float HYPE_SHORYUKEN_FREEZEAMT = 18.0f;
        public static int HYPE_SHORYUKEN_TIME = 36;

        public CombatMoveHypeShoryuken() : base()
        {
            animationToPlay = "shoryuken";
            moveLength = HYPE_SHORYUKEN_TIME;
            isInterruptable = false;
            moveTime = moveLength;
            moveState = Entities.Stormdark.MoveState.SHORYUKEN;
        }

        public override void Startup()
        {
            // Hype Shoryuken Startup - Push player into air, start moving to the left / right
            thePlayer.myMoveState = moveState;
            thePlayer.myPlatforming.Speed.Y = -300.0f;

            // Divide existing extraspeed by 6, so that we can get momentum from a dash but not go too far with it
            thePlayer.myPlatforming.ExtraSpeed.X /= 6;
            thePlayer.myPlatforming.ExtraSpeed.Y /= 6;

            if (thePlayer.spriteSheet.FlippedX)
            {
                thePlayer.myPlatforming.ExtraSpeed.X -= 50.0f;
            }
            else
            {
                thePlayer.myPlatforming.ExtraSpeed.X += 50.0f;
            }


        }

        public override void Update()
        {
            // Update shoryuken state
            if (moveTime > 0)
            {
                thePlayer.myPlatforming.HasJumped = true; //counts as a jump!
                thePlayer.shoryukened = true;

                thePlayer.Starticles();
              

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
                        atk.impulseAmt = HYPE_SHORYUKEN_PUSHAMT;

                        dam.Attacked(HYPE_SHORYUKEN_DAMAGE, atk);
                        dam.Invulnerable = true;
                        dam.InvulnTime = HYPE_SHORYUKEN_INVTIME;


                        // Freeze game a sec
                        Global.paused = true;
                        Global.pauseTime = HYPE_SHORYUKEN_FREEZEAMT;
                        Global.theCameraShaker.ShakeCamera(20.0f);


                        // Play sound
                        thePlayer.shoryukenSound.Play();

                        thePlayer.Scene.PauseGroup(Global.GROUP_ACTIVEOBJECTS);
                        thePlayer.hypeAmt += 5.0f;
                    }
                }

                moveTime--;
            }
            if (moveTime <= 0)
            {
                moveTime = 0;
                thePlayer.myPlatforming.ExtraSpeed.X = 0;
            }
        }
    }
}
