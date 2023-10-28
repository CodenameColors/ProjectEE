using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BixBite;
using BixBite.Rendering;
using PropertyGridEditor;

namespace AmethystEngine.Forms
{
	public partial class EngineEditor 
	{
		public class LevelEditorProp
		{
			public String PropertyName { get; set; }
			public String PropertyData { get; set; }

			public LevelEditorProp()
			{
			}

			public LevelEditorProp(String PName)
			{
				PropertyName = PName;
				PropertyData = "";
			}
		}

		#region Properties

		#region LevelEditorVars

		#region Vars

		public ObservableCollection<Level> OpenLevels { get; set; }
		public Level CurrentLevel = new Level();
		public ImageBrush Imgtilebrush { get; private set; }
		public Control currentCC = new ContentControl(); //the current selected sprite
		private Point TileMapScrollVals = new Point();
		Point[] SelectionRectPoints = new Point[2];
		Rectangle LESelectRect;
		Point[] shiftpoints = new Point[3]; //0 = mouse down , 1 = mouse up , 2 on shifting "tick"
		private List<Image> TileSets = new List<Image>();
		private int GameEventNum;
		int LEGridHeight = 40;
		int LEGridWidth = 40;
		public double LEZoomLevel = 1;
		public List<EditorObject> LESpriteObjectList = new List<EditorObject>();
		private Sprite LESelectedSprite;

		#endregion

		#region LevelEditorUIPTRs

		ContentControl LESelectedSpriteControl = new ContentControl();
		Canvas MapLEditor_Canvas = new Canvas();
		Canvas FullMapLEditor_Canvas = new Canvas();
		VisualBrush FullMapLEditor_VB = new VisualBrush();
		Rectangle FullMapCanvasHightlight_rect = new Rectangle();

		VisualBrush TileMap_VB = new VisualBrush();
		Canvas TileMap_Canvas = new Canvas();
		ComboBox TileSets_CB = new ComboBox();
		Rectangle TileMapTiles_Rect = new Rectangle();
		Rectangle FullMapSelection_Rect = new Rectangle();
		Rectangle TileMapGrid_Rect = new Rectangle(); //TODO: add the selection to the tile map.
		public Rectangle LEcurect = new Rectangle();
		Point LEGridOffset = new Point();

		#endregion

		#endregion


		#endregion


		private void LevelEditorLoaded()
		{
			FullMapLEditor_Canvas =
				(Canvas)ObjectProperties_Control.Template.FindName("LevelEditor_Canvas", ObjectProperties_Control);
			FullMapLEditor_VB =
				(VisualBrush)ObjectProperties_Control.Template.FindName("FullLeditorGrid_VB", ObjectProperties_Control);
			TileMapTiles_Rect =
				(Rectangle)ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB_Rect",
					ContentLibrary_Control);
			TileMap_VB =
				(VisualBrush)ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB", ContentLibrary_Control);
			TileMapGrid_Rect =
				(Rectangle)ContentLibrary_Control.Template.FindName("TileMapGrid_Rect", ContentLibrary_Control);
			SceneExplorer_TreeView =
				(TreeView)SceneExplorer_Control.Template.FindName("LESceneExplorer_TreeView", SceneExplorer_Control);

			OpenLevels = new ObservableCollection<Level>();
			LoadMainLevel(ProjectFilePath);
		}

		#region "Level Editor"

		#region "Tile Map"

		/// <summary>
		/// This method is called when the user clicks on the tile map after they have imported one.
		/// After clicking it then creates a tile brush for painting the level later.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TileMap_Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			int xtile, ytile, TileSetOffest = 0;
			xtile = CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item3;
			ytile = CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item4;

			//LevelEditorTIleMap_SV.scrol


			//there can be multiple tile sets per level file. 
			for (int i = 0; i < TileSets_CB.SelectedIndex; i++)
			{
				BitmapImage im1 = new BitmapImage();
				im1.BeginInit();
				im1.UriSource = new Uri(CurrentLevel.TileSet[i].Item2);
				im1.EndInit();

				System.Drawing.Image img = System.Drawing.Image.FromFile(CurrentLevel.TileSet[i].Item2);
				//img.Source = im1;
				TileSetOffest +=
					(int)((img.Width / CurrentLevel.TileSet[i].Item3) * (img.Height / CurrentLevel.TileSet[i].Item4));
			}

			TileMapGrid_Rect.Visibility = Visibility.Hidden;
			SelectedTile_Canvas.Children.Clear();
			Point pp = Mouse.GetPosition(TileMap_Canvas);
			pp.X -= Math.Floor(pp.X) % xtile; //TODO: Add the offset so we can fill the grid AFTER PAnNNG
			pp.Y -= Math.Floor(pp.Y) % ytile;
			int x = (int)pp.X;
			int y = (int)pp.Y;
			Console.WriteLine(String.Format("x: {0},  y: {1}", x, y));
			Console.WriteLine("");

			string tileSetPath = CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item2;
			tileSetPath = tileSetPath.Replace("{Content}", EditorProjectContentDirectory);
			if (!System.IO.File.Exists(tileSetPath))
			{
				Console.WriteLine("Tile Set image doesn't exist.");
				return;
			}

			BitmapImage bmp = new BitmapImage(new Uri(tileSetPath));
			var crop = new CroppedBitmap(bmp, new Int32Rect(x, y, xtile, ytile));
			// using BitmapImage version to prove its created successfully
			Image image2 = new Image
			{
				Source = crop //cropped
			};
			Imgtilebrush = new ImageBrush(image2.Source);
			//calculate the int value in canvas "array"
			int tilenumdata = ((y / ytile) * ((int)TileMap_Canvas.ActualWidth / xtile)) + (x / xtile);
			tilenumdata += TileSetOffest;

			SelectedTile_Canvas.Children.Add(new Rectangle()
			{
				Width = xtile,
				Height = ytile,
				Fill = Imgtilebrush,
				Tag = tilenumdata,
				RenderTransform = new ScaleTransform(.8, .8)
			});
			SelectedTile_Canvas.ToolTip = tilenumdata;
			TileMapGrid_Rect.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// Used for displaying the position of the mouse on the canvas of the tile map
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TileMap_Canvas_MouseMove(object sender, MouseEventArgs e)
		{
			Canvas TileMap_Canvas_temp =
				(Canvas)(ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control));
			Point p = Mouse.GetPosition(TileMap_Canvas_temp);
			String point = String.Format("({0}, {1})", (int)p.X, (int)p.Y);
			LevelEditorCords_TB.Text = point;
		}

		#endregion

		#region "Main Editor Canvas"

		/// <summary>
		/// handles left mouse button down events.
		/// Painting the tile canvas is handled here.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;

			Point pos = Mouse.GetPosition(LevelEditor_BackCanvas);
			//Point p = GetGridSnapCords(Mouse.GetPosition(LevelEditor_Canvas));
			Point p = GetGridSnapCords(pos);
			Console.WriteLine(String.Format("Snapped grid cords: {0}", p.ToString()));
			//we need to get the current Sprite layer that is currently clicked.
			int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);

			if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
			{
				if (CurrentTool == EditorTool.Brush)
				{
					//Are we allowed to paint?
					if (Canvas_grid.Viewport.X > 0 || Canvas_grid.Viewport.Y > 0)
						return; //we are out of the bounds negative wise (viewport)
					if ((int)CurrentLevel.GetPropertyData("xCells") < p.X / 40 ||
							(int)CurrentLevel.GetPropertyData("yCells") < p.Y / 40)
						return;

					if (SelectedTile_Canvas.Children.Count == 0) return;
					Rectangle
						r = new Rectangle()
						{
							Width = 40,
							Height = 40,
							Fill = Imgtilebrush
						}; //create the tile that we wish to add to the grid. always 40 becasue thats the base. 

					Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas,
						LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(),
						curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
					if (rr != null)
					{
						//If Desired tile is the same as the painted tile. Do nothing
						int x = (int)Canvas.GetTop(rr) / 40;
						int y = (int)Canvas.GetLeft(rr) / 40;
						int arrval = ((int[,])CurrentLevel.Layers[curLayer].LayerObjects)[x, y];
						Console.WriteLine(arrval);
						if (int.Parse(((Rectangle)SelectedTile_Canvas.Children[0]).Tag.ToString()) == arrval)
						{
							//the value of data 
							Console.WriteLine(String.Format("Cell ({0},{1}) already is filled", (int)pos.X, (int)pos.Y));
							return; //check to see if the current tile exists. if so then don't add.
						}
						else //they are different. So delete it.
						{
							LevelEditor_Canvas.Children.Remove(rr);
						}
					}
					else
					{
						Console.WriteLine("Rectangle not found So imma add one");
					}

					Canvas.SetLeft(r, (int)p.X);
					Canvas.SetTop(r, (int)p.Y);
					Canvas.SetZIndex(r, curLayer); //place the tile position wise
					LevelEditor_Canvas.Children.Add(r); //actual place it on the canvas

					//add offset to point P to turn rel to Abs pos.
					p.X += Math.Ceiling(Math.Abs(Canvas_grid.Viewport.X));
					p.Y += Math.Ceiling(Math.Abs(Canvas_grid.Viewport.Y));
					p.X = (int)p.X;
					p.Y = (int)p.Y;
					FullMapEditorFill(p, curLayer); //update the fullmap display to reflect this change
				}
				else if (CurrentTool == EditorTool.Select)
				{
					//find the tile
					if (SceneExplorer_TreeView.SelectedValue is SpriteLayer &&
							((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
					{
						SelectionRectPoints[0] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X,
							(int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
					}
					else if (SceneExplorer_TreeView.SelectedValue is SpriteLayer &&
									 ((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.GameEvent)
					{
						//this selection works similar to tile. But instead it looks for borders not rectangles. It also will retrieve the game event group number
						Rectangle r = new Rectangle()
						{ Tag = "selection", Width = 40, Height = 40, Fill = new SolidColorBrush(Color.FromArgb(100, 0, 20, 100)) };
					}
				}
				else if (CurrentTool == EditorTool.Eraser)
				{
					//find the tile on the layer that is selected.
					Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas,
						LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(),
						curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
					if (rr == null) return; //if you click on a empty rect return
																	//find the rect in the current layers displayed tiles
					int i = LevelEditor_Canvas.Children.IndexOf(rr);
					int x = -1;
					int y = -1;
					if (i >= 0)
					{
						y = (int)Canvas.GetLeft(rr) / 40;
						x = (int)Canvas.GetTop(rr) / 40;
						LevelEditor_Canvas.Children.RemoveAt(i); //delete it.
					}

					if (x < 0 || y < 0)
					{
						Console.WriteLine("desired cell to delete DNE");
						return;
					}

					//find the data in the level objects sprite layer. And then clear it.
					SpriteLayer curlayer = (SpriteLayer)SceneExplorer_TreeView.SelectedValue;
					curlayer.DeleteFromLayer(x, y);

					//delect
					Deselect();
				}
				else if (CurrentTool == EditorTool.Move)
				{
					//SelectionRectPoints[0] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
					shiftpoints[0] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X,
						(int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
					shiftpoints[2] = shiftpoints[0];
				}

			}
			else if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Sprite)
			{
				TabControl Content_TC =
					(TabControl)(ContentLibrary_Control.Template.FindName("LevelEditorLibary_TabControl", ContentLibrary_Control)
					);
				//make sure we are in the correct tab of the level editor content libary
				if (Content_TC.SelectedIndex == 1 && CurrentTool == EditorTool.Image)
				{
					//is there a sprite selected?
					ListBox Sprite_LB =
						(ListBox)(ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control));
					if (Sprite_LB.SelectedIndex >= 0)
					{
						List<EditorObject> sprites = (List<EditorObject>)Sprite_LB.ItemsSource;
						EditorObject currentobj = sprites[Sprite_LB.SelectedIndex];
						Console.WriteLine(currentobj.Thumbnail.AbsolutePath); //prints the path

						System.Drawing.Image currentimg = System.Drawing.Image.FromFile(currentobj.Thumbnail.AbsolutePath);
						BitmapImage bitmap = new BitmapImage(new Uri(currentobj.Thumbnail.AbsolutePath, UriKind.Absolute));
						Image img = new Image
						{
							Source = bitmap
						};
						Rectangle r = new Rectangle()
						{
							Width = currentimg.Width,
							Height = currentimg.Height,
							Fill = new ImageBrush(img.Source)
						}; //Make a rectange teh size of the image
						Canvas.SetLeft(r, pos.X);
						Canvas.SetTop(r, pos.Y);
						Canvas.SetZIndex(r, curLayer);
						//LevelEditor_Canvas.Children.Add(r);

						ContentControl CC = ((ContentControl)this.TryFindResource("MoveableImages_Template"));
						CC.Width = currentimg.Width;
						CC.Height = currentimg.Height;

						Canvas.SetLeft(CC, pos.X);
						Canvas.SetTop(CC, pos.Y);
						Canvas.SetZIndex(CC, curLayer);
						Selector.SetIsSelected(CC, false);
						Selector.AddUnselectedHandler(CC, Sprite_OnUnselected); //an event to call when we un select a sprite!
						CC.MouseRightButtonDown += ContentControl_MouseLeftButtonDown;
						var m = (CC.FindName("ResizeImage_rect"));
						((Rectangle)CC.Content).Fill = new ImageBrush(img.Source);
						LevelEditor_Canvas.Children.Add(CC);

						//add to minimap
						Sprite s = new Sprite("new sprite", currentobj.Thumbnail.AbsolutePath, (int)pos.X, (int)pos.Y,
							currentimg.Width, currentimg.Height);
						FullMapEditorFill(s, new ImageBrush(img.Source), curLayer);
						//add to the current levers layer.
						CurrentLevel.Layers[curLayer].AddToLayer(s);

					}
				}

				//is there a sprite selected?
			}
			else if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.GameEvent)
			{
				if (CurrentTool == EditorTool.Gameevent)
				{
					Border bor = SelectTool.FindBorder(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Border>().ToList(),
						curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
					if (bor != null) return; //the game event is already declared here!

					List<GameEvent> layergameevents =
						((Tuple<int[,], List<GameEvent>>)((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerObjects).Item2;
					if (GameEventNum < 0) return;

					TextBlock tb = new TextBlock()
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						TextAlignment = TextAlignment.Center,
						TextWrapping = TextWrapping.Wrap,
						Width = 40,
						Height = 40,
						FontSize = 18,
						Text = ((int)layergameevents[GameEventNum].GetPropertyData("group") == -1
							? ""
							: layergameevents[GameEventNum].GetPropertyData("group").ToString()),
						Tag = layergameevents[GameEventNum].GetPropertyData("group").ToString(),
						Foreground = new SolidColorBrush(Colors.Black),
						Background = ((int)layergameevents[GameEventNum].GetPropertyData("group") == -1
							? new SolidColorBrush(Color.FromArgb(100, 255, 0, 0))
							: new SolidColorBrush(Color.FromArgb(100, 100, 100, 100))),
					};
					Border b = new Border() { Width = 40, Height = 40 };
					b.Child = tb;

					Canvas.SetLeft(b, (int)p.X);
					Canvas.SetTop(b, (int)p.Y);
					Canvas.SetZIndex(b, curLayer); //place the tile position wise
					LevelEditor_Canvas.Children.Add(b); //actual place it on the canvas

					//create event data for the game event.


					CurrentLevel.Layers[curLayer].AddToLayer((int)layergameevents[GameEventNum].GetPropertyData("group"),
						(int)p.Y / 40, (int)p.X / 40, null);
					EventData ed = new EventData()
					{
						newx = 0,
						newy = 0,
						MoveTime = 0,
						NewFileToLoad = ""
					};

					//add the event data. how the palyer will change FROM this event
					//((Tuple<int[,], List<GameEvent>>)((SpriteLayer)CurrentLevel.Layers[curLayer]).LayerObjects).
					//	SpriteSheetName.Last().AddEventData(ed, "DeleTest1", "NULL");
				}
				else if (CurrentTool == EditorTool.Select) //selected a game event square.
				{
					//use the snapped grid cords to find the Border that we are clicking in.
					Border bor = SelectTool.FindBorder(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Border>().ToList(),
						curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
					int DesGroup = 0;
					GameEvent ge;
					if (bor == null) return;
					else if (Int32.TryParse(((TextBlock)bor.Child).Text, out DesGroup))
						ge = SelectTool.FindGameEvent(CurrentLevel.Layers[curLayer], bor, DesGroup);
					else return;
					//if (ge == null) return; //Event found but on the wrong layer so ignore

					PropGrid PGrid =
						((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
					PGrid.ClearProperties();
					int i = 0;
					foreach (object o in ge.GetProperties().Select(m => m.Item1))
					{
						if (o is String || o is int)
						{
							PGrid.AddProperty(ge.GetProperties().Select(m => m.Item2).ToList()[i].ToString(),
								new TextBox() { IsEnabled = false }, o.ToString(), ge.PropertyCallback);
						}

						i++;
					}

					Console.WriteLine("GE - Selected");
				}
				else if (CurrentTool == EditorTool.Eraser)
				{
					//use the snapped grid cords to find the Border that we are clicking in.
					Border bor = SelectTool.FindBorder(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Border>().ToList(),
						curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
					int DesGroup = 0;
					GameEvent ge;
					if (bor == null) return;
					else if (Int32.TryParse(((TextBlock)bor.Child).Tag.ToString(), out DesGroup))
						ge = SelectTool.FindGameEvent(CurrentLevel.Layers[curLayer], bor, DesGroup);
					else return;

					LevelEditor_Canvas.Children.Remove(bor); //deletes display wise

					//Deletes data wise.
					int cellx = (int)p.X / 40;
					int celly = (int)p.Y / 40;
					((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[curLayer].LayerObjects).Item1[celly, cellx] = 0;
				}
			}
		}

		/// <summary>
		/// This method will be called when mousedown occurs on a SPRITE object.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContentControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			//are we on a sprite layer?
			if (!(SceneExplorer_TreeView.SelectedItem is SpriteLayer)) return;
			if (((SpriteLayer)SceneExplorer_TreeView.SelectedItem).layerType != LayerType.Sprite) return;


			Console.WriteLine("testing");
			Selector.SetIsSelected(((Control)currentCC), false);
			LEcurect.IsHitTestVisible = true;

			//the current CC has been changed. so we need to reflect that in the data
			//TODO:
			if (currentCC != null)
			{

			}

			currentCC = ((ContentControl)sender);
			Selector.SetIsSelected(((Control)currentCC), true);
			LEcurect.IsHitTestVisible = false;
		}

		/// <summary>
		/// Set the current selected sprite on a mouse click. And display its properties in the properties grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Sprite_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			//are we on a sprite layer?
			if (!(SceneExplorer_TreeView.SelectedItem is SpriteLayer)) return;
			if (((SpriteLayer)SceneExplorer_TreeView.SelectedItem).layerType != LayerType.Sprite) return;

			Console.WriteLine("sjdsjdns");
			LEcurect.IsHitTestVisible = true;
			LEcurect = ((Rectangle)sender);
			LEcurect.IsHitTestVisible = false;
			//var m = Application.Current.MainWindow.FindResource("DesignerItemStyle");

			//find out what sprite have we clicked on!
			Point pos = Mouse.GetPosition(LevelEditor_BackCanvas);
			int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
			List<Sprite> lsprites = (List<Sprite>)((SpriteLayer)SceneExplorer_TreeView.SelectedItem).LayerObjects;

			ContentControl cc = SelectTool.FindSpriteControl(LevelEditor_Canvas,
				LevelEditor_Canvas.Children.OfType<ContentControl>().ToList(),
				curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
			Sprite spr = SelectTool.FindSprite(lsprites, cc);

			if (spr == null) return; //cannot find to return.

			int i = 0;
			//display properties to the properties grid
			PropGrid LB =
				((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
			LB.ClearProperties();
			foreach (object o in spr.GetProperties().Select(m => m.Item2))
			{
				if (o is String || o is int)
				{
					LB.AddProperty(spr.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(),
						spr.PropertyTBCallback);
				}

				i++;
			}


		}

		/// <summary>
		/// This method is called then the mouse click is ending on the Level Editor canvas.
		/// This is used to set variables like selection area etc.
		/// Because its better to set those at the END of mouse movement.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditor_BackCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if ((SceneExplorer_TreeView.SelectedValue) == null) return;
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;
			if ((SceneExplorer_TreeView.SelectedValue is SpriteLayer))
			{
				if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
				{
					if (CurrentTool == EditorTool.Brush)
					{

					}
					else if (CurrentTool == EditorTool.Select)
					{

						Point pos = Mouse.GetPosition(LevelEditor_BackCanvas);
						//we need to get the current Sprite layer that is currently clicked.
						if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue == null)) return;
						int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);

						//SelectionRectPoints[1] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y);

						//change the selection points data to match the new selection. DESPITE the cord system it was moved in.
						int x0, x1, y0, y1;
						x0 = (int)Canvas.GetLeft(LESelectRect);
						y0 = (int)Canvas.GetTop(LESelectRect);


						x1 = (int)(x0 + LESelectRect.Width);
						y1 = (int)(y0 + LESelectRect.Height);

						if (x0 + x1 + y0 + y1 <= 0)
							return;

						SelectionRectPoints[0] = new Point(x0, y0);
						SelectionRectPoints[1] = new Point(x1, y1);

						Console.WriteLine("Select MUP");

						//At this point the entire selection area should be drawn.
						int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
						int columns =
							(int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
						int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

						columns /= relgridsize;
						rows /= relgridsize;

						Point begginning = RelativeGridSnap(SelectionRectPoints[0]);
						begginning.X = (int)begginning.X;
						begginning.Y = (int)begginning.Y;
						for (int i = 0; i < columns; i++)
						{
							for (int j = 0; j < rows; j++)
							{
								//find the rectangle
								Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas,
									LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(),
									curLayer, (int)begginning.X + (relgridsize * i) + 1, (int)begginning.Y + (relgridsize * j) + 1,
									(int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
								if (!selectTool.SelectedTiles.Contains(rr))
								{
									if (rr != null)
										selectTool.SelectedTiles.Add(rr);
								}
							}
						}
					}
					else if (CurrentTool == EditorTool.Eraser)
					{

					}
					else if (CurrentTool == EditorTool.Fill)
					{

					}
					else if (CurrentTool == EditorTool.Move)
					{
						int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
						int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));


						Point begginning = RelativeGridSnap(SelectionRectPoints[0]);
						begginning.X = (int)begginning.X;
						begginning.Y = (int)begginning.Y;
						shiftpoints[1] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X,
							(int)e.GetPosition(LevelEditor_BackCanvas).Y);

						int columns =
							(int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
						int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);
						//how much to move/shift the data.
						int shiftcolumns = (int)(RelativeGridSnap(shiftpoints[1]).X - RelativeGridSnap(shiftpoints[0]).X);
						int shiftrows = (int)(RelativeGridSnap(shiftpoints[1]).Y - RelativeGridSnap(shiftpoints[0]).Y);
						int absrow = 0;
						int abscol = 0;
						Point p = GetGridSnapCords(Mouse.GetPosition(LevelEditor_BackCanvas));
						columns /= relgridsize;
						shiftcolumns /= relgridsize;
						rows /= relgridsize;
						shiftrows /= relgridsize;
						for (int i = 0; i < columns; i++)
						{
							for (int j = 0; j < rows; j++)
							{
								absrow = ((int)begginning.Y + (relgridsize * j) + (int)Math.Abs(Canvas_grid.Viewport.Y)) /
												 LEGridHeight;
								abscol = ((int)begginning.X + (relgridsize * i) + (int)Math.Abs(Canvas_grid.Viewport.X)) /
												 LEGridWidth;
								CurrentLevel.Movedata(absrow, abscol, shiftrows, shiftcolumns, curLayer, LayerType.Tile);
							}
						}
					}
				}

				else if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Sprite)
				{

				}
				else if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.GameEvent)
				{

				}

			}
		}

		/// <summary>
		/// Use this to get any Zindex of an object on a canvas. 
		/// </summary>
		/// <param name="treeitem"></param>
		/// <returns></returns>
		private int GetTileZIndex(TreeView treeitem)
		{
			//are we clicked on a spritelayer? AND a tile layer?
			if (treeitem.SelectedValue is SpriteLayer && ((SpriteLayer)treeitem.SelectedValue).layerType == LayerType.Tile)
			{
				//what layer are we on?
				foreach (Level lev in OpenLevels)
				{
					foreach (SpriteLayer sl in lev.Layers)
					{
						if (sl.LayerName == ((SpriteLayer)treeitem.SelectedItem).LayerName)
						{
							return lev.Layers.IndexOf(sl);
						}
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Used to get the location to snap to based on the current grid scale. TOP LEFT snap
		/// </summary>
		/// <param name="p"></param>
		/// <param name="abs"></param>
		/// <returns></returns>
		private Point RelativeGridSnap(Point p, bool abs = true)
		{

			//find the Relative grid size in pixels
			int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
			//find the offset amount.
			int Xoff = (int)(Math.Abs(Canvas_grid.Viewport.X));
			int YOff = (int)(Math.Abs(Canvas_grid.Viewport.Y));

			//what is the left over amount?
			Xoff %= 40;
			YOff %= 40;

			//relative snap offset
			Xoff = 40 - Xoff;
			YOff = 40 - YOff;

			Xoff = (int)(Xoff * LevelEditor_Canvas.RenderTransform.Value.M11);
			YOff = (int)(YOff * LevelEditor_Canvas.RenderTransform.Value.M11);

			if (Xoff == 40) Xoff = 0;
			if (YOff == 40) YOff = 0;

			//divide the sumation by the relative grid size
			Point relpoint = new Point((int)((p.X - Xoff) / relgridsize), (int)((p.Y - YOff) / relgridsize));
			relpoint.X *= (relgridsize);
			relpoint.Y *= (relgridsize);

			if (abs)
			{
				//return the abs size. Base 40x40 grid.
				return
					new Point(relpoint.X + Xoff,
						relpoint.Y + YOff); //this gives us the cell number. Use this and multiply by the base value.
			}
			else //rel grid size
			{
				return new Point();
			}


		}

		/// <summary>
		/// Used to get the location to snap to based on the current grid scale. TOP LEFT snap
		/// </summary>
		/// <param name="p"></param>
		/// <param name="abs"></param>
		/// <returns></returns>
		private Point GetGridSnapCords(Point p)
		{
			int Xoff = 0;
			int YOff = 0;
			if (p.X >= 40 || (int)(Math.Abs(Canvas_grid.Viewport.X)) > 0)
			{
				Xoff = (int)(Math.Abs(Canvas_grid.Viewport.X)) % LEGridWidth;
				Xoff = LEGridWidth - Xoff;
			} //offset

			if (p.Y >= 40 || (int)(Math.Abs(Canvas_grid.Viewport.Y)) > 0)
			{
				YOff = (int)(Math.Abs(Canvas_grid.Viewport.Y)) % LEGridHeight;
				YOff = LEGridHeight - YOff;
			}

			p.X /= LEZoomLevel;
			p.Y /= LEZoomLevel;
			p.X -= Math.Floor(p.X - Xoff) % LEGridWidth; //TODO: Add the offset so we can fill the grid AFTER PAnNNG
			p.Y -= Math.Floor(p.Y - YOff) % LEGridHeight;
			return p;
		}

		/// <summary>
		/// This method takes care of mouse movement events on the main level editor canvas.
		/// Panning is handled in here.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditor_BackCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;

			//we need to display the cords.
			Point p = Mouse.GetPosition(LevelEditor_BackCanvas);
			String point = String.Format("({0}, {1}) OFF:({2}, {3})", (int)p.X, (int)p.Y, (int)Canvas_grid.Viewport.X,
				(int)Canvas_grid.Viewport.Y);
			LevelEditorCords_TB.Text = point;

			//we need to get the current Sprite layer that is currently clicked.
			int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);


			//which way is mouse moving?
			MPos -= (Vector)e.GetPosition(LevelEditor_Canvas);


			//is the middle mouse button down?
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				LevelEditorPan();
			}

			if (((SpriteLayer)(SceneExplorer_TreeView.SelectedValue)).layerType == LayerType.Tile)
			{
				if (e.LeftButton == MouseButtonState.Pressed && CurrentTool == EditorTool.Brush)
					Canvas_MouseLeftButtonDown(sender, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));
				else if (e.LeftButton == MouseButtonState.Pressed && CurrentTool == EditorTool.Move)
				{
					BixBite.BixBiteTypes.CardinalDirection cd = GetDCirectionalMove(p, 0); //TODO: CHANGE THE Z INDEX TO VARIABLE
					switch (cd)
					{
						//TODO: ADD the logic to change the data positions.
						case (BixBite.BixBiteTypes.CardinalDirection.N):
							for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
							{
								Canvas.SetTop(selectTool.SelectedTiles[i],
									Canvas.GetTop(selectTool.SelectedTiles[i]) - selectTool.SelectedTiles[i].ActualWidth);
							}

							//EditorObjectAnimationPreviewsList_Template
							shiftpoints[2] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X,
								(int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
							break;
						case (BixBite.BixBiteTypes.CardinalDirection.S):
							for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
							{
								Canvas.SetTop(selectTool.SelectedTiles[i],
									Canvas.GetTop(selectTool.SelectedTiles[i]) + selectTool.SelectedTiles[i].ActualWidth);
							}

							shiftpoints[2] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X,
								(int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
							break;
						case (BixBite.BixBiteTypes.CardinalDirection.W):
							for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
							{
								Canvas.SetLeft(selectTool.SelectedTiles[i],
									Canvas.GetLeft(selectTool.SelectedTiles[i]) - selectTool.SelectedTiles[i].ActualWidth);
							}

							shiftpoints[2] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X,
								(int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
							break;
						case (BixBite.BixBiteTypes.CardinalDirection.E):
							for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
							{
								Canvas.SetLeft(selectTool.SelectedTiles[i],
									Canvas.GetLeft(selectTool.SelectedTiles[i]) + selectTool.SelectedTiles[i].ActualWidth);
							}

							shiftpoints[2] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X,
								(int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
							break;
					}
				}
				else if (e.LeftButton == MouseButtonState.Pressed && CurrentTool == EditorTool.Select)
				{
					Point pp = GetGridSnapCords(SelectionRectPoints[0]);
					Point Snapped = RelativeGridSnap(p); //If we have then find the bottom right cords of that cell.
					int wid = (int)GetGridSnapCords(p).X - ((int)pp.X);
					int heigh = (int)GetGridSnapCords(p).Y - ((int)pp.Y);

					int[] CurrentPos = { (int)GetGridSnapCords(p).X, (int)GetGridSnapCords(p).Y };

					if (wid >= 0 && heigh < 0) //Quadrant [+ -]
					{
						LESelectRect.Width = CurrentPos[0] - pp.X + 40;
						LESelectRect.Height = pp.Y + 40 - CurrentPos[1];
						Canvas.SetLeft(LESelectRect, pp.X);
						Canvas.SetTop(LESelectRect, CurrentPos[1]);
						Canvas.SetZIndex(LESelectRect, 100);
						//SelectionRectPoints[1] = new Point
					}
					else if (wid < 0 && heigh >= 0) //Quadrant [- +]
					{
						LESelectRect.Width = pp.X - CurrentPos[0] + 40;
						LESelectRect.Height = CurrentPos[1] - pp.Y + 40;
						Canvas.SetLeft(LESelectRect, CurrentPos[0]);
						Canvas.SetTop(LESelectRect, pp.Y);
						Canvas.SetZIndex(LESelectRect, 100);
					}
					else if (wid >= 0 && heigh >= 0) //Quadrant [+ +]
					{
						LESelectRect.Width = CurrentPos[0] - pp.X + 40;
						LESelectRect.Height = CurrentPos[1] - pp.Y + 40;
						Canvas.SetLeft(LESelectRect, pp.X);
						Canvas.SetTop(LESelectRect, pp.Y);
						Canvas.SetZIndex(LESelectRect, 100);
					}
					else //Quadrant [- -]
					{
						LESelectRect.Width = (wid * -1) + 40;
						LESelectRect.Height = (heigh * -1) + 40;
						Canvas.SetLeft(LESelectRect, CurrentPos[0]);
						Canvas.SetTop(LESelectRect, CurrentPos[1]);
						Canvas.SetZIndex(LESelectRect, 100);
					}



					Console.WriteLine(String.Format("W:{0}, H{1}", wid, heigh));

					//the drawing, and data manuplation will have to occur on LEFTMOUSEBUTTONUP
					LESelectRect.Fill = new SolidColorBrush(Color.FromArgb(100, 0, 20, 100));
					LevelEditor_Canvas.Children.Remove(LESelectRect);
					LevelEditor_Canvas.Children.Add(LESelectRect);
					//r.MouseLeftButtonUp += LevelEditor_BackCanvas_MouseLeftButtonUp;
					//Deselect();


				}
			}
			else if (((SpriteLayer)(SceneExplorer_TreeView.SelectedValue)).layerType == LayerType.Sprite)
			{

			}
			else if (((SpriteLayer)(SceneExplorer_TreeView.SelectedValue)).layerType == LayerType.GameEvent)
			{

			}

			MPos = e.GetPosition(LevelEditor_Canvas); //set this for the iteration
		}

		//this method is here to update the size of rectangle on the fullmap on the right.
		/// <summary>
		/// This method handles the changing of canvas size.
		/// Changes the Selection Rect size to match the ratio of the main editor canvas.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditor_BackCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (FullMapLEditor_Canvas == null) return;
			if (FullMapLEditor_Canvas.Children.Count == 0) return;

			int MainCurCellsX = ((int)Math.Ceiling((LevelEditor_BackCanvas.ActualWidth / Canvas_grid.Viewport.Width)));
			int MainCurCellsY = ((int)Math.Ceiling(LevelEditor_BackCanvas.ActualHeight / Canvas_grid.Viewport.Height));


			//FullMapLEditor_Canvas.Children.RemoveAt(0);

			FullMapSelection_Rect = new Rectangle()
			{
				Width = MainCurCellsX * 10,
				Height = MainCurCellsY * 10,
				Stroke = Brushes.White,
				StrokeThickness = 1,
				Name = "SelectionRect"
			};
			Canvas.SetLeft(FullMapSelection_Rect, 0);
			Canvas.SetTop(FullMapSelection_Rect, 0);
			Canvas.SetZIndex(FullMapSelection_Rect, 100);

			//FullMapLEditor_Canvas.Children.Add(FullMapSelection_Rect);

		}

		/// <summary>
		/// Choose the cells in the X direction of the new map.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void XCellsVal_TB_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (int.TryParse(((TextBox)sender).Text, out int numval) && Int32.TryParse(XCellsWidth_TB.Text, out int pixval))
			{
				LevelWidth_TB.Text = (numval * pixval).ToString();
			}
			else LevelWidth_TB.Text = "0";
		}

		/// <summary>
		/// Choose the cells in the Y direction of the new map
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void YCellsVal_TB_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Int32.TryParse(((TextBox)sender).Text, out int numval) &&
					Int32.TryParse(XCellsHeight_TB.Text, out int pixval))
			{
				LevelHeight_TB.Text = (numval * pixval).ToString();
			}
			else LevelHeight_TB.Text = "0";
		}

		/// <summary>
		/// Creates a new level. This method is here to grab all the properties.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CreateLevel_BTN_Click(object sender, RoutedEventArgs e)
		{
			String LName = LevelName_TB.Text;
			bool bLName =
				System.Text.RegularExpressions.Regex.IsMatch(LName, @"^[A-Za-z][A-Za-z0-9]+");

			if (bLName)
			{
				Console.WriteLine("Valid");
				int ynum = 0;
				if (Int32.TryParse(XCellsVal_TB.Text, out int xnum))
				{
					if (Int32.TryParse(YCellsVal_TB.Text, out ynum))
					{
						CreateLevel(LName, xnum, ynum);
					}
				}
			}
			else MessageBox.Show("Invalid Level name");

		}

		#region "Panning"

		/// <summary>
		/// Performs the panning effect on the main level editor canvas.
		/// </summary>
		private void LevelEditorPan()
		{
			//this is here so when we pan the tiles work with the relative cords we are moving to. Its allows the tiles to maintain position data.
			foreach (UIElement child in LevelEditor_Canvas.Children)
			{

				double x = Canvas.GetLeft(child);
				double y = Canvas.GetTop(child);
				Canvas.SetLeft(child, x + MPos.X);
				Canvas.SetTop(child, y + MPos.Y);
			}

			//moves the Grid, and canvas to perform a panning effect/.
			Canvas_grid.Viewport = new Rect(Canvas_grid.Viewport.X + MPos.X, Canvas_grid.Viewport.Y + MPos.Y,
				Canvas_grid.Viewport.Width, Canvas_grid.Viewport.Height);

			//Moves ONLY the selection rectangle in the full map viewer.
			foreach (UIElement child in FullMapLEditor_Canvas.Children)
			{
				Console.WriteLine("Moved in bounds");
				if (((Rectangle)child) == FullMapSelection_Rect)
				{
					double x = Canvas.GetLeft(child);
					double y = Canvas.GetTop(child);

					Canvas.SetLeft(child, x - MPos.X / 10); //TODO: give this an if statment so the grid cannot go off the screen.
					Canvas.SetTop(child, y - MPos.Y / 10); //SCALE use the ratio to pan and link both accurately

				}
			}

			LEGridOffset.X -= MPos.X / 10; //keeps in sync
			LEGridOffset.Y -= MPos.Y / 10;
		}

		#endregion

		#region "Zooming"

		/// <summary>
		/// handles mouse scroll events
		/// Zooming is handled here.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditor_Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Console.WriteLine("scroollll");


			if (e.Delta > 0) //zoom in!
			{
				LEZoomLevel += .2;
				Canvas_grid.Transform = new ScaleTransform(LEZoomLevel, LEZoomLevel);
				LevelEditor_Canvas.RenderTransform = new ScaleTransform(LEZoomLevel, LEZoomLevel);
				Console.WriteLine(String.Format("W:{0},  H{1}", LEGridWidth, LEGridHeight));
				//TODO: resize selection rectangle
			}
			else //zoom out!
			{
				LEZoomLevel -= .2;
				Canvas_grid.Transform = new ScaleTransform(LEZoomLevel, LEZoomLevel);
				LevelEditor_Canvas.RenderTransform = new ScaleTransform(LEZoomLevel, LEZoomLevel);
				Console.WriteLine(String.Format("W:{0},  H{1}", LEGridWidth, LEGridHeight));
				//TODO: resize selection rectangle
			}

			if (LEZoomLevel < .2)
			{
				LEZoomLevel = .2;
				Canvas_grid.Transform = new ScaleTransform(LEZoomLevel, LEZoomLevel);
				LevelEditor_Canvas.RenderTransform = new ScaleTransform(LEZoomLevel, LEZoomLevel);
				return;
			} //do not allow this be 0 which in turn / by 0;

			ZoomFactor_TB.Text = String.Format("({0})%  ({1}x{1})", 100 * LEZoomLevel, LEGridWidth * LEZoomLevel);
			ScaleFullMapEditor();
		}

		#endregion

		#endregion

		#region "Full Level Canvas"

		private void ScaleFullMapEditor()
		{
			Level TempLevel = CurrentLevel;
			if (TempLevel == null) return; //TODO: Remove after i make force select work on tree view.
			FullMapLEditor_Canvas.Width = (int)TempLevel.GetPropertyData("xCells") * 10;
			FullMapLEditor_Canvas.Height = (int)TempLevel.GetPropertyData("yCells") * 10;

			FullMapLEditor_VB.Viewport = new Rect(0, 0, 10, 10);

			int MainCurCellsX = (int)Math.Ceiling(LevelEditor_BackCanvas.RenderSize.Width / (40 * LEZoomLevel));
			int MainCurCellsY = (int)Math.Ceiling(LevelEditor_BackCanvas.RenderSize.Height / (40 * LEZoomLevel));

			double pastx, pasty = 0;
			pastx = Canvas.GetLeft(FullMapSelection_Rect);
			pasty = Canvas.GetTop(FullMapSelection_Rect);

			FullMapLEditor_Canvas.Children.Remove(FullMapSelection_Rect);
			FullMapSelection_Rect = new Rectangle()
			{
				Width = MainCurCellsX * 10,
				Height = MainCurCellsY * 10,
				Stroke = Brushes.White,
				StrokeThickness = 1,
				Name = "SelectionRect"
			};
			Canvas.SetLeft(FullMapSelection_Rect, pastx);
			Canvas.SetTop(FullMapSelection_Rect, pasty);
			Canvas.SetZIndex(FullMapSelection_Rect, 100); //100 is the selection layer.

			FullMapLEditor_Canvas.Children.Add(FullMapSelection_Rect);
		}

		//quick move of the level editor canvas. when you click the screen renders that section.
		private void EditorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{

			//LevelEditor_Canvas.se
		}


		/// <summary>
		/// paint the full map editor for TILES
		/// </summary>
		/// <param name="p"></param>
		/// <param name="zindex"></param>
		private void FullMapEditorFill(Point p, int zindex)
		{
			SpriteLayer curlayer = (SpriteLayer)SceneExplorer_TreeView.SelectedValue;
			curlayer.AddToLayer(
				((int)p.Y + (int)Math.Abs(Canvas_grid.Viewport.Y)) / LEGridHeight, //current row
				((int)p.X + (int)Math.Abs(Canvas_grid.Viewport.X)) / LEGridWidth, // current column
				int.Parse(((Rectangle)SelectedTile_Canvas.Children[0]).Tag.ToString())); //the value of data 

			Rectangle r = new Rectangle() { Width = 10, Height = 10, Fill = Imgtilebrush };

			int setX = (10 * (((int)p.X / LEGridWidth)));
			int setY = (10 * (((int)p.Y / LEGridHeight)));

			Canvas.SetLeft(r, setX);
			Canvas.SetTop(r, setY);
			Canvas.SetZIndex(r, zindex);
			FullMapLEditor_Canvas.Children.Add(r);
		}

		/// <summary>
		/// Paint the full map editor for SPRITES
		/// </summary>
		/// <param name="spr"></param>
		/// <param name="i"></param>
		/// <param name="zindex"></param>
		private void FullMapEditorFill(Sprite spr, ImageBrush i, int zindex)
		{
			Rectangle r = new Rectangle() { Width = 10, Height = 10, Fill = i };
			Canvas.SetLeft(r, (int)spr.GetPropertyData("x") / 4);
			Canvas.SetTop(r, (int)spr.GetPropertyData("y") / 4);
			Canvas.SetZIndex(r, zindex); // divide 4 because scaling.
			FullMapLEditor_Canvas.Children.Add(r);
		}

		#region "Property Hot Reloading"

		#endregion

		#endregion

		#region "Tools"

		/// <summary>
		/// Set the Level editor to selection AND deselect.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditorSelection_Click(object sender, RoutedEventArgs e)
		{
			if (CurrentTool != EditorTool.Select)
			{
				CurrentTool = EditorTool.Select;
				Deselect();
				if (SceneExplorer_TreeView.SelectedItem is SpriteLayer &&
						((SpriteLayer)SceneExplorer_TreeView.SelectedItem).layerType == LayerType.Sprite)
					SetSpriteHitState(true);
			}

			if (SceneExplorer_TreeView.SelectedItem is SpriteLayer &&
					((SpriteLayer)SceneExplorer_TreeView.SelectedItem).layerType == LayerType.Sprite)
				SetSpriteHitState(true);
		}

		/// <summary>
		/// Set the Level editor to brush
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditorBrush_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Brush;
		}

		/// <summary>
		/// Takes a given selected area and fills it with the selected paint brush tile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Fill_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedTile_Canvas.Children.Count == 0) return;

			CurrentTool = EditorTool.Fill;
			int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
			int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
			int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

			columns /= relgridsize;
			rows /= relgridsize;

			Point begginning = GetGridSnapCords(SelectionRectPoints[0]);
			for (int i = 0; i < columns; i++)
			{
				for (int j = 0; j < rows; j++)
				{
					if ((int)CurrentLevel.GetPropertyData("xCells") - 1 <= i ||
							(int)CurrentLevel.GetPropertyData("yCells") - 1 <= j)
						break;

					Point p = new Point(begginning.X + (int)(40 * i), begginning.Y + (int)(40 * j));
					int iii = GetTileZIndex(SceneExplorer_TreeView);
					Rectangle r = new Rectangle()
					{ Width = 40, Height = 40, Fill = Imgtilebrush }; //create the tile that we wish to add to the grid.
					r.MouseLeftButtonUp += LevelEditor_BackCanvas_MouseLeftButtonUp;
					SpriteLayer curlayer = (SpriteLayer)SceneExplorer_TreeView.SelectedValue;
					curlayer.AddToLayer(((int)p.Y + (int)Math.Abs(Canvas_grid.Viewport.Y)) / LEGridHeight,
						((int)p.X + (int)Math.Abs(Canvas_grid.Viewport.X)) / LEGridWidth,
						int.Parse(((Rectangle)SelectedTile_Canvas.Children[0]).Tag.ToString()));

					Canvas.SetLeft(r, (int)p.X);
					Canvas.SetTop(r, (int)p.Y);
					Canvas.SetZIndex(r, iii); //place the tile position wise
					LevelEditor_Canvas.Children.Add(r); //actual place it on the canvas
					FullMapEditorFill(p, iii); //update the fullmap display to reflect this change
				}
			}

			Deselect();
		}

		/// <summary>
		/// Given a selection move these tiles down a layer. If it's the first layer don't move
		/// ALSO only move to a layer with the same layer type.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DownLaverLevelEditor_BTN_Click(object sender, RoutedEventArgs e)
		{
			//we need to make sure that we have selected a sprite layer in the tree view.
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;

			if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
			{
				if (selectTool.SelectedTiles.Count == 0) return; //Do we have a selected area?
																												 //is there a layer above the current to transfer the data to?
																												 //we need to get the current Sprite layer that is currently clicked.
				int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
				if (curLayer - 1 < 0) return;

				//ONLY move this data to a layer with the same Layer type
				int deslayer = -1;
				for (int i = curLayer - 1; i >= 0; i--)
				{
					if (CurrentLevel.Layers[curLayer].layerType == CurrentLevel.Layers[i].layerType)
					{
						deslayer = i;
						break;
					} // we found a matching layer
				}

				if (deslayer == -1) return; //no layer found

				//from here all the prereqs are fulfilled so we can apply the up layer VISUALLY 
				for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
				{
					Canvas.SetZIndex(selectTool.SelectedTiles[i], deslayer);
				}

				//Change the data for the level objects 
				int absrow = 0;
				int abscol = 0;
				Point p = GetGridSnapCords(Mouse.GetPosition(LevelEditor_BackCanvas));
				int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
				int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
				int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

				columns /= relgridsize;
				rows /= relgridsize;

				Point begginning = RelativeGridSnap(SelectionRectPoints[0]);
				begginning.X = (int)begginning.X;
				begginning.Y = (int)begginning.Y;
				for (int i = 0; i < columns; i++)
				{
					for (int j = 0; j < rows; j++)
					{
						absrow = ((int)begginning.Y + (relgridsize * j) + (int)Math.Abs(Canvas_grid.Viewport.Y)) / LEGridHeight;
						abscol = ((int)begginning.X + (relgridsize * i) + (int)Math.Abs(Canvas_grid.Viewport.X)) / LEGridWidth;
						CurrentLevel.ChangeLayer(absrow, abscol, curLayer, deslayer, LayerType.Tile);
					}
				}
			}
			else if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Sprite)
			{

			}
			else if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.GameEvent)
			{

			}
		}

		/// <summary>
		/// Given a selection move these tiles up a layer. If it's the last layer don't move
		/// ALSO only move to a layer with the same layer type. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpLaverLevelEditor_BTN_Click(object sender, RoutedEventArgs e)
		{
			//we need to make sure that we have selected a sprite layer in the tree view.
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;
			if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
			{
				if (selectTool.SelectedTiles.Count == 0) return; //Do we have a selected area?
				int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
				if (CurrentLevel.Layers.Count - 1 == curLayer) return; //last layer.

				//ONLY move this data to a layer with the same Layer type
				int deslayer = -1;
				for (int i = curLayer + 1; i < CurrentLevel.Layers.Count; i++)
				{
					if (CurrentLevel.Layers[curLayer].layerType == CurrentLevel.Layers[i].layerType)
					{
						deslayer = i;
						break;
					} // we found a matching layer
				}

				if (deslayer == -1) return; //no layer found

				//from here all the prereqs are fulfilled so we can apply the up layer VISUALLY 
				for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
				{
					Canvas.SetZIndex(selectTool.SelectedTiles[i], deslayer);
				}

				//Change the data for the level objects 
				int absrow = 0;
				int abscol = 0;
				Point p = GetGridSnapCords(Mouse.GetPosition(LevelEditor_BackCanvas));
				int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
				int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
				int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

				columns /= relgridsize;
				rows /= relgridsize;

				Point begginning = RelativeGridSnap(SelectionRectPoints[0]);
				begginning.X = (int)begginning.X;
				begginning.Y = (int)begginning.Y;
				for (int i = 0; i < columns; i++)
				{
					for (int j = 0; j < rows; j++)
					{
						absrow = ((int)begginning.Y + (relgridsize * j) + (int)Math.Abs(Canvas_grid.Viewport.Y)) / LEGridHeight;
						abscol = ((int)begginning.X + (relgridsize * i) + (int)Math.Abs(Canvas_grid.Viewport.X)) / LEGridWidth;
						CurrentLevel.ChangeLayer(absrow, abscol, curLayer, deslayer, LayerType.Tile);
					}
				}
			}
			else if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Sprite)
			{

			}
			else if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.GameEvent)
			{

			}
		}

		/// <summary>
		/// Sets the tool to eraser.
		/// Also if there is a selection, erase it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditorEraser_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Eraser;
			if (selectTool.SelectedTiles.Count != 0)
				EraseSelection();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditorMove_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Move;
		}

		/// <summary>
		/// Saves the level file.
		/// Level data, tile sets, and editor info.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveLevel_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
			{
				Title = "Save Level File",
				FileName = "", //default file name
				Filter = "txt files (*.lvl)|*.lvl|All files (*.*)|*.*",
				FilterIndex = 2,
				InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Levels"),
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

			List<Tuple<int, int>> celldim = new List<Tuple<int, int>>();

			//layer moving test.
			List<String> TileSetImages = new List<string>();
			foreach (Tuple<String, String, int, int> tilesetstuple in CurrentLevel.TileSet)
			{
				TileSetImages.Add(tilesetstuple.Item2); //URI isn't supported so turn it local!
				celldim.Add(new Tuple<int, int>(tilesetstuple.Item3, tilesetstuple.Item4));

			}

			CurrentLevel.ExportLevel(dlg.FileName + (dlg.FileName.Contains(".lvl") ? "" : ".lvl"), TileSetImages, EditorProjectContentDirectory, celldim);
		}

		/// <summary>
		/// Takes an existing level file and imports/ opens it in the editor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void OpenLevel_MenuItem_ClickAsync(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "New Level File",
				FileName = "", //default file name
				InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Levels"),
				Filter = "txt files (*.xml)|*.xml|All files (*.*)|*.*",
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

			await ImportLevelAsync(dlg.FileName);

		}

		/// <summary>
		/// Sets the editor window to allow the user to create a new Level
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewLevel_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			NewLevelData_CC.Visibility = Visibility.Visible;
			LevelEditor_BackCanvas.Visibility = Visibility.Hidden;
			LevelEditorStatusBar_Grid.Visibility = Visibility.Hidden;
		}

		/// <summary>
		/// Deselects all current selected tiles.
		/// </summary>
		private void Deselect()
		{
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;
			if (((SpriteLayer)(SceneExplorer_TreeView.SelectedValue)).layerType == LayerType.Sprite)
			{
				foreach (UIElement fe in LevelEditor_Canvas.Children)
				{
					if (fe is ContentControl)
					{
						Selector.SetIsSelected(fe, false);
					}
				}
			}
			else
			{
				//delete the selection area display. Its on zindex 100.
				List<UIElement> ue = new List<UIElement>();
				foreach (UIElement fe in LevelEditor_Canvas.Children)
				{
					int z = Canvas.GetZIndex(fe);
					Console.WriteLine(z);
					if (z == 100)
					{
						ue.Add(fe);
					}
				}

				foreach (UIElement ueee in ue)
				{
					LevelEditor_Canvas.Children.Remove(ueee);
				}

				ue.Clear();
				selectTool.SelectedTiles.Clear();
			}

		}

		/// <summary>
		/// Erases all data in a selection.
		/// </summary>
		private void EraseSelection()
		{
			Point pos = Mouse.GetPosition(LevelEditor_BackCanvas);
			int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
			int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
			int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
			int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

			columns /= relgridsize;
			rows /= relgridsize;

			Point begginning = GetGridSnapCords(SelectionRectPoints[0]);
			for (int i = 0; i < columns; i++)
			{
				for (int j = 0; j < rows; j++)
				{
					if ((int)CurrentLevel.GetPropertyData("xCells") - 1 <= i ||
							(int)CurrentLevel.GetPropertyData("yCells") - 1 <= j)
						break;
					//find the tile on the layer that is selected.
					Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas,
						LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(),
						curLayer, (int)begginning.X + (i * 40), (int)begginning.Y + (j * 40),
						(int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
					if (rr == null) continue; //if you click on a empty rect return
																		//find the rect in the current layers displayed tiles
					int k = LevelEditor_Canvas.Children.IndexOf(rr);
					int x = -1;
					int y = -1;
					if (k >= 0)
					{
						y = (int)Canvas.GetLeft(rr) / 40;
						x = (int)Canvas.GetTop(rr) / 40;
						LevelEditor_Canvas.Children.RemoveAt(k); //delete it.
					}

					if (x < 0 || y < 0)
					{
						Console.WriteLine("desired cell to delete DNE");
						continue;
					}

					//find the data in the level objects sprite layer. And then clear it.
					SpriteLayer curlayer = (SpriteLayer)SceneExplorer_TreeView.SelectedValue;
					curlayer.DeleteFromLayer(x, y);

				}
			}

			Deselect();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditorImage_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Image;
		}

		/// <summary>
		/// Opens up the game event window to allow users to create new game events or edit current ones
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddGameEvent_Click(object sender, RoutedEventArgs e)
		{
			if (CurrentLevel == null) return;
			GameEventsSettings ff = new GameEventsSettings(ref CurrentLevel, ProjectFilePath, 1);
			ff.Show();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditGameEvents_Click(object sender, RoutedEventArgs e)
		{
			if (CurrentLevel == null) return;
			GameEventsSettings ff = new GameEventsSettings(ref CurrentLevel, ProjectFilePath, 0);
			ff.Show();
		}

		/// <summary>
		/// Chooses a currently existing game event, and sets the ref so the user can paint the level with game events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeclareGameEvent_MI_Click(object sender, RoutedEventArgs e)
		{
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;
			if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.GameEvent)
			{
				//clicked on the event
				MenuItem MI = ((MenuItem)(EditorToolBar_CC.Template.FindName("DeclareGameEvent_MI", EditorToolBar_CC)));
				//TODO: This needs to have the offset. Since there can be multiple GameEventLayers. similar to how the tile map has an offset.
				GameEventNum = Int32.Parse(((MenuItem)sender).Tag.ToString());
				Console.WriteLine(GameEventNum);
				CurrentTool = EditorTool.Gameevent;
			}
		}

		/// <summary>
		/// This will auto load all the game events that all allowed to be painted to the context menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;
			if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.GameEvent)
			{
				//we are clicked on a gameevent layer!
				List<GameEvent> layergameevents =
					((Tuple<int[,], List<GameEvent>>)((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerObjects).Item2;
				MenuItem MI = ((MenuItem)(EditorToolBar_CC.Template.FindName("DeclareGameEvent_MI", EditorToolBar_CC)));
				MI.Items.Clear();
				int i = 0;
				foreach (GameEvent ge in layergameevents)
				{
					MI.Items.Add(new MenuItem() { Header = ge.EventName, Tag = i });
					((MenuItem)MI.Items[MI.Items.Count - 1]).Click += DeclareGameEvent_MI_Click;
					i++;
				}

			}
		}

		/// <summary>
		/// deselect all current selected data.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditorDeselect_Click(object sender, RoutedEventArgs e)
		{
			Deselect();
		}

		#endregion

		/// <summary>
		/// There are 2 different tool bar options for the level editor. Sprite, and Tile.
		/// This method will set the correct one 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditorLibary_TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabControl LELibary_TC =
				(TabControl)ContentLibrary_Control.Template.FindName("LevelEditorLibary_TabControl", ContentLibrary_Control);
			if (LELibary_TC.SelectedIndex == 0)
			{
				EditorToolBar_CC.Template = (ControlTemplate)this.TryFindResource("LevelEditorTileMapToolBar_Template");

			}
			else if (LELibary_TC.SelectedIndex == 1)
			{
				EditorToolBar_CC.Template = (ControlTemplate)this.TryFindResource("LevelEditorSpriteToolBar_Template");
			}
			else if (LELibary_TC.SelectedIndex == 2)
			{
				EditorToolBar_CC.Template = null;
			}
		}

		/// <summary>
		/// This method is activated when the user selects a different object in the scene explorer.
		/// It will print out properties to the properties grid, and change displayed level if needed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SceneExplorerLevel_TreeView_SelectedItemChanged(object sender,
			RoutedPropertyChangedEventArgs<object> e)
		{
			Console.WriteLine("Changed Scene Object");
			if (e.NewValue is Level)
			{
				//set the current level PTR
				CurrentLevel = (Level)e.NewValue;

				TileSets_CB.Items.Clear(); //remove the past data.
				foreach (Tuple<String, String, int, int> tilesetTuples in ((Level)e.NewValue).TileSet)
				{
					TileSets_CB.Items.Add(tilesetTuples.Item1);
					//CreateTileMap(tilesetTuples.SpriteSheetName, tilesetTuples.Item3, tilesetTuples.Item4); //fill in the new data.

					String tileSetPath = tilesetTuples.Item2.Replace("{Content}", EditorProjectContentDirectory);
					Image image = new Image();
					var pic = new System.Windows.Media.Imaging.BitmapImage();
					pic.BeginInit();
					pic.UriSource = new Uri(tileSetPath); // url is from the xml
					pic.EndInit();

					System.Drawing.Image img = System.Drawing.Image.FromFile(tileSetPath);
					image.Source = pic;
					image.Width = img.Width;
					image.Height = img.Height;
					//Interaction interaction

					int len = pic.UriSource.ToString().LastIndexOf('.') -
										pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' });
					String Name = pic.UriSource.ToString()
						.Substring(pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' }) + 1, len - 1);

					TileSets_CB.SelectedIndex = 0;
				}

				int i = 0;
				PropGrid LB =
					((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
				LB.ClearProperties();
				foreach (object o in ((Level)e.NewValue).GetProperties().Select(m => m.Item2))
				{
					if (o is String || o is int)
					{
						LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(),
							((Level)e.NewValue).PropertyTBCallback);
					}
					else if (o is bool)
					{
						LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new CheckBox(), (bool)o,
							((Level)e.NewValue).PropertyCheckBoxCallback);
					}

					i++;
				}

				foreach (ContentControl cc in LevelEditor_Canvas.Children.OfType<ContentControl>().ToList())
				{
					cc.IsHitTestVisible = false;
				}

				LevelEditor_Canvas.Children.Clear();
				FullMapLEditor_Canvas.Children.Clear();
				RedrawLevel(CurrentLevel);
				DeselectSprites();
			}

			if (e.NewValue is SpriteLayer)
			{
				int i = 0;
				PropGrid LB =
					((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
				SpriteLayer SL = (SpriteLayer)e.NewValue;
				LB.ClearProperties();
				foreach (object o in SL.GetProperties().Select(m => m.Item2))
				{
					if (o is String || o is int)
					{
						LB.AddProperty(SL.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(),
							SL.PropertyTBCallback);
					}
					else if (o is bool)
					{
						LB.AddProperty(SL.GetProperties().Select(m => m.Item1).ToList()[i], new CheckBox(), (bool)o,
							SL.PropertyCheckBoxCallback);
					}

					i++;
				}

				if (SL.layerType == LayerType.Sprite)
					SetSpriteHitState(false);
				else
				{
					SetSpriteHitState(false);
					DeselectSprites();
				}


				//ListBox LB = ((ListBox)(ObjectProperties_Control.Template.FindName("LEditProperty_LB", ObjectProperties_Control)));
				//LB.ItemsSource = null;
				//LB.ItemsSource = LEditorTS;
			}
		}

		private void DeselectSprites(bool b = false)
		{
			foreach (ContentControl cc in LevelEditor_Canvas.Children.OfType<ContentControl>().ToList())
			{
				Selector.SetIsSelected(cc, b);
			}
		}

		private void SetSpriteHitState(bool b)
		{
			foreach (ContentControl cc in LevelEditor_Canvas.Children.OfType<ContentControl>().ToList())
			{
				cc.IsHitTestVisible = b;
			}
		}

		private void RedrawLevel(Level LevelToDraw)
		{
			LevelEditor_Canvas.Children.Clear(); // CLEAR EVERYTHING!

			//redraw each layer of the new level!
			foreach (SpriteLayer layer in CurrentLevel.Layers)
			{
				int Zindex = CurrentLevel.Layers.IndexOf(layer);
				if (layer.layerType == LayerType.Tile)
				{
					int[,] tilemap = (int[,])layer.LayerObjects; //tile map.
					List<int> TileMapThresholds = new List<int>();
					//find out the thresholds per tile map.
					foreach (Tuple<String, String, int, int> tilesetstuple in CurrentLevel.TileSet)
					{
						String tileSetPath = tilesetstuple.Item2;
						tileSetPath = tileSetPath.Replace("{Content}", EditorProjectContentDirectory);
						if (!System.IO.File.Exists(tileSetPath))
						{
							EngineOutputLog.AddErrorLogItem(-1, String.Format("Tile Set image doesn't exist: {0}", tileSetPath), "Level Editor", false);
							return;
						}

						//find out the next tile data number using the tuple
						//this is wrong. It needs to be (imgwidth / tilewidth) * (imgHeight / tileHieght)
						System.Drawing.Image imgTemp = System.Drawing.Image.FromFile(tileSetPath);
						TileMapThresholds.Add((TileMapThresholds.Count == 0
							? (imgTemp.Width / tilesetstuple.Item3) * (imgTemp.Height / tilesetstuple.Item4)
							: (int)TileMapThresholds.Last() +
								(imgTemp.Width / tilesetstuple.Item3) * (imgTemp.Height / tilesetstuple.Item4)));
					}

					TileMapThresholds.Insert(0, 0);
					//scan through the 2D array of int data
					for (int i = 0; i < tilemap.GetLength(0); i++) //rows
					{
						for (int j = 0; j < tilemap.GetLength(1); j++) //columns
						{
							//retrieve the tileset value, position to crop.
							int CurTileData = (int)((int[,])layer.LayerObjects).GetValue(i, j);
							if (CurTileData == -1) continue; //this indicates no tile data, so ignore.
							int TilesetInc = 1;
							String Tilesetpath = CurrentLevel.TileSet[0].Item2; //init the first set

							//using the tile data determine the tileset that is being used. offset wise 
							while (CurTileData > TileMapThresholds[TilesetInc]) //if cur is greater than move to the next tileset.
							{
								Tilesetpath = CurrentLevel.TileSet[TilesetInc].Item2;
								
								TilesetInc++;
							}

							Tilesetpath = Tilesetpath.Replace("{Content}", EditorProjectContentDirectory);
							if (!System.IO.File.Exists(Tilesetpath))
							{
								EngineOutputLog.AddErrorLogItem(-1, String.Format("Tile Set image doesn't exist: {0}", Tilesetpath), "Level Editor", false);
								return;
							}

							//create/get the tilebrush of the current tile.
							Imgtilebrush = null;

							var pic = new System.Windows.Media.Imaging.BitmapImage();
							pic.BeginInit();
							pic.UriSource = new Uri(Tilesetpath); // url is from the xml
							pic.EndInit();

							int rowtilemappos;
							int coltilemappos;
							if (TilesetInc - 1 == 0)
							{
								rowtilemappos = (int)CurTileData / (pic.PixelWidth / CurrentLevel.TileSet[TilesetInc - 1].Item3);
								coltilemappos = (int)CurTileData % (pic.PixelHeight / CurrentLevel.TileSet[TilesetInc - 1].Item4);
							}
							else
							{
								rowtilemappos = (CurTileData - TileMapThresholds[TilesetInc - 1]) /
																(pic.PixelWidth / CurrentLevel.TileSet[TilesetInc - 1].Item3);
								coltilemappos = (CurTileData - TileMapThresholds[TilesetInc - 1]) %
																(pic.PixelHeight / CurrentLevel.TileSet[TilesetInc - 1].Item4);
							}

							//crop based on the current 
							CroppedBitmap crop = new CroppedBitmap(pic,
								new Int32Rect(coltilemappos * CurrentLevel.TileSet[TilesetInc - 1].Item3,
									rowtilemappos * CurrentLevel.TileSet[TilesetInc - 1].Item4,
									CurrentLevel.TileSet[TilesetInc - 1].Item3,
									CurrentLevel.TileSet[TilesetInc - 1].Item4));
							Image TileBrushImage = new Image
							{
								Source = crop //cropped
							};

							Imgtilebrush = new ImageBrush(TileBrushImage.Source);

							Rectangle ToPaint = new Rectangle()
							{
								Width = 40,
								Height = 40,
								Fill = new ImageBrush(TileBrushImage.Source)
							};

							Canvas.SetLeft(ToPaint, j * 40);
							Canvas.SetTop(ToPaint, i * 40);
							Canvas.SetZIndex(ToPaint, Zindex);

							LevelEditor_Canvas.Children.Add(ToPaint);

							Rectangle r = new Rectangle() { Width = 10, Height = 10, Fill = Imgtilebrush };

							Canvas.SetLeft(r, j * 10);
							Canvas.SetTop(r, i * 10);
							Canvas.SetZIndex(r, Zindex);
							FullMapLEditor_Canvas.Children.Add(r);
							//clear memory
							ToPaint = null;
							crop.Source = null;
							pic = null;
							crop = null;
							TileBrushImage = null;
							ToPaint = null;
							//paint the current tile with said brush
						}

						if (i % 50 == 0)
						{
							//GC.Collect();
						}

						Console.WriteLine(i);
					}
				}
				else if (layer.layerType == LayerType.Sprite)
				{
					//the current layer is a spritelayer which contains a list of sprite objects
					foreach (Sprite sprite in ((List<Sprite>)layer.LayerObjects))
					{
						String spritePath = sprite.ImgPathLocation.Replace("{Content}", EditorProjectContentDirectory);
						if (!System.IO.File.Exists(spritePath))
						{
							EngineOutputLog.AddErrorLogItem(-1, String.Format("Tile Set image doesn't exist: {0}", spritePath), "Level Editor", false);
							return;
						}


						BitmapImage bitmap = new BitmapImage(new Uri(spritePath, UriKind.Absolute));
						Image img = new Image
						{
							Source = bitmap
						};
						Rectangle r = new Rectangle()
						{
							Width = (int)sprite.GetPropertyData("width"),
							Height = (int)sprite.GetPropertyData("height"),
							Fill = new ImageBrush(img.Source)
						}; //Make a rectange the size of the image

						ContentControl CC = ((ContentControl)this.TryFindResource("MoveableImages_Template"));
						CC.Width = (int)sprite.GetPropertyData("width");
						CC.Height = (int)sprite.GetPropertyData("height");

						Canvas.SetLeft(CC, (int)sprite.GetPropertyData("x"));
						Canvas.SetTop(CC, (int)sprite.GetPropertyData("y"));
						Canvas.SetZIndex(CC, Zindex);
						Selector.SetIsSelected(CC, false);
						CC.MouseRightButtonDown += ContentControl_MouseLeftButtonDown;
						((Rectangle)CC.Content).Fill = new ImageBrush(img.Source);
						LevelEditor_Canvas.Children.Add(CC);
					}
				}
				else if (layer.layerType == LayerType.GameEvent)
				{
					Console.WriteLine("Gameevent");

					int[,] griddata = ((Tuple<int[,], List<GameEvent>>)layer.LayerObjects).Item1;

					//scan through the 2D array of gameevent locations
					for (int i = 0; i < griddata.GetLength(0); i++) //rows
					{
						for (int j = 0; j < griddata.GetLength(1); j++) //columns
						{
							if (griddata[i, j] != 0)
							{
								TextBlock tb = new TextBlock()
								{
									HorizontalAlignment = HorizontalAlignment.Center,
									VerticalAlignment = VerticalAlignment.Center,
									TextAlignment = TextAlignment.Center,
									TextWrapping = TextWrapping.Wrap,
									Width = 40,
									Height = 40,
									FontSize = 18,
									Text = (griddata[i, j] == -1 ? "" : griddata[i, j].ToString()),
									Tag = griddata[i, j].ToString(),
									Foreground = new SolidColorBrush(Colors.Black),
									Background = (griddata[i, j] != -1
										? new SolidColorBrush(Color.FromArgb(100, 100, 100, 100))
										: new SolidColorBrush(Color.FromArgb(100, 255, 0, 0))),
								};
								Border b = new Border() { Width = 40, Height = 40 };
								b.Child = tb;

								Canvas.SetLeft(b, j * 40);
								Canvas.SetTop(b, i * 40);
								Canvas.SetZIndex(b, Zindex); //place the tile position wise
								LevelEditor_Canvas.Children.Add(b); //actual place it on the canvas

							}
						}
					}
				}

				Zindex++;
			}
		}

		public void ImportLevel(String filename)
		{
			if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level"))
			{
				CurrentLevel = ((Level.ImportLevel(filename)));
				OpenLevels.Add(CurrentLevel);
				//change focus:
				SceneExplorer_TreeView.ItemsSource = OpenLevels;
				SceneExplorer_TreeView.Items.Refresh();
				SceneExplorer_TreeView.UpdateLayout();

				NewLevelData_CC.Visibility = Visibility.Hidden;
				LevelEditor_BackCanvas.Visibility = Visibility.Visible;
				LevelEditorStatusBar_Grid.Visibility = Visibility.Visible;
				((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control))
					.IsEnabled = true;
				LevelEditor_Canvas.IsEnabled = true;
				ContentLibaryImport_BTN.IsEnabled = true;
				TileSets_CB.Items.Clear(); //remove the past data.

				//tilemaps
				foreach (Tuple<String, String, int, int> tilesetTuples in CurrentLevel.TileSet)
				{

					Image image = new Image();
					var pic = new System.Windows.Media.Imaging.BitmapImage();
					pic.BeginInit();
					pic.UriSource = new Uri(tilesetTuples.Item2); // url is from the xml
					pic.EndInit();

					System.Drawing.Image img = System.Drawing.Image.FromFile(tilesetTuples.Item2);
					image.Source = pic;
					image.Width = img.Width;
					image.Height = img.Height;

					int len = pic.UriSource.ToString().LastIndexOf('.') -
										pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' });
					String Name = pic.UriSource.ToString()
						.Substring(pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' }) + 1, len - 1);

					TileSets_CB.Items.Add(Name);

					//CreateTileMap(tilesetTuples.SpriteSheetName, tilesetTuples.Item3, tilesetTuples.Item4);
				}

				foreach (Tuple<string, string> t in CurrentLevel.sprites)
				{
					String imagePath = t.Item2.Replace("{Content}", EditorProjectContentDirectory);

					LESpriteObjectList.Add(new EditorObject(t.Item2, imagePath,  t.Item1, false));
				}

				ListBox SpriteLibary_LB =
					(ListBox)ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control);
				SpriteLibary_LB.ItemsSource = null;
				SpriteLibary_LB.ItemsSource = LESpriteObjectList;

				TileSets_CB.SelectedIndex = 0;
				//draw the level
				RedrawLevel(CurrentLevel);

				//set visabilty. 
				Grid Prob_Grid =
					(Grid)ContentLibrary_Control.Template.FindName("TileSetProperties_Grid", ContentLibrary_Control);
				Prob_Grid.Visibility = Visibility.Hidden;
				((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control))
					.Visibility = Visibility.Visible;
				((ComboBox)ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control)).Visibility =
					Visibility.Visible;
				((Label)ContentLibrary_Control.Template.FindName("TileSet_LBL", ContentLibrary_Control)).Visibility =
					Visibility.Visible;


				int i = 0;
				PropGrid LB =
					((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
				foreach (object o in CurrentLevel.GetProperties().Select(m => m.Item2))
				{
					if (o is String || o is int)
					{
						LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(),
							CurrentLevel.PropertyTBCallback);
					}
					else if (o is bool)
					{
						LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new CheckBox(), (bool)o,
							CurrentLevel.PropertyCheckBoxCallback);
					}

					i++;
				}
			}
			//CurrentLevel.setProperties(((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary);
			//((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary = CurrentLevel.getProperties();


		}

		public async System.Threading.Tasks.Task ImportLevelAsync(String filename)
		{
			CurrentLevel = ((await Level.ImportLevelAsync(filename)));
			OpenLevels.Add(CurrentLevel);
			//change focus:
			SceneExplorer_TreeView.ItemsSource = OpenLevels;
			SceneExplorer_TreeView.Items.Refresh();
			SceneExplorer_TreeView.UpdateLayout();

			NewLevelData_CC.Visibility = Visibility.Hidden;
			LevelEditor_BackCanvas.Visibility = Visibility.Visible;
			LevelEditorStatusBar_Grid.Visibility = Visibility.Visible;
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control))
				.IsEnabled = true;
			LevelEditor_Canvas.IsEnabled = true;
			ContentLibaryImport_BTN.IsEnabled = true;
			TileSets_CB.Items.Clear(); //remove the past data.

			//tilemaps
			foreach (Tuple<String, String, int, int> tilesetTuples in CurrentLevel.TileSet)
			{

				String tileSetPath = tilesetTuples.Item2;
				tileSetPath = tileSetPath.Replace("{Content}", EditorProjectContentDirectory);
				if (!System.IO.File.Exists(tileSetPath))
				{
					EngineOutputLog.AddErrorLogItem(-1, String.Format("Tile Set image doesn't exist: {0}", tileSetPath), "Level Editor", false);
					return;
				}

				Image image = new Image();
				var pic = new System.Windows.Media.Imaging.BitmapImage();
				pic.BeginInit();
				pic.UriSource = new Uri(tileSetPath); // url is from the xml
				pic.EndInit();

				System.Drawing.Image img = System.Drawing.Image.FromFile(tileSetPath);
				image.Source = pic;
				image.Width = img.Width;
				image.Height = img.Height;

				int len = pic.UriSource.ToString().LastIndexOf('.') -
									pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' });
				String Name = pic.UriSource.ToString()
					.Substring(pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' }) + 1, len - 1);

				TileSets_CB.Items.Add(Name);

				//CreateTileMap(tilesetTuples.SpriteSheetName, tilesetTuples.Item3, tilesetTuples.Item4);
			}

			TileSets_CB.SelectedIndex = 0;
			//draw the level
			RedrawLevel(CurrentLevel);

			int i = 0;
			PropGrid LB =
				((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
			foreach (object o in CurrentLevel.GetProperties().Select(m => m.Item2))
			{
				if (o is String || o is int)
				{
					LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(),
						CurrentLevel.PropertyTBCallback);
				}
				else if (o is bool)
				{
					LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new CheckBox(), (bool)o,
						CurrentLevel.PropertyCheckBoxCallback);
				}

				i++;
			}
			//CurrentLevel.setProperties(((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary);
			//((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary = CurrentLevel.getProperties();


			//set visabilty. 
			Grid Prob_Grid =
				(Grid)ContentLibrary_Control.Template.FindName("TileSetProperties_Grid", ContentLibrary_Control);
			Prob_Grid.Visibility = Visibility.Hidden;
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control))
				.Visibility = Visibility.Visible;
			((ComboBox)ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control)).Visibility =
				Visibility.Visible;
			((Label)ContentLibrary_Control.Template.FindName("TileSet_LBL", ContentLibrary_Control)).Visibility =
				Visibility.Visible;
		}

		private void CreateLevel(String LevelName, int XCellsVal, int YCellsVal)
		{
			//create new Level
			Level TempLevel = new Level(LevelName);
			SpriteLayer TempLevelChild = new SpriteLayer(LayerType.Tile, TempLevel) { LayerName = "Background" };
			TempLevelChild.DefineLayerDataType(LayerType.Tile, XCellsVal, YCellsVal);
			TempLevel.Layers.Add(TempLevelChild);
			TempLevelChild = new SpriteLayer(LayerType.GameEvent, TempLevel) { LayerName = "Collision" };
			TempLevelChild.DefineLayerDataType(LayerType.GameEvent, XCellsVal, YCellsVal);
			TempLevel.Layers.Add(TempLevelChild);
			TempLevelChild = new SpriteLayer(LayerType.Sprite, TempLevel) { LayerName = "Sprite" };
			TempLevelChild.DefineLayerDataType(LayerType.Sprite, XCellsVal, YCellsVal);
			TempLevel.Layers.Add(TempLevelChild);

			TempLevel.SetProperty("xCells", XCellsVal);
			TempLevel.SetProperty("yCells", YCellsVal);

			CurrentLevel = TempLevel;

			//change focus:
			OpenLevels.Add(TempLevel);
			SceneExplorer_TreeView.ItemsSource = OpenLevels;
			SceneExplorer_TreeView.Items.Refresh();
			SceneExplorer_TreeView.UpdateLayout();

			NewLevelData_CC.Visibility = Visibility.Hidden;
			LevelEditor_BackCanvas.Visibility = Visibility.Visible;
			LevelEditorStatusBar_Grid.Visibility = Visibility.Visible;
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control))
				.IsEnabled = true;
			LevelEditor_Canvas.IsEnabled = true;
			ContentLibaryImport_BTN.IsEnabled = true;

			int i = 0;
			PropGrid LB =
				((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
			foreach (object o in CurrentLevel.GetProperties().Select(m => m.Item2))
			{
				if (o is String || o is int)
				{
					LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(),
						CurrentLevel.PropertyTBCallback);
				}
				else if (o is bool)
				{
					LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new CheckBox(), (bool)o,
						CurrentLevel.PropertyCheckBoxCallback);
				}

				i++;
			}
			//CurrentLevel.setProperties(((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary);
			//((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary = CurrentLevel.getProperties();

			LevelEditor_Canvas.Children.Clear();
			FullMapLEditor_Canvas.Children.Clear();
		}

		#endregion // end of Level editor region


	}
}
