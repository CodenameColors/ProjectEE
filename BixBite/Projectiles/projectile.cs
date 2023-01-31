using System;
using BixBite.Rendering.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Projectiles
{
	public class Projectile
	{

		#region Properties

		public int PosX;
		public int PosY;
		public int velX;
		public int velY;
		public int Mass = 1;
		public int Radius;
		public double BounceMultiplier = -1;
		public double FrictionMultiplier = 1;
		public double GravityMultiplier = 0;
		public int Width;
		public int Height;
		public float ScaleX = 1.0f;
		public float ScaleY = 1.0f;

		public Rectangle DrawRectangle
		{
			get
			{
				_drawRectangle.X = PosX;
				_drawRectangle.Y = PosY;
				_drawRectangle.Width = (int)(Width * ScaleX);
				_drawRectangle.Height = (int)(Height * ScaleY);
				return _drawRectangle;
			}
			set => _drawRectangle = value;
		}

		#endregion

		private Texture2D _texture;
		private Rectangle _drawRectangle;

		public Projectile(int xPos, int yPos, float speed, float direction, Texture2D texture, float scaleX, float scaleY, float gravity = 0)
		{
			this.PosX = xPos;
			this.PosY = yPos;
			this.velX = (int)(Math.Cos(direction) * speed);
			this.velY = (int)(Math.Sin(direction) * speed);
			this.ScaleX = scaleX;
			this.ScaleY = scaleY;
			this.GravityMultiplier = gravity;

			this.Width = texture.Width;
			this.Height = texture.Height;

			this._texture = texture;
		}

		public Projectile(int xPos, int yPos, float speed, float direction, int radius, GraphicsDevice graphicsDevice, float gravity = 0)
		{
			this.PosX = xPos;
			this.PosY = yPos;
			this.velX = (int)(Math.Cos(direction) * speed);
			this.velY = (int)(Math.Sin(direction) * speed);
			this.GravityMultiplier = gravity;

			_texture = new Texture2D(graphicsDevice, radius, radius);
			_texture.CreateCircle(10);
		}

		public Projectile(int xPos, int yPos, float speed, float direction, float gravity = 0)
		{
			this.PosX = xPos;
			this.PosY = yPos;
			this.velX = (int) (Math.Cos(direction) * speed);
			this.velY = (int) (Math.Sin(direction) * speed);
			this.GravityMultiplier = gravity;
		}

		public void Accelerate(int accelX, int accelY)
		{
			this.velX += accelX;
			this.velY += accelY;
		}

		public double AngleTo(Projectile projectile)
		{
			return Math.Atan2(projectile.PosY - this.PosY, projectile.PosX - this.PosX);
		}

		public double distanceTo(Projectile projectile)
		{
			var deltaX = projectile.PosX - this.PosX;
			var deltaY = projectile.PosY - this.PosY;

			return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
		}

		public void GravitateTo(Projectile projectile)
		{
			var deltaX = this.PosX - projectile.PosX;
			var deltaY = this.PosY - projectile.PosY;
			var distSQ = deltaX * deltaX + deltaY * deltaY;
			var distance = Math.Sqrt(distSQ);
			var force = projectile.Mass / distSQ;
			var accelX = deltaX / distance * force;
			var accelY = deltaY / distance * force;

			this.velX += (int)accelX;
			this.velY += (int)accelY;
		}

		private float GetForwardVector()
		{
			Vector2 norm = Vector2.Normalize(new Vector2(velX, velY));

			return (float)(Math.Sin(norm.X) * Math.Cos(norm.Y));
		}

		float VectorToAngle(Vector2 vector)
		{
			return (float)Math.Atan2(vector.Y, vector.X);
		}

		float VectorToAngle()
		{
			return (float)Math.Atan2(this.velY, this.velX);
		}

		public void Update(GameTime gameTime)
		{

			if (velX != 0 || velX != 0)
			{
				this.PosX += velX;
				this.PosY += velY;
				this.PosY = (int)(this.PosY * GravityMultiplier);
			}
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Draw(_texture, DrawRectangle, null, Color.White, (float)VectorToAngle(), 
				Vector2.Zero, SpriteEffects.None, .01f);
		}



	}
}
