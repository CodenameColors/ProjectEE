
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite;
using System.IO;
using Microsoft.Xna.Framework;
using BixBite.Rendering;

namespace ProjectE_E.Components
{
	public class Map
	{
		String MapName;
		private int mapwidth, mapheight;
		private List<Tuple<Texture2D,int>> TileMaps = new List<Tuple<Texture2D,int>>();
		private Dictionary<String, Texture2D> sprites = new Dictionary<string, Texture2D>();
		public Level level = new Level();
		private List<GameEvent> MapEvents;
		public List<Tile> MapTiles = new List<Tile>();

		//Look up table of this maps Gameevents
		public Dictionary<int, System.Reflection.MethodInfo> EventLUT = new Dictionary<int, System.Reflection.MethodInfo>();


		public int Width
		{
			get { return mapwidth; }
		}
		public int Height
		{
			get { return mapheight; }
		}
		private List<CollisionTiles> collisionTiles = new List<CollisionTiles>();

		public List<CollisionTiles> CollisionTiles;

		public Map(){

			mapwidth = 1000;
			mapheight = 1000;
		}

		public void LoadTileMaps(GraphicsDevice graphicsDevice, Level currentLevel)
		{
			foreach (Tuple<String, String, int, int> tuplestileset in currentLevel.TileSet)
			{
				Texture2D text;

				using (var stream = new System.IO.FileStream(tuplestileset.Item2, FileMode.Open))
				{
					var texture = Texture2D.FromStream(graphicsDevice, stream);
					text = Texture2D.FromStream(graphicsDevice, stream);
				}
				TileMaps.Add(new Tuple<Texture2D, int>(text, tuplestileset.Item3));
			}
		}

		public void LoadSprites(GraphicsDevice graphicsDevice, Level currentLevel)
		{
			foreach(Tuple<String, String> tuple in currentLevel.sprites)
			{
				Texture2D text;

				using (var stream = new System.IO.FileStream(tuple.Item2, FileMode.Open))
				{
					//using (Bitmap img = (Bitmap)Bitmap.FromStream(stream))
					var texture = Texture2D.FromStream(graphicsDevice, stream);
					text = Texture2D.FromStream(graphicsDevice, stream);
				}
				sprites.Add(tuple.Item2.Replace("\\","/"), text);
			}
		}

		/// <summary>
		/// Returns the list of thresholds for the tiles of the level
		/// The numbers represent the next top left posion/val for the NEXT tileset.
		/// </summary>
		/// <returns></returns>
		public int GetTileMapThreshold(Texture2D TileMap, int tilesize)
		{
			return ((TileMap.Width / tilesize) * (TileMap.Height / tilesize));
		}

		public void GenerateLevel(Level level, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
		{
			foreach(SpriteLayer spriteLayer in level.Layers)
			{
				if (spriteLayer.layerType == LayerType.Tile)
					GenerateTileMap(((int[,])spriteLayer.LayerObjects), graphicsDevice, spriteBatch);
				else if (spriteLayer.layerType == LayerType.Sprite)
					GenerateSprites(((List<Sprite>)spriteLayer.LayerObjects), graphicsDevice, spriteBatch);
				else if (spriteLayer.layerType == LayerType.GameEvent)
					GenerateGameEvents(((Tuple<int[,], List<GameEvent>>)spriteLayer.LayerObjects).Item1, ((Tuple<int[,], List<GameEvent>>)spriteLayer.LayerObjects).Item2);
			}
		}

		public void GenerateTileMap(int[,] map, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
		{
			List<int> thresholds = new List<int>();  thresholds.Add(0);
			foreach (Tuple<Texture2D, int> t in TileMaps)
			{
				thresholds.Add(GetTileMapThreshold(t.Item1, t.Item2));
			}

			Console.WriteLine(map.GetLength(1));
			for (int i = 0; i < map.GetLength(0); i++)
			{
				for(int j = 0; j < map.GetLength(1); j++)
				{
					if (map[i, j] == -1) continue; //this tile is null so don't draw.
					 
					// Get your texture
					 

					 //which tile do i want to crop?
					int rowtilemappos, coltilemappos = 0;
					int tilesetinc = 1;
					int CurTileData = map[i, j];
					

					//what tile set are we currently using?
					while(CurTileData > thresholds[tilesetinc])
					{
						tilesetinc++;
					}

					//get the tile position of the current tile set.
					if (tilesetinc - 1 == 0)
					{
						rowtilemappos = (int)CurTileData / (TileMaps[tilesetinc -1].Item1.Width / TileMaps[tilesetinc - 1].Item2);
						coltilemappos = (int)CurTileData % (TileMaps[tilesetinc - 1].Item1.Height / TileMaps[tilesetinc - 1].Item2);
					}
					else
					{
						rowtilemappos = (thresholds[tilesetinc - 1] - CurTileData) / (TileMaps[tilesetinc - 1].Item1.Width / TileMaps[tilesetinc - 1].Item2);
						coltilemappos = (thresholds[tilesetinc - 1] - CurTileData) % (TileMaps[tilesetinc - 1].Item1.Height / TileMaps[tilesetinc - 1].Item2);
					}

					Texture2D texture = TileMaps[tilesetinc - 1].Item1; //select the correct tile set.

					Rectangle r = new Rectangle();
					r.X = coltilemappos * 32; //left
					r.Y = rowtilemappos * 32; //top
					r.Width = TileMaps[tilesetinc - 1].Item2;
					r.Height = TileMaps[tilesetinc - 1].Item2;

					// Create a new texture of the desired size
					Texture2D croppedTexture = new Texture2D(graphicsDevice, r.Width, r.Height);

					// Copy the data from the cropped region into a buffer, then into the new texture
					Color[] data = new Color[r.Width * r.Height];
					texture.GetData(0, r, data, 0, r.Width * r.Height);
					croppedTexture.SetData(data);

					MapTiles.Add(new Tile(croppedTexture, new Rectangle(j*40, i*40, 40, 40), 0, 0));

					//Tile tileTemp = new Tile(croppedTexture, r);
					//tileTemp.Draw(spriteBatch);

				}
			}
		}

		public void GenerateSprites(List<Sprite> sprites_, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
		{
			foreach(Sprite spr in sprites_)
			{
				Texture2D texture = sprites[spr.ImgPathLocation];
				Rectangle r = new Rectangle();
				r.X = (int)spr.GetProperty("x");
				r.Y = (int)spr.GetProperty("y");
				r.Width = (int)spr.GetProperty("width");
				r.Height = (int)spr.GetProperty("height");
				MapTiles.Add(new Tile(texture, r, 0, 0));
			}
		}

		public void GenerateGameEvents(int[,] GELocations, List<GameEvent> gameEvents)
		{

			List<Tile> GETiles = new List<Tile>();//MapTiles.Where(item => item.EventGroup != 0);
			foreach (Tile t in MapTiles)
			{
				if (t.EventGroup != 0)
					GETiles.Add(t);
			}

			//scan through the 2D array
			for (int i = 0; i < GELocations.GetLength(0); i++)
			{
				for (int j = 0; j < GELocations.GetLength(1); j++)
				{
					// a value of 0 is NOTHING don't create a game event
					if (GELocations[i, j] == 0) continue;
					
					foreach (Tile t in GETiles)
					{
						if (t.Rectangle.X == j * 40 && t.Rectangle.Y == i * 40)
							continue; //this tile already has an event... don't add it.
					}
					
					//create the Tile for game events.
					Rectangle GErect = new Rectangle()
					{
						X = j * 40,
						Y = i * 40,
						Width = 40,
						Height = 40
					};
					int eventnum = 0;
					if (gameEvents.Count > 0)
					{
						foreach(GameEvent g in gameEvents)
						{
							if((int)g.GetProperty("group") == GELocations[i, j])
							{
								eventnum = (int)g.eventType;
									break;
							}
						}
					}
						//eventnum = (int)(gameEvents.Find(t => (int)t.GetProperty("group") == GELocations[i, j]).eventType);

					Tile tt = new Tile(null, GErect, GELocations[i, j], eventnum);
					MapTiles.Add(tt); //update map;
					GETiles.Add(tt); //update checking of map tiles.
				}
			}
		}

		public async Task<Level> GetLevelFromFile(String FilePath)
		{
			Level l = await Level.ImportLevelAsync(FilePath);
			level = l; 
			return l;
		}

		public void ClearEventLUT()
		{
			EventLUT.Clear();
		}

		public void FillDictLUT(List<GameEvent> gameEvents)
		{
			foreach(GameEvent ge in gameEvents)
			{
				if (EventLUT.ContainsKey((int)ge.GetProperty("group"))) continue;
				EventLUT.Add(
					(int)ge.GetProperty("group"),
					Type.GetType("ProjectE_E.Components.Cuprite.MapEvents").GetMethod(ge.GetProperty("DelegateEventName").ToString())
					);
			}		
		}

		public System.Reflection.MethodInfo GetMapEvent(int key)
		{
			if (EventLUT.ContainsKey(key))
			{
				return EventLUT[key];
			}
			return null;
		}

		/// <summary>
		/// Unload the map from screen and memory
		/// </summary>
		public void UnloadMap()
		{
			foreach(Tile tile in MapTiles)
			{
				tile.UnloadTile();
			}
			MapTiles = null;
			GC.Collect(); //clear memory 
		}

		public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
		{
			foreach (Tile tiles in MapTiles)
				tiles.Draw(spriteBatch);
		}




	}
}
