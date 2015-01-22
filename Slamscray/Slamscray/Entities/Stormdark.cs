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

        public enum MoveState { GROUND, SHORYUKEN, FALL };


        public Spritemap<string> spriteSheet;
        private PlatformingMovement myPlatforming;
        private BoxCollider myCollider;
        public float shoryukenTime = 0;
        public MoveState myMoveState;

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
            spriteSheet = new Spritemap<string>(Assets.STORMDARK_SHEET, 24, 32);

            // Add animations
            // ------------- name ------- frames -------- framedelays
            spriteSheet.Add("idle", new int[] { 0 }, new float[] { 10f });
            spriteSheet.Add("shoryuken", new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new float[] { 4f });
            spriteSheet.Add("airshoryuken", new int[] { 2, 3, 4, 5, 6, 7, 8 }, new float[] { 4f });
            spriteSheet.Add("jumpingup", new int[] { 9, 10, 11 }, new float[] { 2f });
            spriteSheet.Add("jumpmid", new int[] { 12 }, new float[] { 10f });
            spriteSheet.Add("fall", new int[] { 13, 14, 15 }, new float[] { 2f });
            spriteSheet.Add("run", new int[] { 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 }, new float[] { 4f });

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
            
            // Update shoryuken state
            CheckShoryuken();


            //Check Controls
            if (Global.playerSession.Controller.Down.Down)
            {
                myPlatforming.JumpStrength = 200.0f;
            }
            else
            {
                myPlatforming.JumpStrength = 250.0f;
            }

            if (Global.playerSession.Controller.X.Pressed && myMoveState != MoveState.SHORYUKEN)
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


            // Update Animation
            UpdateAnimations();


            base.Update();
        }


        public void Attack()
        {
            // Punch Combo

            // Uppercut
            if(Global.playerSession.Controller.Up.Down)
            {
                myMoveState = MoveState.SHORYUKEN;
                shoryukenTime = 4 * 9;
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

        public void CheckShoryuken()
        {
            // Update shoryuken state
            if (myMoveState == MoveState.SHORYUKEN && shoryukenTime > 0)
            {
                shoryukenTime--;
            }
            if (shoryukenTime <= 0)
            {
                shoryukenTime = 0;
                myMoveState = MoveState.GROUND;
                myPlatforming.ExtraSpeed.X = 0;

                // Check air
                if (!myPlatforming.OnGround)
                {
                    myMoveState = MoveState.FALL;
                }
            }
        }

        public void UpdateAnimations()
        {
            if(myMoveState == MoveState.GROUND)
            {
                // Check if running
                if (Math.Abs(myPlatforming.TargetSpeed.X) > 0)
                {
                    spriteSheet.Play("run");
                }

               
                else
                {
                    spriteSheet.Play("idle");
                }
            }
            if (myMoveState == MoveState.SHORYUKEN)
            {
                spriteSheet.Play("shoryuken");
            }
            if (myMoveState == MoveState.FALL)
            {
                // Check Y speed
                if(myPlatforming.Speed.Y < -20)
                {
                    spriteSheet.Play("jumpingup");
                }

                else if(myPlatforming.Speed.Y > 20)
                {
                    spriteSheet.Play("fall");
                }

                else
                {
                    spriteSheet.Play("jumpmid");
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
