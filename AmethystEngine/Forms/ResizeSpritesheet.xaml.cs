using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	/// Interaction logic for ResizeSpritesheet.xaml
	/// </summary>
	public partial class ResizeSpritesheet
	{
		public delegate void UpdateSize_Hook(int width, int height);
		public UpdateSize_Hook UpdateSizeHook;

		private int previousWidth;
		private int previousHeight;

		public ResizeSpritesheet()
		{
			InitializeComponent();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			//PreviousWidth_TB.Text = previousWidth.ToString();
			//PreviousHeight_TB.Text = previousHeight.ToString();
		}

		private void LBind_DragMove(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void LBind_close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}


		private void ConfirmChange_BTN_Click(object sender, RoutedEventArgs e)
		{

			//Make sure we have all the new values we need to change the size.
			if (int.TryParse(NewWidth_TB.Text, out int width) && int.TryParse(NewHeight_TB.Text, out int height))
			{
				UpdateSizeHook?.Invoke(width, height);

				this.Close();
			}

		}
	}
}
