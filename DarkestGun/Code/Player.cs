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
		public int Speed = 100;				//Speed in pixels/s
		public float Interval;				//Animation frames Interval in seconds
		public bool FacingRight = true;
		private SpriteEffects flip = SpriteEffects.None;
		public Rectangle SourceRect;
		public Rectangle DestinationRect;
		Vector2 Velocity = new Vector2();
		private bool isOnGround;
		private float previousBottom;

		//Animations
		private Animation idleAnimation, walkAnimation;

		//Frame timing variables
		private int currentFrame = 0;
		private int totalFrames;
		private float timer = 0;

		/*public enum PlayerStates
		{
			idle,
			walk
		}
		public static PlayerStates PlayerState;*/

		public Rectangle BoundingRectangle
		{
			get
			{
				return new Rectangle((int)Position.X + 9 , (int)Position.Y, 14, 32);
			}
		}

		#endregion
		public Player(Level level, Vector2 position) 
        {
			Position = position;
			this.level = level;
			
			//Load animations
			idleAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Player/PlayerIdle"), 0.1f, true);
			walkAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Player/PlayerWalk"), 0.1f, true);
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState)
		{
			
			//Movement inputs
			previousKBState = currentKBState;
			currentKBState = keyboardState;

			Velocity = Vector2.Zero;

			//Idle check
			if (!currentKBState.IsKeyDown(Keys.D) && !currentKBState.IsKeyDown(Keys.A) && !currentKBState.IsKeyDown(Keys.S) && !currentKBState.IsKeyDown(Keys.W))
			{
				idleAnimation.PlayAnimation();
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
				walkAnimation.PlayAnimation();
				Velocity.X = Speed;
			}

			else if (currentKBState.IsKeyDown(Keys.A))
			{
				walkAnimation.PlayAnimation();
				Velocity.X = -Speed;
			}

			if (currentKBState.IsKeyDown(Keys.S))
            {
				walkAnimation.PlayAnimation();
				Velocity.Y = Speed;
			}

            else if (currentKBState.IsKeyDown(Keys.W))
			{
				walkAnimation.PlayAnimation();
				Velocity.Y = -Speed;
			}

			//Applying velocity
			Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

			//Facing direction check
			if (Velocity.X < 0)
				flip = SpriteEffects.FlipHorizontally;
			else if (Velocity.X > 0)
				flip = SpriteEffects.None;

			HandleCollisions();
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (Velocity.X < 0) 
				walkAnimation.Draw(gameTime, spriteBatch, Position, flip);
			else if (Velocity.X > 0)
				walkAnimation.Draw(gameTime, spriteBatch, Position, flip);
			else
				idleAnimation.Draw(gameTime, spriteBatch, Position, flip);
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
					if (collision == TileCollision.Impassable)
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

									// Resolve the collision along the Y axis.
									Position = new Vector2(Position.X, Position.Y + depth.Y);

									// Perform further collisions with the new bounds.
									bounds = BoundingRectangle;
							}
							else
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
