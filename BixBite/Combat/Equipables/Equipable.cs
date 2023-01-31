using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Crafting;
using Microsoft.Xna.Framework;

namespace BixBite.Combat
{

	public class Point_Coeffiencts
	{
		public int ID { get; set; }
		public int Max_Size { get; set; }
		public int Min_Size { get; set; }
		public int Min_Quality { get; set; }
		public int Max_Quality { get; set; }
		public int Min_Rarity { get; set; }
		public int Max_Rarity { get; set; }
		public int Min_Points { get; set; }
		public int Max_Points { get; set; }
		public long possible_piece_types { get; set; }

		public Point_Coeffiencts()
		{
			//Random val between min and max sizes

			//Random val between min and max quality

			//Random val between min and max rarity

			//Random val between min and max points

			//Random val for the piece type we will create.

		}

	}

	public class Equipable
	{
		public String ID { get; set; }
		public String Desc = ""; //{ get; set; }
		public bool bDamage { get; set; }
		public int Weight { get; set; }
		public int Rarity { get; set; }
		public int Creation_Type {get; set;}


		//Foreign Keys
		public int Stats_FK { get; set; }
		public int Weakness_Strength_FK { get; set; }

		/// <summary>
		/// Use this to get the correct point coeffienct data object to calculate
		///	the point values of the created equipable 
		/// </summary>
		public int Point_Coeffiencts_FK { get; set; }

		public BaseStats Stats = new BaseStats();
		public WeakStrength WeaknessAndStrengths = new WeakStrength();
		public Point_Coeffiencts PointCoeffiencts = new Point_Coeffiencts();

		public List<ModifierData> Traits = new List<ModifierData>();
		public List<ModifierData> Effects = new List<ModifierData>();

		// Every single equipable can be used in the crafting system. Which means 
		//EACH single equipable needs a list of possible piece types they can become on init/pickup
		public List<int> PossiblePieceTypes_List = new List<int>();

	}


	public class Created_Equipable
	{
		public int ID { get; set; }
		public String Name { get; set; }
		public String Desc = ""; //{ get; set; }
		public String function_ptr { get; set; } //{ get; set; }
		public int Item_Type { get; set; }
		public int Weight { get; set; }
		public int size { get; set; }
		public int Rarity { get; set; }
		public int Quality { get; set; }
		public int Piece_Type { get; set; }
		public int Total_Points { get; set; }
		//public long PossiblePieceTypes { get; set; }

		//Foreign Keys
		public int Stats_FK { get; set; }
		public int Weakness_Strength_FK { get; set; }
		public String Recipes_Ref_FK { get; set; }

		public Recipe RecipeRef = new Recipe();
		public BaseStats Stats = new BaseStats();
		public WeakStrength WeaknessAndStrengths = new WeakStrength();

		public List<ModifierData> Traits = new List<ModifierData>(2);
		public List<ModifierData> Effects = new List<ModifierData>(2);
		public Equipable ParentRecipe = new Equipable();

	}

}
