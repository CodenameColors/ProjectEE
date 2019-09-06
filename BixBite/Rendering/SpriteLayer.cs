using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.SceneObject;
using BixBite.Effects;
using BixBite.Resources;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;

namespace BixBite.Rendering
{
	//This allows this class to have multiple different types/applications.
	public enum LayerType
	{
		None,
		Tile,			//used for background/tile art
		Sprite,		//used for sprites, NOT tile
		GameEvent		//used for location to indicate script triggers. 
	};

	public class SpriteLayer : IProperties
	{

		public delegate void PGridSync_Hook(String Key, object Property, System.Collections.Specialized.NotifyCollectionChangedAction action);
		public PGridSync_Hook PGridSync = null;

		//instance variables
		public String LayerName { get; set; }
		public LayerType layerType = LayerType.None;

		private ObservableCollection<Tuple<String, object>> Properties = new ObservableCollection<Tuple<string, object>>();
		public object LayerObjects = new object(); //contains the objects for this layer. 
		ImageEffect layereffect = new ImageEffect(); //and image effect that will effect THE WHOLE layer. So windDistort for example.
		public Level ParentLevel;

		
		public SpriteLayer()
		{
		}

		public SpriteLayer(LayerType desltype, Level Parent)
		{
			DefineLayerDataType(layerType = desltype); //set the objectdata datatype
			ParentLevel = Parent;

			Properties = new ObservableCollection<Tuple<string, object>>();
			Properties.CollectionChanged += Properties_Changed;
			AddProperty("LayerName", "");
			AddProperty("#LayerObjects", 0);
			AddProperty("LayerType", "");

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

		public void PropertyTBCallback(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				String PName = ((TextBox)sender).Tag.ToString();
				if (GetPropertyData(PName) is String)
				{
					SetProperty(PName, ((TextBox)sender).Text);
				}

			}
		}

		public void PropertyCheckBoxCallback(object sender, RoutedEventArgs e)
		{
			String PName = ((CheckBox)sender).Tag.ToString();
			if (GetPropertyData(PName) is bool)
			{

				if (PName == "bMainLevel")
				{
					SetProperty(PName, ((CheckBox)sender).IsChecked);
				}
				else
				{
					Console.WriteLine("Others... Saved should be enabled= false...");
				}


			}

		}

		#endregion


		/// <summary>
		/// This method allows the "hot reloading" of the tile grids MAX size
		/// This method allows the "hot reloading" of the tile grids MAX size
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public int[,] ResizeLayerBounds(int x, int y)
		{
			if (layerType != LayerType.Tile) return null; //only is allowed on TILE sprite layers.
			int[,] temparr = new int[x,y];
			LayerObjects = temparr;
			return null;
		}

		/// <summary>
		/// Used to define what hte structure that will hold the Layer data. 
		/// ALSO used to hot reset the size of the tile grid
		/// </summary>
		public void DefineLayerDataType(LayerType desltype, int width = 0, int height = 0)
		{
			switch (layerType)
			{
				case (LayerType.Tile):
					LayerObjects = new int[height, width];
					for(int i = 0; i < height; i++)
					{
						for(int j = 0; j < width; j++)
						{
							((int[,])LayerObjects)[i, j] = -1;
						}
					}
					break;
				case (LayerType.Sprite):
					LayerObjects = new List<Sprite>();
					break;
				case (LayerType.GameEvent):
					LayerObjects = new Tuple<int[,], List<GameEvent>>(new int[height, width], new List<GameEvent>());
					break;
			}
		}

		/// <summary>
		/// Add objects to the current sprite layer depending on type.
		/// </summary>
		/// <param name="newLayerObject">Desired object to add.</param>
		public void AddToLayer(int xcell = 0, int ycell = 0, int tiledata = 0)
		{
			Console.WriteLine(String.Format("{0},{1}",xcell, ycell));
			if (LayerObjects is Array && ((Array)LayerObjects).Rank == 2)
			{
				if(((Array)LayerObjects).GetLength(0) >= xcell && ((Array)LayerObjects).GetLength(1) >= ycell)
					((Array)LayerObjects).SetValue(tiledata, xcell, ycell);
			}
			else Console.WriteLine("Invalid defined Layerobject type. Not a List of Tiles");
			return;
		}
	

		public void AddToLayer(String SpriteName, String imglogc, int x, int y, int w, int h)
		{
			if (layerType == LayerType.None)
				throw new SpriteLayerException(LayerType.None);
			if (layerType == LayerType.Sprite) {
				if (LayerObjects is List<Sprite>)
					((List<Sprite>)LayerObjects).Add(new Sprite(SpriteName, imglogc, x, y, w, h));
				else Console.WriteLine("incorrect layer type!");
			return;
			}
		}

		public void AddToLayer(Sprite s)
		{
			if (layerType == LayerType.None)
				throw new SpriteLayerException(LayerType.None);
			if (layerType == LayerType.Sprite)
			{
				if (LayerObjects is List<Sprite>)
					((List<Sprite>)LayerObjects).Add(s);
				else Console.WriteLine("incorrect layer type!");
				return;
			}
			SetProperty("#LayerObjects", (int)(GetPropertyData("#LayerObjects")));
		}

		public void AddToLayer(int groupnum, int xcell, int ycell, GameEvent g )
		{
			if (layerType == LayerType.None)
				throw new SpriteLayerException(LayerType.None);
			if (layerType == LayerType.GameEvent)
			{
				if (LayerObjects is Tuple<int[,], List<GameEvent>>)
				{
					((Tuple<int[,], List<GameEvent>>)LayerObjects).Item1.SetValue(groupnum, xcell, ycell); //change the tile group num data
					if(g != null)
						((Tuple<int[,], List<GameEvent>>)LayerObjects).Item2.Add(g); //add the game event data!
				}
				else Console.WriteLine("Incorrect Layer type!");
				return;
			}
		}


		/// <summary>
		/// ADeletes objects from the sprite layers ONLY for tiles.
		/// </summary>
		/// <param name="newLayerObject">Desired object to add.</param>
		public void DeleteFromLayer(int xcell, int ycell)
		{
			if (layerType == LayerType.Tile)
			{
					((int[,])LayerObjects)[xcell, ycell] = -1;
			}
		}

		//TODO: Create the deletion methods for the other sprite layer types.
		/// <summary>
		/// Overwrites objects on the sprite layer. Changes thier data values.
		/// </summary>
		/// <param name="newLayerObject">Desired object to add.</param>
		public void OverwriteOnLayer(int xcell, int ycell, int newData)
		{
			if (layerType == LayerType.Tile)
			{
				((int[,])LayerObjects)[xcell, ycell] = newData;
			}
		}


		class SpriteLayerException : Exception
		{
			public SpriteLayerException()
			{

			}

			public SpriteLayerException(LayerType layerType)
				: base(String.Format("Invalid Layer Type addtion. Current Layer Type{0}", layerType))
			{

			}
		}
	}
}
