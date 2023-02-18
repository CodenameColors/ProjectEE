using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{

	public enum EStanceType
	{
		NONE,
		Immovable = 1,
		PewPew = 2, //ranged/shooting
		Reading =3 , //For magic increasing
		Praying = 4, //healing
		Focus = 5, //next attack will deal more damage SKILLS ONLY
		Bloody = 6, //sacrifices something random for big boosts
		Singing = 7, //for buffs and debuffs
		Feather = 8
	}


	public class FollowUpAttack
	{
		public String Name { get; set; }
		public String Job_1 { get; set; }
		public String Job_2 { get; set; }
		public String Job_3 { get; set; }
		public String Job_4 { get; set; }
		
		public String Function_PTR { get; set; }

	}

	public class FollowUp_Attacks : FollowUpAttack
	{

	}

}
