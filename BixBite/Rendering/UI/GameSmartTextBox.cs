using BixBite.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI
{
	class GameSmartTextBox : GameUI, IProperties
	{
		public GameSmartTextBox(string UIName, int Width, int Height, int Zindex, string BackgroundPath = "") : base(UIName, Width, Height, Zindex, BackgroundPath)
		{
		}
	}
}
