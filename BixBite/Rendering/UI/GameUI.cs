using BixBite.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace BixBite.Rendering.UI
{
	public class GameUI : IProperties
	{
		public String UIName { get; set; }
		protected ObservableDictionary<string, object> Properties { get; set; }
		public ObservableCollection<GameUI> UIElements { get; set; }

		public GameUI(String UIName, int Width, int Height, int Zindex, String BackgroundPath = "")
		{
			Properties = new ObservableDictionary<string, object>();
			UIElements = new ObservableCollection<GameUI>();
			this.UIName = UIName;
			AddProperty("Name", UIName);
			AddProperty("Width", Width);
			AddProperty("Height", Height);
			AddProperty("Background", BackgroundPath);
			AddProperty("ShowBorder", true);
			AddProperty("Zindex", Zindex);
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
		public virtual void PropertyCallback(object sender, System.Windows.RoutedEventArgs e)
		{
			Console.WriteLine("Main UI Callback");
		}

		public virtual void PropertyCallbackTB(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				//base.PropertyCallback(sender, e);
				Console.WriteLine("GAMETB UI CALLBACK");
				Console.WriteLine(((TextBox)sender).Tag.ToString());

				String Property = ((TextBox)sender).Tag.ToString();
				if (Properties.ContainsKey(Property))
				{
					SetProperty(Property, ((TextBox)sender).Text);
					if (Property == "FontSize")
					{
						//((TextBox)sender).FontSize = Int32.Parse(((TextBox)sender).Text);
					}
				}
			}
		}


		#endregion

		public ObservableCollection<GameUI> GetUIelements()
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

		public void ExportUI(String FilePath)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace

			};
			//settings.Async = true;
			//settings.NewLineHandling = NewLineHandling.Entitize;

			using (XmlWriter writer = XmlWriter.Create(FilePath, settings))
			{
				//create the GameUI (Main)
				writer.WriteStartElement(null, "GameUI", null);
				//all the properties...
				for(int i = 0; i < getProperties().Count; i++)
				{
					writer.WriteAttributeString(null, getProperties().Keys.ToList()[i].ToString(), null, getProperties().Values.ToList()[i].ToString());
				}

				//child UI
				foreach (GameUI childUI in UIElements)
				{
					if(childUI is GameTextBlock)
					writer.WriteStartElement(null, "GameTextBox", null);
					for (int i = 0; i < childUI.getProperties().Count; i++)
					{
						writer.WriteAttributeString(null, childUI.getProperties().Keys.ToList()[i].ToString(), null, childUI.getProperties().Values.ToList()[i].ToString());
					}
					writer.WriteEndElement();//end child UI
				}

				writer.WriteEndElement();//end base ui
			}
		}

		public static GameUI ImportGameUI(String FileName)
		{
			throw new NotImplementedException();
		}

	}
}
