using BixBite;
using BixBite.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmethystEngine.Components.Tools
{
	/// <summary>
	/// this class is my code generation tool. It is used to check, and write code to the game project files.
	/// </summary>
  public static class Cuprite
  {
		
		public static void BuildGameProjectFiles()
		{

		}
		
		#region MapEventGeneration

		//public static List<String> GetMethodTemplate(EventType etype)
		//{
			
		//}

		//public static void GenerateMethod()
		//{

		//}

		//public static bool isMethodCreated(String Methodname)
		//{

		//}

		public static Dictionary<String, List<GameEvent>> GetProjectGameEvents(String ProjectPath)
		{
			Dictionary<String, List<GameEvent>> ProjectGameEvents = new Dictionary<string, List<GameEvent>>();
			
			//scan through EVERY level file in the project.
			ProjectPath = ProjectPath.Substring(0, ProjectPath.LastIndexOfAny(new char[] { '\\', '/' }));
			List<String> LevelFiles = Directory.GetFiles(ProjectPath).ToList();

			foreach (String path in LevelFiles)
			{
				String Levelname = path.Substring(path.LastIndexOfAny(new char[] { '/', '\\' }));
				Level CurrentLevel = Level.ImportLevel(path);
				//get the GameEvents and add the game events list to FULL list
				ProjectGameEvents.Add(Levelname, GetLevelEvents(CurrentLevel));
			}
			return ProjectGameEvents;
		}

		public static List<GameEvent> GetLevelEvents(Level CurrentLevel)
		{
			List<GameEvent> LevelGameevent = new List<GameEvent>();
			foreach (SpriteLayer sl in CurrentLevel.Layers )
			{
				foreach(GameEvent ge in ((Tuple<int[,], List<GameEvent>>)sl.LayerObjects).Item2)
				{
					LevelGameevent.Add(ge);
				}
			}
			return LevelGameevent;
		}

		#endregion
	}
}
