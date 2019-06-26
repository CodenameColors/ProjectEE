﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using BixBite.Rendering;
using BixBite.Resources;

namespace BixBite
{
	public class Level : IProperties
	{
		//instance variables
		
		/// <summary>
		/// Item1: TileSetName Item2: TileSet Image Location Item3: TileWidth Item4:TileHeight
		/// </summary>
		public List<Tuple<String, String, int, int>> TileSet = new List<Tuple<string, string, int, int>>();
		public List<Tuple<String, String>> sprites = new  List<Tuple<string, string>>();
		public ObservableCollection<SpriteLayer> Layers { get; set; }


		ObservableDictionary<string, object> Properties
		{
			get; set;
		}

		//public bool bSaved = false; //indicates whether the user has saved their progress.
		//public bool bMainLevel { get; set; } // ONLY ONE CAN BE TRUE in a project. this is the startup level.
		//public int xCells {get; set;}
		//public int yCells { get; set; }

		//keeping this for databinding of tree view scene viewer.
		public String LevelName { get; set; }

		/// <summary>
		/// This  variable stores all the anyonmous methods for event triggers.
		/// </summary>
		//private Dictionary<string, System.Reflection.MethodInfo> EventsLUT = new Dictionary<string, System.Reflection.MethodInfo>();
		public Level()
		{
			Properties = new ObservableDictionary<string, object>();
			Layers = new ObservableCollection<SpriteLayer>();
			Properties.Add("LevelName", "");
			Properties.Add("bMainLevel", false);
			Properties.Add("xCells", 0);
			Properties.Add("yCells", 0);
			Properties.Add("bSaved", false);

		}

		public Level(String desname, bool MainLevel = false)
		{
			LevelName = desname;
			Properties = new ObservableDictionary<string, object>();
			Properties.Add("LevelName", desname);
			Properties.Add("bMainLevel", MainLevel);
			Properties.Add("xCells", 0);
			Properties.Add("yCells", 0);
			Properties.Add("bSaved", false);
			Layers = new ObservableCollection<SpriteLayer>();
		}

		//PROPERTIES 
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

		#region PropertyCallbacks

		public void PropertyTBCallback(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				String PName = ((TextBox)sender).Tag.ToString();
				if (GetProperty(PName) is bool)
				{
					SetProperty("Pname", ((CheckBox)sender).IsChecked);
				}
				else if(GetProperty(PName) is int)
				{
					int num = 0;
					if (Int32.TryParse((((TextBox)sender).Tag.ToString()), out num)) {
						if (PName == "xCells")
						{
							SetProperty(PName, num);
							//TODO:Add hot reload logic for spritelayers
						}
						else if (PName == "yCells")
						{
							SetProperty(PName, num);
							//TODO:Add hot reload logic for spritelayers
						}
					}
				}
				else
				{
					SetProperty(PName, ((TextBox)sender).Text);
				}
				
			}
		}

		public void PropertyCheckBoxCallback(object sender, RoutedEventArgs e)
		{
			String PName = ((CheckBox)sender).Tag.ToString();
			if (GetProperty(PName) is bool)
			{
				
				if(PName == "bMainLevel")
				{
					SetProperty(PName, ((CheckBox)sender).IsChecked);
				}
				else{
					Console.WriteLine("Others... Saved should be enabled= false...");
				}


			}
			
		}

		#endregion

		/// <summary>
		/// This method is here to add a new sprite layer to the current level object
		/// </summary>
		/// <param name="LayerName">The name of this layer</param>
		/// <param name="deslayertype">The type of layer we are adding</param>
		public void AddLayer(String LayerName, LayerType deslayertype)
		{
			int val = 0;
			for (int i = 0; i < Layers.Count; i++)
			{
				if(Layers[i].LayerName == LayerName)
				{
					val++;
					LayerName += val;
					i = 0; //reset. This will make sure to check ALL layers even  already processed ones.
				}
			}
			Layers.Add(new SpriteLayer(deslayertype, this) { LayerName = LayerName }); //add the layer
			Layers[Layers.Count - 1].SetProperty("LayerName", LayerName);
			Layers[Layers.Count - 1].SetProperty("LayerType", deslayertype.ToString());

		}

		/// <summary>
		/// Finds the SpriteLayer in the current levels layers array. 
		/// </summary>
		/// <param name="DesName">The name of the current layer.</param>
		/// <returns>TRUE: index of the SpriteLayer			FALSE: -1</returns>
		public int FindLayerindex(String DesName)
		{
			for(int i =0; i <Layers.Count; i++)
			{
				if(DesName == Layers[i].LayerName)
				{
					return i;
				}
			}
			return -1; //failed. The value doesn't exist.
		}

		/// <summary>
		/// Changes the data of level object and moves it to a different existing layer.
		/// </summary>
		/// <param name="cellx">The desired row of tile array</param>
		/// <param name="celly">The desired column of the tile array</param>
		/// <param name="FromLayer">The current layer </param>
		/// <param name="DesLayer">The desired layer</param>
		/// <param name="DesiredLayerType">What type of layer is the current layer.</param>
		public void ChangeLayer(int cellx, int celly, int FromLayer, int DesLayer, LayerType DesiredLayerType)
		{
			//what type of sprite layer is this?
			if (DesiredLayerType == LayerType.Tile)
			{
				//does this cell contain data?
				int[,] tiledata = (int[,])Layers[FromLayer].LayerObjects;
				if (tiledata[cellx, celly] == 0) return;
				else
				{
					SpriteLayer deslayer = Layers[DesLayer]; //get the desired layer
					((int[,])deslayer.LayerObjects)[cellx, celly] = tiledata[cellx, celly];
					((int[,])Layers[FromLayer].LayerObjects)[cellx, celly] = 0; //clear the data
				}
			}
			else if (DesiredLayerType == LayerType.Sprite)
			{

			}
			else if (DesiredLayerType == LayerType.GameEvent)
			{
			}
		}

		/// <summary>
		/// Moves the data in the array from its original location, to the new location.
		/// </summary>
		/// <param name="cellx">Original Roww number</param>
		/// <param name="celly">Original column number</param>
		/// <param name="shiftx">How many rows to shift the cell over</param>
		/// <param name="shifty">How many columns to shift the cell over</param>
		/// <param name="CurLayer">the int value of the current layer in the array</param>
		/// <param name="layertype">What type of the sprite layer is this layer</param>
		public void Movedata(int cellx, int celly, int shiftx, int shifty, int CurLayer, LayerType layertype)
		{
			//what type of sprite layer is this?
			if (layertype == LayerType.Tile)
			{
				//does this cell contain data?
				int[,] tiledata = (int[,])Layers[CurLayer].LayerObjects;
				if (tiledata[cellx, celly] == 0) return;
				else
				{
					((int[,])Layers[CurLayer].LayerObjects)[cellx + shiftx, celly + shifty] = ((int[,])Layers[CurLayer].LayerObjects)[cellx, celly];
					((int[,])Layers[CurLayer].LayerObjects)[cellx, celly] = 0; //clear the data
				}
			}
			else if (layertype == LayerType.Sprite)
			{

			}
			else if (layertype == LayerType.GameEvent)
			{
			}
		}

		public static Level ImportLevel(String filename)
		{
			Level TempLevel = new Level();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.Async = true;
			//Create the Level object from the file
			using (XmlReader reader = XmlReader.Create(filename, settings))
			{
				while (reader.Read())
				{
					//The first tag should be a level tag. it includes name, and map width/height
					if (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Level")
					{
						Console.WriteLine("Start Element {0}", reader.Name);
						Console.WriteLine(reader.AttributeCount);

						//set properties
						TempLevel.Properties["LevelName"] = reader.GetAttribute("Name");
						TempLevel.LevelName = reader.GetAttribute("Name");
						TempLevel.Properties["bMainLevel"] = (reader.GetAttribute("MainLevel").ToLower() == "false" ? false : true);
						TempLevel.Properties["xCells"] = Int32.Parse(reader.GetAttribute("Width")) / 40;
						TempLevel.Properties["yCells"] = Int32.Parse(reader.GetAttribute("Height")) / 40;

						while (reader.Name.Trim() != "TileSet") //ignore whitespace
							reader.Read();

						//next up is the tilesets for the map.
						while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "TileSet")
						{

							String Name = reader.GetAttribute("Name");
							String Location = reader.GetAttribute("Location");
							int Width = Int32.Parse(reader.GetAttribute("TileWidth"));
							int Height = Int32.Parse(reader.GetAttribute("TileHeight"));
							TempLevel.TileSet.Add(new Tuple<string, string, int, int>(Name, Location, Width, Height));
							reader.Read(); reader.Read();
						}


						while (reader.Name.Trim() != "Sprites" &&  reader.Name.Trim() != "Layers") //ignore whitespace
							reader.Read();

						//next up is the tilesets for the map.
						while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Sprites")
						{
							String Name = reader.GetAttribute("Name");
							String Location = reader.GetAttribute("Location");
							TempLevel.sprites.Add(new Tuple<string, String>(Name, Location));
							reader.Read(); reader.Read();
						}

						//the next thing is the layers . LOOPS
						while ((reader.NodeType == XmlNodeType.EndElement && reader.Name.Trim() != "Layers") || (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Layers")) //loop through all the TileSets
						{

							//create a temp array.//Tile int[,]
							List<List<int>> LevelData = new List<List<int>>();
							reader.Read(); reader.Read(); //move to next element
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "TileLayer")
							{

								TempLevel.AddLayer(reader.GetAttribute("Name"), LayerType.Tile);
								while (reader.Name.Trim() != "Row") //ignore whitespace
									reader.Read();
								while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Row")
								{
									List<int> rowdata = new List<int>();
									reader.Read();
									String s = ( reader.Value);
									String[] row = s.Split(',');
									foreach (string ss in row) //parse the ints
										rowdata.Add(Int32.Parse(ss));
									 reader.Read(); reader.Read();  reader.Read();
									LevelData.Add(rowdata);

								}

							}
							if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Trim() == "TileLayer")
							{
								int[,] str = new int[LevelData.Count, LevelData[0].Count];
								for (int j = 0; j < LevelData.Count; j++)
								{
									for (int i = 0; i < LevelData[j].Count; i++)
									{
										str[j, i] = LevelData[j][i];
									}
								}

								TempLevel.Layers[TempLevel.Layers.Count - 1].LayerObjects = str;//we have the row data so add to the level data

							}

							//Sprite
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "SpriteLayer")
							{
								if (!reader.IsEmptyElement)
								{
									Console.WriteLine("SpriteLayer");
									String SLName = reader.GetAttribute("Name");
									while (reader.Name.Trim() != "Sprite" || (reader.Name.Trim() == "Sprites" && reader.NodeType == XmlNodeType.EndElement)) //ignore whitespace
										reader.Read();

									//we have found a list of sprites we need to parse
									List<Sprite> sprites_ = new List<Sprite>();

									while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Sprite")
									{
										String SpriteName = reader.GetAttribute("Name");
										String SpriteLoc = reader.GetAttribute("Location");
										int w = Int32.Parse(reader.GetAttribute("Width"));
										int h = Int32.Parse(reader.GetAttribute("Height"));
										int x = Int32.Parse(reader.GetAttribute("x"));
										int y = Int32.Parse(reader.GetAttribute("y"));

										sprites_.Add(new Sprite(SpriteName, SpriteLoc, x, y, w, h));
										reader.Read(); reader.Read();
									}
									TempLevel.Layers.Add(new SpriteLayer(LayerType.Sprite, TempLevel));
									TempLevel.Layers[TempLevel.Layers.Count - 1].LayerName = SLName;
									TempLevel.Layers[TempLevel.Layers.Count - 1].LayerObjects = sprites_;
									TempLevel.Layers[TempLevel.Layers.Count - 1].SetProperty("LayerName", SLName);
									TempLevel.Layers[TempLevel.Layers.Count - 1].SetProperty("LayerType", LayerType.Sprite.ToString());
									TempLevel.Layers[TempLevel.Layers.Count - 1].SetProperty("#LayerObjects", sprites_.Count);
									reader.Read();
								}
								else
								{
									reader.Read();
									reader.Read();
								}
							}
							//gameevent int[,]
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "GameEventsLayer")
							{
								Console.WriteLine("GameEventLayer");
								TempLevel.AddLayer(reader.GetAttribute("Name"), LayerType.GameEvent);

								while (reader.Name.Trim() != "Row") //ignore whitespace
									reader.Read();

								while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Row")
								{
									List<int> rowdata = new List<int>();
									reader.Read();
									String s = (reader.Value);
									String[] row = s.Split(',');
									foreach (string ss in row) //parse the ints
										rowdata.Add(Int32.Parse(ss));
									reader.Read(); reader.Read(); reader.Read();
									LevelData.Add(rowdata);
								}

							}
							if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Trim() == "GameEventsLayer")
							{
								int[,] str = new int[LevelData.Count, LevelData[0].Count];
								for (int j = 0; j < LevelData.Count; j++)
								{
									for (int i = 0; i < LevelData[j].Count; i++)
									{
										str[j, i] = LevelData[j][i];
									}
								}

								TempLevel.Layers[TempLevel.Layers.Count - 1].LayerObjects = new Tuple<int[,], List<GameEvent>>(str, new List<GameEvent>());//we have the row data so add to the level data
							}
						}
						//advance
						while (reader.NodeType != XmlNodeType.Element)
							reader.Read();
						//Game events!
						while ((reader.NodeType != XmlNodeType.EndElement && reader.Name.Trim() == "GameEvents") || (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Event")) //loop through all the TileSets
																																																																																									 //while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "GameEvents")
						{
							Console.WriteLine("GameEvents Found");
							reader.Read();
							reader.Read();

							//Create a Event to add to the layers Game events
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Event")
							{
								String GEName = reader.GetAttribute("Name");
								String DeleName = reader.GetAttribute("Function");
								String ActButton = reader.GetAttribute("Activation");
								bool isActive = (reader.GetAttribute("isActive") == "false" ? false : true);
								EventType etype = (EventType)Int32.Parse(reader.GetAttribute("Type"));
								int group = Int32.Parse(reader.GetAttribute("Group"));

								GameEvent ge = new GameEvent(GEName, etype, group);
								ge.SetProperty("DelegateEventName", DeleName);
								ge.SetProperty("ActivationButton", ActButton);
								ge.SetProperty("isActive", isActive);

								//advance
								if (!reader.IsEmptyElement)
								{
									reader.Read();
									reader.Read();
								}

								//do we have data?
								if (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "EventData" && etype == EventType.Collision)
								{
									reader.Read(); reader.Read(); reader.Read(); reader.Read();

								}
								else if (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "EventData")
								{
									String FileToAdd = reader.GetAttribute("FileToLoad");
									int xnew = Int32.Parse(reader.GetAttribute("Newxpos"));
									int ynew = Int32.Parse(reader.GetAttribute("Newypos"));
									int movet = Int32.Parse(reader.GetAttribute("MoveTime"));
									EventData ed = new EventData()
									{
										NewFileToLoad = FileToAdd,
										newx = xnew,
										newy = ynew,
										MoveTime = movet
									};
									ge.datatoload = ed;
								}
								//add to layer object list.
								//TODO: THIS currently will break. Also doesn't add the GE to the right layer...
								((Tuple<int[,], List<GameEvent>>)TempLevel.Layers[TempLevel.Layers.Count - 1].LayerObjects).Item2.Add(ge);

								//advance
								reader.Read();
								reader.Read();
							}
							Console.WriteLine(reader.Value);
						}
					}
				}
			}
			//	break;
			//case XmlNodeType.Text:
			//	Console.WriteLine("Text Node: {0}",
			//					 await reader.GetValueAsync());
			//	break;
			//case XmlNodeType.EndElement:
			//	Console.WriteLine("End Element {0}", reader.Name);
			//	break;
			//default:
			//	Console.WriteLine("Other node {0} with value {1}",
			//									reader.NodeType, reader.Value);
			//	break;
			return TempLevel;

			//Create the Level object from the file

			//The first tag should be a level tag. it includes name, and map width/height

			//next up is the tilesets for the map.

			//the next thing is the layers . LOOPS
			//Tile int[,]
			//Sprite
			//gameevent int[,]

			//GameEvents content includes group number and eventname LOOPS
		}

		async public static System.Threading.Tasks.Task<Level> ImportLevelAsync(String filename)
		{
			Level TempLevel = new Level();

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.Async = true;
			//Create the Level object from the file
			using (XmlReader reader = XmlReader.Create(filename, settings))
			{
				while (await reader.ReadAsync())
				{
					//The first tag should be a level tag. it includes name, and map width/height
					if (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Level")
					{
						Console.WriteLine("Start Element {0}", reader.Name);
						Console.WriteLine(reader.AttributeCount);

						//set properties
						TempLevel.Properties["LevelName"] = reader.GetAttribute("Name");
						TempLevel.LevelName = reader.GetAttribute("Name");
						TempLevel.Properties["bMainLevel"] = (reader.GetAttribute("MainLevel").ToLower() == "false" ? false : true);
						TempLevel.Properties["xCells"] = Int32.Parse(reader.GetAttribute("Width")) / 40;
						TempLevel.Properties["yCells"] = Int32.Parse(reader.GetAttribute("Height")) / 40;

						while (reader.Name.Trim() != "TileSet") //ignore whitespace
							await reader.ReadAsync();

						//next up is the tilesets for the map.
						while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "TileSet")
						{
							
							String Name = reader.GetAttribute("Name");
							String Location = reader.GetAttribute("Location");
							int Width = Int32.Parse(reader.GetAttribute("TileWidth"));
							int Height = Int32.Parse(reader.GetAttribute("TileHeight"));
							TempLevel.TileSet.Add(new Tuple<string, string, int, int>(Name, Location, Width, Height));
							await reader.ReadAsync(); await reader.ReadAsync();
						}

						while (reader.Name.Trim() != "Sprites") //ignore whitespace
							await reader.ReadAsync();

						//next up is the tilesets for the map.
						while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Sprites")
						{
							String Name = reader.GetAttribute("Name");
							String Location = reader.GetAttribute("Location");
							TempLevel.sprites.Add(new Tuple<string,String>(Name, Location));
							await reader.ReadAsync(); await reader.ReadAsync();
						}

						//the next thing is the layers . LOOPS
						while ((reader.NodeType == XmlNodeType.EndElement && reader.Name.Trim() != "Layers") || (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Layers")) //loop through all the TileSets
						{
							
							//create a temp array.//Tile int[,]
							List<List<int>> LevelData = new List<List<int>>();
							await reader.ReadAsync(); await reader.ReadAsync(); //move to next element
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "TileLayer")
							{

								TempLevel.AddLayer(reader.GetAttribute("Name"), LayerType.Tile);
								while (reader.Name.Trim() != "Row") //ignore whitespace
									await reader.ReadAsync();
								while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Row")
								{
									List<int> rowdata = new List<int>();
									await reader.ReadAsync();
									String s = (await reader.GetValueAsync());
									String[] row = s.Split(',');
									foreach (string ss in row) //parse the ints
										rowdata.Add(Int32.Parse(ss));
									await reader.ReadAsync(); await reader.ReadAsync(); await reader.ReadAsync();
									LevelData.Add(rowdata);
									
								}
								
							}
							if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Trim() == "TileLayer")
							{
								int[,] str = new int[LevelData.Count, LevelData[0].Count];
								for (int j = 0; j < LevelData.Count; j++)
								{
									for (int i = 0; i < LevelData[j].Count; i++)
									{
										str[j, i] = LevelData[j][i];
									}
								}

								TempLevel.Layers[TempLevel.Layers.Count - 1].LayerObjects = str;//we have the row data so add to the level data
							}

							//Sprite
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "SpriteLayer")
							{
								if (!reader.IsEmptyElement)
								{
									Console.WriteLine("SpriteLayer");
									while (reader.Name.Trim() != "Sprite" || (reader.Name.Trim() == "Sprites" && reader.NodeType == XmlNodeType.EndElement)) //ignore whitespace
										await reader.ReadAsync();

									//we have found a list of sprites we need to parse
									List<Sprite> sprites_ = new List<Sprite>();

									while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Sprite")
									{
										String SpriteName = reader.GetAttribute("Name");
										String SpriteLoc = reader.GetAttribute("Location");
										int w = Int32.Parse(reader.GetAttribute("Width"));
										int h = Int32.Parse(reader.GetAttribute("Height"));
										int x = Int32.Parse(reader.GetAttribute("x"));
										int y = Int32.Parse(reader.GetAttribute("y"));

										sprites_.Add(new Sprite(SpriteName, SpriteLoc, x, y, w, h));
										await reader.ReadAsync(); await reader.ReadAsync();
									}
									TempLevel.Layers.Add(new SpriteLayer(LayerType.Sprite, TempLevel));
									TempLevel.Layers[TempLevel.Layers.Count - 1].LayerName = reader.GetAttribute("Name");
									TempLevel.Layers[TempLevel.Layers.Count - 1].LayerObjects = sprites_;
									await reader.ReadAsync();
								}
								else
								{
									await reader.ReadAsync();
									await reader.ReadAsync();
								}

							}

							//gameevent int[,]
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "GameEventsLayer")
							{
								Console.WriteLine("GameEventLayer");
								TempLevel.AddLayer(reader.GetAttribute("Name"), LayerType.GameEvent);

								while (reader.Name.Trim() != "Row") //ignore whitespace
									await reader.ReadAsync();

								while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Row")
								{
									List<int> rowdata = new List<int>();
									await reader.ReadAsync();
									String s = (await reader.GetValueAsync());
									String[] row = s.Split(',');
									foreach (string ss in row) //parse the ints
										rowdata.Add(Int32.Parse(ss));
									await reader.ReadAsync(); await reader.ReadAsync(); await reader.ReadAsync();
									LevelData.Add(rowdata);
								}

							}
							if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Trim() == "GameEventsLayer")
							{
								int[,] str = new int[LevelData.Count, LevelData[0].Count];
								for (int j = 0; j < LevelData.Count; j++)
								{
									for (int i = 0; i < LevelData[j].Count; i++)
									{
										str[j, i] = LevelData[j][i];
									}
								}

								TempLevel.Layers[TempLevel.Layers.Count - 1].LayerObjects = new Tuple<int[,], List<GameEvent>>(str, new List<GameEvent>());//we have the row data so add to the level data
							}
						}
						while (reader.NodeType != XmlNodeType.Element)
							await reader.ReadAsync();
						//Game events!
						while ((reader.NodeType != XmlNodeType.EndElement && reader.Name.Trim() == "GameEvents") || (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Event")) //loop through all the TileSets
						//while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "GameEvents")
						{
							Console.WriteLine("GameEvents Found");
							await reader.ReadAsync();
							await reader.ReadAsync();

							//Create a Event to add to the layers Game events
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "Event")
							{
								String GEName = reader.GetAttribute("Name");
								String DeleName = reader.GetAttribute("Function");
								String ActButton = reader.GetAttribute("Activation");
								bool isActive = (reader.GetAttribute("isActive") == "false" ? false : true);
								EventType etype = (EventType)Int32.Parse(reader.GetAttribute("Type"));
								int group = Int32.Parse(reader.GetAttribute("Group"));

								GameEvent ge = new GameEvent(GEName, etype, group);
								ge.SetProperty("DelegateEventName", DeleName);
								ge.SetProperty("ActivationButton", ActButton);
								ge.SetProperty("isActive", isActive);

								//advance
								if (!reader.IsEmptyElement)
								{
									await reader.ReadAsync();
									await reader.ReadAsync();
								}

								//do we have data?
								if(reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "EventData" && etype == EventType.Collision)
								{
									await reader.ReadAsync(); await reader.ReadAsync(); await reader.ReadAsync(); await reader.ReadAsync();

								}
								else if(reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "EventData")
								{
									String FileToAdd = reader.GetAttribute("FileToLoad");
									int xnew = Int32.Parse(reader.GetAttribute("Newxpos"));
									int ynew = Int32.Parse(reader.GetAttribute("Newypos"));
									int movet = Int32.Parse(reader.GetAttribute("MoveTime"));
									EventData ed = new EventData()
									{
										NewFileToLoad = FileToAdd,
										newx = xnew,
										newy = ynew,
										MoveTime = movet
									};
									ge.datatoload = ed;
								}
								//add to layer object list.
								((Tuple<int[,], List<GameEvent>>)TempLevel.Layers[TempLevel.Layers.Count - 1].LayerObjects).Item2.Add(ge);

								//advance
								await reader.ReadAsync();
								await reader.ReadAsync();
							}
							Console.WriteLine(reader.Value);
						}
					}
				}
			}
//	break;
						//case XmlNodeType.Text:
						//	Console.WriteLine("Text Node: {0}",
						//					 await reader.GetValueAsync());
						//	break;
						//case XmlNodeType.EndElement:
						//	Console.WriteLine("End Element {0}", reader.Name);
						//	break;
						//default:
						//	Console.WriteLine("Other node {0} with value {1}",
						//									reader.NodeType, reader.Value);
						//	break;
			return TempLevel;

			//Create the Level object from the file

			//The first tag should be a level tag. it includes name, and map width/height

			//next up is the tilesets for the map.

			//the next thing is the layers . LOOPS
			//Tile int[,]
			//Sprite
			//gameevent int[,]

			//GameEvents content includes group number and eventname LOOPS
		}

		async public void ExportLevel(String FilePath, List<String> TileSets, List<Tuple<int, int>> CellDimen = null)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace
				
			};
			settings.Async = true;
			//settings.NewLineHandling = NewLineHandling.Entitize;

			using (XmlWriter writer = XmlWriter.Create(FilePath, settings))
			{
				//Level attritbutes instance
				await writer.WriteStartElementAsync(null, "Level", null);
				await writer.WriteAttributeStringAsync(null, "Name", null, Properties["LevelName"].ToString());
				await writer.WriteAttributeStringAsync(null, "MainLevel", null, Properties["bMainLevel"].ToString().ToLower());
				await writer.WriteAttributeStringAsync(null, "Width", null, ((int)Properties["xCells"] * 40).ToString());
				await writer.WriteAttributeStringAsync(null, "Height", null, ((int)Properties["yCells"] * 40).ToString());

				for(int i = 0; i < TileSets.Count; i++)
				{ 
					System.Drawing.Image img = System.Drawing.Image.FromFile(TileSets[i]);
					int len = TileSets[i].LastIndexOf('.') - TileSets[i].LastIndexOfAny(new char[] { '/', '\\' });
					String name = TileSets[i].Substring(TileSets[i].LastIndexOfAny(new char[] { '/', '\\' }) + 1, len - 1);
					await writer.WriteStartElementAsync(null, "TileSet", null);
					await writer.WriteAttributeStringAsync(null, "Name", null, name);
					await writer.WriteAttributeStringAsync(null, "Location", null, TileSets[i]);
					await writer.WriteAttributeStringAsync(null, "MapWidth", null, img.Width.ToString());
					await writer.WriteAttributeStringAsync(null, "MapHeight", null, img.Height.ToString());
					await writer.WriteAttributeStringAsync(null, "TileWidth", null, CellDimen[i].Item1.ToString());
					await writer.WriteAttributeStringAsync(null, "TileHeight", null, CellDimen[i].Item2.ToString());
					await writer.WriteEndElementAsync();//end of tile set
				}

				for (int i = 0; i < sprites.Count; i++)
				{
					System.Drawing.Image img = System.Drawing.Image.FromFile(TileSets[i]);
					int len = TileSets[i].LastIndexOf('.') - TileSets[i].LastIndexOfAny(new char[] { '/', '\\' });
					String name = sprites[i].Item2.Substring(sprites[i].Item2.LastIndexOfAny(new char[] { '/', '\\' }) + 1, len - 1);
					await writer.WriteStartElementAsync(null, "Sprites", null);
					await writer.WriteAttributeStringAsync(null, "Name", null, sprites[i].Item2);
					await writer.WriteAttributeStringAsync(null, "Location", null, sprites[i].Item2);
					await writer.WriteEndElementAsync();//end of tile set
				}

				await writer.WriteStartElementAsync(null, "Layers", null);
				
				foreach(SpriteLayer layer in Layers)
				{
					if (Layers.Count == 0) break;
					else if (layer.layerType == LayerType.Tile)
					{
						await writer.WriteStartElementAsync(null, "TileLayer", null);
						await writer.WriteAttributeStringAsync(null, "Name", null, layer.LayerName);

						//level data tile wise
						int[,] tiledata = ((int[,])layer.LayerObjects);
						for (int i = 0; i < tiledata.GetLength(0); i++)
						{
							String rowdata = "";
							for (int j = 0; j < tiledata.GetLength(1); j++)
							{
								rowdata += String.Format("{0},", tiledata[i, j]);
							}
							await writer.WriteStartElementAsync(null, "Row", null);
							await writer.WriteStringAsync(rowdata.Substring(0, rowdata.Length - 1));
							await writer.WriteEndElementAsync(); //end of row
						}

						await writer.WriteEndElementAsync(); //end of tile layer
					}
					else if (layer.layerType == LayerType.Sprite)
					{
						await writer.WriteStartElementAsync(null, "SpriteLayer", null);
						await writer.WriteAttributeStringAsync(null, "Name", null, layer.LayerName);

						//level data tile wise
						List<Sprite> SpriteData = (List<Sprite>)layer.LayerObjects;
						foreach (Sprite sprite in SpriteData)
						{
							await writer.WriteStartElementAsync(null, "Sprite", null);
							await writer.WriteAttributeStringAsync(null, "Name", null, sprite.Name);
							await writer.WriteAttributeStringAsync(null, "Location", null, sprite.ImgPathLocation);
							await writer.WriteAttributeStringAsync(null, "Width", null, sprite.GetProperty("width").ToString());
							await writer.WriteAttributeStringAsync(null, "Height", null, sprite.GetProperty("height").ToString());
							await writer.WriteAttributeStringAsync(null, "x", null, sprite.GetProperty("x").ToString());
							await writer.WriteAttributeStringAsync(null, "y", null, sprite.GetProperty("y").ToString());
							await writer.WriteEndElementAsync();
						}

						await writer.WriteEndElementAsync();
					}
					else if (layer.layerType == LayerType.GameEvent)
					{
						await writer.WriteStartElementAsync(null, "GameEventsLayer", null);
						await writer.WriteAttributeStringAsync(null, "Name", null, layer.LayerName);
						int[,] tiledata = ((Tuple<int[,], List<GameEvent>>)layer.LayerObjects).Item1;
						if (tiledata == null) goto Skiplayer;
						for (int i = 0; i < tiledata.GetLength(0); i++)
						{
							String rowdata = "";
							for (int j = 0; j < tiledata.GetLength(1); j++)
							{
								rowdata += String.Format("{0},", tiledata[i, j]);
							}
							await writer.WriteStartElementAsync(null, "Row", null);
							await writer.WriteStringAsync(rowdata.Substring(0, rowdata.Length - 1));
							await writer.WriteEndElementAsync(); //end of row
						}
						Skiplayer:

						await writer.WriteEndElementAsync();
					}
				}


				await writer.WriteEndElementAsync(); //end of layers

				List<SpriteLayer> gameeventlayers = new List<SpriteLayer>();
				
				foreach (SpriteLayer g in Layers)
				{ 
					if (g.layerType == LayerType.GameEvent)
					gameeventlayers.Add(g);
				}


				await writer.WriteStartElementAsync(null, "GameEvents", null); //list of Level game events
				
				//there will always be a default collision event!
				await writer.WriteStartElementAsync(null, "Event", null);
				await writer.WriteAttributeStringAsync(null, "Type", null, ((int)EventType.Collision).ToString());
				await writer.WriteAttributeStringAsync(null, "Group", null, "-1");
				await writer.WriteAttributeStringAsync(null, "Function", null, "TileCollisionFound"); //this is the function/delegate name
				await writer.WriteStartElementAsync(null, "EventData", null);
				await writer.WriteStringAsync("Default Collision");
				//await writer.WriteAttributeStringAsync(null, "Activiation", null, "[Button]");
				//await writer.WriteStringAsync("Objects to load here");
				await writer.WriteEndElementAsync();
				await writer.WriteEndElementAsync();

				//loop through all the game events layers to AND thier internal game events.
				foreach(SpriteLayer sl in gameeventlayers)
				{
					List<GameEvent> LGE = ((Tuple<int[,], List<GameEvent>>)sl.LayerObjects).Item2;
					foreach(GameEvent ge in LGE)
					{
						await writer.WriteStartElementAsync(null, "Event", null); //create new event
						await writer.WriteAttributeStringAsync(null, "Name", null, ge.EventName);
						switch (ge.eventType)
						{
							//TODO YO ASSHOLE... MAKE A METHOD FOR THIS. FOR THE LOVE GOD ITS DISGUSTING...
							case (EventType.None):
								break;
							case (EventType.Collision):
								await writer.WriteAttributeStringAsync(null, "Type", null, ((int)EventType.Collision).ToString());
								await writer.WriteAttributeStringAsync(null, "Group", null, ge.GetProperty("group").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "isActive", null, ge.GetProperty("isActive").ToString());
								await writer.WriteAttributeStringAsync(null, "Function", null, ge.GetProperty("DelegateEventName").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "Activation", null, "NONE");
								break;
							case (EventType.Trigger):
								await writer.WriteAttributeStringAsync(null, "Type", null, ((int)EventType.Trigger).ToString());
								await writer.WriteAttributeStringAsync(null, "Group", null, ge.GetProperty("group").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "isActive", null, ge.GetProperty("isActive").ToString());
								await writer.WriteAttributeStringAsync(null, "Function", null, ge.GetProperty("DelegateEventName").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "Activation", null, ge.GetProperty("ActivationButton").ToString()); //button needed to activate our delegate
								break;
							case (EventType.LevelTransistion):
								await writer.WriteAttributeStringAsync(null, "Type", null, ((int)EventType.LevelTransistion).ToString());
								await writer.WriteAttributeStringAsync(null, "Group", null, ge.GetProperty("group").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "isActive", null, ge.GetProperty("isActive").ToString());
								await writer.WriteAttributeStringAsync(null, "Function", null, ge.GetProperty("DelegateEventName").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "Activation", null, ge.GetProperty("ActivationButton").ToString()); //button needed to activate our delegate


								await writer.WriteStartElementAsync(null, "EventData", null); //Event Data
								await writer.WriteAttributeStringAsync(null, "Newxpos", null, ge.datatoload.newx.ToString());
								await writer.WriteAttributeStringAsync(null, "Newypos", null, ge.datatoload.newy.ToString());
								await writer.WriteAttributeStringAsync(null, "MoveTime", null, ge.datatoload.MoveTime.ToString());
								await writer.WriteAttributeStringAsync(null, "FileToLoad", null, ge.datatoload.NewFileToLoad);
								await writer.WriteEndElementAsync(); //end of event data

								break;
							case (EventType.DialougeScene):
								await writer.WriteAttributeStringAsync(null, "Type", null, ((int)EventType.DialougeScene).ToString());
								await writer.WriteAttributeStringAsync(null, "Group", null, ge.GetProperty("group").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "isActive", null, ge.GetProperty("isActive").ToString());
								await writer.WriteAttributeStringAsync(null, "Function", null, ge.GetProperty("DelegateEventName").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "Activation", null, ge.GetProperty("ActivationButton").ToString()); //button needed to activate our delegate

								await writer.WriteStartElementAsync(null, "EventData", null); //Event Data
								await writer.WriteAttributeStringAsync(null, "Newxpos", null, ge.datatoload.newx.ToString());
								await writer.WriteAttributeStringAsync(null, "Newypos", null, ge.datatoload.newy.ToString());
								await writer.WriteAttributeStringAsync(null, "MoveTime", null, ge.datatoload.MoveTime.ToString());
								await writer.WriteAttributeStringAsync(null, "FileToLoad", null, ge.datatoload.NewFileToLoad);
								await writer.WriteEndElementAsync(); //end of event data

								break;
							case (EventType.Cutscene):
								await writer.WriteAttributeStringAsync(null, "Type", null, ((int)EventType.Cutscene).ToString());
								await writer.WriteAttributeStringAsync(null, "Group", null, ge.GetProperty("group").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "isActive", null, ge.GetProperty("isActive").ToString());
								await writer.WriteAttributeStringAsync(null, "Function", null, ge.GetProperty("DelegateEventName").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "Activation", null, ge.GetProperty("ActivationButton").ToString()); //button needed to activate our delegate

								await writer.WriteStartElementAsync(null, "EventData", null); //Event Data
								await writer.WriteAttributeStringAsync(null, "Newxpos", null, ge.datatoload.newx.ToString());
								await writer.WriteAttributeStringAsync(null, "Newypos", null, ge.datatoload.newy.ToString());
								await writer.WriteAttributeStringAsync(null, "MoveTime", null, ge.datatoload.MoveTime.ToString());
								await writer.WriteAttributeStringAsync(null, "FileToLoad", null, ge.datatoload.NewFileToLoad);
								await writer.WriteEndElementAsync(); //end of event data

								break;
							case (EventType.BGM):
								await writer.WriteAttributeStringAsync(null, "Type", null, ((int)EventType.BGM).ToString());
								await writer.WriteAttributeStringAsync(null, "Group", null, ge.GetProperty("group").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "isActive", null, ge.GetProperty("isActive").ToString());
								await writer.WriteAttributeStringAsync(null, "Function", null, ge.GetProperty("DelegateEventName").ToString()); //this is the function/delegate name
								await writer.WriteAttributeStringAsync(null, "Activation", null, ge.GetProperty("ActivationButton").ToString()); //button needed to activate our delegate
								break;
						}
						await writer.WriteEndElementAsync(); //end creation of event.
					}
				}


				await writer.WriteEndElementAsync();
				await writer.WriteEndElementAsync();

				await writer.FlushAsync();
			}
		}

		
	}
}
