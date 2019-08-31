using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BixBite.Resources;
using System.Windows.Controls;

namespace BixBite.Rendering
{
	public class Sprite : IProperties
	{

		public delegate void ChangePathLocation(String newpath);
		ChangePathLocation OnChangePathLocation = null;

		public String ImgPathLocation
		{
			get { return imgpathlocation; }
			set
			{
				imgpathlocation = value;
				if (OnChangePathLocation != null)
					OnChangePathLocation(value);
			}
		}
		private String imgpathlocation = "";
		public String Name { get; set; }
		//public int Width, Height;
		//public int xpos, ypos;


		public double X
		{
			get
			{
				return (double)GetProperty("x");
			}
			set
			{
				SetProperty("x", value);
			}
		}
		public double Y
		{
			get
			{
				return (double)GetProperty("y");
			}
			set
			{
				SetProperty("y", value);
			}
		}
		public double Width
		{
			get
			{
				return (double)GetProperty("width");
			}
			set
			{
				SetProperty("width", value);
			}
		}
		public double Height
		{
			get
			{
				return (double)GetProperty("height");
			}
			set
			{
				SetProperty("height", value);
			}
		}

		ObservableCollection<String, object> Properties { get; set; }

		public Sprite(String Name, String imgLoc, int x, int y, int w, int h)
		{
			this.Name = Name;
			this.imgpathlocation = imgLoc;

			Properties = new ObservableCollection<string, object>();
			AddProperty("Name", Name);
			AddProperty("x", x);
			AddProperty("y", y);
			AddProperty("width", w);
			AddProperty("height", h);


		}

		#region Properties
		public void UpdateProperties(Dictionary<String, object> newdict)
		{
			Properties = new ObservableCollection<string, object>(newdict);
		}

		public void ClearProperties()
		{
			Properties.Clear();
		}

		public void AddProperty(string Pname, object data)
		{
			Properties.Add(Pname, data);
		}


		public ObservableCollection<string, object> getProperties()
		{
			return Properties;
		}

		public void setProperties(ObservableCollection<string, object> newprops)
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


		#region PropertyCallbacks

		public void PropertyTBCallback(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				String PName = ((TextBox)sender).Tag.ToString();
				if (GetProperty(PName) is int)
				{
					int num = 0;
					if (Int32.TryParse((((TextBox)sender).Tag.ToString()), out num))
					{
						if (PName == "x" || PName == "y")
						{
							SetProperty(PName, num);
							//TODO:Add hot reload logic for spritelayers
						}
						else if (PName == "width" || PName == "height")
						{
							SetProperty(PName, num);
							//TODO:Add hot reload logic for spritelayers
						}
					}
				}
				else
				{
					SetProperty(PName, ((TextBox)sender).Text);
				}

			}
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
