using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Particles
{
	public class HitSparkPEmitter : PEmitter
	{
		public HitSparkPEmitter(Rectangle screenbounds, Texture2D emitterImage, Rectangle destrect,
			int numParticles, float launchFrequency, int particlePerLaunch, bool bCycleParticles, 
			Texture2D particleImage, 
			int particleWidth, int particleHeight, float transparency, float scalarX, float ScalarY, bool bColliable, 
			float gravity, float wind, float lowAngle, float highAngle, float lowSpeed, float highSpeed, float lowLifeSpan, float highLifeSpawn, 
			Random rng) : base(screenbounds, emitterImage, destrect, numParticles, 0, particlePerLaunch, false,
			particleImage, particleWidth, particleHeight, transparency, scalarX, ScalarY, false, 0, 0, 
			0, 360, 400, 500, 100, 200, rng)
		{



		}
	}
}
