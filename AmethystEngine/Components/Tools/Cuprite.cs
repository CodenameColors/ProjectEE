using BixBite;
using BixBite.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BixBite;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

		public static List<String> GetMethodTemplate(GameEvent ge)
		{
			List<String> CodeLines = new List<string>();
			using (XmlReader reader = XmlReader.Create("MapEventTemplate.xml"))
			{
				while (reader.Read())
				{
					//find the right tag
					String EName = ge.eventType.ToString().ToLower();
					if(reader.Name.ToLower() == EName)
					{
						reader.Read();
						while (reader.Name.ToLower() != EName && reader.NodeType != XmlNodeType.EndElement)
						{
							CodeLines =((reader.Value.Split('\n').ToList()));
							reader.Read();
						}
					}
				}
			}
			//remove whitespace
			CodeLines = CodeLines.Where(x => !string.IsNullOrEmpty(x)).ToList();

			//FIll in the names in the array!
			for(int i = 0; i< CodeLines.Count; i++)
			{
				if (CodeLines[i].Contains("[NAMEHERE]"))
				{
					CodeLines[i] = CodeLines[i].Replace("[NAMEHERE]", ge.GetProperty("DelegateEventName").ToString());
				}
			}
			return CodeLines; //return the lines to gen.
		}

		public static void GenerateMethod(List<String> CodeLines, String ProjectPath, String Region)
		{
			//scan through EVERY level file in the project.
			int len = ProjectPath.Length - ProjectPath.LastIndexOfAny(new char[] { '\\', '/' }) - 5;
			String ProjectName = ProjectPath.Substring(ProjectPath.LastIndexOfAny(new char[] { '\\', '/' }) + 1, len);
			ProjectPath = ProjectPath.Substring(0, ProjectPath.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
			ProjectPath += String.Format("{0}_Game\\Components\\Cuprite\\MapEvents.cs", ProjectName);




		}

		public static bool isRegionCreated(String CodeFilePath, String RegionName)
		{

		}

		public static bool isMethodCreated(String CodeFilePath, String Methodname)
		{
			return false;
		}

		private static String GetFilePath(ref String ProjectPath, String FileType)
		{
			if(FileType == "Level")
			{
				int len = ProjectPath.Length - ProjectPath.LastIndexOfAny(new char[] { '\\', '/' }) - 5;
				String ProjectName = ProjectPath.Substring(ProjectPath.LastIndexOfAny(new char[] { '\\', '/' }) + 1, len);
				ProjectPath = ProjectPath.Substring(0, ProjectPath.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
				ProjectPath += String.Format("{0}_Game\\Components\\Cuprite\\MapEvents.cs", ProjectName);
			}
			return ProjectPath;
		}

		public static Dictionary<String, List<GameEvent>> GetProjectGameEvents(String ProjectPath)
		{
			Dictionary <String, List<GameEvent>> ProjectGameEvents = new Dictionary<string, List<GameEvent>>();

			//scan through EVERY level file in the project.
			int len = ProjectPath.Length - ProjectPath.LastIndexOfAny(new char[] { '\\', '/' }) - 5;
			String ProjectName = ProjectPath.Substring(ProjectPath.LastIndexOfAny(new char[] { '\\', '/' }) + 1, len);
			ProjectPath = ProjectPath.Substring(0, ProjectPath.LastIndexOfAny(new char[] { '\\', '/' })+1);
			ProjectPath += String.Format("{0}_Game\\Content\\Levels", ProjectName); 
			List<String> LevelFiles = Directory.GetFiles(ProjectPath).ToList();

			foreach (String path in LevelFiles)
			{
				String Levelname = path.Substring(path.LastIndexOfAny(new char[] { '/', '\\' })+1);
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
				if (sl.layerType != LayerType.GameEvent) continue;
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
