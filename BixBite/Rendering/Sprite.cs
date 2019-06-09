using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering
{
	public class Sprite
	{
		public String ImgPathLocation;
		public String Name { get; set; }
		public int Width, Height;
		public int xpos, ypos;

		public Sprite(String Name, String imgLoc, int x, int y, int w, int h)
		{
			this.Name = Name;
			this.ImgPathLocation = imgLoc;
			this.xpos = x;
			this.ypos = y;
			this.Width = w;
			this.Height = h;
		}

		//protected Texture2D text;

		//public void setTexture(Texture2D text)
		//{
		//	this.text = text;
		//}
		//public Texture2D getTexture()
		//{
		//	return text;
		//}

		//public void Draw(SpriteBatch sb)
		//{
		//	sb.Draw(text, Screen_pos);
		//}
	}
	
}
