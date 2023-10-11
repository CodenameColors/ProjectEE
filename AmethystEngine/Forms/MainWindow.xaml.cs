using AmethystEngine.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AmethystEngine.Forms
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
  {
    //List<projdata> recentprojs = new List<projdata>();
  public List<EditorObject> recentprojs = new List<EditorObject>();
    List<String> TFiles = new List<string>();
    public MainWindow()
    {
      InitializeComponent();
      CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
          new ExecutedRoutedEventHandler(delegate (object sender, ExecutedRoutedEventArgs args) { this.Close(); })));
    }

    public void StopApp(object sender, RoutedEventArgs e)
    {
      LBind_close(sender, e);
    }

		private void LoadRecentProjectsToMainWindow()
		{
			//String pathString = System.Environment.CurrentDirectory + "\\rpj.txt";
			String pathString = String.Format("{0}/AmethystEngine/{1}", Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
				"rpj.txt");
			List<String> EditedOutputList = new List<string>();
			if (!System.IO.File.Exists(pathString))
			{
				using (System.IO.FileStream fs = System.IO.File.Create(pathString))
				{

				}
			}

			//read the file to a list 
			RpjFileToList(ref TFiles, pathString);

			//each string in the list is a file. so we need to read that file.
			foreach (String tempFile in TFiles)
			{
				System.IO.StreamReader file;
				try
				{
					file = new System.IO.StreamReader(tempFile);
					EditedOutputList.Add(tempFile);

				}
				catch (DirectoryNotFoundException)
				{
					MessageBox.Show("Project not found. Have you deleted a project without using the editor to delete? " +
						"The unfound project has been removed from recent projects for the future");
					using (System.IO.FileStream fs = System.IO.File.Create(pathString))
					{
						foreach (String ss in EditedOutputList)
						{
							fs.Write((byte[])new UTF8Encoding(true).GetBytes(ss.ToString()), 0, ss.Length + 1);
						}
					}
					continue;
				}
				catch (FileNotFoundException)
				{
					MessageBox.Show("Project not found. Have you deleted a project without using the editor to delete? " +
						"The unfound project has been removed from recent projects for the future");
					using (System.IO.FileStream fs = System.IO.File.Create(pathString))
					{
						foreach (String ss in EditedOutputList)
						{
							fs.Write((byte[])new UTF8Encoding(true).GetBytes(ss.ToString()), 0, ss.Length + 1);
						}
					}
					continue;
				}

				String tline = "";
				EditorObject TempEO = new EditorObject();
				while ((tline = file.ReadLine()) != null)
				{
					System.Console.WriteLine(tline);
					if (tline.Contains("ProjectName"))
					{
						tline = file.ReadLine();
						TempEO.Name = tline;
						//recentprojs.Add(new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png", tline));
					}
					else if (tline.Contains("Thumbnail"))
					{
						tline = file.ReadLine();
						if (tline.Contains(";"))
							TempEO.SetThumbnail(tline);
						else
						{
							if (File.Exists(tline))
							{
								TempEO.SetThumbnail(tline, false);
							}
							else { TempEO.SetThumbnail("/AmethystEngine;component/images/Ame_icon_small.png", true); }
						}
						//recentprojs.Add(new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png", tline));
					}
				}
				recentprojs.Add(TempEO);
			}
			RecentProj_LB.ItemsSource = recentprojs;


		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
    {
			LoadRecentProjectsToMainWindow();
    }

    private void RpjFileToList(ref List<String> TFiles, String pathString)
    {
      //read the file to a list 
      String tline = "";
      System.IO.StreamReader file = new System.IO.StreamReader(pathString);
      while ((tline = file.ReadLine()) != null)
      {
        System.Console.WriteLine(tline);
        TFiles.Add(tline);
      }
      file.Close();
    }

    #region OpenProject
    private void OpenProj_BTN_Click(object sender, RoutedEventArgs e)
    {
      if (RecentProj_LB.SelectedIndex > -1)
      {
        EngineEditor f = new EngineEditor(TFiles[RecentProj_LB.SelectedIndex]);
        f.Show();
				this.Hide();
			}
      else
      {
        MessageBox.Show("Click on a recent project!");
      }
    }
    #endregion

    #region CreateProject
    private void CreateNewProject_BTN(object sender, RoutedEventArgs e)
    {
      if (ProjectPath_TB.Text.Length == 0 || ProName_TB.Text.Length == 0)
      {
        MessageBox.Show("Please enter vaild names, and a valid file location");
        return;
      }

      String path = ProjectPath_TB.Text;
      String pname = ProName_TB.Text;

      if (path != "")
      {
        System.IO.Directory.CreateDirectory(path + "\\" + pname);
        System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game");
        System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Editor");
        using (System.IO.FileStream f = System.IO.File.Create(path + "\\" + pname + "\\" + pname + ".gem"))
        {
          StringBuilder sb = new StringBuilder();
          sb.AppendLine("-ProjectName:");
          sb.AppendLine(pname);
          sb.AppendLine("-Thumbnail:");
          sb.AppendLine("/AmethystEngine;component/images/Ame_icon_small.png");
          sb.AppendLine("-GameLocation:");
          sb.AppendLine(path + "\\" + pname + "\\" + pname + "_Game\\" + "bin\\DesktopGL\\AnyCPU\\Debug\\Game1.exe");
          sb.AppendLine("-ConfigLocation:");
          sb.AppendLine(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Config");
					sb.AppendLine("-Levels:");
					sb.AppendLine(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Levels");
					sb.AppendLine("-MainLevel:");
					sb.AppendLine("[FILLINLATER]");
					sb.AppendLine("-Dialogue:");
					sb.AppendLine(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Dialogue");
					byte[] data = new UTF8Encoding(true).GetBytes(sb.ToString());
          f.Write(data, 0, data.Length);
        }
      }

      CreateGameFiles(path + "\\" + pname + "\\" + pname + "_Game" + "\\");

			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Images");
			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Levels");
			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Config");
			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Dialogue");
			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\UI");
			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Animations");

			String pathString = String.Format("{0}/AmethystEngine/{1}", Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
				"rpj.txt");

			if (!System.IO.File.Exists(pathString))
      {
        System.IO.File.AppendAllText(pathString, String.Format("{0}{1}{2}", path + "\\" + pname + "\\", pname, ".gem\n"));
      }
      else
      {
        System.IO.File.AppendAllText(pathString, String.Format("{0}{1}{2}", path + "\\" + pname + "\\", pname, ".gem\n"));
      }
      this.Hide();
      EngineEditor ff = new EngineEditor(String.Format("{0}{1}{2}", path + "\\" + pname + "\\", pname, ".gem"), pname);

			ff.Show();
    }

    private void ProPath_BTN_Click(object sender, RoutedEventArgs e)
    {
      String path = EngineEditor.GetFilePath("Create New Project");

      if (path == String.Empty || path == null)
      {
        MessageBox.Show("could not retrieve file path. Please try a different path");
        return;
      }
      else
      {
        if (path.Contains(" "))
        {
          MessageBox.Show("No spaces allowed in the file path!");
          return;
        }
        else
        {
          Console.WriteLine(path);
          ProjectPath_TB.Text = path;
        }
      }
    }

    public static void CreateGameFiles(String projloc)
    {
			String AppData = String.Format("{0}/AmethystEngine/", Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData));


			File.WriteAllBytes(AppData + "\\temp", Properties.Resources.Bmg);
      ZipFile.ExtractToDirectory(AppData + "\\temp", projloc);
      File.Delete(AppData + "\\temp");
    }

    #endregion

    #region RemoveProject
    private void RemoveProj_BTN_Click(object sender, RoutedEventArgs e)
    {
      if (RecentProj_LB.SelectedIndex > -1)
      {
        //remove the directory.
        RemoveProject(TFiles[RecentProj_LB.SelectedIndex].Substring(0, TFiles[RecentProj_LB.SelectedIndex].LastIndexOfAny(new char[] { '\\', '/' })));

        //drop the project from the file.
        TFiles.RemoveAt(RecentProj_LB.SelectedIndex);
        String pathString = String.Format("{0}/AmethystEngine/{1}", Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
	        "rpj.txt");
				if (!System.IO.File.Exists(pathString))
        {
          System.IO.File.WriteAllLines(pathString, TFiles.ToArray());
        }
        else
        {
          System.IO.File.WriteAllLines(pathString, TFiles.ToArray());
        }
        //drop the project from the LB refresh the LB
        recentprojs.RemoveAt(RecentProj_LB.SelectedIndex);
        RecentProj_LB.ItemsSource = null;
        RecentProj_LB.ItemsSource = recentprojs;

      }
      else
      {
        MessageBox.Show("Click on a recent project!");
      }
    }

    private void RemoveProject(String ProjectLoc)
    {
      System.IO.Directory.Delete(ProjectLoc, true);
    }
    #endregion

    #region This handles all the windows GUI features. Resize, fullscreen. etc

    private void LBind_close(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
    private void LBind_FullScreen(object sender, RoutedEventArgs e)
    {
      if (WindowState != WindowState.Maximized)
      {
        WindowState = WindowState.Maximized;
        WindowStyle = WindowStyle.None;
      }
      else
      {
        WindowState = WindowState.Normal;
        WindowStyle = WindowStyle.None;
      }
    }

    private void LBind_Minimize(object sender, RoutedEventArgs e)
    {
      WindowState = WindowState.Minimized;
      WindowStyle = WindowStyle.None;

    }

    private void LBind_DragMove(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }

    public void DragWindow(object sender, MouseButtonEventArgs args)
    {
      DragMove();
    }


		#endregion

		private void AddExistingProj_BTN_Click(object sender, RoutedEventArgs e)
		{
			String path = EngineEditor.GetFilePath("Create New Project", true, true);

			if (path == String.Empty || path == null)
			{
				MessageBox.Show("could not retrieve file path. Please try a different path");
				return;
			}
			else if(!File.Exists(path))
			{
				MessageBox.Show("Given File doesn't Exist please try again.");
				return;
			}
			else
			{
				if(System.IO.Path.GetExtension(path) == ".gem")
				{
					// get the app data file that holds that list of project locations.
					String pathString = String.Format("{0}/AmethystEngine/{1}", Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
					"rpj.txt");
					if (!System.IO.File.Exists(pathString))
					{
						MessageBox.Show("your RPJ file isn't created doesn't exist. so i'm going to make it for you.");
						using (System.IO.FileStream fs = System.IO.File.Create(pathString))
						{

						}
						TFiles.Clear();
					}

					TFiles.Add(path);
					System.IO.File.WriteAllLines(pathString, TFiles.ToArray());

					// Hot reload
					LoadRecentProjectsToMainWindow();
					MessageBox.Show("File Added to the Recent Projects. Please Restart to take effect");

				}
				else
				{
					MessageBox.Show("File type MUST BE a \".gem\" file. please try try again.");
					return;
				}
			}
		}
	}
}


//MSBUILD PATH FOR LATER:
// C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe