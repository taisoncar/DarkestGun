using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DarkestGun
{
	
	public class Player
    {
		//Including other classes
		private KeyboardState currentKBState;
		private KeyboardState previousKBState;

		//Player Variables
		public Vector2 PlayerPosition;
		public int PlayerSpeed = 100;					//Speed in pixels/s
		public float Interval;						//Animation frames Interval in seconds
		public bool FacingRight = true;
		public Rectangle SourceRect;
		public Rectangle DestinationRect;
		Vector2 PlayerVelocity = new Vector2();

		//Contents
		private Texture2D[] PlayerSprite = new Texture2D[2];
		private int[] PlayerSpriteColumnCount = new int[2];

		//Frame timing variables
		private int currentFrame = 0;
		private int totalFrames;
		private float timer = 0;

		public enum PlayerStates
		{
			idle,
			walk
		}
		public PlayerStates PlayerState;

		public Player(Level level, Vector2 position) 
        {
			PlayerPosition = position;
			
			//Load player animation sprites
			PlayerSprite[(int)PlayerStates.idle] = level.Content.Load<Texture2D>("PlayerIdle");
			PlayerSpriteColumnCount[(int)PlayerStates.idle] = 11;

			PlayerSprite[(int)PlayerStates.walk] = level.Content.Load<Texture2D>("PlayerWalk");
			PlayerSpriteColumnCount[(int)PlayerStates.walk] = 10;
		}

		public void Update(GameTime gameTime)
		{
			
			//Movement inputs
			previousKBState = currentKBState;
			currentKBState = Keyboard.GetState();

			PlayerVelocity = Vector2.Zero;

			//Idle check
			if (!currentKBState.IsKeyDown(Keys.D) && !currentKBState.IsKeyDown(Keys.A) && !currentKBState.IsKeyDown(Keys.S) && !currentKBState.IsKeyDown(Keys.W))
			{
				PlayerState = PlayerStates.idle;
			}

			//Sprint check
			if (currentKBState.IsKeyDown(Keys.Space))
			{
				PlayerSpeed = 200;
				Interval = 0.05f;
			}
			else
			{
				PlayerSpeed = 100;
				Interval = 0.1f;
			}

			//Movement input
			if (currentKBState.IsKeyDown(Keys.D))
			{
				PlayerState = PlayerStates.walk;
				PlayerVelocity.X = PlayerSpeed;
			}

			if (currentKBState.IsKeyDown(Keys.A))
			{
				PlayerState = PlayerStates.walk;
				PlayerVelocity.X = -PlayerSpeed;
			}

			if (currentKBState.IsKeyDown(Keys.S))
            {
                PlayerState = PlayerStates.walk;
				PlayerVelocity.Y = PlayerSpeed;
			}

            if (currentKBState.IsKeyDown(Keys.W))
			{
				PlayerState = PlayerStates.walk;
				PlayerVelocity.Y = -PlayerSpeed;
			}

			//Applying velocity
			PlayerPosition += PlayerVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

			//Facing direction check
			if (PlayerVelocity.X > 0)
				FacingRight = true;
			else if (PlayerVelocity.X < 0)
				FacingRight = false;

			//Update frame
			UpdateFrame(gameTime, PlayerSpriteColumnCount[(int)PlayerState]);
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			PrepareFrame(PlayerSprite[(int)PlayerState], PlayerSpriteColumnCount[(int)PlayerState]);

			if (FacingRight)
			{
				spriteBatch.Draw(PlayerSprite[(int)PlayerState], DestinationRect, SourceRect, Color.White);
			}

            else
            {
				spriteBatch.Draw(PlayerSprite[(int)PlayerState], DestinationRect, SourceRect, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally,1);
			}
		}

		public void PrepareFrame(Texture2D texture, int totalColumns)
        {
			int frameWidth = texture.Width / totalColumns;
			int frameHeight = texture.Height;

			SourceRect = new Rectangle(frameWidth * currentFrame, 0, frameWidth, frameHeight);
			DestinationRect = new Rectangle((int)PlayerPosition.X, (int)PlayerPosition.Y, frameWidth, frameHeight);
		}

		public void UpdateFrame(GameTime gameTime, int totalColumns)
		{
			totalFrames = totalColumns;
			if (currentKBState != previousKBState)
			{
				currentFrame = 0;
			}

			timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (timer > Interval)
			{
				currentFrame++;

				if (currentFrame >= totalFrames-1)
				{
					currentFrame = 0;
				}
				timer = 0f;
			}
		}
	}
}
