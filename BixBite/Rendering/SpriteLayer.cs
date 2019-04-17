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
		public String LayerName;
		public LayerType layerType = LayerType.None;
		List<object> LayerObjects = new List<object>(); //contains the objects for this layer. 
		ImageEffect layereffect = new ImageEffect();

		public SpriteLayer()
		{
		}
		public SpriteLayer(LayerType desltype)
		{
			layerType = desltype;
		}


		/// <summary>
		/// Add objects to the current sprite layer depending on type.
		/// </summary>
		/// <param name="newLayerObject">Desired object to add.</param>
		public void AddToLayer(object newLayerObject)
		{
			switch (layerType)
			{
				case (LayerType.None):
					throw new SpriteLayerException(LayerType.None);
				case (LayerType.Sprite):
					if (newLayerObject is Sprite)
						LayerObjects.Add(newLayerObject);
					return;
				case (LayerType.Tile):					
					return;
				case (LayerType.Gameobject):
					if (newLayerObject is Prop || newLayerObject is Spawner || newLayerObject is Transistion)
						LayerObjects.Add(newLayerObject);
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
