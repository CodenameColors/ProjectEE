
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
	class Map
	{
		String MapName;
		private int mapwidth, mapheight;
		private List<Tuple<Texture2D,int>> TileMaps = new List<Tuple<Texture2D,int>>();
		private Dictionary<String, Texture2D> sprites = new Dictionary<string, Texture2D>();
		public Level level = new Level();
		private List<GameEvent> MapEvents;
		List<Tile> MapTiles = new List<Tile>();

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
		//{
		//	get { return collisionTiles; }

		//}
		//private int width, height;

		//public int Width
		//{
		//	get { return width; }
		//}
		//public int Height
		//{
		//	get { return height; }
		//}

		public Map(){ }

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
				if(spriteLayer.layerType == LayerType.Tile)
				{
					GenerateTileMap(((int[,])spriteLayer.LayerObjects), graphicsDevice, spriteBatch);
				}
				else if (spriteLayer.layerType == LayerType.Sprite)
				{
					GenerateSprites(((List<Sprite>)spriteLayer.LayerObjects), graphicsDevice, spriteBatch);
				}
			}
		}

		public void GenerateTileMap(int[,] map, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
		{
			List<int> thresholds = new List<int>();  thresholds.Add(0);
			foreach (Tuple<Texture2D, int> t in TileMaps)
			{
				thresholds.Add(GetTileMapThreshold(t.Item1, t.Item2));
			}
			

			for (int i = 0; i < map.GetLength(1); i++)
			{
				for(int j = 0; j < map.GetLength(0); j++)
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

					MapTiles.Add(new Tile(croppedTexture, new Rectangle(j*40, i*40, 40, 40)));

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
				MapTiles.Add(new Tile(texture, r));
			}
		}


		public async Task<Level> GetLevelFromFile(String FilePath)
		{
			Level l = await Level.ImportLevelAsync(FilePath);
			level = l; 
			return l;
		}

		public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
		{
			foreach (Tile tiles in MapTiles)
				tiles.Draw(spriteBatch);
		}




	}
}
