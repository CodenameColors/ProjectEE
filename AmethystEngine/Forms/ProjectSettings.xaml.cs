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
		List<Level> levels = new List<Level>();

    public ProjectSettings(String ProjectFilepath)
    {
      InitializeComponent();
			LoadInitalVars(ProjectFilepath);
		}

		private void LoadInitalVars(String FilePath)
		{
			byte[] byteArray = Encoding.UTF8.GetBytes(FilePath);

			using (StreamReader file = new StreamReader(FilePath))
			{
				int counter = 0;
				string ln;

				while ((ln = file.ReadLine()) != null)
				{
					if(ln == "-ProjectName:")
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
					Console.WriteLine(ln);
					counter++;
				}
				file.Close();
				Console.WriteLine($"File has {counter} lines.");
			}
			
		}

    private void ProjSettings_DragMove(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
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
