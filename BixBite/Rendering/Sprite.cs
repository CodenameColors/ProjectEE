using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BixBite.Resources;

namespace BixBite.Rendering
{
	public class Sprite : IProperties
	{
		public String ImgPathLocation;
		public String Name { get; set; }
		public int Width, Height;
		public int xpos, ypos;

		ObservableDictionary<String, object> Properties { get; set; }

		public Sprite(String Name, String imgLoc, int x, int y, int w, int h)
		{
			this.Name = Name;
			this.ImgPathLocation = imgLoc;
			this.xpos = x;
			this.ypos = y;
			this.Width = w;
			this.Height = h;
		}

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
