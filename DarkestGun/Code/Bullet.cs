using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace DarkestGun
{
    public class Bullet
    {
        private Vector2 destination, origin, direction, position;
        private float rotation;
        private Texture2D texture;
        private float speed = 50f;
        Color color;

        public Bullet(Texture2D texture, Vector2 origin, Vector2 destination)
        {
            position = origin;
            this.destination = destination;
            direction = Vector2.Normalize(destination - origin);

            this.texture = texture;

            int r, g, b;
            Random rand = new Random();
            {
                r = rand.Next(255);
                g = rand.Next(255);
                b = rand.Next(255);
            }

            color = new Color(r, g, b);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += speed * elapsed * direction;
            rotation = (float)Math.Atan2(direction.Y, direction.X);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            
            
            spriteBatch.Draw(texture, position, source, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f );
        }
    }
}
