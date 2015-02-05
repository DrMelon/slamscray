//@Author: J. Brown / DrMelon
//@Purpose: This is the main gameplay scene; when a level is loaded, this scene is played using the level data.


using Otter;
using Slamscray;
using Slamscray.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slamscray.Scenes
{
    public class LevelScene : Scene
    {

    

        public OgmoProject levelProject = new OgmoProject(Assets.OGMO_TESTPROJECT, "../../Assets/Graphics/");

        

        public LevelScene()
        {
            // Camera Setup
            this.UseCameraBounds = true;
            this.ApplyCamera = true;
            this.CameraBounds.X = 0;
            this.CameraBounds.Y = 0;
            this.CameraBounds.Width = 400;
            this.CameraBounds.Height = 240;

            // Level Loading
            levelProject.RegisterTag(0, "Coll");
            levelProject.DisplayGrids = false;
            levelProject.LoadLevel(Assets.OGMO_TESTLEVEL, this);
            levelProject.UseCameraBounds = true;


            // Make sure Level's entities are in BG
            foreach (Entity levelEnt in levelProject.Entities.Values)
            {
                levelEnt.Layer = 20;
            }
            
            

            // Character Placement
            Global.thePlayer = new Stormdark(100, 75);
            Global.theCameraShaker = new Utils.CameraShaker();

            Add(Global.theCameraShaker);

            // Add player to scene
            Add(Global.thePlayer);
            


        }


        public override void Update()
        {
            // Don't update if paused.
            if(Global.paused == true)
            {
                Global.pauseTime--;
                if (Global.pauseTime <= 0)
                {
                    Global.pauseTime = 0;
                    Global.paused = false;
                    PauseGroupToggle(Global.GROUP_ACTIVEOBJECTS);
                }
                return;
            }


            base.Update();
        }


        public override void UpdateLast()
        {

            base.UpdateLast();
            // Recentering Camera Based on Player Position & Direction

            float targetCamX = 0;
            float targetCamY = 0;


            if (Global.thePlayer.spriteSheet.FlippedX)
            {
                // Left
                targetCamX = (Global.thePlayer.X + Global.thePlayer.spriteSheet.Width / 2.0f) -16.0f;
            }
            else
            {
                // Right
                targetCamX = (Global.thePlayer.X + Global.thePlayer.spriteSheet.Width / 2.0f) +16.0f;
            }

            targetCamY = Global.thePlayer.Y - 16.0f; // Above player a little.


            targetCamX = Util.Approach(this.CameraCenterX, targetCamX, 3.0f);
            targetCamY = Util.Approach(this.CameraCenterY, targetCamY, 8.0f);

            this.CenterCamera(targetCamX, targetCamY);

        }


    }
}
