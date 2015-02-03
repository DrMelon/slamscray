//@Author: J. Brown / DrMelon
//@Purpose: Main file for SLAMSCRAY: A GAME ABOUT BEATING UP STUFF WITH MAGIC

// Using Kyle Pulver's Otter2D framework!
using Otter;

using Slamscray;
using Slamscray.Scenes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slamscray
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new window with internal resolution of 400x240 (Widescreen NES)
            Global.theGame = new Game("SLAMSCRAY", 400, 240);

            // Viewing resolution is 720p
            //Global.theGame.SetWindow(400*2, 240*2, false, true);
            Global.theGame.SetWindow(1366, 768, true, true);

            

            // Create player session (allows controls)
            Global.playerSession = Global.theGame.AddSession("Player");

            // Bind Up, Down, Left, Right, Z, X, Space, Esc
            Global.playerSession.Controller.Left.AddKey(Key.Left);
            Global.playerSession.Controller.Right.AddKey(Key.Right);
            Global.playerSession.Controller.Up.AddKey(Key.Up);
            Global.playerSession.Controller.Down.AddKey(Key.Down);
            Global.playerSession.Controller.A.AddKey(Key.Z);
            Global.playerSession.Controller.X.AddKey(Key.X);
            Global.playerSession.Controller.Y.AddKey(Key.C);
            Global.playerSession.Controller.Start.AddKey(Key.Return);

            // Initial game state
            Global.theGame.FirstScene = new LevelScene();

            Global.theGame.Color = new Color("736763");

           
            
           
            
            // Launch window
            Global.theGame.Start();

        }
    }
}
