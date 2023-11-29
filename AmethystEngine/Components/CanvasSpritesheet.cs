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
using ImageCropper;

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

		public int CropX
		{
			get => _cropXPos;
			set
			{
				_cropXPos = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CropX"));
			}
		}

		public int CropY
		{
			get => _cropYPos;
			set
			{
				_cropYPos = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CropY"));
			}
		}

		public int RX
		{
			get => _rx;
			set
			{
				_rx = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RX"));
			}
		}

		public int RY
		{
			get => _ry;
			set
			{
				_ry = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RY"));
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

		public ObservableCollection<CanvasSubLayerPoint> SubLayerPoints = new ObservableCollection<CanvasSubLayerPoint>();
		//public List<Image> SubLayerCrossHairsImages = new List<Image>();

		public float SX { get; set; }
		public float SY { get; set; }
		public Border LinkedBorderImage = null;
		public CroppableImage LinkedCroppableImage = null;
		private int _x;
		private int _y;
		private int _rx;
		private int _ry;
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


		public static CanvasSpritesheet ImportSpriteSheet(String filePath, String contentDirectory)
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
						retSpriteSheet.ImagePath = retSpriteSheet.ImagePath.Replace("{Content}", contentDirectory);

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
								uint numOfFrames = uint.Parse(reader.GetAttribute("NumOfFrames"));
								CanvasAnimation spriteAnimation = new CanvasAnimation(animName){NumOfFrames = numOfFrames };

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
										imageProperty.RX = int.Parse(reader.GetAttribute("RX") ?? "0"); // fallback
										imageProperty.RY = int.Parse(reader.GetAttribute("RY") ?? "0"); // fallback
										imageProperty.CropX = int.Parse(reader.GetAttribute("CX") ?? "0"); // fallback
										imageProperty.CropY = int.Parse(reader.GetAttribute("CY") ?? "0"); // fallback
										imageProperty.ImageLocation = (reader.GetAttribute("Path") ?? ""); // fallback
										imageProperty.ImageLocation = imageProperty.ImageLocation.Replace("{Content}", contentDirectory);

										spriteAnimation.CanvasFrames.Add(imageProperty);

										do
										{
											reader.Read();
											if (reader.Name == "SubLayerPoints" && reader.NodeType == XmlNodeType.Element)
											{
												CanvasSubLayerPoint subLayerPoint = new CanvasSubLayerPoint();
												subLayerPoint.LayerName = reader.GetAttribute("LayerName");
												subLayerPoint.RX = int.Parse(reader.GetAttribute("RX") ?? "0"); // fallback
												subLayerPoint.RY = int.Parse(reader.GetAttribute("RY") ?? "0"); // fallback
												spriteAnimation.CanvasFrames.Last().SubLayerPoints.Add(subLayerPoint);
												if(!spriteAnimation.NamesOfSubLayers.Contains(subLayerPoint.LayerName))
													spriteAnimation.NamesOfSubLayers.Add(subLayerPoint.LayerName);
											}
										} while (reader.Name.Trim() != "Frame");
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


		public static bool ExportSpriteSheet(CanvasSpritesheet desiredSheet, String filepath, String editorContentPath, String mmonogameContentPath)
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

				String finalFilePath = filepath + ".png";
				finalFilePath = finalFilePath.Replace(mmonogameContentPath, "{Content}\\");
				writer.WriteAttributeString(null, "File", null, finalFilePath);	// We are creating this sheet. so we need to use our sent path

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

					writer.WriteStartElement(null, "SubLayers", null);
					foreach (var layerName in anim.NamesOfSubLayers)
					{
						writer.WriteStartElement(null, "Layer", null);
						writer.WriteAttributeString(null, "Name", null, layerName);
						writer.WriteEndElement(); //end of the Layer Tag
					}
					writer.WriteEndElement(); //end of the SubLayers Tag


					// Animation Frames!
					//writer.WriteStartElement(null, "Frames", null);
					foreach (CanvasImageProperties imgpProperty in anim.CanvasFrames)
					{
						writer.WriteStartElement(null, "Frame", null);
						writer.WriteAttributeString(null, "X", null, imgpProperty.X.ToString());
						writer.WriteAttributeString(null, "Y", null, imgpProperty.Y.ToString());
						writer.WriteAttributeString(null, "W", null, imgpProperty.W.ToString());
						writer.WriteAttributeString(null, "H", null, imgpProperty.H.ToString());
						writer.WriteAttributeString(null, "RX", null, imgpProperty.RX.ToString());
						writer.WriteAttributeString(null, "RY", null, imgpProperty.RY.ToString());
						writer.WriteAttributeString(null, "CX", null, imgpProperty.CropX.ToString());
						writer.WriteAttributeString(null, "CY", null, imgpProperty.CropY.ToString());

						finalFilePath = imgpProperty.ImageLocation.ToString();
						finalFilePath = finalFilePath.Replace(editorContentPath, "{Content}");
						writer.WriteAttributeString(null, "Path", null, finalFilePath);

						// Export all the Sub layer points
						writer.WriteStartElement(null, "SubLayerPoints", null);
						foreach (var subLayerPoint in imgpProperty.SubLayerPoints)
						{
							writer.WriteAttributeString(null, "LayerName", null, subLayerPoint.LayerName);
							writer.WriteAttributeString(null, "RX", null, subLayerPoint.RX.ToString());
							writer.WriteAttributeString(null, "RY", null, subLayerPoint.RY.ToString());
						}
						writer.WriteEndElement(); //end of the SubLayerPoints Tag


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
		public ObservableCollection<String> NamesOfSubLayers = new ObservableCollection<string>();

		public CanvasAnimation(String name)
		{
			this.AnimName = name;
		}
	}

	public class CanvasSubLayerPoint : INotifyPropertyChanged
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

		private String _layerName = "";
		private int _rx;
		private int _ry;
		public Image LinkedImage = null;

		public String LayerName
		{
			get => _layerName;
			set
			{
				_layerName = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LayerName"));
			}
		}

		public int RX
		{
			get => _rx;
			set
			{
				_rx = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RX"));
			}
		}

		public int RY
		{
			get => _ry;
			set
			{
				_ry = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RY"));
			}
		}

		public CanvasSubLayerPoint()
		{

		}
	}

}
