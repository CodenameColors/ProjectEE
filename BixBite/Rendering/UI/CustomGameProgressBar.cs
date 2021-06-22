using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI
{
	/// <summary>
	/// This is the same as a normal game progress bar. BUT the bar is a texture.
	/// </summary>
	public class CustomGameProgressBar : GameProgressBar
	{
		public CustomGameProgressBar(Texture2D barTexture, string UIName, int Width, int Height, int Zindex, 
			int xoff, int yoff, bool LeftToRight, GraphicsDevice graphicsDevice,
			string BackgroundPath = "#00000000") : 
			base(UIName, Width, Height, Zindex, xoff, yoff, LeftToRight, graphicsDevice, BackgroundPath)
		{
			this.barTexture2D = barTexture;
			this._sourceWidth = barTexture.Width;
		}
	}
}
