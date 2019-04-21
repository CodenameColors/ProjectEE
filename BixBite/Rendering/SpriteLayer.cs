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
		Gameobject		//used for location to indicate script triggers.
	};

	public class SpriteLayer
	{
		//instance variables
		public String LayerName { get; set; }
		public LayerType layerType = LayerType.None;
		object LayerObjects = new object(); //contains the objects for this layer. 
		ImageEffect layereffect = new ImageEffect();

		public SpriteLayer()
		{
		}
		public SpriteLayer(LayerType desltype)
		{
			DefineLayerDataType(layerType = desltype); //set the objectdata datatype
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
				case (LayerType.Gameobject):
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
					if (newLayerObject is Microsoft.Xna.Framework.Point)
					{
						if (LayerObjects is int[,])
							((int[,])LayerObjects)[xcell, ycell] = tiledata;
						else Console.WriteLine("Invalid defined Layerobject type. Not a List of Tiles");
					}
					return;
				case (LayerType.Gameobject):
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
