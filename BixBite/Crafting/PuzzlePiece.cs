
#define EE_DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using BixBite.Combat;
using BixBite.Crafting.HexGrid;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Crafting
{
	public partial class PuzzlePiece
	{

		public CraftingHelpers.ElementalPoints ElementalCellCounts_S = new CraftingHelpers.ElementalPoints();
		public CraftingHelpers.ElementalPoints HomogenousConnectionCounts = new CraftingHelpers.ElementalPoints();


		#region Delegates/Events

		#endregion

		#region Fields
		private bool _bIsActive = false;
		private bool _bIsSelectable = true;
		private bool _bIsOnGrid = false;
		private bool _isSelected = false;

		private int _pieceType = -1;

		private int _xpos = -1;
		private int _ypos = -1;

		private int _spawnpos_x = -1;
		private int _spawnpos_y = -1;

		private int _xoffset = 0;
		private int _yoffset = 0;

		private float _scaleX = -1f;
		private float _scaleY = -1f;

		private int _basewidth = -1;
		private int _baseheight = -1;
		private int _width = -1;
		private int _height = -1;
		private int _basecellWidth = -1;
		private int _cellWidth = -1;
		private int _basecellHeight = -1;
		private int _cellHeight = -1;

		private bool _bIsMixedPiece = false;
		private int _allowedMagicTypes = -1;

		private int _totalPuzzlePiecePower = -1;

		public int _selectedIndex = 0;

		private int Quality = -1;

		private int _roationCount = 0;

		private ERarityType RarityType = ERarityType.NONE;
		private ESize SizeType = ESize.NONE;
		private Rectangle _drawrectangle = new Rectangle();
		private Texture2D _fillTexture2D = null;
		private MouseState _prevMouseState = new MouseState();
		private Color _fillColor;
		private EMagicType _magicType;

		private SpriteFont spriteFont;

		#endregion

		#region Properties
		public String Name = "";

		//public InternalMagicTypeConnections MagicTypeConnectionsCount_Struct = new InternalMagicTypeConnections();
		//public ExternalMagicTypeConnections ExtMagicTypeConnectionsCount_Struct = new ExternalMagicTypeConnections();
		//public InternalTypeConnections ConnectionTypeCount_Struct = new InternalTypeConnections();

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

		public int XOffset
		{
			get => _xoffset;
			set => _xoffset = value;
		}
		public int YOffset
		{
			get => _yoffset;
			set => _yoffset = value;
		}

		public virtual float ScaleX
		{
			get => _scaleX;
			set => _scaleX = value;
		}

		public virtual float ScaleY
		{
			get => _scaleY;
			set => _scaleY = value;
		}


		public bool bIsActive
		{
			get=> _bIsActive;
			set => _bIsActive = value;
		}

		public bool bIsSelectable
		{
			get => _bIsSelectable;
			set => _bIsSelectable = value;
		}

		public bool bIsOnGrid
		{
			get => _bIsOnGrid;
			set => _bIsOnGrid = value;
		}

		public bool bIsSelected
		{
			get => _isSelected;
			set => _isSelected = value;
		}

		public int SelectedIndex
		{
			get => _selectedIndex;
			set => _selectedIndex = value;

		}

		public int RotationCount
		{
			get => _roationCount;
			set => _roationCount = value;

		}

		public Color FillColor
		{
			get => _fillColor;
			set => _fillColor = value;
		}

		public EMagicType MagicType
		{ get => _magicType;
			set => _magicType = value;
		}

		public int BaseCellWidth
		{
			get => _basecellWidth;
			set => _basecellWidth = value;
		}

		public int BaseCellHeight
		{
			get => _basecellHeight;
			set => _basecellHeight = value;
		}

		public int AllowedMagicTypes
		{
			get => _allowedMagicTypes;
			set => _allowedMagicTypes = value;
		}

		public int TotalPuzzlePiecePower
		{
			get => _totalPuzzlePiecePower;
			set => _totalPuzzlePiecePower = value;
		}

		public PuzzlePieceHexCell SelectedHexCell
		{
			get => InternalCells[SelectedIndex];
		}
		public List<PuzzlePieceHexCell> InternalCells = new List<PuzzlePieceHexCell>();

		public Rectangle DrawRectangle
		{
			get
			{
				_drawrectangle.X = _xpos + _xoffset;
				_drawrectangle.Y = _ypos + _yoffset;
				_drawrectangle.Width = (int)(_width * _scaleX);
				_drawrectangle.Height = (int)(_height * _scaleY);
				return _drawrectangle;
			}
			set => _drawrectangle = value;
		}

		#endregion

		#region Constructor


		/// <summary>
		/// This constructor will create a new puzzle piece, BUT copy the property data from the given piece.
		/// WITH RESCALING
		/// </summary>
		/// <param name="toCopy"></param>
		public PuzzlePiece(String name, PuzzlePiece toCopy, float sx, float sy, SpriteFont sf, List<PuzzlePieceHexCell> internalCells = null)
		{
			CraftingMinigame.DebugOutToConsole(String.Format("\t  Creating new Puzzle piece"));

			this.Name = name;

			this._xpos = toCopy._xpos;
			this._ypos = toCopy._ypos;

			this.XOffset = toCopy.XOffset;
			this.YOffset = toCopy.YOffset;

			this._spawnpos_x = toCopy._spawnpos_x;
			this._spawnpos_y = toCopy._spawnpos_y;

			this._scaleX = sx;
			this._scaleY = sy;

			this._width = (int)(toCopy._basewidth * sx);
			this._basewidth = toCopy._basewidth;
			this._height = (int)(toCopy._baseheight * sy);
			this._baseheight = toCopy._baseheight;

			this._cellWidth = (int)(toCopy._basecellWidth * sx);
			this._basecellWidth = toCopy._basecellWidth;
			this._cellHeight = (int)(toCopy._basecellHeight * sy);
			this._basecellHeight = toCopy._basecellHeight;

			this._magicType = toCopy._magicType;

			this._totalPuzzlePiecePower = toCopy._totalPuzzlePiecePower;
			this._allowedMagicTypes = toCopy._allowedMagicTypes;
			if (_allowedMagicTypes % 2 == 0) _bIsMixedPiece = false;
			else _bIsMixedPiece = true;

			this._pieceType = toCopy._pieceType;

			this._fillTexture2D = toCopy._fillTexture2D;
			this._fillColor = toCopy.FillColor;

			this._isSelected = toCopy._isSelected;
			//this.SelectedHexCell = toCopy.SelectedHexCell; //DOESN'T SET REFERENCE
			SetSelectedHexCellRef(toCopy.SelectedHexCell);

			this.spriteFont = toCopy.spriteFont;

			this._prevMouseState = toCopy._prevMouseState;

			if (internalCells == null)
			{
				//create the internal cells.
				CreatePuzzlePiece(_pieceType, toCopy._fillTexture2D, toCopy._magicType);
			}
			else
			{
				//This copies a new list... but the objects in the list are not copied. they are referenced. 

				//this.InternalCells = new List<PuzzlePieceHexCell>(internalCells);

				this._fillColor = Color.Gray;

				//SOOOO we need to copy those.
				foreach (PuzzlePieceHexCell toCopyCells in internalCells)
				{
					this.InternalCells.Add(new PuzzlePieceHexCell(this, toCopyCells)
					{
						FillColor = _fillColor,
						MagicType = toCopyCells.MagicType
						//XPos = toCopyCells.XPos,
						//YPos = toCopyCells.YPos,
						//YPos_Lossless = toCopyCells.YPos_Lossless,
						//XPos_Lossless = toCopyCells.XPos_Lossless,
						//DrawRectangle =  toCopyCells.DrawRectangle
					});
				}
				//Give it the created cells list. This will make it so it ONLY sets the logic connections. and doesn't rebuild them AGAIn
				//THIS ALSO allows the cloned piece to rotate!
				CreatePuzzlePiece(_pieceType, toCopy._fillTexture2D, toCopy._magicType, 100.0f, this.InternalCells);


			}
		}

		/// <summary>
		/// This constructor will create a new puzzle piece, BUT copy the property data from the given piece.
		/// </summary>
		/// <param name="toCopy"></param>
		public PuzzlePiece(PuzzlePiece toCopy, SpriteFont sf)
		{
			this._xpos = toCopy._xpos;
			this._ypos = toCopy._ypos;

			this.XOffset = toCopy.XOffset;
			this.YOffset = toCopy.YOffset;

			this._spawnpos_x = toCopy._spawnpos_x;
			this._spawnpos_y = toCopy._spawnpos_y;

			this._scaleX = toCopy._scaleX;
			this._scaleY = toCopy._scaleY;

			this._width = (int)(toCopy._basewidth * toCopy.ScaleX);
			this._basewidth = toCopy._basewidth;
			this._height = (int)(toCopy._baseheight * toCopy.ScaleY);
			this._baseheight = toCopy._baseheight;

			this._cellWidth = (int)(toCopy._basecellWidth * toCopy.ScaleX);
			this._basecellWidth = toCopy._basecellWidth;
			this._cellHeight = (int) (toCopy._basecellHeight * toCopy.ScaleY);
			this._basecellHeight = toCopy._basecellHeight;

			this._magicType = toCopy._magicType;

			this._totalPuzzlePiecePower = toCopy._totalPuzzlePiecePower;
			this._allowedMagicTypes = toCopy._allowedMagicTypes;
			if (_allowedMagicTypes % 2 == 0) _bIsMixedPiece = false;
			else _bIsMixedPiece = true;

			this._fillTexture2D = toCopy._fillTexture2D;
			this._pieceType = toCopy._pieceType;
			this._prevMouseState = toCopy._prevMouseState;
			this.spriteFont = toCopy.spriteFont;

			//create the internal cells.
			CreatePuzzlePiece(_pieceType, toCopy._fillTexture2D, toCopy._magicType);
		}

		/// <summary>
		/// Default creating brand new from from given parameters
		/// </summary>
		/// <param name="fillTexture"></param>
		/// <param name="xpos"></param>
		/// <param name="ypos"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="cellWidth"></param>
		/// <param name="cellHeight"></param>
		/// <param name="scaleX"></param>
		/// <param name="scaleY"></param>
		/// <param name="desiredPiece"></param>
		public PuzzlePiece(String name, Texture2D fillTexture, EMagicType desiredMagicType, float xpos, float ypos, int width, int height, int cellWidth, int cellHeight, float scaleX, float scaleY, int desiredPiece, int totalPowerPoints, int allowedMagicTypes, SpriteFont sf)
		{
			this.Name = name;

			this._xpos = (int)xpos;
			this._ypos = (int)ypos;

			this._spawnpos_x = (int)xpos;
			this._spawnpos_y = (int)ypos;

			this._scaleX = scaleX;
			this._scaleY = scaleY;

			this._width = (int)(width * scaleX);
			this._basewidth = width;
			this._height = (int)(height * scaleY);
			this._baseheight = height;

			this._basecellWidth = cellWidth;
			this._cellWidth = (int)(cellWidth * scaleX);
			this._basecellHeight = cellHeight;
			this._cellHeight = (int)(cellHeight * scaleY);

			this._magicType = desiredMagicType;

			this._totalPuzzlePiecePower = totalPowerPoints;
			this._allowedMagicTypes = allowedMagicTypes;
			if (_allowedMagicTypes % 2 == 0 || _allowedMagicTypes == 1) _bIsMixedPiece = false;
			else _bIsMixedPiece = true;

			this._pieceType = desiredPiece;
			this._fillTexture2D = fillTexture;
			this._fillColor = CraftingMinigame.GetMagicColor(desiredMagicType);
			this.spriteFont = sf;

			CreatePuzzlePiece(desiredPiece, fillTexture, desiredMagicType);

			//After we have created the internal piece cells then we assign them the values, and types.
			for (int i = 0; i < InternalCells.Count; i++)
			{
				if (!this._bIsMixedPiece)
				{
					InternalCells[i].MagicType = (EMagicType) this._allowedMagicTypes;
					InternalCells[i].CellValue = this._totalPuzzlePiecePower / InternalCells.Count;
				}
				else
				{
					//TODO: Logic for mixed piece creation!
				}

			}

		}

		#endregion

		#region Methods

		public void ResetToSpawnPosition()
		{
			_xpos = _spawnpos_x;
			_ypos = _spawnpos_y;
		}

		public void ResetPlacementFlag()
		{
			foreach (HexCell hc in InternalCells)
			{
				hc.bIsPlaced = false;
				for (int i = 0; i < 6; i++)
				{
					hc.GetConnection(i).bIsRotated = false;
				}
				
			}
		}

		public void SetTransparency(float f)
		{
			foreach (HexCell cell in InternalCells)
			{
				(cell as PuzzlePieceHexCell)?.SetTransparency(f);
			}
		}



		public void ResetRescalePiece(float x, float y, List<PuzzlePieceHexCell> internaList = null) 
		{
			this._scaleX = x;
			this._scaleY = y;

			this._width = (int)(_width * x);
			this._height = (int)(_height * y);

			this._cellWidth = (int)(_basecellWidth * x);
			this._cellHeight = (int)(_basecellHeight * y);


			if (internaList != null)
			{
				//this.InternalCells = internaList;


				this.InternalCells.ForEach( p => p.Rescale(x, y));
			}
			else
			{
				//Delete the cells.
				InternalCells.Clear();
				//Remake them
				CreatePuzzlePiece(_pieceType, _fillTexture2D, _magicType);
			}
		}

		public void RescalePiece(float x, float y)
		{
			this._scaleX = x;
			this._scaleY = y;

			this._width = (int)(_width * x);
			this._height = (int)(_height * y);

			this._cellWidth = (int)(_basecellWidth * x);
			this._cellHeight = (int)(_basecellHeight * y);



			//Delete the cells.
			//InternalCells.Clear();
			//Remake them
			//CreatePuzzlePiece(_pieceType, _fillTexture2D, _fillColor);

		}

		public void SetSelectedHexCellRef(PuzzlePieceHexCell puzzlePieceCellRef)
		{
			CraftingMinigame.DebugOutToConsole(String.Format("\t  Set the selected cell of the current puzzle piece."));

			SelectedIndex =  puzzlePieceCellRef.GetPuzzlePieceParent().InternalCells.FindIndex(x => x == puzzlePieceCellRef);
		}

		/// <summary>
		/// This is here for when you click on a puzzle piece CELL. BUT ONLY when its not selected.
		/// <para>only activates if currently selected, and currently cloned are NULL</para>
		/// </summary>
		/// <param name="reference">The current Cell that the player is hovering over.</param>
		/// <param name="mouseState"></param>
		public void OnHexCellEvent(PuzzlePieceHexCell reference, MouseState mouseState)
		{
			if (mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
			{
				//Logging
				CraftingMinigame.DebugOutToConsole(String.Format("Clicked on a puzzle piece!! STARTING EVENT CALLBACK. NAME = {0}", this.Name));

				CraftingMinigame.DebugOutToConsole(String.Format("\t Left click detected"));

				// So if the player is clicking left click. They are trying to pick up a piece. So we need to determine if the piece is on the grid first
				//this._xoffset = 0 - (reference.XPos + reference.Width / 2);
				//this._yoffset = 0 - (reference.YPos + reference.Height / 2);
				if (this.bIsOnGrid)
				{
					//What type of input is the play exhibiting as they hover over the hex cell.
					if (CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key == null)
					{
						CraftingMinigame.DebugOutToConsole(
							String.Format("On puzzle piece click ERROR!!! CurrentSelected Key pair is NULL!?!"));
						return;
					}


					CraftingMinigame.DebugOutToConsole(String.Format("\t The piece is already on the grid So reset it's active status to allow for pick up"));
					//If it's on the grid let's just use that reference in the crafting system to re allow movement and placement.
					//CraftingMinigame.Instance.CurrentlyClonedPuzzlePiece = this;
					//CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair 
					CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.bIsSelected = true;
					CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.bIsActive = true;
					CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.SetTransparency(1.0f);
					CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.bIsSelectable = false;
					CraftingMinigame.Instance.waitforpieceframes = 2;

					SetSelectedHexCellRef(reference);

					CraftingMinigame.DebugOutToConsole(String.Format("END OF ON CLICK PUZZLE PIECE CALLBACK. NAME = {0}", this.Name));
					return;

				}
				else
				{
					CraftingMinigame.DebugOutToConsole(String.Format("\t The puzzle piece is NOT on the grid. So we need to clone the puzzle piece for movement player movement"));

					//If it's not on the grid then we need to create a clone of the the current puzzle piece in question.
					SetSelectedHexCellRef(reference);
					this._bIsSelectable = false;
					this.SetTransparency(.25f);
					CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair = new KeyValuePair<PuzzlePiece, PuzzlePiece>(
						new PuzzlePiece(String.Format("{0}_{1}",this.Name, "copy"), this, 1.0f, 1.0f, spriteFont) {bIsActive = true}, reference.GetPuzzlePieceParent()); 
					CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.SetTransparency(1.0f);

					
					CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.XOffset = (int)(0 - (reference.XPos_Lossless + (reference.Width) / 2.0f) / this.ScaleX);
					CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.YOffset = (int)(0 - (reference.YPos_Lossless + (reference.Height) / 2.0f) / this.ScaleY);

					// This is a double check. Since we lost accuracy with scaling, this makes sure the offset is OFFSET MOD WIDTH  == 0
					//CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.XOffset += (int)((reference.Width / this.ScaleX) -
					//	CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.XOffset % (reference.Width / this.ScaleX));
					//CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.YOffset += (int)((reference.Height / this.ScaleY) -
					//	CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key.YOffset % (reference.Height / this.ScaleY));

					CraftingMinigame.DebugOutToConsole(String.Format("END OF ON CLICK PUZZLE PIECE CALLBACK. NAME = {0}", this.Name));
					return;

				}

				CraftingMinigame.DebugOutToConsole(String.Format("END OF ON CLICK PUZZLE PIECE CALLBACK. NAME = {0}", this.Name));
			}
			else if (mouseState.RightButton == ButtonState.Pressed && _prevMouseState.RightButton == ButtonState.Released)
			{
				//Reset the piece.

				//Logging
				CraftingMinigame.DebugOutToConsole(String.Format("Clicked on a puzzle piece!! STARTING EVENT CALLBACK. NAME = {0}", this.Name));

				CraftingMinigame.DebugOutToConsole(String.Format("\t Right click detected"));
				if (this.bIsOnGrid)
				{
				}

				else
				{


				}


				this.XPos = _spawnpos_x;
				this.YPos = _spawnpos_y;
				this._bIsOnGrid = false;
				this._bIsSelectable = true;
				this._bIsActive = true;

				CraftingMinigame.DebugOutToConsole(String.Format("END OF ON CLICK PUZZLE PIECE CALLBACK. NAME = {0}", this.Name));
			}
			else
			{
				//This is just hovering over the cells.
				if (reference.Row >= 0 && reference.Column >= 0)
				{
					CraftingMinigame.DebugOutToConsole(String.Format("Hovering over grid cell [{0},{1}]", reference.Row, reference.Column));
				}
			}


			//			//Make sure it is only clicked ONCE
			//			if (mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
			//			{
			//				Console.WriteLine("Clicked in a puzzle piece");

			//				SetSelectedHexCellRef(reference);

			//				//this._xoffset = 0 - (reference.XPos + reference.Width/2);
			//				//this._yoffset = 0 - (reference.YPos + reference.Height/2);

			//				if (this._bIsOnGrid)
			//				{

			//					bIsActive = false;
			//					CraftingMinigame.Instance.RemovePuzzlePieceFromGrid(this);
			//				}
			//				else
			//				{
			//					this._bIsSelectable = false;
			//					this.SetTransparency(.25f);
			//				}

			//				this.bIsSelected = true;

			//				//We are picking this piece back up so we must remove the connection data.
			//				//if (this._bIsOnGrid)


			//				//Create a copy for the player to manipulate and keep the original intact. 
			//				CraftingMinigame.Instance.CurrentSelectedPuzzlePiece = this;

			//				if (!this._bIsOnGrid)
			//					CraftingMinigame.Instance.CurrentlyClonedPuzzlePiece = new PuzzlePiece("SelectedPiece",this, 1.0f, 1.0f, spriteFont) {bIsActive =  true};
			//				else
			//					CraftingMinigame.Instance.CurrentlyClonedPuzzlePiece = new PuzzlePiece("SelectedPiece", this, 1.0f, 1.0f, spriteFont, InternalCells) { bIsActive = true };
			//				CraftingMinigame.Instance.CurrentlyClonedPuzzlePiece.XOffset = (int)(0 - (reference.XPos_Lossless + (reference.Width) / 2.0f) / this.ScaleX);
			//				CraftingMinigame.Instance.CurrentlyClonedPuzzlePiece.YOffset = (int)(0 - (reference.YPos_Lossless + (reference.Height) / 2.0f) / this.ScaleY);

			//				CraftingMinigame.Instance.waitforpieceframes = 2;

			//			}

			//			if (mouseState.RightButton == ButtonState.Pressed && _prevMouseState.RightButton == ButtonState.Released)
			//			{
			//				//Reset the piece.
			//				//CraftingMinigame.Instance.CurrentlyClonedPuzzlePiece = this;

			//				//this.RescalePiece(.5f, .5f);  //this breaks linked data to require redundant parent checking...
			//				// now that i think about it... this is probably here, because before base scaling was added into fields it would stay X size and never default.


			//				this.XPos = _spawnpos_x;
			//				this.YPos = _spawnpos_y;
			//				this._bIsOnGrid = false;
			//				this._bIsSelectable = true;
			//				this._bIsActive = true;
			//			}
			//#if EE_DEBUG
			//			if (mouseState.MiddleButton == ButtonState.Pressed)
			//			{
			//				//CraftingMinigame.Instance.CurrentlyClonedPuzzlePiece = this;
			//			}
			//#endif



		}

		public void RotatePiece()
		{
			LinkedList<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>> RotationLayerLL = new LinkedList<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>>();
			PuzzlePieceHexCell AnchorCell = this.SelectedHexCell;
			RotationLayerLL.AddLast(new CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>()
			{
				PuzzlePieceHexCell = AnchorCell,
				ConnDirection = 0
			});

			Console.WriteLine("*************Starting Rotate piece request algo*************");

			LinkedList<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>> NextRotationLayerLL = new LinkedList<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>>();
			LinkedListNode<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>> currentLinkedListNode;
			AnchorCell.bIsPlaced = true; // Doesn't allow the anchor to mess up connection scanning.
			Console.WriteLine("Anchor point is hex cell: [{0}]", AnchorCell.GetGameTextbox().Text);
			do
			{
				currentLinkedListNode = RotationLayerLL.First;
				//First we need to Scan every Single cell in the current layer.
				while (currentLinkedListNode != null)
				{
					//CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int> currentCellPair = RotationLayerLL.First.Value;


					for (int i = 0; i < 6; i++)
					{
						// is there a valid cell we are connected to? AND if so has the connection to the cell not been rotated yet
						if (currentLinkedListNode.Value.PuzzlePieceHexCell.GetConnection(i).ChildCell != null &&
								!currentLinkedListNode.Value.PuzzlePieceHexCell.GetConnection(i).ChildCell.bIsPlaced
						)
						{
							//DO NOT INSERT cells of the SAME layer in here.
							if (Array.FindIndex(RotationLayerLL.ToArray(), x => x.PuzzlePieceHexCell == currentLinkedListNode.Value.PuzzlePieceHexCell.GetConnection(i).ChildCell) != -1)
								continue;
							NextRotationLayerLL.AddLast(new CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>()
							{
								PuzzlePieceHexCell = (PuzzlePieceHexCell)currentLinkedListNode.Value.PuzzlePieceHexCell.GetConnection(i).ChildCell,
								ConnDirection = i
							});
							Console.WriteLine("Queueing Next layer hex cell. {0} Parent Connection {1}",
								((PuzzlePieceHexCell)currentLinkedListNode.Value.PuzzlePieceHexCell.GetConnection(i).ChildCell).GetGameTextbox().Text
								, i);

							//Make sure to not RE ADD this to the next list.
							currentLinkedListNode.Value.PuzzlePieceHexCell.GetConnection(i).ChildCell.bIsPlaced = true;
						}
					}
					currentLinkedListNode = currentLinkedListNode.Next;
				}

				//Make sure to not try to rotate the anchor... it can't rotate around itself
				currentLinkedListNode = RotationLayerLL.First;
				if (RotationLayerLL.Count >= 1 && RotationLayerLL.First.Value.PuzzlePieceHexCell != AnchorCell)
				{
					//Now that we have all the data for the next layer, we need to rotate the current layer
					while (currentLinkedListNode != null)
					{
						PuzzlePieceHexCell CurrentHexCell = currentLinkedListNode.Value.PuzzlePieceHexCell;
						int ConnectionDirectionWithOffset = (currentLinkedListNode.Value.ConnDirection + this._roationCount) % 6;
						int ConnectionDirection = currentLinkedListNode.Value.ConnDirection;

						HexCell.ApplyDisplayRotationMath((PuzzlePieceHexCell)CurrentHexCell.GetConnection(HexCell.GetCardinalDirectionOpposite(ConnectionDirection)).ChildCell,
							CurrentHexCell, ConnectionDirectionWithOffset, true, false);

						//CurrentHexCell.GetConnection(HexCell.GetCardinalDirectionOpposite(ConnectionDirection)).bIsRotated = true;            // Handled already in the rotation math function above.
						//CurrentHexCell.bIsPlaced = true; // Handled already in the rotation math function above.


						RotationLayerLL.RemoveFirst(); //Remove the rotated piece
						currentLinkedListNode = RotationLayerLL.First;

					}
				}
				else RotationLayerLL.RemoveFirst(); //Remove the anchor cell

				RotationLayerLL = new LinkedList<CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int>>(NextRotationLayerLL);
				NextRotationLayerLL.Clear();
				Console.WriteLine("Removing Current Layer.");

			} while (RotationLayerLL.Count != 0);
			Console.WriteLine("*************Done Rotating piece request algo*************");


			/*
			Console.WriteLine("*************Starting Rotate piece request algo*************");
			Console.WriteLine("Anchor point is hex cell: [{0}]", this.SelectedHexCell.GetGameTextbox().Text);

			List<PuzzlePieceHexCell> currentList = new List<PuzzlePieceHexCell>();
			currentList.Add(this.SelectedHexCell); //make the current selected piece the anchor cell.
			List<int> ConnectionDirections = new List<int>();
			this.SelectedHexCell.bIsPlaced = true;
			//we need to recalcuate the x,y positions for the current rotation of this puzzle piece.
			
			for (int a = 0; a < currentList.Count; a++)
			{
				PuzzlePieceHexCell hcell = currentList[a];
				//CraftingHelpers.RotationQueuePair<PuzzlePieceHexCell, int> currentCellPair = RotationLayerLL.First.Value;
				List<PuzzlePieceHexCell> NextRotationLayerList = new List<PuzzlePieceHexCell>();

				for (int i = 0; i < 6; i++)
				{
					// is there a valid cell we are connected to? AND if so has the connection to the cell not been rotated yet
					if (hcell.GetConnection(i).ChildCell != null && !hcell.GetConnection(i).ChildCell.bIsPlaced)
					{
						//DO NOT INSERT cells of the SAME layer in here.
						if (Array.FindIndex(currentList.ToArray(), x => x == hcell.GetConnection(i).ChildCell) != -1)
							continue;
						NextRotationLayerList.Add((PuzzlePieceHexCell)hcell.GetConnection(i).ChildCell);
						ConnectionDirections.Add(i);
						Console.WriteLine("Queueing Next layer hex cell. {0} Parent Connection {1}", ((PuzzlePieceHexCell)hcell.GetConnection(i).ChildCell).GetGameTextbox().Text, i);
						hcell.GetConnection(i).ChildCell.bIsPlaced = true;
					}
				}
				if (NextRotationLayerList.Count == 0)
					currentList.Clear();  //There isn't a new rotation layer to process so... we done.
				else
				{
					//Make sure you don't rotate the ANCHOR.
					if (currentList.Count == 1 && currentList.First() == this.SelectedHexCell)
					{
						//This is your anchor cell, so don't process
					}
					else
					{
						for (int i = 0; i < currentList.Count; i++)
						{
							PuzzlePieceHexCell hexCell = currentList[i];
							int rotationDirection = (this.RotationCount + ConnectionDirections[i]) % 6;

							HexCell.ApplyDisplayRotationMath(hexCell, (PuzzlePieceHexCell) hexCell.GetConnectionCell(HexCell.GetCardinalDirectionOpposite(rotationDirection)),
								HexCell.GetCardinalDirectionOpposite(rotationDirection), true, false  );
						}
					}

					//At the very end make sure we set up the NEW rotation layer for processing
					currentList = new List<PuzzlePieceHexCell>(NextRotationLayerList);
					ConnectionDirections.Clear();
					a = 0;
				}
			}
			*/
		}

		#region Mongame.

		public void Load()
		{

		}

		public void Unload()
		{

		}

		public void Update(GameTime gameTime)
		{
			if(!bIsActive || (!bIsSelectable & !bIsOnGrid )) return;

			//This is here to allow for onHover work
			for (int i = 0; i < InternalCells.Count; i++)
			{
				InternalCells[i].Update(gameTime);
			}
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!bIsActive) return;

			//This is how we draw the cells on the boards
			foreach (HexCell cell in InternalCells)
			{
				cell.Draw(spriteBatch, gameTime);
			}
		}

		#endregion

		#endregion
	}
}
