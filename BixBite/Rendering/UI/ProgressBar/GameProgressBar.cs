using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI.ProgressBar
{
	public class GameProgressBar : BaseProgressBar
	{
		#region Delegates

		#endregion

		#region Fields
		protected Texture2D _borderTexture2D;
		protected GraphicsDevice _graphicsDevice;

		protected Texture2D barTexture2D;
		protected Color _barColor;
		protected Color _borderColor;

		protected Vector2 _position = new Vector2();
		protected Vector2 _scale = new Vector2();
		protected Rectangle _drawRectangle = Rectangle.Empty;
		protected Rectangle _sourceRectangle = Rectangle.Empty;

		protected Vector2 _borderPositon = Vector2.Zero;

		#endregion

		#region Properties

		public override int CurrentVal
		{
			get => int.Parse(GetPropertyData("CurrentVal").ToString());
			set
			{
				//Clamping
				if(value < 0) SetProperty("CurrentVal", 0);
				else if(value >  MaxVal) SetProperty("CurrentVal", MaxVal);
				else SetProperty("CurrentVal", value);

				if (bHorizontal)
					SetBarWidth((float)CurrentVal / MaxVal);
				else SetBarHeight((float)CurrentVal / MaxVal);
			}
		}

		public Vector2 Scale
		{
			get
			{
				_scale.X = _scaleX;
				_scale.Y = _scaleY;
				return _scale;
			}
		}

		public virtual Vector2 BorderPosition
		{
			get
			{
				_borderPositon.X = XPos;
				_borderPositon.Y = YPos;
				return _borderPositon;
			}
		}

		public virtual Vector2 BarPosition
		{
			get
			{
				if (bInverseDirection)
				{
					if (bHorizontal)
					{
						_position.X = _originx + (_maxWidth - (SourceWidth * _scaleX));
						_position.Y = YPos;
					}
					else
					{
						_position.X = XPos;
						_position.Y = _originy + (_maxHeight - (SourceHeight * _scaleY));
					}
				}
				else
				{
					_position.X = XPos;
					_position.Y = YPos;
				}
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
				_drawRectangle.X = (int)BarPosition.X + XOffset + BorderWidth;
				_drawRectangle.Y = (int)BarPosition.Y + YOffset + BorderWidth;
				_drawRectangle.Width = Width - BorderWidth *2;
				_drawRectangle.Height = Height - BorderWidth * 2;
				return _drawRectangle;
			}
		}

		public Rectangle SourceRectangle
		{
			get
			{
				if (bInverseDirection)
				{
					if (bHorizontal)
					{
						_sourceRectangle.X = 0 + (barTexture2D.Width - SourceWidth);
						_sourceRectangle.Y = 0;
					}
					else
					{
						_sourceRectangle.X = 0;
						_sourceRectangle.Y = 0 + (barTexture2D.Height - SourceHeight);
					}
				}
				else
				{
					_sourceRectangle.X = 0;
					_sourceRectangle.Y = 0;
				}

				_sourceRectangle.Width = SourceWidth;
				_sourceRectangle.Height = SourceHeight;
				return _sourceRectangle;
			}
		}
		#endregion

		#region Contructors

		public GameProgressBar(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, 
			int borderWidth, int xOff, int yOff, int currentVal, int maxVal, bool binverse, bool bHorizontal, GraphicsDevice graphicsDevice,
			Color barColor, Color borderColor) : 
			base(UIName, xPos, yPos, width, height, zindex, border, borderWidth, xOff, yOff, currentVal, maxVal, binverse, bHorizontal)
		{
			this._graphicsDevice = graphicsDevice;
			this._barColor = barColor;
			this._borderColor = borderColor;

			if (bBorder)
				SetBorder(BorderWidth, borderColor);
			SetBarTexture(((double) currentVal / MaxVal), barColor); //Default Bar

			//Currently Redundant
			if (bHorizontal)
				SetBarWidth((float) currentVal / MaxVal);
			else SetBarHeight((float) currentVal / MaxVal);
		}

		#endregion

		#region Methods

		public void SetBorder(int borderwidth, Color color)
		{
			_borderTexture2D = new Texture2D(_graphicsDevice, Width, Height);
			Utilities.CreateBorder(_borderTexture2D, borderwidth, color);
			this.BorderWidth = borderwidth;
		}

		/// <summary>
		/// This sets the Percentage filled of the rectangle ON INIT
		/// </summary>
		/// <param name="StatPercent"></param>
		public void SetBarTexture(double StatPercent, Color color)
		{
			if (StatPercent > 1 || StatPercent < 0) return;
			this._barColor = color;
			barTexture2D = new Texture2D(_graphicsDevice, Width - BorderWidth *2, Height - BorderWidth * 2);
			Utilities.CreateFilledRectangle(barTexture2D, BorderWidth, color);
			//CurrentVal = barTexture2D.Width;
			//if (!bLeftToRight)
			//	XOffset += (int)(MaxVal - CurrentVal);

		}

		//public void IncrementBar(int value)
		//{
		//	CurrentVal += value;
		//	if (CurrentVal > MaxVal) { CurrentVal = MaxVal; return; }
		//	SetBarTexture(CurrentVal / (double)MaxVal, _barColor);
		//	if (!bLeftToRight)
		//		XOffset = (int)(Width - barTexture2D.Width);

		//}
		//public void decrementBar(int value)
		//{
		//	CurrentVal -= value;
		//	if (CurrentVal <= 0) { CurrentVal = 1; return; }
		//	SetBarTexture(CurrentVal / (double)MaxVal, _barColor);
		//	if (!bLeftToRight)
		//		XOffset = (int)(Width - CurrentVal);

		//}

		public virtual void SetBarHeight(float percent)
		{
			this.SourceHeight = (int)(_maxHeight * percent);
			this.Height = (int)(_maxHeight * percent);
		}

		public virtual void SetBarWidth(float percent)
		{
			this.SourceWidth = (int)(_maxWidth * percent);
			this.Width = (int)(_maxWidth * percent);
		}

		//public void SetBarLength(int value)
		//{
		//	if (value <= MaxVal)
		//	{
		//		CurrentVal = value;
		//		SetBarTexture(CurrentVal / (double)MaxVal, _barColor);

		//		if (!bLeftToRight)
		//			XOffset = (int)(MaxVal - CurrentVal);
		//	}
		//}

		public void Update(GameTime gameTime)
		{
			if (!bIsActive) return;
		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!bIsActive) return;
			//spriteBatch.Begin();
			if (_borderTexture2D != null)
				spriteBatch.Draw(_borderTexture2D, BorderPosition, Color.White);
			if (barTexture2D != null)
				spriteBatch.Draw(barTexture2D, DrawRectangle, SourceRectangle, Color.White);

			//spriteBatch.End();B
		}
		#endregion

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
					for (int i = 0; i < borderWidth; i++)
					{
						if (x == i || y == i || x == texture.Width -1 - i || y == texture.Height -1 -i)
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
			for (int x = 0 ; x < texture.Width ; x++)
			{
				for (int y = 0 ; y < texture.Height ; y++)
				{
					colors[x + y * texture.Width] = borderColor;
				}
			}

			texture.SetData(colors);
		}

	}

}
