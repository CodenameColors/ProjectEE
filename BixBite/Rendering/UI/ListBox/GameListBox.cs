using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Rendering.UI.ListBox
{
	public class GameListBox : BaseListBox
	{
		#region Delegates

		#endregion

		#region Fields
		//Keys
		private Keys IncrementKey;
		private Keys DecrementKey;
		private Keys PageIncrementKey;
		private Keys PageDecrementKey;

		private Keys SelectionKey;
		private Keys BackKey;

		private int _listBoxRenderPointer;
		private int _keyDownHeldFrames;
		private int _holdDownFrameLimit;

		private Vector2 _position = new Vector2();

		private Rectangle _drawRectangle = Rectangle.Empty;
		private Rectangle _highlightRectangle = Rectangle.Empty;

		private Texture2D _borderTexture;
		private Texture2D _highlightedTexture;

		private KeyboardState _prevKeyboardState;

		protected List<Vector2> _itemPositions_List = new List<Vector2>();
		private List<Vector2> _highLightedPositions_List = new List<Vector2>();
		protected GraphicsDevice _graphicsDevice;
		#endregion

		#region Properties
		public Vector2 Position
		{
			get
			{
				_position.X = XPos;
				_position.Y = YPos;
				return _position;
			}
		}

		/// <summary>
		/// Where this Button is drawn on the screen.  XPos, YPos, Width, Height
		/// </summary>
		public Rectangle DrawRectangle
		{
			get
			{
				_drawRectangle.X = XPos + XOffset;
				_drawRectangle.Y = YPos + YOffset;
				_drawRectangle.Width = Width;
				_drawRectangle.Height = Height;
				return _drawRectangle;
			}
		}

		public Rectangle HighlightRectangle
		{
			get
			{
				_highlightRectangle.X = (int)HighlightedPosition.X;
				_highlightRectangle.Y = (int)HighlightedPosition.Y;

				_highlightRectangle.Width = InnerWidth + _highlightTextureWidth_offset * 2;
				_highlightRectangle.Height = InnerHeight + _highlightTextureHeight_offset * 2;
				return _highlightRectangle;
			}
			set { _highlightRectangle = value; }
		}

		public override int SelectedIndex
		{
			get => int.Parse(GetPropertyData("SelectedIndex").ToString());
			set
			{
				SetProperty("SelectedIndex", value);
				ChangeDisplayedItems(value);
			}
		}

		public Vector2 HighlightedPosition;
		#endregion

		#region Contructors
		public GameListBox(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, int borderW, Color borderColor,
			int xOff, int yOff, int innerW, int InnerH, int innerSpacing, int maxDisplayedItems, int holdFrameLimit,
			Keys Inc, Keys Dec, Keys PageInc, Keys PageDec, Keys Select, Keys back, GraphicsDevice graphicsDevice,
			Texture2D borderTexture = null, Texture2D highlightedTexture = null, 
			EPositionType positionType = EPositionType.Vertical) 
			: base(UIName, xPos, yPos, width, height, zindex, border, borderW, innerSpacing, xOff, yOff, innerW, InnerH, maxDisplayedItems, positionType)
		{
			this.IncrementKey = Inc;
			this.DecrementKey = Dec;
			this.PageIncrementKey = PageInc;
			this.PageDecrementKey = PageDec;
			this.SelectionKey = Select;
			this.BackKey = back;

			this._borderTexture = borderTexture;
			this._highlightedTexture = highlightedTexture;

			this._holdDownFrameLimit = holdFrameLimit;

			this._graphicsDevice = graphicsDevice;

			if (borderTexture == null && border)
			{
				SetBorder(BorderWidth, borderColor);
			}

		}


		#endregion

		#region Methods

		public void SetBorder(int borderwidth, Color color)
		{
			_borderTexture = new Texture2D(_graphicsDevice, Width, Height);
			ProgressBar.Utilities.CreateBorder(_borderTexture, borderwidth, color);
			this.BorderWidth = borderwidth;
		}

		public void LoadBorderTexture(Texture2D text)
		{
			this._borderTexture = text;
		}

		public void LoadHighlightedTexture(Texture2D text)
		{
			this._highlightedTexture = text;
			HighlightedPosition = new Vector2(Items[0].XPos - ((_highlightTextureWidth_offset)), Items[0].YPos - ((_highlightTextureHeight_offset)));


		}

		public void SetActiveStatus(bool newact)
		{
			this.bIsActive = newact;
			foreach (ListBoxItems.GameListBoxItem lbi in Items)
			{
				lbi.SetActiveStatus(true);
			}
		}

		public void SetSpacing(int newspace)
		{
			_spacing = newspace;
		}

		public virtual void SetAbsolutePosition_Items(Vector2 startpos)
		{
			if (_listBoxEPositionType == EPositionType.Vertical) //Vertical Placement
			{
				int currentpos = (int)startpos.Y;
				for (int i = 0; i < Items.Count; i++)
				{
					(Items[i] as ListBoxItems.GameListBoxItem).XPos = (int)startpos.X; 
					(Items[i] as ListBoxItems.GameListBoxItem).YPos = (int)currentpos + InnerHeight + _spacing;
					//Add the initial Objects Origin
					(Items[i] as ListBoxItems.GameListBoxItem).XPos += (int)this.Position.X;
					(Items[i] as ListBoxItems.GameListBoxItem).YPos += (int)this.Position.Y;

					if (i < MaxDisplayedItems)
						_itemPositions_List.Add(((Items[i] as ListBoxItems.GameListBoxItem) as ListBoxItems.GameListBoxItem).Position);

					currentpos += InnerHeight + _spacing;
				}
			}
			else if (_listBoxEPositionType == EPositionType.Horizontal) //Vertical Placement//Horizontal Placement
			{

				int currentpos = (int)startpos.X;
				for (int i = 0; i < Items.Count; i++)
				{
					(Items[i] as ListBoxItems.GameListBoxItem).XPos = currentpos + InnerWidth + _spacing;
					(Items[i] as ListBoxItems.GameListBoxItem).YPos = (int)startpos.Y;
					//Add the initial Objects Origin
					(Items[i] as ListBoxItems.GameListBoxItem).XPos += (int)this.Position.X;
					(Items[i] as ListBoxItems.GameListBoxItem).YPos += (int)this.Position.Y;
					//(Items[i] as ListBoxItems.GameListBoxItem).Position = this.Position;

					if (i < MaxDisplayedItems)
						_itemPositions_List.Add(((Items[i] as ListBoxItems.GameListBoxItem) as ListBoxItems.GameListBoxItem).Position);
					currentpos += InnerWidth + _spacing;
				}

			}
		}

		public virtual void SetHighlightedPositions_Items(int xoff, int yoff)
		{

			for (int i = 0; i < Items.Count; i++)
			{
				ListBoxItems.GameListBoxItem gamelb = ((Items[i] as ListBoxItems.GameListBoxItem) as ListBoxItems.GameListBoxItem);

				(Items[i] as ListBoxItems.GameListBoxItem).SetHighLightedPosition(new Vector2((Items[i] as ListBoxItems.GameListBoxItem).Position.X - xoff,
					(Items[i] as ListBoxItems.GameListBoxItem).YPos - yoff));

				//Set the highlightedPositions for later HAS TO BE less the max displayed count. Or it will GO OFF SCREEN
				if (i < MaxDisplayedItems)
					_highLightedPositions_List.Add((Items[i] as ListBoxItems.GameListBoxItem).GetHighLightedPosition());

			}

			//for (int i = 0; i < Items.Count; i++)
			//{
			//	(Items[i] as ListBoxItems.GameListBoxItem).SetHighLightedPosition(new Vector2((Items[i] as ListBoxItems.GameListBoxItem).Position.X - xoff,
			//		(Items[i] as ListBoxItems.GameListBoxItem).Position.Y - yoff));

			//	//Set the highlightedPositions for later HAS TO BE less the max displayed count. Or it will GO OFF SCREEN
			//	if (i < MaxDisplayedItems)
			//		_highLightedPositions_List.Add((Items[i] as ListBoxItems.GameListBoxItem).GetHighLightedPosition());
			//}

			//Set the correct width and height offset
			_highlightTextureWidth_offset = xoff;
			_highlightTextureHeight_offset = yoff;
			if (_highLightedPositions_List.Count > 0)
				HighlightedPosition = _highLightedPositions_List[0];
		}

		public void IncrementSelectedIndex()
		{
			_SelectedDisplayedIndex++;
			SelectedIndex++;
		}

		public void DecrementSelectedIndex()
		{
			_SelectedDisplayedIndex--;
			SelectedIndex--;
		}

		public void PageIncrementSelectedIndex()
		{
			//_selectedDisplayedIndex +=  MaxDisplayed; //UNCOMMENT to Clamp the movement to edge. Current stays in place.
			//SelectedIndex += MaxDisplayedItems;
			if (SelectedIndex + MaxDisplayedItems > Items.Count - 1)
			{
				SelectedIndex = Items.Count-1;
				_SelectedDisplayedIndex = MaxDisplayedItems - 1;
			}
			else SelectedIndex += MaxDisplayedItems;
		}

		public void ResetSelected()
		{
			_SelectedDisplayedIndex = 0;
			SelectedIndex = 0;
		}

		public void PageDecrementSelectedIndex()
		{
			//_selectedDisplayedIndex -= MaxDisplayed; /UNCOMMENT to Clamp the movement to edge. Current stays in place.
			SelectedIndex -= MaxDisplayedItems;
		}

		public void ChangeDisplayedItems(int CurrentIndex)
		{

			//CHANGING HIGHLIGHT TEXTURE////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			//From 0 -> Max
			if (CurrentIndex < 0)
			{
				_SelectedDisplayedIndex = (Items.Count < MaxDisplayedItems ? Items.Count - 1 : MaxDisplayedItems - 1);
				SelectedIndex = Items.Count - 1;
			}
			//Incremented But don't move the selectedTexture
			else if (CurrentIndex < Items.Count && _SelectedDisplayedIndex >= MaxDisplayedItems)
			{
				_SelectedDisplayedIndex = MaxDisplayedItems - 1;
			}
			//Decremented But don't move the selectedTexture
			else if (CurrentIndex >= 0 && _SelectedDisplayedIndex < 0)
			{
				_SelectedDisplayedIndex = 0;
			}
			//Incremented > Max So rollback to 0
			else if (CurrentIndex >= Items.Count)
			{
				_SelectedDisplayedIndex = 0;
				SelectedIndex = 0;
				
			}
			//Just move it normally
			else
			{

			}
			HighlightedPosition = _highLightedPositions_List[_SelectedDisplayedIndex];
			//CHANGING HIGHLIGHT TEXTURE////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			//CHANGING DISPLAYED DATA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			//Get the first item we need to render
			_listBoxRenderPointer = (SelectedIndex - _SelectedDisplayedIndex < 0 ? 0 : SelectedIndex - _SelectedDisplayedIndex);

			//Set the Positions of the items that need to be rendered
			int limit = (Items.Count < MaxDisplayedItems
				? Items.Count
				: (Items.Count - 1 - _listBoxRenderPointer < MaxDisplayedItems
					? Items.Count - _listBoxRenderPointer
					: MaxDisplayedItems));
			for (int i = 0; i < limit; i++)
			{
				(Items[i + _listBoxRenderPointer] as ListBoxItems.GameListBoxItem).SetAbsolutePosition(_itemPositions_List[i]);

			}

		

			//CHANGING DISPLAYED DATA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}

		/// <summary>
		/// This method is here to disable game listbox items from be selected.
		/// AND also if a textbox is detected inside any of the children change the text color.
		/// </summary>
		/// <param name="grayList"></param>
		/// <param name="disabledTextColor"></param>
		public void GreyOutOptions(List<int> grayList, Color disabledTextColor)
		{
			for (int i = 0; i < grayList.Count; i++)
			{
				(Items[i] as ListBoxItems.GameListBoxItem).bCanSelect = false;

				for (int j = 0; j < (Items[i] as ListBoxItems.GameListBoxItem).Controls.Count; j++)
				{
					if ((Items[i] as ListBoxItems.GameListBoxItem).Controls[j] is TextBlock.GameTextBlock gtb)
						gtb.TextColor = disabledTextColor;
				}
			}
		}

		public void GreyOutOptions()
		{

		}

		public void SetPosition(Vector2 newpos)
		{
			//Move the Inner Items

			for (int i = 0; i < (Items.Count < MaxDisplayedItems ? Items.Count : MaxDisplayedItems); i++)
			{
				//Get the Difference of Position
				Vector2 TempPos_vec = (Items[i] as ListBoxItems.GameListBoxItem).Position;
				Vector2 diff_vec = (Items[i] as ListBoxItems.GameListBoxItem).Position - this.Position;
				//Set with new position.
				(Items[i] as ListBoxItems.GameListBoxItem).XPos = (int)(newpos + diff_vec).X;
				(Items[i] as ListBoxItems.GameListBoxItem).YPos = (int)(newpos + diff_vec).Y;
				_itemPositions_List[i] = (Items[i] as ListBoxItems.GameListBoxItem).Position;

				//Set the Position of the new Highlighted Positions.
				if (_highLightedPositions_List.Count > 0) // Does the user WANT to have highlighting?
					_highLightedPositions_List[i] = (_highLightedPositions_List[i] - this.Position) + newpos;

				//We need to NOW MOVE THE INNER controls of the inner items.
				for (int j = 0; j < (Items[i] as ListBoxItems.GameListBoxItem).Controls.Count; j++)
				{
					//Get the Difference.
					(Items[i] as ListBoxItems.GameListBoxItem).Controls[j].XPos = (int)(((Items[i] as ListBoxItems.GameListBoxItem).Controls[j].XPos - TempPos_vec.X) + (Items[i] as ListBoxItems.GameListBoxItem).Position.X);
					(Items[i] as ListBoxItems.GameListBoxItem).Controls[j].YPos = (int)(((Items[i] as ListBoxItems.GameListBoxItem).Controls[j].YPos - TempPos_vec.Y) + (Items[i] as ListBoxItems.GameListBoxItem).Position.Y);
				}
			}
			if (_highLightedPositions_List.Count > 0) // Does the user WANT to have highlighting?
				HighlightedPosition = _highLightedPositions_List[0]; //Reset Our Current Selection Position.
			XPos = (int)newpos.X;
			YPos = (int)newpos.Y;
		}

		#endregion

		#region Monogame
		public void Update(GameTime gameTime)
		{
			if (bIsActive)
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
				if (SelectRequest_Hook != null && Items.Count > 0)
				{
					if (Items[SelectedIndex].bCanSelect)
						SelectRequest_Hook(SelectedIndex);
				}
			}


			//BACK REQUEST
			if (Keyboard.GetState().IsKeyDown(BackKey) && _prevKeyboardState.IsKeyUp(BackKey))
			{
				//Back.
				this.bIsActive = false;
			}



			_prevKeyboardState = Keyboard.GetState();
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!bIsActive) return;

			//spriteBatch.Draw(_borderTexture, Position, null, Color.White, 0, 
			//	Vector2.Zero, new Vector2(Width/(float)_borderTexture.Width, Height/(float)_borderTexture.Height)
			//	, SpriteEffects.None, 0 );
			if (_borderTexture != null)
				spriteBatch.Draw(_borderTexture, DrawRectangle, Color.White);

			int limit = (Items.Count < MaxDisplayedItems
				? Items.Count
				: (Items.Count - 1 - _listBoxRenderPointer < MaxDisplayedItems
					? Items.Count - _listBoxRenderPointer
					: MaxDisplayedItems));
			for (int i = 0; i < limit; i++)
			{
				(Items[i + _listBoxRenderPointer] as ListBoxItems.GameListBoxItem)?.Draw(gameTime, spriteBatch);
			}

			//We need to draw all the internal Components inside the listbox. make sure to draw this ABOVE
			if (_highlightedTexture != null && Items.Count > 0)
				spriteBatch.Draw(_highlightedTexture, HighlightRectangle, Color.White);

		}
		#endregion


	}
}