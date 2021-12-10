using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Crafting
{

	public enum ECreationTypes
	{
		NONE = 0,
		NATURE = 1,
		METALIC = 2,
		CRYSTAL = 4,
		PLASTICS = 8,

	}

	public class Recipe
	{

		#region fields

		#endregion

		#region property

		public String Name { get; set; }
		public int CreationType { get; set; }
		public String ImagePath { get; set; }
		public int RequiredLevel { get; set; }
		public int QualityThreshold { get; set; }
		public float TimeToMake { get; set; }

		public int BaseStats_FK { get; set; }

		public CraftingHelpers.MagicTypeValues MaxPoints = new CraftingHelpers.MagicTypeValues();
		public CraftingHelpers.PossibleCraftingRewards PossibleRewards = new CraftingHelpers.PossibleCraftingRewards();
		//this is the rewards that are possible when you are creating this recipe
		public List<CraftingHelpers.RecipeIngredient> RequiredIngredients = new List<CraftingHelpers.RecipeIngredient>();



		#endregion


	}

	public class Recipes : Recipe
	{ }

}
