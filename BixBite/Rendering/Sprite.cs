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
		public Vector2 Screen_pos;
		public Vector2 Screen_scale;
		protected Texture2D text;

		public void setTexture(Texture2D text)
		{
			this.text = text;
		}
		public Texture2D getTexture()
		{
			return text;
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw(text, Screen_pos);
		}
	}
	
}
