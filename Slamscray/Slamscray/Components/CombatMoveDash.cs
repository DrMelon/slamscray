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

        public CombatMoveDash() : base()
        {
            animationToPlay = "dashflash";
            moveLength = DASH_TIME;
            isInterruptable = true;
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

                if (thePlayer.myMoveState == Slamscray.Entities.Stormdark.MoveState.DASHLEFT)
                {
                    thePlayer.myPlatforming.ExtraSpeed.X = -650.0f;
                }
                if (thePlayer.myMoveState == Slamscray.Entities.Stormdark.MoveState.DASHRIGHT)
                {
                    thePlayer.myPlatforming.ExtraSpeed.X = 650.0f;
                }
                if (thePlayer.myMoveState == Slamscray.Entities.Stormdark.MoveState.DASHDOWN)
                {
                    thePlayer.myPlatforming.Speed.Y = 450.0f;
                }
                if (thePlayer.myMoveState == Slamscray.Entities.Stormdark.MoveState.DASHUP)
                {
                    thePlayer.myPlatforming.Speed.Y = -450.0f;
                    thePlayer.spriteSheet.FlippedY = true;
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
