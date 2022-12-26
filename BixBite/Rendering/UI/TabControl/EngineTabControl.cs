using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI.TabControl
{
	public class EngineTabControl : BaseTabControl
	{
		public EngineTabControl(string UIName, int xPos, int yPos, int xOff, int yOff, int width, int height, int zindex, bool bBorder, String backColor, String backImage) : 
			base(UIName, xPos, yPos, xOff, yOff, width, height, zindex, bBorder, backColor, backImage)
		{
		}
	}
}
