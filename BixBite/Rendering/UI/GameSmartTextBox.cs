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
		public GameSmartTextBox(string UIName, int maxWidth, int Height, int Zindex, string BackgroundPath = "") : base(UIName, maxWidth, Height, Zindex, BackgroundPath)
		{
		}
	}
}
