using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{

	public enum EMagicType
	{
		NONE = 0,
		Fire = 1,
		Ice = 2,
		Earth = 4,
		Water = 8,
		Lightning = 16,
		Explosive = 32,
		Shadow = 64,
		Luminous = 128
	}


	public class Skill
	{
		public String Name { get; set; }
		public bool bPhys { get; set; }
		public bool bDamage { get; set; }
		public double Damage_Multiplier { get; set; }
		public int Elemental { get; set; }
		public int Weapon_Type { get; set; }
		public int AOE_w { get; set; }
		public int AOE_h { get; set; }
		public bool bAllies { get; set; }
		public String Modifier_FK { get; set; }
		public String Function_PTR { get; set; }


		private bool _bIsAllowed = true;
		public bool bIsAllowed
		{
			get => _bIsAllowed;
			set => _bIsAllowed = value;
		}

		public ModifierData ModifierLinked;


	}

	public class Skills : Skill
	{
	}

}
