using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Resources;

namespace BixBite.Rendering.UI
{
	public class BaseUI : BaseUIComponent
	{
		#region Fields
		protected float _scaleX
		{
			get => float.Parse(GetPropertyData("ScaleX").ToString());
			set => SetProperty("ScaleX", value);
		}
		protected  float _scaleY
		{
			get => float.Parse(GetPropertyData("ScaleY").ToString());
			set => SetProperty("ScaleY", value);
		}
		#endregion

		#region Properties
		public ObservableCollection<BaseUI> UIElements { get; set; }

		public String UIName
		{
			get => GetPropertyData("Name").ToString();
			set => SetProperty("Name", value);
		}

		public virtual int XPos
		{
			get => int.Parse(GetPropertyData("XPos").ToString());
			set => SetProperty("XPos", value);
		}

		public virtual int YPos
		{
			get => int.Parse(GetPropertyData("YPos").ToString());
			set => SetProperty("YPos", value);
		}

		public int Width
		{
			get => int.Parse(GetPropertyData("Width").ToString());
			set => SetProperty("Width", value);
		}

		public int Height
		{
			get => int.Parse(GetPropertyData("Height").ToString());
			set => SetProperty("Height", value);
		}

		public float ZIndex
		{
			get
			{
				return ((int)GetProperty("ZIndex").Item2) / 100f;
			}
			set => SetProperty("ZIndex", value);
		}

		#endregion

		#region Properties Interface
		#region IPropertiesImplementation
		public override void SetNewProperties(ObservableCollection<Tuple<string, object>> NewProperties)
		{
			Properties = NewProperties;
		}

		public override void ClearProperties()
		{
			Properties.Clear();
		}

		public override void SetProperty(string Key, object Property)
		{
			if (Properties.Any(m => m.Item1 == Key))
				Properties[GetPropertyIndex(Key)] = new Tuple<string, object>(Key, Property);
			else throw new PropertyNotFoundException(Key);

		}

		public override void AddProperty(string Key, object data)
		{
			if (!Properties.Any(m => m.Item1 == Key))
				Properties.Add(new Tuple<String, object>(Key, data));
		}

		public override Tuple<String, object> GetProperty(String Key)
		{
			if (Properties.Any(m => m.Item1 == Key))
				return Properties.Single(m => m.Item1 == Key);
			else throw new PropertyNotFoundException();
		}

		public override object GetPropertyData(string Key)
		{
			int i = GetPropertyIndex(Key);
			if (-1 == i) throw new PropertyNotFoundException(Key);
			return Properties[i].Item2;
		}

		public override ObservableCollection<Tuple<String, object>> GetProperties()
		{
			return Properties;
		}

		#endregion

		#region Helper
		public override int GetPropertyIndex(String Key)
		{
			int i = 0;
			foreach (Tuple<String, object> tuple in Properties)
			{
				if (tuple.Item1 == Key)
					return i;
				i++;
			}
			return -1;
		}
		#endregion
		#endregion

		#region Contructors

		public BaseUI(String UIName, int xPos, int yPos, int width, int height, int zindex)
		{
			Properties = new ObservableCollection<Tuple<string, object>>();
			UIElements = new ObservableCollection<BaseUI>();
			AddProperty("Name", UIName);
			AddProperty("XPos", xPos);
			AddProperty("YPos", yPos);
			AddProperty("Width", width);
			AddProperty("Height", height);
			AddProperty("ScaleX", 1.0f);
			AddProperty("ScaleY", 1.0f);
			AddProperty("ZIndex", zindex);
		}

		#endregion

		#region Methods
		public ObservableCollection<BaseUI> GetUIelements()
		{
			return UIElements;
		}

		public void AddUIElement(BaseUI gameUI)
		{
			UIElements.Add(gameUI);
		}

		public void ClearGameUI()
		{
			UIElements.Clear();
		}

		public void RemoveGameUI(int element)
		{
			if (UIElements.Count > element)
			{
				UIElements.RemoveAt(element);
			}
		}

		public void RemoveGameUI(BaseUI gameUI)
		{
			if (UIElements.Contains(gameUI))
			{
				UIElements.Remove(gameUI);
			}
		}
		#endregion


	}
}
