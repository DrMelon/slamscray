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
        //TODO: 
        // Generalize combat moves into an object/component thing. So that the combat move checks its own timer etc.
        // That way you can add and remove abilities, and each ability manages itself.
        // Simply check for currentCombatMove & make sure it's not set to anything before performing a combat move.
        public enum MoveState { GROUND, SHORYUKEN, FALL, PUNCH, GRASP, SLAMUP, SLAMDOWN, DASHLEFT, DASHRIGHT };

        // Constants
        public static float SHORYUKEN_DAMAGE = 5.0f;
        public static float SHORYUKEN_INVTIME = 45.0f;
        public static float SHORYUKEN_PUSHAMT = 125.0f;
        public static float SHORYUKEN_FREEZEAMT = 5.0f;
        //public static float SHORYUKEN_FREEZEAMT = 18.0f; // Great for a more powerful hit... hype mode?

        public static float PUNCH_DAMAGE = 1.0f;
        public static float PUNCH_INVTIME = 10.0f;
        public static float PUNCH_PUSHAMT = 50.0f;
        public static float PUNCH_FREEZEAMT = 0.0f;

        public static float MOVESPEED = 150.0f;
        public static float HYPEMOVESPEED = 180.0f;

        public Spritemap<string> spriteSheet;
        private PlatformingMovement myPlatforming;
        private BoxCollider myCollider;
        private Components.HealthDamageComponent myHealth;


        private Sound punchSound;
        private Sound hypePunchSound;
        private Sound shoryukenSound;
        private Sound hypeShoryukenSound;
        private Sound shingSound;

        public float shoryukenTime = 0;
        public float punchTime = 0;
        public float dashTime = 0;

        public MoveState myMoveState;
        public bool shoryukened = false; // used to prevent repeated air-ryukens.

        public bool hypeMode = false; // Hypemode increases damage etc
        public float hypeAmt = 0.0f; // When hypeamt reaches 100, it reduces until 0 and hypemode is engaged.

        public Stormdark(float x = 0, float y = 0)
        {
            // Set default X,Y coords
            X = x;
            Y = y;

            // Create movement
            myPlatforming = new PlatformingMovement(3200.0f, 3200.0f, 4.5f);
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
            spriteSheet.Add("punch", new int[] { 27, 29 }, new float[] { 2f, 8f });
            spriteSheet.Add("grasp", new int[] { 28, 29, 30, 31, 32, 33, 34, 35, 36}, new float[] { 4f, 4f, 2f, 2f, 2f, 2f, 8f, 16f, 16f } );
            spriteSheet.Add("dashflash", new int[] { 37, 38, 39, 40}, new float[] {3f, 3f, 3f, 3f});

            // Use idle animation to start
            spriteSheet.Play("idle");

            

            // Set display to use this spritesheet
            Graphic = spriteSheet;

            // Setup controls
            myPlatforming.JumpButton = Global.playerSession.Controller.A;

            // Health/damage component
            myHealth = new Slamscray.Components.HealthDamageComponent();
            AddComponent(myHealth);

            // Add Components
            AddCollider(myCollider);
            this.Collider = myCollider;
            myPlatforming.Collider = myCollider;
            myPlatforming.AddCollision(0);
            myPlatforming.JumpStrength = 250.0f;
            myPlatforming.VariableJumpHeight = false;
            AddComponent(myPlatforming);

            
            // Make sure to add to pausegroup.
            Group = Global.GROUP_ACTIVEOBJECTS;

            // Load sounds
            punchSound = new Sound(Assets.SND_PUNCH);
            hypePunchSound = new Sound(Assets.SND_PUNCH_HYPE);

            shoryukenSound = new Sound(Assets.SND_SHORYUKEN);
            hypeShoryukenSound = new Sound(Assets.SND_SHORYUKEN_HYPE);

            shingSound = new Sound(Assets.SND_SHING);
            
            
        }
        
        public override void Update()
        {
            


            // Check hype
            if(hypeMode)
            {
                hypeAmt -= 0.1f;
                if(hypeAmt <= 0)
                {
                    // off!
                    hypeMode = false;
                }
            }
            if(hypeAmt >= 100.0f)
            {
                hypeAmt = 100;
                if (!hypeMode)
                {
                    shingSound.Play();
                    Otter.Flash f = new Flash(Color.White);
                    this.Scene.Add(f);
                    hypeMode = true;
                    
                }
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

            if (Global.playerSession.Controller.X.Pressed)
            {
                Attack();
            }
            if (Global.playerSession.Controller.Y.Pressed)
            {
                DashAct();
            }

            
            if(Global.playerSession.Controller.Left.Down)
            {
                myPlatforming.TargetSpeed.X = -MOVESPEED;
                if(hypeMode)
                {
                    myPlatforming.TargetSpeed.X = -HYPEMOVESPEED;
                }
                spriteSheet.FlippedX = true;
            }
            else if (Global.playerSession.Controller.Right.Down)
            {
                myPlatforming.TargetSpeed.X = MOVESPEED;
                if (hypeMode)
                {
                    myPlatforming.TargetSpeed.X = HYPEMOVESPEED;
                }
                spriteSheet.FlippedX = false;
            }
            else
            {
                myPlatforming.TargetSpeed.X = 0.0f;
            }

            myPlatforming.Speed.X = Util.Approach(myPlatforming.Speed.X, myPlatforming.TargetSpeed.X, myPlatforming.CurrentAccel);
            if (myMoveState != MoveState.SHORYUKEN && myMoveState != MoveState.PUNCH && myMoveState != MoveState.DASHLEFT && myMoveState != MoveState.DASHRIGHT)
            {
                myPlatforming.ExtraSpeed.X = Util.Approach(myPlatforming.ExtraSpeed.X, 0, myPlatforming.CurrentAccel);
            }

            if (myPlatforming.Speed.X < 0 && myPlatforming.AgainstWallLeft)
            {
                myPlatforming.Speed.X = 0;
            }
            if (myPlatforming.Speed.X > 0 && myPlatforming.AgainstWallRight)
            {
                myPlatforming.Speed.X = 0;
            }

            // Update shoryuken state
            if (myMoveState != MoveState.SHORYUKEN && myMoveState != MoveState.DASHLEFT && myMoveState != MoveState.DASHRIGHT)
            {
                CheckPunch();
                
            }
            if (myMoveState != MoveState.PUNCH && myMoveState != MoveState.DASHLEFT && myMoveState != MoveState.DASHRIGHT)
            {
                CheckShoryuken();   
            }

            if (myMoveState != MoveState.SHORYUKEN && myMoveState != MoveState.PUNCH)
            {
                CheckDash();
            }


            if (shoryukenTime > 0)
            {
                shoryukenTime--;
            }
            if (punchTime > 0)
            {
                punchTime--;
            }
            if (dashTime > 0)
            {
                dashTime--;
            }
            
            // Update Animation
            UpdateMoveStates();
            UpdateAnimations();



            if (hypeMode)
            {
                MoveTrail();
            }
            
            base.Update();
        }

       

        public void DashAct()
        {
            // Dashing etc
            if(Global.playerSession.Controller.Left.Down)
            {
                // Dash left
                myMoveState = MoveState.DASHLEFT;
                dashTime = 15.0f;

            }
            if(Global.playerSession.Controller.Right.Down)
            {
                // Dash right
                myMoveState = MoveState.DASHRIGHT;
                dashTime = 15.0f;

            }
            
        }

        public void Attack()
        {
          
            // Uppercut
            if (Global.playerSession.Controller.Up.Down && myMoveState != MoveState.SHORYUKEN && shoryukened == false)
            {



                myMoveState = MoveState.SHORYUKEN;
                shoryukenTime = 4 * 9;
                // Leap into the air
                myPlatforming.Speed.Y = -200.0f;
                if(hypeMode)
                {
                    myPlatforming.Speed.Y = -300.0f;
                }
                if (spriteSheet.FlippedX)
                {
                    myPlatforming.ExtraSpeed.X -= 50.0f;
                }
                else
                {
                    myPlatforming.ExtraSpeed.X += 50.0f;
                }
            }

            // Normal Punch
            else if (myPlatforming.OnGround == true)
            {
                myMoveState = MoveState.PUNCH;
                punchTime = 10; //10 frames

            }
        }

        public void Starticles()
        {
            //Push Particles!
            Particle newParticle = new Particle(X + spriteSheet.HalfWidth, Y + spriteSheet.HalfHeight, Rand.Choose<string>(new string[] { Assets.STARTICLE_FLASH_PINK, Assets.STARTICLE_FLASH_CYAN }), 27, 27) { };


            newParticle.FinalX = X - Rand.Float(-65.0f, 65.0f);
            newParticle.FinalY = Y - Rand.Float(-65.0f, 65.0f);
            newParticle.LifeSpan = 60.0f;
            newParticle.Animate = true;
            newParticle.FrameCount = 10;
            newParticle.Layer = this.Layer + 2; // Always spawn behind player
            this.Scene.Add(newParticle);

            newParticle.Start();
        }

        public void MoveTrail()
        {
            if (Global.theGame.Timer % 1.0f == 0)
            {
                //Push Particles!
                Particle newParticle = new Particle(X + spriteSheet.HalfWidth, Y + spriteSheet.HalfHeight, spriteSheet.Texture, spriteSheet.Width, spriteSheet.Height);
                //newParticle.Graphic = this.Graphic;
                //newParticle.Graphic.Color = Color.Cyan;

                newParticle.FinalX = X + spriteSheet.HalfWidth;//+ Rand.Float(-2, 2);
                newParticle.FinalY = Y + spriteSheet.HalfHeight;
                newParticle.FinalAlpha = 0.2f;
                newParticle.Alpha = 0.2f;
                newParticle.LifeSpan = 5.0f;
                newParticle.Image.Frame = this.spriteSheet.CurrentFrame;
                newParticle.FrameCount = this.spriteSheet.Frames;
                newParticle.Animate = false;
                newParticle.Frames = new List<int>() { this.spriteSheet.CurrentFrame };
                newParticle.Layer = this.Layer + 1; // Always spawn behind player
                newParticle.FlipX = this.spriteSheet.FlippedX;
                newParticle.Color = Color.White;
                newParticle.FinalColor = Color.White;
                newParticle.Graphic = newParticle.Image;
                newParticle.Graphic.ShakeX = 6;
                newParticle.Graphic.ShakeY = 6;
                newParticle.Group = Global.GROUP_ACTIVEOBJECTS;
                this.Scene.Add(newParticle);
                
            }

            
        }

        public void HypeWhiteTrail()
        {
            // Nice little white trail, fairly short, made of a shrinking white circle.
            Particle newParticle = new Particle(X + spriteSheet.HalfWidth, Y + spriteSheet.HalfHeight, Assets.PARTICLE_WHITE, 16, 16);
            newParticle.FinalX = X + spriteSheet.HalfWidth;
            newParticle.FinalY = Y + spriteSheet.HalfHeight;
            newParticle.FinalScaleX = 0;
            newParticle.FinalScaleY = 0;
            newParticle.LifeSpan = 20.0f;
            newParticle.Layer = this.Layer + 10;
            this.Scene.Add(newParticle);
        }

        public void CheckShoryuken()
        {

            // Update shoryuken state
            if (myMoveState == MoveState.SHORYUKEN && shoryukenTime > 0)
            {
                myPlatforming.HasJumped = true; //counts as a jump!
                shoryukened = true;    

                if (hypeMode)
                {
                    Starticles();
                }

                // Damage anything we hit with a healthdamage component, and shove it around!
                List<Entity> collisionList = myCollider.CollideEntities(X, Y, myCollider.Tags);
                foreach (Entity ent in collisionList)
                {
                    if(ent == this)
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
                        atk.facingLeft = spriteSheet.FlippedX;
                        atk.impulseAmt = SHORYUKEN_PUSHAMT;
                        if (hypeMode)
                        {
                            atk.impulseAmt = SHORYUKEN_PUSHAMT * 2;
                            dam.Attacked(SHORYUKEN_DAMAGE * 2, atk);
                        }
                        else
                        {
                            dam.Attacked(SHORYUKEN_DAMAGE, atk);
                        }
                        dam.Invulnerable = true;
                        dam.InvulnTime = SHORYUKEN_INVTIME;

                        
                        // Freeze game a sec
                        Global.paused = true;
                        Global.pauseTime = SHORYUKEN_FREEZEAMT;
                        if(hypeMode)
                        {
                            Global.pauseTime = 18.0f;
                            Global.theCameraShaker.ShakeCamera(20.0f);
                            dam.InvulnTime = SHORYUKEN_INVTIME / 6;
                            // Play sound
                            hypeShoryukenSound.Play();
                        }
                        else
                        {
                            // Play sound
                            shoryukenSound.Play();
                        }
                        this.Scene.PauseGroup(Global.GROUP_ACTIVEOBJECTS);
                        hypeAmt += 5.0f;
                    }
                }


            }
            if (shoryukenTime <= 0)
            {
                
                shoryukenTime = 0;
                myPlatforming.ExtraSpeed.X = 0;            
            }
        }

        public void CheckPunch()
        {
            if (myMoveState == MoveState.PUNCH && punchTime > 0)
            {
                // Speed down
                myPlatforming.Speed.X = Util.Approach(myPlatforming.Speed.X, 0, myPlatforming.CurrentAccel / 1.3f);


                

                // Damage anything we hit with a healthdamage component, and shove it around!
                List<Entity> collisionList = myCollider.CollideEntities(X, Y, myCollider.Tags);
                foreach (Entity ent in collisionList)
                {
                    if (ent == this)
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
                        atk.facingLeft = spriteSheet.FlippedX;
                        atk.impulseAmt = PUNCH_PUSHAMT;
                        if(hypeMode)
                        {
                            atk.impulseAmt = PUNCH_PUSHAMT * 2;
                        }
                        dam.Attacked(PUNCH_DAMAGE, atk);
                        dam.Invulnerable = true;
                        dam.InvulnTime = PUNCH_INVTIME;

                        // Freeze game a sec
                        Global.paused = true;
                        Global.pauseTime = PUNCH_FREEZEAMT;
                        
                        this.Scene.PauseGroup(Global.GROUP_ACTIVEOBJECTS);                      
                        
                        if(hypeMode)
                        {
                            Global.theCameraShaker.ShakeCamera();
                            Starticles();
                            //Playsound
                            hypePunchSound.Play();
                        }
                        else
                        {
                            //Playsound
                            punchSound.Play();
                        }
                        hypeAmt += 5.0f;
                    }
                }
            }
            if(punchTime <= 0)
            {
                punchTime = 0;
            }
        }

        public void CheckDash()
        {
            if((myMoveState == MoveState.DASHLEFT || myMoveState == MoveState.DASHRIGHT) && dashTime > 0)
            {
                if(myMoveState == MoveState.DASHLEFT)
                {
                    myPlatforming.ExtraSpeed.X = -650.0f;
                }
                if (myMoveState == MoveState.DASHRIGHT)
                {
                    myPlatforming.ExtraSpeed.X = 650.0f;
                }
            }
        }

        public void UpdateMoveStates()
        {
            if ((punchTime <= 0) && (shoryukenTime <= 0) && (dashTime <= 0))
            {

                    myMoveState = MoveState.GROUND;
                    // Check air
                    if (!myPlatforming.OnGround)
                    {
                        myMoveState = MoveState.FALL;
                    }
                

            }

            if(myPlatforming.HasJumped == false)
            {
                shoryukened = false; 
            }
        }

        public void UpdateAnimations()
        {
            if (myMoveState == MoveState.GRASP)
            {
                spriteSheet.Play("grasp");
                return;
            }
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
            if (myMoveState == MoveState.PUNCH)
            {
                spriteSheet.Play("punch");
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
            if(myMoveState == MoveState.DASHLEFT || myMoveState == MoveState.DASHRIGHT)
            {
                spriteSheet.Play("dashflash");
                HypeWhiteTrail();
            }
        }


        public override void Render()
        {
            base.Render();
            //this.Collider.Render();
        }

    }
}
