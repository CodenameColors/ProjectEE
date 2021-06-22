using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Combat;

namespace BixBite.Items
{

	public enum EItemType
	{
		NONE = 0,
		Usable = 1,
		Craftable = 2,
		Consumable = 4
	}

	public class Item : Equipable
	{
		public int Elemental { get; set; }
		public int Weapon_Type { get; set; }
		public int Item_Type { get; set; }
		public int AoE_W { get; set; }
		public int AoE_H { get; set; }
		public bool bAllies { get; set; }
		public String Function_PTR { get; set; }


		public List<Skill> ItemSkills = new List<Skill>();

	}

	public class Items : Item
	{

	}

}
