using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Rendering.UI.ProgressBar
{
	public class EngineProgressBar : BaseProgressBar
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

		#endregion

		#region Contructors
		public EngineProgressBar(string UIName, int xPos, int yPos, int width, int height, int zindex, bool border,
			int borderWidth, int xOff, int yOff, int currentVal, int maxVal, bool bLeftToRight, bool bHori) 
			: base(UIName, xPos, yPos, width, height, zindex, border, borderWidth, xOff, yOff, currentVal, maxVal, bLeftToRight, bHori)
		{
		}
		#endregion

		#region Methods

		#endregion

	}
}
