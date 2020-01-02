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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI
{
	public class GameUI : UIComponent, IProperties
	{
		public delegate void PGridSync_Hook(String Key, object Property, System.Collections.Specialized.NotifyCollectionChangedAction action);
		public PGridSync_Hook PGridSync = null;

		public String UIName { get; set; }
		protected ObservableCollection<Tuple<string, object>> Properties { get; set; }
		public ObservableCollection<GameUI> UIElements { get; set; }

		public GameUI(String UIName, int Width, int Height, int Zindex, String BackgroundPath = "#00000000")
		{
			Properties = new ObservableCollection<Tuple<string, object>>();
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

		#region IPropertiesImplementation
		public void SetNewProperties(ObservableCollection<Tuple<string, object>> NewProperties)
		{
			Properties = NewProperties;
		}

		public void ClearProperties()
		{
			Properties.Clear();
		}

		public void SetProperty(string Key, object Property)
		{
			if (Properties.Any(m => m.Item1 == Key))
				Properties[GetPropertyIndex(Key)] = new Tuple<string, object>(Key, Property);
			else throw new PropertyNotFoundException(Key);

		}

		public void AddProperty(string Key, object data)
		{
			if (!Properties.Any(m => m.Item1 == Key))
				Properties.Add(new Tuple<String, object>(Key, data));
		}

		public Tuple<String, object> GetProperty(String Key)
		{
			if (Properties.Any(m => m.Item1 == Key))
				return Properties.Single(m => m.Item1 == Key);
			else throw new PropertyNotFoundException();
		}

		public object GetPropertyData(string Key)
		{
			int i = GetPropertyIndex(Key);
			if (-1 == i) throw new PropertyNotFoundException(Key);
			return Properties[i].Item2;
		}

		public ObservableCollection<Tuple<String, object>> GetProperties()
		{
			return Properties;
		}

		#endregion

		#region Helper
		public int GetPropertyIndex(String Key)
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

		#region PropertiesCallBack
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
				if (Properties.Any(m => m.Item1 == Property))
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
				for(int i = 0; i < GetProperties().Count; i++)
				{
					writer.WriteAttributeString(null, GetProperties().Select(M => M.Item1).ToList()[i].ToString(), null, GetProperties().Select(m=>m.Item2).ToList()[i].ToString());
				}

				//child UI
				foreach (GameUI childUI in UIElements)
				{
					if(childUI is GameTextBlock)
						writer.WriteStartElement(null, "GameTextBlock", null);
					if (childUI is GameIMG)
						writer.WriteStartElement(null, "GameIMG", null);
					for (int i = 0; i < childUI.GetProperties().Count; i++)
					{
						writer.WriteAttributeString(null, childUI.GetProperties().Select(m => m.Item1).ToList()[i].ToString(), null, childUI.GetProperties().Select(m => m.Item2).ToList()[i].ToString());
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

		public virtual void SetUITexture()
		{
			throw new NotImplementedException();
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (GameUI gameUi in UIElements)
			{
				gameUi.Draw(gameTime, spriteBatch);
			}
		}

		public override void Update(GameTime gameTime)
		{
			//throw new NotImplementedException();
		}
	}
}


//AddProperty("Name", UIName);
//AddProperty("Width", Width);
//AddProperty("Height", Height);
//AddProperty("Background", BackgroundPath);
//AddProperty("ShowBorder", true);
//AddProperty("Zindex", Zindex);