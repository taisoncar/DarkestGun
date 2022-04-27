using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DarkestGun
{
	
	public class Player
    {
        #region Declarations

        //Including other classes
        private KeyboardState currentKBState;
		private KeyboardState previousKBState;
		private Level level;

		//Player Variables
		public Vector2 Position;
		public int Speed = 100;		//Speed in pixels/s
		public float Interval;				//Animation frames Interval in seconds
		public bool FacingRight = true;
		public Rectangle SourceRect;
		public Rectangle DestinationRect;
		Vector2 Velocity = new Vector2();
		private bool isOnGround;
		private float previousBottom;

		//Contents
		private Texture2D[] sprite = new Texture2D[2];
		private int[] columnCount = new int[2];

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

		public Rectangle BoundingRectangle
		{
			get
			{
				return new Rectangle((int)Position.X + 9, (int)Position.Y, 14, 32);
			}
		}

		#endregion
		public Player(Level level, Vector2 position) 
        {
			Position = position;
			this.level = level;
			
			//Load player animation sprites
			sprite[(int)PlayerStates.idle] = level.Content.Load<Texture2D>("Sprites/Player/PlayerIdle");
			columnCount[(int)PlayerStates.idle] = 11;

			sprite[(int)PlayerStates.walk] = level.Content.Load<Texture2D>("Sprites/Player/PlayerWalk");
			columnCount[(int)PlayerStates.walk] = 10;
		}

		public void Update(GameTime gameTime)
		{
			
			//Movement inputs
			previousKBState = currentKBState;
			currentKBState = Keyboard.GetState();

			Velocity = Vector2.Zero;

			//Idle check
			if (!currentKBState.IsKeyDown(Keys.D) && !currentKBState.IsKeyDown(Keys.A) && !currentKBState.IsKeyDown(Keys.S) && !currentKBState.IsKeyDown(Keys.W))
			{
				PlayerState = PlayerStates.idle;
			}

			//Sprint check
			if (currentKBState.IsKeyDown(Keys.Q))
			{
				Speed = 200;
				Interval = 0.05f;
			}
			else
			{
				Speed = 100;
				Interval = 0.1f;
			}

			//Movement input
			if (currentKBState.IsKeyDown(Keys.D))
			{
				PlayerState = PlayerStates.walk;
				Velocity.X = Speed;
			}

			if (currentKBState.IsKeyDown(Keys.A))
			{
				PlayerState = PlayerStates.walk;
				Velocity.X = -Speed;
			}

			if (currentKBState.IsKeyDown(Keys.S))
            {
                PlayerState = PlayerStates.walk;
				Velocity.Y = Speed;
			}

            if (currentKBState.IsKeyDown(Keys.W))
			{
				PlayerState = PlayerStates.walk;
				Velocity.Y = -Speed;
			}

			//Applying velocity
			Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

			//Facing direction check
			if (Velocity.X > 0)
				FacingRight = true;
			else if (Velocity.X < 0)
				FacingRight = false;

			HandleCollisions();
			
			//Update frame
			UpdateFrame(gameTime, columnCount[(int)PlayerState]);
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			PrepareFrame(sprite[(int)PlayerState], columnCount[(int)PlayerState]);

			if (FacingRight)
			{
				spriteBatch.Draw(sprite[(int)PlayerState], DestinationRect, SourceRect, Color.White);
			}

            else
            {
				spriteBatch.Draw(sprite[(int)PlayerState], DestinationRect, SourceRect, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally,1);
			}
		}

		public void PrepareFrame(Texture2D texture, int totalColumns)
        {
			int frameWidth = texture.Width / totalColumns;
			int frameHeight = texture.Height;

			SourceRect = new Rectangle(frameWidth * currentFrame, 0, frameWidth, frameHeight);
			DestinationRect = new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);
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

		private void HandleCollisions()
		{
			// Get the player's bounding rectangle and find neighboring tiles.
			Rectangle bounds = BoundingRectangle;
			int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
			int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
			int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
			int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

			// Reset flag to search for ground collision.
			isOnGround = false;

			// For each potentially colliding tile,
			for (int y = topTile; y <= bottomTile; ++y)
			{
				for (int x = leftTile; x <= rightTile; ++x)
				{
					// If this tile is collidable,
					TileCollision collision = level.GetCollision(x, y);
					if (collision != TileCollision.Passable)
					{
						// Determine collision depth (with direction) and magnitude.
						Rectangle tileBounds = level.GetBounds(x, y);
						Vector2 depth = Collision.GetIntersectionDepth(bounds, tileBounds);
						if (depth != Vector2.Zero)
						{
							float absDepthX = Math.Abs(depth.X);
							float absDepthY = Math.Abs(depth.Y);
							
							if (absDepthY < absDepthX)
							{
								// If we crossed the top of a tile, we are on the ground.
								if (previousBottom <= tileBounds.Top)
									isOnGround = true;

								// Ignore platforms, unless we are on the ground.
								if (collision == TileCollision.Impassable || isOnGround)
								{
									// Resolve the collision along the Y axis.
									Position = new Vector2(Position.X, Position.Y + depth.Y);

									// Perform further collisions with the new bounds.
									bounds = BoundingRectangle;
								}
							}
							else if (collision == TileCollision.Impassable) // Ignore platforms.
							{
								// Resolve the collision along the X axis.
								Position = new Vector2(Position.X + depth.X, Position.Y);

								// Perform further collisions with the new bounds.
								bounds = BoundingRectangle;
							}
						}
					}
				}
			}

			// Save the new bounds bottom.
			previousBottom = bounds.Bottom;
		}
	}
}
