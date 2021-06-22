using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Rendering.UI
{

	public enum EPositionType
	{
		NONE,
		Vertical,
		Horizontal,
		Custom,
	}

	public class GameListBox : UIComponent
	{

		
		#region Properties

		public int Width;
		public int Height;
		public int InnerWidth;
		public int InnerHeight;

		public override bool bIsActive
		{
			get => _isActive;
			set => _isActive = value;
		}

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{

				_selectedIndex = value;

				ChangeDisplayedItems(value);

			}
		}

		public int MaxDisplayed; //How many internal objects should be rendered at once.

		public Vector2 HightlightedPosition;

		public List<GameListBoxItem> Items = new List<GameListBoxItem>();
		
		//Keys
		public Keys IncrementKey;
		public Keys DecrementKey;
		public Keys PageIncrementKey;
		public Keys PageDecrementKey;

		public Keys SelectionKey;
		public Keys BackKey;
		
		//

		public Rectangle BorderRectangle
		{
			get
			{
				_borderRectangle.X = (int)Position.X;
				_borderRectangle.Y = (int)Position.Y;

				_borderRectangle.Width = Width;
				_borderRectangle.Height = Height;
				return _borderRectangle;
			}
			set { _borderRectangle = value; }
		}

		public Rectangle HightlightRectangle
		{
			get
			{
				_hightlightRectangle.X = (int)HightlightedPosition.X;
				_hightlightRectangle.Y = (int)HightlightedPosition.Y;

				_hightlightRectangle.Width = InnerWidth + _hightlightTextureWidth_offset *2;
				_hightlightRectangle.Height = InnerHeight + _hightlightTextureHeight_offset *2;
				return _hightlightRectangle;
			}
			set { _hightlightRectangle = value; }
		}

		#endregion

		#region Field

		private int _listBoxRenderPointer;
		private int _selectedIndex = 0;
		private int _selectedDisplayedIndex = 0;
		private int _spacing;
		private int _hightlightTextureWidth_offset;
		private int _hightlightTextureHeight_offset;

		private bool _bShowBorder = true;
		private bool _isActive = false;

		private EPositionType _listBoxEPositionType = EPositionType.NONE;

		private Rectangle _borderRectangle = Rectangle.Empty;
		private Rectangle _hightlightRectangle = Rectangle.Empty;

		private Texture2D _borderTexture;
		private Texture2D _highlightedTexture;

		protected List<Vector2> _itemPositions_List = new List<Vector2>();
		private List<Vector2> _highLightedPositions_List = new List<Vector2>();

		private KeyboardState _prevKeyboardState;
		private int _keyDownHeldFrames;
		private int _holdDownFrameLimit;
		#endregion

		#region Hooks/Delegates

		public delegate void SelectRequest_Delegate(int selectedValue);
		public SelectRequest_Delegate SelectRequest_Hook = null;
		#endregion

		#region Constructors
		public GameListBox()
		{

		}

		public GameListBox(Vector2 pos, int width, int height, int innerWidth, int innerHeight, int maxDisplayed, Keys Inc, Keys Dec, Keys PageInc, Keys PageDec, Keys Select, Keys back, int holdFrameLimit, 
		bool showBorder = true, EPositionType posType = EPositionType.Vertical)
		{
			this.Position = pos;
			this.Width = width;
			this.InnerWidth = innerWidth;
			this.InnerHeight = innerHeight;
			this.Height = height;
			this.MaxDisplayed = maxDisplayed;
			this._bShowBorder = showBorder;
			this._listBoxEPositionType = posType;

			this.IncrementKey = Inc;
			this.DecrementKey = Dec;
			this.PageIncrementKey = PageInc;
			this.PageDecrementKey = PageDec;
			this._holdDownFrameLimit = holdFrameLimit;

			this.SelectionKey = Select;
			this.BackKey = back;


		}
		#endregion

		#region Methods

		public void LoadBorderTexture(Texture2D text)
		{
			this._borderTexture = text;
		}

		public void LoadHighlightedTexture(Texture2D text)
		{
			this._highlightedTexture = text;
			HightlightedPosition = new Vector2(Items[0].AbsolutePosition.X - ((_hightlightTextureWidth_offset)), Items[0].AbsolutePosition.Y - ((_hightlightTextureHeight_offset)) );


		}

		public void SetActiveStatus(bool newact)
		{
			this._isActive = newact;
			foreach (GameListBoxItem lbi in Items)
			{
				lbi.SetActiveStatus(true);
			}
		}

		public void SetSpacing(int newspace)
		{
			_spacing = newspace;
		}

		public virtual void SetAbsolutePosition_Items(Vector2 startpos, int innerHeight = 0, int innerWidth = 0)
		{
			if (_listBoxEPositionType ==  EPositionType.Vertical) //Vertical Placement
			{
				int currentpos = (int) startpos.Y;
				for (int i = 0; i < Items.Count; i++)
				{
					Items[i].AbsolutePosition = new Vector2(startpos.X, currentpos + innerHeight + _spacing);
					Items[i].AbsolutePosition += this.Position;

					if (i < MaxDisplayed)
						_itemPositions_List.Add(new Vector2(startpos.X, currentpos + innerHeight + _spacing));

					currentpos += innerHeight + _spacing;
				}
			}
			else if (_listBoxEPositionType == EPositionType.Horizontal) //Vertical Placement//Horizontal Placement
			{

				int currentpos = (int) startpos.X;
				for (int i = 0; i < Items.Count; i++)
				{
					Items[i].AbsolutePosition = new Vector2(currentpos + innerWidth + _spacing, startpos.Y );
					//Items[i].AbsolutePosition = this.Position;

					if(i < MaxDisplayed)
						_itemPositions_List.Add(Items[i].AbsolutePosition);
					currentpos += innerWidth + _spacing;
				}

			}
		}

		public virtual void SetHighlightedPositions_Items(int xoff, int yoff)
		{

			for (int i = 0; i < Items.Count; i++)
			{
				Items[i].SetHighLightedPosition(new Vector2(Items[i].AbsolutePosition.X - xoff,
					Items[i].AbsolutePosition.Y - yoff));

				//Set the highlightedPositions for later HAS TO BE less the max displayed count. Or it will GO OFF SCREEN
				if (i < MaxDisplayed)
					_highLightedPositions_List.Add(Items[i].GetHighLightedPosition());

			}

			for (int i = 0; i < Items.Count; i++)
			{
				Items[i].SetHighLightedPosition(new Vector2(Items[i].AbsolutePosition.X - xoff,
					Items[i].AbsolutePosition.Y - yoff));

				//Set the highlightedPositions for later HAS TO BE less the max displayed count. Or it will GO OFF SCREEN
				if (i < MaxDisplayed)
					_highLightedPositions_List.Add(Items[i].GetHighLightedPosition());
			}

			//Set the correct width and height offset
			_hightlightTextureWidth_offset = xoff;
			_hightlightTextureHeight_offset = yoff;
			if (_highLightedPositions_List.Count > 0)
				HightlightedPosition = _highLightedPositions_List[0];
		}

		public void IncrementSelectedIndex()
		{
			_selectedDisplayedIndex++;
			SelectedIndex++;
		}

		public void DecrementSelectedIndex()
		{
			_selectedDisplayedIndex--;
			SelectedIndex--;
		}

		public void PageIncrementSelectedIndex()
		{
			//_selectedDisplayedIndex +=  MaxDisplayed; //UNCOMMENT to Clamp the movement to edge. Current stays in place.
			SelectedIndex += MaxDisplayed;
		}

		public void ResetSelected()
		{
			_selectedDisplayedIndex = 0;
			SelectedIndex = 0;
		}

		public void PageDecrementSelectedIndex()
		{
			//_selectedDisplayedIndex -= MaxDisplayed; /UNCOMMENT to Clamp the movement to edge. Current stays in place.
			SelectedIndex -= MaxDisplayed;
		}

		public void ChangeDisplayedItems(int CurrentIndex)
		{

			//CHANGING HIGHLIGHT TEXTURE////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			//From 0 -> Max
			if (CurrentIndex < 0)
			{
				_selectedIndex = Items.Count - 1;
				_selectedDisplayedIndex = (Items.Count < MaxDisplayed ? Items.Count -1 : MaxDisplayed -1);
			}
			//Incremented But don't move the selectedTexture
			else if (CurrentIndex < Items.Count && _selectedDisplayedIndex >= MaxDisplayed )
			{
				_selectedDisplayedIndex = MaxDisplayed - 1;
			}
			//Decremented But don't move the selectedTexture
			else if (CurrentIndex >= 0 && _selectedDisplayedIndex < 0)
			{
				_selectedDisplayedIndex = 0;
			}
				//Incremented > Max So rollback to 0
			else if (CurrentIndex >= Items.Count)
			{
				_selectedIndex = 0;
				_selectedDisplayedIndex = 0;
			}
			//Just move it normally
			else
			{
				
			}
			HightlightedPosition = _highLightedPositions_List[_selectedDisplayedIndex];
			//CHANGING HIGHLIGHT TEXTURE////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			//CHANGING DISPLAYED DATA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			//Get the first item we need to render
			_listBoxRenderPointer = (_selectedIndex - _selectedDisplayedIndex < 0 ? 0 : _selectedIndex - _selectedDisplayedIndex );

			//Set the Positions of the items that need to be rendered
			for (int i = 0; i < (Items.Count < MaxDisplayed ? Items.Count : (Items.Count -1 - _listBoxRenderPointer < MaxDisplayed ? Items.Count - _listBoxRenderPointer : MaxDisplayed)); i++)
			{
				Items[i + _listBoxRenderPointer].SetAbsolutePosition(_itemPositions_List[i]);

			}
			//CHANGING DISPLAYED DATA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}

		/// <summary>
		/// This method is here to disable game listbox items from be selected.
		/// AND also if a textbox is detected inside any of the children change the text color.
		/// </summary>
		/// <param name="grayList"></param>
		/// <param name="disabledTextColor"></param>
		public void GreyOutOptions(List<int> grayList, Color disabledTextColor  )
		{
			for (int i = 0; i < grayList.Count; i++)
			{
				Items[i].bCanSelect = false;

				for (int j = 0; j < Items[i].Controls.Count; j++)
				{
					if (Items[i].Controls[j] is GameTextBlock gtb)
						gtb.PenColour = disabledTextColor;
				}
			}
		}

		public void GreyOutOptions()
		{
			
		}

		public void SetPosition(Vector2 newpos)
		{
			//Move the Inner Items

			for (int i = 0; i < (Items.Count < MaxDisplayed ? Items.Count : MaxDisplayed); i++)
			{
				//Get the Difference of Position
				Vector2 TempPos_vec = Items[i].AbsolutePosition;
				Vector2 diff_vec = Items[i].AbsolutePosition - this.Position;
				//Set with new position.
				Items[i].AbsolutePosition = newpos + diff_vec;
				_itemPositions_List[i] = Items[i].AbsolutePosition;

				//Set the Position of the new Highlighted Positions.
				if(_highLightedPositions_List.Count > 0) // Does the user WANT to have highlighting?
					_highLightedPositions_List[i] = (_highLightedPositions_List[i] - this.Position) + newpos;

				//We need to NOW MOVE THE INNER controls of the inner items.
				for (int j = 0; j < Items[i].Controls.Count; j++)
				{
					//Get the Difference.
					Items[i].Controls[j].Position = (Items[i].Controls[j].Position - TempPos_vec) + Items[i].AbsolutePosition;
				}
			}
			if (_highLightedPositions_List.Count > 0) // Does the user WANT to have highlighting?
				HightlightedPosition = _highLightedPositions_List[0]; //Reset Our Current Selection Position.
			this.Position = newpos;
		}

		#endregion

		#region Monogame
		public override void Update(GameTime gameTime)
		{
			if (_isActive)
			{
				//Increment/Decrement one at a time.
				if (Keyboard.GetState().IsKeyDown(DecrementKey) && _prevKeyboardState.IsKeyUp(DecrementKey))
				{
					//Move up
					this.DecrementSelectedIndex();
					_keyDownHeldFrames = 0; //reset timer.
				}
				else
				{
					if (_keyDownHeldFrames > _holdDownFrameLimit && Keyboard.GetState().IsKeyDown(DecrementKey))
					{
						_keyDownHeldFrames = 0; //reset timer.
						this.DecrementSelectedIndex();
					}

					_keyDownHeldFrames++;
				}

				if (Keyboard.GetState().IsKeyDown(IncrementKey) && _prevKeyboardState.IsKeyUp(IncrementKey))
				{
					//Move Down
					this.IncrementSelectedIndex();
					_keyDownHeldFrames = 0; //reset timer.

				}
				else
				{
					if (_keyDownHeldFrames > _holdDownFrameLimit && Keyboard.GetState().IsKeyDown(IncrementKey))
					{

						_keyDownHeldFrames = 0; //reset timer.
						this.IncrementSelectedIndex();
					}

					_keyDownHeldFrames++;
				}
			}

			//Increment/Decrement One PAGE at a time.
			if (Keyboard.GetState().IsKeyDown(PageDecrementKey) && _prevKeyboardState.IsKeyUp(PageDecrementKey))
			{
				//Move up
				this.PageDecrementSelectedIndex();
				_keyDownHeldFrames = 0; //reset timer.
			}
			else
			{
				if (_keyDownHeldFrames > _holdDownFrameLimit && Keyboard.GetState().IsKeyDown(PageDecrementKey))
				{
					_keyDownHeldFrames = 0; //reset timer.
					this.PageDecrementSelectedIndex();
				}

				_keyDownHeldFrames++;
			}

			if (Keyboard.GetState().IsKeyDown(PageIncrementKey) && _prevKeyboardState.IsKeyUp(PageIncrementKey))
			{
				//Move Down
				this.PageIncrementSelectedIndex();
				_keyDownHeldFrames = 0; //reset timer.

			}
			else
			{
				if (_keyDownHeldFrames > _holdDownFrameLimit && Keyboard.GetState().IsKeyDown(PageIncrementKey))
				{

					_keyDownHeldFrames = 0; //reset timer.
					this.PageIncrementSelectedIndex();
				}

				_keyDownHeldFrames++;
			}

			//SELECTED REQUEST
			if (Keyboard.GetState().IsKeyDown(SelectionKey) && _prevKeyboardState.IsKeyUp(SelectionKey))
			{
				//Selection.
				if (SelectRequest_Hook != null)
				{
					if(Items[_selectedIndex].bCanSelect)
						SelectRequest_Hook(_selectedIndex);
				}
			}


			//BACK REQUEST
			if (Keyboard.GetState().IsKeyDown(BackKey) && _prevKeyboardState.IsKeyUp(BackKey))
			{
				//Back.
				this._isActive = false;
			}



			_prevKeyboardState = Keyboard.GetState();
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if(!_isActive) return;

			//spriteBatch.Draw(_borderTexture, Position, null, Color.White, 0, 
			//	Vector2.Zero, new Vector2(Width/(float)_borderTexture.Width, Height/(float)_borderTexture.Height)
			//	, SpriteEffects.None, 0 );
			if(_borderTexture != null)
				spriteBatch.Draw(_borderTexture, BorderRectangle, Color.White);
			
			for (int i = 0; i < ( Items.Count < MaxDisplayed ? Items.Count : MaxDisplayed ); i++)
			{

				if (i + _listBoxRenderPointer <= Items.Count - 1)
					Items[i + _listBoxRenderPointer].Draw(gameTime, spriteBatch);
			}

			//We need to draw all the internal Components inside the listbox. make sure to draw this ABOVE
			if (_highlightedTexture != null)
				spriteBatch.Draw(_highlightedTexture, HightlightRectangle, Color.White);

		}
		#endregion


	}
}
