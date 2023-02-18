using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Particles
{
	public class PEmitter
	{

		#region fields

		public const int EXPLODE = 0;

		//Emitter data
		protected Texture2D emittterImage;
		protected Rectangle destRectangle;
		protected Rectangle ScreenBounds;
		protected int numParticles;
		protected float launchFrequency;
		protected int particlesPerLaunch;
		protected bool bCycleParticles;

		public bool bIsActive = false;
		protected bool bLaunched = false;
		protected float timePassed = 0;
		protected int DeadParticles = 0;
		protected int AliveParticles = 0;
		protected List<Particle> particles;

		//particle data
		protected Texture2D particleImage;
		protected float particleWidth;
		protected float particleHeight;
		protected float transparency;
		protected bool bColliableParticle = false;
		protected float gravity;
		protected float wind;
		protected float ScalarX;
		protected float ScalarY;

		//ranged Particle Data
		protected float lowAngle;
		protected float highAngle;

		protected float lowSpeed;
		protected float highSpeed;

		protected float lowLifeSpan;
		protected float highLifeSpan;

		protected Color colorTint;

		protected Random rng;


		#endregion

		#region properties
		public object LinkedParentObject { get; set; }
		#endregion

		#region Constructors

		public PEmitter(Rectangle screenbounds, Texture2D emitterImage, Rectangle destrect, int numParticles,
			float launchFrequency, int particlePerLaunch, bool bCycleParticles, Texture2D particleImage, int particleWidth,
			int particleHeight, float transparency,
			float scalarX, float ScalarY,
			bool bColliable, float gravity, float wind, float lowAngle, float highAngle,
			float lowSpeed, float highSpeed, float lowLifeSpan, float highLifeSpawn, Random rng)
		{
			//store basic emitter data
			this.emittterImage = emitterImage;
			this.destRectangle = destrect;
			this.ScreenBounds = screenbounds;
			this.numParticles = numParticles;
			this.launchFrequency = launchFrequency;
			this.particlesPerLaunch = particlePerLaunch;
			this.bCycleParticles = bCycleParticles;
			this.bColliableParticle = bColliable;

			//setup basic emitter data
			this.bIsActive = false;
			this.timePassed = 0;
			this.particles = new List<Particle>();

			//store general particle used by all particles
			this.particleImage = particleImage;
			this.particleWidth = particleWidth;
			this.particleHeight = particleHeight;
			this.transparency = Math.Min(1.0f, Math.Max(0f, transparency)); //0 -> 1
			this.gravity = gravity;
			this.wind = wind;

			this.ScalarX = scalarX;
			this.ScalarY = ScalarY;

			//store data to be used for random particle data
			this.lowAngle = lowAngle;
			this.highAngle = highAngle;
			this.lowSpeed = lowSpeed;
			this.highSpeed = highSpeed;
			this.lowLifeSpan = lowLifeSpan;
			this.highLifeSpan = highLifeSpawn;

			//randomizer
			this.rng = rng;

			//create all the emitter particles
			for (int i = 0; i < numParticles; i++)
			{
				particles.Add(new Particle(
					particleImage,
					destRectangle,
					gravity, wind,
					rng.Next(((int) lowAngle), (int) highAngle + 1),
					rng.Next(((int) this.lowSpeed), (int) this.highSpeed + 1),
					rng.Next(((int)this.lowLifeSpan), (int)this.highLifeSpan + 1),
					transparency,
					bColliable,
					scalarX, ScalarY,
					screenbounds, rng
				));
			}
		}
		#endregion

		/// <summary>
		/// This method is similar to Reset. However it resets the position of ALL particles back to the emittiers spawn position.
		/// </summary>
		public void HardReset()
		{
			timePassed = 0;
			AliveParticles = 0;
			DeadParticles = 0;

			bIsActive = false;
			bLaunched = false;

			//reset all particles
			for (int i = 0; i < particles.Count; i++)
			{
				particles[i].ResetParticle(GetParticleLaunchPoint());
				--AliveParticles;
			}
		}

		public Vector2 GetScaleVector()
		{
			return new Vector2(ScalarX, ScalarY);
		}

		public Rectangle GetScreenBounds()
		{
			return ScreenBounds;
		}

		public void SetCycleStatus(bool bCycle)
		{
			this.bCycleParticles = bCycle;
		}

		public void SetScreenBounds(Rectangle r)
		{
			for (int i = 0; i < particles.Count; i++)
			{
				particles[i].SetScreenBounds(r);
			}

			this.ScreenBounds = r;
		}

		public void SetParticleScale(float sx, float sy)
		{
			for (int i = 0; i < particles.Count; i++)
			{
				particles[i].SetParticleScaling(sx,sy);
			}
		}

		public void SetParticleTexture(Texture2D texture)
		{
			this.particleImage = texture;
		}

		public void SetColorTint(Color newcolor)
		{
			this.colorTint = newcolor;
		}

		public void StopCyclying()
		{
			bLaunched = true;
			//Reset();
		}

		public void StopCyclyingWithDelay()
		{
			bCycleParticles = false;
			
		}

		public virtual void Update(float deltaTime)
		{
			if (bIsActive)
			{
				//particle explosion
				//if freq is 0 the particles are not active yet... EXPLODE.
				if (launchFrequency == EXPLODE && !bLaunched)
				{
					bLaunched = true;
					for (int i = 0; i < particles.Count; i++)
					{
						if (particles[i].Launch())
						{
							++AliveParticles;
						}
					}
				}
				else if (launchFrequency > 0 && !bLaunched)
				{
					timePassed += deltaTime;
					if (timePassed >= launchFrequency)
					{
						if (bCycleParticles || !(bCycleParticles && DeadParticles + AliveParticles < numParticles))
						{
							timePassed = 0;

							LaunchParticles(particlesPerLaunch);
						}
					}
				}

				//update all active particles and kill off newly dead ones
				for (int i = 0; i < particles.Count; i++)
				{
					if (particles[i].Update(GetParticleLaunchPoint(), deltaTime) == false)
					{
						//particle has died. randomize its trajectory
						particles[i].RandomizeTrajectory(lowSpeed, highSpeed, lowAngle, highAngle);
						//particles[i].Kill();
						++DeadParticles;
					}
				}

				//check for the end of a non cycling emitter
				if (!bCycleParticles && DeadParticles >= numParticles)
				{
					//reset all particles
					for (int i = 0; i < particles.Count; i++)
					{
						particles[i].ResetParticle(GetParticleLaunchPoint());
						--AliveParticles;
					}

					//emitter is done, reset its counting and status data. DOESN'T AUTO LAUNCH
					Reset();
				}

				//Checks for the death of a Cycling Emitter. AND forces the emittier to let all the particles die one by one.
				// NO HARD RENDERING STOP.
				if (bCycleParticles && DeadParticles >= numParticles)
				{
					bool tempb = true;
					if (particles.Any(x => x.IsActive()) == false)
					{
						//reset all particles
						for (int i = 0; i < particles.Count; i++)
						{
							particles[i].ResetParticle(GetParticleLaunchPoint());
							--AliveParticles;
						}
						Reset();
					}
				}
			}
		}
		#region Methods

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color color = default(Color) )
		{
			if (bIsActive)
			{
				if (this.colorTint == default(Color))
				{
					if (color != default(Color))
						this.colorTint = color;
					else
						this.colorTint = Color.White;
				}

				//draw particles
				for (int i = 0; i < particles.Count; i++)
				{
					//draw the current particle
					particles[i].Draw(spriteBatch, colorTint);
				}
			}
		}

		public void Start()
		{
			if (!bIsActive)
			{
				Reset();
				bIsActive = true;
			}
		}

		public void Reset()
		{
			timePassed = 0;
			AliveParticles = 0;
			DeadParticles = 0;

			bIsActive = false;
			bLaunched = false;
		}

		public void LaunchParticles(int numParticles)
		{
			int numLaunched = 0;

			for (int i = 0; i < particles.Count && numLaunched < numParticles; i++)
			{
				if (!particles[i].IsActive())
				{
					if (particles[i].Launch())
					{
						numLaunched++;
						AliveParticles++;
					}
				}
			}
		}


		public bool IsActive()
		{
			return bIsActive;
		}

		public virtual void SetPosition(Vector2 newPos)
		{
			destRectangle.X = (int) newPos.X;
			destRectangle.Y = (int) newPos.Y;

			for (int i = 0; i < particles.Count; i++)
			{
				if (!particles[i].IsActive())
				{
					particles[i].ResetParticle(newPos);
				}
			}
		}

		public Vector2 GetPosition()
		{
			return new Vector2(destRectangle.X, destRectangle.Y);
		}

		protected Vector2 GetParticleLaunchPoint()
		{
			//1. find emitter center
			Vector2 emitterCenter = new Vector2( destRectangle.X, // + (int) (destRectangle.Width * .5),
				destRectangle.Y);  //+ (int) (destRectangle.Height * .5));

			//2.) center the particle and return the x,y cords. then offset by half the particles dimensions
			return new Vector2(emitterCenter.X, //- (int) (particleWidth * ScalarX * 0.5f),
				emitterCenter.Y); //- (int)(particleHeight * ScalarY * 0.5f));
		}


		#endregion

	}
}
