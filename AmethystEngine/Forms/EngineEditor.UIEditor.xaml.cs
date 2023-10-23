using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AmethystEngine.Components;
using AmethystEngine.Components.Tools;
using BixBite.Rendering;
using BixBite.Rendering.UI;
using BixBite.Rendering.UI.TextBlock;
using PropertyGridEditor;
using GameImage = BixBite.Rendering.UI.Image.GameImage;

namespace AmethystEngine.Forms
{
	public partial class EngineEditor
	{

		#region UIEditorVars

		NewUITool CurrentNewUI = NewUITool.NONE;
		ContentControl SelectedBaseUIControl;

		ContentControl SelectedUIControl;

		//Dictionary<String, BaseUI> OpenUIEdits = new Dictionary<string, BaseUI>();
		public ObservableCollection<BaseUI> OpenUIEdits { get; set; }
		BaseUI SelectedUI;
		Dictionary<String, BaseUI> CurrentUIDictionary = new Dictionary<String, BaseUI>();

		#endregion

		private void UIEditorLoaded()
		{

			OpenUIEdits = new ObservableCollection<BaseUI>();
		}


		#region UI

		/// <summary>
		/// Deselects all selected sprite when clicking on the background canvas
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UICanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//you clicked on nothing. so deselect UI CCs
			foreach (ContentControl cc in UIEditor_Canvas.Children.OfType<ContentControl>().ToList())
			{
				Selector.SetIsSelected(cc, false);
			}
		}

		/// <summary>
		/// It occurs when the mouse up on the background canvas
		/// WIP...noting yet.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UIEditor_BackCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{

		}

		/// <summary>
		/// occurs when the mouse moves over the background canvas
		/// Displays the mouse position
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UIEditor_BackCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			Point p = Mouse.GetPosition(LevelEditor_BackCanvas);
			String point = String.Format("({0}, {1}) OFF:({2}, {3})", (int)p.X, (int)p.Y, (int)Canvas_grid.Viewport.X,
				(int)Canvas_grid.Viewport.Y);
			LevelEditorCords_TB.Text = point;

			//which way is mouse moving?
			MPos -= (Vector)e.GetPosition(LevelEditor_Canvas);

			//is the middle mouse button down?
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				//LevelEditorPan();
			}

		}


		public virtual void PropertyCallbackTB(object sender, KeyEventArgs e, BaseUI baseUi)
		{
			if (e.Key == Key.Enter)
			{
				//base.PropertyCallback(sender, e);
				Console.WriteLine("GAMETB UI CALLBACK");
				Console.WriteLine(((TextBox)sender).Tag.ToString());

				String Property = ((TextBox)sender).Tag.ToString();
				if (baseUi.GetProperties().Any(m => m.Item1 == Property))
				{
					baseUi.SetProperty(Property, ((TextBox)sender).Text);
					if (Property == "FontSize")
					{
						//((TextBox)sender).FontSize = Int32.Parse(((TextBox)sender).Text);
					}
					else if (Property == "Text")
					{
						((GameTextBlock)baseUi).Text = (((TextBox)sender).Text);
					}
				}
			}
		}


		/// <summary>
		/// This method is here when a movable image has been clicked and let go. (SPRITES)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContentControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (CurrentTool == EditorTool.Select && LESelectedSprite != null)
			{
				Point point = new Point(Canvas.GetLeft((ContentControl)sender), Canvas.GetTop((ContentControl)sender));
				LESelectedSprite.SetProperty("x", (int)point.X);
				LESelectedSprite.SetProperty("y", (int)point.Y);
				Console.WriteLine(point.ToString() + "MUP");
			}
		}

		/// <summary>
		/// this method is here when a movable image has been scaled (SPRITES)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContentControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (SelectedUI == null) return;
			if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level"))
			{
				if (CurrentTool == EditorTool.Select)
				{
					LESelectedSprite.SetProperty("width", (int)((ContentControl)sender).Width);
					LESelectedSprite.SetProperty("height", (int)((ContentControl)sender).Height);
					Console.WriteLine("SpriteSizeChanged");
				}
			}
			else if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("UI"))
			{
				SelectedUI.SetProperty("Width", (int)((ContentControl)sender).Width);
				SelectedUI.SetProperty("Height", (int)((ContentControl)sender).Height);
			}
		}

		/// <summary>
		/// This method will add a Textbox to the UI editor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UIEditoGameTB_BTN_Click(object sender, RoutedEventArgs e)
		{
			CurrentNewUI = NewUITool.Textbox;
			ContentControl CC = ((ContentControl)this.TryFindResource("MoveableControls_Template"));
			CC.Tag = "TEXTBOX";
			CC.HorizontalAlignment = HorizontalAlignment.Center;
			CC.VerticalAlignment = VerticalAlignment.Center;
			Canvas.SetZIndex(CC, 1);

			String TB = "/AmethystEngine;component/images/SmallTextBubble_Purple.png";
			String TB1 = "/AmethystEngine;component/images/SmallTextBubble_Orange.png";
			ImageBrush ib = new ImageBrush();
			Image img = new Image();
			img.Stretch = Stretch.Fill;
			//img.Source = new BitmapImage(new Uri(TB, UriKind.RelativeOrAbsolute));
			img.IsHitTestVisible = false;
			((Grid)CC.Content).RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
			((Grid)CC.Content).RowDefinitions.Add(new RowDefinition() { });
			((Grid)CC.Content).RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

			((Grid)CC.Content).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
			((Grid)CC.Content).ColumnDefinitions.Add(new ColumnDefinition() { });
			((Grid)CC.Content).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });

			((Grid)CC.Content).ShowGridLines = true;

			((Grid)CC.Content).Children.Add(new Border()
			{
				BorderThickness = new Thickness(2),
				BorderBrush = Brushes.Gray,
				Background = Brushes.Transparent,
				IsHitTestVisible = false
			});
			Grid.SetColumnSpan(((Grid)CC.Content).Children[((Grid)CC.Content).Children.Count - 1], 3);
			Grid.SetRowSpan(((Grid)CC.Content).Children[((Grid)CC.Content).Children.Count - 1], 3);

			InitimagesTBGrid((Grid)CC.Content);
			((Grid)CC.Content).Children.Add(new TextBox()
			{
				Text = "Sameple Text",
				Margin = new Thickness(2),
				BorderBrush = Brushes.Transparent,
				IsHitTestVisible = false,
				Background = Brushes.Transparent,
				VerticalContentAlignment = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				FontSize = 24,
				Foreground = Brushes.White
			});
			Grid.SetRow(((Grid)CC.Content).Children[((Grid)CC.Content).Children.Count - 1], 1);
			Grid.SetColumn(((Grid)CC.Content).Children[((Grid)CC.Content).Children.Count - 1], 1);


			int i = 1;
			String name = "NewTextBox";
			while (CurrentUIDictionary.ContainsKey(name))
			{
				name += i++;
			}

			CC.Name = name;
			UIEditor_Canvas.Children.Add(CC);
			CurrentUIDictionary.Add(name, new GameTextBlock(name, 0, 0, 50, 50, 1, false, 0, 0, "",
				0.0f, "", "", null, null, Microsoft.Xna.Framework.Color.Black));
			OpenUIEdits[0].AddUIElement(CurrentUIDictionary.Values.Last()); //TODO: use the selection Treeview
		}

		/// <summary>
		/// This method will add a Image to the UI editor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UIEditoGameImage_BTN_Click(object sender, RoutedEventArgs e)
		{
			ContentControl CC = ((ContentControl)this.TryFindResource("MoveableControls_Template"));
			CC.HorizontalAlignment = HorizontalAlignment.Center;
			CC.VerticalAlignment = VerticalAlignment.Center;
			Canvas.SetZIndex(CC, 1);

			CC.Tag = "IMAGE";


			String TB = "/AmethystEngine;component/images/emma_colors_oc.png";
			ImageBrush ib = new ImageBrush();
			Image img = new Image() { IsHitTestVisible = false };
			img.Stretch = Stretch.Fill;
			img.Source = new BitmapImage(new Uri(TB, UriKind.RelativeOrAbsolute));
			img.IsHitTestVisible = false;
			((Grid)CC.Content).Children.Add(new Border()
			{ BorderThickness = new Thickness(2), BorderBrush = Brushes.Gray, IsHitTestVisible = false });
			((Grid)CC.Content).Children.Add(new Rectangle() { });
			((Grid)CC.Content).Children.Add(img);

			int i = 1;
			String name = "NewImgBox";
			while (CurrentUIDictionary.ContainsKey(name))
			{
				name += i++;
			}

			CC.Name = name;
			UIEditor_Canvas.Children.Add(CC);
			CurrentUIDictionary.Add(name, new BixBite.Rendering.UI.Image.GameImage(name, 0, 0, 50, 50, 1, 0, 0));
			OpenUIEdits[0].AddUIElement(CurrentUIDictionary.Values.Last()); //TODO: use the selection Treeview
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
					Grid.SetRow(img, i);
					Grid.SetColumn(img, j);
					g.Children.Add(img);
				}
			}
		}

		private void ContentControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{

		}

		/// <summary>
		/// Change the Text's font color for the game control in the UI editor.
		/// </summary>
		/// <param name="obj"></param>
		void customCP_FontColorChanged(Color obj)
		{
			((TextBox)((Grid)SelectedUIControl.Content).Children[10]).Foreground = new SolidColorBrush(obj);
			SelectedUI.SetProperty("FontColor", obj.ToString());
		}

		/// <summary>
		/// Change's the background color for the game control in the UI editor.
		/// </summary>
		/// <param name="obj"></param>
		void customCP_BackgroundColorChanged(Color obj)
		{
			((Grid)SelectedUIControl.Content).Background = new SolidColorBrush(obj);
			SelectedUI.SetProperty("Background", obj.ToString());
		}

		/// <summary>
		/// Changes the visibility of a border control. Also doesn't create it in the game.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SetBorderVisibility(object sender, RoutedEventArgs e)
		{
			String Property = ((CheckBox)sender).Tag.ToString();
			if (Property == "ShowBorder")
			{
				Console.WriteLine("Change BorderDisplay");
				if (((CheckBox)sender).IsChecked == false) //T->F
					((Border)((Grid)SelectedUIControl.Content).Children[0]).Visibility = Visibility.Hidden;
				//((TextBox)((Grid)SelectedUIControl.Content).Children[1]).FontSize = Int32.Parse(((TextBox)sender).Text);
				else
					((Border)((Grid)SelectedUIControl.Content).Children[0]).Visibility = Visibility.Visible;

				SelectedUI.SetProperty("ShowBorder", !((bool)SelectedUI.GetPropertyData("ShowBorder")));
			}
		}

		/// <summary>
		/// Changes the visibility of a border control. Also doesn't create it in the game.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SetMessageText(object sender, KeyEventArgs e)
		{
			if (Key.Enter == e.Key)
			{
				String Property = ((TextBox)sender).Tag.ToString();
				if (Property.Contains( "Text"))
				{
					Console.WriteLine("Set text to target");
					String desiredText = ((TextBox)sender).Text;

					// Let's find the Textbox
					foreach(var uiElement in ((Grid)SelectedUIControl.Content).Children)
					{
						if (uiElement is TextBox textBox)
							textBox.Text = desiredText;
					}
					SelectedUI.SetProperty("Text", desiredText);
				}
			}
		}

		/// <summary>
		/// This method is here as the DEFAULT property callback.
		/// IF THIS IS CALLED YOU NEED TO FIX IT AND DECLARE THE CALLBACK 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UIPropertyCallback(object sender, KeyEventArgs e)
		{
			if (Key.Enter == e.Key)
			{
				//base.PropertyCallback(sender, e);
				Console.WriteLine("GAMETB UI CALLBACK");
				Console.WriteLine(((TextBox)sender).Tag.ToString());

				String Property = ((TextBox)sender).Tag.ToString();
				if (Property == "FontSize")
				{
					((TextBox)((Grid)SelectedUIControl.Content).Children[((Grid)SelectedUIControl.Content).Children.Count - 1])
						.FontSize =
						Int32.Parse(((TextBox)sender).Text);
				}
			}

			//throw new NotImplementedException();
		}

		/// <summary>
		/// This method will change the image fill .
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SelectedUIControl.Tag.ToString() == "IMAGE")
				((Image)((Grid)SelectedUIControl.Content).Children[2]).Source =
					new BitmapImage(((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail);

			if (SelectedUI is BixBite.Rendering.UI.Image.GameImage)
			{
				SelectedUI.SetProperty("Image", ((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
			}

			if (SelectedUIControl.Tag.ToString() == "TEXTBOX")
			{
				//this sets it display wise
				SetTBBackgroundImage(((Grid)SelectedUIControl.Content),
					((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
				SelectedUI.SetProperty("Image", ((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
				//this sets it data wise.
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
				double width = 0;
				double height = 0;
				double x = 0;
				double y = 0;

				BitmapImage bmp = new BitmapImage(new Uri(IMGPath));

				if (vert != 1)
				{
					height = 90;
				}
				else
				{
					height = bmp.Height - 180;
					y = 90;
				}

				if (vert == 2) y = bmp.Height - 90;

				if (hori != 1)
				{
					width = 90;
				}
				else
				{
					width = bmp.Width - 180;
					x = 90;
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

		/// <summary>
		/// change the Z index of the UI object.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BaseUI_ZIndex_Changed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (((ContentControl)SelectedUIControl).Tag.ToString() == "Border")
				{
					return;
				}
				else
				{
					if (Int32.TryParse(((TextBox)sender).Text, out int val))
					{
						Canvas.SetZIndex((ContentControl)SelectedUIControl, val);
						SelectedUI.SetProperty("Zindex", val);
					}
				}

			}
		}

		/// <summary>
		/// Show the context menu which allows editing if clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContentControl_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			ContextMenu cm = this.FindResource("EditMovableControls_Template") as ContextMenu;
			//((MenuItem)cm.Items[0]).IsChecked = ((MenuItem)cm.Items[0]).IsChecked;
			cm.PlacementTarget = sender as ContentControl;
			cm.IsOpen = true;
		}

		/// <summary>
		/// When you click the Editable option in the context menu REENABLE click events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditMoveableControl_Click(object sender, RoutedEventArgs e)
		{
			((MenuItem)sender).IsChecked = !SelectedUIControl.IsHitTestVisible;
			foreach (UIElement item in ((Grid)SelectedUIControl.Content).Children)
			{
				item.IsHitTestVisible = !item.IsHitTestVisible;
			}
		}

		private void UISceneExplorer_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
		}

		private void UISceneExplorer_TreeView_Loaded(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// This method is here for when the user clicks and drags a BaseUI object to move.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContentControl_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{

				Console.WriteLine("Moved UI CC");
				if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("UI"))
				{
					if (SelectedBaseUIControl != null && (SelectedUI is GameTextBlock || SelectedUI is GameImage))
					{
						Vector RelOrigin = new Vector((int)Canvas.GetLeft(SelectedBaseUIControl),
							(int)Canvas.GetTop(SelectedBaseUIControl));
						Vector ControlPos = new Vector((int)Canvas.GetLeft(SelectedUIControl),
							(int)Canvas.GetTop(SelectedUIControl));
						Vector Offset = ControlPos - RelOrigin;
						SelectedUI.SetProperty("Xoffset", (int)Offset.X);
						SelectedUI.SetProperty("YOffset", (int)Offset.Y);
					}
				}
				else if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Dialogue"))
				{

				}
			}
		}

		/// <summary>
		/// Start to create a new UI file in editor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewUI_BTN_Click(object sender, RoutedEventArgs e)
		{
			ControlTemplate cc = (ControlTemplate)this.Resources["UIEditorSceneExplorer_Template"];

			TreeView tv = (TreeView)cc.FindName("UISceneExplorer_TreeView", SceneExplorer_Control);
			if (tv != null)
			{
				SceneExplorer_TreeView = tv;
				SceneExplorer_TreeView.ItemsSource = OpenUIEdits;
			}

			ContentControl CC = ((ContentControl)this.TryFindResource("MoveableControls_Template"));

			CC.HorizontalAlignment = HorizontalAlignment.Center;
			CC.VerticalAlignment = VerticalAlignment.Center;
			((Grid)CC.Content).Children.Add(new Border() { BorderThickness = new Thickness(2), BorderBrush = Brushes.Gray });
			CC.Tag = "Border";
			Canvas.SetZIndex(CC, 0);

			OpenUIEdits.Add(new BaseUI("NewUITool", 0, 0, 0, 0, 50, 50, 1));
			SelectedUI = OpenUIEdits.Last();
			CurrentUIDictionary.Add(OpenUIEdits.Last().UIName, OpenUIEdits.Last());

			CC.Name = OpenUIEdits.Last().UIName;

			UIEditor_Canvas.Children.Add(CC);
			SelectedUIControl = CC;
			SelectedBaseUIControl = CC;
		}

		/// <summary>
		/// Save UI file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveUIAs_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
			{
				Title = "New Level File",
				FileName = "", //default file name
				Filter = "txt files (*.lvl)|*.lvl|All files (*.*)|*.*",
				FilterIndex = 2,
				InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\UI"),
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

			OpenUIEdits[0].ExportUI(dlg.FileName);

		}

		/// <summary>
		/// Get all the projects image paths for the image UI combobox.
		/// </summary>
		/// <returns></returns>
		public List<String> GetAllProjectImages()
		{
			//get the images location
			String InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Images");
			return Directory.GetFiles(InitialDirectory).ToList();
		}

		/// <summary>
		/// import a UI file to the editor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenUIFile_UIE(object sender, RoutedEventArgs e)
		{
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

			BaseUI g = BaseUI.ImportBaseUI(dlg.FileName);
			OpenUIEdits.Add(g);

			ControlTemplate cc = (ControlTemplate)this.Resources["UIEditorSceneExplorer_Template"];

			TreeView tv = (TreeView)cc.FindName("UISceneExplorer_TreeView", SceneExplorer_Control);
			if (tv != null)
			{
				SceneExplorer_TreeView = tv;
				SceneExplorer_TreeView.ItemsSource = OpenUIEdits;
			}

			DrawUIToScreen(UIEditor_Canvas, UIEditor_BackCanvas, OpenUIEdits.Last(), true);

		}


		#endregion


	}
}
