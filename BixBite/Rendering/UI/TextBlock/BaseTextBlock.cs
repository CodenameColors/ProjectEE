using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI.TextBlock
{
	public class BaseTextBlock : BaseUI
	{
		#region Delegates
		public delegate void PGridSync_Hook(String Key, object Property, System.Collections.Specialized.NotifyCollectionChangedAction action);
		public PGridSync_Hook PGridSync = null;

		#endregion

		#region Fields

		#endregion

		#region Properties
		public bool bMiddleHorizontal = false;
		public bool bMiddleVertical = false;

		public float TextTime
		{
			get => float.Parse(GetPropertyData("TextTime").ToString());
			set => SetProperty("TextTime", value);
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

		public string Text
		{
			get => GetPropertyData("Text").ToString();
			set => SetProperty("Text", value);
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
		public BaseTextBlock(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, int xOff, int yOff,
			String text, float textTime, String backColor, String backImage ) : base(UIName, xPos, yPos, width, height, zindex)
		{
			AddProperty("XOffset", xOff);
			AddProperty("YOffset", yOff);
			AddProperty("Text", text);
			AddProperty("Font", "Ariel");
			AddProperty("FontSize", 12);
			AddProperty("FontColor", "Black");
			AddProperty("FontStyle", "Normal");
			AddProperty("TextSpeed", 1.0);
			AddProperty("TextTime", textTime);//number of seconds to "type" the text to the screen

			AddProperty("bBorder", border);
			AddProperty("BackgroundColor", backColor);
			AddProperty("BackgroundImage", backImage);

		}
		#endregion

		#region Methods

		#endregion

	}
}
