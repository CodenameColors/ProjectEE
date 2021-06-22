using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat.Equipables
{

	public enum EClothesType
	{
		NONE = 0,
		Head = 1, 
		Body = 2,
		Legs = 3
	}


	public class Clothes : Equipable
	{

		public int Clothes_Type { get; set; }
		public int Elemental { get; set; }
		public String Function_PTR { get; set; }
		//public int defenseStat = 0;


		//public new List<ModifierData> Traits = new List<ModifierData>(1);
		//public new List<ModifierData> Effects = new List<ModifierData>(1);

		public List<Skill> ClothesSkills = new List<Skill>();

	}
}
