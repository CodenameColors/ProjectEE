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
					break;
				case (LayerType.Sprite):
					LayerObjects = new List<Sprite>();
					break;
				case (LayerType.GameEvent):
					LayerObjects = new List<GameObject>();
					break;
			}
		}

		/// <summary>
		/// Add objects to the current sprite layer depending on type.
		/// </summary>
		/// <param name="newLayerObject">Desired object to add.</param>
		public void AddToLayer(object newLayerObject, int xcell = 0, int ycell = 0, int tiledata = 0)
		{
			Console.WriteLine("tttt");
			switch (layerType)
			{
				case (LayerType.None):
					throw new SpriteLayerException(LayerType.None);
				case (LayerType.Sprite):
					if (newLayerObject is Sprite)
					{
						if (LayerObjects is List<Sprite>)
							((List<Sprite>)LayerObjects).Add((Sprite)newLayerObject);
						else Console.WriteLine("Invalid defined Layerobject type. Not a List of Sprites");
					}
					return;
				case (LayerType.Tile):
					if (LayerObjects is Array && ((Array)LayerObjects).Rank == 2)
					{
						((Array)LayerObjects).SetValue(tiledata, xcell, ycell);
					}
					//((int[,])LayerObjects)[xcell, ycell] = tiledata;
					else Console.WriteLine("Invalid defined Layerobject type. Not a List of Tiles");
					return;
				case (LayerType.GameEvent):
					if (newLayerObject is GameObject)
					{
						if (LayerObjects is List<GameObject>)
							((List<GameObject>)LayerObjects).Add((GameObject)newLayerObject);
						else Console.WriteLine("Invalid defined Layerobject type. Not a List of GameObjects");
					}
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
					((int[,])LayerObjects)[xcell, ycell] = 0;
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
