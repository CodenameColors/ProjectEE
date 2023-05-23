using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using BixBite.Rendering;
using BixBite.Rendering.Animation;
using BixBite.Resources;
using DrWPF.Windows.Data;

namespace AmethystEngine.Components
{

	public class CanvasImageProperties : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		public String ImageLocation { get; set; }

		public int X
		{
			get => _x;
			set
			{
				_x = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X"));
			}
		}

		public int Y
		{
			get => _y;
			set
			{
				_y = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y"));
			}
		}

		public int W
		{
			get => _w;
			set
			{
				_w = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("W"));
			}
		}

		public int H
		{
			get => _h;
			set
			{
				_h = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("H"));
			}
		}

		public float SX { get; set; }
		public float SY { get; set; }
		public Border LinkedBorderImage = null;
		private int _x;
		private int _y;
		private int _w;
		private int _h;
		private int _cropXPos;
		private int _cropYPos;

		public CanvasImageProperties()
		{

		}

		public CanvasImageProperties(String imageLocation, int width, int height)
		{
			this.ImageLocation = imageLocation;
			this.W = width;
			this.H = height;
		}


	}

	public class CanvasSpritesheet : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		public ObservableCollection<CanvasAnimation> AllAnimationOnSheet
		{
			get => _allCanvasAnimations;
			set
			{
				_allCanvasAnimations = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllAnimationOnSheet"));

			}
		}
		private  ObservableCollection<CanvasAnimation> _allCanvasAnimations = new ObservableCollection<CanvasAnimation>();

		public String Name { get; set; }
		public String ImagePath { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public CanvasSpritesheet(String name, int width, int height)
		{
			this.Name = name;
			this.Width = width;
			this.Height = height;
		}


		public static CanvasSpritesheet ImportSpriteSheet(String filePath)
		{
			CanvasSpritesheet retSpriteSheet = new CanvasSpritesheet("", 0,0);
			XmlReaderSettings settings = new XmlReaderSettings
			{
				//Async = true
			};

			using (XmlReader reader = XmlReader.Create(filePath, settings))
			{
				while (reader.Read()) //Read until you can't
				{
					//skip to a SpriteSheet node
					while (reader.Name != "SpriteSheet")
						reader.Read();
					if (reader.NodeType == XmlNodeType.Element)
					{
						//retSpriteSheet.Name = reader.GetAttribute("Name"); // grab the name
						retSpriteSheet.ImagePath = reader.GetAttribute("File"); // grab the name
						retSpriteSheet.Name = reader.GetAttribute("SheetName");
						retSpriteSheet.Width = int.Parse(reader.GetAttribute("Width") ?? "0");	// fallback
						retSpriteSheet.Height = int.Parse(reader.GetAttribute("Height") ?? "0"); // fallback

						//Skip to the AnimationStates node
						while (reader.Name != "AnimationStates")
							reader.Read();

						//When you get here you SHOULD loop until all animation states have been taken care of
						do
						{
							reader.Read();
							if (reader.Name == "AnimationState" && reader.NodeType == XmlNodeType.Element)
							{
								//If you are here you have found a new animation. So create one to fill with data.
								String animName = (reader.GetAttribute("Name"));
								CanvasAnimation spriteAnimation = new CanvasAnimation(animName);

								//now we need to fill in all the offset/frame position data.
								do
								{
									reader.Read();
									if (reader.Name == "Frame" && reader.NodeType == XmlNodeType.Element)
									{
										CanvasImageProperties imageProperty = new CanvasImageProperties();
										imageProperty.X = int.Parse(reader.GetAttribute("X") ?? "0"); // fallback
										imageProperty.Y = int.Parse(reader.GetAttribute("Y") ?? "0"); // fallback
										imageProperty.W = int.Parse(reader.GetAttribute("W") ?? "0"); // fallback
										imageProperty.H = int.Parse(reader.GetAttribute("H") ?? "0"); // fallback
										imageProperty.ImageLocation = (reader.GetAttribute("Path") ?? ""); // fallback

										spriteAnimation.CanvasFrames.Add(imageProperty);
									}
								} while (reader.Name.Trim() != "AnimationState");

								retSpriteSheet.AllAnimationOnSheet.Add(spriteAnimation);
							}

						} while (reader.Name.Trim() != "AnimationStates");
					}
				}
			}

			return retSpriteSheet;
		}


		public static bool ExportSpriteSheet(CanvasSpritesheet desiredSheet, String filepath)
		{
			bool retbool = false;

			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace

			};
			//settings.Async = true;

			using (XmlWriter writer = XmlWriter.Create(filepath + ".spritesheet", settings))
			{

				writer.WriteStartElement(null, "SpriteSheet", null);
				writer.WriteAttributeString(null, "SheetName", null, desiredSheet.Name);
				writer.WriteAttributeString(null, "File", null, filepath + ".png");	// We are creating this sheet. so we need to use our sent path
				writer.WriteAttributeString(null, "AnimationCount", null, desiredSheet.AllAnimationOnSheet.Count.ToString());
				writer.WriteAttributeString(null, "Width", null, desiredSheet.Width.ToString());
				writer.WriteAttributeString(null, "Height", null, desiredSheet.Height.ToString());

				//Animation States
				writer.WriteStartElement(null, "AnimationStates", null);

				foreach (CanvasAnimation anim in desiredSheet.AllAnimationOnSheet)
				{
					writer.WriteStartElement(null, "AnimationState", null);
					writer.WriteAttributeString(null, "Name", null, anim.AnimName.ToString());
					writer.WriteAttributeString(null, "NumOfFrames", null, anim.CanvasFrames.Count.ToString());

					// Animation Frames!
					//writer.WriteStartElement(null, "Frames", null);
					foreach (CanvasImageProperties imgpProperty in anim.CanvasFrames)
					{
						writer.WriteStartElement(null, "Frame", null);
						writer.WriteAttributeString(null, "X", null, imgpProperty.X.ToString());
						writer.WriteAttributeString(null, "Y", null, imgpProperty.Y.ToString());
						writer.WriteAttributeString(null, "W", null, imgpProperty.W.ToString());
						writer.WriteAttributeString(null, "H", null, imgpProperty.H.ToString());
						writer.WriteAttributeString(null, "Path", null, imgpProperty.ImageLocation.ToString());

						writer.WriteEndElement(); //end of the Frame Tag


						//if (animationEvent is ChangeAnimationEvent changeanim)
						//{
						//	writer.WriteStartElement(null, "AllowedAnimName", null);
						//	writer.WriteAttributeString(null, "Name", null, changeanim.ToAnimationName);
						//	writer.WriteAttributeString(null, "bFinishFirst", null, changeanim.bImmediate.ToString());
						//	writer.WriteEndElement(); //end of the AllowedAnimName Tag
						//}
						//else if (animationEvent is AudioEvent soundanim)
						//{
						//	writer.WriteStartElement(null, "SoundEffect", null);
						//	writer.WriteAttributeString(null, "Value", null, soundanim.SoundEffectName);
						//	writer.WriteAttributeString(null, "FrameStart", null, soundanim.FrameStart.ToString());
						//	writer.WriteAttributeString(null, "FrameEnd", null, soundanim.FrameEnd.ToString());
						//	writer.WriteEndElement(); //end of the SoundEffect Tag
						//}
					}
					// writer.WriteEndElement(); // End of Frames tag
					writer.WriteEndElement(); //end of the AnimationState Tag




				}



				writer.WriteEndElement(); //end of the AnimationStates Tag
				//writer.WriteEndElement(); //end of the SpriteSheet Tag

			}


			return retbool;
		}


	}


	public class CanvasAnimation : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		
		public ObservableCollection<CanvasImageProperties> CanvasFrames
		{
			get => _canvasFrames;
			set
			{
				_canvasFrames = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanvasFrames"));
			}
		}
		private ObservableCollection<CanvasImageProperties> _canvasFrames = new ObservableCollection<CanvasImageProperties>();

		public String AnimName { get; set; }
		public uint NumOfFrames { get; set; }


		public CanvasAnimation(String name)
		{
			this.AnimName = name;
		}
	}
}
