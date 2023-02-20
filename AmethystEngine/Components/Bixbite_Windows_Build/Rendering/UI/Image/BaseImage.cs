using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI.Image
{
	public class BaseImage : BaseUI
	{
		#region Delegates

		#endregion

		#region Fields

		#endregion

		#region Properties
		public int XOffset
		{
			get => int.Parse(GetPropertyData("XOffset").ToString());
			set => SetProperty("XOffset", value);
		}

		public int YOffset
		{
			get => int.Parse(GetPropertyData("YOffset").ToString());
			set => SetProperty("YOffset", value);
		}

		public bool bBorder
		{
			get => bool.Parse(GetPropertyData("bBorder").ToString());
			set => SetProperty("bBorder", value);
		}

		/// <summary>
		/// Color in Hex
		/// </summary>
		public String BackgroundColor
		{
			get => (GetPropertyData("BackgoundColor").ToString());
			set => SetProperty("BackgroundColor", value);
		}

		//Relative path from engine project
		public String BackgroundImage
		{
			get => (GetPropertyData("BackgroundImage").ToString());
			set => SetProperty("BackgroundImage", value);
		}

		public float Transparency
		{
			get => float.Parse(GetPropertyData("Transparency").ToString());
			set => SetProperty("Transparency", value);
		}
		#endregion

		#region Contructors

		#endregion

		#region Methods

		#endregion

		public BaseImage(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, 
			int xOff, int yOff, String backImage, String backColor) : 
			base(UIName, xPos, yPos, xOff, yOff, width, height, zindex)
		{
			AddProperty("XOffset", xOff);
			AddProperty("YOffset", yOff);
			AddProperty("Transparency", 1.0);

			AddProperty("bBorder", border);
			AddProperty("BackgroundColor", backColor);
			AddProperty("BackgroundImage", backImage);

		}

		/// <summary>
		/// THIS IS ONLY FOR THE AMETHYST ENGINE IMPORTATION/EXPORT FILES
		/// </summary>
		/// <param name="UIName"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <param name="Zindex"></param>
		/// <param name="xoff"></param>
		/// <param name="yoff"></param>
		/// <param name="ImagePath"></param>
		/// <param name="graphicsDevice"></param>
		/// <param name="BackgroundPath"></param>
		public BaseImage(string UIName, int xPos, int yPos, int Width, int Height, int Zindex, int xoff, int yoff, String ImagePath = "", GraphicsDevice graphicsDevice = null, String BackgroundPath = "#00000000") :
			base(UIName, xPos, yPos, xoff, yoff, Width, Height, Zindex, BackgroundPath)
		{
			AddProperty("Xoffset", xoff);
			AddProperty("YOffset", yoff);
			AddProperty("Image", ImagePath);
			this.graphicsDevice = graphicsDevice;
			if (graphicsDevice != null && ImagePath != "")
				SetUITexture();
		}



	}
}
