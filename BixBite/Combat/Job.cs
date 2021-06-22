using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeEditor.Components;

namespace BixBite.Combat
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


	public class Job
	{

		//public EJob CurrentJob { get; set; }
		//public EStanceType LeftStance { get; set; }
		//public EStanceType RightStance { get; set; }

		//THESE MUST MATCH WITH THE DATBASE NAMES (non caps required)
		public int Id { get; set; }
		public String Name { get; set; }
		public int Left_Stance { get; set; } //FOR THE DATABASE ONLY
		public int Right_Stance { get; set; } //FOR THE DATBASE ONLY


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
