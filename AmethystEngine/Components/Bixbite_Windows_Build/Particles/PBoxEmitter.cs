using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Particles
{

	public class PBoxEmitter : PEmitter
	{
		private Rectangle emitterBounds;
		public int emitterMoveRate;


		public PBoxEmitter( Rectangle EmitterBounds, int EmitterMoveRate,
			Rectangle screenbounds, Texture2D emitterImage, Rectangle destrect, 
			int numParticles, float launchFrequency, int particlePerLaunch, bool bCycleParticles, 
			Texture2D particleImage, int particleWidth, int particleHeight, float transparency, 
			float scalarX, float ScalarY,
			bool bColliable, float gravity, float wind, 
			float lowAngle, float highAngle, float lowSpeed, float highSpeed, 
			float lowLifeSpan, float highLifeSpawn, Random rng) : base(screenbounds, emitterImage, destrect, numParticles, launchFrequency, particlePerLaunch, bCycleParticles, particleImage, particleWidth, particleHeight, transparency, scalarX, ScalarY, bColliable, gravity, wind, lowAngle, highAngle, lowSpeed, highSpeed, lowLifeSpan, highLifeSpawn, rng)
		{
			this.emitterBounds = EmitterBounds;
			this.emitterMoveRate = EmitterMoveRate;


		}

		public Rectangle GetEmitterBounds()
		{
			return emitterBounds;
		}

		public void SetNewEmitterBounds(Rectangle r)
		{
			this.emitterBounds = r;
		}
		

		public override void SetPosition(Vector2 newPos)
		{

			base.SetPosition(newPos);
		}

		public override void Update(float deltaTime)
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
					if (timePassed >= emitterMoveRate)
					{
						//Get a new point in the bounds.
						int x = rng.Next(emitterBounds.X, emitterBounds.X + emitterBounds.Width);
						int y = rng.Next(emitterBounds.Y, emitterBounds.Y + emitterBounds.Height);

						SetPosition(new Vector2(x, y));
					}
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
	}
}
