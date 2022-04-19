﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{
	public class Equipable
	{
		public String ID { get; set; }
		public String Desc = ""; //{ get; set; }
		public bool bDamage { get; set; }
		public int Weight { get; set; }
		public int Rarity { get; set; }
		//public long PieceType { get; set; }
		public long PossiblePieceTypes { get; set; }

		//Foreign Keys
		public int Stats_FK { get; set; }
		public int Weakness_Strength_FK { get; set; }

		public BaseStats Stats = new BaseStats();
		public WeakStrength WeaknessAndStrengths = new WeakStrength();

		public List<ModifierData> Traits = new List<ModifierData>(2);
		public List<ModifierData> Effects = new List<ModifierData>(2);

		// Every single equipable can be used int eh crafting system. Which means 
		//EACH single equipable needs a list of possible piece types they can become on init/pickup
		public List<int> PossiblePieceTypes_List = new List<int>();

	}


	public class Custom_Equipable
	{
		public String ID { get; set; }
		public String Desc = ""; //{ get; set; }
		public bool bDamage { get; set; }
		public int Weight { get; set; }
		public int Rarity { get; set; }
		public long PieceType { get; set; }
		//public long PossiblePieceTypes { get; set; }

		//Foreign Keys
		public int Stats_FK { get; set; }
		public int Weakness_Strength_FK { get; set; }

		public BaseStats Stats = new BaseStats();
		public WeakStrength WeaknessAndStrengths = new WeakStrength();

		public List<ModifierData> Traits = new List<ModifierData>(2);
		public List<ModifierData> Effects = new List<ModifierData>(2);

		// Every single equipable can be used int eh crafting system. Which means 
		//EACH single equipable needs a list of possible piece types they can become on init/pickup
		//public List<int> PossiblePieceTypes_List = new List<int>();

	}

}