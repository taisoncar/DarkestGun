using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace DarkestGun
{
    public class Main : Game
    {
        //Including other classes
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Camera camera;
        private Level level;

        //Contents
        private Texture2D mall;
        private Texture2D toilet;
        private SpriteFont font;

        //Public Variables
        public float FrameRate = 0;
        public static Rectangle ScreenDimension = new Rectangle();

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //Window size
            graphics.PreferredBackBufferWidth = 1600; 
            graphics.PreferredBackBufferHeight = 900;

            ScreenDimension.Width = graphics.PreferredBackBufferWidth;
            ScreenDimension.Height = graphics.PreferredBackBufferHeight;

            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Load other classes
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadNextLevel();
            camera = new Camera();
            
            //Load sprites
            mall = Content.Load<Texture2D>("mall");
            toilet = Content.Load<Texture2D>("toilet");

            //Load fonts
            font = Content.Load<SpriteFont>("rumpi");
        }

        private void LoadNextLevel()
        {

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            string levelPath = "Content/Levels/a.txt";

            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Content, fileStream);
        }

        protected override void Update(GameTime gameTime)
        {
            //Framerate calculation
            FrameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Update calls
            level.Player.Update(gameTime);
            camera.Update(level.Player);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.FrontToBack,
                              BlendState.AlphaBlend,
                              SamplerState.PointClamp,
                              null,
                              null,
                              null,
                              camera.Transform);

            //Toilet background
            //spriteBatch.Draw(toilet, new Rectangle(0, 0, 1600, 900), Color.White);

            //FPS counter
            //spriteBatch.DrawString(font, FrameRate.ToString("0"), new Vector2(30,30), Color.Black);

            //Draw player position as a string
            /*spriteBatch.DrawString(font,
                                    "(" + level.Player.PlayerPosition.X.ToString("0") + "," + level.Player.PlayerPosition.Y.ToString("0") + ")",
                                   new Vector2(0, -50),
                                    Color.Black);*/
            //Player animation
            level.Draw(gameTime, spriteBatch);

            //Toilet
            //spriteBatch.Draw(toilet, new Vector2(450, 240), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
