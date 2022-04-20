using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DarkestGun
{
    public class Main : Game
    {
        //Including other classes
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Camera camera;
        private Player player;

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
            player = new Player(spriteBatch, Content);
            camera = new Camera();
            
            //Load sprites
            mall = Content.Load<Texture2D>("mall");
            toilet = Content.Load<Texture2D>("toilet");

            //Load fonts
            font = Content.Load<SpriteFont>("rumpi");
        }

        protected override void Update(GameTime gameTime)
        {
            //Framerate calculation
            FrameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Update calls
            player.Update(gameTime);
            camera.Update(player);

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
            spriteBatch.Draw(toilet, new Rectangle(0, 0, 1600, 900), Color.White);

            //FPS counter
            spriteBatch.DrawString(font, FrameRate.ToString("0"), new Vector2(30,30), Color.Black);

            //Draw player position as a string
            spriteBatch.DrawString(font,
                                    "(" + player.PlayerPosition.X.ToString("0") + "," + player.PlayerPosition.Y.ToString("0") + ")",
                                    new Vector2(0, -50),
                                    Color.Black);

            //Player animation
            player.Draw(gameTime);

            //Toilet
            spriteBatch.Draw(toilet, new Vector2(450, 240), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
