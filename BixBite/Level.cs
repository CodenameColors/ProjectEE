using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Xml;
using BixBite.Rendering;

namespace BixBite
{
	public class Level
	{
		//instance variables
		public String LevelName { get; set; }
		/// <summary>
		/// Item1: TileSetName Item2: TileSet Image Location Item3: TileWidth Item4:TileHeight
		/// </summary>
		public List<Tuple<String, String, int, int>> TileSet = new List<Tuple<string, string, int, int>>();
		public ObservableCollection<SpriteLayer> Layers { get; set; }
		public int xCells {get; set;}
		public int yCells { get; set; }
		public Dictionary<String, object> Properties = new Dictionary<string, object>();
		public bool bSaved = false; //indicates whether the user has saved their progress.
		public bool bMainLevel { get; set; } // ONLY ONE CAN BE TRUE in a project. this is the startup level.

		/// <summary>
		/// This  variable stores all the anyonmous methods for event triggers.
		/// </summary>
		private Dictionary<string, System.Reflection.MethodInfo> EventsLUT = new Dictionary<string, System.Reflection.MethodInfo>();


		public Level() { Layers = new ObservableCollection<SpriteLayer>(); }

		public Level(String desname, bool MainLevel = false)
		{
			LevelName = desname;
			bMainLevel = MainLevel;
			Layers = new ObservableCollection<SpriteLayer>();
		}

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
						TempLevel.LevelName = reader.GetAttribute("Name");
						TempLevel.bMainLevel = (reader.GetAttribute("MainLevel").ToLower() == "false" ? false : true);
						TempLevel.xCells = Int32.Parse(reader.GetAttribute("Width")) / 40;
						TempLevel.yCells = Int32.Parse(reader.GetAttribute("Height")) / 40;
						//reader.ReadToNextSibling("TileSet");
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
								Console.WriteLine("SpriteLayer");
							  reader.Read();
							}
							//gameevent int[,]
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "GameEvents")
							{
								Console.WriteLine("GameEventLayer");
								reader.Read();
							}
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
						TempLevel.LevelName = reader.GetAttribute("Name");
						TempLevel.bMainLevel = (reader.GetAttribute("MainLevel").ToLower() == "false" ? false : true);
						TempLevel.xCells = Int32.Parse(reader.GetAttribute("Width")) / 40;
						TempLevel.yCells = Int32.Parse(reader.GetAttribute("Height")) / 40;
						//reader.ReadToNextSibling("TileSet");
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
								Console.WriteLine("SpriteLayer");
								await reader.ReadAsync();
							}
							//gameevent int[,]
							while (reader.NodeType == XmlNodeType.Element && reader.Name.Trim() == "GameEvents")
							{
								Console.WriteLine("GameEventLayer");
								await reader.ReadAsync();
							}
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
				await writer.WriteAttributeStringAsync(null, "Name", null, LevelName);
				await writer.WriteAttributeStringAsync(null, "MainLevel", null, bMainLevel.ToString().ToLower());
				await writer.WriteAttributeStringAsync(null, "Width", null, (xCells * 40).ToString());
				await writer.WriteAttributeStringAsync(null, "Height", null, (yCells * 40).ToString());

				//TileSets
				//foreach(String imgloc in TileSets)
				//{
				for(int i = 0; i < TileSets.Count; i++)
				{ 
					Thread.Sleep(100);
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
							await writer.WriteAttributeStringAsync(null, "Location", null, sprite.PathLocation);
							await writer.WriteAttributeStringAsync(null, "Width", null, sprite.Width.ToString());
							await writer.WriteAttributeStringAsync(null, "Height", null, sprite.Hieght.ToString());
							await writer.WriteAttributeStringAsync(null, "x", null, sprite.Screen_pos.X.ToString());
							await writer.WriteAttributeStringAsync(null, "y", null, sprite.Screen_pos.Y.ToString());
							await writer.WriteEndElementAsync();
						}

						await writer.WriteEndElementAsync();
					}
					else if (layer.layerType == LayerType.GameEvent)
					{
						await writer.WriteStartElementAsync(null, "GameEvents", null);
						if (!(layer.LayerObjects is int[,])) goto Skiplayer;
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
						Skiplayer:

						await writer.WriteEndElementAsync();
					}
				}


				await writer.WriteEndElementAsync(); //end of layers

				await writer.WriteStartElementAsync(null, "GameEvents", null);
				
				await writer.WriteStartElementAsync(null, "Event", null);
				await writer.WriteAttributeStringAsync(null, "Type", null, "EventType");
				await writer.WriteAttributeStringAsync(null, "Group", null, "-1");
				await writer.WriteAttributeStringAsync(null, "Function", null, "EventName"); //this is the function/delegate name
				await writer.WriteStartElementAsync(null, "EventData", null);
				await writer.WriteStringAsync("File To Load");
				//await writer.WriteAttributeStringAsync(null, "Activiation", null, "[Button]");
				//await writer.WriteStringAsync("Objects to load here");
				await writer.WriteEndElementAsync();
				await writer.WriteEndElementAsync();

				await writer.WriteEndElementAsync();
				await writer.WriteEndElementAsync();

				await writer.FlushAsync();
			}
		}
	}
}
