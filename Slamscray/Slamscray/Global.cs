//@Author: J. Brown / DrMelon
//@Purpose: A global variable container. Probably unwise to use.

using Otter;
using Slamscray;
using Slamscray.Entities;
using Slamscray.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slamscray
{
    class Global
    {

        public static Game theGame = null;
        public static Session playerSession = null;
        public static Stormdark thePlayer = null;
        public static CameraShaker theCameraShaker = null;
        public static float pauseTime = 0;
        public static bool paused = false;     

        public static int GROUP_ACTIVEOBJECTS = 1;

    }
}
