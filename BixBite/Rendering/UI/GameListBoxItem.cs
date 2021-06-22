using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI
{
	public class GameListBoxItem : UIComponent
	{
		public GameListBox Parent;

		#region Properties

		public override bool bIsActive
		{
			get => _isActive;
			set => _isActive = value;
		}

		public bool bCanSelect = true;

		public int Width;
		public int Height;

		public bool bIsSelected = false;

		public Vector2 RelOriginPosition;
		public Vector2 AbsolutePosition;

		private Vector2 HighLightedPosition;

		public List<UIComponent> Controls = new List<UIComponent>();
		#endregion

		#region Field


		private bool _bShowBorder = true;
		private bool _isActive = false;
		
		private Texture2D _borderTexture;
		//private Texture2D _highlightedTexture;
		#endregion

		#region Hooks/Delegates

		#endregion


		#region Constructors

		public GameListBoxItem(GameListBox Parent, int width, int height)
		{
			this.Parent = Parent;
			this.Width = width;
			this.Height = height;
		}
		#endregion

		#region Methods
		public void LoadBorderTexture(Texture2D text)
		{
			this._borderTexture = text;
		}

		public void SetActiveStatus(bool newact)
		{
			this._isActive = newact;
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
			AbsolutePosition = vec;
			for (int i = 0; i < Controls.Count; i++)
			{

				if (Controls[i] is GameTextBlock GTB)
				{
					GTB.Position = AbsolutePosition;
				}
				else if (Controls[i] is GameIMG img)
				{
					img.Position = AbsolutePosition;
				}

			}

		}

		#region Monogame
		public override void Update(GameTime gameTime)
		{
			if (_isActive)
			{
				//In here we need to think of the players input and render components as needed.



			}

		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!_isActive) return;

			//border
			if(_borderTexture != null)
				//spriteBatch.Draw(_borderTexture, AbsolutePosition, Color.White);
				spriteBatch.Draw(_borderTexture, new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, Width,Height), Color.White);

			//We need to draw all the internal Components inside the listbox.
			for (int i = 0; i < Controls.Count; i++)
			{
				Controls[i].Draw(gameTime, spriteBatch);
			}
		}
		#endregion



	}
}
