using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace DarkestGun
{
    public class Level : IDisposable
    {
        #region Declarations

        // Physical structure of the level.
        private Tile[,] tiles;
        //private Texture2D[] layers;
        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;

        // Entities in the level.
        public Player Player;

        // Key locations in the level.        
        private Vector2 start;

        // Level Content.        
        public ContentManager Content;

        #endregion

        #region Loading

        public Level(ContentManager content, Stream fileStream)
        {
            // Create a new Content manager to load Content used just by this level.
            Content = content;

            LoadTiles(fileStream);
        }

        private void LoadTiles(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '0':
                    return LoadTileContent("0", new Rectangle(0, 0, Tile.Width, Tile.Height), TileCollision.Passable);

                // Impassable blocks
                case '1':
                    return LoadTileContent("ToiletAtlas", new Rectangle(1 * Tile.Width, 2 * Tile.Height, Tile.Width, Tile.Height), TileCollision.Impassable);
                case '2':
                    return LoadTileContent("ToiletAtlas", new Rectangle(1 * Tile.Width, 1 * Tile.Height, Tile.Width, Tile.Height), TileCollision.Impassable);
                case '3':
                    return LoadTileContent("ToiletAtlas", new Rectangle(0 * Tile.Width, 1 * Tile.Height, Tile.Width, Tile.Height), TileCollision.Impassable);
                case '4':
                    return LoadTileContent("ToiletAtlas", new Rectangle(2 * Tile.Width, 1 * Tile.Height, Tile.Width, Tile.Height), TileCollision.Impassable);
                case '5':
                    return LoadTileContent("ToiletAtlas", new Rectangle(1 * Tile.Width, 0 * Tile.Height, Tile.Width, Tile.Height), TileCollision.Impassable);
                case '6':
                    return LoadTileContent("ToiletAtlas", new Rectangle(0 * Tile.Width, 0 * Tile.Height, Tile.Width, Tile.Height), TileCollision.Impassable);
                case '7':
                    return LoadTileContent("ToiletAtlas", new Rectangle(2 * Tile.Width, 0 * Tile.Height, Tile.Width, Tile.Height), TileCollision.Impassable);
                case '8':
                    return LoadTileContent("ToiletAtlas", new Rectangle(0 * Tile.Width, 2 * Tile.Height, Tile.Width, Tile.Height), TileCollision.Impassable);
                case '9':
                    return LoadTileContent("ToiletAtlas", new Rectangle(2 * Tile.Width, 2 * Tile.Height, Tile.Width, Tile.Height), TileCollision.Impassable);

                // Passable block
                case 'a':
                    return LoadTileContent("a", new Rectangle(0, 0, Tile.Width, Tile.Height), TileCollision.Passable);

                case 'b':
                    return LoadTileContent("b", new Rectangle(0, 0, Tile.Width, Tile.Height), TileCollision.Passable);

                // Player 1 start point
                case 'P':
                    return LoadStartTile(x, y);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        private Tile LoadTileContent(string name, Rectangle source, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), source, collision);
        }

        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start.X = GetBounds(x, y).X;
            start.Y = GetBounds(x, y).Y;
            Player = new Player(this, start);

            return new Tile(null, new Rectangle(0, 0, Tile.Width, Tile.Height), TileCollision.Passable);
        }

        /// <summary>
        /// Unloads the level Content.
        /// </summary>
        public void Dispose()
        {
            Content.Unload();
        }

        #endregion

        #region Bounds and collision

        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;

            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].TileCollision;
        }
   
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            Player.Update(gameTime, keyboardState);
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //for (int i = 0; i <= EntityLayer; ++i)
            //    spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);

            DrawTiles(spriteBatch);

            Player.Draw(gameTime, spriteBatch);

            //for (int i = EntityLayer + 1; i < layers.Length; ++i)
            //    spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
        }

        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        RectangleExtension.DrawRectangle(spriteBatch, GetBounds(x, y), Color.Green, 1);
                        spriteBatch.Draw(texture, position, tiles[x, y].Source, Color.White);
                    }
                }
            }
        }

        #endregion
    }
}
