//@Author: J. Brown / DrMelon
//@Purpose: This is the player's class!


using Otter;
using Slamscray;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slamscray.Entities
{
    public class Stormdark : Entity
    {

        public Spritemap<string> spriteSheet;
        private PlatformingMovement myPlatforming;
        private BoxCollider myCollider;
        public bool isAttacking = false;
        public float attackTime = 0;

        public Stormdark(float x = 0, float y = 0)
        {
            // Set default X,Y coords
            X = x;
            Y = y;

            // Create movement
            myPlatforming = new PlatformingMovement(1600.0f, 1600.0f, 4.5f);
            myPlatforming.UseAxis = false;

            // Hitbox substantially smaller than the sprite itself.
            myCollider = new BoxCollider(24 / 2, 28, 1);
            myCollider.X = 24 / 4; // Center the hitbox.
            myCollider.Y += 4;

            // Load Spritesheet
            spriteSheet = new Spritemap<string>(Assets.STORMDARK_PUNCH, 24, 32);

            // Add animations
            // ------------- name ------- frames -------- framedelays
            spriteSheet.Add("idle", new int[] { 0 }, new float[] { 10f });
            spriteSheet.Add("shoryuken", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, new float[] { 4f});
            spriteSheet.Add("airshoryuken", new int[] { 2, 3, 4, 5, 6, 7 }, new float[] { 2f, 2f, 2f, 2f, 2f, 2f });

            // Use idle animation to start
            spriteSheet.Play("idle");

            // Set display to use this spritesheet
            Graphic = spriteSheet;

            // Setup controls
            myPlatforming.JumpButton = Global.playerSession.Controller.A;

            // Add Components
            AddCollider(myCollider);
            this.Collider = myCollider;
            myPlatforming.Collider = myCollider;
            myPlatforming.AddCollision(0);
            myPlatforming.JumpStrength = 250.0f;
            myPlatforming.VariableJumpHeight = false;
            AddComponent(myPlatforming);

            
            
        }
        
        public override void Update()
        {
            // Update attacking state
            if (isAttacking && attackTime > 0)
            {
                attackTime--;
            }
            if(attackTime <= 0)
            {
                attackTime = 0;
                isAttacking = false;
                spriteSheet.Play("idle");

                // checking for shoryuken states bleh bleh bleh
                myPlatforming.ExtraSpeed.X = 0;
            }


            //Check Controls
            if (Global.playerSession.Controller.Down.Down)
            {
                myPlatforming.JumpStrength = 200.0f;
            }
            else
            {
                myPlatforming.JumpStrength = 250.0f;
            }

            if (Global.playerSession.Controller.X.Pressed && !isAttacking)
            {
                Attack();
            }


            if(Global.playerSession.Controller.Left.Down)
            {
                myPlatforming.TargetSpeed.X = -150.0f;
                spriteSheet.FlippedX = true;
            }
            else if (Global.playerSession.Controller.Right.Down)
            {
                myPlatforming.TargetSpeed.X = 150.0f;
                spriteSheet.FlippedX = false;
            }
            else
            {
                myPlatforming.TargetSpeed.X = 0.0f;
            }

            myPlatforming.Speed.X = Util.Approach(myPlatforming.Speed.X, myPlatforming.TargetSpeed.X, myPlatforming.CurrentAccel);

            if (myPlatforming.Speed.X < 0 && myPlatforming.AgainstWallLeft)
            {
                myPlatforming.Speed.X = 0;
            }
            if (myPlatforming.Speed.X > 0 && myPlatforming.AgainstWallRight)
            {
                myPlatforming.Speed.X = 0;
            }




            base.Update();
        }

        public void Attack()
        {
            // Punch Combo

            // Uppercut
            if(Global.playerSession.Controller.Up.Down)
            {
                isAttacking = true;
                attackTime = 4 * 9;
                spriteSheet.Play("shoryuken");
                // Leap into the air
                myPlatforming.Speed.Y = -200.0f;
                if (spriteSheet.FlippedX)
                {
                    myPlatforming.ExtraSpeed.X -= 50.0f;
                    
                }
                else
                {
                    myPlatforming.ExtraSpeed.X += 50.0f;
                }
            }


        }

        public override void Render()
        {
            base.Render();
            //this.Collider.Render();
        }

    }
}
