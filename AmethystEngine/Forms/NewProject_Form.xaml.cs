using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace AmethystEngine.Forms
{
  /// <summary>
  /// Interaction logic for NewProject_Form.xaml
  /// </summary>
  public partial class NewProject_Form : Window
  {
		EngineEditor cureditor;
    public NewProject_Form(EngineEditor editor)
    {
      InitializeComponent();
			cureditor = editor;
    }

		#region windowfunctions
		private void ProjSettings_DragMove(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }

    private void ProjSettings_close(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void ProjSettings_Minimize(object sender, RoutedEventArgs e)
    {
      WindowState = WindowState.Minimized;
      WindowStyle = WindowStyle.None;

    }
		#endregion

		private void CreateNewProject_BTN(object sender, RoutedEventArgs e)
		{
			if (ProjectLoction_TB.Text.Length == 0 || ProjName_TB.Text.Length == 0)
			{
				MessageBox.Show("Please enter vaild names, and a valid file location");
				return;
			}



			String path = ProjectLoction_TB.Text;
			String pname = ProjName_TB.Text;

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

      MainWindow.CreateGameFiles(path + "\\" + pname + "\\" + pname + "_Game" + "\\");

			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Images");
			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Levels");
			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Config");
			System.IO.Directory.CreateDirectory(path + "\\" + pname + "\\" + pname + "_Game" + "\\Content\\Dialogue");

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

		private void Browse_BTN_Click(object sender, RoutedEventArgs e)
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
					ProjectLoction_TB.Text = path;
				}
			}
		}
	}
}
