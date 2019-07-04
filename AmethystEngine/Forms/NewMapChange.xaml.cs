using BixBite;
using BixBite.Rendering;
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
	/// Interaction logic for NewMapChange.xaml
	/// </summary>
	public partial class NewMapChange : Window
	{

		Point MPos = new Point();
		Point GridOffset = new Point();
		private ImageBrush imgtilebrush;
		private Level CurrentLevel;
		GameEventsSettings GEForm;
		private int newx;
		private int newy;
		Rectangle selectrect = new Rectangle();
		bool bAdd = false;

		public NewMapChange(String LevelFile, GameEventsSettings GESettingsWindow, bool bAdd = true)
		{
			InitializeComponent();
			CurrentLevel = Level.ImportLevel(LevelFile);
			RedrawLevel(CurrentLevel);
			GEForm = GESettingsWindow;
			selectrect.Fill = new SolidColorBrush(Color.FromArgb(100, 0, 20, 100));
			selectrect.Width = 40; selectrect.Height = 40;

			this.bAdd = bAdd;
		}

		/// <summary>
		/// This method takes care of mouse movement events on the main level editor canvas.
		/// Panning is handled in here.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LevelEditor_BackCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			//we need to display the cords.
			Point p = Mouse.GetPosition(LevelEditor_BackCanvas);
			String point = String.Format("({0}, {1}) OFF:({2}, {3})", (int)p.X, (int)p.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);

			//which way is mouse moving?
			MPos -= (Vector)e.GetPosition(LevelEditor_Canvas);
			//is the middle mouse button down?
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				LevelEditorPan();
			}

			MPos = e.GetPosition(LevelEditor_Canvas); //set this for the iteration
		}

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
			{ //return the abs size. Base 40x40 grid.
				return new Point(relpoint.X + Xoff, relpoint.Y + YOff);//this gives us the cell number. Use this and multiply by the base value.
			}
			else //rel grid size
			{
				return new Point();
			}


		}

		private Point GetGridSnapCords(Point p)
		{
			int Xoff = (int)(Math.Abs(Canvas_grid.Viewport.X)) % 40; Xoff = 40 - Xoff; //offset
			int YOff = (int)(Math.Abs(Canvas_grid.Viewport.Y)) % 40; YOff = 40 - YOff;

			p.X -= Math.Floor(p.X - Xoff) % 40;  //TODO: Add the offset so we can fill the grid AFTER PAnNNG
			p.Y -= Math.Floor(p.Y - YOff) % 40;
			return p;
		}

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

			GridOffset.X -= MPos.X / 10; //keeps in sync
			GridOffset.Y -= MPos.Y / 10;
		}

		private void RedrawLevel(Level LevelToDraw)
		{
			LevelEditor_Canvas.Children.Clear(); // CLEAR EVERYTHING!

			//redraw each layer of the new level!
			foreach (BixBite.Rendering.SpriteLayer layer in CurrentLevel.Layers)
			{
				int Zindex = CurrentLevel.Layers.IndexOf(layer);
				if (layer.layerType == LayerType.Tile)
				{
					int[,] tilemap = (int[,])layer.LayerObjects; //tile map.
					List<int> TileMapThresholds = new List<int>();
					//find out the thresholds per tile map.
					foreach (Tuple<String, String, int, int> tilesetstuple in CurrentLevel.TileSet)
					{
						//find out the next tile data number using the tuple
						//this is wrong. It needs to be (imgwidth / tilewidth) * (imgHeight / tileHieght)
						System.Drawing.Image imgTemp = System.Drawing.Image.FromFile(tilesetstuple.Item2);
						TileMapThresholds.Add((TileMapThresholds.Count == 0 ? (imgTemp.Width / tilesetstuple.Item3) * (imgTemp.Height / tilesetstuple.Item4)
																	: (int)TileMapThresholds.Last() + (imgTemp.Width / tilesetstuple.Item3) * (imgTemp.Height / tilesetstuple.Item4)));
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

							//create/get the tilebrush of the current tile.
							imgtilebrush = null;

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
								rowtilemappos = (CurTileData - TileMapThresholds[TilesetInc - 1]) / (pic.PixelWidth / CurrentLevel.TileSet[TilesetInc - 1].Item3);
								coltilemappos = (CurTileData - TileMapThresholds[TilesetInc - 1]) % (pic.PixelHeight / CurrentLevel.TileSet[TilesetInc - 1].Item4);
							}

							//crop based on the current 
							CroppedBitmap crop = new CroppedBitmap(pic, new Int32Rect(coltilemappos * CurrentLevel.TileSet[TilesetInc - 1].Item3,
																															 rowtilemappos * CurrentLevel.TileSet[TilesetInc - 1].Item4,
																															 CurrentLevel.TileSet[TilesetInc - 1].Item3,
																															 CurrentLevel.TileSet[TilesetInc - 1].Item4));
							Image TileBrushImage = new Image
							{
								Source = crop //cropped
							};

							imgtilebrush = new ImageBrush(TileBrushImage.Source);

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
						BitmapImage bitmap = new BitmapImage(new Uri(sprite.ImgPathLocation, UriKind.Absolute));
						Image img = new Image(); img.Source = bitmap;
						Rectangle r = new Rectangle() { Width = (int)sprite.GetProperty("width"), Height = (int)sprite.GetProperty("height"), Fill = new ImageBrush(img.Source) };//Make a rectange the size of the image

						ContentControl CC = ((ContentControl)this.TryFindResource("MoveableImages_Template"));
						CC.Width = (int)sprite.GetProperty("width");
						CC.Height = (int)sprite.GetProperty("height");

						Canvas.SetLeft(CC, (int)sprite.GetProperty("x")); Canvas.SetTop(CC, (int)sprite.GetProperty("y")); Canvas.SetZIndex(CC, Zindex);
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
									Text = griddata[i, j].ToString(),
									Tag = griddata[i, j].ToString(),
									Foreground = new SolidColorBrush(Colors.Black),
									Background = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100)),
								};
								Border b = new Border() { Width = 40, Height = 40 };
								b.Child = tb;

								Canvas.SetLeft(b, j * 40); Canvas.SetTop(b, i * 40); Canvas.SetZIndex(b, Zindex); //place the tile position wise
								LevelEditor_Canvas.Children.Add(b); //actual place it on the canvas

							}
						}
					}
				}
				Zindex++;
			}
		}

		//sets the new position in the gameevents 
		private void SetNewPos_BTN_Click(object sender, RoutedEventArgs e)
		{
			GEForm.AddNewX = newx;
			GEForm.AddnewY = newy;
			if (bAdd)
			{
				GEForm.AddEventNewPosX_TB.Text = newx.ToString();
				GEForm.AddEventNewPosY_TB.Text = newy.ToString();
			}
			else
			{
				GEForm.EventNewPosX_TB.Text = newx.ToString();

				var key = Key.Enter;                    // Key to send
				var target = GEForm.EventNewPosX_TB;  // Target element
				var routedEvent = Keyboard.KeyDownEvent; // Event to send

				target.RaiseEvent(
					new KeyEventArgs(
						Keyboard.PrimaryDevice,
						PresentationSource.FromVisual(target),
						0,
						key)
					{ RoutedEvent = routedEvent }
				);

				GEForm.EventNewPosY_TB.Text = newy.ToString();
				target = GEForm.EventNewPosY_TB;  // Target element
				target.RaiseEvent(
					new KeyEventArgs(
						Keyboard.PrimaryDevice,
						PresentationSource.FromVisual(target),
						0,
						key)
					{ RoutedEvent = routedEvent }
				);

			}
			
			this.Close(); 
		}

		private void LevelEditor_BackCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point pos = Mouse.GetPosition(LevelEditor_BackCanvas);
			//Point p = GetGridSnapCords(Mouse.GetPosition(LevelEditor_Canvas));
			Point p = GetGridSnapCords(pos);

			newx = (int)p.X; newy = (int)p.Y;
			NewPos_TB.Text = p.ToString();

			if (LevelEditor_Canvas.Children.Contains(selectrect))
				LevelEditor_Canvas.Children.Remove(selectrect);
			Canvas.SetLeft(selectrect, newx); Canvas.SetTop(selectrect, newy); Canvas.SetZIndex(selectrect, 100);
			LevelEditor_Canvas.Children.Add(selectrect);

		}
	}
}
