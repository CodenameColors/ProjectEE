using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Rendering.UI.ListBox.ListBoxItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BixBite.Rendering.UI.ListBox
{
	public class GameMultiSelectionListbox : GameListBox
	{

		#region Delegates
		public delegate void OnMaxSelected_Delegate();
		public OnMaxSelected_Delegate MaxSeleectionReached_Hook = null;
		#endregion

		#region Properties

		#endregion

		#region Fields
		private int _maxSelectableNumber = 1;
		private List<int> _selectedIndexes = new List<int>();
		#endregion

		#region Constructor
		///
		public GameMultiSelectionListbox(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, int borderW,
			Color borderColor, int xOff, int yOff, int innerW, int InnerH, int innerSpacing, int maxDisplayedItems, int selectableNumber, int holdFrameLimit,
			Keys Inc, Keys Dec, Keys PageInc, Keys PageDec, Keys Select, Keys back,
			GraphicsDevice graphicsDevice, Texture2D borderTexture = null, Texture2D highlightedTexture = null, EPositionType positionType = EPositionType.Vertical) :
			base(UIName, xPos, yPos, width, height, zindex, border, borderW, borderColor, xOff, yOff, innerW, InnerH, innerSpacing, maxDisplayedItems, holdFrameLimit, Inc, Dec, PageInc, PageDec, Select, back, graphicsDevice, borderTexture, highlightedTexture, positionType)
		{
			_maxSelectableNumber = selectableNumber;

		}
		#endregion

		#region Methods

		protected override bool IsSelectInList(Keys selectKey)
		{
			bool retBool = false;
			//SELECTED REQUEST
			if (Keyboard.GetState().IsKeyDown(selectKey) && _prevKeyboardState.IsKeyUp(selectKey))
			{
				retBool = true;

				// Do we need to Add or Remove this index?
				if (_selectedIndexes.Count < _maxSelectableNumber)
				{
					if (!_selectedIndexes.Contains(SelectedIndex))
					{
						_selectedIndexes.Add(SelectedIndex);
						if (Items[SelectedIndex] is GameListBoxItemSelectable selectable)
							selectable.bIsSelected = true;
					}
				}
				else
				{
					if (Items[SelectedIndex] is GameListBoxItemSelectable selectable)
						selectable.bIsSelected = false;
					_selectedIndexes.Remove(SelectedIndex);
				}

				// One Selection.
				if (SelectRequest_Hook != null && Items.Count >= 0)
				{
					if (Items[SelectedIndex].bCanSelect)
						SelectRequest_Hook(SelectedIndex);
				}

				// All selection reached delegate
				if (MaxSeleectionReached_Hook != null && _selectedIndexes.Count == _maxSelectableNumber)
				{
					//_selectedIndexes.Add(SelectedIndex);
					MaxSeleectionReached_Hook();
				}

			}

			return retBool;
		}

		protected override bool IsBackInList(Keys bacKey)
		{
			bool retBool = false;
			//BACK REQUEST
			if (Keyboard.GetState().IsKeyDown(bacKey) && _prevKeyboardState.IsKeyUp(bacKey))
			{
				if (_selectedIndexes.Contains(SelectedIndex))
				{
					if (Items[SelectedIndex] is GameListBoxItemSelectable selectable)
						selectable.bIsSelected = false;
					_selectedIndexes.Remove(SelectedIndex);
				}
				retBool = true;

			}
			return retBool;
		}

		#endregion

		#region Monogame

		public void Update(GameTime gameTime)
		{
			// Remember to handle all the base game list box updates.
			base.Update(gameTime);


			// This is where the multi selection updates go
		}


		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Remember to handle all the base game list box Draws.
			base.Draw(gameTime, spriteBatch);

			// This is where we start drawing the multi selection features.
		}

		#endregion




	}
}
