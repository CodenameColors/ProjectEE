using System;
using System.Collections.Generic;
using BixBite.Combat;
using BixBite.Rendering.UI.TextBlock;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Crafting.HexGrid
{
	/// <summary>
	/// These hex cells are used to create a whole puzzle piece. AND ONLY FOR THAT.
	/// Contains the Data for the each puzzle piece.
	/// </summary>
	public class PuzzlePieceHexCell : HexCell
	{

		#region Delegates/Events

		public delegate void OnClick_Hook(PuzzlePieceHexCell reference, MouseState mouseState);
		public OnClick_Hook OnClickHook = null;
		#endregion

		#region Fields

		/// <summary>
		/// Here to serve as a flag when calculating the total point vals.
		/// </summary>
		public bool bUsedForPointCalculation = false;
		private PuzzlePiece _parentPiece = null;

		private int _cellValue = -1;
		private EMagicType _magicType = EMagicType.NONE;
		//private HexPuzzleCellData _celldata = null;

		protected Color _basefillColor = Color.White;

		#endregion

		#region Properties

		public HexCell LinkedGridCell = null;


		public override int Spawn_XPos
		{
			get => _spawn_xpos;
			set => _spawn_xpos = value;
		}

		public override int Spawn_YPos
		{
			get => _spawn_ypos;
			set => _spawn_ypos = value;
		}

		public override int XPos
		{
			get => _xpos;
			set => _xpos = value;
		}

		public override int YPos
		{
			get => _ypos;
			set => _ypos = value;
		}

		public int CellValue
		{
			get => _cellValue;
			set => _cellValue = value;
		}

		public EMagicType MagicType
		{
			get => _magicType;
			set => _magicType = value;
		}

		//public override int Width
		//{
		//	get => (int)(_width * _parentPiece.ScaleX);
		//}
		//public virtual int Height 
		//{
		//	get => (int) (_height * _parentPiece.ScaleY);
		//}


		public bool bIsHoveringOver { get; set; }

		public override Rectangle DrawRectangle
		{
			get
			{
				_drawrectangle.X = _xpos + _parentPiece.DrawRectangle.X;
				_drawrectangle.Y = _ypos + _parentPiece.DrawRectangle.Y;
				_drawrectangle.Width = _width;
				_drawrectangle.Height = _height; //.Height
				return _drawrectangle;
			}
			set => _drawrectangle = value;
		}

		

		//public bool bCalcUsedFlag { get; set; }

		public List<PuzzlePieceHexCell> AlreadyCalculatedRefs = new List<PuzzlePieceHexCell>();

		#endregion

		#region Constructor

		public PuzzlePieceHexCell(PuzzlePiece parentRef, int numVal, float x, float y, int cellW, int cellH,
			int? row, int? column, Texture2D cellImageFill, Texture2D cellImageOutline, SpriteFont sf) 
			: base(parentRef, numVal, x, y, cellW, cellH, row, column,null, cellImageFill, cellImageOutline, sf)
		{
			_parentPiece = parentRef;

			//TODO: Remove
			_debug = true;
			

				CellValueNUm_GTB = new BixBite.Rendering.UI.TextBlock.GameTextBlock("NumVal", _spawn_xpos + parentRef.XPos, _spawn_ypos + parentRef.YPos, _width, _height, 1, false,
					(_width / 2), (_height / 2), numVal.ToString(), 0.0f, "#00000000", "", sf, null, Color.Black);
				CellValueNUm_GTB.bIsActive = true;
		}

		//FOR COPYING
		public PuzzlePieceHexCell(PuzzlePiece parent, PuzzlePieceHexCell referenceCell)
		:base(referenceCell._parentPiece, int.Parse(referenceCell.CellValueNUm_GTB.Text), referenceCell.XPos, referenceCell.YPos,
			referenceCell.Width, referenceCell.Height, referenceCell.Row, referenceCell.Column, null, referenceCell.CellImageFill, referenceCell.CellImageOutline, referenceCell._spriteFont)
		{
			this._parentPiece = parent;

			this._fillColor = referenceCell.FillColor;
			this.OnClickHook = referenceCell.OnClickHook;


			//TODO: Remove
			_debug = true;

			CellValueNUm_GTB = new BixBite.Rendering.UI.TextBlock.GameTextBlock("NumVal", _spawn_xpos + parent.XPos, _spawn_ypos + parent.YPos, _width, _height, 1, false,
				(_width / 2), (_height / 2), referenceCell.CellValueNUm_GTB.Text, 0.0f, "#00000000", "", referenceCell._spriteFont, null, Color.Black);
			CellValueNUm_GTB.bIsActive = true;

		}

		#endregion

		#region Methods

		public void SetPosition_GTB(int x, int y)
		{
			CellValueNUm_GTB.XPos = x + _parentPiece.XPos;

			CellValueNUm_GTB.YPos = y + _parentPiece.YPos;
		}

		public GameTextBlock GetGameTextbox()
		{
			return CellValueNUm_GTB;
		}

		public PuzzlePiece GetPuzzlePieceParent()
		{
			return _parentPiece;
		}

		public void Rescale(float x, float y)
		{
			_width =  (int)(_parentPiece.BaseCellWidth * x);
			_height = (int)(_parentPiece.BaseCellHeight * y);
		}

		public override bool IsInside(Vector2 posToTest)
		{
			float _hori = _width / 2.0f;
			float _vert = _height / 4.0f;

			float q2x = Math.Abs(posToTest.X - (_xpos + _parentPiece.XPos + (_width / 2)));         // transform the test point locally and to quadrant 2
			float q2y = Math.Abs(posToTest.Y - (_ypos + _parentPiece.YPos + (_height / 2)));         // transform the test point locally and to quadrant 2
			if (q2x > _hori || q2y > _vert * 2) return false;           // bounding test (since q2 is in quadrant 2 only 2 tests are needed)
			return 2 * _vert * _hori - _vert * q2x - _hori * q2y >= 0;   // finally the dot product can be reduced to this due to the hexagon symmetry
		}


		#region Mongame Rendering.

		public override void Update(GameTime gameTime)
		{
			MouseState mouseState = Mouse.GetState();
			if (IsInside(Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2()))
			{
				// Console.WriteLine("Mouse is inside: [{0}]", Data);
				//ONLY activate if the player hasn't selected a piece.
				if (CraftingMinigame.Instance.CurrentSelectedPuzzlePiecePair.Key == null)
				{
					_transparency = .25f;

					bIsHoveringOver = true;
					OnClickHook?.Invoke(this, mouseState);
				}
			}
			else
			{
				if(!_parentPiece.bIsSelected)
					_transparency = 1f;

				bIsHoveringOver = false;
			}
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{



			spriteBatch.Draw(CellImageOutline, DrawRectangle, _fillColor * _transparency);

			if (_debug)
			{
				CellValueNUm_GTB.XPos = XPos + _parentPiece.DrawRectangle.X;
				CellValueNUm_GTB.YPos = YPos + _parentPiece.DrawRectangle.Y;
				CellValueNUm_GTB.Draw(gameTime, spriteBatch);

			}
		}

		#endregion

		#endregion


	}
}
