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

		private Texture2D barTexture2D;
		private Color barColor;

		private int _maxStatNum;
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

		public GameProgressBar(String UIName, int Width, int Height, int Zindex, int xoff, int yoff, bool LeftToRight, GraphicsDevice graphicsDevice ,String BackgroundPath = "#00000000") :
			base(UIName, Width, Height, Zindex, BackgroundPath)
		{
			this.xrelorigin = xoff;
			this.yrelorigin = yoff;
			this.bLeftToRight = LeftToRight;
			_maxStatNum = Width;
			currentStatNum = _maxStatNum;
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
				xoff += (int)(_maxStatNum - CurrentStatNum);
			
		}

		public void IncrementBar(int value)
		{
			currentStatNum += value;
			if (currentStatNum > _maxStatNum){ currentStatNum = _maxStatNum; return;}
			SetBarTexture(currentStatNum/(double)_maxStatNum, barColor);
			if (!bLeftToRight)
				xoff = (int) (_maxStatNum - barTexture2D.Width);

		}
		public void decrementBar(int value)
		{
			currentStatNum -= value;
			if (currentStatNum <= 0) {currentStatNum = 1; return;}
			SetBarTexture(currentStatNum / (double)_maxStatNum, barColor);
			if (!bLeftToRight)
				xoff = (int) (_maxStatNum - currentStatNum);
			
		}

		public void SetBarLength(int value)
		{
			if (value <= _maxStatNum)
			{
				currentStatNum = value;
				SetBarTexture(currentStatNum / (double)_maxStatNum, barColor);
				if (!bLeftToRight)
					xoff = (int) (_maxStatNum - currentStatNum);
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
				spriteBatch.Draw(barTexture2D, new Rectangle(xrelorigin + xoff, yrelorigin + yoff, barTexture2D.Width, barTexture2D.Height), Color.White);

			//spriteBatch.End();
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
