﻿using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slamscray;

//@Author: J. Brown / DrMelon
//@Purpose: This is the CombatMove for the Shoryuken movement.

namespace Slamscray.Components
{
    public class CombatMoveShoryuken : Slamscray.Components.CombatMove
    {
        public static float SHORYUKEN_DAMAGE = 5.0f;
        public static float SHORYUKEN_INVTIME = 45.0f;
        public static float SHORYUKEN_PUSHAMT = 125.0f;
        public static float SHORYUKEN_FREEZEAMT = 5.0f;
        public static int SHORYUKEN_TIME = 36;

        public CombatMoveShoryuken() : base()
        {
            animationToPlay = "shoryuken";
            moveLength = SHORYUKEN_TIME;
            isInterruptable = false;
            moveTime = moveLength;
            moveState = Entities.Stormdark.MoveState.SHORYUKEN;
        }

        public override void Startup()
        {
            thePlayer.myMoveState = moveState;
            
            // Shoryuken Startup - Push player into air, start moving to the left / right
            thePlayer.myPlatforming.Speed.Y = -200.0f;

            // Divide existing extraspeed by 8, so that we can get momentum from a dash but not go too far with it
            thePlayer.myPlatforming.ExtraSpeed.X /= 8;
            thePlayer.myPlatforming.ExtraSpeed.Y /= 8;

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
                        atk.impulseAmt = SHORYUKEN_PUSHAMT;
                        dam.Attacked(SHORYUKEN_DAMAGE, atk);
                        dam.Invulnerable = true;
                        dam.InvulnTime = SHORYUKEN_INVTIME;

                        // Freeze game a sec
                        Global.paused = true;
                        Global.pauseTime = SHORYUKEN_FREEZEAMT;

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
