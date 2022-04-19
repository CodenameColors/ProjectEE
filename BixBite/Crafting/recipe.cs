using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Combat;

namespace BixBite.Crafting
{

	public enum ECreationTypes
	{
		NONE = 0,
		NATURE = 1,
		METALIC = 2,
		CRYSTAL = 4,
		PLASTICS = 8,
		Burnable = 16,
		Liquid = 32,

	}

	public class Recipe
	{

		#region fields

		#endregion

		#region property

		public String Name { get; set; }
		public String ImagePath = "";

		public int Creation_Type { get; set; }
		public int Required_Quality { get; set; }
		public int Required_Level { get; set; }
		public float Hours_To_Make { get; set; }

		public int Use_Count { get; set; }
		public int Rating { get; set; }
		public int Quality { get; set; }
		public int Size { get; set; }

		public int Max_Elemental_FK { get; set; }
		public int Weakness_Strength_FK { get; set; }
		public int Stats_FK { get; set; }

		public BaseStats stats = new BaseStats();
		public weaknesses_strengths WeaknessesStrengths = new weaknesses_strengths();

		public CraftingHelpers.Max_Elemental_Points MaxPoints = new CraftingHelpers.Max_Elemental_Points();
		public CraftingHelpers.PossibleCraftingRewards PossibleRewards = new CraftingHelpers.PossibleCraftingRewards();
		//this is the rewards that are possible when you are creating this recipe
		public List<CraftingHelpers.RecipeIngredient> RequiredIngredients = new List<CraftingHelpers.RecipeIngredient>();



		#endregion


	}

	public class Recipes : Recipe
	{ }

	public class Recipe_Rewards
	{

		//public int ID { get; set; }

		public String Req_Recipe { get; set; }
		public String Modifier_ID { get; set; }

		public int Magic_Type { get; set; }
		public int Point_Threshold { get; set; }
	}

	public class RecipeReward : Recipe_Rewards
	{

	}

	public class Recipe_Ingredients
	{
		public String Req_Recipe { get; set; }
		public String Type { get; set; }
		public String Value { get; set; }

	}

	public class Recipe_Ingredient : Recipe_Ingredients
	{

	}



}
