using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Rendering.Animation
{

	public struct SpriteLayerKeys<T1, T2>
	{
		public readonly T1 LayerName;
		public readonly T2 SpriteSheetName;
		public SpriteLayerKeys(T1 layerName, T2 spriteSheetName) { LayerName = layerName; SpriteSheetName = spriteSheetName; }
	}

	public static class Tuple
	{ // for type-inference goodness.
		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
			return new Tuple<T1, T2>(item1, item2);
		}
	}

	public class LayeredSpriteSheet 
	{



		#region Fields
		
		#endregion

		#region Properties

		//This MUST be set at all times!
		public SpriteSheet BaseLayer = null;

		//okay so each single sublayer can have MULTIPLE different sprite sheets in it.
		//So we can call the sublayer with the first dictionary key.
		//BUT because there can multiple entries in the that layer.
		public Dictionary<String, List<SpriteSheet>> subLayerSpritesheets_Dict = 
			new Dictionary<string, List<SpriteSheet>>();

		/// <summary>
		/// Key is the string name of the layer animation layer name
		/// Returns the SpriteSheet if given the right key
		/// </summary>
		public Dictionary<String, SpriteSheet> ActiveSubLayerSheet = new Dictionary<String, SpriteSheet>();

		#endregion

		public LayeredSpriteSheet(SpriteSheet baselayer)
		{

		}

		public SpriteSheet GetBaseLayerSheet()
		{
			return BaseLayer;
		}


		public void AddSpriteSheetToSubLayer(String layerName, SpriteSheet sheet)
		{
			if(!(subLayerSpritesheets_Dict[layerName].Contains(sheet)))
				subLayerSpritesheets_Dict[layerName].Add(sheet);

			//Don't let it be null forever
			if (ActiveSubLayerSheet[layerName] == null)
			{
				ActiveSubLayerSheet.Remove(layerName);
				ActiveSubLayerSheet.Add(layerName, sheet);
			}
		}

		public void AddSubLayer(String layerName)
		{
			subLayerSpritesheets_Dict.Add(layerName, new List<SpriteSheet>());
			ActiveSubLayerSheet.Add(layerName, null);
		}

		/// <summary>
		/// This is a tough one. Because when you change the base layer, ALL of the sub layers need to change too.
		/// <param name="layerAndStateDictionary">Key = layer name || Value is the animations state name to change to</param>
		/// </summary>
		public void ChangeAnimationState(Dictionary<String, String> layerAndStateDictionary)
		{
			foreach (var keyval in layerAndStateDictionary)
			{
				ActiveSubLayerSheet[keyval.Key].ChangeAnimation(keyval.Value);
			}
		}

		/// <summary>
		/// This function will use the currently loaded animation data. And if given the correct parameters it will change the active one.
		/// AKA what is currently being drawn on that layer.
		/// </summary>
		/// <param name="layerName">The animation layer name</param>
		/// <param name="oldSheet">The name of the current/old sheet that is being drawn on the desired layer</param>
		/// <param name="newSheet">the name of the desired sheet we want to make active on the desired layer given.</param>
		/// <returns>True if change occurs||  False if change dpesn't occur./</returns>
		public bool ChangeActiveSheetString(String layerName, String oldSheet, String newSheet)
		{
			if (ActiveSubLayerSheet.ContainsKey(layerName))
			{
				if (ActiveSubLayerSheet[layerName].SheetName == oldSheet)
				{
					int indexVal = subLayerSpritesheets_Dict[layerName].FindIndex(x => x.SheetName == newSheet);
					if (indexVal >= 0)// it exists
					{
						ActiveSubLayerSheet.Remove(layerName);
						ActiveSubLayerSheet.Add(layerName, subLayerSpritesheets_Dict[layerName][indexVal]);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Returns ALL sheets on a given layer
		/// Returns null if DNE
		/// </summary>
		/// <param name="LayerName"></param>
		/// <returns></returns>
		public List<SpriteSheet> GetAllSpriteSheetsOnLayer(String LayerName)
		{
			//SubLayerSpriteSheets_Dict.Keys.ToList()[0].
			if(subLayerSpritesheets_Dict.ContainsKey(LayerName))
				return subLayerSpritesheets_Dict[LayerName];
			return null;
		}

		/// <summary>
		/// Returns the Sprite Sheet from a given layer, and sheet name
		/// returns NULL if DNE
		/// </summary>
		/// <param name="layerName"></param>
		/// <param name="SheetName"></param>
		/// <returns></returns>
		public SpriteSheet GetSpriteSheet(String layerName, String SheetName)
		{
			if (subLayerSpritesheets_Dict.ContainsKey(layerName))
			{
				int indexVal = subLayerSpritesheets_Dict[layerName].FindIndex(x => x.SheetName == SheetName);
				if(indexVal >= 0)
					return subLayerSpritesheets_Dict[layerName][indexVal];
			}

			return null;
		}

		#region Monogame

		public void Load(ContentManager contentManager)
		{

		}

		public void Update(GameTime gametime)
		{


		}

		public void Draw(SpriteBatch spriteBatch)
		{

			foreach (SpriteSheet sheet in ActiveSubLayerSheet.Values)
			{
				sheet.CurrentAnimation.Draw(spriteBatch);
			}

		}
		#endregion

	}
}
