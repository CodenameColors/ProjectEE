using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobStanceAnalysis
{
	public enum EJob
	{
		NONE,
		Hero,
		Knight,
		Sniper,
		Doctor,
		Necromancer,
		Bandit,
		LuckyStar,
		Gambler,
	}

	public enum EStanceType
	{
		NONE,
		Immovable,
		PewPew, //ranged/shooting
		Reading, //For magic increasing
		Praying, //healing
		Focus, //next attack will deal more damage SKILLS ONLY
		Bloody, //sacrifices something random for big boosts
		Singing, //for buffs and debuffs
		Feather
	}


	public class Job
	{
		public EJob Name { get; set; }
		//public EStanceType Left { get; set{} }
		//public EStanceType Middle { get; set; }
		//public EStanceType Right { get; set; }

		private  EStanceType[] stances = new EStanceType[3];

		public EStanceType[] Stances
		{
			get => stances;
			set => stances = value;
		}

		public Job()
		{
			
		}

	}
}
