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
        private KeyboardState keyboardState;

        //Contents
        private Texture2D mall;
        private Texture2D toilet;
        private SpriteFont font;
        
        //Public Variables
        public float FrameRate = 0;
        public static Rectangle ScreenDimensions = new Rectangle();

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //Window size
            graphics.PreferredBackBufferWidth = 1600; 
            graphics.PreferredBackBufferHeight = 900;

            ScreenDimensions.Width = graphics.PreferredBackBufferWidth;
            ScreenDimensions.Height = graphics.PreferredBackBufferHeight;

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
            
            //Load HUD

            //Load fonts
            font = Content.Load<SpriteFont>("rumpi");
        }

        private void LoadNextLevel()
        {
            if (level != null)
                level.Dispose();

            string levelPath = "Content/Levels/a.txt";

            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Content, fileStream);
        }

        protected override void Update(GameTime gameTime)
        {
            //Framerate calculation
            FrameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Input polling
            HandleInput(gameTime);

            //Update calls
            level.Update(gameTime, keyboardState);
            camera.Update(level.Player);

            base.Update(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            //Movement inputs
            keyboardState = Keyboard.GetState();
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

            //Draw player position as a string
            Vector2 topLeft = level.Player.Position + new Vector2(10, 10);
            topLeft.X -= ScreenDimensions.Width / 2;
            topLeft.Y -= ScreenDimensions.Height / 2;
            spriteBatch.DrawString(font,
                                    "(" + level.Player.Position.X.ToString("0") + "," + level.Player.Position.Y.ToString("0") + ")",
                                   topLeft,
                                    Color.Black);
            
            //Draw level
            level.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
