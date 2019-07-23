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
		public GameIMG(string UIName, int Width, int Height, int Zindex, string BackgroundPath = "") : base(UIName, Width, Height, Zindex, BackgroundPath)
		{
			AddProperty("Xoffset", 0);
			AddProperty("YOffset", 0);
			AddProperty("Image", "");
		}
	}
}
