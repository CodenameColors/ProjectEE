using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
		private String _name = "";
		#endregion

		#region Properties

		public String Name
		{
			get => _name;
			set => _name = value;
		}

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
		public List<ChangeLayeredAnimationEvent> AnimationEvents = new List<ChangeLayeredAnimationEvent>();

		#endregion

		public LayeredSpriteSheet()
		{

		}


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

		public bool AddSubLayer(String layerName)
		{
			if(subLayerSpritesheets_Dict.ContainsKey(layerName)) return false;
			subLayerSpritesheets_Dict.Add(layerName, new List<SpriteSheet>());
			ActiveSubLayerSheet.Add(layerName, null);
			return true;
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
		#region File Import and Export


		public static LayeredSpriteSheet ImportlayeredAnimationSheet(String filePath)
		{
			LayeredSpriteSheet retLayeredSpriteSheet = new LayeredSpriteSheet();

			//It's that time again... import parsing.

			XmlReaderSettings settings = new XmlReaderSettings
			{
				//Async = true
			};

			using (XmlReader reader = XmlReader.Create(filePath, settings))
			{
				while (reader.Read()) //Read until you can't
				{
					//skip to a SpriteSheet node
					while (reader.Name != "LayeredSpriteSheets")
						reader.Read();
					if (reader.NodeType == XmlNodeType.Element)
					{
						//Get the name of the sheet.
						retLayeredSpriteSheet.Name = reader.GetAttribute("SheetName");

						//Now we need to find the BaseSheet
						while (reader.Name != "BaseSheet")
							reader.Read();

						//By this point we should have the base sheet. So let's make that.
						String baseFilePath = reader.GetAttribute("File");
						retLayeredSpriteSheet.BaseLayer = SpriteSheet.ImportSpriteSheet(baseFilePath);


						//Now we need to find the SubLayers
						while (reader.Name != "SubLayers")
							reader.Read();

						//Now we need to read ALL the sub layers
						do
						{
							reader.Read();

							if (reader.Name == "SubLayer" && reader.NodeType == XmlNodeType.Element)
							{
								String tempSubLayerName = reader.GetAttribute("LayerName");
								retLayeredSpriteSheet.AddSubLayer(tempSubLayerName);
								do
								{
									reader.Read();

									if (reader.Name == "SpriteSheet" && reader.NodeType == XmlNodeType.Element)
									{
										//Add the possible Sprite sheet to the desired layer
										retLayeredSpriteSheet.AddSpriteSheetToSubLayer(tempSubLayerName,
											SpriteSheet.ImportSpriteSheet(reader.GetAttribute("File")));
									}
								} while (reader.Name.Trim() != "SubLayer");
							}
						} while (reader.Name.Trim() != "SubLayers");

						//at this point we need to set up the animation change events
						while (reader.Name != "AnimationStates")
							reader.Read();

						do
						{
							reader.Read();

							if (reader.Name == "AnimationState" && reader.NodeType == XmlNodeType.Element)
							{
								//If you are here you have found a new animation. So create one to fill with data.
								String animName = (reader.GetAttribute("Name"));
								int startx = int.Parse(reader.GetAttribute("StartX"));
								int starty = int.Parse(reader.GetAttribute("StartY"));
								int numframes = int.Parse(reader.GetAttribute("NumOfFrames"));
								int fwidth = int.Parse(reader.GetAttribute("FrameWidth"));
								int fheight = int.Parse(reader.GetAttribute("FrameHeight"));
								int fps = int.Parse(reader.GetAttribute("FPS"));
								bool isDefault = bool.Parse(reader.GetAttribute("isDefault"));

								SpriteAnimation tempAnimation = new SpriteAnimation(retLayeredSpriteSheet.BaseLayer, animName, numframes, fps)
								{

								};
								//now we need to fill in all the offset/frame position data.
								for (int i = 0; i < numframes; i++)
								{
									// tempAnimation.AddFramePosition(new Vector2(startx + (fwidth * i), starty));
								}

								tempAnimation.ResetAnimation(); //set the first position/pointer.
								tempAnimation.bIsDefualt = isDefault;

								//Skip to the AnimationStates node
								while (reader.Name != "Events")
									reader.Read();

								do
								{
									reader.Read();
									if (reader.Name == "AllowedAnimName" && reader.NodeType == XmlNodeType.Element)
									{
										String toAnimName = reader.GetAttribute("Name");
										String fromAnimName = tempAnimation.Name;
										bool bfinishFirst = bool.Parse(reader.GetAttribute("bFinishFirst"));

										AnimationEvent animationEvent = new ChangeAnimationEvent(fromAnimName, toAnimName, bfinishFirst);
										tempAnimation.AddAnimationEvents(animationEvent);

										//Layered Animation Shprite Sheets has too keep track of all the different layer changes
										do
										{
											reader.Read();

											if (reader.Name == "SubLayerChange" && reader.NodeType == XmlNodeType.Element)
											{
												retLayeredSpriteSheet.AnimationEvents.Add(
													new ChangeLayeredAnimationEvent(
													reader.GetAttribute("LayerName"), reader.GetAttribute("RequestedAnimState")));
											}
										} while (reader.Name != "AllowedAnimName");

									}
								} while (reader.Name != "Events");
							}
						} while (reader.Name.Trim() != "AnimationStates");
					}
				}
			}
			return retLayeredSpriteSheet;
		}

		#endregion


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
