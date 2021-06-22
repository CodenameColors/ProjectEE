using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Resources;

namespace BixBite.Rendering.UI.Button
{
	public class BaseButtonUI : BaseUI
	{

		#region Fields
		protected bool _isHovering;
		#endregion

		#region Properties
		public bool bClicked { get; private set; }

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

		public string ButtonText
		{
			get => GetPropertyData("ButtonText").ToString();
			set => SetProperty("ButtonText", value);
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

		#region Contructors
		public BaseButtonUI(string UIName, int xPos, int yPos, int width, int height, int zindex, bool bBorder,
			int xOff, int yOff, String buttonText, String backColor, String backImage ) 
			: base(UIName, xPos, yPos, width, height, zindex)
		{
			AddProperty("XOffset", xOff);
			AddProperty("YOffset", yOff);
			AddProperty("ButtonText", buttonText);

			AddProperty("FontName", "Ariel");
			AddProperty("FontSize", 24);
			AddProperty("FontColor", "Black");
			AddProperty("FontStyle", "Normal");

			AddProperty("bBorder", bBorder);
			AddProperty("BackgroundColor", backColor);
			AddProperty("BackgroundImage", backImage);
		}
		#endregion

		#region Methods

		#endregion

	}
}
