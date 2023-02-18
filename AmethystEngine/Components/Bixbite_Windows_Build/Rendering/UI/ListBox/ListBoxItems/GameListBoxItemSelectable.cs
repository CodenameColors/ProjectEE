using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI.ListBox.ListBoxItems
{


	public class GameListBoxItemSelectable : GameListBoxItem
	{
		public bool bIsSelected = false;
		private Texture2D _selectionTexture;
		private Rectangle selectionDrawRectangle = Rectangle.Empty;

		private int _selectedTextureXPos;
		private int _selectedTextureYPos;
		private int _selectedTextureWidth;
		private int _selectedTextureHeight;

		/// <summary>
		/// Where this listbox item drawn on the screen.  XPos, YPos, Width, Height
		/// </summary>
		public Rectangle SelectionRectangle
		{
			get
			{
				selectionDrawRectangle.X = _selectedTextureXPos + XPos; //+ XOffset;
				selectionDrawRectangle.Y = _selectedTextureYPos + YPos ; //+ YOffset;
				selectionDrawRectangle.Width = _selectedTextureWidth;
				selectionDrawRectangle.Height = _selectedTextureHeight;
				return selectionDrawRectangle;
			}
		}

		public GameListBoxItemSelectable(BaseListBox parentListBox, string UIName, int xPos, int yPos, int xSelectedPos, int ySelectedPos,
		int width, int height, int selectedWidth, int selectedHeight, int zindex, bool border, bool bCanSelect,
			Texture2D selectedTexture2D = null, Texture2D borderTexture = null, object linkedDataObject = null) : base(parentListBox, UIName, xPos, yPos, width, height, zindex, border, bCanSelect, borderTexture, linkedDataObject)
		{
			_selectionTexture = selectedTexture2D;

			_selectedTextureXPos = xSelectedPos;
			_selectedTextureYPos = ySelectedPos;
			_selectedTextureWidth = selectedWidth;
			_selectedTextureHeight = selectedHeight;

		}

		public void Update(GameTime gameTime)
		{
			//Handle all the updates for base List Items
			base.Update(gameTime);

		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// rWe need to draw the base class elements
			base.Draw(gameTime, spriteBatch);

			// Now we need to draw all selectable elements
			if(_selectionTexture != null && bIsSelected)
				spriteBatch.Draw(_selectionTexture, SelectionRectangle, Color.White);


		}
	}
}
