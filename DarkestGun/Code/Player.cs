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
		private Level level;

		//Player Variables
		public Vector2 Position;
		public int Speed = 100;				//Speed in pixels/s
		private SpriteEffects flip = SpriteEffects.None;
		private Vector2 velocity = new Vector2();
		public bool IsOnGround;
		private float previousBottom;

		//Animations
		private AnimationSprite idleAnimation, walkAnimation, jumpAnimation;
		private Animation animation;

		private const float MaxJumpTime = 0.5f;
		private const float JumpLaunchVelocity = -3600.0f;
		private const float GravityAcceleration = 3000.0f;
		private const float MaxFallSpeed = 550.0f;
		private const float JumpControlPower = 0.14f;
		 
		private bool isJumping = false;
		private bool wasJumping = false;
		private float jumpTime = 0f;

		private Rectangle localBounds;
		public Rectangle BoundingRectangle
		{
			get
			{
				//return new Rectangle((int)Position.X + 9, (int)Position.Y, 14, 32);
				return new Rectangle((int)Position.X, (int)Position.Y, 32, 32);

				/*int left = (int)Math.Round(Position.X) + localBounds.X;
				int top = (int)Math.Round(Position.Y) + localBounds.Y;

				return new Rectangle(left, top, localBounds.Width, localBounds.Height);*/
			}
		}

		#endregion
		public Player(Level level, Vector2 position) 
        {
			Position = position;
			this.level = level;
			
			//Load animations
			idleAnimation = new AnimationSprite(level.Content.Load<Texture2D>("Sprites/Player/PlayerIdle"), 0.1f, true);
			walkAnimation = new AnimationSprite(level.Content.Load<Texture2D>("Sprites/Player/PlayerWalk"), 0.1f, true);
			jumpAnimation = new AnimationSprite(level.Content.Load<Texture2D>("Sprites/Player/PlayerJump"), 0.05f, false);

			int width = (int)(idleAnimation.FrameWidth * 0.4);
			int left = (idleAnimation.FrameWidth - width) / 2;
			int height = (int)(idleAnimation.FrameHeight);
			int top = idleAnimation.FrameHeight - height;
			localBounds = new Rectangle(left, top, width, height);

			animation = new Animation();
		}

		public void Update(GameTime gameTime, KeyboardState keyboardState)
		{

			velocity.X = 0f;

			//Idle check
			if (!keyboardState.IsKeyDown(Keys.D) && !keyboardState.IsKeyDown(Keys.A) && !keyboardState.IsKeyDown(Keys.S) && !keyboardState.IsKeyDown(Keys.W))
			{
				animation.PlayAnimation(idleAnimation);
			}

			//Sprint check
			if (keyboardState.IsKeyDown(Keys.Q))
			{
				Speed = 200;
				walkAnimation.Interval = 0.07f;
			}
			else
			{
				Speed = 100;
				walkAnimation.Interval = 0.1f;
			}

			//Movement input
			if (keyboardState.IsKeyDown(Keys.D) && keyboardState.IsKeyDown(Keys.A))
			{
				velocity.X = 0f;
			}
			else if (keyboardState.IsKeyDown(Keys.D))
			{
				velocity.X = Speed;
			}
			else if (keyboardState.IsKeyDown(Keys.A))
			{
				velocity.X = -Speed;
			}
			if (keyboardState.IsKeyDown(Keys.W))
			{
				isJumping = true;
			}
				

			//Applying velocity
			ApplyPhysics(gameTime);

			if (IsOnGround)
			{
				if (Math.Abs(velocity.X) - 0.02f > 0)
				{
					animation.PlayAnimation(walkAnimation);
				}
				else
				{
					animation.PlayAnimation(idleAnimation);
				}
			}

			isJumping = false;
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			//Facing direction check
			if (velocity.X < 0)
				flip = SpriteEffects.FlipHorizontally;
			else if (velocity.X > 0)
				flip = SpriteEffects.None;

			RectangleExtension.DrawRectangle(spriteBatch, BoundingRectangle, Color.Red, 1);
			animation.Draw(gameTime, spriteBatch, Position, flip);
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
			IsOnGround = false;

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
									IsOnGround = true;

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

		public void ApplyPhysics(GameTime gameTime)
		{
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

			Vector2 previousPosition = Position;

			// Base velocity is a combination of horizontal movement control and
			// acceleration downward due to gravity.
			velocity.Y = MathHelper.Clamp(velocity.Y + 
				GravityAcceleration 
				* elapsed, -MaxFallSpeed, MaxFallSpeed);

			velocity.Y = DoJump(velocity.Y, gameTime);

			// Apply velocity.
			Position += velocity * elapsed;
			Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

			// If the player is now colliding with the level, separate them.
			HandleCollisions();

			// If the collision stopped us from moving, reset the velocity to zero.

			if (Position.X == previousPosition.X)
				velocity.X = 0;

			if (Position.Y == previousPosition.Y)
				velocity.Y = 0;
		}

		
		private float DoJump(float velocityY, GameTime gameTime)
		{
			// If the player wants to jump
			if (isJumping)
			{
				// Begin or continue a jump
				if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
				{
					jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
					animation.PlayAnimation(jumpAnimation);
				}

				// If we are in the ascent of the jump
				if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
				{
					// Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
					velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
				}
				else
				{
					// Reached the apex of the jump
					jumpTime = 0.0f;
				}
			}
			else
			{
				jumpTime = 0.0f;
			}
			wasJumping = isJumping;

			return velocityY;
		}
	}
}
