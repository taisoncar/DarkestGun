using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DarkestGun
{
    public enum TileCollision
    {
        Passable,
        Impassable,
    }

    public class Tile
    {
        public Texture2D Texture;
        public TileCollision Collision;

        public const int Width = 16;
        public const int Height = 16;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}
