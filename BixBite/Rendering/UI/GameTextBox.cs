using BixBite.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI
{
	public class GameTextBox : GameUI, IProperties
	{
		public GameTextBox(string UIName, int Width, int Height, int xoff, int yoff, string BackgroundPath = "", String Text= "") 
			: base(UIName, Width, Height, BackgroundPath)
		{
			AddProperty("Xoffset", xoff);
			AddProperty("YOffset", yoff);
			AddProperty("ContentText", Text);
			AddProperty("TextSpeed", 1.0);
			AddProperty("TextTime", 1.0);//number of seconds to "type" the text to the screen
		}



	}
}
