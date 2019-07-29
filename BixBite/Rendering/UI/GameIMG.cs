using BixBite.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI
{
	public class GameIMG : GameUI, IProperties
	{
		public GameIMG(string UIName, int Width, int Height, int Zindex, int xoff, int yoff, String ImagePath = "", String BackgroundPath = "") : base(UIName, Width, Height, Zindex, BackgroundPath)
		{
			AddProperty("Xoffset", xoff);
			AddProperty("YOffset", yoff);
			AddProperty("Image", ImagePath);
		}
	}
}
