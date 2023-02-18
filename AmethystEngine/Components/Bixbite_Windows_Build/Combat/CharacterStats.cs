using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{

	//public enum EStatChange
	//{
	//	NONE,
	//	Health,
	//	Mana,
	//	Essence,
	//	Attack,
	//	Defense,
	//	Dexterity,
	//	Agility,
	//	Morality,
	//	Wisdom,
	//	Resistance,
	//	Luck,
	//	Risk,
	//	Intelligence
	//}

	public class BaseStatChange
	{
		//What type of stat change?
		//public EStatChange StatToChange;
		public BaseStats StatToChange;

		//The Amount to change the stat.
		//public float ChangeValue { get; set; }

		//The length of time in turns this stat change occurs
		public int LengthInTurns { get; set; }

		public bool bIsActive { get; set; }

		public ModifierData LinkedModifierData { get; set; }

		public void DecrementTurnCounter(int decreaseAmount = 1)
		{
			LengthInTurns -= decreaseAmount;
		}
	}

	public class AmmoStatChange
	{
		//What type of stat change?
		//public EStatChange StatToChange;
		public AmmoStats StatToChange;

		//The Amount to change the stat.
		//public float ChangeValue { get; set; }

		//The length of time in turns this stat change occurs
		public int LengthInTurns { get; set; }

		public bool bIsActive { get; set; }

		public ModifierData LinkedModifierData { get; set; }

		public void DecrementTurnCounter(int decreaseAmount = 1)
		{
			LengthInTurns -= decreaseAmount;
		}
	}

	public class StatusEffectChange 
	{
		//Status effect to apply
		public EStatusEffectModifiers StatusEffect;

		////The Amount to change the stat.
		//public float ChangeValue { get; set; }

		//The length of time in turns this stat change occurs
		public int LengthInTurns { get; set; }

		public void DecrementTurnCounter(int decreaseAmount = 1)
		{
			LengthInTurns -= decreaseAmount;
		}
	}

	public class BaseStats
	{
		public int ID { get; set; }

		public int Max_Health { get; set; }
		public int Current_Health { get; set; }
		public int Max_Mana { get; set; }
		public int Current_Mana { get; set; }
		//public int EssencePoints { get; set; }

		public int Attack { get; set; }
		public int Defense { get; set; }
		public int Dexterity { get; set; }
		public int Agility { get; set; }
		public int Morality { get; set; }
		public int Wisdom { get; set; }
		public int Resistance { get; set; }
		public int Luck { get; set; }
		public int Risk { get; set; }
		public int Intelligence { get; set; }
	}
	//public class CharacterStats
	//{

	//}

	public class Base_Stats : BaseStats 
	{

	}

}

public class AmmoStats
{
	public int ID { get; set; }
	public int Current_Ammo { get; set; }
	public int Max_Ammo{ get; set; }
	public int Bullets_Per_Use { get; set; }

	//public class CharacterStats
	//{

	//}

	/// <summary>
	/// This is specifically to make the database happy, while i still get to keep my CamelCase 
	/// </summary>
	public class Ammo_Stats : AmmoStats
	{

}

}
