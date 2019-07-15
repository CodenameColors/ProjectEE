using BixBite.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BixBite.Rendering.UI
{
	public class GameUI : IProperties
	{
		public String UIName { get; set; }
		protected ObservableDictionary<string, object> Properties { get; set; }
		private List<GameUI> UIElements = new List<GameUI>();

		public GameUI(String UIName, int Width, int Height, String BackgroundPath = "")
		{
			Properties = new ObservableDictionary<string, object>();

			this.UIName = UIName;
			AddProperty("Width", Width);
			AddProperty("Height", Height);
			AddProperty("Background", BackgroundPath);
		}

		#region Properties
		public void UpdateProperties(Dictionary<String, object> newdict)
		{
			Properties = new ObservableDictionary<string, object>(newdict);
		}

		public void ClearProperties()
		{
			Properties.Clear();
		}

		public void AddProperty(string Pname, object data)
		{
			Properties.Add(Pname, data);
		}


		public ObservableDictionary<string, object> getProperties()
		{
			return Properties;
		}

		public void setProperties(ObservableDictionary<string, object> newprops)
		{
			Properties = newprops;
		}

		public void SetProperty(string PName, object data)
		{
			Properties[PName] = data;
		}

		public object GetProperty(String PName)
		{
			return Properties[PName];
		}
		#endregion

		#region PropertiesCallBack
		public void PropertyCallback(object sender, System.Windows.RoutedEventArgs e)
		{

		}
		#endregion

		public List<GameUI> GetUIelements()
		{
			return UIElements;
		}

		public void AddUIElement(GameUI gameUI)
		{
			UIElements.Add(gameUI);
		}

		public void ClearGameUI()
		{
			UIElements.Clear();
		}

		public void RemoveGameUI(int element)
		{
			if(UIElements.Count > element)
			{
				UIElements.RemoveAt(element);
			}
		}

		public void RemoveGameUI(GameUI gameUI)
		{
			if (UIElements.Contains(gameUI)){
				UIElements.Remove(gameUI);
			}
		}

	}
}
