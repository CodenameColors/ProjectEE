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

		}

		private void LoadInitalVars(String FilePath)
		{
			byte[] byteArray = Encoding.UTF8.GetBytes(FilePath);
			using (TextReader reader = new StreamReader(new MemoryStream(byteArray)))
			{


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
