using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slamscray;

//@Author: J. Brown / DrMelon
//@Purpose: This is the CombatMove for the Punch movement.

namespace Slamscray.Components
{
    public class CombatMoveDash : Slamscray.Components.CombatMove
    {

        public static int DASH_TIME = 15;
        public Speed DashSpeed;

        public CombatMoveDash() : base()
        {
            animationToPlay = "dashflash";
            moveLength = DASH_TIME;
            isInterruptable = false;
            moveTime = moveLength;
        }

        public override void Startup()
        {
            //nada
        }

        public override void Update()
        {
            thePlayer.spriteSheet.FlippedY = false;
            // Update shoryuken state
            if (moveTime > 0)
            {
                // Move left/right depending on what direction we dashed in.
                if (DashSpeed.X != 0)
                {
                    thePlayer.myPlatforming.ExtraSpeed.X = DashSpeed.X;
                }
                if (DashSpeed.Y != 0)
                {
                    thePlayer.myPlatforming.Speed.Y = DashSpeed.Y;
                }
                

                
                

                moveTime--;
            }
            if (moveTime <= 0)
            {
                moveTime = 0;

                thePlayer.spriteSheet.FlippedY = false;
            }
        }
    }
}
