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
	public class Tile
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

		private int eventgroup;
		public int EventGroup
		{
			get { return eventgroup; }
			set { eventgroup = value; }
		}

		private int eventtype;
		public int EventType
		{
			get { return eventgroup; }
			set { eventgroup = value; }
		}

		public Tile(Texture2D text, Rectangle r, int TileEventGroup, int EventType)
		{
			if(text != null)
				this.texture = text;
			this.rectangle = r;
			this.eventgroup = TileEventGroup;
			this.eventtype = EventType;

		}

		public void UnloadTile()
		{
			texture = null;
			
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if(texture != null)
				spriteBatch.Draw(texture, rectangle, Color.White);
		}
	}

	public class CollisionTiles : Tile
	{
		//public CollisionTiles(int i, Rectangle newRectangle)
		//{
		//	base()
		//	texture = Content.Load<Texture2D>("Images/Tile" + i);
		//	this.Rectangle = newRectangle;
		//}
		public CollisionTiles(Texture2D text, Rectangle r, int TileEventGroup, int EventType) : base(text, r, TileEventGroup, EventType)
		{

		}
	}

}
