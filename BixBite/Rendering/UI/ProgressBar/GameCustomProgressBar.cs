using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI.ProgressBar
{
	public class GameCustomProgressBar : GameProgressBar
	{

		public override Vector2 BarPosition
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
					_position.X = XPos + XOffset + BorderWidth;
					_position.Y = YPos + YOffset + BorderWidth;
				}
				return _position;
			}
		}


		//public override Vector2 Position
		//{
		//	get
		//	{
		//		_position.X = XPos + XOffset + BorderWidth;
		//		_position.Y = YPos + YOffset + BorderWidth;
		//		return _position;
		//	}
		//}

		public GameCustomProgressBar(string UIName, int xPos, int yPos, int width, int height, int zindex,
			bool border, int borderWidth, int xOff, int yOff, int currentVal, int maxVal, bool bInverses, bool bHori,
			GraphicsDevice graphicsDevice, Color barColor, Texture2D borderTexture2D, Texture2D barTexture)
			: base(UIName, xPos, yPos, width, height, zindex, border, borderWidth, xOff, yOff, currentVal, maxVal, bInverses, bHori, 
				graphicsDevice, barColor, Color.White)
		{
			this.barTexture2D = barTexture;
			this._borderTexture2D = borderTexture2D;


			//Texture doesn't always fit. So we gotta scale it!
			_scaleX = (width / (float) barTexture2D.Width);
			_scaleY = (height / (float) barTexture2D.Height);
			
			//Call the overrided methods so the correct source is set
			if (bHorizontal)
				SetBarWidth((float)currentVal / MaxVal);
			else SetBarHeight((float)currentVal / MaxVal);
		}


		public override void SetBarHeight(float percent)
		{
			if (barTexture2D == null) return;
			this.SourceHeight = (int)(barTexture2D.Height * percent);
			this.SourceWidth = (int) barTexture2D.Width;
			this.Height = (int)(_maxHeight * percent);
		}

		public override void SetBarWidth(float percent)
		{
			if (barTexture2D == null) return;
			this.SourceWidth = (int)(barTexture2D.Width  * percent);
			this.SourceHeight = (int)barTexture2D.Height;
			this.Width = (int)(_maxWidth * percent);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!bIsActive) return;
			//spriteBatch.Begin();
			if (_borderTexture2D != null)
				spriteBatch.Draw(_borderTexture2D, BorderPosition, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, ZIndex);
			if (barTexture2D != null)
				spriteBatch.Draw(barTexture2D, BarPosition, SourceRectangle, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, ZIndex);

			//spriteBatch.End();B
		}

	}
}
