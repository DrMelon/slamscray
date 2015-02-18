//@Author: J. Brown / DrMelon
//@Purpose: This is the player's class!


using Otter;
using Slamscray;
using Slamscray.Components;

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
        public enum MoveState { GROUND, SHORYUKEN, FALL, PUNCH, GRASP, SLAMUP, SLAMDOWN, DASHLEFT, DASHRIGHT, DASHDOWN, DASHUP };

        // Constants




        public static float MOVESPEED = 150.0f;
        public static float HYPEMOVESPEED = 180.0f;

        public Spritemap<string> spriteSheet;
        public PlatformingMovement myPlatforming;
        public BoxCollider myCollider;
        public Components.HealthDamageComponent myHealth;


        public Sound punchSound;
        public Sound hypePunchSound;
        public Sound shoryukenSound;
        public Sound hypeShoryukenSound;
        public Sound shingSound;

        public CombatMove currentCombatMove = null;
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
            spriteSheet.Add("dashflash_d", new int[] { 41, 42, 43, 44 }, new float[] { 3f, 3f, 3f, 3f });

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
            if (currentCombatMove == null)
            {
                myPlatforming.ExtraSpeed.X = Util.Approach(myPlatforming.ExtraSpeed.X, 0, myPlatforming.CurrentAccel * 4);
            }

            if (myPlatforming.Speed.X < 0 && myPlatforming.AgainstWallLeft)
            {
                myPlatforming.Speed.X = 0;
            }
            if (myPlatforming.Speed.X > 0 && myPlatforming.AgainstWallRight)
            {
                myPlatforming.Speed.X = 0;
            }


            // Update combat moves
            if(currentCombatMove != null)
            {
                if(currentCombatMove.moveTime > 0)
                {
                    currentCombatMove.Update();
                }
                else
                {
                    currentCombatMove = null;
                }
            }


            // Update shoryuken state
            if (myMoveState != MoveState.SHORYUKEN && myMoveState != MoveState.DASHLEFT && myMoveState != MoveState.DASHRIGHT)
            {
               // CheckPunch();
                
            }
            if (myMoveState != MoveState.PUNCH && myMoveState != MoveState.DASHLEFT && myMoveState != MoveState.DASHRIGHT)
            {
                
            }

            if (myMoveState != MoveState.SHORYUKEN && myMoveState != MoveState.PUNCH)
            {
               // CheckDash();
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
            if (currentCombatMove == null || currentCombatMove.isInterruptable)
            {
                currentCombatMove = null;
                currentCombatMove = new CombatMoveDash();
                CombatMoveDash refer = currentCombatMove as CombatMoveDash;
                refer.DashSpeed = new Speed(1600);
                refer.DashSpeed.X = 0;
                refer.DashSpeed.Y = 0;

                if (Global.playerSession.Controller.Left.Down)
                {
                    // Dash left
                    myMoveState = MoveState.DASHLEFT;
                    refer.DashSpeed.X = -650.0f;

                }
                if (Global.playerSession.Controller.Right.Down)
                {
                    // Dash right
                    myMoveState = MoveState.DASHRIGHT;
                    refer.DashSpeed.X = 650.0f;
                }

                
                if(Global.playerSession.Controller.Down.Down && myPlatforming.OnGround == false)
                {
                    // Dash down / slam
                    myMoveState = MoveState.DASHDOWN;
                    refer.DashSpeed.Y = 450.0f;
                }

                if (Global.playerSession.Controller.Up.Down )
                {
                    // Dash down / slam
                    myMoveState = MoveState.DASHUP;

                    refer.DashSpeed.Y = -450.0f;
                }
                


                currentCombatMove.thePlayer = this;
                currentCombatMove.Startup();
            }
            
        }

        public void Attack()
        {
            if (currentCombatMove == null || currentCombatMove.isInterruptable)
            {
                // Uppercut
                if (Global.playerSession.Controller.Up.Down && shoryukened == false)
                {
                    if (hypeMode)
                    {
                        currentCombatMove = null;
                        currentCombatMove = new CombatMoveHypeShoryuken();
                    }
                    else
                    {
                        currentCombatMove = null;
                        currentCombatMove = new CombatMoveShoryuken();
                    }


                }

                // Normal Punch
                else if (myPlatforming.OnGround == true)
                {
                    if (hypeMode)
                    {
                        currentCombatMove = null;
                        currentCombatMove = new CombatMoveHypePunch();
                    }
                    else
                    {
                        currentCombatMove = null;
                        currentCombatMove = new CombatMovePunch();
                    }
                }



                if(currentCombatMove != null)
                {
                    currentCombatMove.thePlayer = this;
                    currentCombatMove.Startup();
                }
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
            newParticle.FinalX = X + spriteSheet.HalfWidth + Rand.Float(-4, 4);
            newParticle.FinalY = Y + spriteSheet.HalfHeight + Rand.Float(-4, 4);
            newParticle.FinalScaleX = 0;
            newParticle.FinalScaleY = 0;
            newParticle.LifeSpan = 20.0f;
            newParticle.Layer = this.Layer + 10;
            this.Scene.Add(newParticle);
        }


        public void UpdateMoveStates()
        {
            if (currentCombatMove == null)
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
            spriteSheet.FlippedY = false;
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
            if(myMoveState == MoveState.DASHDOWN)
            {
                spriteSheet.Play("dashflash_d");
                HypeWhiteTrail();
            }
            if (myMoveState == MoveState.DASHUP)
            {
                spriteSheet.Play("dashflash_d");
                spriteSheet.FlippedY = true;
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
