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
        public TileCollision TileCollision;

        public const int Width = 32;
        public const int Height = 32;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(Texture2D texture, TileCollision tileCollision)
        {
            Texture = texture;
            TileCollision = tileCollision;
        }
    }
}
