using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI.TabControl
{
	public class BaseTabControl : BaseUI
	{

		#region Fields
		protected int _hoverTabIndex;
		protected int _selectedTabIndex;
		protected Dictionary<int, BaseUI> _UIComponents_Dict = new Dictionary<int, BaseUI>();
		#endregion

		#region Properties
		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set => _selectedTabIndex = value;
		}

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

		public string CheckBoxContentText
		{
			get => GetPropertyData("CheckBoxContentText").ToString();
			set => SetProperty("CheckBoxContentText", value);
		}

		public bool bBorder
		{
			get => bool.Parse(GetPropertyData("bBorder").ToString());
			set => SetProperty("bBorder", value);
		}


		public String FontName
		{
			get => (GetPropertyData("BackgoundColor").ToString());
			set => SetProperty("BackgroundColor", value);
		}
		public int FontSize
		{
			get => int.Parse(GetPropertyData("FontSize").ToString());
			set => SetProperty("FontSize", value);
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

		#endregion

		public BaseTabControl(string UIName, int xPos, int yPos, int xOff, int yOff, int width, int height, int zindex, bool bBorder,
													String backColor, String backImage) 
			: base(UIName, xPos, yPos, width, height, zindex)
		{
			AddProperty("XOffset", xOff);
			AddProperty("YOffset", yOff);

			AddProperty("FontName", "Ariel");
			AddProperty("FontSize", 24);
			AddProperty("FontColor", "Black");
			AddProperty("FontStyle", "Normal");

			AddProperty("bBorder", bBorder);
			AddProperty("BackgroundColor", backColor);
			AddProperty("BackgroundImage", backImage);
		}
	}
}
