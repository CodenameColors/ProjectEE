using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat.Equipables
{

	public enum EAccessoryType
	{
		NONE = 0,
		Ring = 1,
		Neckalace = 2,
		Bracelet = 3,
		Headband = 4,

	}

	public class Accessory : Equipable
	{
		public int Elemental { get; set; }
		public int Accessory_Type { get; set; }
		public String Function_PTR { get; set; }

		public List<Skill> AccessorySkills = new List<Skill>();

	}

	public class Accessories : Accessory
	{

	}

}
