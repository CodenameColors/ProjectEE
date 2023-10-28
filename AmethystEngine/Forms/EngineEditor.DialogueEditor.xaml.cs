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
using BixBite.Characters;
using BixBite.Rendering;
using BixBite.Rendering.UI;
using BixBite.Rendering.UI.Button;
using BixBite.Rendering.UI.TextBlock;
using NodeEditor;
using PropertyGridEditor;
using TimelinePlayer.Components;
using GameImage = BixBite.Rendering.UI.Image.GameImage;
using BixBite;

namespace AmethystEngine.Forms
{
	public partial class EngineEditor
	{

		#region Pointers

		ObservableCollection<DialogueScene> ActiveDialogueScenes = new ObservableCollection<DialogueScene>();
		DialogueScene CurActiveDialogueScene;
		TreeView Dialogue_CE_Tree;

		List<Tuple<ContentControl, ContentControl>> CurSceneEntityDisplays =
			new List<Tuple<ContentControl, ContentControl>>();

		CollapsedPropertyGrid.CollapsedPropertyGrid CPGrid = new CollapsedPropertyGrid.CollapsedPropertyGrid();
		object DialogueEditorSelectedControl = null;

		#endregion

		#region Vars

		ObservableCollection<object> PropertyBags { get; set; }

		#endregion

		private void DialogueEditorLoaded()
		{

		}

		#region Dialogue

		#region Timeline

		/// <summary>
		/// When you have the a Timeblock selected and are trying to change the start time in properties editor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void SetStartTime(object sender, EventArgs e)
		{
			((TimeBlock)DialogueEditor_Timeline.SelectedControl).StartTime = double.Parse(((TextBox)sender).Text);
		}

		/// <summary>
		/// When you have the a Timeblock selected and are trying to change the end time in properties editor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void SetEndTime(object sender, EventArgs e)
		{
			((TimeBlock)DialogueEditor_Timeline.SelectedControl).EndTime = double.Parse(((TextBox)sender).Text);
		}

		#endregion

		#region NodeEditor

		#endregion

		#region DialogueEditor

		#endregion

		#endregion

		/// <summary>
		/// method to draw a newly added/imported UI to the screen. Choose whether or not to breakdown the components for editing.
		/// </summary>
		/// <param name="CurrentEditorCanvas">Current Canvas that you will draw the UI too</param>
		/// <param name="gameUI">The Custom created UI that you want to draw</param>
		/// <param name="bcomps">TRUE = multiple ContentControls will be drawn, and allowed to be edited 
		/// <para/>
		/// FALSE = ONLY ONE Content control will be drawn. Base UI is editable in size, and position. Children are editable via properties
		/// </param>
		public ContentControl DrawUIToScreen(Canvas CurrentEditorCanvas, Canvas CurrentEditorCanvas_Back, BaseUI gameUI,
			bool bcomps, String DesiredImageName_CC)
		{
			//set the position and the size of the Base UI
			ContentControl BaseUI = ((ContentControl)this.TryFindResource("MoveableControls_Template"));
			ContentControl RetCC = null;

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
			((Grid)BaseUI.Content).Children.Add(new Border()
			{
				BorderThickness = new Thickness(2),
				BorderBrush = (((bool)gameUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent)
			});

			//get the middle!
			Point mid = new Point(CurrentEditorCanvas_Back.ActualWidth / 2, CurrentEditorCanvas_Back.ActualHeight / 2);
			//get the true center point. Since origin is top left.
			mid.X -= BaseUI.Width / 2;
			mid.Y -= BaseUI.Height / 2;
			Canvas.SetLeft(BaseUI, mid.X);
			Canvas.SetTop(BaseUI, mid.Y);

			//which drawing type?
			if (bcomps)
			{
				//create all the child UI elements as editable content controls
				foreach (BaseUI childUI in gameUI.UIElements)
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
						Background =
							(SolidColorBrush)new BrushConverter().ConvertFromString(childUI.GetPropertyData("BackgroundColor")
								.ToString()),
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
					Canvas.SetZIndex(CUI, Int32.Parse(childUI.GetPropertyData("ZIndex").ToString()));
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
						//
						TextBox tb = new TextBox()
						{
							Text = childUI.GetPropertyData("Text").ToString(),
							Margin = new Thickness(2),
							BorderThickness = new Thickness(0),
							IsHitTestVisible = false,
							VerticalContentAlignment = VerticalAlignment.Center,
							HorizontalContentAlignment = HorizontalAlignment.Center,
							FontSize = Int32.Parse(childUI.GetPropertyData("FontSize").ToString()),
							Foreground =
								(SolidColorBrush)new BrushConverter().ConvertFromString(
									childUI.GetPropertyData("FontColor").ToString()),
							Background = Brushes.Transparent
						};
						Grid.SetColumn(tb, 1);
						Grid.SetRow(tb, 1);
						((Grid)CUI.Content).Children.Add(tb);

					}
					else if (childUI is GameImage)
					{
						CUI.Tag = "IMAGE";

						String TB = childUI.GetPropertyData("Image").ToString();
						TB = TB.Replace("{Content}", EditorProjectContentDirectory);
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

					CurrentUIDictionary.Add(childUI.UIName, childUI);
				}

				CurrentUIDictionary.Add(OpenUIEdits.Last().UIName, OpenUIEdits.Last());

			}
			else //all as one
			{
				//create all the child UI elements as editable content controls
				foreach (BaseUI childUI in gameUI.UIElements)
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
						Background =
							(SolidColorBrush)new BrushConverter().ConvertFromString(
								(childUI.GetPropertyData("BackgroundColor") == null
									? "#00000000"
									: childUI.GetPropertyData("BackgroundColor"))
								.ToString()),
						IsHitTestVisible = false,
					};
					((Grid)CUI.Content).Children.Add(new Border()
					{
						BorderThickness =
							new Thickness(0), //(((bool)childUI.GetProperty("ShowBorder")) ? new Thickness(2) : new Thickness(0)),
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
						//
						TextBox tb = new TextBox()
						{
							Text = childUI.GetPropertyData("Text").ToString(),
							Margin = new Thickness(2),
							BorderThickness = new Thickness(0),
							IsHitTestVisible = false,
							VerticalContentAlignment = VerticalAlignment.Center,
							HorizontalContentAlignment = HorizontalAlignment.Center,
							FontSize = Int32.Parse(childUI.GetPropertyData("FontSize").ToString()),
							Foreground =
								(SolidColorBrush)new BrushConverter().ConvertFromString(
									childUI.GetPropertyData("FontColor").ToString()),
							Background = Brushes.Transparent
						};
						Grid.SetColumn(tb, 1);
						Grid.SetRow(tb, 1);
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
					}
					else if (childUI is GameImage)
					{
						CUI.Tag = "IMAGE";

						String TB = childUI.GetPropertyData("Image").ToString();
						TB = TB.Replace("{Content}", EditorProjectContentDirectory);
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
						((Grid)CUI.Content).Children.Add(img);
						((Grid)BaseUI.Content).Children.Add(CUI);

						if (childUI.UIName == DesiredImageName_CC)
							RetCC = CUI;
					}
					else if (childUI is GameButton)
					{

					}
				}
			}

			SelectedBaseUIControl = BaseUI;
			SelectedUI = OpenUIEdits.Last();
			CurrentEditorCanvas.Children.Add(BaseUI);
			return RetCC;
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
		public void DrawUIToScreen(Canvas CurrentEditorCanvas, Canvas CurrentEditorCanvas_Back, BaseUI gameUI, bool bcomps)
		{
			//set the position and the size of the Base UI
			ContentControl baseUi_CC = ((ContentControl)this.TryFindResource("MoveableControls_Template"));

			baseUi_CC.Width = Int32.Parse(gameUI.GetPropertyData("Width").ToString());
			baseUi_CC.Height = Int32.Parse(gameUI.GetPropertyData("Height").ToString());
			baseUi_CC.BorderBrush = (((bool)gameUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent);
			baseUi_CC.BorderThickness = new Thickness(0);
			baseUi_CC.Tag = "Border";
			baseUi_CC.Name = gameUI.UIName;
			//};
			baseUi_CC.Content = new Grid()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
			};
			((Grid)baseUi_CC.Content).Children.Add(new Border()
			{
				BorderThickness = new Thickness(2),
				BorderBrush = (((bool)gameUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent)
			});

			//get the middle!
			Point mid = new Point(CurrentEditorCanvas_Back.ActualWidth / 2, CurrentEditorCanvas_Back.ActualHeight / 2);
			//get the true center point. Since origin is top left.
			mid.X -= baseUi_CC.Width / 2;
			mid.Y -= baseUi_CC.Height / 2;
			Canvas.SetLeft(baseUi_CC, mid.X);
			Canvas.SetTop(baseUi_CC, mid.Y);

			//which drawing type?
			if (bcomps)
			{
				//create all the child UI elements as editable content controls
				foreach (BaseUI childUI in gameUI.UIElements)
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
						Background =
							(SolidColorBrush)new BrushConverter().ConvertFromString(
								(childUI.GetPropertyData("BackgroundColor") == null
									? "#00000000"
									: childUI.GetPropertyData("BackgroundColor"))
								.ToString()),
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
					Canvas.SetZIndex(CUI, Int32.Parse(childUI.GetPropertyData("ZIndex").ToString()));
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
						//
						TextBox tb = new TextBox()
						{
							Text = childUI.GetPropertyData("Text").ToString(),
							Margin = new Thickness(2),
							BorderThickness = new Thickness(0),
							IsHitTestVisible = false,
							VerticalContentAlignment = VerticalAlignment.Center,
							HorizontalContentAlignment = HorizontalAlignment.Center,
							FontSize = Int32.Parse(childUI.GetPropertyData("FontSize").ToString()),
							Foreground =
								(SolidColorBrush)new BrushConverter().ConvertFromString(
									childUI.GetPropertyData("FontColor").ToString()),
							Background = Brushes.Transparent
						};
						Grid.SetColumn(tb, 1);
						Grid.SetRow(tb, 1);
						((Grid)CUI.Content).Children.Add(tb);

					}
					else if (childUI is GameImage)
					{
						CUI.Tag = "IMAGE";

						String TB = childUI.GetPropertyData("Image").ToString();
						TB = TB.Replace("{Content}", EditorProjectContentDirectory);
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

					CurrentUIDictionary.Add(childUI.UIName, childUI);
				}

				CurrentUIDictionary.Add(OpenUIEdits.Last().UIName, OpenUIEdits.Last());

			}
			else //all as one
			{
				//create all the child UI elements as editable content controls
				foreach (BaseUI childUI in gameUI.UIElements)
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
						Background =
							(SolidColorBrush)new BrushConverter().ConvertFromString(childUI.GetPropertyData("BackgroundColor")
								.ToString()),
						IsHitTestVisible = false,
					};
					((Grid)CUI.Content).Children.Add(new Border()
					{
						BorderThickness =
							new Thickness(0), //(((bool)childUI.GetProperty("ShowBorder")) ? new Thickness(2) : new Thickness(0)),
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
							Text = childUI.GetPropertyData("Text").ToString(),
							Margin = new Thickness(2),
							BorderThickness = new Thickness(0),
							IsHitTestVisible = false,
							VerticalContentAlignment = VerticalAlignment.Center,
							HorizontalContentAlignment = HorizontalAlignment.Center,
							FontSize = Int32.Parse(childUI.GetPropertyData("FontSize").ToString()),
							Foreground =
								(SolidColorBrush)new BrushConverter().ConvertFromString(
									childUI.GetPropertyData("FontColor").ToString()),
							Background = Brushes.Transparent
						};
						Grid.SetColumn(tb, 1);
						Grid.SetRow(tb, 1);
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
						((Grid)baseUi_CC.Content).Children.Add(CUI);
					}
					else if (childUI is GameImage)
					{
						CUI.Tag = "IMAGE";

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
						((Grid)CUI.Content).Children.Add(img);
						((Grid)baseUi_CC.Content).Children.Add(CUI);
					}
					else if (childUI is GameButton)
					{

					}
				}
			}

			SelectedBaseUIControl = baseUi_CC;
			SelectedUI = OpenUIEdits.Last();
			CurrentEditorCanvas.Children.Add(baseUi_CC);
		}

		/// <summary>
		/// Create a new Dialogue Scene
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewDialogueScene_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			SetupDialogueSceneHooks();
			CurActiveDialogueScene = new DialogueScene("Dialogue1");
			ActiveDialogueScenes.Add(CurActiveDialogueScene);

			//DialogueEditor_Timeline.ItemsSource = new List<String>();

		}

		private void ChangeDialogueUIAnchorPostion_Hook(HorizontalAlignment hori, VerticalAlignment vert, int timelineind)
		{
			double gridw = DialogueEditor_BackCanvas.ActualWidth;
			double gridh = DialogueEditor_BackCanvas.ActualHeight;
			//CurSceneEntityDisplays[timelineind].SpriteSheetName

			if (hori == HorizontalAlignment.Left && vert == VerticalAlignment.Top)
			{
				Canvas.SetLeft(CurSceneEntityDisplays[timelineind].Item2, 0);
				Canvas.SetTop(CurSceneEntityDisplays[timelineind].Item2, 0);

				CurSceneEntityDisplays[timelineind].Item2.HorizontalAlignment = HorizontalAlignment.Left;
				CurSceneEntityDisplays[timelineind].Item2.VerticalAlignment = VerticalAlignment.Top;

				//for file data
				CurActiveDialogueScene.Characters[timelineind].HorizontalAnchor =
					CurSceneEntityDisplays[timelineind].Item2.HorizontalAlignment.ToString();
				CurActiveDialogueScene.Characters[timelineind].VerticalAnchor =
					CurSceneEntityDisplays[timelineind].Item2.VerticalAlignment.ToString();
			}
			else if (hori == HorizontalAlignment.Right && vert == VerticalAlignment.Top)
			{
				Canvas.SetLeft(CurSceneEntityDisplays[timelineind].Item2,
					gridw - CurSceneEntityDisplays[timelineind].Item2.Width);
				Canvas.SetTop(CurSceneEntityDisplays[timelineind].Item2, 0);

				CurSceneEntityDisplays[timelineind].Item2.HorizontalAlignment = HorizontalAlignment.Right;
				CurSceneEntityDisplays[timelineind].Item2.VerticalAlignment = VerticalAlignment.Top;

				//for file data
				CurActiveDialogueScene.Characters[timelineind].HorizontalAnchor =
					CurSceneEntityDisplays[timelineind].Item2.HorizontalAlignment.ToString();
				CurActiveDialogueScene.Characters[timelineind].VerticalAnchor =
					CurSceneEntityDisplays[timelineind].Item2.VerticalAlignment.ToString();
			}
			else if (hori == HorizontalAlignment.Left && vert == VerticalAlignment.Bottom)
			{
				Canvas.SetLeft(CurSceneEntityDisplays[timelineind].Item2, 0);
				Canvas.SetTop(CurSceneEntityDisplays[timelineind].Item2,
					gridh - CurSceneEntityDisplays[timelineind].Item2.Height);

				CurSceneEntityDisplays[timelineind].Item2.HorizontalAlignment = HorizontalAlignment.Left;
				CurSceneEntityDisplays[timelineind].Item2.VerticalAlignment = VerticalAlignment.Bottom;

				//for file data
				CurActiveDialogueScene.Characters[timelineind].HorizontalAnchor =
					CurSceneEntityDisplays[timelineind].Item2.HorizontalAlignment.ToString();
				CurActiveDialogueScene.Characters[timelineind].VerticalAnchor =
					CurSceneEntityDisplays[timelineind].Item2.VerticalAlignment.ToString();
			}
			else if (hori == HorizontalAlignment.Right && vert == VerticalAlignment.Bottom)
			{
				Canvas.SetLeft(CurSceneEntityDisplays[timelineind].Item2,
					gridw - CurSceneEntityDisplays[timelineind].Item2.Width);
				Canvas.SetTop(CurSceneEntityDisplays[timelineind].Item2,
					gridh - CurSceneEntityDisplays[timelineind].Item2.Height);

				CurSceneEntityDisplays[timelineind].Item2.HorizontalAlignment = HorizontalAlignment.Right;
				CurSceneEntityDisplays[timelineind].Item2.VerticalAlignment = VerticalAlignment.Bottom;

				//for file data
				CurActiveDialogueScene.Characters[timelineind].HorizontalAnchor =
					CurSceneEntityDisplays[timelineind].Item2.HorizontalAlignment.ToString();
				CurActiveDialogueScene.Characters[timelineind].VerticalAnchor =
					CurSceneEntityDisplays[timelineind].Item2.VerticalAlignment.ToString();
			}
		}

		private void DialogueEditor_BackCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			double gridw = DialogueEditor_BackCanvas.ActualWidth;
			double gridh = DialogueEditor_BackCanvas.ActualHeight;
			foreach (Tuple<ContentControl, ContentControl> BaseUICC in CurSceneEntityDisplays)
			{
				HorizontalAlignment hori = BaseUICC.Item2.HorizontalAlignment;
				VerticalAlignment vert = BaseUICC.Item2.VerticalAlignment;


				if (hori == HorizontalAlignment.Left && vert == VerticalAlignment.Top)
				{
					Canvas.SetLeft(BaseUICC.Item2, 0);
					Canvas.SetTop(BaseUICC.Item2, 0);
				}
				else if (hori == HorizontalAlignment.Right && vert == VerticalAlignment.Top)
				{
					Canvas.SetLeft(BaseUICC.Item2,
						gridw - BaseUICC.Item2.ActualWidth);
					Canvas.SetTop(BaseUICC.Item2, 0);
				}
				else if (hori == HorizontalAlignment.Left && vert == VerticalAlignment.Bottom)
				{
					Canvas.SetLeft(BaseUICC.Item2, 0);
					Canvas.SetTop(BaseUICC.Item2,
						gridh - BaseUICC.Item2.ActualHeight);
				}
				else if (hori == HorizontalAlignment.Right && vert == VerticalAlignment.Bottom)
				{
					Canvas.SetLeft(BaseUICC.Item2,
						gridw - BaseUICC.Item2.ActualWidth);
					Canvas.SetTop(BaseUICC.Item2,
						gridh - BaseUICC.Item2.ActualHeight);
				}
			}
		}


		private LinkedList<TimeBlock> defLL;

		private void ResetExecution_Hook()
		{
			DialogueEditor_NodeGraph.ResetExecution();
			DialogueEditor_NodeGraph.EndblockExecution();
			DialogueEditor_NodeGraph.StartBlockExecution();

			//let's get the current DEFAULT timeblock LL
			defLL = GetDefaultTimeBlocks_LL();
			DisplayTimeBlocks_LL(defLL);

		}

		/// <summary>
		/// Occurs when a TimeBlock is clicked and Moved.
		/// Displays ALL properties of the time block to the property grids
		/// HOOK METHOD into timeline.dll
		/// </summary>
		/// <param name="sender"></param>
		public void ShowTimelineSelectedProperties(object sender)
		{
			//CollapsedPropertyGrid.CollapsedPropertyGrid LB = ((CollapsedPropertyGrid.CollapsedPropertyGrid)(ObjectProperties_Control.Template.FindName("DialoguePropertyGrid", ObjectProperties_Control)));

			CollapsedPropertyGrid.CollapsedPropertyGrid LB =
				((CollapsedPropertyGrid.CollapsedPropertyGrid)(ObjectProperties_Control.Template.FindName(
					"DialoguePropertyGrid", ObjectProperties_Control)));
			if (sender is null) return;
			else if (sender is TimeBlock)
			{
				DialogueEditorSelectedControl = sender;
				PropertyBag timeblockPropertyBag = new PropertyBag(sender);
				timeblockPropertyBag.Name = "Time Block Properties";
				PropertyBag dialoguePropertyBag = new PropertyBag(sender);
				dialoguePropertyBag.Name = "Dialogue Data";
				TimeBlock TimeB = sender as TimeBlock;

				if (PropertyBags.Count > 0)
				{
					PropertyBags.Clear(); //clear the current property displayed
					PropertyBags.Add(timeblockPropertyBag);
					PropertyBags.Add(dialoguePropertyBag);
				}
				else
				{
					PropertyBags.Add(timeblockPropertyBag);
					PropertyBags.Add(dialoguePropertyBag);
				}

				try
				{
					timeblockPropertyBag.Properties.Add(
						new Tuple<string, object, Control>("Type", "Time Block", new TextBox() { IsEnabled = false }));
				}
				catch
				{
					return;
				}

				TextBox startime_TB = new TextBox();
				startime_TB.KeyDown += DE_SetStartTime;
				timeblockPropertyBag.Properties.Add(
					new Tuple<string, object, Control>("Start Time", TimeB.StartTime.ToString(), startime_TB));
				TextBox endtime_TB = new TextBox();
				endtime_TB.KeyDown += DE_SetEndTime;
				timeblockPropertyBag.Properties.Add(
					new Tuple<string, object, Control>("End Time", TimeB.EndTime.ToString(), endtime_TB));
				TextBox durationime_TB = new TextBox();
				durationime_TB.KeyDown += DE_SetDurationTime;
				timeblockPropertyBag.Properties.Add(
					new Tuple<string, object, Control>("Duration", TimeB.Duration.ToString(), durationime_TB));

				int i = 0;
				foreach (String s in (TimeB.LinkedDialogueBlock as NodeEditor.Components.DialogueNodeBlock)
					?.DialogueTextOptions)
				{
					ComboBox CB = new ComboBox() { Height = 50, ItemTemplate = (DataTemplate)this.Resources["CBIMGItems"] };
					CB.SelectionChanged += SetSpriteImagePath_Dia;
					List<EditorObject> ComboItems = new List<EditorObject>();
					foreach (Sprite filepath in CurActiveDialogueScene
						.Characters[DialogueEditor_Timeline.GetTimelinePosition(((TimeBlock)sender).TimelineParent)]
						.DialogueSprites)
					{
						try
						{
							String filePathImage = filepath.ImgPathLocation;
							filePathImage = filePathImage.Replace("{Content}", EditorProjectContentDirectory);

							ComboItems.Add(new EditorObject(filePathImage, filepath.ImgPathLocation,
								filePathImage.Substring(filePathImage.LastIndexOfAny(new char[] { '\\', '/' })),
								false));
						}
						catch (ArgumentException ae)
						{
							Console.WriteLine(ae.Message);
							continue;
						}
					}

					CB.ItemsSource = ComboItems;
					int index = Array.FindIndex(ComboItems.ToArray(), x => x.Thumbnail.AbsolutePath == TimeB.TrackSpritePath);
					if (index >= 0) CB.SelectedIndex = index;

					dialoguePropertyBag.Properties.Add(
						new Tuple<string, object, Control>("Sprite Image " + i, new List<String>(), CB));
					TextBox tb = new TextBox();
					tb.KeyDown += SetDialogueText;
					dialoguePropertyBag.Properties.Add(new Tuple<string, object, Control>("Dialogue Text " + i,
						s, tb));

					ComboBox CB1 = new ComboBox() { Height = 50 };
					CB1.SelectionChanged += SetLinkedTBName;
					List<String> ComboItems1 = new List<String>();
					foreach (BaseUI Gameui in CurActiveDialogueScene
						.DialogueBoxes[DialogueEditor_Timeline.GetTimelinePosition(TimeB.TimelineParent)].UIElements)
					{
						if (Gameui is GameTextBlock)
						{
							ComboItems1.Add(Gameui.UIName);
						}
					}

					index = Array.FindIndex(ComboItems1.ToArray(), x => x == TimeB.LinkedTextBoxName);
					if (index >= 0) CB1.SelectedIndex = index;

					CB1.ItemsSource = ComboItems1;
					dialoguePropertyBag.Properties.Add(
						new Tuple<string, object, Control>("Linked TextBox " + i, new List<String>(), CB1));
					i++;
				}
			}
			else if (sender is NodeEditor.Components.DialogueNodeBlock dialogueNodeBlock)
			{
				DialogueEditorSelectedControl = sender;
				PropertyBag timeblockPropertyBag = new PropertyBag(sender);
				timeblockPropertyBag.Name = "Time Block Properties";
				PropertyBag dialoguePropertyBag = new PropertyBag(sender);
				dialoguePropertyBag.Name = "Dialogue Data";

				TimelinePlayer.Components.TimeBlock TimeB =
					dialogueNodeBlock.LinkedTimeBlock as TimelinePlayer.Components.TimeBlock;
				if (TimeB == null) throw new NotImplementedException("The Linked TextBox has NOT been set!");

				if (PropertyBags.Count > 0)
				{
					PropertyBags.Clear(); //clear the current property displayed
					PropertyBags.Add(timeblockPropertyBag);
					PropertyBags.Add(dialoguePropertyBag);
				}
				else
				{
					PropertyBags.Add(timeblockPropertyBag);
					PropertyBags.Add(dialoguePropertyBag);
				}

				try
				{
					timeblockPropertyBag.Properties.Add(
						new Tuple<string, object, Control>("Type", "Dialogue Block", new TextBox() { IsEnabled = false }));
				}
				catch
				{
					return;
				}

				TextBox startime_TB = new TextBox();
				startime_TB.KeyDown += DE_SetStartTime;
				timeblockPropertyBag.Properties.Add(
					new Tuple<string, object, Control>("Start Time", TimeB.StartTime.ToString(), startime_TB));
				TextBox endtime_TB = new TextBox();
				endtime_TB.KeyDown += DE_SetEndTime;
				timeblockPropertyBag.Properties.Add(
					new Tuple<string, object, Control>("End Time", TimeB.EndTime.ToString(), endtime_TB));
				TextBox durationime_TB = new TextBox();
				durationime_TB.KeyDown += DE_SetDurationTime;
				timeblockPropertyBag.Properties.Add(
					new Tuple<string, object, Control>("Duration", TimeB.Duration.ToString(), durationime_TB));

				int i = 0;
				foreach (String s in dialogueNodeBlock.DialogueTextOptions)
				{
					ComboBox CB = new ComboBox() { Height = 50, ItemTemplate = (DataTemplate)this.Resources["CBIMGItems"] };
					CB.SelectionChanged += SetSpriteImagePath_Dia;
					List<EditorObject> ComboItems = new List<EditorObject>();
					foreach (Sprite filepath in CurActiveDialogueScene
						.Characters[DialogueEditor_Timeline.GetTimelinePosition(null, TimeB)].DialogueSprites)
					{
						try
						{
							String filePathImage = filepath.ImgPathLocation;
							filePathImage = filePathImage.Replace("{Content}", EditorProjectContentDirectory);

							ComboItems.Add(new EditorObject(filepath.ImgPathLocation, filePathImage,
								filePathImage.Substring(filePathImage.LastIndexOfAny(new char[] { '\\', '/' })),
								false));

						}
						catch (ArgumentException ae)
						{
							Console.WriteLine(ae.Message);
							continue;
						}
					}

					CB.ItemsSource = ComboItems;
					int index = Array.FindIndex(ComboItems.ToArray(), x => x.ContentPath.Replace("{Content}", EditorProjectContentDirectory) == TimeB.TrackSpritePath);
					if (index >= 0) CB.SelectedIndex = index;

					dialoguePropertyBag.Properties.Add(
						new Tuple<string, object, Control>("Sprite Image " + i, new List<String>(), CB));
					TextBox tb = new TextBox();
					tb.KeyDown += SetDialogueText;
					dialoguePropertyBag.Properties.Add(new Tuple<string, object, Control>("Dialogue Text " + i,
						s, tb));

					ComboBox CB1 = new ComboBox() { Height = 50 };
					CB1.SelectionChanged += SetLinkedTBName;
					List<String> ComboItems1 = new List<String>();
					foreach (BaseUI Gameui in CurActiveDialogueScene
						.DialogueBoxes[DialogueEditor_Timeline.GetTimelinePosition(TimeB.TimelineParent)].UIElements)
					{
						if (Gameui is GameTextBlock)
						{
							ComboItems1.Add(Gameui.UIName);
						}
					}

					index = Array.FindIndex(ComboItems1.ToArray(), x => x == TimeB.LinkedTextBoxName);
					if (index >= 0) CB1.SelectedIndex = index;

					CB1.ItemsSource = ComboItems1;
					dialoguePropertyBag.Properties.Add(
						new Tuple<string, object, Control>("Linked TextBox " + i, new List<String>(), CB1));
					i++;
				}
			}
			else if (sender is Timeline)
			{
				PropertyBags.Clear(); //clear the current property displayed
			}
		}

		/// <summary>
		/// Sets the duration time of the time block.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void DE_SetDurationTime(object sender, KeyEventArgs e)
		{
			if (Key.Enter == e.Key)
			{
				if (DialogueEditorSelectedControl is TimeBlock timeBlock)
				{
					if (Int32.TryParse((sender as TextBox).Text, out int val))
						timeBlock.Duration = val;
				}
				else if (DialogueEditorSelectedControl is NodeEditor.Components.DialogueNodeBlock dialogue)
				{
					if (Int32.TryParse((sender as TextBox).Text, out int val))
						(dialogue.LinkedTimeBlock as TimeBlock).Duration = val;
				}
			}
		}

		public void DE_SetStartTime(object sender, KeyEventArgs e)
		{
			if (Key.Enter == e.Key)
			{
				if (DialogueEditorSelectedControl is TimeBlock timeBlock)
				{
					if (Int32.TryParse((sender as TextBox).Text, out int val))
						timeBlock.StartTime = val;
				}
				else if (DialogueEditorSelectedControl is NodeEditor.Components.DialogueNodeBlock dialogue)
				{
					if (Int32.TryParse((sender as TextBox).Text, out int val))
						(dialogue.LinkedTimeBlock as TimeBlock).StartTime = val;
				}
			}
		}

		public void DE_SetEndTime(object sender, KeyEventArgs e)
		{
			if (Key.Enter == e.Key)
			{
				if (DialogueEditorSelectedControl is TimeBlock timeBlock)
				{
					if (Int32.TryParse((sender as TextBox).Text, out int val))
						timeBlock.EndTime = val;
				}
				else if (DialogueEditorSelectedControl is NodeEditor.Components.DialogueNodeBlock dialogue)
				{
					if (Int32.TryParse((sender as TextBox).Text, out int val))
						(dialogue.LinkedTimeBlock as TimeBlock).EndTime = val;
				}
			}
		}

		/// <summary>
		/// Sets the dialogue text of the time block
		/// Display is handled by binding in the timeline.dll
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void SetDialogueText(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (DialogueEditorSelectedControl is TimeBlock timeBlock)
				{
					(timeBlock.LinkedDialogueBlock as NodeEditor.Components.DialogueNodeBlock).DialogueTextOptions[
							(Grid.GetRow(sender as TextBox)) / 3]
						= (sender as TextBox).Text; //set the dialogue data
					timeBlock.CurrentDialogue = (sender as TextBox).Text; //set the timeblock data
				}
				else if (DialogueEditorSelectedControl is NodeEditor.Components.DialogueNodeBlock dialogue)
				{
					dialogue.DialogueTextOptions[(Grid.GetRow(sender as TextBox)) / 3] = (sender as TextBox).Text;
					(dialogue.LinkedTimeBlock as TimeBlock).CurrentDialogue = (sender as TextBox).Text;
				}

				//(((TimeBlock) DialogueEditor_Timeline.SelectedControl).LinkedDialogueBlock as DialogueNodeBlock)
				//	.DialogueData[Grid.GetRow(sender as TextBox)/3] = ((TextBox)sender).Text;
				//((TimeBlock) DialogueEditor_Timeline.SelectedControl).CurrentDialogue =
				//	(((TimeBlock) DialogueEditor_Timeline.SelectedControl).LinkedDialogueBlock as DialogueNodeBlock)
				//	.DialogueData[0];
			}
		}

		/// <summary>
		/// Sets the Sprite Image for the timeblock. Also WIP for the timeline later.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void SetSpriteImagePath_Dia(object sender, EventArgs e)
		{
			if (DialogueEditorSelectedControl is null) return;
			else if (DialogueEditorSelectedControl is ChoiceTimeBlock choiceTime)
			{
				choiceTime.TrackSpritePath = (((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
				choiceTime.Trackname = CurActiveDialogueScene.Characters[0].Name;
			}
			else if (DialogueEditorSelectedControl is TimeBlock timeblock)
			{
				timeblock.TrackSpritePath = (((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
				timeblock.Trackname = DialogueEditor_Timeline.GetTimelines()[
					DialogueEditor_Timeline.GetTimelinePosition(null, timeblock)].TrackName;

				//((TimeBlock)DialogueEditor_Timeline.SelectedControl).TrackSpritePath = (((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
				//((TimeBlock)DialogueEditor_Timeline.SelectedControl).Trackname = (((EditorObject)((ComboBox)sender).SelectedValue).Name);
			}
			else if (DialogueEditorSelectedControl is NodeEditor.Components.DialogueNodeBlock dialogue)
			{
				(dialogue.LinkedTimeBlock as TimeBlock).TrackSpritePath =
					(((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
				(dialogue.LinkedTimeBlock as TimeBlock).Trackname = dialogue.Header;
				dialogue.DialogueSprites.Add(new Sprite(
					((EditorObject)((ComboBox)sender).SelectedValue).Name,
					((EditorObject)((ComboBox)sender).SelectedValue).ContentPath,
					0, 0, 0, 0));
			}
			else if (DialogueEditor_Timeline.SelectedControl is Timeline timeline)
			{
				//timeline.
				//throw new NotImplementedException("The uer has somehow wanted to change the timeline sprite header image.");
			}
		}

		/// <summary>
		/// Set the lined Textbox that this Timeblock will change/interact with.
		/// WIP : this can later be set on init default wise. Since all blocks in a row should be for ONE character
		/// thus ONE TEXT box. but can be changed if needed still like before.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void SetLinkedTBName(object sender, EventArgs e)
		{
			if (DialogueEditor_Timeline.SelectedControl is Timeline)
			{

			}
			else if (DialogueEditorSelectedControl is TimeBlock timeBlock)
			{
				timeBlock.LinkedTextBoxName = ((ComboBox)sender).SelectedItem.ToString();
				//
				//((TimeBlock)sender).
			}
			else if (DialogueEditorSelectedControl is NodeEditor.Components.DialogueNodeBlock dialogue)
			{
				(dialogue.LinkedTimeBlock as TimeBlock).LinkedTextBoxName = (sender as ComboBox).SelectedItem.ToString();
			}
		}

		/// <summary>
		/// When adding a character to scene it will call this method. AFTER correct info is given.
		/// This method right now will add a sprite, that is can be linked to change, and a UI object to the screen.
		/// </summary>
		/// <param name="c"></param>
		public void AddCharacterHook(SceneEntity c, BaseUI gameUi, String GameFileUI, String LinkedTextboxName,
			String LinkedDialogueImage_Text = null)
		{
			CurActiveDialogueScene.Characters.Add(c);
			DialogueEditor_Timeline.AddTimeline(c.Name);

			if (LinkedDialogueImage_Text == null)
			{
				//add moveable scaleable sprite control.
				ContentControl CC = ((ContentControl)this.TryFindResource("MoveableControls_Template"));
				CC.HorizontalAlignment = HorizontalAlignment.Center;
				CC.VerticalAlignment = VerticalAlignment.Center;
				Canvas.SetZIndex(CC, 1);

				CC.Tag = "IMAGE";

				String TB = "/AmethystEngine;component/images/emma_colors_oc.png";
				ImageBrush ib = new ImageBrush();
				Image img = new Image();
				img.Stretch = Stretch.Fill;
				img.Source = new BitmapImage(new Uri(TB, UriKind.RelativeOrAbsolute));
				img.IsHitTestVisible = false;
				((Grid)CC.Content).Children.Add(new Border()
				{ BorderThickness = new Thickness(2), BorderBrush = Brushes.Gray, IsHitTestVisible = false });
				((Grid)CC.Content).Children.Add(new Rectangle() { });
				((Grid)CC.Content).Children.Add(img);

				DialogueEditore_Canvas.Children.Add(CC);
				CurActiveDialogueScene.Characters.Last().DialogueSprites.Add(
					new Sprite(img.Source.ToString(), img.Source.ToString(), 0, 0, (int)img.ActualWidth, (int)ActualHeight));

				OpenUIEdits.Add(gameUi);
				DrawUIToScreen(DialogueEditore_Canvas, DialogueEditor_BackCanvas, OpenUIEdits.Last(), false);
				CurActiveDialogueScene.DialogueBoxes.Add(OpenUIEdits.Last());
				CurActiveDialogueScene.DialogueBoxesFilePaths.Add(GameFileUI);

				//create the pointers to the content controls. which is what displays my images to the screen
				CurSceneEntityDisplays.Add(new Tuple<ContentControl, ContentControl>(CC, SelectedBaseUIControl));
			}
			else
			{
				OpenUIEdits.Add(gameUi);
				ContentControl CC = DrawUIToScreen(DialogueEditore_Canvas, DialogueEditor_BackCanvas, OpenUIEdits.Last(), false,
					LinkedDialogueImage_Text);
				Image img = (CC.Content as Grid).Children[2] as Image;

				CurActiveDialogueScene.Characters[CurActiveDialogueScene.Characters.IndexOf(c)].DialogueSprites.Add(
					new Sprite(img.Source.ToString(), img.Source.ToString(), 0, 0, (int)img.ActualWidth, (int)ActualHeight));
				CurActiveDialogueScene.Characters.Last().LinkedImageBox = LinkedDialogueImage_Text;


				CurActiveDialogueScene.DialogueBoxes.Add(OpenUIEdits.Last());
				CurActiveDialogueScene.DialogueBoxesFilePaths.Add(GameFileUI);

				//create the pointers to the content controls. which is what displays my images to the screen
				CurSceneEntityDisplays.Add(new Tuple<ContentControl, ContentControl>(CC, SelectedBaseUIControl));
			}

		}

		/// <summary>
		/// This method is called on the play button press in the timeline editor
		/// and it will set the Timeblock linked list. so the timeline knows how to traverse/increment after start.
		/// </summary>
		public void DialogueHook()
		{
			Console.WriteLine("Hook Activated");
			//CurActiveDialogueScene.SetTrack(CurActiveDialogueScene.Characters[0].Name, DialogueEditor_Timeline.GetTimelines()[0].timeBlocksLL);
		}

		/// <summary>
		/// Testing my casting from one dll to another.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DialogueEditorTesting_BTN(object sender, RoutedEventArgs e)
		{
			object t = DialogueEditor_Timeline.GetTimelines()[0];
			Timeline tt = (Timeline)t;
		}

		/// <summary>
		/// Occurs on the add a Character "+" button.
		/// Opens up the add character form. uses hooking.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddCharacterToScene(object sender, RoutedEventArgs e)
		{
			if (CurActiveDialogueScene == null)
			{
				EngineOutputLog.AddErrorLogItem(-3, "New Dialogue Scene hasn't been created yet.", "DialogueEditor", true);
				EngineOutputLog.AddLogItem("Dialogue Scene error. See Error log for details");
				if (resizeGrid.RowDefinitions.Last().Height.Value < 100)
					resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
				OutputLogSpliter.IsEnabled = true;
				return;
			}

			Dialogue_CE_Tree.ItemsSource = CurActiveDialogueScene.Characters;

			//CurActiveDialogueScene.Characters.Add(new Character() { Name = "Antonio" });

			SceneEntity c = new SceneEntity(HorizontalAlignment.Left.ToString(), VerticalAlignment.Bottom.ToString());
			Window w = new AddCharacterForm(ProjectFilePath, EditorProjectContentDirectory) { AddToScene = AddCharacterHook };
			w.ShowDialog();

			try
			{
				DialogueEditor_NodeGraph.SceneCharacters_list.Add(CurActiveDialogueScene.Characters.Last().Name);
			}
			catch (Exception ex)
			{
				Console.WriteLine("No character was created because of faulty data given");
			}

			//CurActiveDialogueScene.Characters.Add(c);
			//DialogueEditor_Timeline.AddTimeline(c.Name);

		}

		/// <summary>
		/// This method occurs AS the timeline is playing and the timeline has "entered" a new time block for that row.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ActiveTBblocks_CollectionChanged(object sender,
			System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems[0] is ChoiceTimeBlock choice)
				{
					//TODO: DISPLAY CHOICES LATER
					DialogueEditor_Timeline.PauseTimeline();
					return;
				}

				DialogueEditor_NodeGraph.EndblockExecution();
				if (DialogueEditor_NodeGraph.CurrentExecutionBlock is NodeEditor.Components.SetConstantNodeBlock)
				{
					if (DialogueEditor_NodeGraph.StartBlockExecution())
					{
						DialogueEditor_NodeGraph.ExecuteBlock();
						DialogueEditor_NodeGraph.EndblockExecution();
					}
				}

				if (DialogueEditor_NodeGraph.CurrentExecutionBlock is NodeEditor.ExitBlockNode)
				{
					DialogueEditor_NodeGraph.EndblockExecution();
					DialogueEditor_Timeline.PauseTimeline();
				}
			}

			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems == null && !File.Exists(((TimeBlock)e.NewItems[0]).TrackSpritePath)) return;
				//changing the Image on the screen Sprite dialogue
				Console.WriteLine("Added Active Time Block");
				if (e.NewItems[0] is ChoiceTimeBlock choiceTimeBlock)
				{
					//this means we need to display the choices to the user.

					//first hide the current UI controls.
					for (int i = 0; i < CurSceneEntityDisplays.Count; i++)
					{
						CurSceneEntityDisplays[i].Item1.Visibility = Visibility.Hidden;
						CurSceneEntityDisplays[i].Item2.Visibility = Visibility.Hidden;
						Selector.SetIsSelected(CurSceneEntityDisplays[i].Item2, false);
					}

					//FOR NOW display a list box of the options the user will choose from.
					//TODO: change this to the GamelistBox after this is created
					ListBox lb = new ListBox();
					lb.VerticalAlignment = VerticalAlignment.Bottom;
					lb.HorizontalAlignment = HorizontalAlignment.Stretch;
					foreach (String datachoices in
						(choiceTimeBlock.LinkedDialogueBlock as NodeEditor.Components.DialogueNodeBlock).DialogueTextOptions)
					{
						lb.Items.Add(datachoices);
					}

					lb.SelectionChanged += UserChooseDialogueChoice;
					DialogueEditor_Grid.Children.Add(lb);

					//give the list box a onselectionchanged event.
					//this event should CHANGE/LOAD in the new LinkedTimeblocks from the nodegraphs Dialogueblocks
					//this should continue UNTIl an exit block is found
					//THEN resume playing.
				}
				else
				{
					try
					{
						((Image)((Grid)CurSceneEntityDisplays[
										DialogueEditor_Timeline.GetTimelinePosition(((TimeBlock)e.NewItems[0]).TimelineParent)].Item1
									.Content)
								.Children[2]).Source =
							new BitmapImage(new Uri(((TimeBlock)e.NewItems[0]).TrackSpritePath, UriKind.Absolute));
					}
					catch (UriFormatException uri)
					{
						Console.WriteLine("INVAILD URI. The sprite image wasn't set");
						EngineOutputLog.AddErrorLogItem(-1, "Timeblocks sprite image wasn't set.", "DialogueEditor", false);
						EngineOutputLog.AddLogItem("Timeblock failed on activated. See Error Log for more details");
						if (resizeGrid.RowDefinitions.Last().Height.Value < 100)
							resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
						OutputLogSpliter.IsEnabled = true;
						DialogueEditor_Timeline.PauseTimeline();
						return;
					}
					catch
					{
						return;
					}

					//here we change the dialogue text itself

					BaseUI gtb = null;
					try
					{
						gtb = CurActiveDialogueScene
							.DialogueBoxes[DialogueEditor_Timeline.GetTimelinePosition(((TimeBlock)e.NewItems[0]).TimelineParent)]
							.UIElements.Single(m => m.UIName == ((TimeBlock)e.NewItems[0]).LinkedTextBoxName);
					}
					catch (InvalidOperationException ioe)
					{
						Console.WriteLine("The Linked Textbox wasn't set!");
						EngineOutputLog.AddErrorLogItem(-2, "Timeblocks linked textbox wasn't set.", "DialogueEditor", false);
						EngineOutputLog.AddLogItem("Timeblock failed on activated. See Error Log for more details");
						if (resizeGrid.RowDefinitions.Last().Height.Value < 100)
							resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
						OutputLogSpliter.IsEnabled = true;
						DialogueEditor_Timeline.PauseTimeline();
						return;
					}

					gtb.SetProperty("Text",
						((((TimeBlock)e.NewItems[0]).LinkedDialogueBlock) as NodeEditor.Components.DialogueNodeBlock)
						.DialogueTextOptions[0]);

					UIElementCollection uie =
						((Grid)CurSceneEntityDisplays[
							DialogueEditor_Timeline.GetTimelinePosition(((TimelinePlayer.Components.TimeBlock)e.NewItems[0])
								.TimelineParent)].Item2.Content)
						.Children;
					ContentControl CC = null;
					foreach (UIElement ccc in uie)
					{
						if (!(ccc is ContentControl)) continue;
						if (((ContentControl)ccc).Name == ((TimeBlock)e.NewItems[0]).LinkedTextBoxName)
						{
							CC = ((ContentControl)ccc);
						}
					}

					if (CC == null) return;
					Grid g = (Grid)CC.Content;
					foreach (UIElement c in g.Children)
					{
						if (c is TextBox)
						{
							((TextBox)c).Text = gtb.GetPropertyData("Text").ToString();
						}
					}

					//gtb.GetProperty("CurrentDialogue").ToString();
				}

				//we need imcrement our node editor pointers
				if (e.NewItems[0] is ChoiceTimeBlock)
				{
					//only check to make sure that we use this block, let the execution be handled by the selection changed event
					DialogueEditor_NodeGraph.StartBlockExecution();
					return;
				}

				if (DialogueEditor_NodeGraph.StartBlockExecution())
				{
					if (DialogueEditor_NodeGraph.ExecuteBlock())
					{
					}
					else
					{
						if (resizeGrid.RowDefinitions.Last().Height.Value < 100)
							resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
						OutputLogSpliter.IsEnabled = true;
						DialogueEditor_Timeline.PauseTimeline();
					}
				}
				else
				{
					if (resizeGrid.RowDefinitions.Last().Height.Value < 100)
						resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
					OutputLogSpliter.IsEnabled = true;
					DialogueEditor_Timeline.PauseTimeline();
				}
			}
			else
			{
				Console.WriteLine("Removed Active Time block");
			}



		}

		private void UserChooseDialogueChoice(object sender, SelectionChangedEventArgs e)
		{
			//show the controls again.
			for (int i = 0; i < CurSceneEntityDisplays.Count; i++)
			{
				CurSceneEntityDisplays[i].Item1.Visibility = Visibility.Visible;
				CurSceneEntityDisplays[i].Item2.Visibility = Visibility.Visible;
			}

			DialogueEditor_Grid.Children.Remove(sender as ListBox);
			DialogueEditor_Grid.UpdateLayout(); //GC

			DialogueEditor_NodeGraph.ChangeChoiceVar((sender as ListBox).SelectedIndex);

			defLL = GetChoiceTimeBlocks_LL(DialogueEditor_NodeGraph.CurrentExecutionBlock, (sender as ListBox).SelectedIndex);
			if (defLL is null) return;
			DisplayTimeBlocksAfter_LL(defLL);

			DialogueEditor_Timeline.UpdateLayout();
			DialogueEditor_Timeline.ResumeTimeline();
			if (DialogueEditor_NodeGraph.StartBlockExecution())
			{
				if (DialogueEditor_NodeGraph.ExecuteBlock())
				{
					DialogueEditor_NodeGraph.EndblockExecution();
					DialogueEditor_NodeGraph.StartBlockExecution();

					if (DialogueEditor_NodeGraph.CurrentExecutionBlock is NodeEditor.ExitBlockNode)
					{
						DialogueEditor_NodeGraph.EndblockExecution();
						DialogueEditor_Timeline.PauseTimeline();
					}
				}
			}
			else
			{
				DialogueEditor_Timeline.PauseTimeline();
				if (resizeGrid.RowDefinitions.Last().Height.Value < 100)
					resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
				OutputLogSpliter.IsEnabled = true;
			}

			//(sender as ListBox).SelectedIndex;
			//throw new NotImplementedException();
		}

		private void NodeEditorCurrentErrorsOnCollectionChanged(object sender,
			System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			foreach (Exception ex in e.NewItems)
			{
				if (ex.Message == "Dialogue Scene Completed!")
				{
					EngineOutputLog.AddLogItem("Dialogue Scene Completed! :D");
					EngineOutputLog.AddErrorLogItem(0, ex.Message, "BlockNodeEditor", false);
				}
				else if (ex.Message.Contains("Updated Runtime Var"))
				{
					EngineOutputLog.AddLogItem("Updated Global Runtime Var");
					EngineOutputLog.AddErrorLogItem(0, ex.Message, "BlockNodeEditor", true);
				}
				else
				{
					EngineOutputLog.AddErrorLogItem(-1, ex.Message, "BlockNodeEditor", false);
					EngineOutputLog.AddLogItem("Dialogue Error Found! Check Error Log for details.");

				}
			}
		}

		public void ChangeSpriteIMG(ContentControl CC, String newimg)
		{

		}

		/// <summary>
		/// more testing methods. I use this one to add a sprite to the screen.
		/// Test1 button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TestingAddingCharacterpictersdia(object sender, RoutedEventArgs e)
		{
			Sprite s = new Sprite("new sprite", "/AmethystEngine;component/images/Ame_icon_small.png", 0, 0, 400, 400);
			CurActiveDialogueScene.Characters[0].DialogueSprites.Add(s);
		}

		/// <summary>
		/// Used to change the ui and image manually . TESTING.
		/// Test2 button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Test2Dia_Click(object sender, RoutedEventArgs e)
		{
			DialogueEditor_Timeline.GetTimelines()[0].timeBlocksLL.First.Value.TrackSpritePath =
				"/AmethystEngine;component/images/Ame_icon_small.png";
			DialogueEditor_Timeline.GetTimelines()[0].timeBlocksLL.First.Value.Trackname = "DefaultImage";
			DialogueEditor_Timeline.GetTimelines()[0].timeBlocksLL.First.Value.CurrentDialogue = "Yo my dude!";
		}

		/// <summary>
		/// Clicking on a timeblock will recieve the properties of said object.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DialogueEditor_Timeline_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Console.WriteLine("Clicked on Timeline");

			if (sender is TimeBlock)
			{
				TimeBlock tb = (TimeBlock)sender;
				PropGrid LB =
					((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));

			}
			else if (sender is Timeline)
			{
				Timeline tl = (Timeline)sender;
				PropGrid LB =
					((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
			}
			else Console.WriteLine("Unsupported type");

		}

		public object CreateDialogueBlockForTimeline(object Timeblock, bool bChoices)
		{
			int i = DialogueEditor_Timeline.GetTimelinePosition(DialogueEditor_Timeline.selectedTimeline, null);
			NodeEditor.Components.DialogueNodeBlock dialogueNode =
				new NodeEditor.Components.DialogueNodeBlock(CurActiveDialogueScene.Characters[i].Name);
			DialogueEditor_NodeGraph.AddDialogueBlockToGraph(dialogueNode, CurActiveDialogueScene.Characters[i].Name,
				Timeblock);

			dialogueNode.bChoice = bChoices;
			if (bChoices)
			{
				(dialogueNode.LinkedTimeBlock as TimeBlock).Trackname = "choice";
				dialogueNode.Header = "Dialogue Choice";
			}
			else
				(dialogueNode.LinkedTimeBlock as TimeBlock).Trackname = CurActiveDialogueScene.Characters[i].Name;

			//TODO: This needs to be changed for the word choice.

			//Character
			return dialogueNode;
		}

		public object CreateTimeBlockForDialogue(object DialogueBlock)
		{
			int i = DialogueEditor_Timeline.GetTimelinePosition(DialogueEditor_Timeline.selectedTimeline, null);
			TimelinePlayer.Components.TimeBlock timeBlock =
				new TimelinePlayer.Components.TimeBlock(DialogueEditor_Timeline.GetTimelines()[0], 0);
			timeBlock.Trackname = (DialogueBlock as NodeEditor.Components.DialogueNodeBlock)?.Header;

			//DialogueEditor_NodeGraph.AddDialogueBlockToGraph(timeBlock, CurActiveDialogueScene.Characters[i].Name, DialogueBlock);
			(DialogueBlock as NodeEditor.Components.DialogueNodeBlock).LinkedTimeBlock = timeBlock;
			timeBlock.LinkedDialogueBlock = (DialogueBlock as NodeEditor.Components.DialogueNodeBlock);
			//Character
			return timeBlock;
		}

		private void ViewOutputLog_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem mi)
			{
				if (!mi.IsChecked) //hide it
				{
					resizeGrid.RowDefinitions.Last().Height = new GridLength(1);
					OutputLogSpliter.IsEnabled = false;
				}
				else //show it
				{
					resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
					OutputLogSpliter.IsEnabled = true;
				}

			}
		}

		private LinkedList<TimeBlock> GetDefaultTimeBlocks_LL()
		{
			LinkedList<TimeBlock> retLL = new LinkedList<TimeBlock>();
			NodeEditor.Components.BaseNodeBlock currentBlock = DialogueEditor_NodeGraph.StartExecutionBlock;
			AddTimeBlocksTillExit(ref retLL, currentBlock);

			return retLL;
		}

		private LinkedList<TimeBlock> GetChoiceTimeBlocks_LL(NodeEditor.Components.BaseNodeBlock currentBlock, int choice)
		{
			if (currentBlock.OutputNodes[choice].ConnectedNodes[0] != null &&
					currentBlock.OutputNodes[choice].ConnectedNodes[0].ParentBlock is NodeEditor.ExitBlockNode) return null;
			LinkedList<TimeBlock> retLL = new LinkedList<TimeBlock>();
			//clear the timeblocks
			DialogueEditor_Timeline.DeleteTimeBlocksAfter();

			try
			{
				//choose path and change pointer
				currentBlock = currentBlock.OutputNodes[choice].ConnectedNodes[0].ParentBlock;

				//add until exit is found.
				AddTimeBlocksTillExit(ref retLL, currentBlock);
			}
			catch
			{
				return retLL;
			}

			return retLL;
		}

		private void AddTimeBlocksTillExit(ref LinkedList<TimeBlock> output,
			NodeEditor.Components.BaseNodeBlock currentBlock)
		{
			//add until exit is found.
			try
			{
				while (!(currentBlock is NodeEditor.ExitBlockNode))
				{
					if (currentBlock is NodeEditor.StartBlockNode start)
					{
						currentBlock = start.ExitNode.ConnectedNodes[0].ParentBlock;
					}
					else if (currentBlock is NodeEditor.Components.DialogueNodeBlock dialogue)
					{
						if (!output.Contains(dialogue.LinkedTimeBlock as TimeBlock))
							output.AddLast(dialogue.LinkedTimeBlock as TimeBlock);
						currentBlock = dialogue.OutputNodes[0].ConnectedNodes[0].ParentBlock;
					}
					else if (currentBlock is NodeEditor.Components.SetConstantNodeBlock setConstant)
					{
						currentBlock = setConstant.ExitNode.ConnectedNodes[0].ParentBlock;
					}
					else if (currentBlock is NodeEditor.Components.ConditionalNodeBlock conditional)
					{
						currentBlock = conditional.TrueOutput.ConnectedNodes[0].ParentBlock;
					}
					else
					{
						currentBlock = currentBlock.OutputNodes[0].ConnectedNodes[0].ParentBlock;
					}
				}
			}
			catch
			{
				Console.WriteLine("Error Adding blocks to LL while traversing to exit");
				return;
			}
		}

		private void DisplayTimeBlocks_LL(LinkedList<TimeBlock> desiredLL)
		{
			DialogueEditor_Timeline.DeleteAllTimeBlocks();
			List<String> chars = new List<string>();
			foreach (SceneEntity character in CurActiveDialogueScene.Characters)
			{
				chars.Add(character.Name);
			}

			DialogueEditor_Timeline.AddTimeblock_LL(desiredLL, chars);
		}

		private void DisplayTimeBlocksAfter_LL(LinkedList<TimeBlock> desiredLL)
		{
			List<String> chars = new List<string>();
			foreach (SceneEntity character in CurActiveDialogueScene.Characters)
			{
				chars.Add(character.Name);
			}

			DialogueEditor_Timeline.AddTimeblock_LL(desiredLL, chars, false);

			foreach (var timeline in DialogueEditor_Timeline.GetTimelines())
			{
				if (timeline.TimelineisNull_flag)
				{
					TimeBlock fir = desiredLL.FirstOrDefault(x => x.Trackname == timeline.TrackName);
					LinkedListNode<TimeBlock> tbLL = timeline.timeBlocksLL.First;
					while (tbLL != null)
					{
						if (tbLL.Value == fir)
						{
							timeline.ActiveBlock = tbLL;
							break;
						}

						tbLL = tbLL.Next;
					}
				}
				else if (timeline.ActiveBlock != null)
					timeline.ActiveBlock = timeline.ActiveBlock.Next;
				else if (timeline.ActiveBlock == null)
					timeline.ActiveBlock = timeline.timeBlocksLL.First;

				timeline.TimelineisNull_flag = false;
			}

		}

		private void LoadTimelineWithSelectedPath(object selectedblock)
		{
			if (!(selectedblock is NodeEditor.Components.BaseNodeBlock)) return;
			DialogueEditor_Timeline.DeleteAllTimeBlocks(); //delete all timeline data

			//next up use the current block and build the new list from that.
			NodeEditor.Components.BaseNodeBlock tempblock = selectedblock as NodeEditor.Components.BaseNodeBlock;
			LinkedList<TimeBlock> timeBlocks = new LinkedList<TimeBlock>();

			AddTimeblocksTillStart(ref timeBlocks, tempblock);
			AddTimeBlocksTillExit(ref timeBlocks, tempblock);

			DisplayTimeBlocks_LL(timeBlocks);
		}

		private void AddTimeblocksTillStart(ref LinkedList<TimeBlock> output, NodeEditor.Components.BaseNodeBlock tempblock)
		{
			try
			{
				while (!(tempblock is NodeEditor.StartBlockNode))
				{
					//back track and add.
					if (tempblock is NodeEditor.ExitBlockNode exit)
					{
						tempblock = exit.EntryNode.ConnectedNodes[0].ParentBlock;
					}
					else if (tempblock is NodeEditor.Components.DialogueNodeBlock dialogue)
					{
						tempblock = dialogue.EntryNode.ConnectedNodes[0].ParentBlock;
						output.AddFirst(dialogue.LinkedTimeBlock as TimeBlock);
					}
					else if (tempblock is NodeEditor.Components.SetConstantNodeBlock setConstant)
					{
						tempblock = setConstant.EntryNode.ConnectedNodes[0].ParentBlock;
					}
					else if (tempblock is NodeEditor.Components.ConditionalNodeBlock conditional)
					{
						tempblock = conditional.EntryNode.ConnectedNodes[0].ParentBlock;
					}
					else
					{
						tempblock = tempblock.InputNodes[0].ConnectedNodes[0].ParentBlock;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private void SaveDialogueSceneAs_MI_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
			{
				Title = "New Level File",
				FileName = "", //default file name
				Filter = "txt files (*.dials)|*.lvl|All files (*.*)|*.*",
				FilterIndex = 2,
				InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Dialogue"),
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

			//get the Params list of this scene
			List<Tuple<String, object>> varDataList = new List<Tuple<string, object>>();
			foreach (BlockNodeEditor.RuntimeVars rtVars in DialogueEditor_NodeGraph.TestingVars_list)
			{
				varDataList.Add(new Tuple<string, object>(rtVars.VarName, rtVars.OrginalVarData));
			}

			List<NodeEditor.Components.BaseNodeBlock> BlockNodes = new List<NodeEditor.Components.BaseNodeBlock>();
			foreach (UIElement uie in DialogueEditor_NodeGraph.NodeCanvas.Children)
			{
				if (uie is NodeEditor.Components.BaseNodeBlock bn)
				{
					bn.LocX = Canvas.GetLeft(bn);
					bn.LocY = Canvas.GetTop(bn);
					BlockNodes.Add(bn);
				}
			}

			CurActiveDialogueScene.ExportScene(
				dlg.FileName, CurActiveDialogueScene.Characters.ToList(), CurActiveDialogueScene.DialogueBoxesFilePaths
				, varDataList, BlockNodes);
		}

		private void OpenDialogueScene_MI_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open Dialogue File",
				FileName = "", //default file name
				InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Dialogue"),
				Filter = "Dialogue Scene files (*.dials)|*.dials",
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
			int charcnt = 0;
			//DIALOGUE SCENE HOOKS
			SetupDialogueSceneHooks();

			List<Tuple<String, String, int, String, String, int>> connectiList =
				new List<Tuple<string, String, int, String, string, int>>();

			DialogueScene dia = DialogueScene.ImportScene(dlg.FileName, ref connectiList, EditorProjectContentDirectory);

			foreach (Timeline tl in dia.Timelines)
			{
				String actualFilePath = dia.Characters[charcnt++].DialogueSprites[0].ImgPathLocation;
				actualFilePath = actualFilePath.Replace("{Content}", EditorProjectContentDirectory);

				tl.TrackImagePath = actualFilePath;
				DialogueEditor_Timeline.AddTimeline(tl);
			}

			//this is an import function so we need to remove the DEFAULT start and End Block.
			//AND SET THE DialogueNodegraph.Start ref.
			DialogueEditor_NodeGraph.NodeCanvas.Children.Clear();
			foreach (NodeEditor.Components.BaseNodeBlock bn in dia.DialogueBlockNodes)
			{
				if (bn is NodeEditor.StartBlockNode)
					DialogueEditor_NodeGraph.StartExecutionBlock = bn;
			}

			//Add the variables to the scene
			for (int i = 1; i < dia.DialogueSceneParams.Count; i++)
			{
				DialogueEditor_NodeGraph.bAddNew_Flag = true;
				DialogueEditor_NodeGraph.AddRuntimeVar(dia.DialogueSceneParams[i]);
			}

			//If there are dialogue that have choices we need to set them.
			SetUpAddedDialogueBlocks(dia);

			//add characters to the editor objects section
			CurActiveDialogueScene = dia;
			Dialogue_CE_Tree.ItemsSource = CurActiveDialogueScene.Characters;
			for (int i = 0; i < CurActiveDialogueScene.Characters.Count; i++)
			{
				String filePath = CurActiveDialogueScene.DialogueBoxesFilePaths[i];
				filePath = filePath.Replace("{Content}", MonoGameProjectContentDirectory);

				CurActiveDialogueScene.DialogueBoxes.Add(BaseUI.ImportBaseUI(filePath));
				DialogueEditor_NodeGraph.SceneCharacters_list.Add(CurActiveDialogueScene.Characters[i].Name);
			}

			Console.WriteLine(Canvas.GetLeft(dia.DialogueBlockNodes[dia.DialogueBlockNodes.Count - 1]));


			DialogueEditor_NodeGraph.NodeCanvas.UpdateLayout();
			//connect all the blocks that we just added.
			foreach (Tuple<String, String, int, String, String, int> tup in connectiList)
			{
				try
				{
					NodeEditor.Components.BaseNodeBlock fromBlock = dia.DialogueBlockNodes.Find(x => x.Name == tup.Item1);
					NodeEditor.Components.ConnectionNode fromBlockNode = GetFromConnectionNode(fromBlock, tup);


					NodeEditor.Components.BaseNodeBlock toBlock = dia.DialogueBlockNodes.Find(x => x.Name == tup.Item4);
					NodeEditor.Components.ConnectionNode toBlockNode = GetToConnectionNode(toBlock, tup);

					DialogueEditor_NodeGraph.ConnectNodes(
						fromBlockNode,
						GetNodePosition(fromBlock, fromBlockNode),
						toBlockNode,
						GetNodePosition(toBlock, toBlockNode)
					);
				}
				catch
				{
					Console.WriteLine("FAILED CONNECTION @ :\n" + tup.ToString());
					continue;
				}
			}

			charcnt = 0;
			//Dialogue Boxes per character
			foreach (BaseUI gameUi in dia.DialogueBoxes)
			{
				OpenUIEdits.Add(gameUi);
				ContentControl CC = DrawUIToScreen(DialogueEditore_Canvas, DialogueEditor_BackCanvas, OpenUIEdits.Last(), false,
					CurActiveDialogueScene.Characters[charcnt].LinkedImageBox);

				//(DialogueEditor_Grid.Children[DialogueEditor_Grid.Children.Count - 1] as ContentControl).HorizontalAlignment =
				//	HorizontalAlignment.Right;
				//(DialogueEditor_Grid.Children[DialogueEditor_Grid.Children.Count - 1] as ContentControl).VerticalAlignment= VerticalAlignment.Bottom;
				//DrawUIToScreen(DialogueEditore_Canvas, DialogueEditor_BackCanvas, OpenUIEdits.Last(), false);

				//create the pointers to the content controls. which is what displays my images to the screen
				CurSceneEntityDisplays.Add(new Tuple<ContentControl, ContentControl>(CC, SelectedBaseUIControl));

				//Change the Display positon of the 
				HorizontalAlignment hori = 0;
				VerticalAlignment vert = 0;
				if (CurActiveDialogueScene.Characters[charcnt].HorizontalAnchor == HorizontalAlignment.Left.ToString())
				{
					hori = HorizontalAlignment.Left;
				}
				else
				{
					hori = HorizontalAlignment.Right;
				}

				if (CurActiveDialogueScene.Characters[charcnt].VerticalAnchor == VerticalAlignment.Top.ToString())
				{
					vert = VerticalAlignment.Top;
				}
				else
				{
					vert = VerticalAlignment.Bottom;
				}

				ChangeDialogueUIAnchorPostion_Hook(hori, vert, charcnt);
				DialogueEditor_NodeGraph.bAddNew_Flag = true;
				charcnt++;
			}
		}

		private NodeEditor.Components.ConnectionNode GetFromConnectionNode(NodeEditor.Components.BaseNodeBlock fromBlock,
			Tuple<String, String, int, String, String, int> tup)
		{
			NodeEditor.Components.ConnectionNode fromBlockNode = null;
			if (fromBlock is NodeEditor.StartBlockNode start) //|| fromBlock is ExitBlockNode)
			{
				fromBlockNode = start.ExitNode;
			}
			else if (fromBlock is NodeEditor.Components.Arithmetic.BaseArithmeticBlock arth)
			{
				fromBlockNode = arth.OutValue;
			}
			else if (fromBlock is NodeEditor.Components.Logic.BaseLogicNodeBlock logi)
			{
				fromBlockNode = logi.Output;
			}
			else if (fromBlock is NodeEditor.Components.ConditionalNodeBlock condi)
			{
				fromBlockNode = (tup.Item3 == 0 ? condi.TrueOutput : condi.FalseOutput);
			}
			else if (fromBlock is NodeEditor.Components.SetConstantNodeBlock seti)
			{
				if (tup.Item2 == "Exit")
					fromBlockNode = seti.ExitNode;
				else fromBlockNode = seti.OutValue;
			}
			else if (fromBlock is NodeEditor.Components.GetConstantNodeBlock getvar)
			{
				fromBlockNode = getvar.output;
			}
			else if (fromBlock is NodeEditor.Components.DialogueNodeBlock dialogue)
			{
				fromBlockNode = dialogue.OutputNodes[tup.Item3];
			}

			return fromBlockNode;
		}

		private NodeEditor.Components.ConnectionNode GetToConnectionNode(NodeEditor.Components.BaseNodeBlock fromBlock,
			Tuple<String, String, int, String, String, int> tup)
		{
			NodeEditor.Components.ConnectionNode fromBlockNode = null;
			if (fromBlock is NodeEditor.ExitBlockNode exit) //|| fromBlock is ExitBlockNode)
			{
				fromBlockNode = exit.EntryNode;
			}
			else if (fromBlock is NodeEditor.Components.Arithmetic.BaseArithmeticBlock arth)
			{
				fromBlockNode = arth.InputNodes[tup.Item6];
			}
			else if (fromBlock is NodeEditor.Components.Logic.BaseLogicNodeBlock logi)
			{
				fromBlockNode = logi.InputNodes[tup.Item6];
			}
			else if (fromBlock is NodeEditor.Components.ConditionalNodeBlock condi)
			{
				if (tup.Item5 == "Enter")
					fromBlockNode = condi.EntryNode;
				else fromBlockNode = condi.InputNodes[tup.Item6];
			}
			else if (fromBlock is NodeEditor.Components.SetConstantNodeBlock seti)
			{
				if (tup.Item5 == "Enter")
					fromBlockNode = seti.EntryNode;
				else fromBlockNode = seti.InputNodes[tup.Item6];
			}
			else if (fromBlock is NodeEditor.Components.DialogueNodeBlock dialogue)
			{
				if (tup.Item5 == "Enter")
					fromBlockNode = dialogue.EntryNode;
				else fromBlockNode = dialogue.InputNodes[tup.Item6];
			}

			return fromBlockNode;
		}

		private void SetUpAddedDialogueBlocks(DialogueScene dia)
		{
			//If there are dialogue that have choices we need to set them.
			foreach (NodeEditor.Components.BaseNodeBlock bn in dia.DialogueBlockNodes)
			{
				//display the nodes.
				Canvas.SetLeft(bn, bn.LocX);
				Canvas.SetTop(bn, bn.LocY);

				AllDialogueEditor_Grid.Visibility = Visibility.Visible;
				bn.ApplyTemplate();
				DialogueEditor_NodeGraph.AddToNodeEditor(bn);
				bn.ApplyTemplate();
				//var v = bn.FindResource("MainBorder");
				if (bn is NodeEditor.Components.DialogueNodeBlock dialo)
				{
					//add outputs display wise
					Button v = dialo.Template.FindName("AddOutputNode_BTN", dialo) as Button;
					if (dialo.OutputNodes.Count > 1)
					{
						dialo.bChoice = true;
						int i = 1;
						int j = dialo.DialogueTextOptions.Count;
						while (i++ < j)
						{
							DialogueEditor_NodeGraph.bAddNew_Flag = false;
							DialogueEditor_NodeGraph.AddDialogueBlockOutput(v);
							(dialo.LinkedTimeBlock as TimeBlock).Trackname = "choice";
						}
					}


					//add Inputs display wise
					Button b = dialo.Template.FindName("AddInputNode_BTN", dialo) as Button;
					if (dialo.InputNodes.Count > 1)
					{
						dialo.bChoice = true;
						int i = 1;
						int j = dialo.InputNodes.Count;
						while (i++ < j)
						{
							DialogueEditor_NodeGraph.bAddNew_Flag = false;
							DialogueEditor_NodeGraph.AddDialogueInput(b);
							(dialo.LinkedTimeBlock as TimeBlock).Trackname = "choice";
						}
					}
				}

				//CurSceneEntityDisplays.Add(new Tuple<ContentControl, ContentControl>(CC, SelectedBaseUIControl));
			}
		}

		private Point GetNodePosition(NodeEditor.Components.BaseNodeBlock NodeBlock,
			NodeEditor.Components.ConnectionNode DesNode)
		{
			Point retPoint = new Point(0, 0);

			if (NodeBlock is NodeEditor.StartBlockNode || NodeBlock is NodeEditor.ExitBlockNode)
			{
				if (NodeBlock.EntryNode == DesNode)
				{
					retPoint.X = Canvas.GetLeft(NodeBlock);
					retPoint.Y = Canvas.GetTop(NodeBlock) + 30;
				}
				else if (NodeBlock.ExitNode == DesNode)
				{
					retPoint.X = Canvas.GetLeft(NodeBlock) + 75;
					retPoint.Y = Canvas.GetTop(NodeBlock) + 30;
				}
			}
			else if (NodeBlock is NodeEditor.Components.Arithmetic.BaseArithmeticBlock ||
							 NodeBlock is NodeEditor.Components.Logic.BaseLogicNodeBlock
							 || NodeBlock is NodeEditor.Components.ConditionalNodeBlock
							 || NodeBlock is NodeEditor.Components.SetConstantNodeBlock)
			{
				if (NodeBlock.EntryNode == DesNode)
				{
					retPoint.X = Canvas.GetLeft(NodeBlock);
					retPoint.Y = Canvas.GetTop(NodeBlock) + 10;
				}
				else if (NodeBlock.ExitNode == DesNode)
				{
					retPoint.X = Canvas.GetLeft(NodeBlock) + 150;
					retPoint.Y = Canvas.GetTop(NodeBlock) + 10;
				}
				else if (NodeBlock.InputNodes.FindIndex(x => x == DesNode) >= 0)
				{
					int idx = NodeBlock.InputNodes.FindIndex(x => x == DesNode);
					retPoint.X = Canvas.GetLeft(NodeBlock);
					retPoint.Y = 20 + Canvas.GetTop(NodeBlock) + ((idx * 30) + 15);
				}
				else if (NodeBlock.OutputNodes.FindIndex(x => x == DesNode) >= 0)
				{
					int idx = NodeBlock.OutputNodes.FindIndex(x => x == DesNode);
					retPoint.X = 150 + Canvas.GetLeft(NodeBlock);
					retPoint.Y = 20 + Canvas.GetTop(NodeBlock) + ((idx * 30) + 15);
				}
			}
			else if (NodeBlock is NodeEditor.Components.GetConstantNodeBlock getvar)
			{
				if (getvar.output == DesNode)
				{
					retPoint.X = Canvas.GetLeft(NodeBlock) + 95;
					retPoint.Y = Canvas.GetTop(NodeBlock) + 40;
				}
			}
			else if (NodeBlock is NodeEditor.Components.DialogueNodeBlock)
			{
				//- I1=>20 + (40 * (i)) + 20
				//- O1=>X = 15,Y = 20 + (40 * (i)) + 20
				if (NodeBlock.EntryNode == DesNode)
				{
					retPoint.X = Canvas.GetLeft(NodeBlock);
					retPoint.Y = Canvas.GetTop(NodeBlock) + 10;
				}
				else if (NodeBlock.InputNodes.FindIndex(x => x == DesNode) >= 0)
				{
					int idx = NodeBlock.InputNodes.FindIndex(x => x == DesNode);
					retPoint.X = Canvas.GetLeft(NodeBlock);
					retPoint.Y = 20 + Canvas.GetTop(NodeBlock) + ((idx * 40) + 20);
				}
				else if (NodeBlock.OutputNodes.FindIndex(x => x == DesNode) >= 0)
				{
					int idx = NodeBlock.OutputNodes.FindIndex(x => x == DesNode);
					retPoint.X = 150 + Canvas.GetLeft(NodeBlock);
					retPoint.Y = 20 + (NodeBlock.InputNodes.Count * 40) + Canvas.GetTop(NodeBlock) + ((idx * 40) + 20);
				}
			}

			Console.WriteLine(retPoint);
			return retPoint;
		}

		private void SetupDialogueSceneHooks()
		{
			//DIALOGUE SCENE HOOKS
			AllDialogueEditor_Grid.Visibility = Visibility.Visible;

			DialogueEditor_Timeline.TimeBlockSync = DialogueHook;
			DialogueEditor_Timeline.SelectionChanged_Hook = ShowTimelineSelectedProperties;
			DialogueEditor_Timeline.ActiveTBblocks.CollectionChanged += ActiveTBblocks_CollectionChanged;
			DialogueEditor_Timeline.OnCreateTimeblockHook += CreateDialogueBlockForTimeline;
			DialogueEditor_Timeline.Reset_Hook += ResetExecution_Hook;
			DialogueEditor_Timeline.ChangeUIPosition_Hook += ChangeDialogueUIAnchorPostion_Hook;

			DialogueEditor_NodeGraph.CurrentErrors.CollectionChanged += NodeEditorCurrentErrorsOnCollectionChanged;
			DialogueEditor_NodeGraph.SelectionChanged_Hook = ShowTimelineSelectedProperties;
			DialogueEditor_NodeGraph.OnCreateTimeblockHook += CreateTimeBlockForDialogue;
			DialogueEditor_NodeGraph.TimelineLoad_Hook += LoadTimelineWithSelectedPath;


		}

	}
}
