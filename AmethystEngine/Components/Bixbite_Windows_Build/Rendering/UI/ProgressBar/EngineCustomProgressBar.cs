using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI.ProgressBar
{
	public class EngineCustomProgressBar : EngineProgressBar
	{
		public EngineCustomProgressBar(string UIName, int xPos, int yPos, int width, int height, int zindex,
			bool border, int borderWidth, int xOff, int yOff, int currentVal, int maxVal, bool bLeftToRight, bool bHori,
			GraphicsDevice graphicsDevice, Color barColor, Color borderColor, Texture2D barTexture)
			: base(UIName, xPos, yPos, width, height, zindex, border, borderWidth, xOff, yOff, currentVal, maxVal, bLeftToRight, bHori)
		{
			//this.barTexture2D = barTexture;
			//this.SourceWidth = barTexture.Width;
		}
	}
}
