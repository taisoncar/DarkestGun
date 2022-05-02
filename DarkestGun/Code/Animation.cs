using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DarkestGun
{
    public class Animation
    {
        private int frameIndex;
        private float PlayingTime;
        private AnimationSprite animationSprite;

        public void PlayAnimation(AnimationSprite animationSprite)
        {
            if (this.animationSprite == animationSprite)
                return;

            this.animationSprite = animationSprite;
            frameIndex = 0;
            PlayingTime = 0.0f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (animationSprite == null)
                throw new NotSupportedException("No animation is currently playing.");

            PlayingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (PlayingTime > animationSprite.Interval)
            {
                PlayingTime -= animationSprite.Interval;

                if (animationSprite.IsLooping)
                {
                    frameIndex = (frameIndex + 1) % animationSprite.FrameCount;
                }
                else
                {
                    frameIndex = Math.Min(frameIndex + 1, animationSprite.FrameCount - 1);
                }
            }

            Rectangle source = new Rectangle(frameIndex * animationSprite.FrameWidth, 0, animationSprite.FrameWidth, animationSprite.FrameHeight);
            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, animationSprite.FrameWidth, animationSprite.FrameHeight);

            spriteBatch.Draw(animationSprite.Texture, destination, source, Color.White, 0.0f, Vector2.Zero, spriteEffects, 0.0f);
        }
    }
}

