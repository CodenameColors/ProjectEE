using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Characters;
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


		public bool bAllies { get; set; }

		public int Elemental { get; set; }
		public int AoE_W { get; set; }
		public int AoE_H { get; set; }

		public int Weapon_Type { get; set; }
		public int Item_Type { get; set; }

		public String Function_PTR { get; set; }


		public List<Skill> ItemSkills = new List<Skill>();

	}

	public class Items : Item
	{

	}

	public class Created_Items : Created_Equipable
	{
		protected readonly Random _randomizer = new Random();

		/// <summary>
		/// Default constructor. used by the SQLite wrapper when loading the database.
		/// </summary>
		public Created_Items()
		{

		}

		/// <summary>
		/// Construct a new Created Item from the player, and or the game.
		/// This will set the puzzle point data for the crafting system to use.
		/// </summary>
		/// <param name="baseItem"></param>
		/// <param name="user"></param>
		public Created_Items(Item baseItem, player user)
		{
			this.Name = baseItem.ID;
			this.Desc = baseItem.Desc;
			this.Stats = baseItem.Stats;
			this.Recipes_Ref_FK = baseItem.ID;
			this.function_ptr = baseItem.Function_PTR;
			this.Item_Type = baseItem.Item_Type;
			this.WeaknessAndStrengths = baseItem.WeaknessAndStrengths;
			this.Stats_FK = baseItem.Stats_FK;
			this.Weakness_Strength_FK = baseItem.Weakness_Strength_FK;

			//Set the traits and effects.
			if (baseItem.Traits.Count > 0)
			{
				for (int i = 0; i < this.Traits.Capacity; i++)
				{
					int listIndex = _randomizer.Next(0, baseItem.Traits.Count - 1);
					if (!baseItem.Traits.Contains(baseItem.Traits[listIndex]))
						this.Traits.Add(baseItem.Traits[listIndex]);
				}
			}

			if (baseItem.Effects.Count > 0)
			{
				for (int i = 0; i < this.Effects.Capacity; i++)
				{
					int listIndex = _randomizer.Next(0, baseItem.Effects.Count - 1);
					if (!baseItem.Effects.Contains(baseItem.Effects[listIndex]))
						this.Effects.Add(baseItem.Effects[listIndex]);
				}
			}

			// Set up the point values that will be used by the crafting system.
			CreateNewItemPoints(baseItem, user);
		}

		/// <summary>
		/// Takes the base item recipe/base template and use that and the user/players levels
		/// to calculate the point values of the item we are creating!
		/// </summary>
		/// <param name="baseItem"></param>
		/// <param name="user"></param>
		public void CreateNewItemPoints(Item baseItem, player user)
		{
			// TODO: MAKE THE USER ALCHEMEY LEVEL USED IN THE POINT EQUATION
			// TODO: CALCULATE THE WEIGHT
			// TODO: ALLOW THE USER TO NAME THE ITEM

			this.size = _randomizer.Next(
				baseItem.PointCoeffiencts.Min_Size, baseItem.PointCoeffiencts.Max_Size);
			this.Rarity = _randomizer.Next(
				baseItem.PointCoeffiencts.Min_Rarity, baseItem.PointCoeffiencts.Max_Rarity);
			this.Quality = _randomizer.Next(
				baseItem.PointCoeffiencts.Min_Quality, baseItem.PointCoeffiencts.Max_Quality);

			//What piece are we going to use?
			long pieceType = 0;
			List<int> possiblePieceTypes = new List<int>();
			for (int i = 0; i < 64; i++)
			{
				if ((baseItem.PointCoeffiencts.possible_piece_types & (0x1 << i)) > 0)
					possiblePieceTypes.Add(i);
			}
			if (possiblePieceTypes.Count > 0)
				this.Piece_Type = _randomizer.Next(0, possiblePieceTypes.Count - 1);

			this.Total_Points = _randomizer.Next(
				baseItem.PointCoeffiencts.Min_Size, baseItem.PointCoeffiencts.Max_Size);

		}

	}

	public class CreatedItem : Created_Items
	{
		

	}
}
