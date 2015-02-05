using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;

//@Author: J. Brown / DrMelon
//@Purpose: The SpriteTrail class allows the creation of an after-image effect. The object is attached to a sprite, and then every
// nth frame it creates an afterimage which fades over time.
// MAYBE NOT NEEDED
namespace Slamscray.Utils
{
    class SpriteTrail : Otter.Entity
    {
        private Entity attachTo = null;
        public float UpdateEveryNthFrame = 5.0f; // every 5th frame by default
        public Color tint = Color.White; // colorize frame?
        public float LifetimeOfEffect = 30.0f; // how many frames a thing stays
        public bool Emitting = false;
        public List<Entity> createdSprites;

        public SpriteTrail()
        {
            createdSprites = new List<Entity>();
        }

        public void AttachTo(Entity attach)
        {
            attachTo = attach;
        }

        public override void Update()
        {
            if((attachTo != null) && (Emitting || createdSprites.Count > 0))
            {
                // Create more entities here, using the entity's graphics.
                if(Global.theGame.Timer % UpdateEveryNthFrame == 0)
                {
                    Entity newEnt = new Entity();
                    newEnt.X = attachTo.X;
                    newEnt.Y = attachTo.Y;
                    newEnt.Graphic = attachTo.Graphic;
                    newEnt.Graphic.Color = tint;
                    newEnt.LifeSpan = LifetimeOfEffect;
                    createdSprites.Add(newEnt);
                    attachTo.Scene.Add(newEnt);
                }
            }


            base.Update();
        }
    }
}
