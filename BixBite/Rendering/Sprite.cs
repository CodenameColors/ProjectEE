using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BixBite.Resources;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.IO;

namespace BixBite.Rendering
{
	public class Sprite : IProperties
	{
		public delegate void PGridSync_Hook(String Key, object Property,
			System.Collections.Specialized.NotifyCollectionChangedAction action);

		public PGridSync_Hook PGridSync = null;

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
		//public String Name { get; set; }
		//public int Width, Height;
		//public int xpos, ypos;

		public String Name
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "Name"))
				{
					return GetPropertyData("Name").ToString();
				}

				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "Name"))
				{
					SetProperty("Name", value);
				}
				else
				{
					AddProperty("Name", value);
				}
			}
		}

		public double X
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "x"))
				{
					return (int) GetPropertyData("x");
				}

				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "x"))
				{
					SetProperty("x", value);
				}
				else
				{
					AddProperty("x", value);
				}
			}
		}

		public double Y
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "y"))
				{
					return (int) GetPropertyData("y");
				}

				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "y"))
				{
					SetProperty("y", value);
				}
				else
				{
					AddProperty("y", value);
				}
			}
		}

		public double Width
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "width"))
				{
					return (int) GetPropertyData("width");
				}

				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "width"))
				{
					SetProperty("width", value);
				}
				else
				{
					AddProperty("width", value);
				}
			}
		}

		public double Height
		{
			get
			{
				if (Properties.Any(m => m.Item1 == "height"))
				{
					return (int) GetPropertyData("height");
				}

				throw new NullReferenceException(); //doesn't exist
			}
			set
			{
				if (Properties.Any(m => m.Item1 == "height"))
				{
					SetProperty("height", value);
				}
				else
				{
					AddProperty("height", value);
				}
			}
		}

		private float _Alpha = 1.0f;

		public float Alpha
		{
			get => _Alpha;
			set
			{
				_Alpha = value;
			}
		}

		ObservableCollection<Tuple<String, object>> Properties { get; set; }

		public Vector2 Screen_pos;

		public Sprite()
		{
			Properties = new ObservableCollection<Tuple<string, object>>();
			Properties.CollectionChanged += Properties_Changed;

			AddProperty("Name", Name);
			AddProperty("x", 0);
			AddProperty("y", 0);
			AddProperty("width", 0);
			AddProperty("height", 0);

			this.Name = Name;

		}


		public Sprite(String Name, int x, int y, int w, int h)
		{
			Properties = new ObservableCollection<Tuple<string, object>>();
			Properties.CollectionChanged += Properties_Changed;

			

			AddProperty("Name", Name);
			AddProperty("x", x);
			AddProperty("y", y);
			AddProperty("width", w);
			AddProperty("height", h);

			this.Name = Name;

		}

		public Sprite(String Name, String imgLoc, int x, int y, int w, int h)
		{
			Properties = new ObservableCollection<Tuple<string, object>>();
			Properties.CollectionChanged += Properties_Changed;


			AddProperty("Name", Name);
			AddProperty("x", x);
			AddProperty("y", y);
			AddProperty("width", w);
			AddProperty("height", h);


			this.Name = Name;
			this.imgpathlocation = imgLoc;

		}


		#region Properties

		#region IPropertiesImplementation
		public void SetNewProperties(ObservableCollection<Tuple<string, object>> NewProperties)
		{
			Properties = NewProperties;
		}

		public void ClearProperties()
		{
			Properties.Clear();
		}

		public void SetProperty(string Key, object Property)
		{
			if (Properties.Any(m => m.Item1 == Key))
				Properties[GetPropertyIndex(Key)] = new Tuple<string, object>(Key, Property);
			else throw new PropertyNotFoundException(Key);

		}

		public void AddProperty(string Key, object data)
		{
			if (!Properties.Any(m => m.Item1 == Key))
				Properties.Add(new Tuple<String, object>(Key, data));
		}

		public Tuple<String, object> GetProperty(String Key)
		{
			if (Properties.Any(m => m.Item1 == Key))
				return Properties.Single(m => m.Item1 == Key);
			else throw new PropertyNotFoundException();
		}

		public object GetPropertyData(string Key)
		{
			int i = GetPropertyIndex(Key);
			if (-1 == i) throw new PropertyNotFoundException(Key);
			return Properties[i].Item2;
		}

		public ObservableCollection<Tuple<String, object>> GetProperties()
		{
			return Properties;
		}

		#endregion

		#region Helper
		public int GetPropertyIndex(String Key)
		{
			int i = 0;
			foreach (Tuple<String, object> tuple in Properties)
			{
				if (tuple.Item1 == Key)
					return i;
				i++;
			}
			return -1;
		}
		#endregion

		#region PropertiesCallBack
		private void Properties_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Add);
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Remove);
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Replace);
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				if (PGridSync != null)
				{
					foreach (Tuple<String, object> tuple in e.NewItems)
					{
						PGridSync(tuple.Item1, tuple.Item2, System.Collections.Specialized.NotifyCollectionChangedAction.Reset);
					}
				}
			}
		}

		#endregion
		#endregion


		#region PropertyCallbacks

		public void PropertyTBCallback(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				String PName = ((TextBox)sender).Tag.ToString();
				if (GetPropertyData(PName) is int)
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


		protected Texture2D text;

		public void SetTexture(Texture2D text)
		{
			this.text = text;
		}

		public void SetTexture(String TexturePath, GraphicsDevice graphicsDevice)
		{
			try
			{
				using (var stream = new System.IO.FileStream(TexturePath, FileMode.Open))
				{
					var texture = Texture2D.FromStream(graphicsDevice, stream);
					this.text = Texture2D.FromStream(graphicsDevice, stream);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("SPRITE CLASS: SetTexture Failure: {0}", ex.Message);
			}
		}

		public Texture2D getTexture()
		{
			return text;
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw(text, Screen_pos);
		}

		public virtual void Draw_Crop(SpriteBatch sb, int posx, int posy, int x, int y, int w, int h)
		{
			sb.Draw(text, new Vector2(posx, posy), new Rectangle(x, y, w, h), new Color(255, 255, 255, 255), 0.0f, new Vector2(0,0), new Vector2(1,1), SpriteEffects.None, 0   );
		}

		public virtual void Draw_Crop(SpriteBatch sb, int posx, int posy, int x, int y, int w, int h, float alpha)
		{
			sb.Draw(text, new Vector2(posx, posy), new Rectangle(x, y, w, h), Color.White * alpha, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
			this.Alpha = alpha;
		}


		public virtual void Draw_Crop_Scale(SpriteBatch sb, int posx, int posy, int x, int y, int w, int h, double sx, double sy)
		{
			sb.Draw(text, new Vector2(posx, posy), new Rectangle(x, y, w, h), new Color(255,255,255,255) , 0.0f, new Vector2(0, 0), new Vector2((float)sx, (float)sy), SpriteEffects.None, 0);
		}

		public virtual void Draw_Crop_Scale(SpriteBatch sb, int posx, int posy, int x, int y, int w, int h, double sx, double sy, float alpha)
		{
			sb.Draw(text, new Vector2(posx, posy), new Rectangle(x, y, w, h), Color.White * alpha, 0.0f, new Vector2(0, 0), new Vector2((float)sx, (float)sy), SpriteEffects.None, 0);
			this.Alpha = alpha;
		}


	}

}
