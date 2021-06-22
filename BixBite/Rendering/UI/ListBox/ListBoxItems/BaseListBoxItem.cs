using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI.ListBox.ListBoxItems
{
	public class BaseListBoxItem : BaseUI
	{
		#region Delegates

		#endregion

		#region Fields

		#endregion

		#region Properties
		public List<BaseUI> Controls = new List<BaseUI>();
		public BaseListBox ParentListBox;

		public bool bCanSelect
		{
			get => bool.Parse(GetPropertyData("bCanSelect").ToString());
			set => SetProperty("bCanSelect", value);
		}

		public bool bIsSelected
		{
			get => bool.Parse(GetPropertyData("bCanSelect").ToString());
			set => SetProperty("bCanSelect", value);
		}
		public bool bBorder
		{
			get => bool.Parse(GetPropertyData("bBorder").ToString());
			set => SetProperty("bBorder", value);
		}

		#endregion

		#region Contructors

		public BaseListBoxItem(BaseListBox parentListBox ,string UIName, int xPos, int yPos, int width, int height,
			int zindex, bool border, bool bCanSelect)
			: base(UIName, xPos, yPos, width, height, zindex)
		{
			this.ParentListBox = parentListBox;

			AddProperty("bBorder", border);
			AddProperty("bIsSelected", bCanSelect);
			AddProperty("bIsSelected", false);

		}

		#endregion

		#region Methods

		#endregion


	}
}
