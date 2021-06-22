using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI
{
	public class GameProgressBar : GameUI
	{
		private Texture2D borderTexture2D;
		private int borderwidth = 0;

		protected Texture2D barTexture2D;
		protected Color barColor;

	
		private int currentStatNum;
		public int CurrentStatNum
		{
			get => currentStatNum;
		}

		public bool bLeftToRight = true;

		private int xrelorigin;
		private int yrelorigin;
		private int xoff;
		private int yoff;

		private int _maxWidth;
		private int _width;
		protected int _sourceWidth;
		private int _maxHeight;
		private int _height;

		public GameProgressBar(String UIName, int Width, int Height, int Zindex, int xoff, int yoff, bool LeftToRight, GraphicsDevice graphicsDevice ,String BackgroundPath = "#00000000") :
			base(UIName, Width, Height, Zindex, BackgroundPath)
		{
			this.xrelorigin = xoff;
			this.yrelorigin = yoff;
			this.bLeftToRight = LeftToRight;
			_maxWidth = Width;
			currentStatNum = _maxWidth;

			this._height = Height;
			this._maxHeight = Height;
			this._width = Width;

			this.graphicsDevice = graphicsDevice;
		}

		public void SetBorder(int borderwidth, Color color)
		{
			borderTexture2D = new Texture2D(graphicsDevice, 
				int.Parse(GetPropertyData("Width").ToString()), 
				int.Parse(GetPropertyData("Height").ToString()));
			Utilities.CreateBorder(borderTexture2D,borderwidth, color);
			this.borderwidth = borderwidth;
		}

		/// <summary>
		/// This sets the Percentage filled of the rectangle ON INIT
		/// </summary>
		/// <param name="StatPercent"></param>
		public void SetBarTexture(double StatPercent, Color color)
		{
			if (StatPercent > 1 || StatPercent <= 0) return;
			this.barColor = color;
			barTexture2D = new Texture2D(graphicsDevice,
				(int)(int.Parse(GetPropertyData("Width").ToString()) * StatPercent),
				int.Parse(GetPropertyData("Height").ToString()));
			Utilities.CreateFilledRectangle(barTexture2D, borderwidth, color);
			currentStatNum = barTexture2D.Width;
			if(!bLeftToRight)
				xoff += (int)(_maxWidth - CurrentStatNum);
			
		}

		public void IncrementBar(int value)
		{
			currentStatNum += value;
			if (currentStatNum > _maxWidth){ currentStatNum = _maxWidth; return;}
			SetBarTexture(currentStatNum/(double)_maxWidth, barColor);
			if (!bLeftToRight)
				xoff = (int) (_maxWidth - barTexture2D.Width);

		}
		public void decrementBar(int value)
		{
			currentStatNum -= value;
			if (currentStatNum <= 0) {currentStatNum = 1; return;}
			SetBarTexture(currentStatNum / (double)_maxWidth, barColor);
			if (!bLeftToRight)
				xoff = (int) (_maxWidth - currentStatNum);
			
		}

		public void SetBarWidth(float percent)
		{
			this._width = (int) (_maxWidth * percent);
			this._sourceWidth = (int) (barTexture2D.Width * percent);
		}

		public void SetBarLength(int value)
		{
			if (value <= _maxWidth)
			{
				currentStatNum = value;
				SetBarTexture(currentStatNum / (double)_maxWidth, barColor);
				if (!bLeftToRight)
					xoff = (int) (_maxWidth - currentStatNum);
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			//spriteBatch.Begin();
			if(borderTexture2D != null)
				spriteBatch.Draw(borderTexture2D, new Vector2(xrelorigin, yrelorigin), Color.White);
			if (barTexture2D != null)
				spriteBatch.Draw(barTexture2D, new Rectangle(xrelorigin, yrelorigin , _width, _maxHeight), 
					new Rectangle(0,0, _sourceWidth, barTexture2D.Height), Color.White);

			//spriteBatch.End();B
		}
	}

	static class Utilities
	{
		public static void CreateBorder(this Texture2D texture, int borderWidth, Color borderColor)
		{
			Color[] colors = new Color[texture.Width * texture.Height];

			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					bool colored = false;
					for (int i = 0; i <= borderWidth; i++)
					{
						if (x == i || y == i || x == texture.Width - 1 - i || y == texture.Height - 1 - i)
						{
							colors[x + y * texture.Width] = borderColor;
							colored = true;
							break;
						}
					}

					if (colored == false)
						colors[x + y * texture.Width] = Color.Transparent;
				}
			}

			texture.SetData(colors);
		}

		public static void CreateFilledRectangle(this Texture2D texture, int borderWidth, Color borderColor)
		{
			Color[] colors = new Color[texture.Width * texture.Height];
			for (int x = 0 + borderWidth +1; x < texture.Width - borderWidth- borderWidth -1; x++)
			{
				for (int y = 0 + borderWidth +1; y < texture.Height - borderWidth-1; y++)
				{
					colors[x + y * texture.Width] = borderColor;
				}
			}

			texture.SetData(colors);
		}

	}

}
