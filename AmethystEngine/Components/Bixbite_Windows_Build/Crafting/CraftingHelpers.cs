using BixBite.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Crafting.HexGrid;

namespace BixBite.Crafting
{
	public class CraftingHelpers
	{

		public class ElementalPoints
		{
			public int ID { get; set; }

			public int Fire { get; set; }
			public int Ice { get; set; }
			public int Earth { get; set; }
			public int Water { get; set; }
			public int Lightning { get; set; }
			public int Explosive { get; set; }
			public int Shadow { get; set; }
			public int Luminous { get; set; }

			public void AddExistingStruct(ref CraftingHelpers.ElementalPoints changingStruct,
				CraftingHelpers.ElementalPoints operandStruct)
			{
				changingStruct.Fire 		  += operandStruct.Fire;	
				changingStruct.Ice 			  += operandStruct.Ice;	
				changingStruct.Earth  		+= operandStruct.Earth;	
				changingStruct.Water  		+= operandStruct.Water;	
				changingStruct.Lightning	+= operandStruct.Lightning;	
				changingStruct.Explosive	+= operandStruct.Explosive;	
				changingStruct.Shadow 		+= operandStruct.Shadow;	
				changingStruct.Luminous		+= operandStruct.Luminous;	
			}

			public void RemoveExistingStruct(ref CraftingHelpers.ElementalPoints changingStruct,
				CraftingHelpers.ElementalPoints operandStruct)
			{
				changingStruct.Fire 		  -= operandStruct.Fire;	
				changingStruct.Ice 			  -= operandStruct.Ice;	
				changingStruct.Earth  		-= operandStruct.Earth;	
				changingStruct.Water  		-= operandStruct.Water;	
				changingStruct.Lightning	-= operandStruct.Lightning;	
				changingStruct.Explosive	-= operandStruct.Explosive;	
				changingStruct.Shadow 		-= operandStruct.Shadow;	
				changingStruct.Luminous		-= operandStruct.Luminous;	
			}


			public int GetValueFromMagicType(EMagicType desiredMagicType)
			{
				switch (desiredMagicType)
				{
					case EMagicType.NONE:
						break;
					case EMagicType.Fire:
						return Fire;
					case EMagicType.Ice:
						return Ice;
					case EMagicType.Earth:
						return Earth;
					case EMagicType.Water:
						return Water;
					case EMagicType.Lightning:
						return Lightning;
					case EMagicType.Explosive:
						return Explosive;
					case EMagicType.Shadow:
						return Shadow;
					case EMagicType.Luminous:
						return Luminous;
				}
				return 0;
			}

			public void AddElementalValue(EMagicType desiredMagicType, int val = 1)
			{
				switch (desiredMagicType)
				{
					case EMagicType.NONE:
						break;
					case EMagicType.Fire:
						Fire += val;
						break;
					case EMagicType.Ice:
						Ice += val;
						break;
					case EMagicType.Earth:
						Earth += val;
						break;
					case EMagicType.Water:
						Water += val;
						break;
					case EMagicType.Lightning:
						Lightning += val;
						break;
					case EMagicType.Explosive:
						Explosive += val;
						break;
					case EMagicType.Shadow:
						Shadow += val;
						break;
					case EMagicType.Luminous:
						Luminous += val;
						break;
				}
			}

			public int GetConnectionSum()
			{
				return Fire + Ice + Earth + Water + Lightning + Explosive + Shadow + Luminous;
			}


			public String ToString()
			{
				return String.Format("internal Cell Count Structure values \n Fire: {0}\nIce: {1}\nEarth: {2}\nWater: {3}\nLightning: {4}" +
														 "\nExplosive: {5}\nShadow: {6} Luminous: {7}", Fire, Ice, Earth, Water, Lightning, Explosive, Shadow, Luminous);
			}


		}

		/// <summary>
		/// This is here mainly for the database binding stuff
		/// </summary>
		public class Max_Elemental_Points : ElementalPoints
		{


		}

		public class RecipeIngredient : Tuple<object, int, String>
		{

			public RecipeIngredient(object requiredIngredient, int numberOf, String type)
					: base(requiredIngredient, numberOf, type)
			{

			}

			public object Ingredient { get { return this.Item1; } }
			public int NumberOfIngredient { get { return this.Item2; } }
			public String IngredientType { get { return this.Item3; } }

		}

		public class PossibleCraftingRewards : List<Reward_Requirement>
		{
			public PossibleCraftingRewards() : base()
			{

			}

			/// <summary>
			/// get a list of all the rewards for a desired magic type
			/// </summary>
			/// <param name="desiredMagicType"></param>
			/// <returns>returns the list of rewards for a given magic type. NULL if none found</returns>
			public List<Reward_Requirement> GetListOfMagicTypeRewards(EMagicType desiredMagicType)
			{
				List<Reward_Requirement> retList = new List<Reward_Requirement>();
				retList = base.FindAll(x => desiredMagicType == x.EMagicType);
				if (retList.Count == 0)
					return new List<Reward_Requirement>(0);
				else
					return retList;
			}

		}

		public class Reward_Requirement : Tuple<EMagicType, int, GameplayModifier>
		{
			public Reward_Requirement(EMagicType eMagicType, int pointValue, GameplayModifier modifier)
					: base(eMagicType, pointValue, modifier)
			{

			}

			public EMagicType EMagicType { get { return this.Item1; } }
			public int PointThreshold { get { return this.Item2; } }
			public GameplayModifier ModifierID { get { return this.Item3; } }

		}


		public class RotationQueuePair<T1, T2>
		{
			public PuzzlePieceHexCell PuzzlePieceHexCell { get; set; }
			//public PuzzlePieceHexCell ParentConnectionCell { get; set; }
			public int ConnDirection{ get; set; }

		}

	}

}


	

