using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI.ListBox
{
	public class EngineCustomListBox : EngineListBox
	{
		#region Delegates
		public delegate void PGridSync_Hook(String Key, object Property, System.Collections.Specialized.NotifyCollectionChangedAction action);
		public PGridSync_Hook PGridSync = null;
		#endregion

		#region Fields

		public String UIDescFile
		{
			get => GetPropertyData("UIFile").ToString();
			set => SetProperty("UIFile", value);
		}
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

		#endregion

		#region Contructors
		public EngineCustomListBox(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border, int borderW, int spacing, 
			int xOff, int yOff, int innerW, int InnerH, int maxDisplayedItems, String uiFile, EPositionType positionType = EPositionType.Vertical) : base(UIName, xPos, yPos, width, height, zindex, border, borderW, spacing, xOff, yOff, innerW, InnerH, maxDisplayedItems, positionType)
		{
			AddProperty("UIFile",UIName);
		}

		#endregion

		#region Methods

		#endregion


		
	}
}
