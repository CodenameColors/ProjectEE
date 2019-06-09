using System;
using System.Collections.Generic;
using System.IO;
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
using BixBite;

namespace AmethystEngine.Forms
{
  /// <summary>
  /// Interaction logic for ProjectSettings.xaml
  /// </summary>
  public partial class ProjectSettings : Window
  {
		String ProjectName, Thumbnail, GameLocation, ConfigLocation = String.Empty;
		List<String> levelpaths = new List<String>();
		String ProjectLocation;


    public ProjectSettings(String ProjectFilepath)
    {
      InitializeComponent();
			LoadInitalVars(ProjectFilepath);
		}

		private void LoadInitalVars(String FilePath)
		{
			ProjectLocation = FilePath;
			byte[] byteArray = Encoding.UTF8.GetBytes(FilePath);

			using (StreamReader file = new StreamReader(FilePath))
			{
				int counter = 0;
				string ln;

				while ((ln = file.ReadLine()) != null)
				{
					if (ln == "-ProjectName:")
					{
						ln = file.ReadLine();
						ProjectName_TB.Text = ln;
					}
					else if (ln == "-GameLocation:")
					{
						ln = file.ReadLine();
						GameLocation_TB.Text = ln;
					}
					else if (ln == "-ConfigLocation:")
					{
						ln = file.ReadLine();
						ConfigLoction_TB.Text = ln;
					}
					else if (ln.Contains("Thumbnail")) {
						ln = file.ReadLine();
						if(ln.Contains(";"))
							ProjectThumbnail_IMG.Source = new BitmapImage(new Uri(ln, UriKind.Relative));
						else {
							if (File.Exists(ln))
							{
								ProjectThumbnail_IMG.Source = new BitmapImage(new Uri(ln));
							}
							else { ProjectThumbnail_IMG.Source = new BitmapImage(new Uri("/AmethystEngine;component/images/Ame_icon_small.png", UriKind.Relative)); }
						}
					}
					else if (ln.Contains("MainLevel"))
					{
						ln = file.ReadLine();
						if (ln.Contains("FILL"))
							MainLevel_TB.Text = "Not set yet!";
						else
							MainLevel_TB.Text = ln.Substring(ln.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
					}
						Console.WriteLine(ln);
					counter++;
				}
				file.Close();
				Console.WriteLine($"File has {counter} lines.");
				//GameEvent game = new GameEvent();
				
			}

			//show levels
			foreach (String s in Directory.GetFiles(FilePath.Replace(".gem", "_Game\\Content\\Levels")))
			{
				Levels_LB.Items.Add(s.Substring(s.LastIndexOfAny(new char[] { '/', '\\' }) + 1));
				levelpaths.Add(s);
			}
		}

    private void ProjSettings_DragMove(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }

		private void ChangeThumbnail_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.Title = "Choose New Thumbnail";
			dlg.DefaultExt = "png file(*.png)|*.png"; //default file extension
			dlg.Filter = "Image Files|*.png;*";

			// Show save file dialog box
			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results kicks out if the user doesn't select an item.
			String importfilename, destfilename = "";
			if (result == true)
			{
				importfilename = dlg.FileName;
			}
			else
				return;

			using (StreamReader file = new StreamReader(ProjectLocation))
			{
				int counter = 0;
				string ln;
				List<String> filelines = new List<string>();
				while ((ln = file.ReadLine()) != null)
				{
					filelines.Add(ln);
				}
				file.Close();
				filelines[3] = importfilename;
				ProjectThumbnail_IMG.Source = new BitmapImage(new Uri(importfilename));
				File.WriteAllLines(ProjectLocation, filelines.ToArray());
			}

		}

		private void SetMainLevel_Click(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Set Main Level");
			MainLevel_TB.Text = levelpaths[Levels_LB.SelectedIndex].Substring(levelpaths[Levels_LB.SelectedIndex].LastIndexOfAny(new char[] { '/', '\\' }) + 1);

			using (StreamReader file = new StreamReader(ProjectLocation))
			{
				int counter = 0;
				string ln;
				List<String> filelines = new List<string>();
				while ((ln = file.ReadLine()) != null)
				{
					filelines.Add(ln);
				}
				file.Close();
				filelines[11] = levelpaths[Levels_LB.SelectedIndex];
				File.WriteAllLines(ProjectLocation, filelines.ToArray());
			}
		}

		private void ProjSettings_close(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
    private void ProjSettings_FullScreen(object sender, RoutedEventArgs e)
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

    private void ProjSettings_Minimize(object sender, RoutedEventArgs e)
    {
      WindowState = WindowState.Minimized;
      WindowStyle = WindowStyle.None;

    }

  }
}
