using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{

	public enum EWeaponType
	{
		NONE = 0,
		OneHandSword = 1,
		TwoHandSword = 2,
		Shield = 4,
		Axe = 8,
		Bow = 16,
		Spear = 32,
		Staff = 64,
		Scythe = 128,
		Rapier = 256,
		Gauntlets = 512,
		Dagger = 1024,
		Wand = 2048,
		Gun = 4096,
		Explosive = 8192
	}

	public class Weapon : Equipable
	{
		public int? Elemental { get; set; }
		public int Weapon_Type { get; set; }

		public List<Skill> WeaponSkills = new List<Skill>();

	}

	public class Weapons : Weapon
	{

	}

}
