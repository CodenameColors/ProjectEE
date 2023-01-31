using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Particles
{
	public class Particle
	{

		#region Fields
		//launch status
		private bool bIsActive = false;

		//Movement data.
		private float gravity;
		private float wind;
		private float angle;
		private float speed;

		//image data
		private Texture2D img;
		private Rectangle destRectangle;
		private float transparency;
		private float starttransparency;

		//vectors to collect the related data.
		private Vector2 pos = Vector2.Zero;
		private Vector2 traj = Vector2.Zero;
		private Vector2 forces = Vector2.Zero;

		//start vectors for simple resets.
		private Vector2 startpos = Vector2.Zero;
		private Vector2 starttraj = Vector2.Zero;
		private Vector2 startforces = Vector2.Zero;

		//collision data
		private bool bIsColliable = false;
		private Rectangle screenBounds = new Rectangle(0,0,1,1);

		//life span
		private int lifeSpan;
		private int lifeSpent;

		private Random rng;

		#endregion

		#region Properites

		#endregion

		#region Constructors
		public Particle(Texture2D newImg, Rectangle newDestRect, float newGravity, float newWind, float newAngle, 
			float newSpeed, int newLifeSpan, float newTransparency, bool bIsColliable,
			float scalarX, float ScalarY,
			Rectangle newScreenBounds, Random newrng)

		{
			this.img = newImg;
			this.destRectangle = newDestRect;
			this.gravity = newGravity;
			this.wind = newWind;
			this.angle = newAngle;
			this.speed = newSpeed;
			this.lifeSpan = newLifeSpan;
			this.transparency = newTransparency;
			this.bIsColliable = bIsColliable;
			this.screenBounds = newScreenBounds;
			this.rng = newrng; //reused from others

			//scaling
			destRectangle.Width = (int)(destRectangle.Width * scalarX);
			destRectangle.Height = (int)(destRectangle.Height * ScalarY);


			//calculate and assign the projectile data
			startpos = new Vector2(destRectangle.X, destRectangle.Y);
			starttraj = CalculateLaunchTrajectory(speed, angle);
			startforces = SetForceVector(wind, gravity);

		}
		#endregion

		#region methods

		public void SetParticleScaling(float scalarX, float scalarY)
		{
			destRectangle.Width = (int)(img.Width * scalarX);
			destRectangle.Height = (int)(img.Height * scalarY);
		}

		public void Kill()
		{
			bIsActive = false;
		}

		public bool Update(Vector2 launchPos, float deltaTime)
		{
			if (bIsActive)
			{
				lifeSpent += (int) deltaTime;
				if (lifeSpent >= lifeSpan)
				{
					ResetParticle(launchPos); //its dead so reset it.

					return false; //make sure to return the particles alive state.
				}

				UpdateProjectile(deltaTime);

				if (bIsColliable)
				{
					ProjectileCollision();
				}
				//Make sure the particle is WITHIN the bounds of the sub screen we set up
				if ((pos.X < screenBounds.X + screenBounds.Width && pos.X > screenBounds.X) &&
				    pos.Y < screenBounds.Y + screenBounds.Height && pos.Y > screenBounds.Y) { }
				else
				{
					//its not in the bounds. So kill it.
					Kill();
				}
			}
			return true;
		}

		public void Draw(SpriteBatch spriteBatch, Color color)
		{
			if (bIsActive)
			{
				spriteBatch.Draw(img, destRectangle, color * 1);
			}
		}

		public void SetScreenBounds(Rectangle r)
		{
			this.screenBounds = r;
		}

		public bool IsActive()
		{
			return bIsActive;
		}

		public void SetTransparency(float newtransparency)
		{
			transparency = Math.Min(1.0f, Math.Max(0f, newtransparency));
		}

		public float GetTransparency()
		{
			return transparency;
		}

		private float GetLifeSpawn()
		{
			return lifeSpan;
		}

		public int GetLifeSpent()
		{
			return lifeSpent;
		}

		public bool Launch()
		{
			if (!bIsActive)
			{
				bIsActive = true;

				return true;
			}
			return false;
		}

		public void ResetParticle(Vector2 launchpos)
		{
			pos = CopyVector(launchpos);
			traj = CopyVector(starttraj);
			forces = CopyVector(startforces);
			destRectangle.X = (int) pos.X;
			destRectangle.Y = (int) pos.Y;

			lifeSpent = 0;
			SetTransparency(starttransparency);

			bIsActive = false;
		}

		private Vector2 CalculateLaunchTrajectory(float speed, float angle)
		{
			return new Vector2(speed * (float)Math.Cos(MathHelper.ToRadians(angle)),
											   speed * (float)Math.Sin(MathHelper.ToRadians(angle))
												);
		}

		private Vector2 SetForceVector(float wind, float gravity)
		{
			return new Vector2(wind, gravity);
		}

		private void UpdateProjectile(float deltaTime)
		{
			float time = deltaTime * .001f; //to seconds
			traj = AddVectors(traj, ScaleVector(forces, time)); //scale down in relative to deltatime


			//adjust the projectiles position based on its trajectory (move per seconds) but multiply
			//that update by the time passed.
			pos.X += (traj.X * time);
			pos.Y += (traj.Y * time);

			destRectangle.X = (int) pos.X;
			destRectangle.Y = (int) pos.Y;
			
		}

		private void ProjectileCollision()
		{
			//floor collision then flip the Y traj and reduce it by 20% - 40% speed.
			if (destRectangle.Y >= screenBounds.Height - destRectangle.Height)
			{
				//set the position to ground level and launch state to false
				pos.Y = screenBounds.Height - destRectangle.Height;
				destRectangle.Y = (int) pos.Y;
				traj.Y *= -1;
				traj = ScaleVector(traj, ((float) rng.Next(2, 5) / 10f));
			}

			//check top wall
			if (destRectangle.Y <= 0)
			{
				//set the position to roof level and reserve y Traj
				pos.Y = 0;
				destRectangle.Y = (int) pos.Y;
				traj.Y *= -1;
			}

			//check right wall
			if (destRectangle.X + destRectangle.Width >= screenBounds.Width)
			{
				pos.X = screenBounds.Width - destRectangle.Width;
				destRectangle.X = (int) pos.X;
				traj.X *= -1;
			}

			//check left wall
			if (destRectangle.X <= 0)
			{
				pos.X = 0;
				destRectangle.X = (int) pos.X;
				traj.X *= -1;
			}

		}


		public void RandomizeTrajectory(float lowspeed, float highspeed, float lowangle, float highangle)
		{
			if (!bIsActive)
			{
				//Randomize speed and angle
				speed = rng.Next((int) lowspeed, (int) highspeed + 1);
				angle = rng.Next((int) lowangle, (int) highangle+ 1);

				//Recalc trajectories
				starttraj = CalculateLaunchTrajectory(speed, angle);
				startforces = SetForceVector(wind, gravity);

				//copy start data into current data variabls
				traj = CopyVector(starttraj);
				forces = CopyVector(startforces);
			}
		}

		private Vector2 CopyVector(Vector2 vec)
		{
			return  new Vector2(vec.X, vec.Y);
		}

		private Vector2 ScaleVector(Vector2 vec, float scaler)
		{
			return  new Vector2(vec.X * scaler, vec.Y * scaler);
		}

		private Vector2 AddVectors(Vector2 vec1, Vector2 vec2)
		{
			return new Vector2(vec1.X + vec2.X, vec1.Y + vec2.Y);
		}
		#endregion

	}
}
