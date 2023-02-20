using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BixBite.Rendering.UI.TextBlock;
using BixBite.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.UI
{
	public class BaseUI : BaseUIComponent
	{
		public delegate void PGridSync_Hook(String Key, object Property, System.Collections.Specialized.NotifyCollectionChangedAction action);
		public PGridSync_Hook PGridSync = null;

		protected GraphicsDevice graphicsDevice;

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

		public BaseUI(String UIName, int xPos, int yPos, int xoff, int yoff, int width, int height, int zindex)
		{
			Properties = new ObservableCollection<Tuple<string, object>>();
			UIElements = new ObservableCollection<BaseUI>();
			AddProperty("Name", UIName);
			AddProperty("XPos", xPos);
			AddProperty("YPos", yPos);
			AddProperty("Xoffset", xoff);
			AddProperty("YOffset", yoff);
			AddProperty("Width", width);
			AddProperty("Height", height);
			AddProperty("ScaleX", 1.0f);
			AddProperty("ScaleY", 1.0f);
			AddProperty("ZIndex", zindex);
			AddProperty("ShowBorder", true);
		}	

		/// <summary>
		/// THIS CONSTRUCTOR IS HERE FOR AMETHYST ENGINE IMPORTATION ONLY
		/// </summary>
		/// <param name="UIName">Name of UI</param>
		/// <param name="Width">Width of UI</param>
		/// <param name="Height">Height of UI</param>
		/// <param name="Zindex">ZIndex of UI</param>
		public BaseUI(String UIName, int xPos, int yPos, int xOffsetPos, int yXOffsetPos, int Width, int Height, int Zindex, String BackgroundPath = "#00000000")
		{
			Properties = new ObservableCollection<Tuple<string, object>>();
			UIElements = new ObservableCollection<BaseUI>();
			// this.UIName = UIName;
			
			AddProperty("Name", UIName);
			AddProperty("XPos", xPos);
			AddProperty("YPos", yPos);
			AddProperty("Width", Width);
			AddProperty("Height", Height);
			AddProperty("Xoffset", xOffsetPos);
			AddProperty("YOffset", yXOffsetPos);
			AddProperty("BackgroundColor", BackgroundPath);
			AddProperty("ShowBorder", true);
			AddProperty("ZIndex", Zindex);
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
				foreach (BaseUI childUI in UIElements)
				{
					if(childUI is GameTextBlock)
						writer.WriteStartElement(null, "GameTextBlock", null);
					if (childUI is Image.GameImage)
						writer.WriteStartElement(null, "GameIMG", null);
					for (int i = 0; i < childUI.GetProperties().Count; i++)
					{
						if(childUI.GetProperties().Select(m => m.Item2).ToList()[i] != null)
							writer.WriteAttributeString(null, childUI.GetProperties().Select(m => m.Item1).ToList()[i].ToString(), null, childUI.GetProperties().Select(m => m.Item2).ToList()[i].ToString());
						else
							writer.WriteAttributeString(null, childUI.GetProperties().Select(m => m.Item1).ToList()[i].ToString(), null, "null");

					}
					writer.WriteEndElement();//end child UI
				}

				writer.WriteEndElement();//end base ui
			}
		}

		public static BaseUI ImportBaseUI(String FileName)
		{
			//Create our return GameUI
			BaseUI retGameUI = null;

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
					retGameUI = new BaseUI(reader.GetAttribute("Name"), 0,0,0,0, Int32.Parse(reader.GetAttribute("Width")), 
						Int32.Parse(reader.GetAttribute("Height")), Int32.Parse(reader.GetAttribute("ZIndex")));

					//After creating the Initial BASE object get all the child UI objects!
					while(reader.NodeType != XmlNodeType.EndElement && reader.Name != " GameUI" && reader.Read())
					{//There are MULTIPLE different types of UI
						//Texblock
						if(reader.Name == "GameTextBlock" && reader.NodeType == XmlNodeType.Element)
						{
							//get the attributes 
							String Name = reader.GetAttribute("Name");
							String Background = reader.GetAttribute("BackgroundColor");
							String ContentText = reader.GetAttribute("Text");
							bool showBorder = (reader.GetAttribute("ShowBorder") == "True" ? true : false);
							int width = Int32.Parse(reader.GetAttribute("Width"));
							int height = Int32.Parse(reader.GetAttribute("Height"));
							int zindex = Int32.Parse(reader.GetAttribute("ZIndex"));
							int xoffset = Int32.Parse(reader.GetAttribute("Xoffset"));
							int yoffset = Int32.Parse(reader.GetAttribute("YOffset"));

							GameTextBlock childUI = new GameTextBlock(Name,0,0 , width, height, zindex, false,
								xoffset, yoffset, ContentText, 0.0f, Background, "", null,null, Color.Black);
							childUI.SetProperty("ShowBorder", showBorder);
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
							String Background = reader.GetAttribute("BackgroundColor");
							String Image = reader.GetAttribute("Image");
							bool showBorder = (reader.GetAttribute("ShowBorder") == "True" ? true : false);
							int width = Int32.Parse(reader.GetAttribute("Width"));
							int height = Int32.Parse(reader.GetAttribute("Height"));
							int zindex = Int32.Parse(reader.GetAttribute("ZIndex"));
							int xoffset = Int32.Parse(reader.GetAttribute("Xoffset"));
							int yoffset = Int32.Parse(reader.GetAttribute("YOffset"));

							// TODO: Give the file X and Y POS
							Image.GameImage childUI = new Image.GameImage(Name, 0,0, width, height, zindex, xoffset, yoffset, Image, null, Background);
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

		public void AddInterpolationMovement(Tweening.Tweening tween)
		{
			interpolationMovement.Add(tween);
		}

		public virtual void Update(GameTime gameTime)
		{
			for (int i = interpolationMovement.Count - 1; i >= 0; i--)
			{
				if (interpolationMovement[i].bIsDone)
					interpolationMovement.Remove(interpolationMovement[i]);
				else
					interpolationMovement[i].Update(gameTime);
			}
		}


		#endregion


	}
}
