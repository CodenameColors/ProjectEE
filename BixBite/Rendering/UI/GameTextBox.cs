using BixBite.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BixBite.Rendering.UI
{
	public class GameTextBox : GameUI, IProperties
	{

		public String TBName { get; set; } 

		public GameTextBox(string UIName, int Width, int Height, int xoff, int yoff, int Zindex ,string BackgroundPath = "", String Text= "") 
			: base(UIName, Width, Height, Zindex ,BackgroundPath)
		{
			TBName = UIName;
			AddProperty("Xoffset", xoff);
			AddProperty("YOffset", yoff);
			AddProperty("ContentText", Text);
			AddProperty("Font", "Ariel");
			AddProperty("FontSize", 24);
			AddProperty("FontColor", "Black");
			AddProperty("FontStyle", "Normal");
			AddProperty("TextSpeed", 1.0);
			AddProperty("TextTime", 1.0);//number of seconds to "type" the text to the screen
		}

		public override void PropertyCallback(object sender, RoutedEventArgs e)
		{

		}




	}
}
