using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slamscray;
using Slamscray.Entities;

//@Author: J. Brown / DrMelon
//@Purpose: Combat moves are used to attack enemies and alter the player's movement. 
// Other moves can't be performed while a combat move is being performed.
// Combat moves have an associated animation, a length (in frames), a reference to the player, and their own behaviours that they define.
// Combat moves have a Startup function, which does things like pushing the player into the air etc when the move begins.
// Combat moves have an Update function which allows the move to play out and affect stuff.

namespace Slamscray.Components
{
    class CombatMove
    {
        public Stormdark thePlayer = null;
        public string animationToPlay = "";
        public int moveLength = 0;

        private int moveTime;

        CombatMove()
        {
            moveTime = moveLength;    
        }

        public void Startup()
        {
        }

        public void Update()
        {
        }

        

    }
}
