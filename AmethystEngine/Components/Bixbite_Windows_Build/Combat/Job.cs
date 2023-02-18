using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{
	public enum EJob
	{
		NONE,
		Hero,					// Basic default stats, but less defense, and more more physical attack.
		Knight,				// Doesn't do a lot of damage, but can tank a lot physical defense
		Sniper,				// They have the ability to use default ammo. User created, npc bought etc.
		Doctor,				// Healer yeppp... that's it. HIGH magic defense
		Necromancer,	// White magic will hurt them if they use it (maybe if they get hit by it?)
		Bandit,				// Steal's shit from the enemies. you can keep it, if the character likes you.
		LuckyStar,		// Lucky star has an increased amount of luck. Helps with luck based skills.
		Gambler,			// Gambler will have the ability to roll a dice with effects/traits on their turn.
	}


	public class Job
	{

		//public EJob CurrentJob { get; set; }
		//public EStanceType LeftStance { get; set; }
		//public EStanceType RightStance { get; set; }

		//THESE MUST MATCH WITH THE DATBASE NAMES (non caps required)
		public int Id { get; set; }
		public String Name { get; set; }
		public int Left_Stance { get; set; }		// FOR THE DATABASE ONLY
		public int Right_Stance { get; set; }		// FOR THE DATABASE ONLY


		public Job(EJob desiredJob)
		{
			this.Id = (int)desiredJob;
			Name = "NONE";
		}

		public Job() {  }

		//STAT BOOST Equations for jobs here.
		//also based on the current weapon being used, and character stats. It's gonna be fun....
	}

}
