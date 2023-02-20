using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using BixBite.Rendering.UI.ListBox.ListBoxItems;

namespace BixBite.Rendering.UI.ListBox
{
	public enum EPositionType
	{
		NONE,
		Vertical,
		Horizontal,
		Custom,
	}

	public class BaseListBox : BaseUI
	{

		#region Delegates
		public delegate void SelectRequest_Delegate(int selectedValue);
		public SelectRequest_Delegate SelectRequest_Hook = null;

		#endregion

		#region Fields

		//private int _listBoxRenderPointer;
		//private bool _bShowBorder = true;
		//private bool _isActive = false;

		//private int _keyDownHeldFrames;
		//private int _holdDownFrameLimit;

		protected EPositionType _listBoxEPositionType
		{
			get => (EPositionType)Enum.Parse(typeof(EPositionType), GetPropertyData("ListBoxPositionType").ToString());
			set => SetProperty("ListBoxPositionType", value);
		}

		protected int _SelectedDisplayedIndex
		{
			get => int.Parse(GetPropertyData("SelectedDisplayedIndex").ToString());
			set => SetProperty("SelectedDisplayedIndex", value);
		}

		protected int _spacing
		{
			get => int.Parse(GetPropertyData("Spacing").ToString());
			set => SetProperty("Spacing", value);
		}

		protected int _highlightTextureWidth_offset
		{
			get => int.Parse(GetPropertyData("HighLightTextureWidth_offset").ToString());
			set => SetProperty("HighLightTextureWidth_offset", value);
		}

		protected int _highlightTextureHeight_offset
		{
			get => int.Parse(GetPropertyData("HighLightTextureHeight_offset").ToString());
			set => SetProperty("HighLightTextureHeight_offset", value);
		}


		#endregion

		#region Properties
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

		public int InnerWidth
		{
			get => int.Parse(GetPropertyData("InnerWidth").ToString());
			set => SetProperty("InnerWidth", value);
		}
		public int InnerHeight
		{
			get => int.Parse(GetPropertyData("InnerHeight").ToString());
			set => SetProperty("InnerHeight", value);
		}

		public virtual int SelectedIndex
		{
			get => int.Parse(GetPropertyData("SelectedIndex").ToString());
			set => SetProperty("SelectedIndex", value);
		}
		public int MaxDisplayedItems
		{
			get => int.Parse(GetPropertyData("MaxDisplayedItems").ToString());
			set => SetProperty("MaxDisplayedItems", value);
		}
		public bool bBorder
		{
			get => bool.Parse(GetPropertyData("bBorder").ToString());
			set => SetProperty("bBorder", value);
		}

		public int BorderWidth
		{
			get => int.Parse(GetPropertyData("BorderWidth").ToString());
			set => SetProperty("BorderWidth", value);
		}

		public List<BaseListBoxItem> Items = new List<BaseListBoxItem>();
		#endregion

		#region Contructors

		public BaseListBox(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, int borderW, int spacing,
		int xOff, int yOff, int innerW, int InnerH, int maxDisplayedItems, EPositionType positionType = EPositionType.Vertical) : 
			base(UIName, xPos, yPos, xOff, yOff, width, height, zindex)
		{
			AddProperty("XOffset", xOff);
			AddProperty("YOffset", yOff);
			AddProperty("InnerWidth", innerW);
			AddProperty("InnerHeight", InnerH);

			AddProperty("SelectedIndex", 0);
			AddProperty("SelectedDisplayedIndex", 0);

			AddProperty("bBorder", border);
			AddProperty("BorderWidth", borderW);

			AddProperty("Spacing", spacing);
			AddProperty("HighLightTextureWidth_offset", 0);
			AddProperty("HighLightTextureHeight_offset", 0);

			AddProperty("MaxDisplayedItems", maxDisplayedItems);
			AddProperty("ListBoxPositionType", positionType);

		}

		#endregion

		#region Methods

		#endregion

	}
}
