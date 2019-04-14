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

namespace AmethystEngine.Forms
{
  /// <summary>
  /// Interaction logic for ProjectSettings.xaml
  /// </summary>
  public partial class ProjectSettings : Window
  {
    public ProjectSettings()
    {
      InitializeComponent();
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
