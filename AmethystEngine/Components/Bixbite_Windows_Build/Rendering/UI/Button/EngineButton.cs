using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BixBite.Rendering.UI.Button
{
	public class EngineButton : BaseButtonUI
	{
		#region Delegates
		public delegate void PGridSync_Hook(String Key, object Property, System.Collections.Specialized.NotifyCollectionChangedAction action);
		public PGridSync_Hook PGridSync = null;

		#endregion

		#region Fields

		#endregion

		#region Properties Interface CallBacks
		private void Properties_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Add);
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Remove);
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Replace);
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Reset);
					}
				}
			}
		}

		#endregion

		#region Properties
		//public System.Windows.Media.Color FontColor
		//{
		//	get => (GetPropertyData("FontColor").ToString());
		//	set => SetProperty("FontColor", value);
		//}
		public System.Drawing.FontStyle FontStyle
		{
			get
			{
				if(System.Drawing.FontStyle.TryParse(GetPropertyData("FontStyle").ToString(), out System.Drawing.FontStyle val)) return val;
				return System.Drawing.FontStyle.Regular;
			}
			set => SetProperty("FontStyle", value);
		}
		


		#endregion

		#region Contructors
		public EngineButton(string UIName, int xPos, int yPos, int width, int height, int zindex, bool bBorder,
			int xOff, int yOff, String buttonText, String backColor, String backImage
		)
			: base(UIName, xPos, yPos, width, height, zindex, bBorder, xOff, yOff, buttonText, backColor, backImage)
		{
			AddProperty("FontColor", "Black");
			AddProperty("FontStyle", "Normal");
			AddProperty("FontWeight", "Normal");
		}
		#endregion

		#region Methods

		#endregion


	}
}
