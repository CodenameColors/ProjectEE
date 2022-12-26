using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI.Checkbox
{
	public class EngineCheckBox : BaseCheckBoxUI
	{
		public EngineCheckBox(string UIName, int xPos, int yPos, int xOff, int yOff, int width, int height, int zindex, bool bBorder, 
			string checkBoxContentText, string backColor, string backImage) :
			base(UIName, xPos, yPos, xOff, yOff, width, height, zindex, bBorder, checkBoxContentText, backColor, backImage)
		{
		}
	}
}
