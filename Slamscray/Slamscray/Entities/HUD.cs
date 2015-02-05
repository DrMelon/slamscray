using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;


namespace Slamscray.Entities
{
    class HUD : Entity
    {

        float OffsetX, OffsetY;

        private Spritemap<string> hypeBarBack;
        private Spritemap<string> hypeBarImage;

        public HUD(float x = 0, float y = 0)
        {
            OffsetX = x;
            OffsetY = y;

            hypeBarImage = new Spritemap<string>(Assets.HYPE_BAR, 44, 9); // Load hype bar image
            hypeBarBack = new Spritemap<string>(Assets.HYPE_BAR_BACK, 44, 11); // Load hype bar image
            hypeBarBack.Y -= 1;
            hypeBarImage.Add("flash", new int[] { 15, 16, 17 }, new float[] { 3f, 3f, 3f }); // flash anim
            

            this.Layer = -20; // Always be on top
            this.AddGraphic(hypeBarBack);
            this.AddGraphic(hypeBarImage);
        }

        // The HUD attaches itself to the camera.
        public override void UpdateLast()
        {
            X = this.Scene.CameraX + OffsetX;
            Y = this.Scene.CameraY + OffsetY;

            if(Global.thePlayer.hypeMode)
            {
                // Bar is flashing last 3 frames
                hypeBarImage.Play("flash");
            }
            else
            {
                int curFrame = 0;
                // 14 segments in bar, divide (hypeAmt / 100) * 14
                curFrame = (int)((Global.thePlayer.hypeAmt / 100.0f) * 14.0f);
                hypeBarImage.SetGlobalFrame(curFrame);
            }

            base.UpdateLast();
        }


    }
}
