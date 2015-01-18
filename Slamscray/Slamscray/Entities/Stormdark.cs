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

        public Stormdark(float x = 0, float y = 0)
        {
            // Set default X,Y coords
            X = x;
            Y = y;

            

            // Create movement
            myPlatforming = new PlatformingMovement(1000.0f, 1000.0f, 4.5f);
            myPlatforming.UseAxis = false;

            myCollider = new BoxCollider(24, 32, 1);

  

            // Load Spritesheet
            spriteSheet = new Spritemap<string>(Assets.STORMDARK_PUNCH, 24, 32);

            // Add animations
            // ------------- name ------- frames -------- framedelays
            spriteSheet.Add("idle", new int[] { 0 }, new float[] { 10f });
            spriteSheet.Add("shoryuken", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, new float[] { 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f });
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
            AddComponent(myPlatforming);

            
            
        }

        public override void Update()
        {

            //Check Controls
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

        public override void Render()
        {
            base.Render();
            //this.Collider.Render();
        }

    }
}
