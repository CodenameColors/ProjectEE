using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.Helpers
{
	public static partial class Utilities
	{
		public static void CreateCircle(this Texture2D texture2D, int radius)
		{
			Color[] colorData = new Color[radius * radius];

			float diam = radius / 2f;
			float diamsq = diam * diam;

			for (int x = 0; x < radius; x++)
			{
				for (int y = 0; y < radius; y++)
				{
					int index = x * radius + y;
					Vector2 pos = new Vector2(x - diam, y - diam);
					if (pos.LengthSquared() <= diamsq)
					{
						colorData[index] = Color.White;
					}
					else
					{
						colorData[index] = Color.Transparent;
					}
				}
			}

			texture2D.SetData(colorData);
		}
	}
}
