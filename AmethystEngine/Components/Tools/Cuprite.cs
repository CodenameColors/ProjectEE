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
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

namespace AmethystEngine.Components.Tools
{
	/// <summary>
	/// this class is my code generation tool. It is used to check, and write code to the game project files.
	/// </summary>
  public static class Cuprite
  {
		
		public static bool BuildGameProjectFiles(String ProjectFilePath)
		{
			string projectFilePath = ProjectFilePath.Replace(".gem", "_Game\\");
			string[] ProjFileName = Directory.GetFiles(projectFilePath, "*.csproj");
			if (ProjFileName.Length > 0)
			{
				Console.WriteLine(ProjFileName[0]);

				ProjectCollection pc = new ProjectCollection();

				// THERE ARE A LOT OF PROPERTIES HERE, THESE MAP TO THE MSBUILD CLI PROPERTIES
				Dictionary<string, string> globalProperty = new Dictionary<string, string>();
				globalProperty.Add("Configuration", "Debug");
				globalProperty.Add("Platform", "Any CPU");
				globalProperty.Add("OutputPath", @"bin\DesktopGL\AnyCPU\Debug");

				BuildParameters bp = new BuildParameters(pc);
				BuildRequestData buildRequest = new BuildRequestData(ProjFileName[0], globalProperty, "4.0", new string[] { "Build" }, null);
				// THIS IS WHERE THE MAGIC HAPPENS - IN PROCESS MSBUILD
				BuildResult buildResult = BuildManager.DefaultBuildManager.Build(bp, buildRequest);
				// A SIMPLE WAY TO CHECK THE RESULT
				if (buildResult.OverallResult == BuildResultCode.Success)
				{
					Console.WriteLine("Build Successful!");
					return true;
				}
				return false;
			}
			return false;
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

		public static void GenerateMethod(List<String> CodeLines, ref List<String> FileCodeLines, String MethodName, String Region)
		{
			int num = GetFileWriteLocation(ref FileCodeLines, Region, MethodName); //where to write?
			if (num > 0) //should we write?
			{
				for (int i = 0; i < CodeLines.Count; i++) //insert the lines code!
				{
					FileCodeLines.Insert(num+i, CodeLines[i]);
				}
			}
		}

		//public static void GenerateMethod(String ProjectPath, String MethodName, String Region)
		//{
		//	//scan through EVERY level file in the project.
		//	ProjectPath += GetFilePath(ref ProjectPath);
		//	int num = GetFileWriteLocation(CodeLines, Region, MethodName);
		//	if (num > 0)
		//	{

		//	}



		//}


		private static int GetFileWriteLocation(ref List<String> FileCodeLines, String RegionName, String MethodName)
		{
			if (!isMethodCreated(FileCodeLines, MethodName)) //Does this method exist?
			{
				int num = GetEndOfRegion(ref FileCodeLines, RegionName);
				if (num > 0) //Is the region created?
					return num;
				else //the region is not created. so... create it at the end of the class!
				{
					num = GetEndOfClass(FileCodeLines);

					FileCodeLines.Insert(num - 1, String.Format("	#region {0}", RegionName));
					FileCodeLines.Insert(num, "");
					FileCodeLines.Insert(num + 1, "	#endregion");
					return num;
				}
			}
			return -1;
		}

		private static bool isRegionCreated(String CodeFilePath, String RegionName)
		{
			using (StreamReader sr = new StreamReader(CodeFilePath))
			{
				String ln = "";
				while ((ln = sr.ReadLine()) != null)
				{
					if (ln.Contains(RegionName))
						return true;
				}
			}
			return false;
		}
		
		private static bool isRegionCreated(List<String> CodeFileLines, String RegionName)
		{
			foreach (String s in CodeFileLines)
			{
				if (s.Contains("#region") && s.Contains(RegionName))
					return true;
			}
			return false;
		}

		private static int GetEndOfClass(List<String> CodeFileLines)
		{
			for (int i = 0; i < CodeFileLines.Count; i++)
			{
				Stack<String> Blocks = new Stack<string>();
				String line = CodeFileLines[i];
				if (line.Contains("class"))
				{
					//white space ignore
					while (!line.Contains("{"))
						line = CodeFileLines[i++];
					Blocks.Push("{");

					while (Blocks.Count > 0)
					{
						line = CodeFileLines[i++];
						if (line.Contains("{")) Blocks.Push("{");
						else if (line.Contains("}"))
						{
							if (Blocks.Peek() == "{")
								Blocks.Pop();
							else return -1; //Incorrect code blocks!
						}
					}
					return i;
				}
			}
			return -1;
		}

		private static int GetEndOfRegion(ref List<String> CodeFileLines, String RegionName)
		{
			if (isRegionCreated(CodeFileLines, RegionName)) //does this region exist?
			{
				for(int i = 0; i < CodeFileLines.Count; i++) //scan all lines for the beginning region
				{
					if (CodeFileLines[i].Contains(RegionName)) //found beginning of the region
					{
						//there can be multiple regions in this code... because i hate myself. so use a stack
						Stack<String> RegionStack = new Stack<string>(); RegionStack.Push(CodeFileLines[i++]);
						while (RegionStack.Count > 0) //scan through the lines UNTIL the stack is empty
						{
							if (CodeFileLines[i].Contains("endregion")) //pop
								RegionStack.Pop();
							else if (CodeFileLines[i].Contains("#region")) //push
								RegionStack.Push(CodeFileLines[i]);
							i++;
						}
						return i-1;
					}
				}
			}
			return -1;
		}

		private static bool isMethodCreated(String CodeFilePath, String Methodname)
		{
			using (StreamReader sr = new StreamReader(CodeFilePath))
			{
				String ln = "";
				while ((ln = sr.ReadLine()) != null)
				{
					if (ln.Contains(Methodname))
						return true;
				}
			}
				return false;
		}

		private static bool isMethodCreated(List<String> CodeFileLines, String Methodname)
		{
			foreach (String s in CodeFileLines)
			{
				if (s.Contains(Methodname))
					return true;
			}
			return false;
		}

		public static String GetFilePath(String ProjPath, String FileType = "Level")
		{
			String ProjectPath = ProjPath;
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
