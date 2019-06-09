using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.SceneObject;
using BixBite.Effects;

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

	public class SpriteLayer
	{
		//instance variables
		public String LayerName { get; set; }
		public LayerType layerType = LayerType.None;

		private Dictionary<String, object> Properties = new Dictionary<string, object>();
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
		}

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
					LayerObjects = new int[width,height];
					for(int i = 0; i < width; i++)
					{
						for(int j = 0; j < height; j++)
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
			Console.WriteLine("tttt");
			if (LayerObjects is Array && ((Array)LayerObjects).Rank == 2)
			{
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

		/// <summary>
		/// Get the data of a desired property. MUST EXIST ALREADY. 
		/// </summary>
		/// <param name="PropertyName"></param>
		/// <returns>Property Data if exists \n Returns null if it doens't exist</returns>
		public object GetProperty(String PropertyName)
		{
			if (Properties.ContainsKey(PropertyName))
			{
				return Properties[PropertyName];
			}
			return null;
		}

		/// <summary>
		/// Adds a new property IF it doesn't exist already
		/// </summary>
		/// <param name="PropertyName">The Property name that you would like to add</param>
		/// <param name="newpData"> new property data.</param>
		public void AddProperty(String PropertyName, object PropertyData)
		{
			if (Properties.ContainsKey(PropertyName))
			{
				return;
			}
			else
			{
				Properties.Add(PropertyName, PropertyData);
			}
		}

		/// <summary>
		/// Set a property to a new value
		/// </summary>
		/// <param name="PropertyName">The Property name that you would like to change</param>
		/// <param name="newpData"> new property data. MUST MATCH TYPE</param>
		public void ChangeProperty(String PropertyName, object newpData)
		{
			if (Properties.ContainsKey(PropertyName))
			{
				if (Properties[PropertyName].GetType() == newpData.GetType())
					Properties[PropertyName] = newpData;
			}
			return;
		}

		/// <summary>
		/// Handles the drawing of the current layer.
		/// </summary>
		public void Draw()
		{

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
