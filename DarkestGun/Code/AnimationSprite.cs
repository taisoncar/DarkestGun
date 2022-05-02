using System;
using Microsoft.Xna.Framework.Graphics;

namespace DarkestGun
{
    public class AnimationSprite
    {
        public Texture2D Texture;
        public float Interval;
        public bool IsLooping;
        public int FrameWidth, FrameHeight, FrameCount;

        public AnimationSprite(Texture2D texture, float interval, bool isLooping)
        {
            Texture = texture;
            Interval = interval;
            IsLooping = isLooping;
            FrameWidth = Texture.Height;
            FrameHeight = Texture.Height;
            FrameCount = Texture.Width / FrameWidth;
        }
    }
}
