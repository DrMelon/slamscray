﻿
//@Author: J. Brown / DrMelon
//@Purpose: Simple test enemy to beat on!

using Otter;
using Slamscray;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slamscray.Entities.Enemies
{
    public class TestEnemy : Entity
    {

        public Spritemap<string> spriteSheet;
        private PlatformingMovement myPlatforming;
        private BoxCollider myCollider;

        private int moveTimer;
        private bool isMoving;
        private int nextMove;
        private int moveDir;

        public TestEnemy(float x = 0, float y = 0)
        {
            // Set default X,Y coords
            X = x;
            Y = y;

            // Create movement
            myPlatforming = new PlatformingMovement(1600.0f, 1600.0f, 4.5f);
            myPlatforming.UseAxis = false;
            myPlatforming.JumpEnabled = false;

            // Hitbox 
            myCollider = new BoxCollider(24, 24, 1);

            // Load Spritesheet
            spriteSheet = new Spritemap<string>(Assets.TESTENEMY, 24, 24);

            // Add animations
            // ------------- name ------- frames -------- framedelays
            spriteSheet.Add("idle", new int[] { 0, 1 }, new float[] { 65f, 8f });
            spriteSheet.Add("hop", new int[] { 0, 1, 2 }, new float[] { 4f, 4f, 6f });
            spriteSheet.Add("fall", new int[] { 3 }, new float[] { 10f });

            // Use idle animation to start
            spriteSheet.Play("idle");

            // Set display to use this spritesheet
            Graphic = spriteSheet;

            // Add Components
            AddCollider(myCollider);
            this.Collider = myCollider;
            myPlatforming.Collider = myCollider;
            myPlatforming.AddCollision(0);
            AddComponent(myPlatforming);

            
        }

        public override void Update()
        {

            // Move left and right occasionally.
            if (!isMoving)
            {
                spriteSheet.Play("idle");
                myPlatforming.TargetSpeed.X = 0;
                
                
            }
            else
            {
                moveTimer--;
                spriteSheet.Play("hop");

                myPlatforming.TargetSpeed.X = 40.0f * moveDir;
                

                if (moveTimer <= 0)
                {
                    isMoving = false;
                    nextMove = Rand.Int(30, 320);
                }
            }

            if (nextMove > 0 && !isMoving)
            {
                nextMove--;
            }
            if (nextMove <= 0 && isMoving == false)
            {
                isMoving = true;
                moveTimer = Rand.Int(60, 120);
                moveDir = Rand.Choose<int>(new int[] { -1, 1 });
            }

            // Set sprite orientation
            if(myPlatforming.Speed.X < 0)
            {
                spriteSheet.FlippedX = true;
            }
            if(myPlatforming.Speed.X > 0)
            {
                spriteSheet.FlippedX = false;
            }

            // Set falling animation
            if(myPlatforming.OnGround == false)
            {
                spriteSheet.Play("fall");
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

        public override void Render()
        {
            base.Render();
        }
    }
}