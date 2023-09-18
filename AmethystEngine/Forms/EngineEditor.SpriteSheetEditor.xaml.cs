using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AmethystEngine.Components;
using BixBite.Rendering;
using BixBite.Rendering.Animation;
using ImageCropper;

namespace AmethystEngine.Forms
{
	public partial class EngineEditor
	{

		public double SpritesheetEditorZoomLevel = 1;
		int SpritesheetGridHeight = 32;
		int SpritesheetGridWidth = 32;

		TreeView SpriteSheet_CE_Tree;
		CanvasImageProperties currentCanvasImagePropertiesSelected = null;
		CanvasAnimation currentCanvasAnimationPropertiesSelected = null;
		ObservableCollection<AnimationStateMachine> ActiveAnimationStateMachines = new ObservableCollection<AnimationStateMachine>();
		AnimationStateMachine CurrentAnimationStateMachine;
		AnimationState CurrentlySelectedAnimationState;
		CanvasSpritesheet CurrentSelectedSpriteSheet = null;
		ObservableCollection<CanvasSpritesheet> ActiveCanvasSpritesheets = new ObservableCollection<CanvasSpritesheet>();
		//LayeredSpriteSheet currentLayeredSpriteSheet;

		CanvasImageProperties currentSelectedCanvasFrame = null;
		//List<String> CurrentLayeredSpriteSheet_SubLayerNames = new List<string>();
		//List<String> CurrentAnimationSubLayer_AnimStates = new List<string>();
		//List<String> CurrentAnimationSubLayer_AnimStates = new List<string>();


		BitmapImage CurrentSpriteSheet_Image = new BitmapImage();
		Border CurrentSpritesheetBorderForLinking = null;

		public void SpriteSheetEditorLoaded()
		{
			SpriteSheet_CE_Tree =
				(TreeView)ContentLibrary_Control.Template.FindName("SpriteSheetEditor_CE_TV", ContentLibrary_Control);
		}


		private void LeftMouseDowndOnImageFrame_SpriteSheetEditor_CB(object sender, MouseButtonEventArgs e)
		{
			// DO NOT DO ANYTHING if we have a cropper open already
			if (SpritesheetEditor_CropImage.ResizeService != null)
			{
				return;
			}

			Image img = sender as Image;
			if (img != null)
			{
				string imagePath = ((img.Source as CroppedBitmap)?.Source as CroppedBitmap)?.Source.ToString();
				if (imagePath != null)
				{
					SpritesheetEditor_CropImage.SetImage(imagePath, true, (img.Source as CroppedBitmap).SourceRect);
				}
				else
				{
					if (((img.Source as CroppedBitmap)?.Source != null))
					{
						imagePath = (img.Source as CroppedBitmap)?.Source.ToString();
						SpritesheetEditor_CropImage.SetImage(imagePath, true, (img.Source as CroppedBitmap).SourceRect);

					}
					else
					{
						imagePath = ((BitmapImage)img.Source).UriSource.ToString();
						SpritesheetEditor_CropImage.SetImage(imagePath, true);
					}
				}


				// SpritesheetEditor_CropImage = new CroppableImage(img){bHasFocus = true};
				//SpritesheetEditor_CropImage.SetImage(imagePath, true);

				SpritesheetEditor_CropImage.bHasFocus = true;
				SpritesheetEditor_CropImage.Visibility = Visibility.Visible;

				Border parentBorder = (img.Parent as Canvas).Parent as Border;
				if (parentBorder != null)
				{
					// Get the image loaction, and set that to the cropper location
					Canvas.SetLeft(SpritesheetEditor_CropImage, Canvas.GetLeft(parentBorder));
					Canvas.SetTop(SpritesheetEditor_CropImage, Canvas.GetTop(parentBorder));

					// We need to set the cropper to the new image size
					SpritesheetEditor_CropImage.MaxHeight = img.Height;
					SpritesheetEditor_CropImage.Height = img.Height;
					SpritesheetEditor_CropImage.MaxWidth = img.Width;
					SpritesheetEditor_CropImage.Width = img.Width;

					// We need to find out what this croppable image is linked to!
					CanvasImageProperties foundFrame = FindCanvasFrame(parentBorder);
					if (foundFrame != null)
					{
						foundFrame.LinkedCroppableImage = SpritesheetEditor_CropImage;
						foundFrame.LinkedBorderImage = null;
					}

					SpritesheetEditor_Canvas.Children.Remove(parentBorder);
					if (!SpritesheetEditor_Canvas.Children.Contains(SpritesheetEditor_CropImage))
						SpritesheetEditor_Canvas.Children.Add(SpritesheetEditor_CropImage);

					// Create a new MouseEventArgs
					var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
					{
						RoutedEvent = UIElement.MouseLeftButtonDownEvent // Set the RoutedEvent property
					};
					SpritesheetEditor_CropImage.RaiseEvent(args);

					// Handle the mouse down event on the child control here
					// Make sure to set e.Handled = true to prevent the event from bubbling up to the parent control
					e.Handled = true;
				}
			}


		}

		private void LeftMouseUpdOnImageFrame_SpriteSheetEditor_CB(object sender, MouseButtonEventArgs e)
		{
			Image img = sender as Image;
			if (img != null)
			{
				// Release the captured mouse events
				img.ReleaseMouseCapture();

			}


		}

		private void SpriteSheet_Resize_MI_Click(object sender, RoutedEventArgs e)
		{
			Window
				w = new ResizeSpritesheet()
				{
					UpdateSizeHook = UpdateSpriteSheetSize
				}; //((int) SpritesheetEditor_BackCanvas.Width, (int) SpritesheetEditor_BackCanvas.Height); //{ UpdateSizeHook = UpdateSpriteSheetSize};
					 // w.Content = this;
			w.ShowDialog();
		}

		private void UpdateSpriteSheetSize(int width, int height)
		{
			SpritesheetEditor_BackCanvas.Width = width;
			SpritesheetEditor_BackCanvas.Height = height;
			SpritesheetEditor_Canvas.Width = width;
			SpritesheetEditor_Canvas.Height = height;
		}

		private Point _SpriteShheetCanvasStartPoint = new Point();

		private void SpritesheetEditor_BackCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				_SpriteShheetCanvasStartPoint = e.GetPosition(SpritesheetEditor_Canvas);
				SpritesheetEditor_Canvas.CaptureMouse();
			}
		}

		private void SpritesheetEditor_BackCanvas_OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed && SpritesheetEditor_Canvas.IsMouseCaptured)
			{
				Point currentPoint = e.GetPosition(SpritesheetEditor_BackCanvas);
				double deltaX = currentPoint.X - _SpriteShheetCanvasStartPoint.X;
				double deltaY = currentPoint.Y - _SpriteShheetCanvasStartPoint.Y;

				Canvas.SetLeft(SpritesheetEditor_Canvas, Canvas.GetLeft(SpritesheetEditor_Canvas) + deltaX);
				Canvas.SetTop(SpritesheetEditor_Canvas, Canvas.GetTop(SpritesheetEditor_Canvas) + deltaY);
			}
		}

		private void SpritesheetEditor_BackCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Released)
			{
				SpritesheetEditor_Canvas.ReleaseMouseCapture();
			}
		}

		private bool _isPanning;
		private Point _lastMousePosition;

		private void ScrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				_isPanning = true;
				_lastMousePosition = e.GetPosition(SpritesheetEditor_ScrollViewer);
				SpritesheetEditor_ScrollViewer.CaptureMouse();
			}
		}

		private void ScrollViewer_MouseMove(object sender, MouseEventArgs e)
		{
			if (_isPanning)
			{
				Point currentMousePosition = e.GetPosition(SpritesheetEditor_ScrollViewer);
				double deltaX = currentMousePosition.X - _lastMousePosition.X;
				double deltaY = currentMousePosition.Y - _lastMousePosition.Y;

				SpritesheetEditor_ScrollViewer.ScrollToHorizontalOffset(
					SpritesheetEditor_ScrollViewer.HorizontalOffset - deltaX);
				SpritesheetEditor_ScrollViewer.ScrollToVerticalOffset(SpritesheetEditor_ScrollViewer.VerticalOffset - deltaY);

				_lastMousePosition = currentMousePosition;
			}
		}

		private void ScrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Released)
			{
				_isPanning = false;
				SpritesheetEditor_ScrollViewer.ReleaseMouseCapture();
			}
		}

		private void SpritesheetScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			// e.Handled = true; // Mark the event as handled to prevent scrolling
			SpritesheetEditor_BackCanvas.CaptureMouse();
		}

		private Border CreateDashedLineBorder()
		{
			Border border = new Border();
			DrawingBrush brush = new DrawingBrush();

			brush.Viewport = new Rect(0, 0, 8, 8);
			brush.ViewportUnits = BrushMappingMode.Absolute;
			brush.TileMode = TileMode.Tile;

			DrawingGroup drawingGroup = new DrawingGroup();
			GeometryDrawing geometryDrawing = new GeometryDrawing();
			geometryDrawing.Brush = Brushes.GreenYellow;

			GeometryGroup geometryGroup = new GeometryGroup();
			geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, 50, 50)));
			geometryGroup.Children.Add(new RectangleGeometry(new Rect(50, 50, 50, 50)));

			geometryDrawing.Geometry = geometryGroup;
			drawingGroup.Children.Add(geometryDrawing);
			brush.Drawing = drawingGroup;

			border.BorderBrush = brush;
			border.Margin = new Thickness(-1);
			border.BorderThickness = new Thickness(0);
			border.HorizontalAlignment = HorizontalAlignment.Center;
			border.VerticalAlignment = VerticalAlignment.Center;
			border.Visibility = Visibility.Visible;

			return border;
		}


		/// <summary>
		/// Event for global key events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EngineEditor_OnKeyDown(object sender, KeyEventArgs e)
		{
			// Moving a sprite on the spritesheet editor canvas
			if (EditorWindows_TC.SelectedIndex == 1)
			{
				if (SpritesheetEditor_CropImage.Visibility == Visibility.Visible)
				{
					switch (e.Key)
					{
						case Key.Left:
							Canvas.SetLeft(SpritesheetEditor_CropImage, Canvas.GetLeft(SpritesheetEditor_CropImage) - 1);
							break;
						case Key.Right:
							Canvas.SetLeft(SpritesheetEditor_CropImage, Canvas.GetLeft(SpritesheetEditor_CropImage) + 1);
							break;
						case Key.Up:
							Canvas.SetTop(SpritesheetEditor_CropImage, Canvas.GetTop(SpritesheetEditor_CropImage) - 1);
							break;
						case Key.Down:
							Canvas.SetTop(SpritesheetEditor_CropImage, Canvas.GetTop(SpritesheetEditor_CropImage) + 1);
							break;
					}

					SpritesheetEditor_CropImage.Focus();
				}
			}

		}

		private void SpriteSheetEditor_CE_TV_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeView treeViewItem = sender as TreeView;
			if (treeViewItem != null)
			{
				if (treeViewItem.SelectedItem is CanvasAnimation canvasAnimationProp)
				{
					ObjectProperties_Control.Template =
						(ControlTemplate)this.Resources["SpritesheetEditorProperties_Anim_Template"];
					ObjectProperties_Control.UpdateLayout();

					currentCanvasImagePropertiesSelected = null;
					currentCanvasAnimationPropertiesSelected = canvasAnimationProp;

					ItemsControl SubLayerIC =
						(ItemsControl)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_SubLayerInfo_IC",
							ObjectProperties_Control);
					SubLayerIC.ItemsSource = canvasAnimationProp.NamesOfSubLayers;


				}
				else if (treeViewItem.SelectedItem is CanvasImageProperties canvasFrame)
				{
					if (canvasFrame.LinkedBorderImage is Border parentBorder)
					{
						ObjectProperties_Control.Template =
							(ControlTemplate)this.Resources["SpritesheetEditorProperties_Template"];
						ObjectProperties_Control.UpdateLayout();
						// parentBorder.BorderThickness = new Thickness(1);

						// We need to create the croppable control again.

						// We are going to fill in the frame properties.

						// Keep track of the frame property
						currentCanvasImagePropertiesSelected = canvasFrame;
						currentCanvasAnimationPropertiesSelected = null;

						TextBox FrameNumber_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_FrameNumber_TB",
								ObjectProperties_Control);
						FrameNumber_TB.Text = GetFrameNumberFromFrame(canvasFrame).ToString();

						TextBox ImagePath_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_ImagePath_TB",
								ObjectProperties_Control);
						ImagePath_TB.Text = canvasFrame.ImageLocation.Substring(canvasFrame.ImageLocation.LastIndexOf("\\"));
						ImagePath_TB.ToolTip = canvasFrame.ImageLocation;

						TextBox XPosition_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_XPos_TB",
								ObjectProperties_Control);
						XPosition_TB.Text = canvasFrame.X.ToString();

						TextBox YPosition_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_YPos_TB",
								ObjectProperties_Control);
						YPosition_TB.Text = canvasFrame.Y.ToString();

						TextBox Width_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_Width_TB",
								ObjectProperties_Control);
						Width_TB.Text = canvasFrame.W.ToString();

						TextBox Height_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_Height_TB",
								ObjectProperties_Control);
						Height_TB.Text = canvasFrame.H.ToString();

						TextBox CropX_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_CropWidth_TB",
								ObjectProperties_Control);
						CropX_TB.Text = canvasFrame.CropX.ToString();

						TextBox CropY_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_CropHeight_TB",
								ObjectProperties_Control);
						CropY_TB.Text = canvasFrame.CropY.ToString();

						TextBox RenderPointX_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_RenderPointX_TB",
								ObjectProperties_Control);
						RenderPointX_TB.Text = canvasFrame.RX.ToString();

						TextBox RenderPointY_TB =
							(TextBox)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_RenderPointY_TB",
								ObjectProperties_Control);
						RenderPointY_TB.Text = canvasFrame.RY.ToString();

						// We need to Fill in the ITEM CONTROL of sublayers now!
						ItemsControl SubLayerIC =
							(ItemsControl)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_SubLayerInfo_IC",
								ObjectProperties_Control);
						CanvasAnimation canvasAnimation = GetCanvasAnimationFromFrame(canvasFrame);
						if (canvasAnimation != null)
						{
							SubLayerIC.Items.Clear();
							for (int i = 0; i < canvasAnimation.NamesOfSubLayers.Count; i++)
							{
								if (canvasFrame.SubLayerPoints.Count < canvasAnimation.NamesOfSubLayers.Count)
								{
									CanvasSubLayerPoint subRenderPoint = new CanvasSubLayerPoint()
									{ LayerName = canvasAnimation.NamesOfSubLayers[i] };
									SubLayerIC.Items.Add(subRenderPoint);
									canvasFrame.SubLayerPoints.Add(subRenderPoint);
								}
								else
								{
									// We don't need to add a new layer here. it's already been added
									SubLayerIC.Items.Add(canvasFrame.SubLayerPoints[i]);
								}
							}
						}

					}
				}
			}
		}

		private void Spritesheet_OE_Delete_Frame_BTN_Click(object sender, RoutedEventArgs e)
		{
			// We need to make sure we know where to add this.
			Button btn = sender as Button;
			if (btn != null)
			{
				// We need to play WPF games... aka find the template, and then with that find the acutal control (treeview)
				ControlTemplate spriteSheeControlTemplate = (ControlTemplate)this.Resources["SpriteSheetObjects_Template"];
				TreeView spritesheetTreeView =
					(TreeView)spriteSheeControlTemplate.FindName("SpriteSheetEditor_CE_TV", ContentLibrary_Control);
				TreeViewItem tvi = FindParentTreeViewItem(btn);

				// Get the actual index of the list from the BUTTON we pressed
				CanvasImageProperties foundFrame = tvi.DataContext as CanvasImageProperties; // MAGIC BULLSHIT
				if (foundFrame != null)
				{
					// We found the data we needed. so let's delete the information from the canvas
					if (foundFrame.LinkedBorderImage != null)
						SpritesheetEditor_Canvas.Children.Remove(foundFrame.LinkedBorderImage);
					if (foundFrame.LinkedCroppableImage != null)
					{
						SpritesheetEditor_CropImage.bHasFocus = false;
						SpritesheetEditor_CropImage.Visibility = Visibility.Hidden;
					}

					RemoveCanvasFrame(foundFrame);
				}
			}
		}

		private void SpritesheetCanvasFrame_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;

			TextBox tb = sender as TextBox;
			if (tb != null)
			{
				if (int.TryParse(tb.Text, out int val))
				{

					// We need to play WPF games... aka find the template, and then with that find the acutal control (treeview)
					ControlTemplate spriteSheeControlTemplate = (ControlTemplate)this.Resources["SpriteSheetObjects_Template"];
					TreeView spritesheetTreeView =
						(TreeView)spriteSheeControlTemplate.FindName("SpriteSheetEditor_CE_TV", ContentLibrary_Control);
					TreeViewItem tvi = FindParentTreeViewItem(tb);

					// Get the actual index of the list from the BUTTON we pressed
					CanvasImageProperties foundFrame = tvi.DataContext as CanvasImageProperties; // MAGIC BULLSHIT
					if (foundFrame != null)
					{

						string propertyTag = tb.Tag.ToString();
						if (propertyTag == "x")
						{
							if (foundFrame.LinkedBorderImage != null)
							{
								Canvas.SetLeft(foundFrame.LinkedBorderImage, val);
							}
							else if (foundFrame.LinkedCroppableImage != null)
							{
								Canvas.SetLeft(foundFrame.LinkedCroppableImage, val);
							}
						}
						else if (propertyTag == "y")
						{
							if (foundFrame.LinkedBorderImage != null)
							{
								Canvas.SetTop(foundFrame.LinkedBorderImage, val);
							}
							else if (foundFrame.LinkedCroppableImage != null)
							{
								Canvas.SetTop(foundFrame.LinkedCroppableImage, val);
							}
						}
						else if (propertyTag == "w")
						{
							if (foundFrame.LinkedBorderImage != null)
							{
								int index = Array.FindIndex<Image>((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children.OfType<Image>().ToArray(),
									x => (x as Image).Name.IndexOf("preview") > 0);
								if (index >= 0)
								{
									foundFrame.LinkedBorderImage.Width = val;
									(foundFrame.LinkedBorderImage.Child as Canvas).Width = val;
									((foundFrame.LinkedBorderImage.Child as Canvas).Children[index] as Image).Width = val;
								}
							}
							else if (foundFrame.LinkedCroppableImage != null)
							{
								// foundFrame.LinkedCroppableImage.MaxWidth = val;
								foundFrame.LinkedCroppableImage.Width = val;
							}
						}
						else if (propertyTag == "h")
						{
							if (foundFrame.LinkedBorderImage != null)
							{
								int index = Array.FindIndex<Image>((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children.OfType<Image>().ToArray(),
									x => (x as Image).Name.IndexOf("preview") > 0);
								if (index >= 0)
								{
									foundFrame.LinkedBorderImage.Width = val;
									(foundFrame.LinkedBorderImage.Child as Canvas).Width = val;
									((foundFrame.LinkedBorderImage.Child as Canvas).Children[index] as Image).Height = val;
								}

							}
							else if (foundFrame.LinkedCroppableImage != null)
							{
								// foundFrame.LinkedCroppableImage.MaxWidth = val;
								foundFrame.LinkedCroppableImage.Height = val;
							}
						}

					}
				}
			}
		}

		private void SpritesheetCanvasFrame_CB_Click(object sender, RoutedEventArgs e)
		{
			CheckBox cb = sender as CheckBox;
			if (cb != null)
			{


				// We need to play WPF games... aka find the template, and then with that find the acutal control (treeview)
				ControlTemplate spriteSheeControlTemplate = (ControlTemplate)this.Resources["SpriteSheetObjects_Template"];
				TreeView spritesheetTreeView =
					(TreeView)spriteSheeControlTemplate.FindName("SpriteSheetEditor_CE_TV", ContentLibrary_Control);
				TreeViewItem tvi = FindParentTreeViewItem(cb);

				// Get the actual index of the list from the BUTTON we pressed
				CanvasImageProperties foundFrame = tvi.DataContext as CanvasImageProperties; // MAGIC BULLSHIT
				if (foundFrame != null)
				{
					if (cb.IsChecked == true)
					{
						if (foundFrame.LinkedBorderImage != null)
							foundFrame.LinkedBorderImage.BorderThickness = new Thickness(1);
						else
							foundFrame.LinkedCroppableImage.BorderThickness = new Thickness(1);

					}
					else
					{
						{
							if (foundFrame.LinkedBorderImage != null)
								foundFrame.LinkedBorderImage.BorderThickness = new Thickness(0);
							else
								foundFrame.LinkedCroppableImage.BorderThickness = new Thickness(0);
						}
					}
				}
			}
		}

		private void SpriteSheetPropertyFrameXPos_TB_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null && int.TryParse(textBox.Text, out int dataVal))
			{
				if (e.Key == Key.Enter)
				{
					if (currentCanvasImagePropertiesSelected != null)
					{
						currentCanvasImagePropertiesSelected.X = dataVal;
						if (currentCanvasImagePropertiesSelected.LinkedBorderImage != null)
							Canvas.SetLeft(currentCanvasImagePropertiesSelected.LinkedBorderImage, dataVal);
						else if (currentCanvasImagePropertiesSelected.LinkedCroppableImage != null)
							Canvas.SetLeft(currentCanvasImagePropertiesSelected.LinkedCroppableImage, dataVal);
					}
				}
			}
		}

		private void SpriteSheetEditorProperties_YPos_TB_OnKeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null && int.TryParse(textBox.Text, out int dataVal))
			{
				if (e.Key == Key.Enter)
				{
					if (currentCanvasImagePropertiesSelected != null)
					{
						currentCanvasImagePropertiesSelected.Y = dataVal;
						if (currentCanvasImagePropertiesSelected.LinkedBorderImage != null)
							Canvas.SetTop(currentCanvasImagePropertiesSelected.LinkedBorderImage, dataVal);
						else if (currentCanvasImagePropertiesSelected.LinkedCroppableImage != null)
							Canvas.SetTop(currentCanvasImagePropertiesSelected.LinkedCroppableImage, dataVal);
					}
				}
			}
		}

		private void SpriteSheetEditorProperties_Width_TB_OnKeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null && int.TryParse(textBox.Text, out int dataVal))
			{
				if (e.Key == Key.Enter)
				{
					if (currentCanvasImagePropertiesSelected != null)
					{
						currentCanvasImagePropertiesSelected.W = dataVal;
						if (currentCanvasImagePropertiesSelected.LinkedBorderImage != null)
						{
							int index = Array.FindIndex<Image>((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children.OfType<Image>().ToArray(),
								x => (x as Image).Name.IndexOf("preview") > 0);
							if (index >= 0)
							{
								currentCanvasImagePropertiesSelected.LinkedBorderImage.Width = dataVal;
								(currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Width = dataVal;
								((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[index] as Image).Width =
									dataVal;
							}
						}
						else if (currentCanvasImagePropertiesSelected.LinkedCroppableImage != null)
						{
							currentCanvasImagePropertiesSelected.LinkedCroppableImage.Width = dataVal;
						}
					}
				}
			}
		}


		private void SpriteSheetEditorProperties_Height_TB_OnKeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null && int.TryParse(textBox.Text, out int dataVal))
			{
				if (e.Key == Key.Enter)
				{
					if (currentCanvasImagePropertiesSelected != null)
					{
						currentCanvasImagePropertiesSelected.H = dataVal;
						if (currentCanvasImagePropertiesSelected.LinkedBorderImage != null)
						{
							int index = Array.FindIndex<Image>((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children.OfType<Image>().ToArray(),
								x => (x as Image).Name.IndexOf("preview") > 0);
							if (index >= 0)
							{
								currentCanvasImagePropertiesSelected.LinkedBorderImage.Height = dataVal;
								(currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Height = dataVal;
								((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[index] as Image).Height =
									dataVal;
							}


							
						}
						else if (currentCanvasImagePropertiesSelected.LinkedCroppableImage != null)
						{
							currentCanvasImagePropertiesSelected.LinkedCroppableImage.Height = dataVal;
						}
					}
				}
			}
		}


		private void SpriteSheetEditorProperties_RenderPointX_TB_OnKeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null && int.TryParse(textBox.Text, out int dataVal))
			{
				if (e.Key == Key.Enter)
				{
					if (currentCanvasImagePropertiesSelected != null)
					{
						currentCanvasImagePropertiesSelected.RX = dataVal;
						if (currentCanvasImagePropertiesSelected.LinkedBorderImage != null)
						{
							int index = Array.FindIndex<Image>((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children.OfType<Image>().ToArray(),
								x => (x as Image).Name.IndexOf("Base") > 0);
							if (index >= 0)
							{
								Canvas.SetLeft(
									((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[index] as Image),
									dataVal);
							}
						}
						else if (currentCanvasImagePropertiesSelected.LinkedCroppableImage != null)
						{
							// This doesn't exist right now
						}
					}
				}
			}
		}

		private void SpriteSheetEditorProperties_RenderPointY_TB_OnKeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null && int.TryParse(textBox.Text, out int dataVal))
			{
				if (e.Key == Key.Enter)
				{
					if (currentCanvasImagePropertiesSelected != null)
					{
						currentCanvasImagePropertiesSelected.RY = dataVal;
						if (currentCanvasImagePropertiesSelected.LinkedBorderImage != null)
						{
							int index = Array.FindIndex<Image>((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children.OfType<Image>().ToArray(),
								x => (x as Image).Name.IndexOf("Base") > 0);
							if (index >= 0)
							{
								Canvas.SetTop(
									((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[index] as Image),
									dataVal);
							}
						}
						else if (currentCanvasImagePropertiesSelected.LinkedCroppableImage != null)
						{
							// This doesn't exist right now
						}
					}
				}
			}
		}


		private void SpriteSheetEditorProperties_ShowBorder_CB_Checked(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = sender as CheckBox;
			if (checkBox != null)
			{
				if (checkBox.IsChecked == true)
				{
					if (currentCanvasImagePropertiesSelected.LinkedBorderImage != null)
						currentCanvasImagePropertiesSelected.LinkedBorderImage.BorderThickness = new Thickness(1);
					else
						currentCanvasImagePropertiesSelected.LinkedCroppableImage.BorderThickness = new Thickness(1);

				}
				else
				{
					{
						if (currentCanvasImagePropertiesSelected.LinkedBorderImage != null)
							currentCanvasImagePropertiesSelected.LinkedBorderImage.BorderThickness = new Thickness(0);
						else
							currentCanvasImagePropertiesSelected.LinkedCroppableImage.BorderThickness = new Thickness(0);
					}
				}
			}
		}


		private void SpriteSheetEditorProperties_ShowRenderPoint_CB_Checked(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = sender as CheckBox;
			if (checkBox != null)
			{
				if (checkBox.IsChecked == true)
				{
					if (currentCanvasImagePropertiesSelected?.LinkedBorderImage != null)
					{
						int index = Array.FindIndex<Image>((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children.OfType<Image>().ToArray(),
							x => (x as Image).Name.IndexOf("Base") > 0);
						if (index >= 0)
						{
							(currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[index].Visibility =
								Visibility.Visible;
						}
					}
				}
				else
				{
					int index = Array.FindIndex<Image>((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children.OfType<Image>().ToArray(),
						x => (x as Image).Name.IndexOf("Base") > 0);
					if (index >= 0)
					{
						(currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[index].Visibility =
							Visibility.Hidden;
					}
				}
			}
		}

		private void SpriteSheetEditorProperties_AddSubLayer_TB_OnKeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				if (e.Key == Key.Enter)
				{
					if (currentCanvasAnimationPropertiesSelected != null)
					{
						int index = GetItemIndex(textBox);
						if (index >= 0)
						{
							currentCanvasAnimationPropertiesSelected.NamesOfSubLayers[index] = textBox.Text;
							foreach (var frame in currentCanvasAnimationPropertiesSelected.CanvasFrames)
							{
								frame.SubLayerPoints[index].LayerName = textBox.Text;
							}
						}
					}
				}
			}
		}

		private void SpriteSheetEditorProperties_AddSubLayer_BTN_Click(object sender, RoutedEventArgs e)
		{
			ItemsControl SubLayerIC =
				(ItemsControl)ObjectProperties_Control.Template.FindName("SpriteSheetEditorProperties_SubLayerInfo_IC",
					ObjectProperties_Control);
			foreach (var frame in currentCanvasAnimationPropertiesSelected.CanvasFrames)
			{
				Image renderPointImage = new Image();
				renderPointImage.Name = String.Format( "Render_Sub_Point_Image{0}", (SubLayerIC.Items.Count + 1));
				renderPointImage.Source =
					new BitmapImage(new Uri(
						String.Format("{0}/Resources/render_point_crosshair_{1}.png", Directory.GetCurrentDirectory(),
							(SubLayerIC.Items.Count + 1).ToString()),
						UriKind.Absolute));

				Canvas overlayCanvas = new Canvas() { Width = frame.W, Height = frame.H };
				(frame.LinkedBorderImage.Child as Canvas)?.Children.Add(renderPointImage);

				frame.SubLayerPoints.Add(new CanvasSubLayerPoint()
				{
					LinkedImage = renderPointImage,
					LayerName = "Layer " + (SubLayerIC.Items.Count)
				});
			}

			currentCanvasAnimationPropertiesSelected.NamesOfSubLayers.Add("Layer " + SubLayerIC.Items.Count);
		}


		private int GetItemIndex(FrameworkElement element)
		{
			ItemsControl itemsControl = FindParentItemsControl(element);
			if (itemsControl != null)
			{
				return itemsControl.Items.IndexOf(element.DataContext);
			}

			return -1;
		}

		private ItemsControl FindParentItemsControl(FrameworkElement element)
		{
			FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
			while (parent != null)
			{
				if (parent is ItemsControl itemsControl)
				{
					return itemsControl;
				}

				parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
			}

			return null;
		}

		private static T FindParent<T>(DependencyObject child) where T : DependencyObject
		{
			DependencyObject parentObject = VisualTreeHelper.GetParent(child);

			if (parentObject == null)
				return null;

			T parent = parentObject as T;
			return parent ?? FindParent<T>(parentObject);
		}

		private void SpriteSheetEditor_SubLayerRX_Keydown(object sender, KeyEventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (e.Key == Key.Enter)
			{
				if (int.TryParse(tb.Text, out int rxVal))
				{
					if (tb != null)
					{
						// We need to play WPF games... aka find the template, and then with that find the acutal control (treeview)
						ControlTemplate spriteSheeControlTemplate =
							(ControlTemplate)this.Resources["SpritesheetEditorProperties_Template"];
						ItemsControl spritesheetTreeView =
							(ItemsControl)spriteSheeControlTemplate.FindName("SpriteSheetEditorProperties_SubLayerInfo_IC",
								ObjectProperties_Control);

						// Get the actual index of the list from the BUTTON we pressed
						CanvasSubLayerPoint subLayerPoint = tb.DataContext as CanvasSubLayerPoint;
						int foundFrameIndex = spritesheetTreeView.Items.IndexOf(tb.DataContext); // MAGIC BULLSHIT
						if (foundFrameIndex >= 0)
						{
							Canvas.SetLeft((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[2 + foundFrameIndex], rxVal);
						}
					}
				}
			}
		}

		private void SpriteSheetEditor_SubLayerRY_Keydown(object sender, KeyEventArgs e)
		{
			TextBox tb = sender as TextBox;

			if (e.Key == Key.Enter)
			{
				if (int.TryParse(tb.Text, out int ryVal))
				{
					if (tb != null)
					{
						// We need to play WPF games... aka find the template, and then with that find the acutal control (treeview)
						ControlTemplate spriteSheeControlTemplate =
							(ControlTemplate)this.Resources["SpritesheetEditorProperties_Template"];
						ItemsControl spritesheetTreeView =
							(ItemsControl)spriteSheeControlTemplate.FindName("SpriteSheetEditorProperties_SubLayerInfo_IC",
								ObjectProperties_Control);

						// Get the actual index of the list from the BUTTON we pressed
						CanvasSubLayerPoint subLayerPoint = tb.DataContext as CanvasSubLayerPoint;
						int foundFrameIndex = spritesheetTreeView.Items.IndexOf(tb.DataContext); // MAGIC BULLSHIT
						if (foundFrameIndex >= 0)
						{
							Canvas.SetTop((currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[2 + foundFrameIndex], ryVal);
						}
					}
				}
			}
		}

		private void SpriteSheetEditor_ShowSubLayer_CB_Click(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = sender as CheckBox;
			if (checkBox != null)
			{
				CanvasSubLayerPoint subLayerPoint = checkBox.DataContext as CanvasSubLayerPoint;
				if (subLayerPoint != null)
				{
					// We need to play WPF games... aka find the template, and then with that find the acutal control (treeview)
					ControlTemplate spriteSheeControlTemplate =
						(ControlTemplate)this.Resources["SpritesheetEditorProperties_Template"];
					ItemsControl spritesheetTreeView =
						(ItemsControl)spriteSheeControlTemplate.FindName("SpriteSheetEditorProperties_SubLayerInfo_IC",
							ObjectProperties_Control);
					int index = spritesheetTreeView.Items.IndexOf(subLayerPoint);
					if (checkBox.IsChecked == true)
					{
						if (subLayerPoint.LinkedImage != null)
						{
							subLayerPoint.LinkedImage.Visibility = Visibility.Visible;
							(currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[2 + index].Visibility = Visibility.Visible;

						}
						//else
						//	currentCanvasImagePropertiesSelected.LinkedCroppableImage.BorderThickness = new Thickness(1);

					}
					else
					{
						if (currentCanvasImagePropertiesSelected.LinkedBorderImage != null)
						{
							subLayerPoint.LinkedImage.Visibility = Visibility.Hidden;
							(currentCanvasImagePropertiesSelected.LinkedBorderImage.Child as Canvas).Children[2 + index].Visibility = Visibility.Hidden;
						}
						//else
						//	currentCanvasImagePropertiesSelected.LinkedCroppableImage.BorderThickness = new Thickness(0);
					}
				}
			}
		}


		private void NewSpriteSheetAs_MI_Click(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void OpenSpritesheet_MI_Click(object sender, RoutedEventArgs e)
		{
			// Get a new image file
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Import Sprite Sheet File",
				FileName = "", //default file name
				Filter = "Sprite Sheet files (*.spritesheet)|*.spritesheet",
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				// filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] {'/', '\\'}));
			}
			else return; //invalid name

			Console.WriteLine(dlg.FileName);

			CurrentSelectedSpriteSheet = CanvasSpritesheet.ImportSpriteSheet(dlg.FileName);

			if (CurrentSelectedSpriteSheet != null)
			{
				if (SpritesheetEditor_CropImage.updateSizeLocation_Hook == null)
					SpritesheetEditor_CropImage.updateSizeLocation_Hook += UpdateSizeLocationHook_FrameInfo;
				if (SpritesheetEditor_CropImage.updateCropLocation_Hook == null)
					SpritesheetEditor_CropImage.updateCropLocation_Hook += UpdateCropLocationHook_FrameInfo;
				if (SpritesheetEditor_CropImage.setRenderPoint_Hook == null)
					SpritesheetEditor_CropImage.setRenderPoint_Hook += UpdateRenderPointLocationHook_FrameInfo;

				var imageControls = SpritesheetEditor_Canvas.Children.OfType<Border>().ToList();
				SpritesheetEditor_BackCanvas.Width = CurrentSelectedSpriteSheet.Width;
				SpritesheetEditor_BackCanvas.Height = CurrentSelectedSpriteSheet.Height;
				SpritesheetEditor_Canvas.Width = CurrentSelectedSpriteSheet.Width;
				SpritesheetEditor_Canvas.Height = CurrentSelectedSpriteSheet.Height;

				// Remove all Image controls from the Canvas
				SpriteSheet_CE_Tree.ItemsSource = null;
				foreach (var imageControl in imageControls)
				{
					SpritesheetEditor_Canvas.Children.Remove(imageControl);
				}

				// Set the Item Source for the Content Editor
				SpriteSheet_CE_Tree.ItemsSource = CurrentSelectedSpriteSheet.AllAnimationOnSheet;

				foreach (CanvasAnimation canvasAnimation in CurrentSelectedSpriteSheet.AllAnimationOnSheet)
				{
					foreach (CanvasImageProperties frame in canvasAnimation.CanvasFrames)
					{
						Border parentBorder = CreateDashedLineBorder();

						Image image = new Image();
						image.Name = "cropped_image_preview";
						BitmapImage bitmap = new BitmapImage(new Uri(frame.ImageLocation));
						CroppedBitmap croppedBitmap = new CroppedBitmap(bitmap,
							new Int32Rect(frame.CropX, frame.CropY, (int)frame.W, (int)frame.H));
						image.Source = croppedBitmap;
						image.Width = frame.W;
						image.Height = frame.H;
						image.HorizontalAlignment = HorizontalAlignment.Center;
						image.VerticalAlignment = VerticalAlignment.Center;
						image.PreviewMouseLeftButtonDown +=
							new MouseButtonEventHandler(LeftMouseDowndOnImageFrame_SpriteSheetEditor_CB);
						image.PreviewMouseLeftButtonUp +=
							new MouseButtonEventHandler(LeftMouseUpdOnImageFrame_SpriteSheetEditor_CB);

						// Render the BASE RENDER POINT
						Image renderPointImage = new Image();
						renderPointImage.Name = "Render_Base_Point_Image";
						renderPointImage.Source =
							new BitmapImage(new Uri(
								String.Format("{0}/Resources/render_point_crosshair_base.png", Directory.GetCurrentDirectory()),
								UriKind.Absolute));

						Canvas overlayCanvas = new Canvas() { Width = frame.W, Height = frame.H };
						overlayCanvas.Children.Add(image);
						overlayCanvas.Children.Add(renderPointImage);

						// Render all the sub layers render points
						int subLayerCount = 1;
						foreach (var subLayerPoint in frame.SubLayerPoints)
						{
							Image subPointImage = new Image();
							subPointImage.Name = String.Format("Render_SubPoint_Image_{0}", subLayerCount);
							subPointImage.Source =
								new BitmapImage(new Uri(
									String.Format("{0}/Resources/render_point_crosshair_{1}.png", Directory.GetCurrentDirectory(), subLayerCount++),
									UriKind.Absolute));

							overlayCanvas.Children.Add(subPointImage);
							subLayerPoint.LinkedImage = subPointImage;
							Canvas.SetLeft(subPointImage, subLayerPoint.RX);
							Canvas.SetTop(subPointImage, subLayerPoint.RY);
						}

						parentBorder.Child = overlayCanvas;
						frame.LinkedBorderImage = parentBorder;

						SpritesheetEditor_Canvas.Children.Add(parentBorder);

						Canvas.SetLeft(parentBorder, frame.X);
						Canvas.SetTop(parentBorder, frame.Y);

						// Place the Render point in the correct position
						Canvas.SetLeft(renderPointImage, frame.RX);
						Canvas.SetTop(renderPointImage, frame.RY);
					}
				}
			}
		}

		private void SaveSpriteSheet_MI_Click(object sender, RoutedEventArgs e)
		{

			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
			{
				Title = "Save Spritesheet",
				FileName = "", //default file name
				Filter = "Sprite Sheet (*.spritesheet)|*.spritesheet|All files (*.*)|*.*",
				FilterIndex = 2,
				InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Animations"),
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

			Console.WriteLine("Saving Spritesheet");

			CanvasSpritesheet.ExportSpriteSheet(CurrentSelectedSpriteSheet, dlg.FileName.Replace(".spritesheet", ""));

			// Let's not let frames render target crosshairs be visiable
			foreach (var anim in CurrentSelectedSpriteSheet.AllAnimationOnSheet)
			{
				foreach (var frame in anim.CanvasFrames)
				{
					// This is the Cross hair for the render point
					(frame.LinkedBorderImage?.Child as Canvas).Children[1].Visibility = Visibility.Hidden;
					((Border) frame.LinkedBorderImage).Visibility = Visibility.Visible;
					((Border) frame.LinkedBorderImage).BorderThickness = new Thickness(0);

					foreach (CanvasSubLayerPoint subLayerPoint in frame.SubLayerPoints)
					{
						if (subLayerPoint.LinkedImage != null)
						{
							subLayerPoint.LinkedImage.Visibility = Visibility.Hidden;
						}
							
					}
				}
			}

			SpritesheetEditor_Canvas.UpdateLayout();

			// Render the canvas and its child elements onto a RenderTargetBitmap
			var renderTargetBitmap = new RenderTargetBitmap((int)CurrentSelectedSpriteSheet.Width,
				(int)CurrentSelectedSpriteSheet.Height, 96, 96, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(SpritesheetEditor_Canvas);

			// Create a PngBitmapEncoder and add the RenderTargetBitmap to it
			var pngEncoder = new PngBitmapEncoder();
			pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

			// Save the PngBitmapEncoder to a file or stream
			var fileStream = new FileStream(String.Format("{0}.png", dlg.FileName.Replace(".spritesheet", "")),
				FileMode.Create);
			pngEncoder.Save(fileStream);
			fileStream.Close();
		}

		private void SaveSpriteSheetAs_MI_Click(object sender, RoutedEventArgs e)
		{
			// throw new NotImplementedException();
		}

		private void SpritesheetEditor_BackCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			// throw new NotImplementedException();
		}

		/// <summary>
		/// handles mouse scroll events
		/// Zooming is handled here.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SpritesheetEditor_BackCanvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
		{

			Console.WriteLine("canvas scroollll to zoom");


			if (e.Delta > 0) //zoom in!
			{
				SpritesheetEditorZoomLevel += .2;
				SpritesheetEditor_VB.Transform = new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				SpritesheetEditor_Canvas.RenderTransform =
					new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				Console.WriteLine(String.Format("W:{0},  H{1}", SpritesheetGridWidth, SpritesheetGridHeight));

				SpritesheetEditor_VB.Transform = new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				SpritesheetEditor_BackCanvas.RenderTransform =
					new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				Console.WriteLine(String.Format("W:{0},  H{1}", SpritesheetGridWidth, SpritesheetGridHeight));

				// Update the scroll viewer
				var scaleTransform = new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				SpritesheetEditor_Canvas.LayoutTransform = scaleTransform;

				// Adjust scroll position based on zoom
				SpritesheetEditor_ScrollViewer.ScrollToHorizontalOffset(SpritesheetEditor_ScrollViewer.HorizontalOffset *
																																SpritesheetEditorZoomLevel);
				SpritesheetEditor_ScrollViewer.ScrollToVerticalOffset(SpritesheetEditor_ScrollViewer.VerticalOffset *
																															SpritesheetEditorZoomLevel);

				SpritesheetEditor_ScrollViewer.InvalidateScrollInfo();


				//TODO: resize selection rectangle
			}
			else //zoom out!
			{
				SpritesheetEditorZoomLevel -= .2;
				SpritesheetEditor_VB.Transform = new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				SpritesheetEditor_Canvas.RenderTransform =
					new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				Console.WriteLine(String.Format("W:{0},  H{1}", SpritesheetGridWidth, SpritesheetGridHeight));

				SpritesheetEditor_VB.Transform = new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				SpritesheetEditor_BackCanvas.RenderTransform =
					new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				Console.WriteLine(String.Format("W:{0},  H{1}", SpritesheetGridWidth, SpritesheetGridHeight));

				// Update the scroll viewer
				var scaleTransform = new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				SpritesheetEditor_Canvas.LayoutTransform = scaleTransform;

				// Adjust scroll position based on zoom
				SpritesheetEditor_ScrollViewer.ScrollToHorizontalOffset(SpritesheetEditor_ScrollViewer.HorizontalOffset *
																																SpritesheetEditorZoomLevel);
				SpritesheetEditor_ScrollViewer.ScrollToVerticalOffset(SpritesheetEditor_ScrollViewer.VerticalOffset *
																															SpritesheetEditorZoomLevel);

				SpritesheetEditor_ScrollViewer.InvalidateScrollInfo();


				//TODO: resize selection rectangle
			}

			if (SpritesheetEditorZoomLevel < .2)
			{
				SpritesheetEditorZoomLevel = .2;
				SpritesheetEditor_VB.Transform = new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				SpritesheetEditor_Canvas.RenderTransform =
					new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);

				SpritesheetEditor_VB.Transform = new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				SpritesheetEditor_BackCanvas.RenderTransform =
					new ScaleTransform(SpritesheetEditorZoomLevel, SpritesheetEditorZoomLevel);
				return;
			} //do not allow this be 0 which in turn / by 0;

			SpritesheetZoomFactor_TB.Text = String.Format("({0})%  ({1}x{1})", 100 * SpritesheetEditorZoomLevel,
				SpritesheetGridWidth * SpritesheetEditorZoomLevel);
			ScaleSpriteSheetEditorCanvas();

			SpritesheetEditor_BackCanvas.ReleaseMouseCapture();
			e.Handled = true;
		}

		private void ScaleSpriteSheetEditorCanvas()
		{
			//Level TempLevel = CurrentLevel;
			//if (TempLevel == null) return; //TODO: Remove after i make force select work on tree view.
			//FullMapLEditor_Canvas.Width = (int)TempLevel.GetPropertyData("xCells") * 10;
			//FullMapLEditor_Canvas.Height = (int)TempLevel.GetPropertyData("yCells") * 10;

			//FullMapLEditor_VB.Viewport = new Rect(0, 0, 10, 10);

			//int MainCurCellsX = (int)Math.Ceiling(LevelEditor_BackCanvas.RenderSize.Width / (40 * LEZoomLevel));
			//int MainCurCellsY = (int)Math.Ceiling(LevelEditor_BackCanvas.RenderSize.Height / (40 * LEZoomLevel));

			//double pastx, pasty = 0;
			//pastx = Canvas.GetLeft(FullMapSelection_Rect);
			//pasty = Canvas.GetTop(FullMapSelection_Rect);

			//FullMapLEditor_Canvas.Children.Remove(FullMapSelection_Rect);
			//FullMapSelection_Rect = new Rectangle()
			//{
			//	Width = MainCurCellsX * 10,
			//	Height = MainCurCellsY * 10,
			//	Stroke = Brushes.White,
			//	StrokeThickness = 1,
			//	Name = "SelectionRect"
			//};
			//Canvas.SetLeft(FullMapSelection_Rect, pastx);
			//Canvas.SetTop(FullMapSelection_Rect, pasty);
			//Canvas.SetZIndex(FullMapSelection_Rect, 100); //100 is the selection layer.

			//FullMapLEditor_Canvas.Children.Add(FullMapSelection_Rect);
		}

		private void SpritesheetEditor_BackCanvas_OnDragOver(object sender, DragEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void AddNewSpriteAnimation_BTN_Click(object sender, RoutedEventArgs e)
		{
			// Step one find the content control
			SpriteSheet_CE_Tree =
				(TreeView)ContentLibrary_Control.Template.FindName("SpriteSheetEditor_CE_TV", ContentLibrary_Control);

			if (SpriteSheet_CE_Tree != null)
			{
				// When testing i don't have "starting screen for this so it's null. this will be removed later
				if (CurrentSelectedSpriteSheet == null)
				{
					// FOR TESTING ONLY
					CurrentSelectedSpriteSheet = new CanvasSpritesheet("Testing_Sheet", 1000, 1000);
				}

				// If for some dumbass reason i forgot to set it... then set it
				if (SpriteSheet_CE_Tree.ItemsSource == null)
				{
					SpriteSheet_CE_Tree.ItemsSource = CurrentSelectedSpriteSheet.AllAnimationOnSheet;
				}

				SpriteSheet_CE_Tree.ItemsSource = null;

				CurrentSelectedSpriteSheet.AllAnimationOnSheet.Add(new CanvasAnimation("Testing_Idle1"));

				// SpriteSheet_CE_Tree.ItemsSource = CurrentSelectedSpriteSheet;
				SpriteSheet_CE_Tree.ItemsSource = CurrentSelectedSpriteSheet.AllAnimationOnSheet;


			}
		}

		private bool RemoveCanvasFrame(CanvasImageProperties frameToDelete)
		{
			for (int i = 0; i < CurrentSelectedSpriteSheet.AllAnimationOnSheet.Count; i++)
			{
				CanvasAnimation canvasAnim = CurrentSelectedSpriteSheet.AllAnimationOnSheet[i];
				for (int j = 0; j < canvasAnim.CanvasFrames.Count; j++)
				{
					CanvasImageProperties frame = canvasAnim.CanvasFrames[j];
					if (frame == frameToDelete)
					{
						return canvasAnim.CanvasFrames.Remove(frame);
					}
				}
			}

			return false;
		}

		private CanvasImageProperties FindCanvasFrame(Border linkedBorder)
		{
			foreach (CanvasAnimation canvasAnim in CurrentSelectedSpriteSheet.AllAnimationOnSheet)
			{
				foreach (CanvasImageProperties frame in canvasAnim.CanvasFrames)
				{
					if (frame.LinkedBorderImage == linkedBorder)
						return frame;
				}
			}

			return null;
		}

		private CanvasImageProperties FindCanvasFrame(CroppableImage croppableImage)
		{
			foreach (CanvasAnimation canvasAnim in CurrentSelectedSpriteSheet.AllAnimationOnSheet)
			{
				foreach (CanvasImageProperties frame in canvasAnim.CanvasFrames)
				{
					if (frame.LinkedCroppableImage == croppableImage)
						return frame;
				}
			}

			return null;
		}

		private void Spritesheet_OE_Add_Frame_BTN_Click(object sender, RoutedEventArgs e)
		{
			// We need to make sure we know where to add this.
			Button btn = sender as Button;
			if (btn != null)
			{
				// We need to play WPF games... aka find the template, and then with that find the acutal control (treeview)
				ControlTemplate spriteSheeControlTemplate = (ControlTemplate)this.Resources["SpriteSheetObjects_Template"];
				TreeView spritesheetTreeView =
					(TreeView)spriteSheeControlTemplate.FindName("SpriteSheetEditor_CE_TV", ContentLibrary_Control);
				TreeViewItem tvi = FindParentTreeViewItem(btn);

				// Get the actual index of the list from the BUTTON we pressed
				int animationIndex =
					CurrentSelectedSpriteSheet.AllAnimationOnSheet.IndexOf(
						(tvi.DataContext as CanvasAnimation)); // MAGIC BULLSHIT

				if (animationIndex >= 0)
				{
					// Get a new image file
					Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
					{
						Title = "Import PNG File",
						FileName = "", //default file name
						Filter = "Image files (*.png)|*.png",
						RestoreDirectory = true
					};

					Nullable<bool> result = dlg.ShowDialog();
					// Process save file dialog box results
					string filename = "";
					if (result == true)
					{
						// Save document
						filename = dlg.FileName;
						// filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] {'/', '\\'}));
					}
					else return; //invalid name

					Console.WriteLine(dlg.FileName);
					BitmapImage _baseImage = new BitmapImage();
					_baseImage.BeginInit();
					_baseImage.UriSource = new Uri(filename, UriKind.Absolute);
					_baseImage.EndInit();

					//var adorn = new AdornerDecorator();
					CurrentSelectedSpriteSheet.AllAnimationOnSheet[animationIndex].CanvasFrames.Add(
						new CanvasImageProperties(filename, _baseImage.PixelWidth, _baseImage.PixelHeight));

					currentSelectedCanvasFrame =
						CurrentSelectedSpriteSheet.AllAnimationOnSheet[animationIndex].CanvasFrames.Last();

					// we need to check if we are already using the croppable control
					if (SpritesheetEditor_CropImage.ResizeService != null)
					{

						// we need to transfer this image and properties to the canvas we are trying make the new image on
						String path = SpritesheetEditor_CropImage.GetImagePath();
						CanvasImageProperties foundFrame = FindCanvasFrame(SpritesheetEditor_CropImage);
						if (path != null)
						{
							Image image = new Image();

							// Have we cropped the craoppable image yet?
							if (SpritesheetEditor_CropImage.GetCroppedBitmap() != null)
							{
								image.Source = SpritesheetEditor_CropImage.GetCroppedBitmap();
							}
							else
							{
								image.Source = new BitmapImage(new Uri(path));
							}

							image.Stretch = Stretch.Fill;
							image.Name = "cropped_image_preview";
							image.Width = SpritesheetEditor_CropImage.Width;
							image.MaxWidth = SpritesheetEditor_CropImage.Width;
							image.Height = SpritesheetEditor_CropImage.Height;
							image.MaxHeight = SpritesheetEditor_CropImage.Height;
							image.HorizontalAlignment = HorizontalAlignment.Center;
							image.VerticalAlignment = VerticalAlignment.Center;
							image.PreviewMouseLeftButtonDown +=
								new MouseButtonEventHandler(LeftMouseDowndOnImageFrame_SpriteSheetEditor_CB);
							image.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(LeftMouseUpdOnImageFrame_SpriteSheetEditor_CB);

							Image renderPointImage = new Image();
							renderPointImage.Name = "Render_Base_Point_Image";
							renderPointImage.Source =
								new BitmapImage(new Uri(
									String.Format("{0}/Resources/render_point_crosshair_base.png", Directory.GetCurrentDirectory()),
									UriKind.Absolute));

							Canvas overlayCanvas = new Canvas() {Width = _baseImage.PixelWidth, Height = _baseImage.PixelHeight};
							overlayCanvas.Children.Add(image);
							overlayCanvas.Children.Add(renderPointImage);

							Border parentBorder = CreateDashedLineBorder();
							parentBorder.Child = overlayCanvas;

							// First we need to find the current frame we linked to the crop control if there is one.
							if (foundFrame != null)
							{
								foundFrame.LinkedCroppableImage = null;
								foundFrame.LinkedBorderImage = parentBorder;

								// Place the Render point in the correct position
								Canvas.SetLeft(renderPointImage, foundFrame.RX);
								Canvas.SetTop(renderPointImage, foundFrame.RY);
							}

							CurrentSelectedSpriteSheet.AllAnimationOnSheet[animationIndex].CanvasFrames.Last().LinkedCroppableImage =
								SpritesheetEditor_CropImage;

							SpritesheetEditor_Canvas.Children.Add(parentBorder);

							double xPos = Canvas.GetLeft(SpritesheetEditor_CropImage);
							double yPos = Canvas.GetTop(SpritesheetEditor_CropImage);
							Canvas.SetLeft(parentBorder, xPos);
							Canvas.SetTop(parentBorder, yPos);

							// ClearAdorners(c);
							SpritesheetEditor_CropImage.bHasFocus = false;
							SpritesheetEditor_CropImage.Visibility = Visibility.Hidden;
						}
					}
					CurrentSelectedSpriteSheet.AllAnimationOnSheet[animationIndex].CanvasFrames.Last().LinkedCroppableImage =
						SpritesheetEditor_CropImage;

					if(CurrentSelectedSpriteSheet.AllAnimationOnSheet[animationIndex].CanvasFrames.Last().SubLayerPoints.Count > 0)
					{
						CurrentSelectedSpriteSheet.AllAnimationOnSheet[animationIndex].CanvasFrames.Last().SubLayerPoints?
							.Add(new CanvasSubLayerPoint()
							{
								LayerName =
									CurrentSelectedSpriteSheet.AllAnimationOnSheet[animationIndex].NamesOfSubLayers?.Last()
							});

					}


					//SpritesheetEditor_Canvas.Children.Add(croppable);

					//croppable.SetImage(filename, true);

					SpritesheetEditor_CropImage.SetImage(filename, true);
					SpritesheetEditor_CropImage.bHasFocus = true;
					SpritesheetEditor_CropImage.Visibility = Visibility.Visible;
					if (SpritesheetEditor_CropImage.updateSizeLocation_Hook == null)
						SpritesheetEditor_CropImage.updateSizeLocation_Hook += UpdateSizeLocationHook_FrameInfo;
					if (SpritesheetEditor_CropImage.updateCropLocation_Hook == null)
						SpritesheetEditor_CropImage.updateCropLocation_Hook += UpdateCropLocationHook_FrameInfo;
					if (SpritesheetEditor_CropImage.setRenderPoint_Hook == null)
						SpritesheetEditor_CropImage.setRenderPoint_Hook += UpdateRenderPointLocationHook_FrameInfo;

					SpritesheetEditor_CropImage.Focus();

				}
			}
		}

		private void UpdateSizeLocationHook_FrameInfo(double x, double y, double w, double h)
		{
			// We need to find out what this croppable image is linked to!
			CanvasImageProperties foundFrame = FindCanvasFrame(SpritesheetEditor_CropImage);
			if (foundFrame != null)
			{
				foundFrame.X = (int)(x / SpritesheetEditorZoomLevel);
				foundFrame.Y = (int)(y / SpritesheetEditorZoomLevel);
				foundFrame.W = (int)(w / SpritesheetEditorZoomLevel);
				foundFrame.H = (int)(h / SpritesheetEditorZoomLevel);
			}
		}

		private void UpdateCropLocationHook_FrameInfo(double cx, double cy)
		{
			// We need to find out what this croppable image is linked to!
			CanvasImageProperties foundFrame = FindCanvasFrame(SpritesheetEditor_CropImage);
			if (foundFrame != null)
			{
				foundFrame.CropX = (int)(cx);
				foundFrame.CropY = (int)(cy);
			}
		}

		private void UpdateRenderPointLocationHook_FrameInfo(int x, int y)
		{
			Console.WriteLine(String.Format("X:{0}, Y:{1}", x, y));
			CanvasImageProperties foundFrame = FindCanvasFrame(SpritesheetEditor_CropImage);
			if (foundFrame != null)
			{
				foundFrame.RX = x;
				foundFrame.RY = y;

				// we need to find the child for the render point

				if (foundFrame.LinkedBorderImage != null)
				{
					foreach (var childImage in (foundFrame.LinkedBorderImage.Child as Canvas).Children)
					{
						if (childImage is Image img)
						{
							if (img.Name.Contains("Base"))
							{
								Canvas.SetLeft(img, x);
								Canvas.SetTop(img, y);
							}
						}
					}



				}
				else if (foundFrame.LinkedCroppableImage != null)
				{

				}
			}
		}

		private void CreateSpritesheet_BTN_Click(object sender, RoutedEventArgs e)
		{

			// make sure there is valid creation data
			if (SpritesheetName_TB.Text != "" && int.TryParse(SpritesheetWidth_TB.Text, out int width)
																				&& int.TryParse(SpritesheetHeight_TB.Text, out int height))
			{
				SE_NewSpritesheet_MainGrid.Visibility = Visibility.Visible;
				SpritesheetEditorMainGrid_Grid.Visibility = Visibility.Visible;
				SpritesheetEditorCreation_Grid.Visibility = Visibility.Hidden;

				CurrentSelectedSpriteSheet = new CanvasSpritesheet(SpritesheetName_TB.Name, width, height);
				ActiveCanvasSpritesheets.Add(CurrentSelectedSpriteSheet);


				SpritesheetEditor_BackCanvas.Width = width;
				SpritesheetEditor_BackCanvas.Height = height;
			}
		}


		private TreeViewItem FindParentTreeViewItem(object child)
		{
			try
			{
				var parent = VisualTreeHelper.GetParent(child as DependencyObject);
				while ((parent as TreeViewItem) == null)
				{
					parent = VisualTreeHelper.GetParent(parent);
				}

				return parent as TreeViewItem;
			}
			catch (Exception e)
			{
				//could not find a parent of type TreeViewItem
				return null;
			}
		}

		private int? GetTreeViewItemParentIndex(TreeViewItem Item, TreeView tree)
		{
			int index = 0;
			foreach (var _item in tree.Items)
			{
				if (_item == Item.Parent)
				{
					return index;
				}

				index++;
			}

			return null;
			//throw new Exception("No parent window detected");
		}

		/// <summary>
		/// Returns true if we have clicke in the canvas
		/// </summary>
		/// <param name="canvas"></param>
		/// <returns></returns>
		private bool IsClickedOnImageFrame(Canvas canvas)
		{

			foreach (UIElement childImage in canvas.Children)
			{
				// Get the rectangle area for this image.
				if (SpritesheetEditor_CropImage == childImage)
				{
					double left = Canvas.GetLeft(childImage);
					double right = Canvas.GetRight(childImage);
					double top = Canvas.GetTop(childImage);
					double bottom = Canvas.GetBottom(childImage);

					Rect rect = new Rect(new Point(left, top), new Size(right - left, bottom - top));
					if (rect.Contains(Mouse.GetPosition(canvas)))
						return true;
				}
			}

			return false;
		}

		private void SpritesheetEditor_BackCanvas_OnLeftMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (SpritesheetEditor_CropImage.Visibility == Visibility.Hidden ||
					SpritesheetEditor_CropImage.ResizeService == null)
				return;

			Canvas c = sender as Canvas;
			if (c != null)
			{
				if (IsClickedOnImageFrame(c))
					return; // ignore unless we click on the canvas itself

				// we need to transfer this image and properties to the canvas we are trying make the new image on
				String path = SpritesheetEditor_CropImage.GetImagePath();
				if (path != null)
				{
					Image image = new Image();

					// Have we cropped the craoppable image yet?
					if (SpritesheetEditor_CropImage.GetCroppedBitmap() != null)
					{
						image.Source = SpritesheetEditor_CropImage.GetCroppedBitmap();
					}
					else
					{
						image.Source = new BitmapImage(new Uri(path));
					}

					image.Stretch = Stretch.Fill;
					image.Name = "cropped_image_preview";
					image.Width = SpritesheetEditor_CropImage.Width;
					image.MaxWidth = SpritesheetEditor_CropImage.Width;
					image.Height = SpritesheetEditor_CropImage.Height;
					image.MaxHeight = SpritesheetEditor_CropImage.Height;
					image.HorizontalAlignment = HorizontalAlignment.Center;
					image.VerticalAlignment = VerticalAlignment.Center;
					image.PreviewMouseLeftButtonDown +=
						new MouseButtonEventHandler(LeftMouseDowndOnImageFrame_SpriteSheetEditor_CB);
					image.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(LeftMouseUpdOnImageFrame_SpriteSheetEditor_CB);

					// Set up the BASE RENDER POINTS
					Image renderPointImage = new Image();
					renderPointImage.Name = "Render_Base_Point_Image";
					renderPointImage.Source =
						new BitmapImage(new Uri(
							String.Format("{0}/Resources/render_point_crosshair_base.png", Directory.GetCurrentDirectory()),
							UriKind.Absolute));

					Canvas overlayCanvas = new Canvas()
					{ Width = SpritesheetEditor_CropImage.Width, Height = SpritesheetEditor_CropImage.Height };
					overlayCanvas.Children.Add(image);
					overlayCanvas.Children.Add(renderPointImage);

					// Create a border for the image
					Border border = CreateDashedLineBorder();
					border.Child = overlayCanvas;

					// We need to find out what this croppable image is linked to!
					CanvasImageProperties foundFrame = FindCanvasFrame(SpritesheetEditor_CropImage);
					if (foundFrame != null)
					{
						foundFrame.LinkedCroppableImage = null;
						foundFrame.LinkedBorderImage = border;

						// Place the Render point in the correct position
						Canvas.SetLeft(renderPointImage, foundFrame.RX);
						Canvas.SetTop(renderPointImage, foundFrame.RY);

						// We will need to set up the sub layer render points now
						int subLayerCount = 1;
						foreach (var subLayerPoint in foundFrame.SubLayerPoints)
						{
							Image subPointImage = new Image();
							subPointImage.Source = new BitmapImage(new Uri(
									String.Format("{0}/Resources/render_point_crosshair_{1}.png", Directory.GetCurrentDirectory(), subLayerCount++),
									UriKind.Absolute));

							// If we add a new frame the sublayer linking needs to be set up.
							if (subLayerPoint.LinkedImage == null)
								subLayerPoint.LinkedImage = subPointImage;

							overlayCanvas.Children.Add(subPointImage);
							Canvas.SetLeft(subPointImage, subLayerPoint.RX);
							Canvas.SetTop(subPointImage, subLayerPoint.RY);

						}
					}

					SpritesheetEditor_Canvas.Children.Add(border);

					double xPos = Canvas.GetLeft(SpritesheetEditor_CropImage);
					double yPos = Canvas.GetTop(SpritesheetEditor_CropImage);
					Canvas.SetLeft(border, xPos);
					Canvas.SetTop(border, yPos);

					// ClearAdorners(c);
					SpritesheetEditor_CropImage.bHasFocus = false;
					SpritesheetEditor_CropImage.Visibility = Visibility.Hidden;
				}
			}
		}

		private String GetCroppedImagePath(Image img)
		{
			string imagePath = null;
			if (img != null)
			{
				imagePath = ((img.Source as CroppedBitmap)?.Source as CroppedBitmap)?.Source.ToString();
				if (imagePath != null)
				{

				}
				else
				{
					if (((img.Source as CroppedBitmap)?.Source != null))
					{
						imagePath = (img.Source as CroppedBitmap)?.Source.ToString();
					}
					else
					{
						imagePath = ((BitmapImage)img.Source).UriSource.ToString();
					}
				}
			}

			return imagePath;
		}

		public CanvasImageProperties CompareImagePathToCanvasImagePath(Image img)
		{
			string imagePath = GetCroppedImagePath(img);
			String path1 = new Uri(imagePath).LocalPath;

			// Let's find out what SpriteAnimation this image belongs too!
			foreach (CanvasAnimation spriteAntimation in CurrentSelectedSpriteSheet.AllAnimationOnSheet)
			{
				foreach (CanvasImageProperties frame in spriteAntimation.CanvasFrames)
				{
					String path2 = Path.GetFullPath(frame.ImageLocation);

					if (string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase))
					{
						return frame;
					}
				}
			}

			return null;
		}

		private int GetFrameNumberFromFrame(CanvasImageProperties desiredFrame)
		{
			// Let's find out what SpriteAnimation this image belongs too!
			foreach (CanvasAnimation spriteAntimation in CurrentSelectedSpriteSheet.AllAnimationOnSheet)
			{
				for (int i = 0; i < spriteAntimation.CanvasFrames.Count; i++)
				{
					CanvasImageProperties frame = spriteAntimation.CanvasFrames[i];
					if (frame == desiredFrame)
						return i;
				}
			}

			return -1;
		}

		private CanvasAnimation GetCanvasAnimationFromFrame(CanvasImageProperties desiredFrame)
		{
			// Let's find out what SpriteAnimation this image belongs too!
			foreach (CanvasAnimation spriteAntimation in CurrentSelectedSpriteSheet.AllAnimationOnSheet)
			{
				foreach (CanvasImageProperties frame in spriteAntimation.CanvasFrames)
				{
					if (frame == desiredFrame)
						return spriteAntimation;
				}
			}

			return null;
		}


	}
}
