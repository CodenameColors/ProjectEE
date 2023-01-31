using System;
using System.Collections.Generic;
using BixBite.Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Crafting.HexGrid
{
	public class HexGrid2D
	{
		
		public struct CellIndexerStruct
		{
			public int Row { get; set; }
			public int Column { get; set; }


		}


		#region Delegates/Events

		#endregion

		#region Fields
		private int _cellWidth;
		private int _cellHeight;

		private int _maxHoriCells = -1;
		private int _maxVertCells = -1;

		private int _xpos = -1;
		private int _ypos = -1;

		private int _width = -1;
		private int _height = -1;

		private CellIndexerStruct _gridIndexer = new CellIndexerStruct(); //This is so i don't have to make a new tuple every time lol
		private Rectangle _drawRectangle = new Rectangle();

		private HexCell _selectedCell = null;

		private SpriteFont spriteFont;

		// Structs used to keep track of all required data for stats!
		private CraftingHelpers.ElementalPoints _elementCellsCount = new CraftingHelpers.ElementalPoints();
		private CraftingHelpers.ElementalPoints _internalPuzzlePieceConnectionCounts = new CraftingHelpers.ElementalPoints();
		private CraftingHelpers.ElementalPoints _externalHomogenousConnectionCounts = new CraftingHelpers.ElementalPoints();
		private CraftingHelpers.ElementalPoints _externalNonHomogenousConnectionCounts = new CraftingHelpers.ElementalPoints();

		#endregion

		#region Properties

		public Dictionary<CellIndexerStruct, HexCell> DataCells = new Dictionary<CellIndexerStruct, HexCell>();
		//public Dictionary<PuzzlePiece, LinkedList<Tuple<PuzzlePieceHexCell, HexCell>>> CurrentPuzzlePieces = new Dictionary<PuzzlePiece, List<HexCell>>();

		/// <summary>
		/// holds all the CURRENT PUZZLE PIECES ON THE GRID.
		/// </summary>
		//List<PuzzlePiece> CurrentPuzzlePieces = new List<PuzzlePiece>();

		/// <summary>
		/// Key = All puzzle pieces ON GRID. Value is the linked Original non grid puzzle piece.
		/// </summary>
		Dictionary<PuzzlePiece, PuzzlePiece> CurrentLinkedPuzzlepieces = new Dictionary<PuzzlePiece, PuzzlePiece>();

		Dictionary<HexCell, PuzzlePiece> gridCellPuzzlePieces_Dict = new Dictionary<HexCell, PuzzlePiece>();
		public int XPos
		{
			get => _xpos;
			set => _xpos = value;
		}
		public int YPos
		{
			get => _ypos;
			set => _ypos = value;
		}

		public int CurrentFirePoints = 0;
		public int CurrentIcePoints = 0;
		public int CurrentEarthPoints = 0;
		public int CurrentWaterPoints = 0;
		public int CurrentLightningPoints = 0;
		public int CurrentExplosivePoints = 0;
		public int CurrentShadowPoints = 0;
		public int CurrentLuminousPoints = 0;

		public Rectangle DrawRectangle
		{
			get
			{
				_drawRectangle.X = _xpos;
				_drawRectangle.Y = _ypos;
				_drawRectangle.Width = _width;
				_drawRectangle.Height = _height;
				return _drawRectangle;
			}
			set => _drawRectangle = value;
		}

		public HexCell SelectedCell
		{
			get => _selectedCell;
			set
			{
				
				_selectedCell = value;
				if (SelectedCell == null) return;
					CraftingMinigame.Instance.DebugTextblock_TextBind = 
					String.Format("Selected Cell Changed. Now it's Row[{0}], Column[{1}]", _selectedCell.Row, _selectedCell.Column);
					//CraftingMinigame.Instance.SnapToPuzzleGrid(SelectedCell);
			}
		}

		#endregion

		#region Constructor
		/// <summary>
		/// Inits the hexcell grid. As well as create and map/route all the cells connections.
		/// <para></para>
		/// DEFAULTED data == integer => cell number (non zero based).
		/// </summary>
		/// <param name="horiCells"></param>
		/// <param name="vertCells"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="cellwidth"></param>
		/// <param name="cellheight"></param>
		/// <param name="hexCellImage"></param>
		public HexGrid2D(int horiCells, int vertCells, int x, int y, int w, int h, int cellwidth, int cellheight, Texture2D hexCellImageFill, Texture2D hexCellImageOutline, SpriteFont sf)
		{
			_cellWidth = cellwidth;
			_cellHeight = cellheight;

			_maxHoriCells = horiCells;
			_maxVertCells = vertCells;

			_xpos = (int)x;
			_ypos = (int)y;
			_width = w;
			_height = h;

			spriteFont = sf;

			//INIT the base grid.
			for (int i = 0; i < horiCells * vertCells; i++)
			{
				//Add hex cell will AUTO set the row, and column. so they can be ignored here.
				this.AddHexCell(new HexCell(this, i, XPos, YPos, cellwidth, cellheight, 0,0, i + 1, hexCellImageFill, this, hexCellImageOutline, sf){});
			}


		}
		#endregion

		#region Methods

		#region Helpers

		public void UpdateElementalPointValues()
		{

			/************************ EQUATION DEFINITION (VERSION 1 ) ******************************************/
			// The equation (version 1) goes like this. This is for EACH magic type
			// ([cells_placed] + ((internal_connections - external_homogen) * 2) +  (external_homogen * 3) + ( moved_conn * (1 + BRate) * PRate )) * IRate

			// moved_conn = brokenoff pieces connection count
			// BRate = The hex cell that we broke off and moved connects to new external connections. These are Homogenous magic types TODO: Make this actually work.
			// PRate = The hex cell that we broke off and moved connects to new external connections. These are Non Homogenous magic types TODO: Make this actually work.
			// IRate = Instability rate. ==> 1 + [external_homogen] * IRateLUT[external_homogen].
			/************************ END OF EQUATION DEFINITION (VERSION 1 ) ***********************************/

			float moved_conn = 0.0f;
			float Brate = 0.0f;
			float Prate = 0.0f;
			float IRate = CraftingMinigame.Instance.GetInstabilityRate(_externalNonHomogenousConnectionCounts.GetConnectionSum());

			// Let's update the minigames stats!
			CurrentFirePoints = (int)((_elementCellsCount.Fire + (( _internalPuzzlePieceConnectionCounts.Fire - _externalHomogenousConnectionCounts.Fire) * 2) + (moved_conn * (1 + Brate) * Prate)) * IRate);
			CraftingMinigame.Instance.ElementalEffectFirePoints = CurrentFirePoints;

			CurrentIcePoints = (int)((_elementCellsCount.Ice + (( _internalPuzzlePieceConnectionCounts.Ice - _externalHomogenousConnectionCounts.Ice) * 2) + (moved_conn * (1 + Brate) * Prate)) * IRate);
			CraftingMinigame.Instance.ElementalEffectIcePoints = CurrentIcePoints;

			CurrentWaterPoints = (int)((_elementCellsCount.Water + (( _internalPuzzlePieceConnectionCounts.Water - _externalHomogenousConnectionCounts.Water) * 2) + (moved_conn * (1 + Brate) * Prate)) * IRate);
			CraftingMinigame.Instance.ElementalEffectWaterPoints = CurrentWaterPoints;
			
			CurrentEarthPoints = (int)((_elementCellsCount.Earth + (( _internalPuzzlePieceConnectionCounts.Earth - _externalHomogenousConnectionCounts.Earth) * 2) + (moved_conn * (1 + Brate) * Prate)) * IRate);
			CraftingMinigame.Instance.ElementalEffectEarthPoints = CurrentEarthPoints;

			CurrentLightningPoints = (int)((_elementCellsCount.Lightning + (( _internalPuzzlePieceConnectionCounts.Lightning - _externalHomogenousConnectionCounts.Lightning) * 2) + (moved_conn * (1 + Brate) * Prate)) * IRate);
			CraftingMinigame.Instance.ElementalEffectLightningPoints = CurrentLightningPoints;

			CurrentExplosivePoints = (int)((_elementCellsCount.Explosive + (( _internalPuzzlePieceConnectionCounts.Explosive - _externalHomogenousConnectionCounts.Explosive) * 2) + (moved_conn * (1 + Brate) * Prate)) * IRate);
			CraftingMinigame.Instance.ElementalEffectExplosivePoints = CurrentExplosivePoints;

			CurrentShadowPoints = (int)((_elementCellsCount.Shadow + (( _internalPuzzlePieceConnectionCounts.Shadow - _externalHomogenousConnectionCounts.Shadow) * 2) + (moved_conn * (1 + Brate) * Prate)) * IRate);
			CraftingMinigame.Instance.ElementalEffectShadowPoints = CurrentShadowPoints;

			CurrentLuminousPoints = (int)((_elementCellsCount.Luminous + (( _internalPuzzlePieceConnectionCounts.Luminous - _externalHomogenousConnectionCounts.Luminous) * 2) + (moved_conn * (1 + Brate) * Prate)) * IRate);
			CraftingMinigame.Instance.ElementalEffectLuminousPoints = CurrentLuminousPoints;


			// Let's calculate the Stability percent. 
			int stabilityPercent = 100 - ((_externalNonHomogenousConnectionCounts.GetConnectionSum() * 5) );
			CraftingMinigame.Instance.StabilityCurrentPercent = stabilityPercent;
			CraftingMinigame.Instance.StabilityMaxPercent = 100;


		}

		public void AddElementalConnection(EMagicType desiredMagicType, int val = 0)
		{
			switch (desiredMagicType)
			{
				case EMagicType.NONE:
					break;
				case EMagicType.Fire:
					CurrentFirePoints += val;
					CraftingMinigame.Instance.ElementalEffectFirePoints = CurrentFirePoints;
					break;
				case EMagicType.Ice:
					CurrentIcePoints += val;
					CraftingMinigame.Instance.ElementalEffectIcePoints = CurrentIcePoints;
					break;
				case EMagicType.Earth:
					CurrentEarthPoints += val;
					CraftingMinigame.Instance.ElementalEffectEarthPoints = CurrentEarthPoints;
					break;
				case EMagicType.Water:
					CurrentWaterPoints += val;
					CraftingMinigame.Instance.ElementalEffectWaterPoints = CurrentWaterPoints;
					break;
				case EMagicType.Lightning:
					CurrentLightningPoints += val;
					CraftingMinigame.Instance.ElementalEffectLightningPoints = CurrentLightningPoints;
					break;
				case EMagicType.Explosive:
					CurrentExplosivePoints += val;
					CraftingMinigame.Instance.ElementalEffectExplosivePoints = CurrentExplosivePoints;
					break;
				case EMagicType.Shadow:
					CurrentShadowPoints += val;
					CraftingMinigame.Instance.ElementalEffectShadowPoints = CurrentShadowPoints;
					break;
				case EMagicType.Luminous:
					CurrentLuminousPoints += val;
					CraftingMinigame.Instance.ElementalEffectLuminousPoints = CurrentLuminousPoints;
					break;
			}
		}

		public HexCell GetHexCell(int row, int col)
		{
			HexCell retcell = null;

			//first set the indexer
			_gridIndexer.Row = row;
			_gridIndexer.Column = col;

			if (DataCells.ContainsKey(_gridIndexer))
				retcell = DataCells[_gridIndexer];
			return retcell;
		}
		
		/// <summary>
		/// This will add a cell to the current "grid"
		/// //This is here to set up the cell relationships.
		/// </summary>
		/// <param name="cellToAdd"></param>
		public void AddHexCell(HexCell cellToAdd)
		{
			int row = DataCells.Count / _maxHoriCells;
			int column = DataCells.Count % _maxHoriCells;

			if (row < _maxVertCells && column < _maxHoriCells)
			{
				//three cases
				if (row == 0) //Base row
				{
					if (column == 0)
					{
						//first node ever.

						//Finally lets set the position of the newly created cell to screen space.
						cellToAdd.SetPosition(0,0);

						DataCells.Add(new CellIndexerStruct(){Row = 0, Column= 0}, cellToAdd);
					}
					else
					{
						cellToAdd.SetWest(GetHexCell(0, column - 1));
						GetHexCell(0, column - 1).SetEast(cellToAdd);

						//Finally lets set the position of the newly created cell to screen space.
						cellToAdd.SetPosition(column * _cellWidth, 0);

						DataCells.Add(new CellIndexerStruct() { Row = 0, Column = column }, cellToAdd );
					}

				}
				else if (row % 2 == 1) //Odd number rows
				{

					//three states
					if (column == 0)
					{
						//Already existing
						GetHexCell(row - 1, (column)).SetSouthEast(cellToAdd);
						GetHexCell(row - 1, (column + 1)).SetSouthWest(cellToAdd);

						//new one
						cellToAdd.SetNorthWest(GetHexCell(row - 1, (column)));
						cellToAdd.SetNorthEast(GetHexCell(row - 1, column + 1));

						DataCells.Add(new CellIndexerStruct() { Row = row, Column = column }, cellToAdd);
					}
					else if (column == _maxHoriCells-1)
					{
						//Already existing
						GetHexCell(row, (column - 1)).SetEast(cellToAdd);
						GetHexCell(row - 1, (column)).SetSouthEast(cellToAdd);
						
						//new one
						cellToAdd.SetWest(GetHexCell(row, (column - 1)));
						cellToAdd.SetNorthWest(GetHexCell(row - 1, (column)));

						DataCells.Add(new CellIndexerStruct() { Row = row, Column = column }, cellToAdd);
					}
					else
					{
						//Already existing
						GetHexCell(row, (column - 1)).SetEast(cellToAdd);
						GetHexCell(row - 1, (column)).SetSouthEast(cellToAdd);
						GetHexCell(row - 1, column + 1).SetSouthWest(cellToAdd);

						//new one
						cellToAdd.SetWest(GetHexCell(row, (column - 1)));
						cellToAdd.SetNorthWest(GetHexCell(row - 1, (column)));
						cellToAdd.SetNorthEast(GetHexCell(row - 1, column + 1));

						DataCells.Add(new CellIndexerStruct() { Row = row, Column = column }, cellToAdd);
					}
					//Finally lets set the position of the newly created cell to screen space.
					if (row == 1)
						cellToAdd.SetPosition(column * _cellWidth + (_cellWidth/2), row * _cellHeight - (_cellHeight / 4));
					else
						cellToAdd.SetPosition(column * _cellWidth + (_cellWidth/2), row * _cellHeight - (_cellHeight / 4 * row));
				}
				else if (row % 2 == 0) //Even number rows.
				{

					//three states
					if (column == 0)
					{
						//Already existing
						GetHexCell(row - 1, (column)).SetSouthWest(cellToAdd);

						//new one
						cellToAdd.SetNorthEast(GetHexCell(row - 1, (column)));

						DataCells.Add(new CellIndexerStruct() { Row = row, Column = column }, cellToAdd);
					}
					else
					{
						//Already existing
						GetHexCell(row, (column - 1)).SetEast(cellToAdd);
						GetHexCell(row - 1, (column-1)).SetSouthEast(cellToAdd);
						GetHexCell(row - 1, column).SetSouthWest(cellToAdd);

						//new one
						cellToAdd.SetWest(GetHexCell(row, (column - 1)));
						cellToAdd.SetNorthWest(GetHexCell(row - 1, (column-1)));
						cellToAdd.SetNorthEast(GetHexCell(row - 1, column));

						DataCells.Add(new CellIndexerStruct() { Row = row, Column = column }, cellToAdd);
					}
					//Finally lets set the position of the newly created cell to screen space.
					if (row == 1)
						cellToAdd.SetPosition(column * _cellWidth , row * _cellHeight - (_cellHeight / 4));
					else
						cellToAdd.SetPosition(column * _cellWidth , row * _cellHeight - (_cellHeight / 4 * row));
				}
			}
			else
			{
				//don't add the max has been reached.
			}

			cellToAdd.SetRow(row);
			cellToAdd.SetColumn(column);

		}

		public void AddPuzzlePieceOnGrid(KeyValuePair<PuzzlePiece, PuzzlePiece> puzzlePiecePair, LinkedList<Tuple<PuzzlePieceHexCell, HexCell>> gridCells, Vector2 xySnapPos)
		{

			CraftingMinigame.DebugOutToConsole(String.Format("Adding Puzzle piece to the grid!"));
			//CurrentPuzzlePieces
			LinkedListNode<Tuple<PuzzlePieceHexCell, HexCell>> v = gridCells.First;

			CraftingMinigame.DebugOutToConsole(String.Format("\t  Looping through all original puzzle piece hex cells."));
			while (v != null)
			{
				CraftingMinigame.DebugOutToConsole(String.Format("\t\t  Linking the data, and grid cells of the original puzzle piece. Hex Cell = {1} from PP_Name ={0} == Grid Cell [{2}, {3}]",
					puzzlePiecePair.Key.Name, v.Value.Item1.GetGameTextbox().Text, v.Value.Item2.Row, v.Value.Item2.Column));
				
				// Link the Puzzle Piece cells to the Hex Grid cells so we can move/Remove pieces
				// Reactivate the original pieces on the recipe side of the screen!
				v.Value.Item2.LinkedDataCell = v.Value.Item1;
				v.Value.Item1.LinkedGridCell = v.Value.Item2;
				v.Value.Item2.FillColor = v.Value.Item1.FillColor;

				AddElementalConnection(v.Value.Item1.MagicType, v.Value.Item1.CellValue);
				_elementCellsCount.AddElementalValue(v.Value.Item1.MagicType);

				// We need to check All the connections of the cell we are going to add!
				// By this point the grid should have all the links it needs. So we can compare grid only.
				for (int i = 0; i < 6; i++)
				{
					if (v.Value.Item2.GetConnectionCell(i) != null && v.Value.Item2.GetConnectionCell(i).LinkedDataCell != null)
					{
						// Find out the connections magic type
						EMagicType connMagicType = v.Value.Item2.GetConnectionCell(i).LinkedDataCell.MagicType;
						
						// Is this the same as the current cells type?
						bool bIsPartOfToAddPiece = puzzlePiecePair.Key.InternalCells.Contains(v.Value.Item2.GetConnectionCell(i).LinkedDataCell);
						if (v.Value.Item2.LinkedDataCell.MagicType == connMagicType && !bIsPartOfToAddPiece)
						{
							_externalHomogenousConnectionCounts.AddElementalValue(connMagicType);
						}
						else if (v.Value.Item2.LinkedDataCell.MagicType != connMagicType && !bIsPartOfToAddPiece)
						{
							// Not the same so we need to add both for later stats.
							_externalNonHomogenousConnectionCounts.AddElementalValue(v.Value.Item2.LinkedDataCell.MagicType);
							_externalNonHomogenousConnectionCounts.AddElementalValue(connMagicType);
						}
					}
				}
				v = v.Next;
			}

			puzzlePiecePair.Key.bIsOnGrid = true;

			// Snap to grid
			puzzlePiecePair.Key.XPos = (int)xySnapPos.X;
			puzzlePiecePair.Key.YPos = (int)xySnapPos.Y;

			// Add the internal connection for stats!
			_internalPuzzlePieceConnectionCounts.AddExistingStruct(ref _internalPuzzlePieceConnectionCounts, puzzlePiecePair.Key.HomogenousConnectionCounts);

			// Update the stats!
			UpdateElementalPointValues();


			CurrentLinkedPuzzlepieces.Add(puzzlePiecePair.Key, puzzlePiecePair.Value);


		}

		public void RemovePuzzlePieceFromGrid(PuzzlePiece toRemove)
		{
			if (CurrentLinkedPuzzlepieces.ContainsKey(toRemove))
			{
				CurrentLinkedPuzzlepieces[toRemove].bIsSelectable = true;
				CurrentLinkedPuzzlepieces[toRemove].bIsActive = true;
				CurrentLinkedPuzzlepieces[toRemove].bIsOnGrid = false;
				CurrentLinkedPuzzlepieces[toRemove].bIsSelected = false;
				CurrentLinkedPuzzlepieces.Remove(toRemove);

			}
		}

		public bool OnMouseEvent(MouseState mouseState, MouseState prevMouseState, HexCell selectedGirdCell)
		{
			//make sure we are currently hovering over a grid.
			if (SelectedCell != null)
			{
				//Is there a puzzle cell that is linked to current grid cell.
				if (selectedGirdCell.LinkedDataCell != null)
				{
					//selectedGirdCell.SetTransparency(0.5f);

					if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
					{
						Console.WriteLine("Right Clicked on Piece on grid");

						// Are we holding a piece already?
						if (CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Value != null)
						{
							// Get out we need to check for correct placement location first
							CraftingMinigame.DebugOutToConsole("Clicked on the Grid, with a cloned piece, OVER a linked piece. So exit grid removal");
							return false;
						}

						//We need to find the piece this grid belongs to.
						PuzzlePiece parentPiece = selectedGirdCell.LinkedDataCell.GetPuzzlePieceParent();
						foreach (var v in parentPiece.InternalCells)
						{
							// We need to scan all connections from this cell to all other external cell.
							// Find the connections and then remove them
							for (int i = 0; i < 6; i++)
							{
								if (v.LinkedGridCell.GetConnectionCell(i) != null && v.LinkedGridCell.GetConnectionCell(i).LinkedDataCell != null)
								{
									// Find out the connections magic type
									EMagicType connMagicType = v.LinkedGridCell.GetConnectionCell(i).LinkedDataCell.MagicType;

									// Is this the same as the current cells type?
									bool bIsPartOfToAddPiece = parentPiece.InternalCells.Contains(v.LinkedGridCell.GetConnectionCell(i).LinkedDataCell);
									if (v.MagicType == connMagicType && !bIsPartOfToAddPiece)
									{
										_externalHomogenousConnectionCounts.AddElementalValue(connMagicType, -1);
									}
									else if (v.MagicType != connMagicType && !bIsPartOfToAddPiece)
									{
										// Not the same so we need to add both for later stats.
										_externalNonHomogenousConnectionCounts.AddElementalValue(v.MagicType, -1);
										_externalNonHomogenousConnectionCounts.AddElementalValue(connMagicType, -1);
									}
								}
							}

							v.LinkedGridCell.FillColor = Color.White;
							v.LinkedGridCell.LinkedDataCell = null;
							v.LinkedGridCell = null;
							_elementCellsCount.AddElementalValue(v.MagicType, -1);
						}
						parentPiece.SetTransparency(1.0f);
						parentPiece.bIsActive = true;
						parentPiece.bIsOnGrid = false;
						parentPiece.bIsSelectable = true;
						parentPiece.bIsSelected = false;
						

						// we need to use the current HexBoard puzzle piece ref to get and reset the original puzzle piece reference
						if (CurrentLinkedPuzzlepieces[parentPiece] != null)
						{
							CurrentLinkedPuzzlepieces[parentPiece].bIsOnGrid = false;
							CurrentLinkedPuzzlepieces[parentPiece].bIsSelectable = true;
							CurrentLinkedPuzzlepieces[parentPiece].bIsActive = true;
						}

						// Remove the internal puzzle piece connections to update the stats!
						_internalPuzzlePieceConnectionCounts.RemoveExistingStruct(ref _internalPuzzlePieceConnectionCounts, parentPiece.HomogenousConnectionCounts);

						// Update the Stats!
						UpdateElementalPointValues();

						CurrentLinkedPuzzlepieces.Remove(parentPiece);
						return true;
					}
					//we are clicking on a puzzle piece that is already on the hex grid.
					else if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
					{
						CraftingMinigame.DebugOutToConsole("Left Click on Piece on grid");

						// Are we holding a piece already?
						if (CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Value != null)
						{
							// Get out we need to check for correct placement location first
							CraftingMinigame.DebugOutToConsole("Clicked on the Grid, with a cloned piece, OVER a linked piece. So exit grid transfer");
							return false;
						}

						//We have clicked on a puzzle piece. So let's first get that reference
						PuzzlePiece parentPiece = selectedGirdCell.LinkedDataCell.GetPuzzlePieceParent();
						parentPiece.SelectedIndex =  parentPiece.InternalCells.IndexOf(selectedGirdCell.LinkedDataCell);

						//after getting that reference. Let's make a copy.
						PuzzlePiece toMinigame = new PuzzlePiece("SelectedPiece", parentPiece, 1.0f, 1.0f, spriteFont, parentPiece.InternalCells) {bIsActive =  true};

						//Use that copy to set the Minigame instance current cloned piece for mouse movement
						CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair = 
							new KeyValuePair<PuzzlePiece, PuzzlePiece>( toMinigame, CurrentLinkedPuzzlepieces[parentPiece]);
						
						CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.SetTransparency(1.0f);
						CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.bIsSelected = true;
						CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.bIsActive = true;
						CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.bIsSelectable = false;
						CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.bIsOnGrid = true;
						CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.RotationCount = parentPiece.RotationCount;
						CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.SetSelectedHexCellRef(parentPiece.SelectedHexCell);



						// snap to the new cell anchor
						if (SelectedCell.IsInside(Mouse.GetState().Position.ToVector2()))
						{
							HexCell hoveredGridCell = selectedGirdCell;

							Vector2 GridSnapPos = new Vector2(hoveredGridCell.XPos + hoveredGridCell.Width / 2, hoveredGridCell.YPos + hoveredGridCell.Height / 2);

							CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.XOffset = (int)(0 - (toMinigame.SelectedHexCell.XPos_Lossless + (toMinigame.SelectedHexCell.Width) / 2.0f) / toMinigame.ScaleX);
							CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.YOffset = (int)(0 - (toMinigame.SelectedHexCell.YPos_Lossless + (toMinigame.SelectedHexCell.Height) / 2.0f) / toMinigame.ScaleY);

							CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.XPos = (int)GridSnapPos.X;
							CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.YPos = (int)GridSnapPos.Y;
						}
						


						// Wipe all trace of the puzzle piece that placed
						foreach (var v in parentPiece.InternalCells)
						{

							// We need to scan all connections from this cell to all other external cell.
							// Find the connections and then remove them
							for (int i = 0; i < 6; i++)
							{
								if (v.LinkedGridCell.GetConnectionCell(i) != null && v.LinkedGridCell.GetConnectionCell(i).LinkedDataCell != null)
								{
									// Find out the connections magic type
									EMagicType connMagicType = v.LinkedGridCell.GetConnectionCell(i).LinkedDataCell.MagicType;

									// Is this the same as the current cells type?
									bool bIsPartOfToAddPiece = parentPiece.InternalCells.Contains(v.LinkedGridCell.GetConnectionCell(i).LinkedDataCell);
									if (v.MagicType == connMagicType && !bIsPartOfToAddPiece)
									{
										_externalHomogenousConnectionCounts.AddElementalValue(connMagicType, -1);
									}
									else if (v.MagicType != connMagicType && !bIsPartOfToAddPiece)
									{
										// Not the same so we need to add both for later stats.
										_externalNonHomogenousConnectionCounts.AddElementalValue(v.MagicType, -1);
										_externalNonHomogenousConnectionCounts.AddElementalValue(connMagicType, -1);
									}
								}
							}

							v.LinkedGridCell.FillColor = Color.White;
							v.LinkedGridCell.LinkedDataCell = null;
							v.LinkedGridCell = null;
							_elementCellsCount.AddElementalValue(v.MagicType, -1);

						}

						// Remove the internal puzzle piece connections to update the stats!
						_internalPuzzlePieceConnectionCounts.RemoveExistingStruct(ref _internalPuzzlePieceConnectionCounts, parentPiece.HomogenousConnectionCounts);

						// Update the Stas!
						UpdateElementalPointValues();

						CurrentLinkedPuzzlepieces.Remove(parentPiece);
						return true;
					}
				}
			}

			return false;
		}

		#endregion


		#region Mongame 

		public void Load()
		{

		}

		public void Unload()
		{

		}

		public void Update(GameTime gameTime)
		{
			foreach (HexCell cell in DataCells.Values)
			{
				cell.Update(gameTime);

				// Does this cell have a piece connected to it/ on it?
				if (cell.LinkedDataCell != null && cell.IsInside(Mouse.GetState().Position.ToVector2()))
				{
					cell.LinkedDataCell.SetTransparency(.25f);
				}
				else
				{
					cell.LinkedDataCell?.SetTransparency(1.0f);
				}

			}

			foreach (var gridPieces in CurrentLinkedPuzzlepieces.Keys)
			{
				// gridPieces.Update(gameTime);
			}

		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{

			spriteBatch.Begin();

			//First up draw the background cells. (the grid)
			foreach (HexCell cell in DataCells.Values)
				cell.Draw(spriteBatch, gameTime);

			//We don't need to draw the overlaid piece. Because the grid is already colored
			//foreach (var puzzlePiece in CurrentPuzzlePieces)
			//	puzzlePiece.Draw(gameTime, spriteBatch);

			foreach (var gridPieces in CurrentLinkedPuzzlepieces.Keys)
			{
				gridPieces.Draw(gameTime,spriteBatch);
			}

			spriteBatch.End();
		}


		#endregion

		#endregion


	}
}
