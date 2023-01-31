using System;
using BixBite.Rendering.UI.TextBlock;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Crafting.HexGrid
{

	public class HexCellConnection
	{
		public HexCell ParentCell;
		public HexCell ChildCell;

		public int ConnectionDirection = -1;

		/// <summary>
		/// base connection point value.
		/// </summary>
		public int CorruptionPoints;

		/// <summary>
		/// Used to increase the weighted connection.
		/// this is used for when the player breaks connections. the father they place them away the more this goes up etc.
		/// </summary>
		public float CorruptionMultiplier;
		public bool bIsRotated = false;

		public HexCellConnection(HexCell parent, HexCell child, int connectionDirection)
		{
			ParentCell = parent;
			ChildCell = child;
			ConnectionDirection = connectionDirection;
			CorruptionPoints = 1;
			CorruptionMultiplier = 0.0f;
			bIsRotated = false; //just in case.
		}

		public HexCellConnection(HexCell parent, HexCell child, int connectionDirection, int corruptionPoints)
		{
			ParentCell = parent;
			ChildCell = child;
			ConnectionDirection = connectionDirection;
			CorruptionPoints = corruptionPoints;
			CorruptionMultiplier = 0.0f;
			bIsRotated = false; //just in case.
		}

	}

	/// <summary>
	/// This class is here to denote the internal "cells" in a HexGrid
	/// The cells all can connect to one another as long as they are adjacent in the E,W,NE,NW,SE,SW cardinal directions.
	/// You can also store anything with these cells data wise, all be it primitives or objects themselves.
	/// <para></para>
	/// You should think of this class as the OUTLINE of the grid. AKA the Empty cells on the grid itself, BUT it does have access to the actual data linked data of the cells
	///
	/// </summary>
	public class HexCell
	{
		#region Delegates/Events

		#endregion

		#region Fields

		private bool _bIsplaced = false;

		private int? _row = -1;
		private int? _column = -1;

		private PuzzlePieceHexCell _linkedDataCell = null;

		protected int _xpos = -1;
		protected int _spawn_xpos = -1;
		protected int _ypos = -1;
		protected int _spawn_ypos = -1;

		protected float _xpos_lossless = -1;
		protected float _ypos_lossless = -1;

		protected int _width = -1;
		protected int _height = -1;

		protected float _transparency = 1f;

		protected HexCellConnection[] _connections = new HexCellConnection[6];

		protected Color _fillColor = Color.White;

		protected Color _neEdgefillColor = Color.Black;
		protected Color _eEdgefillColor = Color.Black;
		protected Color _seEdgefillColor = Color.Black;

		protected Color _swEdgefillColor = Color.Black;
		protected Color _wEdgefillColor = Color.Black;
		protected Color _nwEdgefillColor = Color.Black;

		protected Rectangle _drawrectangle = new Rectangle();
		protected Texture2D CellImageFill = null;
		protected Texture2D CellImageOutline = null;

		protected SpriteFont _spriteFont;

		private HexGrid2D _parentgrid = null;

		#endregion

		#region Properties

		public Color BaseFillColor
		{
			get => _fillColor;
			set => _fillColor = value;
		}

		public Color FillColor
		{
			get => _fillColor;
			set => _fillColor = value;
		}

		public object Data = null;

		public bool bIsPlaced
		{
			get => _bIsplaced;
			set => _bIsplaced = value;
		}

		public virtual int Spawn_XPos
		{
			get => _spawn_xpos + _parentgrid.XPos;
			set => _spawn_xpos = value;
		}

		public virtual int Spawn_YPos
		{
			get => _spawn_ypos + _parentgrid.YPos;
			set => _spawn_ypos = value;
		}


		public virtual int XPos
		{
			get => _xpos + _parentgrid.XPos;
			set => _xpos = value;
		}

		public virtual int YPos
		{
			get => _ypos + _parentgrid.YPos;
			set => _ypos = value;
		}

		public float XPos_Lossless
		{
			get => _xpos_lossless;
			set => _xpos_lossless = value;
		}
		public float YPos_Lossless
		{
			get => _ypos_lossless;
			set => _ypos_lossless = value;
		}

		public virtual int Width => _width;
		public virtual int Height => _height;

		public int? Row => _row;
		public int? Column => _column;

		public PuzzlePieceHexCell LinkedDataCell
		{
			get => _linkedDataCell;
			set => _linkedDataCell = value;
		}

		public HexCell NorthEastCell => _connections[0].ChildCell;
		public HexCell EastCell => _connections[1].ChildCell;
		public HexCell SouthEastCell => _connections[2].ChildCell;
		public HexCell SouthWestCell => _connections[3].ChildCell;
		public HexCell WestCell => _connections[4].ChildCell;
		public HexCell NorthWestCell => _connections[5].ChildCell;

		protected Rectangle _NorthEastEdge_Texture = new Rectangle(30, 0, 30, 16 );
		protected Rectangle _EastEdge_Texture = new Rectangle(30, 16, 30, 28 );
		protected Rectangle _SouthEastEdge_Texture = new Rectangle(30, 44, 30, 16 );

		protected Rectangle _SouthWestEdge_Texture = new Rectangle(0, 44, 30, 16 );
		protected Rectangle _WestEdge_Texture = new Rectangle(0, 16, 30, 28 );
		protected Rectangle _NorthWestEdge_Texture = new Rectangle(0, 0, 30, 16 );

		protected GameTextBlock CellValueNUm_GTB;
		protected bool _debug = false;

		public virtual Rectangle DrawRectangle
		{
			get
			{
				_drawrectangle.X = _xpos + _parentgrid.DrawRectangle.X;
				_drawrectangle.Y = _ypos + _parentgrid.DrawRectangle.Y;
				_drawrectangle.Width = _width;
				_drawrectangle.Height = _height;
				return _drawrectangle;
			}
			set => _drawrectangle = value;
		}

		#region Hexgon Crop Retangles

		

		public virtual Rectangle NorthEastEdge_Texture
		{
			get
			{
				_drawrectangle.X = _xpos + _parentgrid.DrawRectangle.X + _NorthEastEdge_Texture.X;
				_drawrectangle.Y = _ypos + _parentgrid.DrawRectangle.Y + _NorthEastEdge_Texture.Y;
				_drawrectangle.Width = _NorthEastEdge_Texture.Width;
				_drawrectangle.Height = _NorthEastEdge_Texture.Height;
				return _drawrectangle;
			}
			set => _drawrectangle = value;
		}

		public virtual Rectangle EastEdge_Texture
		{
			get
			{
				_drawrectangle.X = _xpos + _parentgrid.DrawRectangle.X + _EastEdge_Texture.X;
				_drawrectangle.Y = _ypos + _parentgrid.DrawRectangle.Y + _EastEdge_Texture.Y;
				_drawrectangle.Width = _EastEdge_Texture.Width;
				_drawrectangle.Height = _EastEdge_Texture.Height;
				return _drawrectangle;
			}
			set => _drawrectangle = value;
		}

		public virtual Rectangle NorthWestEdge_Texture
		{
			get
			{
				_drawrectangle.X = _xpos + _parentgrid.DrawRectangle.X + _NorthWestEdge_Texture.X;
				_drawrectangle.Y = _ypos + _parentgrid.DrawRectangle.Y + _NorthWestEdge_Texture.Y;
				_drawrectangle.Width = _NorthWestEdge_Texture.Width;
				_drawrectangle.Height = _NorthWestEdge_Texture.Height;
				return _drawrectangle;
			}
			set => _drawrectangle = value;
		}

		public virtual Rectangle SouthWestEdge_Texture
		{
			get
			{
				_drawrectangle.X = _xpos + _parentgrid.DrawRectangle.X + _SouthWestEdge_Texture.X;
				_drawrectangle.Y = _ypos + _parentgrid.DrawRectangle.Y + _SouthWestEdge_Texture.Y;
				_drawrectangle.Width = _SouthWestEdge_Texture.Width;
				_drawrectangle.Height = _SouthWestEdge_Texture.Height;
				return _drawrectangle;
			}
			set => _drawrectangle = value;
		}

		public virtual Rectangle WestEdge_Texture
		{
			get
			{
				_drawrectangle.X = _xpos + _parentgrid.DrawRectangle.X + _WestEdge_Texture.X;
				_drawrectangle.Y = _ypos + _parentgrid.DrawRectangle.Y + _WestEdge_Texture.Y;
				_drawrectangle.Width = _WestEdge_Texture.Width;
				_drawrectangle.Height = _WestEdge_Texture.Height;
				return _drawrectangle;
			}
			set => _drawrectangle = value;
		}

		public virtual Rectangle SouthEastEdge_Texture
		{
			get
			{
				_drawrectangle.X = _xpos + _parentgrid.DrawRectangle.X + _SouthEastEdge_Texture.X;
				_drawrectangle.Y = _ypos + _parentgrid.DrawRectangle.Y + _SouthEastEdge_Texture.Y;
				_drawrectangle.Width = _SouthEastEdge_Texture.Width;
				_drawrectangle.Height = _SouthEastEdge_Texture.Height;
				return _drawrectangle;
			}
			set => _drawrectangle = value;
		}

		#endregion


		#endregion

		#region Constructor

		public HexCell(object parentRef, int numVal, float x, float y, int cellW, int cellH, int? row, int? column, object data, Texture2D cellFillImage, HexGrid2D parentgrid, Texture2D cellImageOutline, SpriteFont sf)
		{
			_xpos = (int)x;
			_ypos = (int)y;

			_spawn_xpos = (int)x;
			_spawn_ypos = (int)y;

			_xpos_lossless = x;
			_ypos_lossless = y;

			_width = cellW;
			_height = cellH;

			_row = row;
			_column = column;
			_parentgrid = parentRef as HexGrid2D;

			Data = data;
			CellImageOutline = cellImageOutline;
			CellImageFill = cellFillImage;
			_spriteFont = sf;
			_parentgrid = parentgrid;

			for (int i = 0; i < 6; i++)
			{
				_connections[i] = new HexCellConnection(this, null, i);
			}


			if (_parentgrid is HexGrid2D)
			{
				CellValueNUm_GTB = new GameTextBlock("NumVal", _spawn_xpos + _parentgrid.XPos, _spawn_ypos + _parentgrid.YPos, _width, _height, 1, false,
					(_width / 2), (_height / 2), numVal.ToString(), 0.0f, "#00000000", "", sf, null, Color.Gray);
				CellValueNUm_GTB.bIsActive = true;
			}
		}

		public HexCell(object parentRef, int numVal, float x, float y, int cellW, int cellH, int? row, int? column, object data, Texture2D cellFillImage, Texture2D cellImageOutline, SpriteFont sf)
		{
			_xpos = (int)x;
			_ypos = (int)y;

			_spawn_xpos = (int)x;
			_spawn_ypos = (int)y;

			_xpos_lossless = x;
			_ypos_lossless = y;

			_width = cellW;
			_height = cellH;

			_row = row;
			_column = column;
			_parentgrid = parentRef as HexGrid2D;

			Data = data;
			CellImageOutline = cellImageOutline;
			CellImageFill = cellFillImage;
			_spriteFont = sf;

			for (int i = 0; i < 6; i++)
			{
				_connections[i] = new HexCellConnection(this, null, i);
			}


			if (_parentgrid is HexGrid2D)
			{
				CellValueNUm_GTB = new GameTextBlock("NumVal", _spawn_xpos + _parentgrid.XPos, _spawn_ypos + _parentgrid.YPos, _width, _height, 1, false,
					(_width / 2), (_height / 2), numVal.ToString(), 0.0f, "#00000000", "", sf, null, Color.Gray);
				CellValueNUm_GTB.bIsActive = true;
			}
		}
		#endregion

		#region Methods

		#region Setters

		public void SetRow(int row)
		{
			_row = row;
		}

		public void SetColumn(int col)
		{
			_column = col;
		}

		public void SetPosition(int x, int y)
		{
			_xpos = x;
			CellValueNUm_GTB.XPos = x + _parentgrid.XPos;

			_ypos = y;
			CellValueNUm_GTB.YPos = y + _parentgrid.YPos;
		}

		public void SetNorthEast(HexCell cellRef, bool bSetRotFlag = false)
		{
			_connections[0].ParentCell = this;
			_connections[0].ChildCell = cellRef;
			_connections[0].ConnectionDirection = 0;

			_connections[0].bIsRotated = bSetRotFlag;
			this.bIsPlaced = bSetRotFlag;

		}
		public void SetEast(HexCell cellRef, bool bSetRotFlag = false)
		{
			_connections[1].ParentCell = this;
			_connections[1].ChildCell = cellRef;
			_connections[1].ConnectionDirection = 1;

			_connections[1].bIsRotated = bSetRotFlag;
			this.bIsPlaced = bSetRotFlag;
		}
		public void SetSouthEast(HexCell cellRef, bool bSetRotFlag = false)
		{
			_connections[2].ParentCell = this;
			_connections[2].ChildCell = cellRef;
			_connections[2].ConnectionDirection = 2;

			_connections[2].bIsRotated = bSetRotFlag;
			this.bIsPlaced = bSetRotFlag;
		}
		public void SetSouthWest(HexCell cellRef, bool bSetRotFlag = false)
		{
			_connections[3].ParentCell = this;
			_connections[3].ChildCell = cellRef;
			_connections[3].ConnectionDirection = 3;

			_connections[3].bIsRotated = bSetRotFlag;
			this.bIsPlaced = bSetRotFlag;
		}
		public void SetWest(HexCell cellRef, bool bSetRotFlag = false)
		{
			_connections[4].ParentCell = this;
			_connections[4].ChildCell = cellRef;
			_connections[4].ConnectionDirection = 4;

			_connections[4].bIsRotated = bSetRotFlag;
			this.bIsPlaced = bSetRotFlag;
		}
		public void SetNorthWest(HexCell cellRef, bool bSetRotFlag = false)
		{
			_connections[5].ParentCell = this;
			_connections[5].ChildCell = cellRef;
			_connections[5].ConnectionDirection = 5;

			_connections[5].bIsRotated = bSetRotFlag;
			this.bIsPlaced = bSetRotFlag;
		}

		public void SetLinkedHexCellRef(ref PuzzlePieceHexCell linkedCell)
		{
			this._linkedDataCell = linkedCell;
		}

		#endregion

		/// <summary>
		/// This is here to get the connection of a carndinal direction from 0 -> 5 Clockwise
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public virtual HexCellConnection GetConnection(int i)
		{
			if (i >= 0 && i <= 5)
				return _connections[i];
			return null;
		}

		/// <summary>
		/// This is here to get the connection of a carndinal direction from 0 -> 5 Clockwise
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public virtual HexCell GetConnectionCell(int i)
		{
			if (i >= 0 && i <= 5)
				return _connections[i].ChildCell;
			return null;
		}

		public virtual void ResetColorEdges()
		{
			_neEdgefillColor = Color.Black;

			_eEdgefillColor = Color.Black;

			_seEdgefillColor = Color.Black;

			_swEdgefillColor = Color.Black;

			_wEdgefillColor = Color.Black;

			_nwEdgefillColor = Color.Black;

		}

		public virtual void ColorConnectionEdge(int i, Color c)
		{
			switch (i)
			{
				case 0:
					_neEdgefillColor = c;
					NorthEastCell._swEdgefillColor= c;
					break;
				case 1:
					_eEdgefillColor = c;
					EastCell._wEdgefillColor = c;
					break;
				case 2:
					_seEdgefillColor = c;
					SouthEastCell._nwEdgefillColor = c;
					break;
				case 3:
					_swEdgefillColor = c;
					SouthWestCell._neEdgefillColor = c;
					break;
				case 4:
					_wEdgefillColor = c;
					WestCell._eEdgefillColor = c;
					break;
				case 5:
					_nwEdgefillColor = c;
					NorthWestCell._seEdgefillColor = c;
					break;
			}
		}

		public virtual bool IsInside(Vector2 posToTest)
		{
			float _hori = _width / 2;
			float _vert = _height / 4;

			float q2x = Math.Abs(posToTest.X - (_xpos + _parentgrid.XPos + (_width/2)));         // transform the test point locally and to quadrant 2
			float q2y = Math.Abs(posToTest.Y - (_ypos + _parentgrid.YPos + (_height / 2)));         // transform the test point locally and to quadrant 2
			if (q2x > _hori || q2y > _vert*2) return false;           // bounding test (since q2 is in quadrant 2 only 2 tests are needed)
			return 2 * _vert* _hori - _vert* q2x - _hori* q2y >= 0;   // finally the dot product can be reduced to this due to the hexagon symmetry
		}

		///// <summary>
		///// Check to see if this hex cell is connected to given hex cell in question
		///// </summary>
		//public virtual bool ConnectionCheck(HexCell toCheckCell)
		//{
		//	//if (this == toCheckCell) return true; 
		//	if (_connections.Contains(toCheckCell))
		//		return true;
		//	return false;
		//}

		public virtual HexCellConnection GetConnectionIfExists(HexCell toCheckCell)
		{
			foreach (HexCellConnection conn in _connections)
			{
				if (conn.ChildCell == toCheckCell)
					return conn;
			}
			return null;
		}

		public static int GetCardinalDirectionOpposite(int dir)
		{
			switch (dir)
			{
				case 0:
					return 3;
				case 1:
					return 4;
				case 2:
					return 5;
				case 3:
					return 0;
				case 4:
					return 1;
				case 5:
					return 2;
			}
			return -1;
		}

		public static string DirectionToString(int dir)
		{
			String retS = "";
			switch (dir)
			{
				case 0:
					retS = "NE";
					break;
				case 1:
					retS = "E";
					break;
				case 2:
					retS = "SE";
					break;
				case 3:
					retS = "SW";
					break;
				case 4:
					retS = "W";
					break;
				case 5:
					retS = "NW";
					break;
			}
			return retS;
		}

		public void SetTransparency(float t)
		{
			_transparency = t;
		}

		public static void ApplyDisplayRotationMath(PuzzlePieceHexCell parentCell, PuzzlePieceHexCell ToRotateCell, int currentConnection, bool bClockWise, bool bClearFlag = true)
		{
			//Is this an anchor?
			if (currentConnection == -1) return;

			Console.WriteLine(String.Format("Rotating cell {0} around cell {1}, connection {2} ", ToRotateCell.GetGameTextbox().Text, parentCell.GetGameTextbox().Text, currentConnection));

			if (bClockWise)
			{
				switch (currentConnection)
				{
					case 0: // NE -> E
						ToRotateCell.XPos = parentCell.XPos + parentCell.Width;
						ToRotateCell.YPos = parentCell.YPos;
						break;
					case 1: // E -> SE

						ToRotateCell.XPos = parentCell.XPos + (int)(parentCell.Width * 0.5f);
						ToRotateCell.YPos = parentCell.YPos + (int)(parentCell.Height * 0.75f);
						break;
					case 2: // SE -> SW
						ToRotateCell.XPos = parentCell.XPos - (int)(parentCell.Width * 0.5f);
						ToRotateCell.YPos = parentCell.YPos + (int)(parentCell.Height * 0.75f);
						break;
					case 3: // SW -> W
						ToRotateCell.XPos = parentCell.XPos - (parentCell.Width);
						ToRotateCell.YPos = parentCell.YPos;

						break;
					case 4: // W -> NW
						ToRotateCell.XPos = parentCell.XPos - (int)(parentCell.Width * 0.5f);
						ToRotateCell.YPos = parentCell.YPos - (int)(parentCell.Height * 0.75f);
						break;
					case 5: // NW -> NE
						ToRotateCell.XPos = parentCell.XPos + (int)(parentCell.Width * 0.5f);
						ToRotateCell.YPos = parentCell.YPos - (int)(parentCell.Height * 0.75f);
						break;

					default: // should NEVER get here

						break;

				}
			}
			else
			{
				switch (currentConnection)
				{
					case 0: // NE -> NW

						break;
					case 1: // E -> NE

						break;
					case 2: // SE -> E

						break;
					case 3: // SW -> SE

						break;
					case 4: // W -> SW

						break;
					case 5: // NW -> W

						break;

					default: // should NEVER get here

						break;
				}
			}

		}


		#region Mongame Specific

		public void Load()
		{

		}

		public void Unload()
		{

		}


		public void UpdateInputHandler(MouseState mouseState, MouseState prevMouseState)
		{

		}

		public virtual void Update(GameTime gameTime)
		{
			if (IsInside(Mouse.GetState().Position.ToVector2()))
			{
				if (_parentgrid != null)
				{
					_parentgrid.SelectedCell = this;
				}
				if (this.LinkedDataCell != null)
				{
					//Console.WriteLine("Mouse is inside: [{0}]", Data);

					_transparency = .5f;
				}
				_fillColor = Color.BlueViolet;
			}
			else
			{
				//This is just hovering over the cells.
				if (Row >= 0 && Column >= 0)
				{
					//CraftingSystemTester.DebugOutToConsole(String.Format("Hovering over grid cell [{0},{1}]", Row, Column));
					_fillColor = Color.White;
				}

				_transparency = 1;
				// _parentgrid.SelectedCell = null;
			}
		}

		public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Draw(CellImageFill, DrawRectangle, _fillColor * _transparency);
			//We need to draw EACH SIDE of the hexagon individually
			spriteBatch.Draw(CellImageOutline, NorthEastEdge_Texture, _NorthEastEdge_Texture, _neEdgefillColor);
			spriteBatch.Draw(CellImageOutline, EastEdge_Texture, _EastEdge_Texture, _eEdgefillColor);
			spriteBatch.Draw(CellImageOutline, SouthEastEdge_Texture, _SouthEastEdge_Texture, _seEdgefillColor);

			spriteBatch.Draw(CellImageOutline, SouthWestEdge_Texture, _SouthWestEdge_Texture, _swEdgefillColor);
			spriteBatch.Draw(CellImageOutline, WestEdge_Texture, _WestEdge_Texture, _wEdgefillColor);
			spriteBatch.Draw(CellImageOutline, NorthWestEdge_Texture, _NorthWestEdge_Texture, _nwEdgefillColor);

			if(_debug)
				CellValueNUm_GTB.Draw(gameTime, spriteBatch);
			//spriteBatch.Draw(CellImageFill, DrawRectangle, _fillColor * _transparency);
		}

		#endregion

		#endregion


	}
}
