
//@Author: J. Brown / DrMelon
//@Purpose: Simple test enemy to beat on!

using Otter;
using Slamscray;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Slamscray.Entities.Enemies
{
    public class TestEnemy : Entity
    {

        public Spritemap<string> spriteSheet;
        private PlatformingMovement myPlatforming;
        private BoxCollider myCollider;
        private Components.HealthDamageComponent myHealth;

        private int moveTimer;
        private bool isMoving;
        private int nextMove;
        private int moveDir;
        private bool inDamageMode;
        private Slamscray.Components.HealthDamageComponent.AttackInfo atk;



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
            spriteSheet.Add("hurt", new int[] { 4, 5 }, new float[] { 2f, 2f });

            // Use idle animation to start
            spriteSheet.Play("idle");

            // Set display to use this spritesheet
            Graphic = spriteSheet;

            // Health/damage component
            myHealth = new Slamscray.Components.HealthDamageComponent();
            myHealth.MaximumHealth = 1000;
            myHealth.Health = 1000;
            myHealth.OnDamageTaken = this.OnTakeDamage;
            AddComponent(myHealth);

            // Add Components
            AddCollider(myCollider);
            this.Collider = myCollider;
            myPlatforming.Collider = myCollider;
            myPlatforming.AddCollision(0);
            
            AddComponent(myPlatforming);

            // Add to pausegroup.
            Group = Global.GROUP_ACTIVEOBJECTS;
        }

        public override void Update()
        {
            AnimationUpdate();


            // Move left and right occasionally.
            if (!isMoving)
            {
                
                myPlatforming.TargetSpeed.X = 0;
                
                
            }
            else
            {
                moveTimer--;

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


            myPlatforming.Speed.X = Util.Approach(myPlatforming.Speed.X, myPlatforming.TargetSpeed.X, myPlatforming.CurrentAccel);
            myPlatforming.ExtraSpeed.X = Util.Approach(myPlatforming.ExtraSpeed.X, 0, myPlatforming.CurrentAccel / 32); // Skid to a halt when knocked around.

            if (myPlatforming.Speed.X < 0 && myPlatforming.AgainstWallLeft)
            {
                myPlatforming.Speed.X = 0;
            }
            if (myPlatforming.Speed.X > 0 && myPlatforming.AgainstWallRight)
            {
                myPlatforming.Speed.X = 0;
            }


            // Destroy self if fall off world.
            if(Y > 8000)
            {
                this.RemoveSelf();
            }



            base.Update();
        }

        public override void Render()
        {
            base.Render();
        }

        public void AnimationUpdate()
        {
            // Choose which animation is playing.
            if(myHealth.Invulnerable)
            {
                this.Graphic.ShakeX = 2;
                spriteSheet.Play("hurt");
                return;
            }
            else
            {
                this.Graphic.ShakeX = 0;
            }

            if(!isMoving)
            {
                spriteSheet.Play("idle");
            }
            else
            {
                spriteSheet.Play("hop");
            }

            if (!myPlatforming.OnGround)
            {
                spriteSheet.Play("fall");
            }
            
        }

        public void OnTakeDamage(Slamscray.Components.HealthDamageComponent.AttackInfo atk)
        {
            // We got hurt! Read the attack information & apply it.
            if(atk.facingLeft)
            {
                myPlatforming.ExtraSpeed.X -= atk.impulseAmt;
            }
            else
            {
                myPlatforming.ExtraSpeed.X += atk.impulseAmt;
            }

            myPlatforming.ExtraSpeed.Y -= atk.impulseAmt * 2;
        }

        // Allows creation from level editor
        public static void CreateFromXML(Scene scene, XmlAttributeCollection attributes)
        {
            // Create enemy at location
            TestEnemy newEnemy = new TestEnemy(int.Parse(attributes["x"].Value), int.Parse(attributes["y"].Value));
            scene.Add(newEnemy);
        }



        
    }
}
