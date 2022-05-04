using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DarkestGun
{
    public enum TileCollision
    {
        Passable,
        Impassable,
        Platform,
    }

    public class Tile
    {
        public Texture2D Texture;        
        public Rectangle Source;
        public TileCollision TileCollision;


        public const int Width = 32;
        public const int Height = 32;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(Texture2D texture, Rectangle source, TileCollision tileCollision)
        {
            Texture = texture;
            Source = source;
            TileCollision = tileCollision;
        }
    }
}
