using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite;
using BixBite.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectE_E.Components.Cuprite
{
	/// <summary>
	/// This class is here to store all the methods that are created from the generation via Cuprite
	/// When you create a delegate in the editor namewise, it will be auto generated here. It allows for changes. 
	/// Once it has been created once it will not over write.
	/// 
	/// I would love for this to have regions the names of each level that auto gens thier methods!
	/// 
	/// </summary>
	public static class MapEvents
	{
		//auto gen start here.

		#region level1_test.lvl
		public static void TileCollisionFound(ref Player player)
		{
			Console.WriteLine(String.Format("Collision Activated {0}", "WIP"));
		}

		#endregion
		#region LevelChangeTestNew.lvl

		public static void TrigTestDele()
		{
			Console.WriteLine(String.Format("Activated Trigger Area {0}", "TrigTestDele"));
		}

		//Used for changing the map/level. given a new position
		public static void LevelTestDele1(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Player player, Map CurrentMap, String FileName, int newx, int newy, int movetime)
		{
			//get the level file!
			Level NewLevel = Level.ImportLevel(FileName);

			CurrentMap.ClearEventLUT(); //clear the dictionary LUT to make room for the new maps LUT vars.

			//Create New Map
			CurrentMap.level = NewLevel;
			CurrentMap.LoadTileMaps(graphicsDevice, CurrentMap.level);
			CurrentMap.LoadSprites(graphicsDevice, CurrentMap.level);
			CurrentMap.GenerateLevel(CurrentMap.level, graphicsDevice, spriteBatch);
			player.SetPosition(newx, newy);
		}
		//Used for changing the map/level. given a new position
		public static void BackToTestMapDele(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Player player, Map CurrentMap, String FileName, int newx, int newy, int movetime)
		{
			//get the level file!
			Level NewLevel = Level.ImportLevel(FileName);

			CurrentMap.ClearEventLUT(); //clear the dictionary LUT to make room for the new maps LUT vars.

			//Create New Map
			CurrentMap.level = NewLevel;
			CurrentMap.LoadTileMaps(graphicsDevice, CurrentMap.level);
			CurrentMap.LoadSprites(graphicsDevice, CurrentMap.level);
			CurrentMap.GenerateLevel(CurrentMap.level, graphicsDevice, spriteBatch);
			player.SetPosition(newx, newy);
		}

		#endregion

	}
}
