using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI.ListBox.ListBoxItems
{
	public class GameListBoxItem : BaseListBoxItem
	{

		#region Delegates

		#endregion

		#region Fields
		private Vector2 HighLightedPosition;
		private Texture2D _borderTexture;

		private Vector2 _position = new Vector2();
		private Rectangle _drawRectangle = Rectangle.Empty;
		#endregion

		#region Properties
		public Vector2 RelOriginPosition;
		//public Vector2 AbsolutePosition;


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
				_drawRectangle.X = XPos; //+ XOffset;
				_drawRectangle.Y = YPos; //+ YOffset;
				_drawRectangle.Width = Width;
				_drawRectangle.Height = Height;
				return _drawRectangle;
			}
		}

		#endregion

		#region Contructors

		public GameListBoxItem(BaseListBox parentListBox, string UIName, int xPos, int yPos, int width, int height,
			int zindex, bool border, bool bCanSelect, Texture2D borderTexture = null)
			: base(parentListBox, UIName, xPos, yPos, width, height, zindex, border, bCanSelect)
		{

			this._borderTexture = borderTexture;

		}

		#endregion

		#region Methods
		public void LoadBorderTexture(Texture2D text)
		{
			this._borderTexture = text;
		}

		public void SetActiveStatus(bool newact)
		{
			this.bIsActive = newact;
		}
		#endregion

		public void SetHighLightedPosition(Vector2 vec)
		{
			this.HighLightedPosition = vec;
		}

		public Vector2 GetHighLightedPosition()
		{
			return HighLightedPosition;
		}

		public void SetAbsolutePosition(Vector2 vec)
		{
			//AbsolutePosition = vec;
			XPos = (int)vec.X;
			YPos = (int)vec.Y;
			for (int i = 0; i < Controls.Count; i++)
			{

				if (Controls[i] is TextBlock.GameTextBlock GTB)
				{
					GTB.XPos = (int)vec.X;
					GTB.YPos = (int)vec.Y;
				}
				else if (Controls[i] is Image.GameImage img)
				{
					img.XPos = (int)vec.X;
					img.YPos = (int)vec.Y;
				}
				else if (Controls[i] is Button.GameButton but)
				{
					but.XPos = (int)vec.X;
					but.YPos = (int)vec.Y;
				}
				else if (Controls[i] is ProgressBar.GameProgressBar GPB)
				{
					GPB.XPos = (int)vec.X;
					GPB.YPos = (int)vec.Y;
				}

			}

		}

		#region Monogame
		public void Update(GameTime gameTime)
		{
			if (bIsActive)
			{
				//In here we need to think of the players input and render components as needed.



			}

		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!bIsActive) return;

			//border
			if (_borderTexture != null)
				//spriteBatch.Draw(_borderTexture, AbsolutePosition, Color.White);
				spriteBatch.Draw(_borderTexture, new Rectangle(XPos, YPos, Width, Height), Color.White);

			//We need to draw all the internal Components inside the listbox.
			for (int i = 0; i < Controls.Count; i++)
			{
				if (Controls[i] is TextBlock.GameTextBlock GTB)
				{
					GTB.Draw(gameTime, spriteBatch);
				}
				else if (Controls[i] is Image.GameImage img)
				{
					img.Draw(gameTime, spriteBatch);
				}
				else if (Controls[i] is Button.GameButton but)
				{
					but.Draw(gameTime, spriteBatch);
				}
				else if (Controls[i] is ProgressBar.GameProgressBar GPB)
				{
					GPB.Draw(gameTime, spriteBatch);
				}


			}
		}
		#endregion


	}
}
