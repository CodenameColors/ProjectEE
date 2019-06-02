using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//lol

namespace ProjectE_E.Components
{
	class Tile
	{
		protected Texture2D texture;

		private Rectangle rectangle;
		public Rectangle Rectangle
		{
			get { return rectangle;}
			protected set { rectangle = value; }
		}

		private static ContentManager content;
		public static ContentManager Content
		{
			protected get { return content; }
			set { content = value; }
		}

		public Tile(Texture2D text, Rectangle r)
		{
			this.texture = text;
			this.rectangle = r;
		}
		
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, rectangle, Color.White);
		}
	}

	class CollisionTiles : Tile
	{
		//public CollisionTiles(int i, Rectangle newRectangle)
		//{
		//	base()
		//	texture = Content.Load<Texture2D>("Images/Tile" + i);
		//	this.Rectangle = newRectangle;
		//}
		public CollisionTiles(Texture2D text, Rectangle r) : base(text, r)
		{

		}
	}

}
