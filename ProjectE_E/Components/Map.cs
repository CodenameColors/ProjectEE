
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectE_E.Components
{
	class Map
	{
		private List<CollisionTiles> collisionTiles = new List<CollisionTiles>();

		public List<CollisionTiles> CollisionTiles
		{
			get { return collisionTiles; }

		}
		private int width, height;

		public int Width
		{
			get { return width; }
		}
		public int Height
		{
			get { return height; }
		}

		public Map(){ }

		public void Generate(int[,] map, int size)
		{
			for (int x = 0; x < map.GetLength(1); x++)
			{
				for (int y = 0; y < map.GetLength(0); y++)
				{
					int number = map[y, x];

					if (number > 0)
						collisionTiles.Add(new CollisionTiles(number, new Microsoft.Xna.Framework.Rectangle(x * size, y * size, size, size)));

					width = (x + 1) * 1;
					height = (y + 1) * 1;
				}
			}
		}

		public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
		{
			foreach (CollisionTiles tiles in collisionTiles)
				tiles.Draw(spriteBatch);
		}




	}
}
