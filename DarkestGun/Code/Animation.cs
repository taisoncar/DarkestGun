using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DarkestGun
{
    public class Animation
    {
        public Texture2D Texture;
        public float Interval;
        public bool IsLooping;
        public bool IsPlaying;
        public int FrameWidth, FrameHeight, FrameCount, FrameIndex;
        public float PlayingTime;

        public Animation(Texture2D texture, float interval, bool isLooping)
        {
            Texture = texture;
            Interval = interval;
            IsLooping = isLooping;
            IsPlaying = false;
            FrameWidth = Texture.Height;
            FrameHeight = Texture.Height;
            FrameCount = Texture.Width / FrameWidth;
        }

        public void PlayAnimation()
        {
            if (IsPlaying)
                return;

            FrameIndex = 0;
            PlayingTime = 0.0f;
            IsPlaying = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (!IsPlaying)
                throw new NotSupportedException("No animation is currently playing.");

            PlayingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (PlayingTime > Interval)
            {
                PlayingTime = 0.0f;

                if (IsLooping)
                {
                    FrameIndex = (FrameIndex + 1) % FrameCount;
                }
                else
                {
                    FrameIndex = Math.Min(FrameIndex + 1, FrameCount - 1);
                }
            }

            Rectangle source = new Rectangle(FrameIndex * FrameWidth, 0, FrameWidth, FrameHeight);
            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, FrameWidth, FrameHeight);
            spriteBatch.Draw(Texture, destination, source, Color.White, 0.0f, Vector2.Zero, spriteEffects, 0.0f);
        }
    }
}

