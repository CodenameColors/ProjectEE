using AmethystEngine.Components;
using BixBite.Characters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
using BixBite.Rendering.UI;

namespace AmethystEngine.Forms
{
  /// <summary>
  /// Interaction logic for AddCharacterForm.xaml
  /// </summary>
  public partial class AddCharacterForm : Window
  {
		Character DesiredCharacter = new Character();
		private GameUI desiredGameUi;
		ObservableCollection<EditorObject> Sprites = new ObservableCollection<EditorObject>();
		public delegate void HooKfunction(Character c, GameUI gameUi, String LinkedTextboxName, String LinkedDialogueImage = null);
		public HooKfunction AddToScene;
		private String ProjectFilePath ="";


		private List<Border> DialogueImageChoices_List = new List<Border>();
		private List<TextBox> DialogueTextboxChoices_List = new List<TextBox>();



		public AddCharacterForm(String projectFilePath)
    {
      InitializeComponent();

			CharacterSprites_LB.ItemsSource = Sprites;
			this.ProjectFilePath = projectFilePath;
    }

		private void BImportFolder_CB_Click(object sender, RoutedEventArgs e)
		{
			if (bImportFolder_CB.IsChecked == true)
			{
				MainGrid.RowDefinitions[2].Height = new GridLength(30);
			}
			else if (bImportFolder_CB.IsChecked == false)
			{
				MainGrid.RowDefinitions[2].Height = new GridLength(0);
			}
		}

		private void AddSpriteToCharacter_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				FileName = "Sprite Image.Png", //default file 
				Title = "Import new sprite image",
				DefaultExt = "All files (*.*)|*.*", //default file extension
				Filter = "png file (*.png)|*.png|All files (*.*)|*.*"
			};

			// Show save file dialog box
			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results kicks out if the user doesn't select an item.
			String importfilename, destfilename = "";
			if (result == true)
			{
				importfilename = dlg.FileName;
			}
			else return;
			String s = dlg.FileName.Substring(dlg.FileName.LastIndexOfAny(new char[] { '/', '\\' })+1);
			s.Substring(0, s.IndexOf('.')-1);
			Sprites.Add(new EditorObject(dlg.FileName, s,false,EObjectType.File));
		}

		private void GetCharacterFolderLoc_BTN_Click(object sender, RoutedEventArgs e)
		{
			Sprites.Clear();
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
			{
				Title = "Folder Location for this characters Sprites",
				FileName = "IGNORE THIS" //default file name
			};
			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			}
			else return;
			Console.WriteLine(filename);

			CharacterFolderLocation_TB.Text = filename;

			foreach (String filepath in getFilesInDir(filename))
			{
				String s = filepath.Substring(filename.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
				s.Substring(0, s.IndexOf('.') - 1);
				Sprites.Add(new EditorObject(filepath, s, false, EObjectType.File));
				
			}

		}

		private void CharacterName_TB_KeyDown(object sender, KeyEventArgs e)
		{
			if(Key.Enter == e.Key)
			{
				DesiredCharacter.Name = CharacterName_TB.Text;
			}
		}


		static List<string> getFilesInDir(string dirPath)
		{
			List<string> retVal = new List<string>();
			try
			{
				retVal = System.IO.Directory.GetFiles(dirPath, "*.*", System.IO.SearchOption.TopDirectoryOnly).ToList();
				foreach (System.IO.DirectoryInfo d in new System.IO.DirectoryInfo(dirPath).GetDirectories("*", System.IO.SearchOption.TopDirectoryOnly))
				{
					retVal.AddRange(getFilesInDir(d.FullName));
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine(dirPath);
			}
			return retVal;
		}

		private void LBind_DragMove(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void Confirm_BTN_Click(object sender, RoutedEventArgs e)
		{
			bool bFail = false;
			foreach(EditorObject eobj in Sprites)
				DesiredCharacter.DialogueSprites.Add(new BixBite.Rendering.Sprite(eobj.Name, eobj.Thumbnail.AbsolutePath,0,0,0,0));
			if (DesiredCharacter.Name == null) DesiredCharacter.Name = CharacterName_TB.Text;
			if (desiredGameUi == null )
			{
				OutputLog_TB.Text += "GameUI not imported!" + "\n";
				bFail = true;
			}
			if (DesiredCharacter == null)
			{
				OutputLog_TB.Text += "Character not set!" + "\n";
				bFail = true;
			}
			if (LinkedTextboxesChoice_CB.SelectedIndex < 0)
			{
				OutputLog_TB.Text += "Linked TextBox is not set!" + "\n";
				bFail = true;
			}
			if (bFail) return;
			if (AddToScene != null) AddToScene(DesiredCharacter, desiredGameUi, LinkedTextboxesChoice_CB.SelectedItem.ToString(), (LinkedImageBoxesChoice_CB.Text));
			this.Close();
		}

		private void ImportGameUI_BTN_Click(object sender, RoutedEventArgs e)
		{

			//add GameUI control.
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open UI File",
				FileName = "", //default file name
				InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\UI"),
				Filter = "UI files (*.ui)|*.ui|All files (*.*)|*.*",
				FilterIndex = 2,
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			}
			else return; //invalid name
			Console.WriteLine(dlg.FileName);

			GameUI g = GameUI.ImportGameUI(dlg.FileName);
			DrawUIToScreen(PreivewUI_Grid, Back_PreivewUI_Grid, g, false);


		}

		/// <summary>
		/// method to draw a newly added/imported UI to the screen. Choose whether or not to breakdown the components for editing.
		/// </summary>
		/// <param name="CurrentEditorCanvas">Current Canvas that you will draw the UI too</param>
		/// <param name="gameUI">The Custom created UI that you want to draw</param>
		/// <param name="bcomps">TRUE = multiple ContentControls will be drawn, and allowed to be edited 
		/// <para/>
		/// FALSE = ONLY ONE Content control will be drawn. Base UI is editable in size, and position. Children are editable via properties
		/// </param>
		public void DrawUIToScreen(Canvas CurrentEditorCanvas, Canvas CurrentEditorCanvas_Back, GameUI gameUI, bool bcomps)
		{
			//set the position and the size of the Base UI
			ContentControl BaseUI = ((ContentControl)this.TryFindResource("MoveableControls_Template"));

			BaseUI.Width = Int32.Parse(gameUI.GetPropertyData("Width").ToString());
			BaseUI.Height = Int32.Parse(gameUI.GetPropertyData("Height").ToString());
			BaseUI.BorderBrush = (((bool)gameUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent);
			BaseUI.BorderThickness = new Thickness(0);
			BaseUI.Tag = "Border";
			BaseUI.Name = gameUI.UIName;
			//};
			BaseUI.Content = new Grid()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
			};
			((Grid)BaseUI.Content).Children.Add(new Border() { BorderThickness = new Thickness(2), BorderBrush = (((bool)gameUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent) });

			//get the middle!
			Point mid = new Point(CurrentEditorCanvas_Back.ActualWidth / 2, CurrentEditorCanvas_Back.ActualHeight / 2);
			//get the true center point. Since origin is top left.
			mid.X = 10; mid.Y = 10;
			Canvas.SetLeft(BaseUI, mid.X); Canvas.SetTop(BaseUI, mid.Y);

			//which drawing type?
			if (bcomps)
			{
				//create all the child UI elements as editable content controls
				foreach (GameUI childUI in gameUI.UIElements)
				{
					#region DefaultProperties
					//set the position and the size of the Base UI
					ContentControl CUI = ((ContentControl)this.TryFindResource("MoveableControls_Template"));
					CUI.Width = Int32.Parse(childUI.GetPropertyData("Width").ToString());
					CUI.Height = Int32.Parse(childUI.GetPropertyData("Height").ToString());
					CUI.BorderBrush = (((bool)childUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent);
					CUI.BorderThickness = new Thickness(2);
					CUI.Tag = "Border";
					CUI.Name = childUI.UIName;

					CUI.Content = new Grid()
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Stretch,
						Background = (SolidColorBrush)new BrushConverter().ConvertFromString(childUI.GetPropertyData("Background").ToString()),
						IsHitTestVisible = false,
					};
					((Grid)CUI.Content).Children.Add(new Border()
					{
						BorderThickness = (((bool)childUI.GetPropertyData("ShowBorder")) ? new Thickness(2) : new Thickness(0)),
						BorderBrush = (((bool)childUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent)
					});
					CUI.Name = childUI.UIName;
					Canvas.SetLeft(CUI, mid.X + Int32.Parse(childUI.GetPropertyData("Xoffset").ToString()));
					Canvas.SetTop(CUI, mid.Y + Int32.Parse(childUI.GetPropertyData("YOffset").ToString()));
					Canvas.SetZIndex(CUI, Int32.Parse(childUI.GetPropertyData("Zindex").ToString()));
					CurrentEditorCanvas.Children.Add(CUI);
					#endregion

					if (childUI is GameTextBlock)
					{
						CUI.Tag = "TEXTBOX";
						//My Game textboxes can have background images. so we need implement my frame logic.
						((Grid)CUI.Content).RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
						((Grid)CUI.Content).RowDefinitions.Add(new RowDefinition() { });
						((Grid)CUI.Content).RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

						((Grid)CUI.Content).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
						((Grid)CUI.Content).ColumnDefinitions.Add(new ColumnDefinition() { });
						((Grid)CUI.Content).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
						((Grid)CUI.Content).ShowGridLines = true;
						InitimagesTBGrid(((Grid)CUI.Content));
						SetTBBackgroundImage(((Grid)CUI.Content), childUI.GetPropertyData("Image").ToString());
						//
						TextBox tb = new TextBox()
						{
							Text = childUI.GetPropertyData("ContentText").ToString(),
							Margin = new Thickness(2),
							BorderThickness = new Thickness(0),
							IsHitTestVisible = false,
							VerticalContentAlignment = VerticalAlignment.Center,
							HorizontalContentAlignment = HorizontalAlignment.Center,
							FontSize = Int32.Parse(childUI.GetPropertyData("FontSize").ToString()),
							Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(childUI.GetPropertyData("FontColor").ToString()),
							Background = Brushes.Transparent
						};
						Grid.SetColumn(tb, 1); Grid.SetRow(tb, 1);
						((Grid)CUI.Content).Children.Add(tb);

					}
					else if (childUI is GameIMG)
					{
						CUI.Tag = "IMAGE";

						String TB = childUI.GetPropertyData("Image").ToString();
						Image img = new Image();
						img.Stretch = Stretch.Fill;
						img.Source = new BitmapImage(new Uri(TB, UriKind.RelativeOrAbsolute));
						img.IsHitTestVisible = false;
						((Grid)CUI.Content).Children.Add(new Rectangle() { });
						((Grid)CUI.Content).Children.Add(img);


					}
					else if (childUI is GameButton)
					{

					}
				}
			}
			else //all as one
			{
				//create all the child UI elements as editable content controls
				foreach (GameUI childUI in gameUI.UIElements)
				{
					#region DefaultProperties
					//set the position and the size of the Base UI
					ContentControl CUI = ((ContentControl)this.TryFindResource("MoveableControls_Template"));
					CUI.Width = Int32.Parse(childUI.GetPropertyData("Width").ToString());
					CUI.Height = Int32.Parse(childUI.GetPropertyData("Height").ToString());
					CUI.BorderBrush = (((bool)childUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent);
					CUI.BorderThickness = new Thickness(0);
					CUI.Tag = "Border";
					CUI.Name = childUI.UIName;

					CUI.Content = new Grid()
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Stretch,
						Background = (SolidColorBrush)new BrushConverter().ConvertFromString(childUI.GetPropertyData("Background").ToString()),
						IsHitTestVisible = false,
					};
					((Grid)CUI.Content).Children.Add(new Border()
					{
						BorderThickness = new Thickness(0),//(((bool)childUI.GetProperty("ShowBorder")) ? new Thickness(2) : new Thickness(0)),
						BorderBrush = (((bool)childUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent),
						IsHitTestVisible = false
					});
					CUI.Name = childUI.UIName;
					#endregion

					if (childUI is GameTextBlock)
					{
						CUI.Tag = "TEXTBOX";
						//My Game textboxes can have background images. so we need implement my frame logic.
						((Grid)CUI.Content).RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
						((Grid)CUI.Content).RowDefinitions.Add(new RowDefinition() { });
						((Grid)CUI.Content).RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

						((Grid)CUI.Content).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
						((Grid)CUI.Content).ColumnDefinitions.Add(new ColumnDefinition() { });
						((Grid)CUI.Content).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
						InitimagesTBGrid(((Grid)CUI.Content));
						SetTBBackgroundImage(((Grid)CUI.Content), childUI.GetPropertyData("Image").ToString());
						//
						TextBox tb = new TextBox()
						{
							Text = childUI.GetPropertyData("ContentText").ToString(),
							Margin = new Thickness(2),
							BorderThickness = new Thickness(0),
							IsHitTestVisible = false,
							VerticalContentAlignment = VerticalAlignment.Center,
							HorizontalContentAlignment = HorizontalAlignment.Center,
							FontSize = Int32.Parse(childUI.GetPropertyData("FontSize").ToString()),
							Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(childUI.GetPropertyData("FontColor").ToString()),
							Background = Brushes.Transparent
						};
						Grid.SetColumn(tb, 1); Grid.SetRow(tb, 1);
						((Grid)CUI.Content).Children.Add(tb);
						CUI.Margin = new Thickness()
						{
							Left = Int32.Parse(childUI.GetPropertyData("Xoffset").ToString()),
							Top = Int32.Parse(childUI.GetPropertyData("YOffset").ToString()),
							//Bottom = BaseUI.Height - (Top + CUI.Height),
							//Right = BaseUI.Width - (Left + CUI.Width)
						};
						CUI.IsHitTestVisible = false;
						CUI.VerticalAlignment = VerticalAlignment.Top;
						CUI.HorizontalAlignment = HorizontalAlignment.Left;
						CUI.BorderThickness = new Thickness(0);
						((Grid)BaseUI.Content).Children.Add(CUI);

						//add to the choices 
						LinkedTextboxesChoice_CB.Items.Add(childUI.UIName);
						DialogueTextboxChoices_List.Add(tb);
					}
					else if (childUI is GameIMG)
					{
						CUI.Tag = "IMAGE";

						Border bor = new Border();

						String TB = childUI.GetPropertyData("Image").ToString();
						Image img = new Image() { IsHitTestVisible = false };
						img.Stretch = Stretch.Fill;
						img.Source = new BitmapImage(new Uri(TB, UriKind.RelativeOrAbsolute));
						img.IsHitTestVisible = false;
						((Grid)CUI.Content).Children.Add(new Rectangle() { });
						CUI.Margin = new Thickness()
						{
							Left = Int32.Parse(childUI.GetPropertyData("Xoffset").ToString()),
							Top = Int32.Parse(childUI.GetPropertyData("YOffset").ToString()),
						};
						CUI.IsHitTestVisible = false;
						CUI.VerticalAlignment = VerticalAlignment.Top;
						CUI.HorizontalAlignment = HorizontalAlignment.Left;
						bor.Child = img;
						((Grid)CUI.Content).Children.Add(bor);
						((Grid)BaseUI.Content).Children.Add(CUI);

						//add to the choices 
						LinkedImageBoxesChoice_CB.Items.Add(childUI.UIName);
						DialogueImageChoices_List.Add(bor);

					}
					else if (childUI is GameButton)
					{

					}
				}
			}

			desiredGameUi = gameUI;
		CurrentEditorCanvas.Children.Add(BaseUI);
		}

		/// <summary>
		/// This method is here to add an image to a grid.
		/// This grid is here to allow the scaling of a text box image and NOT lose shape.
		/// think of this like a frame
		/// </summary>
		/// <param name="g"></param>
		public void InitimagesTBGrid(Grid g)
		{
			for (int i = 0; i < g.RowDefinitions.Count; i++)
			{
				for (int j = 0; j < g.ColumnDefinitions.Count; j++)
				{
					Image img = new Image() { IsHitTestVisible = false, Stretch = Stretch.Fill };
					Grid.SetRow(img, i); Grid.SetColumn(img, j);
					g.Children.Add(img);
				}
			}
		}

		/// <summary>
		/// This method is here to set the background. In order to allow stretching this method using framing logic.
		/// Or "Margins" for the image.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="IMGPath"></param>
		private void SetTBBackgroundImage(Grid g, String IMGPath)
		{
			if (!File.Exists(IMGPath)) return;
			foreach (UIElement uie in g.Children)
			{
				if (!(uie is Image)) continue;

				int vert = Grid.GetRow(uie);
				int hori = Grid.GetColumn(uie);
				double width = 0; double height = 0;
				double x = 0; double y = 0;

				BitmapImage bmp = new BitmapImage(new Uri(IMGPath));

				if (vert != 1) { height = 90; }
				else
				{
					height = bmp.Height - 180; y = 90;
				}
				if (vert == 2) y = bmp.Height - 90;

				if (hori != 1) { width = 90; }
				else
				{
					width = bmp.Width - 180; x = 90;
				}
				if (hori == 2) x = bmp.Width - 90;

				var crop = new CroppedBitmap(bmp, new Int32Rect((int)x, (int)y, (int)width, (int)height));
				// using BitmapImage version to prove its created successfully
				Image image2 = new Image
				{
					Source = crop //cropped
				};

				((Image)uie).Source = crop;
			}

		}

		private void LBind_close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void LinkedImageBoxesChoice_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DialogueImageChoices_List[(sender as ComboBox).SelectedIndex].BorderBrush = Brushes.DeepSkyBlue;
			DialogueImageChoices_List[(sender as ComboBox).SelectedIndex].BorderThickness = new Thickness(5) ;
		}

		private void LinkedTextboxesChoice_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DialogueTextboxChoices_List[(sender as ComboBox).SelectedIndex].BorderBrush = Brushes.Orange;
			DialogueTextboxChoices_List[(sender as ComboBox).SelectedIndex].BorderThickness = new Thickness(5);
		}
	}
}
