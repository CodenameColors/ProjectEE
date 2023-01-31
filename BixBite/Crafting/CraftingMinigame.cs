#define Crafting_EE_Debug

using System;
using System.Collections.Generic;
using System.Linq;
using BixBite.Combat;
using BixBite.Crafting.HexGrid;
using BixBite.Rendering.UI.Image;
using BixBite.Rendering.UI.ListBox;
using BixBite.Rendering.UI.ProgressBar;
using BixBite.Rendering.UI.TextBlock;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Crafting
{
	/// <summary>
	/// This class is a singleton class as there can only be ONE crafting minigame instance at one time.
	/// This class holds all the logic, and rendering steps for the crafting minigame.
	/// <para></para>
	/// IT IS IMPORTANT TO NOTE!!! that this class is the actual "crafting board section" not the recipe book.
	/// This is one of the final steps that go into crafting.
	/// </summary>
	public class CraftingMinigame
	{

		///// <summary>
		///// These are the 'white connections' in the picture
		///  Keeps track of the puzzle piece to puzzle piece connections (per Cell) that DO have matching types
		///// </summary>
		public struct MatchingConections_S
		{
			public int Fire { get; set; }
			public int Ice { get; set; }
			public int Earth { get; set; }
			public int Water { get; set; }
			public int Lightning { get; set; }
			public int Explosive { get; set; }
			public int Shadow { get; set; }
			public int Luminous { get; set; }

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

			public void AddElementalConnection(EMagicType desiredMagicType, int val = 1)
			{
				switch (desiredMagicType)
				{
					case EMagicType.NONE:
						break;
					case EMagicType.Fire:
						Fire+= val;
						break;
					case EMagicType.Ice:
						Ice+= val;
						break;
					case EMagicType.Earth:
						Earth+= val;
						break;
					case EMagicType.Water:
						Water+= val;
						break;
					case EMagicType.Lightning:
						Lightning+= val;
						break;
					case EMagicType.Explosive:
						Explosive+= val;
						break;
					case EMagicType.Shadow:
						Shadow+= val;
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
				return String.Format("SAME External Cell Connection Count Structure values \n Fire: {0}\nIce: {1}\nEarth: {2}\nWater: {3}\nLightning: {4}" +
					"\nExplosive: {5}\nShadow: {6} Luminous: {7}", Fire, Ice, Earth, Water, Lightning, Explosive, Shadow, Luminous);
			}

		}

		///// <summary>
		///// These are the 'Red connections' in the picture
		/// Keeps track of puzzle piece to puzzle piece connections (PER CELL) that do NOT have matching types
		///// </summary>
		public struct NonMatchingConnections_S
		{
			public int Fire { get; set; }
			public int Ice { get; set; }
			public int Earth { get; set; }
			public int Water { get; set; }
			public int Lightning { get; set; }
			public int Explosive { get; set; }
			public int Shadow { get; set; }
			public int Luminous { get; set; }

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

			public void AddElementalConnection(EMagicType desiredMagicType,int val = 1)
			{
				switch (desiredMagicType)
				{
					case EMagicType.NONE:
						break;
					case EMagicType.Fire:
						Fire+= val;
						break;
					case EMagicType.Ice:
						Ice+= val;
						break;
					case EMagicType.Earth:
						Earth+= val;
						break;
					case EMagicType.Water:
						Water+= val;
						break;
					case EMagicType.Lightning:
						Lightning+= val;
						break;
					case EMagicType.Explosive:
						Explosive+= val;
						break;
					case EMagicType.Shadow:
						Shadow+= val;
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
				return String.Format("Different External Cell Connection Count Structure values \n Fire: {0}\nIce: {1}\nEarth: {2}\nWater: {3}\nLightning: {4}" +
					"\nExplosive: {5}\nShadow: {6} Luminous: {7}", Fire, Ice, Earth, Water, Lightning, Explosive, Shadow, Luminous);
			}
		}

		private List<PuzzlePiece> gridPuzzlePieces = new List<PuzzlePiece>();

		//public MagicCellsPlacedCount_S MagicCellsPlacedCount_Struct = new MagicCellsPlacedCount_S();
		//public MatchingInternalCellCount_S MatchingInternalCellCount_Struct = new MatchingInternalCellCount_S();
		//public MatchingExternalCellCount_S MatchingExternalCellCount_Struct = new MatchingExternalCellCount_S();
		//public NonMatchingConnectionTypesCount_S NonMatchingConnectionTypesCount_Struct = new NonMatchingConnectionTypesCount_S();

		#region Delegates/Events

		#endregion

		#region Fields
		//This is a singleton
		private static Lazy<CraftingMinigame> _craftingMinigame = new Lazy<CraftingMinigame>(() => new CraftingMinigame());

		//Lookup table for the corrupted multiplier
		private List<Tuple<int, float>> _corruptedStability_LookUpTable = new List<Tuple<int, float>>();

		//ContentRef
		private ContentManager _Contentref = null;

		private SpriteFont _spriteFont;

		//UI
		private GameImage _backgroundGrid_GameImage = null;
		private GameImage _pcGirl_GameImage = null;
		private GameProgressBar _Stability_ProgressBar = null;
		//End of Textures

		//HexGrid
		

		//primitives
		private int _resolutionWidth = -1;
		private int _resolutionHeight = -1;

		private bool _bIsActive;

		//Elemental Effects section

		#region Elemental Effects

		private int _elementalEffectFireMaxPoints = 100;
		private int _elementalEffectIceMaxPoints = 100;
		private int _elementalEffectEarthMaxPoints = 100;
		private int _elementalEffectWaterMaxPoints = 100;
		private int _elementalEffectLightningMaxPoints = 100;
		private int _elementalEffectExplosiveMaxPoints = 100;
		private int _elementalEffectShadowMaxPoints = 100;
		private int _elementalEffectLuminousMaxPoints = 100;

		private int _elementalEffectFirePoints = 0;
		private int _elementalEffectIcePoints = 0;
		private int _elementalEffectEarthPoints = 0;
		private int _elementalEffectWaterPoints = 0;
		private int _elementalEffectLightningPoints = 0;
		private int _elementalEffectExplosivePoints = 0;
		private int _elementalEffectShadowPoints = 0;
		private int _elementalEffectLuminousPoints = 0;


		private int _stabilityMaxPercent = 0;
		private int _stabilityCurrentPercent = 0;

		//Labels references

		private GameTextBlock[] ElementalEffectsHeaders_GTB_List = new GameTextBlock[8];

		private GameTextBlock FireEffectsHeader_GTB
		{
			get => ElementalEffectsHeaders_GTB_List[0];
			set => ElementalEffectsHeaders_GTB_List[0] = value;
		}
		private GameTextBlock IceEffectsHeader_GTB
		{
			get => ElementalEffectsHeaders_GTB_List[1];
			set => ElementalEffectsHeaders_GTB_List[1] = value;
		}
		private GameTextBlock EarthEffectsHeader_GTB
		{
			get => ElementalEffectsHeaders_GTB_List[2];
			set => ElementalEffectsHeaders_GTB_List[2] = value;
		}
		private GameTextBlock WaterEffectsHeader_GTB
		{
			get => ElementalEffectsHeaders_GTB_List[3];
			set => ElementalEffectsHeaders_GTB_List[3] = value;
		}
		private GameTextBlock LightningEffectsHeader_GTB
		{
			get => ElementalEffectsHeaders_GTB_List[4];
			set => ElementalEffectsHeaders_GTB_List[4] = value;
		}
		private GameTextBlock ExplosiveEffectsHeader_GTB
		{
			get => ElementalEffectsHeaders_GTB_List[5];
			set => ElementalEffectsHeaders_GTB_List[5] = value;
		}
		private GameTextBlock ShadowEffectsHeader_GTB
		{
			get => ElementalEffectsHeaders_GTB_List[6];
			set => ElementalEffectsHeaders_GTB_List[6] = value;
		}
		private GameTextBlock LuminousEffectsHeader_GTB
		{
			get => ElementalEffectsHeaders_GTB_List[7];
			set => ElementalEffectsHeaders_GTB_List[7] = value;
		}


		private GameTextBlock[] ElementalEffectsMinPoints_GTB_List = new GameTextBlock[8];

		private GameTextBlock FireEffectsMinPoints_GTB
		{
			get => ElementalEffectsMinPoints_GTB_List[0];
			set => ElementalEffectsMinPoints_GTB_List[0] = value;
		}
		private GameTextBlock IceEffectsMinPoints_GTB
		{
			get => ElementalEffectsMinPoints_GTB_List[1];
			set => ElementalEffectsMinPoints_GTB_List[1] = value;
		}
		private GameTextBlock EarthEffectsMinPoints_GTB
		{
			get => ElementalEffectsMinPoints_GTB_List[2];
			set => ElementalEffectsMinPoints_GTB_List[2] = value;
		}
		private GameTextBlock WaterEffectsMinPoints_GTB
		{
			get => ElementalEffectsMinPoints_GTB_List[3];
			set => ElementalEffectsMinPoints_GTB_List[3] = value;
		}
		private GameTextBlock LightningEffectsMinPoints_GTB
		{
			get => ElementalEffectsMinPoints_GTB_List[4];
			set => ElementalEffectsMinPoints_GTB_List[4] = value;
		}
		private GameTextBlock ExplosiveEffectsMinPoints_GTB
		{
			get => ElementalEffectsMinPoints_GTB_List[5];
			set => ElementalEffectsMinPoints_GTB_List[5] = value;
		}
		private GameTextBlock ShadowEffectsMinPoints_GTB
		{
			get => ElementalEffectsMinPoints_GTB_List[6];
			set => ElementalEffectsMinPoints_GTB_List[6] = value;
		}
		private GameTextBlock LuminousEffectsMinPoints_GTB
		{
			get => ElementalEffectsMinPoints_GTB_List[7];
			set => ElementalEffectsMinPoints_GTB_List[7] = value;
		}


		private GameTextBlock[] ElementalEffectsMaxPoints_GTB_List = new GameTextBlock[8];

		private GameTextBlock FireEffectsMaxPoints_GTB
		{
			get => ElementalEffectsMaxPoints_GTB_List[0];
			set => ElementalEffectsMaxPoints_GTB_List[0] = value;
		}
		private GameTextBlock IceEffectsMaxPoints_GTB
		{
			get => ElementalEffectsMaxPoints_GTB_List[1];
			set => ElementalEffectsMaxPoints_GTB_List[1] = value;
		}
		private GameTextBlock EarthEffectsMaxPoints_GTB
		{
			get => ElementalEffectsMaxPoints_GTB_List[2];
			set => ElementalEffectsMaxPoints_GTB_List[2] = value;
		}
		private GameTextBlock WaterEffectsMaxPoints_GTB
		{
			get => ElementalEffectsMaxPoints_GTB_List[3];
			set => ElementalEffectsMaxPoints_GTB_List[3] = value;
		}
		private GameTextBlock LightningEffectsMaxPoints_GTB
		{
			get => ElementalEffectsMaxPoints_GTB_List[4];
			set => ElementalEffectsMaxPoints_GTB_List[4] = value;
		}
		private GameTextBlock ExplosiveEffectsMaxPoints_GTB
		{
			get => ElementalEffectsMaxPoints_GTB_List[5];
			set => ElementalEffectsMaxPoints_GTB_List[5] = value;
		}
		private GameTextBlock ShadowEffectsMaxPoints_GTB
		{
			get => ElementalEffectsMaxPoints_GTB_List[6];
			set => ElementalEffectsMaxPoints_GTB_List[6] = value;
		}
		private GameTextBlock LuminousEffectsMaxPoints_GTB
		{
			get => ElementalEffectsMaxPoints_GTB_List[7];
			set => ElementalEffectsMaxPoints_GTB_List[7] = value;
		}


		//Progress Bars references
		private GameProgressBar[] ElementalEffectsPointBar_GPB_List = new GameProgressBar[8];

		private GameProgressBar FireEffectsPointBar_GPB
		{
			get => ElementalEffectsPointBar_GPB_List[0];
			set => ElementalEffectsPointBar_GPB_List[0] = value;
		}
		private GameProgressBar IceEffectsPointBar_GPB
		{
			get => ElementalEffectsPointBar_GPB_List[1];
			set => ElementalEffectsPointBar_GPB_List[1] = value;
		}
		private GameProgressBar EarthEffectsPointBar_GPB
		{
			get => ElementalEffectsPointBar_GPB_List[2];
			set => ElementalEffectsPointBar_GPB_List[2] = value;
		}
		private GameProgressBar WaterEffectsPointBar_GPB
		{
			get => ElementalEffectsPointBar_GPB_List[3];
			set => ElementalEffectsPointBar_GPB_List[3] = value;
		}
		private GameProgressBar LightningEffectsPointBar_GPB
		{
			get => ElementalEffectsPointBar_GPB_List[4];
			set => ElementalEffectsPointBar_GPB_List[4] = value;
		}
		private GameProgressBar ExplosiveEffectsPointBar_GPB
		{
			get => ElementalEffectsPointBar_GPB_List[5];
			set => ElementalEffectsPointBar_GPB_List[5] = value;
		}
		private GameProgressBar ShadowEffectsPointBar_GPB
		{
			get => ElementalEffectsPointBar_GPB_List[6];
			set => ElementalEffectsPointBar_GPB_List[6] = value;
		}
		private GameProgressBar LuminousEffectsPointBar_GPB
		{
			get => ElementalEffectsPointBar_GPB_List[7];
			set => ElementalEffectsPointBar_GPB_List[7] = value;
		}


		//ListBoxes references
		private GameListBox[] ElementalEffectUnlocks_GLB_List = new GameListBox[8];

		private GameListBox FireEffectUnlocked_GLB
		{
			get => ElementalEffectUnlocks_GLB_List[0];
			set => ElementalEffectUnlocks_GLB_List[0] = value;
		}
		private GameListBox IceEffectUnlocked_GLB
		{
			get => ElementalEffectUnlocks_GLB_List[1];
			set => ElementalEffectUnlocks_GLB_List[1] = value;
		}
		private GameListBox EarthEffectUnlocked_GLB
		{
			get => ElementalEffectUnlocks_GLB_List[2];
			set => ElementalEffectUnlocks_GLB_List[2] = value;
		}
		private GameListBox WaterEffectUnlocked_GLB
		{
			get => ElementalEffectUnlocks_GLB_List[3];
			set => ElementalEffectUnlocks_GLB_List[3] = value;
		}
		private GameListBox LightningEffectUnlocked_GLB
		{
			get => ElementalEffectUnlocks_GLB_List[4];
			set => ElementalEffectUnlocks_GLB_List[4] = value;
		}
		private GameListBox ExplosiveEffectUnlocked_GLB
		{
			get => ElementalEffectUnlocks_GLB_List[5];
			set => ElementalEffectUnlocks_GLB_List[5] = value;
		}
		private GameListBox ShadowEffectUnlocked_GLB
		{
			get => ElementalEffectUnlocks_GLB_List[6];
			set => ElementalEffectUnlocks_GLB_List[6] = value;
		}
		private GameListBox LuminousEffectUnlocked_GLB
		{
			get => ElementalEffectUnlocks_GLB_List[7];
			set => ElementalEffectUnlocks_GLB_List[7] = value;
		}


		#endregion
		//END oF Elemental Effects section


#if Crafting_EE_Debug
		//Debug Textblock
		private GameTextBlock _debugGameTextBlock = null;

		private String _debugTextblock_Textbind = "DEBUG LOGGER: ";

		public String DebugTextblock_TextBind
		{
			get => _debugTextblock_Textbind;
			set => _debugTextblock_Textbind = "DEBUG LOGGER: " + value;
		}
#endif
		#endregion
		
		#region Properties

		public int waitforpieceframes = 0;

		//No resetting the Instance. But you can use it and its members/methods.
		public static CraftingMinigame Instance => _craftingMinigame.Value;

		public HexGrid2D PuzzleBoardGrid2D = null;

		//corruptedStability_LookUpTable
		public List<Tuple<int, float>> CorruptedStability_LookUpTable
		{
			get => _corruptedStability_LookUpTable;
			set
			{
				_corruptedStability_LookUpTable = value;
			}
		}

		//PuzzlePiece
		public PuzzlePiece TestingPiece = null;
		public List<PuzzlePiece> TestingPiecesList = new List<PuzzlePiece>();
		

		//Currently Selected piece
		//public PuzzlePiece CurrentSelectedPuzzlePiece = null;
		/// <summary>
		/// Key = Cloned, Value = Original Piece
		/// </summary>
		public KeyValuePair<PuzzlePiece, PuzzlePiece> CurrentSelectedPuzzlePiecePair = new KeyValuePair<PuzzlePiece, PuzzlePiece>();

		//Textures
		public GameImage BackgroundImage
		{
			get => _backgroundGrid_GameImage;
			set => _backgroundGrid_GameImage = value;
		}
		//End of Textures

		//primitives
		public bool BIsActive
		{
			get => _bIsActive;
			set => _bIsActive = value;
		}
		
		#region Elemental Effects


		public int ElementalEffectFireMaxPoints
		{
			get => _elementalEffectFireMaxPoints;
			set
			{
				_elementalEffectFireMaxPoints = value;
				FireEffectsMaxPoints_GTB.Text = value.ToString();
				FireEffectsPointBar_GPB.MaxVal = value;
			}
		}
		public int ElementalEffectIceMaxPoints
		{
			get => _elementalEffectIceMaxPoints;
			set
			{
				_elementalEffectIceMaxPoints = value;
				IceEffectsMaxPoints_GTB.Text = value.ToString();
				IceEffectsPointBar_GPB.MaxVal = value;
			}
		}
		public int ElementalEffectEarthMaxPoints
		{
			get => _elementalEffectEarthMaxPoints;
			set
			{
				_elementalEffectEarthMaxPoints = value;
				EarthEffectsMaxPoints_GTB.Text = value.ToString();
				EarthEffectsPointBar_GPB.MaxVal = value;
			}
		}
		public int ElementalEffectWaterMaxPoints
		{
			get => _elementalEffectEarthMaxPoints;
			set
			{
				_elementalEffectEarthMaxPoints = value;
				EarthEffectsMaxPoints_GTB.Text = value.ToString();
				WaterEffectsPointBar_GPB.MaxVal = value;
			}
		}
		public int ElementalEffectLightningMaxPoints
		{
			get => _elementalEffectLightningMaxPoints;
			set
			{
				_elementalEffectLightningMaxPoints = value;
				LightningEffectsMaxPoints_GTB.Text = value.ToString();
				LightningEffectsPointBar_GPB.MaxVal = value;
			}
		}
		public int ElementalEffectExplosiveMaxPoints
		{
			get => _elementalEffectExplosiveMaxPoints;
			set
			{
				_elementalEffectExplosiveMaxPoints = value;
				ExplosiveEffectsMaxPoints_GTB.Text = value.ToString();
				ExplosiveEffectsPointBar_GPB.MaxVal = value;
			}
		}
		public int ElementalEffectShadowMaxPoints
		{
			get => _elementalEffectShadowMaxPoints;
			set
			{
				_elementalEffectShadowMaxPoints = value;
				ShadowEffectsMaxPoints_GTB.Text = value.ToString();
				ShadowEffectsPointBar_GPB.MaxVal = value;
			}
		}
		public int ElementalEffectLuminousMaxPoints
		{
			get => _elementalEffectLuminousMaxPoints;
			set
			{
				_elementalEffectLuminousMaxPoints = value;
				LuminousEffectsMaxPoints_GTB.Text = value.ToString();
				LuminousEffectsPointBar_GPB.MaxVal = value;
			}
		}


		public int ElementalEffectFirePoints
		{
			get => _elementalEffectFirePoints;
			set
			{
				_elementalEffectFirePoints = value;
				FireEffectsPointBar_GPB.CurrentVal = value;
			}
		}
		public int ElementalEffectIcePoints
		{
			get => _elementalEffectIcePoints;
			set
			{
				_elementalEffectIcePoints = value;
				IceEffectsPointBar_GPB.CurrentVal = value;
			}
		}

		public int ElementalEffectEarthPoints
		{
			get => _elementalEffectEarthPoints;
			set
			{
				_elementalEffectEarthPoints = value;
				EarthEffectsPointBar_GPB.CurrentVal = value;
			}
		}

		public int ElementalEffectWaterPoints
		{
			get => _elementalEffectWaterPoints;
			set
			{
				_elementalEffectWaterPoints = value;
				WaterEffectsPointBar_GPB.CurrentVal = value;
			}
		}
		public int ElementalEffectLightningPoints
		{
			get => _elementalEffectLightningPoints;
			set
			{
				_elementalEffectLightningPoints = value;
				LightningEffectsPointBar_GPB.CurrentVal = value;
			}
		}
		public int ElementalEffectExplosivePoints
		{
			get => _elementalEffectExplosivePoints;
			set
			{
				_elementalEffectExplosivePoints = value;
				ExplosiveEffectsPointBar_GPB.CurrentVal = value;
			}
		}

		public int ElementalEffectShadowPoints
		{
			get => _elementalEffectShadowPoints;
			set
			{
				_elementalEffectShadowPoints = value;
				ShadowEffectsPointBar_GPB.CurrentVal = value;
			}
		}
		public int ElementalEffectLuminousPoints
		{
			get => _elementalEffectLuminousPoints;
			set
			{
				_elementalEffectLuminousPoints = value;
				LuminousEffectsPointBar_GPB.CurrentVal = value;
			}
		}

		public int StabilityMaxPercent
		{
			get => _stabilityMaxPercent;
			set
			{
				_stabilityMaxPercent = value;
				_Stability_ProgressBar.MaxVal = value;
			}
		}

		public int StabilityCurrentPercent
		{
			get => _stabilityCurrentPercent;
			set
			{
				_stabilityCurrentPercent = value;
				_Stability_ProgressBar.CurrentVal = value;
			}
		}

		#endregion
		
		#endregion

		#region Constructors

		#endregion

		#region Methods

		#region PuzzlePieceCreation

		/// <summary>
		/// This method is here to create the puzzle piece in memory. From a Connection logic side
		/// As well as a display logic side.
		/// </summary>
		/// <param name="desiredPiece">The Desired Puzzle Piece Shape</param>
		private void PuzzlePieceCreation(int desiredPiece)
		{
			if (desiredPiece == 1)
			{
			}
		}

		#endregion

		#region Setters

		#endregion

		#region Helpers

		private void RotatePuzzlePiece(PuzzlePieceHexCell AnchorCell, LinkedList<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>> RotationLayerLL, bool bIsClockwise, bool bOverwriteFlag = false)
		{
			PuzzlePiece parentPiece = AnchorCell.GetPuzzlePieceParent();

			parentPiece.RotatePiece();

			if (bIsClockwise)
			{
				if (parentPiece.RotationCount == 5) parentPiece.RotationCount = 0;
				else parentPiece.RotationCount++;
			}
			else
			{
				if (parentPiece.RotationCount == 0) parentPiece.RotationCount = 5;
				else parentPiece.RotationCount--;
			}
		}

		private bool ContainsHexPieceInQueue(LinkedList<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>> linkedList, PuzzlePieceHexCell desiredHexCell)
		{
			var v = linkedList.First;
			while (v == null)
			{
				if (desiredHexCell == v.Value.PuzzlePieceHexCell)
					return true;
				v = v.Next;
			}
			return false;
		}

		/// <summary>
		/// This method will traverse through through the given puzzle piece and check to see if ALL the grids are free for placement.
		/// If so then we return true, else false.
		/// </summary>
		/// <returns></returns>
		private LinkedList<Tuple<PuzzlePieceHexCell, HexCell>> CanPlacePuzzlePiece(PuzzlePiece selectedPuzzlePiece, HexCell gridAnchorCell, bool isClockwise = true)
		{
			Console.WriteLine("*********************Starting Puzzle Piece Placement check!**************************");
			//bool retb = true;
			
			//We need to keep track of the Grid's hex cell and the PuzzlePieces Internal Cells as wel traverse the graph.
			LinkedList<Tuple<PuzzlePieceHexCell, HexCell>> currentNodesLinkedList = new LinkedList<Tuple<PuzzlePieceHexCell, HexCell>>();
			currentNodesLinkedList.AddFirst( new Tuple<PuzzlePieceHexCell, HexCell>(selectedPuzzlePiece.SelectedHexCell, gridAnchorCell));
			LinkedListNode<Tuple<PuzzlePieceHexCell, HexCell>> currentListNode = currentNodesLinkedList.First;

			// Before we scan our whole puzzle piece let's check the Anchor point isn't trying to overlap anything.
			CraftingMinigame.DebugOutToConsole("Checking desired Anchor point on grid for overlap");
			if (gridAnchorCell.LinkedDataCell != null)
			{
				CraftingMinigame.DebugOutToConsole("Desired Anchor point is already taken, we cannot place this piece.");
				return null;
			}
			else CraftingMinigame.DebugOutToConsole("Desired Anchor point isn't taken. We can use this spot");

			//Scan the linked list until the end is reached.
			do
			{
				//Scan the connections of the current node.
				for (int dir = 0; dir < 6; dir++)
				{
					//We need to account of rotation.
					int desDir = -1;
					if (isClockwise)
						desDir = (dir + selectedPuzzlePiece.RotationCount) % 6;

					if (currentListNode.Value.Item1.GetConnectionCell(dir) != null)
					{
						//Make sure you don't add a node that is already in the list.
						if (Array.FindIndex(currentNodesLinkedList.ToArray(), x => x.Item1 == currentListNode.Value.Item1.GetConnectionCell(dir)) != -1)
							continue;

						////TODO: This will ERROR out when not on the grid. it's not super bad, and it does make sense.
						Console.WriteLine(String.Format("Cell found [{1}] From Parent Cell [{0}] in desDirection [{2}] @ Grid Pos [{3}, {4}]",
							currentListNode.Value.Item1.GetGameTextbox().Text, ((PuzzlePieceHexCell)currentListNode.Value.Item1.GetConnectionCell(dir)).GetGameTextbox().Text,
							HexCell.DirectionToString(desDir), currentListNode.Value.Item2.GetConnectionCell(desDir)?.Row, currentListNode.Value.Item2.GetConnectionCell(desDir)?.Column));

						//The cell isn't part of the current list. So let's check the grid to see if we can continue.
						if (currentListNode.Value.Item2.GetConnectionCell(desDir).LinkedDataCell != null)
						{
							Console.WriteLine(String.Format("Return false there occupied cell found at [{0},{1}]", 
								currentListNode.Value.Item2.GetConnectionCell(desDir).Row, currentListNode.Value.Item2.GetConnectionCell(desDir).Column));
							return null; //There is already a cell here, so we need to stop processing
						}
						else
						{
							//There isn't anything occupying the grid's cell so we can continue.
							currentNodesLinkedList.AddLast(new Tuple<PuzzlePieceHexCell, HexCell>(
								(PuzzlePieceHexCell)currentListNode.Value.Item1.GetConnectionCell(dir), currentListNode.Value.Item2.GetConnectionCell(desDir)));

							
						}
					}
				}
				//currentNodesLinkedList.RemoveFirst();

				currentListNode = currentListNode.Next; //move through the list.
			} while (currentListNode != null);


			Console.WriteLine("*********************Ending Puzzle Piece Placement check!**************************");
			return currentNodesLinkedList;
		}

		public static Color GetMagicColor(EMagicType desiredMagicType)
		{
			switch (desiredMagicType)
			{
				case EMagicType.NONE:
					return Color.Transparent;
				case EMagicType.Fire:
					return Color.DarkRed;
				case EMagicType.Ice:
					return Color.LightBlue;
				case EMagicType.Earth:
					return Color.DarkGreen;
				case EMagicType.Water:
					return Color.DarkBlue;
				case EMagicType.Lightning:
					return Color.Goldenrod;
				case EMagicType.Explosive:
					return Color.Orange;
				case EMagicType.Shadow:
					return Color.Black;
				case EMagicType.Luminous:
					return Color.LightGray;
			}
#if Crafting_EE_Debug
			System.Diagnostics.Debug.WriteLine(
				String.Format("Attempted to get a Color from a Magic Type but failed. Given Val was  [{0}]", desiredMagicType));
#endif
			return Color.Transparent;
		}


		/// <summary>
		/// Handles ALL the input from the player when the player has a selected piece.
		/// Allows movement, grid snapping, and resetting, On hover, ghosting effects. and click events.
		/// </summary>
		/// <param name="prevMouseState"></param>
		private void UpdateHandler_SelectedPiece(MouseState prevMouseState, KeyboardState prevKeyboardState)
		{
			MouseState mouseState = Mouse.GetState();
			KeyboardState keyboardState = Keyboard.GetState();

			//Do we have a selected Piece?
			if (this.CurrentSelectedPuzzlePiecePair.Key != null)
			{

				CurrentSelectedPuzzlePiecePair.Key.XPos = Mouse.GetState().X;
				CurrentSelectedPuzzlePiecePair.Key.YPos = Mouse.GetState().Y;

				if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
				{
					CraftingMinigame.DebugOutToConsole("Left Click on Piece not on grid");

					//Can we place the puzzle piece though?
					LinkedList<Tuple<PuzzlePieceHexCell, HexCell>> temp = null;
					try
					{
						this.CurrentSelectedPuzzlePiecePair.Value.RotationCount = this.CurrentSelectedPuzzlePiecePair.Key.RotationCount;
						temp = CanPlacePuzzlePiece(this.CurrentSelectedPuzzlePiecePair.Key, this.PuzzleBoardGrid2D.SelectedCell, true);

						this.CurrentSelectedPuzzlePiecePair.Value.RotationCount = 0; // Reset it. because this is the original piece not meant for grid rotation.

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
#if Crafting_EE_Debug
						DebugTextblock_TextBind = "Tried To place Piece, BUT it off the grid...";
#endif
						//return false;
					}

					if (temp != null)
					{

						//Snap to grid
						HexCell hoveredGridCell =
							PuzzleBoardGrid2D.DataCells.Values.FirstOrDefault(x => x.IsInside(Mouse.GetState().Position.ToVector2()));

						Vector2 GridSnapPos =new Vector2(hoveredGridCell.XPos + hoveredGridCell.Width / 2, hoveredGridCell.YPos + hoveredGridCell.Height / 2);

						Console.WriteLine("We can place this puzzle piece down, so let's place it on the grid");
						PuzzleBoardGrid2D.AddPuzzlePieceOnGrid(CurrentSelectedPuzzlePiecePair, temp, GridSnapPos);

						//Make the Original piece "dead"
						//this.CurrentSelectedPuzzlePiecePair.Key.bIsActive = true;
						//this.CurrentSelectedPuzzlePiecePair.Key.bIsSelectable = true;
						//this.CurrentSelectedPuzzlePiecePair.Key.bIsOnGrid = true;
						//this.CurrentSelectedPuzzlePiecePair.Key.bIsSelected = false;

						PuzzleBoardGrid2D.SelectedCell = null; //Stop double eventing

						CurrentSelectedPuzzlePiecePair = new KeyValuePair<PuzzlePiece, PuzzlePiece>(); //clear
						prevMouseState = mouseState;
						prevKeyboardState = keyboardState;

						//return;
					}

				}
				//Reset if the player has a selected piece currently
				else if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
				{
					CraftingMinigame.DebugOutToConsole("Right Click on Piece not on grid");

					this.CurrentSelectedPuzzlePiecePair.Value.SetTransparency(1.0f);

					CurrentSelectedPuzzlePiecePair.Value.bIsActive = true; //just in case we hid it from view.
					CurrentSelectedPuzzlePiecePair.Value.bIsOnGrid = false; //small flag to avoid duplicate confusion.
					CurrentSelectedPuzzlePiecePair.Value.bIsSelected = false;
					CurrentSelectedPuzzlePiecePair.Value.bIsSelectable = true;
					//CurrentSelectedPuzzlePiecePair.Value.SetTransparency(1f);
					//CurrentSelectedPuzzlePiecePair.Value.ResetRescalePiece(.25f, .25f);
					//CurrentSelectedPuzzlePiecePair.Value.ResetToSpawnPosition();


					this.CurrentSelectedPuzzlePiecePair = new KeyValuePair<PuzzlePiece, PuzzlePiece>();
					

				}
				else
				{
					if (OnRotateRequest(keyboardState, prevKeyboardState, CurrentSelectedPuzzlePiecePair.Key))
					{

					}
				}
			}

			//Snap to grid
			HexCell hexcell =
				PuzzleBoardGrid2D.DataCells.Values.FirstOrDefault(x => x.IsInside(Mouse.GetState().Position.ToVector2()));
			if (hexcell != null)
			{
				if (this.CurrentSelectedPuzzlePiecePair.Key != null)
				{
					this.CurrentSelectedPuzzlePiecePair.Key.XPos = hexcell.XPos + hexcell.Width / 2;
					this.CurrentSelectedPuzzlePiecePair.Key.YPos = hexcell.YPos + hexcell.Height / 2;
				}

				this.PuzzleBoardGrid2D.SelectedCell = hexcell;

			}
			else
			{
				this.PuzzleBoardGrid2D.SelectedCell = hexcell;
			}
#if Crafting_EE_Debug
			//FOR DEBUGGING. Currently reads out HexGrid cell linked data to Console.
			OnDebugRequest(mouseState, prevMouseState);
#endif

			

			prevMouseState = mouseState;
			prevKeyboardState = keyboardState;

			//			PuzzlePiece tempPiece = null; //= CurrentSelectedPuzzlePiece;
			//			tempPiece = CurrentSelectedPuzzlePiece;

			//			if (CurrentlyClonedPuzzlePiece != null && waitforpieceframes <= 0)
			//			{
			//				//Snap to grid
			//				HexCell hexcell =
			//					PuzzleBoardGrid2D.DataCells.Values.FirstOrDefault(x => x.IsInside(Mouse.GetState().Position.ToVector2()));
			//				if (hexcell != null)
			//				{
			//					CurrentlyClonedPuzzlePiece.XPos = hexcell.XPos + hexcell.Width / 2;
			//					CurrentlyClonedPuzzlePiece.YPos = hexcell.YPos + hexcell.Height / 2;

			//					//input state checks
			//					if(OnRotateRequest(keyboardState, prevKeyboardState, CurrentlyClonedPuzzlePiece)) goto EXIT;  //we have rotated so leave.
			//					//if(OnRotateRequest(keyboardState, prevKeyboardState, CurrentlyClonedPuzzlePiece)) goto EXIT; //we have rotated so leave.
			//					if (OnPlacementRequest(mouseState, prevMouseState, hexcell, CurrentlyClonedPuzzlePiece)) goto EXIT;  //we placed, so leave this state.
			//					if(OnCancelRequest(mouseState, prevMouseState, CurrentSelectedPuzzlePiece)) goto EXIT; ; //we have canceled so leave this state.

			//					//we need to also account for the last state. The pick up state
			//				}
			//				else
			//				{
			//					if (OnRotateRequest(keyboardState, prevKeyboardState, CurrentlyClonedPuzzlePiece)) goto EXIT; ; //we have rotated so leave.
			//					if (OnCancelRequest(mouseState, prevMouseState, CurrentSelectedPuzzlePiece)) goto EXIT; ; //Check to see if we need to cancel when NOT over the grid
			//					CurrentlyClonedPuzzlePiece.XPos = Mouse.GetState().X;
			//					CurrentlyClonedPuzzlePiece.YPos = Mouse.GetState().Y;

			//				}
			//			}
			//			//Waiting time on placement of of a piece.
			//			//We can also set up shaders, or other effects here on the placements
			//			else
			//			{
			//				//input state checks
			//				if (OnPlacementCooldownFrames()) return; //We need to wait until the placement is done. One frame has been handled so leave this state.
			//				if (OnCancelRequest(mouseState, prevMouseState, tempPiece)) return; //we have canceled so leave this state.
			//			}

			//#if Crafting_EE_Debug
			//			//FOR DEBUGGING. Currently reads out HexGrid cell linked data to Console.
			//			OnDebugRequest(mouseState, prevMouseState);
			//#endif



		}

		public void SnapToPuzzleGrid(HexCell gridCell)
		{
		//	this.CurrentSelectedPuzzlePiecePair.Key.XPos = hexcell.XPos + hexcell.Width / 2;
		//	this.CurrentSelectedPuzzlePiecePair.Key.YPos = hexcell.YPos + hexcell.Height / 2;

			//lo
			if(CurrentSelectedPuzzlePiecePair.Key != null)
			{
				this.CurrentSelectedPuzzlePiecePair.Key.XPos = gridCell.XPos + gridCell.Width/2;
				this.CurrentSelectedPuzzlePiecePair.Key.YPos = gridCell.YPos + gridCell.Height/2;
			}
		}

		public void RemovePuzzlePieceFromGrid(PuzzlePiece puzzlePiece)
		{
			PuzzleBoardGrid2D.RemovePuzzlePieceFromGrid(puzzlePiece);

			//			for (int i = 0; i < puzzlePiece.InternalCells.Count; i++)
			//			{
			//				int tempindex = PuzzleBoardGrid2D.DataCells.Values.ToList()
			//					.FindIndex(x => x.LinkedDataCell == puzzlePiece.InternalCells[i]);
			//				if (tempindex != -1)
			//				{
			//					PuzzleBoardGrid2D.DataCells.Values.ToList()[tempindex].LinkedDataCell = null;
			//					puzzlePiece.InternalCells[i].LinkedGridCell = null;
			//					PuzzleBoardGrid2D.DataCells.Values.ToList()[tempindex].ResetColorEdges();
			//#if Crafting_EE_Debug
			//					System.Diagnostics.Debug.WriteLine("DELETED GRIDCELL'S LINKED PUZZLE PIECE CELL DATA.");
			//#endif
			//				}
			//			}
			//			//we need to remove that piece from the grid list
			//			this.gridPuzzlePieces.Remove(puzzlePiece);
			//ReCalcuatePointValues();

		}

		public float GetInstabilityRate(int totalNonHomogenousConnections)
		{
			float corruptedmultiplier = 1;
			for (int i = CorruptedStability_LookUpTable.Count - 1; i >= 0; i--)
			{
				if (totalNonHomogenousConnections >= _corruptedStability_LookUpTable[i].Item1)
				{
					corruptedmultiplier += _corruptedStability_LookUpTable[i].Item2;
					break;
				}
			}

			return corruptedmultiplier;
		}

		/// <summary>
		/// <para>This should ONLY BE CALLED AFTER the copy piece is set to null </para>
		/// <para>this method is here to recalculate the minigames points based on the current state of the grid. </para>
		/// </summary>
		private void ReCalcuatePointValues()
		{
			//TODO: this method is O^3... this might need to be sent to another thread to avoid GUI thread freeze time will tell i guess


			//In order to calculate for this data we need to track some values.
			CraftingHelpers.ElementalPoints totalMagicTypeCounts_S = new CraftingHelpers.ElementalPoints();

			MatchingConections_S totalInternalHomogenousConnections_S = new MatchingConections_S();
			MatchingConections_S totalHomogenousConnections_S = new MatchingConections_S();
			NonMatchingConnections_S totalNonHomogenousConnections_S = new NonMatchingConnections_S();

			//We need to iterate through every single puzzle piece on the frid
			foreach(PuzzlePiece piece in gridPuzzlePieces)
			{
				//First up count the magic types
				totalMagicTypeCounts_S.AddElementalValue(piece.MagicType, piece.ElementalCellCounts_S.GetValueFromMagicType(piece.MagicType));
				totalInternalHomogenousConnections_S.AddElementalConnection(piece.MagicType, piece.HomogenousConnectionCounts.GetValueFromMagicType(piece.MagicType));

				//Next up we need to check the internal cells of the puzzle pieces to count the number of external connections
				foreach (PuzzlePieceHexCell innerCell in piece.InternalCells)
				{

					//ohhh i lied, it's O^3
					for (int i = 0; i < 6; i++)
					{
						//Let's keep a local variable of the piece we are comparing to, because casting tiakes time.
						PuzzlePieceHexCell toCompareCell = innerCell.LinkedGridCell?.GetConnection(i)?.ChildCell.LinkedDataCell;
						if (innerCell.LinkedGridCell?.GetConnection(i) != null && toCompareCell != null && toCompareCell.GetPuzzlePieceParent() != piece)
						{
							//if (toCompareCell.bCalcUsedFlag) continue;// We already used this so skip it to avoid double counting.
							if (toCompareCell.AlreadyCalculatedRefs.Contains(innerCell) || innerCell.AlreadyCalculatedRefs.Contains(toCompareCell)) continue;
							//Are these two the same?
							if (innerCell.MagicType == toCompareCell.GetPuzzlePieceParent().MagicType)
							{
								totalHomogenousConnections_S.AddElementalConnection(piece.MagicType);
							}
							else
							{
								totalNonHomogenousConnections_S.AddElementalConnection(piece.MagicType);
								totalNonHomogenousConnections_S.AddElementalConnection(toCompareCell.MagicType);
							}
							//innerCell.bCalcUsedFlag = true;
							toCompareCell.AlreadyCalculatedRefs.Add(innerCell);
						}
					}


					//if (innerCell.GetConnection(0) != null && (innerCell.GetConnection(0) as PuzzlePieceHexCell).GetPuzzlePieceParent() != piece )
					//{
					//	//Are these two the same?
					//	if(innerCell.MagicType == (innerCell.GetConnection(0) as PuzzlePieceHexCell).MagicType )
					//	{
					//		totalHomogenousConnections.AddElementalConnection(piece.MagiCType);
					//	}
					//	else
					//	{
					//		totalHomogenousConnections.AddElementalConnection(piece.MagiCType);
					//		totalNonHomogenousConnections_S.AddElementalConnection((innerCell.GetConnection(0) as PuzzlePieceHexCell).MagicType);
					//	}
					//}
				}
			}

			//by this point we have scaned EVERY puzzle piece and it's connection and have aqquired the correct data in order to calculate the point values of the minigame.
			float corruptedmultiplier = 1;
			for(int i = CorruptedStability_LookUpTable.Count-1; i >= 0; i--)
			{
				if (totalNonHomogenousConnections_S.GetConnectionSum() >= _corruptedStability_LookUpTable[i].Item1)
				{
					corruptedmultiplier += _corruptedStability_LookUpTable[i].Item2;
					break;
				}
			}
			
			//Let's write the calculation down here so we can understand the bottom
			// (Num of Placed[Current Element] + ( (internalSame Connections [Per element] - Same ExternalConnection [Per Element]) * 2 ) + Same External Connection * 3 ) * Corruption Multiplier 
			ElementalEffectFirePoints = (int)((totalMagicTypeCounts_S.Fire + ((totalInternalHomogenousConnections_S.Fire - totalHomogenousConnections_S.Fire) * 2) + totalHomogenousConnections_S.Fire * 3) * corruptedmultiplier);
			ElementalEffectIcePoints = (int)((totalMagicTypeCounts_S.Ice + ((totalInternalHomogenousConnections_S.Ice - totalHomogenousConnections_S.Ice) * 2) + totalHomogenousConnections_S.Ice * 3) * corruptedmultiplier);
			ElementalEffectEarthPoints = (int)((totalMagicTypeCounts_S.Earth + ((totalInternalHomogenousConnections_S.Earth - totalHomogenousConnections_S.Earth) * 2) + totalHomogenousConnections_S.Earth * 3) * corruptedmultiplier);
			ElementalEffectWaterPoints = (int)((totalMagicTypeCounts_S.Water + ((totalInternalHomogenousConnections_S.Water - totalHomogenousConnections_S.Water) * 2) + totalHomogenousConnections_S.Water * 3) * corruptedmultiplier);
			ElementalEffectLightningPoints = (int)((totalMagicTypeCounts_S.Lightning + ((totalInternalHomogenousConnections_S.Lightning - totalHomogenousConnections_S.Lightning) * 2) + totalHomogenousConnections_S.Lightning * 3) * corruptedmultiplier);
			ElementalEffectExplosivePoints = (int)((totalMagicTypeCounts_S.Explosive + ((totalInternalHomogenousConnections_S.Explosive - totalHomogenousConnections_S.Explosive) * 2) + totalHomogenousConnections_S.Explosive * 3 * corruptedmultiplier));
			ElementalEffectShadowPoints = (int)((totalMagicTypeCounts_S.Shadow + ((totalInternalHomogenousConnections_S.Shadow - totalHomogenousConnections_S.Shadow) * 2) + totalHomogenousConnections_S.Shadow * 3) * corruptedmultiplier);
			ElementalEffectLuminousPoints = (int)((totalMagicTypeCounts_S.Luminous + ((totalInternalHomogenousConnections_S.Luminous - totalHomogenousConnections_S.Luminous) * 2) + totalHomogenousConnections_S.Luminous * 3) * corruptedmultiplier);

			//Let's change the stability bar.
			//_Stability_ProgressBar.CurrentVal = _Stability_ProgressBar.MaxVal - Math.Abs(((int)(_Stability_ProgressBar.MaxVal * ((totalNonHomogenousConnections_S.GetConnectionSum() * 5) - (totalHomogenousConnections_S.GetConnectionSum() * 2)) / 100.0f)));

			//Corruption Bar Equation is ({(External Not Same Connections (total) * 5) - (External Same Connection (total) *2) }/100) * ProgressBar MAX
			if (totalNonHomogenousConnections_S.GetConnectionSum() > 0)
				_Stability_ProgressBar.CurrentVal = (int)(_Stability_ProgressBar.MaxVal * (1 - Math.Abs(((totalNonHomogenousConnections_S.GetConnectionSum() * 5) - (totalHomogenousConnections_S.GetConnectionSum() * 2))) / 100.0f));
			else _Stability_ProgressBar.CurrentVal = _Stability_ProgressBar.MaxVal;

#if Crafting_EE_Debug
			//I wanna see the data!
			System.Diagnostics.Trace.WriteLine(totalMagicTypeCounts_S.ToString());
			System.Diagnostics.Trace.WriteLine(totalHomogenousConnections_S.ToString());
			System.Diagnostics.Trace.WriteLine(totalNonHomogenousConnections_S.ToString());
			System.Diagnostics.Trace.WriteLine( "INTERNAL" + totalInternalHomogenousConnections_S.ToString());

			System.Diagnostics.Trace.WriteLine(String.Format("The Corrupted Multiplier: {0}", corruptedmultiplier));
			System.Diagnostics.Trace.WriteLine(String.Format("The Current Stability Value: {0}", _Stability_ProgressBar.CurrentVal));
#endif


		}

#region InputHelpers

		//OnPlacementWaitFrames
		private bool OnPlacementCooldownFrames()
		{
			return false;

			////Waiting time on placement of of a piece.
			////We can also set up shaders, or other effects here on the placements
			//if (CurrentlyClonedPuzzlePiece != null && waitforpieceframes > 0)
			//{
			//	//Snap to grid
			//	HexCell hexcell =
			//		PuzzleBoardGrid2D.DataCells.Values.FirstOrDefault(x => x.IsInside(Mouse.GetState().Position.ToVector2()));

			//	if (hexcell != null)
			//	{
			//		CurrentlyClonedPuzzlePiece.XPos = hexcell.XPos + hexcell.Width / 2;
			//		CurrentlyClonedPuzzlePiece.YPos = hexcell.YPos + hexcell.Height / 2;
			//	}
			//	else
			//	{
			//		CurrentlyClonedPuzzlePiece.XPos = Mouse.GetState().X;
			//		CurrentlyClonedPuzzlePiece.YPos = Mouse.GetState().Y;
			//	}

			//	waitforpieceframes--;
			//	return true;
			//}
			//return false;
		}

		//Rotation
		private bool OnRotateRequest(KeyboardState keyboardState, KeyboardState prevKeyboardState, PuzzlePiece desiredPuzzlePiece)
		{

			//Rotate to the Right/CW
			if (keyboardState.IsKeyDown(Keys.E) && prevKeyboardState.IsKeyUp(Keys.E))
			{
#if Crafting_EE_Debug
				DebugTextblock_TextBind = "Rotate request Received [CW]";
#endif

				//create stacks for recursive calls.
				LinkedList<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>> puzzlepieceCell_LL =
					new LinkedList<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>>();
				puzzlepieceCell_LL.AddFirst(new CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>()
				{ PuzzlePieceHexCell = desiredPuzzlePiece.SelectedHexCell, ConnDirection = -1 }); //-1 to indicate this is the ANCHOR

				desiredPuzzlePiece.ResetPlacementFlag();
				desiredPuzzlePiece.SelectedHexCell.bIsPlaced = true; //don't allow the anchor to move
				RotatePuzzlePiece(desiredPuzzlePiece.SelectedHexCell, puzzlepieceCell_LL, true);


				prevKeyboardState = keyboardState;

				////increment counter.
				//if (desiredPuzzlePiece.RotationCount == 6)
				//{
				//	desiredPuzzlePiece.RotationCount = 0;
				//}
				//else
				//{
				//	desiredPuzzlePiece.RotationCount++;
				//}

				return true; //has rotated.
			}

			//Rotate to the left/CCW
			else if (keyboardState.IsKeyDown(Keys.Q) && prevKeyboardState.IsKeyUp(Keys.Q))
			{
#if Crafting_EE_Debug
				DebugTextblock_TextBind = "Rotate request Received [CCW]";
#endif

				prevKeyboardState = keyboardState;
				return true; //Has rotated
			}
			else
			{

				prevKeyboardState = keyboardState;
				return false; //INVALID will not rotate.
			}
		}

		//Placement
		private bool OnPlacementRequest(MouseState mouseState, MouseState prevMouseState, HexCell hexcell, PuzzlePiece desiredPuzzlePiece)
		{
			bool retb = false;


			return retb;


//			List<HexCell> LinkedGridCells = new List<HexCell>();
//			if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
//			{

//				//create stacks for recursive calls.
//				Stack<HexCell> gridCellStack = new Stack<HexCell>();
//				gridCellStack.Push(hexcell);
//				Stack<PuzzlePieceHexCell> puzzlepieceCellStack = new Stack<PuzzlePieceHexCell>();
//				puzzlepieceCellStack.Push(desiredPuzzlePiece.SelectedHexCell);
//				Stack<int> connectionIntsStack = new Stack<int>();
//				connectionIntsStack.Push(0);

//				//Remember to RESET the piece everytime. else false positives will occur.
//				desiredPuzzlePiece.ResetPlacementFlag(); //Sets the bool flag to false.

//				//CAN WE PLACE IT DOWN
//				LinkedList<Tuple<PuzzlePieceHexCell, HexCell>> retLL = new LinkedList<Tuple<PuzzlePieceHexCell, HexCell>>();

//				try
//				{
//					retLL = CanPlacePuzzlePiece(desiredPuzzlePiece, hexcell, true);
//				}
//				catch(Exception ex)
//				{
//					Console.WriteLine(ex.Message);
//#if Crafting_EE_Debug
//					DebugTextblock_TextBind = "Tried To place Piece, BUT it off the grid...";
//#endif
//					return false;
//				}

//				if (retLL == null)
//				{
//#if Crafting_EE_Debug
//					DebugTextblock_TextBind = "Tried To place Piece, but it is overlapping another piece.";
//					return false; //we failed... do not place.
//#endif
//				}
//				else
//				{

//					//At this point we know what cells we need to fill in from the check method return value.

//					foreach (var v in retLL)
//					{
//						v.Item2.LinkedDataCell = v.Item1;
//						v.Item1.LinkedGridCell = v.Item2;
//						v.Item2.FillColor = v.Item1.FillColor;
//						LinkedGridCells.Add(v.Item2);
//					}
//				}

//				//Placement logic DISPLAY WISE
//				CurrentlyClonedPuzzlePiece.XPos = hexcell.XPos + (hexcell.Width / 2) + CurrentlyClonedPuzzlePiece.XOffset;
//				CurrentlyClonedPuzzlePiece.YPos = hexcell.YPos + (hexcell.Height / 2) + CurrentlyClonedPuzzlePiece.YOffset;

//				//We need rescale the ACTUAL piece not the cloned piece so it fits the grid.
//				desiredPuzzlePiece.ResetRescalePiece(1.0f, 1.0f, CurrentlyClonedPuzzlePiece.InternalCells);
//				//desiredPuzzlePiece.ResetRescalePiece(1.0f, 1.0f);

//				//We need to apply the rotation to this piece if there is any
//				for (int i = 0; i < desiredPuzzlePiece.InternalCells.Count; i++)
//				{
//					//desiredPuzzlePiece.InternalCells[i] = CurrentlyClonedPuzzlePiece.InternalCells[i];
//					desiredPuzzlePiece.InternalCells[i].XPos = CurrentlyClonedPuzzlePiece.InternalCells[i].XPos;
//					desiredPuzzlePiece.InternalCells[i].YPos = CurrentlyClonedPuzzlePiece.InternalCells[i].YPos;

//					//We need to UPDATE THE LOSSLESS property too! this allows the pick up to offset right

//					desiredPuzzlePiece.InternalCells[i].XPos_Lossless = CurrentlyClonedPuzzlePiece.InternalCells[i].XPos_Lossless;
//					desiredPuzzlePiece.InternalCells[i].YPos_Lossless = CurrentlyClonedPuzzlePiece.InternalCells[i].YPos_Lossless;
//				}

//				desiredPuzzlePiece.XOffset = 0;
//				desiredPuzzlePiece.YOffset = 0;

//				desiredPuzzlePiece.XPos = CurrentlyClonedPuzzlePiece.XPos;
//				desiredPuzzlePiece.YPos = CurrentlyClonedPuzzlePiece.YPos;
//				desiredPuzzlePiece.bIsActive = true; //just in case we hid it from view.
//				desiredPuzzlePiece.bIsOnGrid = true; //small flag to avoid duplicate confusion.
//				desiredPuzzlePiece.bIsSelected = false;

//				desiredPuzzlePiece.SetTransparency(1f);
//				desiredPuzzlePiece.SelectedHexCell.bIsPlaced = true; // a flag to avoid infinite looping.

//				desiredPuzzlePiece.ResetPlacementFlag(); //Sets the bool flag to false.

//				gridCellStack = new Stack<HexCell>();
//				gridCellStack.Push(hexcell);
//				puzzlepieceCellStack = new Stack<PuzzlePieceHexCell>();
//				puzzlepieceCellStack.Push(desiredPuzzlePiece.SelectedHexCell);
//				connectionIntsStack = new Stack<int>();
//				connectionIntsStack.Push(0);

//				//placement logic, connection and data wise.
//				//PuzzlePieceConnectionPlacement(desiredPuzzlePiece.SelectedHexCell, gridCellStack, puzzlepieceCellStack, connectionIntsStack);
//				this.PuzzleBoardGrid2D.CurrentPuzzlePieces.Add( CurrentlyClonedPuzzlePiece, LinkedGridCells);
//				CurrentlyClonedPuzzlePiece = null;

//				//By the point the copy of the puzzle piece should be dead, and we have to use the ACTUAL puzzle piece.
//				gridPuzzlePieces.Add(this.CurrentSelectedPuzzlePiece);
//				this.CurrentSelectedPuzzlePiece = null;
//				this.ReCalcuatePointValues();

//				return true; // VALID placement.
//			}
//			else
//			{
//				//INVAILD placement
//				return false;
//			}
		}

		//Canceling
		private bool OnCancelRequest(MouseState mouseState, MouseState prevMouseState, PuzzlePiece desiredPuzzlePiece)
		{
			bool retb = false;


			return retb;

//			//THIS IS OUTSIDE OF THE NORMAL INPUT SECTION because that section only works for if on hover over grid == true. which we need both if on or off the grid
//			if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
//			{
//				HexCell hexcell = PuzzleBoardGrid2D.DataCells.Values.FirstOrDefault(x => x.IsInside(Mouse.GetState().Position.ToVector2()) && x.LinkedDataCell != null);
//				if (hexcell == null && CurrentlyClonedPuzzlePiece == null) return false; //Don't do it

			//				//Needs to check for selected state. AKA we are moving it.
			//				else if (CurrentlyClonedPuzzlePiece != null)
			//				{

			//					CurrentSelectedPuzzlePiece.bIsActive = true; //just in case we hid it from view.
			//					CurrentSelectedPuzzlePiece.bIsOnGrid = false; //small flag to avoid duplicate confusion.
			//					CurrentSelectedPuzzlePiece.bIsSelected = false;
			//					CurrentSelectedPuzzlePiece.bIsSelectable = true;
			//					CurrentSelectedPuzzlePiece.SetTransparency(1f);
			//					CurrentSelectedPuzzlePiece.ResetRescalePiece(.25f, .25f);
			//					CurrentSelectedPuzzlePiece.ResetToSpawnPosition();
			//					CurrentlyClonedPuzzlePiece = null;

			//#if Crafting_EE_Debug
			//					DebugTextblock_TextBind = String.Format("Let Go of current selected piece (while moving)");
			//#endif
			//					this.CurrentlyClonedPuzzlePiece = null;
			//					this.CurrentSelectedPuzzlePiece = null;
			//					return true;  // cancel request finished, and piece is reset.
			//				}
			//				else
			//				{
			//					//We are in a grid cell with a puzzle piece.
			//					desiredPuzzlePiece = hexcell.LinkedDataCell.GetPuzzlePieceParent();

			//					//Reset the piece.
			//					RemovePuzzlePieceFromGrid(desiredPuzzlePiece);

			//					//tempPiece = new PuzzlePiece(tempPiece, .25f, .25f) { bIsActive = true, XOffset = 0, YOffset = 0, bIsOnGrid = true, bIsSelected = false };

			//					desiredPuzzlePiece.SetTransparency(1f); //small flag to avoid duplicate confusion.


			//					desiredPuzzlePiece.ResetRescalePiece(.25f, .25f);
			//					desiredPuzzlePiece.XOffset = 0;
			//					desiredPuzzlePiece.YOffset = 0;
			//					desiredPuzzlePiece.ResetToSpawnPosition();
			//					desiredPuzzlePiece.bIsActive = true; //just in case we hid it from view.
			//					desiredPuzzlePiece.bIsOnGrid = false; //small flag to avoid duplicate confusion.
			//					desiredPuzzlePiece.bIsSelected = false;
			//					desiredPuzzlePiece.bIsSelectable = true;

			//					desiredPuzzlePiece.SetTransparency(1f);


			//					this.CurrentlyClonedPuzzlePiece = null;
			//					this.CurrentSelectedPuzzlePiece = null;

			//#if Crafting_EE_Debug
			//					DebugTextblock_TextBind = String.Format("Location On puzzle piece was reset");
			//#endif
			//					return true;  // cancel request finished, and piece is reset.

			//				}

			//			}
			//			else
			//			{
			//				return false; //no cancel request
			//			}
		}

		//DEBUG.
#if Crafting_EE_Debug
		private void OnDebugRequest(MouseState mouseState, MouseState prevMouseState)
		{
			if (mouseState.MiddleButton == ButtonState.Pressed && prevMouseState.MiddleButton == ButtonState.Released)
			{
				//System.Diagnostics.Debug.WriteLine(CurrentlyClonedPuzzlePiece.SelectedHexCell.LinkedDataCell);
				//Snap to grid
				HexCell hexcell =
					PuzzleBoardGrid2D.DataCells.Values.FirstOrDefault(x => x.IsInside(Mouse.GetState().Position.ToVector2()));
				if (hexcell != null)
					System.Diagnostics.Debug.WriteLine("Linked Data Cell:" + (hexcell.LinkedDataCell == null ? "null" : hexcell.LinkedDataCell.ToString()));
			}
		}
#endif



#endregion

#endregion

#region Monogame

		public void Load(ContentManager content, GraphicsDevice graphicsDevice, SpriteFont sf, int resWidth, int resHeight, int horiCells, int vertCells, int cellWidth, int cellHeight)
		{
			if(_Contentref == null)
				_Contentref = content;

			_spriteFont = sf;

			_resolutionHeight = resHeight;
			_resolutionWidth = resWidth;

			_backgroundGrid_GameImage = new GameImage("BackgroundImage", 0, 0, _resolutionWidth, _resolutionHeight- 20,
				0, false, 0, 0, "", "#00000000", _Contentref.Load<Texture2D>("Images/BackgroundImage")){bIsActive = true};

			_pcGirl_GameImage = new GameImage("PCGirlImage", (int)(_resolutionWidth * .8f), (int)(_resolutionHeight * .25f), 
					(int)(_resolutionWidth *.2f), (int)(_resolutionHeight * .50f), 0, false, 0, 0, "", "#00000000", _Contentref.Load<Texture2D>("Images/PCGirl"))
				{ bIsActive = true };


			_Stability_ProgressBar = new GameProgressBar("StabilityBar", (int)(_resolutionWidth * .79f), (int)(_resolutionHeight * .27f),
				50, (int)(_resolutionHeight * .71f), 0, true, 2, 0, 0, 1000, 1000, true,  false, graphicsDevice, Color.IndianRed,Color.DarkRed ) 
				{bIsActive = true};

			PuzzleBoardGrid2D = new HexGrid2D(horiCells, vertCells, 
				(int)(_resolutionWidth *.205f),(int)(_resolutionHeight * .270f),
				(int)(_resolutionWidth * .60f), (int)(_resolutionHeight * .60f),
				cellWidth, cellHeight, _Contentref.Load<Texture2D>("Images/HexagonCell_Fill"),
				_Contentref.Load<Texture2D>("Images/HexagonCell"),
				_spriteFont
				);

			//Elemental Effects section

#region Elemental Effects

			
			FireEffectsHeader_GTB = new GameTextBlock("FireEffectsLabel", (int)(_resolutionWidth * .205f), (int)(_resolutionHeight * .065), (int)(_resolutionWidth * .09f), 30,
			  2, true,0,0, "Fire", 0f, "#00000000", "", _spriteFont, null, Color.DarkRed) { bIsActive = true, bMiddleHorizontal = true};
			IceEffectsHeader_GTB = new GameTextBlock("IceEffectsLabel", (int)(_resolutionWidth * .305f), (int)(_resolutionHeight * .065), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "Ice", 0f, "#00000000", "", _spriteFont, null, Color.LightBlue) { bIsActive = true, bMiddleHorizontal = true };
			EarthEffectsHeader_GTB = new GameTextBlock("EarthEffectsLabel", (int)(_resolutionWidth * .405f), (int)(_resolutionHeight * .065), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "Earth", 0f, "#00000000", "", _spriteFont, null, Color.Brown) { bIsActive = true, bMiddleHorizontal = true };
			WaterEffectsHeader_GTB = new GameTextBlock("WaterEffectsLabel", (int)(_resolutionWidth * .505f), (int)(_resolutionHeight * .065), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "Water", 0f, "#00000000", "", _spriteFont, null, Color.DarkBlue) { bIsActive = true, bMiddleHorizontal = true };
			LightningEffectsHeader_GTB = new GameTextBlock("LightningEffectsLabel", (int)(_resolutionWidth * .605f), (int)(_resolutionHeight * .065), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "Lightning", 0f, "#00000000", "", _spriteFont, null, Color.Goldenrod) { bIsActive = true, bMiddleHorizontal = true };
			ExplosiveEffectsHeader_GTB = new GameTextBlock("ExplosiveEffectsLabel", (int)(_resolutionWidth * .705f), (int)(_resolutionHeight * .065), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "Explosive", 0f, "#00000000", "", _spriteFont, null, Color.Orange) { bIsActive = true, bMiddleHorizontal = true };
			ShadowEffectsHeader_GTB = new GameTextBlock("ShadowEffectsLabel", (int)(_resolutionWidth * .805f), (int)(_resolutionHeight * .065), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "Shadow", 0f, "#00000000", "", _spriteFont, null, Color.Black) { bIsActive = true, bMiddleHorizontal = true };
			LuminousEffectsHeader_GTB = new GameTextBlock("LuminousEffectsLabel", (int)(_resolutionWidth * .905f), (int)(_resolutionHeight * .065), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "Luminous", 0f, "#00000000", "", _spriteFont, null, Color.LightGray) { bIsActive = true, bMiddleHorizontal = true };


			FireEffectsMinPoints_GTB = new GameTextBlock("FireEffectsMinPoints", (int)(_resolutionWidth * .205f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 5, 0, "0", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8, };
			IceEffectsMinPoints_GTB = new GameTextBlock("IceEffectsMinPoints", (int)(_resolutionWidth * .305f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 5, 0, "0", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8, };
			EarthEffectsMinPoints_GTB = new GameTextBlock("EarthEffectsMinPoints", (int)(_resolutionWidth * .405f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 5, 0, "0", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8, };
			WaterEffectsMinPoints_GTB = new GameTextBlock("WaterEffectsMinPoints", (int)(_resolutionWidth * .505f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 5, 0, "0", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8, };
			LightningEffectsMinPoints_GTB = new GameTextBlock("LightningEffectsMinPoints", (int)(_resolutionWidth * .605f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 5, 0, "0", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8, };
			ExplosiveEffectsMinPoints_GTB = new GameTextBlock("ExplosiveEffectsMinPoints", (int)(_resolutionWidth * .705f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 5, 0, "0", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8, };
			ShadowEffectsMinPoints_GTB = new GameTextBlock("ShadowEffectsMinPoints", (int)(_resolutionWidth * .805f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 5, 0, "0", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8, };
			LuminousEffectsMinPoints_GTB = new GameTextBlock("LuminousEffectsMinPoints", (int)(_resolutionWidth * .905f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 5, 0, "0", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8, };



			FireEffectsMaxPoints_GTB = new GameTextBlock("FireEffectsMaxPoints", (int)(_resolutionWidth * .275f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "150", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8};
			IceEffectsMaxPoints_GTB = new GameTextBlock("IceEffectsMaxPoints", (int)(_resolutionWidth * .375f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "150", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8 };
			EarthEffectsMaxPoints_GTB = new GameTextBlock("IceEffectsMaxPoints", (int)(_resolutionWidth * .475f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "150", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8 };
			WaterEffectsMaxPoints_GTB = new GameTextBlock("WaterEffectsMaxPoints", (int)(_resolutionWidth * .575f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "150", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8 };
			LightningEffectsMaxPoints_GTB = new GameTextBlock("LightningEffectsMaxPoints", (int)(_resolutionWidth * .675f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "150", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8 };
			ExplosiveEffectsMaxPoints_GTB = new GameTextBlock("ExplosiveEffectsMaxPoints", (int)(_resolutionWidth * .775f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "150", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8 };
			ShadowEffectsMaxPoints_GTB = new GameTextBlock("ShadowEffectsMaxPoints", (int)(_resolutionWidth * .875f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "150", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8 };
			LuminousEffectsMaxPoints_GTB = new GameTextBlock("LuminousEffectsMaxPoints", (int)(_resolutionWidth * .975f), (int)(_resolutionHeight * .085), (int)(_resolutionWidth * .09f), 30,
					2, true, 0, 0, "150", 0f, "#00000000", "", _spriteFont, null, Color.DarkGray) { bIsActive = true, bMiddleHorizontal = false, FontSize = 8 };


			FireEffectsPointBar_GPB = new GameProgressBar("FireEffectsPointBar", (int)(_resolutionWidth * .205f), (int)(_resolutionHeight * .105f), (int)(_resolutionWidth * .09f), 25,
				2, true, 1, 0, 0, 0, 150, false, true, graphicsDevice, Color.DarkRed, Color.Black){bIsActive = true};
			IceEffectsPointBar_GPB = new GameProgressBar("IceEffectsPointBar", (int)(_resolutionWidth * .305f), (int)(_resolutionHeight * .105f), (int)(_resolutionWidth * .09f), 25,
					2, true, 1, 0, 0, 0, 150, false, true, graphicsDevice, Color.DarkRed, Color.Black) { bIsActive = true };
			EarthEffectsPointBar_GPB = new GameProgressBar("EarthEffectsPointBar", (int)(_resolutionWidth * .405f), (int)(_resolutionHeight * .105f), (int)(_resolutionWidth * .09f), 25,
					2, true, 1, 0, 0, 0, 150, false, true, graphicsDevice, Color.DarkRed, Color.Black) { bIsActive = true };
			WaterEffectsPointBar_GPB = new GameProgressBar("EarthEffectsPointBar", (int)(_resolutionWidth * .505f), (int)(_resolutionHeight * .105f), (int)(_resolutionWidth * .09f), 25,
					2, true, 1, 0, 0, 0, 150, false, true, graphicsDevice, Color.DarkRed, Color.Black) { bIsActive = true };
			LightningEffectsPointBar_GPB = new GameProgressBar("LightningEffectsPointBar", (int)(_resolutionWidth * .605f), (int)(_resolutionHeight * .105f), (int)(_resolutionWidth * .09f), 25,
					2, true, 1, 0, 0, 0, 150, false, true, graphicsDevice, Color.DarkRed, Color.Black) { bIsActive = true };
			ExplosiveEffectsPointBar_GPB = new GameProgressBar("ExplosiveEffectsPointBar", (int)(_resolutionWidth * .705f), (int)(_resolutionHeight * .105f), (int)(_resolutionWidth * .09f), 25,
					2, true, 1, 0, 0, 0, 150, false, true, graphicsDevice, Color.DarkRed, Color.Black) { bIsActive = true };
			ShadowEffectsPointBar_GPB = new GameProgressBar("ShadowEffectsPointBar", (int)(_resolutionWidth * .805f), (int)(_resolutionHeight * .105f), (int)(_resolutionWidth * .09f), 25,
					2, true, 1, 0, 0, 0, 150, false, true, graphicsDevice, Color.DarkRed, Color.Black) { bIsActive = true };
			LuminousEffectsPointBar_GPB = new GameProgressBar("LuminousEffectsPointBar", (int)(_resolutionWidth * .905f), (int)(_resolutionHeight * .105f), (int)(_resolutionWidth * .09f), 25,
					2, true, 1, 0, 0, 0, 150, false, true, graphicsDevice, Color.DarkRed, Color.Black) { bIsActive = true };


			//ListBoxes
			FireEffectUnlocked_GLB = new GameListBox("FireEffectsUnlocks", (int)(_resolutionWidth * .205f), (int)(_resolutionHeight * .14f), (int)(_resolutionWidth * .09f), (int)(_resolutionHeight * .09f),
				2, true, 1, Color.Black, 0, 0, (int)(_resolutionWidth * .180f), 30, 4, 5,10, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None,
				graphicsDevice, null, null, EPositionType.Vertical) {bIsActive = true};
			IceEffectUnlocked_GLB = new GameListBox("FireEffectsUnlocks", (int)(_resolutionWidth * .305f), (int)(_resolutionHeight * .14f), (int)(_resolutionWidth * .09f), (int)(_resolutionHeight * .09f),
					2, true, 1, Color.Black, 0, 0, (int)(_resolutionWidth * .180f), 30, 4, 5, 10, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None,
					graphicsDevice, null, null, EPositionType.Vertical) { bIsActive = true };
			EarthEffectUnlocked_GLB = new GameListBox("EarthEffectsUnlocks", (int)(_resolutionWidth * .405f), (int)(_resolutionHeight * .14f), (int)(_resolutionWidth * .09f), (int)(_resolutionHeight * .09f),
					2, true, 1, Color.Black, 0, 0, (int)(_resolutionWidth * .180f), 30, 4, 5, 10, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None,
					graphicsDevice, null, null, EPositionType.Vertical) { bIsActive = true };
			WaterEffectUnlocked_GLB = new GameListBox("WaterEffectsUnlocks", (int)(_resolutionWidth * .505f), (int)(_resolutionHeight * .14f), (int)(_resolutionWidth * .09f), (int)(_resolutionHeight * .09f),
					2, true, 1, Color.Black, 0, 0, (int)(_resolutionWidth * .180f), 30, 4, 5, 10, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None,
					graphicsDevice, null, null, EPositionType.Vertical) { bIsActive = true };
			LightningEffectUnlocked_GLB = new GameListBox("LightningEffectsUnlocks", (int)(_resolutionWidth * .605f), (int)(_resolutionHeight * .14f), (int)(_resolutionWidth * .09f), (int)(_resolutionHeight * .09f),
					2, true, 1, Color.Black, 0, 0, (int)(_resolutionWidth * .180f), 30, 4, 5, 10, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None,
					graphicsDevice, null, null, EPositionType.Vertical) { bIsActive = true };
			ExplosiveEffectUnlocked_GLB = new GameListBox("ExplosiveEffectsUnlocks", (int)(_resolutionWidth * .705f), (int)(_resolutionHeight * .14f), (int)(_resolutionWidth * .09f), (int)(_resolutionHeight * .09f),
					2, true, 1, Color.Black, 0, 0, (int)(_resolutionWidth * .180f), 30, 4, 5, 10, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None,
					graphicsDevice, null, null, EPositionType.Vertical) { bIsActive = true };
			ShadowEffectUnlocked_GLB = new GameListBox("ShadowEffectsUnlocks", (int)(_resolutionWidth * .805f), (int)(_resolutionHeight * .14f), (int)(_resolutionWidth * .09f), (int)(_resolutionHeight * .09f),
					2, true, 1, Color.Black, 0, 0, (int)(_resolutionWidth * .180f), 30, 4, 5, 10, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None,
					graphicsDevice, null, null, EPositionType.Vertical) { bIsActive = true };
			LuminousEffectUnlocked_GLB = new GameListBox("LuminousEffectsUnlocks", (int)(_resolutionWidth * .905f), (int)(_resolutionHeight * .14f), (int)(_resolutionWidth * .09f), (int)(_resolutionHeight * .09f),
					2, true, 1, Color.Black, 0, 0, (int)(_resolutionWidth * .180f), 30, 4, 5, 10, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None,
					graphicsDevice, null, null, EPositionType.Vertical) { bIsActive = true };


#endregion
			//END OF Elemental Effects section


#if Crafting_EE_Debug
			//Debug Textblock
			_debugGameTextBlock = new GameTextBlock("debugTB", 10, _resolutionHeight - 20, _resolutionWidth, 40, 1, false, 
			0, 0, DebugTextblock_TextBind, 0, "#00000000", "", _spriteFont, null, Color.Red){bIsActive =  true};
#endif


		}

	public void Unload()
		{

		}

		public void Update(GameTime gameTime, MouseState prevMouseState, KeyboardState preKeyboardState)
		{
			if (!_bIsActive) return;

			if (TestingPiece != null) TestingPiece.Update(gameTime);
			foreach (PuzzlePiece piece in TestingPiecesList)
			{
				piece.Update(gameTime);
			}



			//update the grid, allows drawing of the piece on the grid and other things that need on tick calcs
			PuzzleBoardGrid2D.Update(gameTime);
			//Handles all events of the mouse. Left click, right click, middle mouse. ONLY WHEN ON/OVER THE GRID
			bool retEvent = PuzzleBoardGrid2D.OnMouseEvent(Mouse.GetState(), prevMouseState, PuzzleBoardGrid2D.SelectedCell);

			//Handle the Puzzle piece cell events OFF THE GRID
			if(!retEvent)
			{
				// CraftingMinigame.DebugOutToConsole("Non Grid Mouse events");
				UpdateHandler_SelectedPiece(prevMouseState, preKeyboardState);
			}

#region Elemental Effects

			for (int i = 0; i < 8; i++)
			{
				ElementalEffectUnlocks_GLB_List[i].Update(gameTime);
				ElementalEffectsHeaders_GTB_List[i].Update(gameTime);
				ElementalEffectsMaxPoints_GTB_List[i].Update(gameTime);
				ElementalEffectsMinPoints_GTB_List[i].Update(gameTime);
				ElementalEffectsPointBar_GPB_List[i].Update(gameTime);
			}

#endregion


#if Crafting_EE_Debug
			_debugGameTextBlock.Text = DebugTextblock_TextBind;
			_debugGameTextBlock.Update(gameTime);
#endif

		}

		/// <summary>
		/// Draw everything that has to do with the crafting minigame!
		/// <para></para>
		/// DO NOT CALL spriteBATCH.BEGIN BEFORE CALLING THIS
		/// </summary>
		/// <param name="spriteBatch"></param>
		/// <param name="gameTime"></param>
		public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			if(!_bIsActive) return;
			
			//Step one Draw the Background!
			//TODO: Put actual background. Currently i am using a grid for percentage placement.
			spriteBatch.Begin(SpriteSortMode.BackToFront);
			_backgroundGrid_GameImage.Draw(gameTime, spriteBatch);
			_pcGirl_GameImage.Draw(gameTime, spriteBatch);
			_Stability_ProgressBar.Draw(gameTime, spriteBatch);

			_debugGameTextBlock.Draw(gameTime, spriteBatch);

			spriteBatch.End();


			//Draw the Hex Grid. DO NOT PRE batch begin the object needs to handle shaders.
			PuzzleBoardGrid2D.Draw(spriteBatch, gameTime);

			//DELETE THIS AFTER CONFIRMED
			spriteBatch.Begin(SpriteSortMode.BackToFront);

			if (TestingPiece != null)
				TestingPiece.Draw(gameTime, spriteBatch);

			foreach (PuzzlePiece piece in TestingPiecesList)
			{
				if(!piece.bIsOnGrid)
					piece.Draw(gameTime, spriteBatch);
			}

			CurrentSelectedPuzzlePiecePair.Key?.Draw(gameTime, spriteBatch);

			#region Elemental Point UI
			for (int i = 0; i < 8; i++)
			{
				ElementalEffectUnlocks_GLB_List[i].Draw(gameTime, spriteBatch);
				ElementalEffectsHeaders_GTB_List[i].Draw(gameTime, spriteBatch);
				ElementalEffectsMaxPoints_GTB_List[i].Draw(gameTime, spriteBatch);
				ElementalEffectsMinPoints_GTB_List[i].Draw(gameTime, spriteBatch);
				ElementalEffectsPointBar_GPB_List[i].Draw(gameTime, spriteBatch);
			}
			#endregion


			spriteBatch.End();
		}

		#endregion

		#endregion

		public static void DebugOutToConsole(String s)
		{
#if Crafting_Crafting_EE_Debug
			Console.WriteLine(s);
#endif
		}

	}
}
