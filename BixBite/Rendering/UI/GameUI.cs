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
		protected ObservableCollection<string, object> Properties { get; set; }
		public ObservableCollection<GameUI> UIElements { get; set; }

		public GameUI(String UIName, int Width, int Height, int Zindex, String BackgroundPath = "#00000000")
		{
			Properties = new ObservableCollection<string, object>();
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
			Properties = new ObservableCollection<string, object>(newdict);
		}

		public void ClearProperties()
		{
			Properties.Clear();
		}

		public void AddProperty(string Pname, object data)
		{
			Properties.Add(Pname, data);
		}


		public ObservableCollection<string, object> getProperties()
		{
			return Properties;
		}

		public void setProperties(ObservableCollection<string, object> newprops)
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
						writer.WriteStartElement(null, "GameTextBlock", null);
					if (childUI is GameIMG)
						writer.WriteStartElement(null, "GameIMG", null);
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
			//Create our return GameUI
			GameUI retGameUI = null;

			XmlReaderSettings settings = new XmlReaderSettings
			{
				//Async = true
			};
			//read the UI File.
			using (XmlReader reader  = XmlReader.Create(FileName, settings))
			{
				while (reader.Read())
				{
					//skip to a GameUI node
					while (reader.Name != "GameUI")
						reader.Read();

					//by this time we should have found a Game UI node use this to create the ACUTAL object
					retGameUI = new GameUI(reader.GetAttribute("Name"), Int32.Parse(reader.GetAttribute("Width")), 
						Int32.Parse(reader.GetAttribute("Height")), Int32.Parse(reader.GetAttribute("Zindex")));

					//After creating the Initial BASE object get all the child UI objects!
					while(reader.NodeType != XmlNodeType.EndElement && reader.Name != " GameUI" && reader.Read())
					{//There are MULTIPLE different types of UI
						//Texblock
						if(reader.Name == "GameTextBlock" && reader.NodeType == XmlNodeType.Element)
						{
							//get the attributes 
							String Name = reader.GetAttribute("Name");
							String Background = reader.GetAttribute("Background");
							String ContentText = reader.GetAttribute("ContentText");
							bool showBorder = (reader.GetAttribute("ShowBorder") == "True" ? true : false);
							int width = Int32.Parse(reader.GetAttribute("Width"));
							int height = Int32.Parse(reader.GetAttribute("Height"));
							int zindex = Int32.Parse(reader.GetAttribute("Zindex"));
							int xoffset = Int32.Parse(reader.GetAttribute("Xoffset"));
							int yoffset = Int32.Parse(reader.GetAttribute("YOffset"));

							GameTextBlock childUI = new GameTextBlock(Name, width, height, xoffset, yoffset, zindex, Background, ContentText);
							childUI.SetProperty("ShowBorder", showBorder);
							childUI.SetProperty("Image", reader.GetAttribute("Image"));
							childUI.SetProperty("Font", reader.GetAttribute("Font"));
							childUI.SetProperty("FontSize", Int32.Parse(reader.GetAttribute("FontSize")));
							childUI.SetProperty("FontColor", reader.GetAttribute("FontColor"));
							childUI.SetProperty("FontStyle", reader.GetAttribute("FontStyle"));
							childUI.SetProperty("TextSpeed", Int32.Parse(reader.GetAttribute("TextSpeed")));
							childUI.SetProperty("TextTime", Int32.Parse(reader.GetAttribute("TextTime")));
							retGameUI.AddUIElement(childUI);
						}
						//Image Boxes
						else if (reader.Name == "GameIMG" && reader.NodeType == XmlNodeType.Element)
						{
							//get the attributes 
							String Name = reader.GetAttribute("Name");
							String Background = reader.GetAttribute("Background");
							String Image = reader.GetAttribute("Image");
							bool showBorder = (reader.GetAttribute("ShowBorder") == "True" ? true : false);
							int width = Int32.Parse(reader.GetAttribute("Width"));
							int height = Int32.Parse(reader.GetAttribute("Height"));
							int zindex = Int32.Parse(reader.GetAttribute("Zindex"));
							int xoffset = Int32.Parse(reader.GetAttribute("Xoffset"));
							int yoffset = Int32.Parse(reader.GetAttribute("YOffset"));

							GameIMG childUI = new GameIMG(Name, width, height, zindex, xoffset, yoffset, Image, Background);
							retGameUI.AddUIElement(childUI);
						}
						//Buttons WIP still
						else if (reader.Name == "GameButton" && reader.NodeType == XmlNodeType.Element)
						{
							//GameButton childUI = new GameButton()
							retGameUI = null;
						}
					}
				}
			}
			return retGameUI;
		}

	}
}


//AddProperty("Name", UIName);
//AddProperty("Width", Width);
//AddProperty("Height", Height);
//AddProperty("Background", BackgroundPath);
//AddProperty("ShowBorder", true);
//AddProperty("Zindex", Zindex);