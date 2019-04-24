using AmethystEngine.Components;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using BixBite.Rendering;
using BixBite;

namespace AmethystEngine.Forms
{

  public enum CardinalDirection
  {
    None,
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW,
  }

	public enum SceneObjectType
	{
		None,
		Level,
		Layer,
		LayerData
	}


  public class LevelEditorProp
  {
    public String PropertyName { get; set; }
    public String PropertyData { get; set; }

    public LevelEditorProp() { }

    public LevelEditorProp(String PName)
    {
      PropertyName = PName;
      PropertyData = "";
    }
  }

	/// <summary>
	/// Interaction logic for EngineEditor.xaml
	/// </summary>
	public partial class EngineEditor : Window
	{

		public List<EditorObject> EditorObj_list = new List<EditorObject>();
		//List<Item> Titles { get; set; }
		public List<EditorObject> Titles = new List<EditorObject>();
		TreeViewItem CurTreeViewNode = new TreeViewItem();
		TreeView CurTreeView = new TreeView();

		/// <summary> This is the List that hold all the property data for a given selected LevelEditor Object </summary>
		public List<LevelEditorProp> LEditorTS;

		//these hold the pointers to the controls that i use for my level editor.
		Canvas MapLEditor_Canvas = new Canvas();
		Canvas FullMapLEditor_Canvas = new Canvas();
		ListBox EditorObjects_LB = new ListBox();
		Canvas TileMap_Canvas_temp = new Canvas();
		VisualBrush FullMapLEditor_VB = new VisualBrush();
		Rectangle FullMapCanvasHightlight_rect = new Rectangle();

		int[,] TileMapData = new int[10, 10];
		public ObservableCollection<Level> OpenLevels { get; set;}
		public ImageBrush imgtilebrush { get; private set; }
		private Tuple<object, SceneObjectType> CurrentLevelEditorSceneObject;

    int EditorGridHeight = 40;
    int EditorGridWidth = 40;
    int NumOfCellsX = 10;
    int NumOfCellsY = 10;
    Point GridOffset = new Point();
    Point MPos = new Point();
    List<String> CMDOutput = new List<string>();
    public double ZoomLevel = 1;
    CardinalDirection MouseMovement = CardinalDirection.None;


    double LevelEditorScreenRatio = 0.0f;

    String ProjectFilePath = "";

    public EngineEditor()
    {
      InitializeComponent();
      LoadInitalVars();
      //LevelEditorMain_Canvas.Background = new DrawingBrush();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      //Find and set level editor controls!
      FullMapLEditor_Canvas = (Canvas)FullMapGrid_Control.Template.FindName("LevelEditor_Canvas", FullMapGrid_Control);
      FullMapLEditor_VB = (VisualBrush)FullMapGrid_Control.Template.FindName("FullLeditorGrid_VB", FullMapGrid_Control);
      EditorObjects_LB = (ListBox)ContentLibrary_Control.Template.FindName("EditorObjects_LB", ContentLibrary_Control);
      TileMap_Canvas_temp = (Canvas)(ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control));

			//LevelEditorScreenRatio = Math.Round(LevelEditor_BackCanvas.ActualWidth / LevelEditor_BackCanvas.ActualHeight,1);

			LEditorTS = new List<LevelEditorProp>()
			{
				new LevelEditorProp(){ PropertyName = "Level Name", PropertyData="Default" },
				new LevelEditorProp(){ PropertyName = "Map Width(cells)", PropertyData="200" },
				new LevelEditorProp(){ PropertyName = "Map Height(cells)", PropertyData="400" },
				//new LevelEditorProp("test 2")								
			};

			OpenLevels = new ObservableCollection<Level>();


			ListBox LB = ((ListBox)(FullMapGrid_Control.Template.FindName("LEditProperty_LB", FullMapGrid_Control)));
      LB.ItemsSource = LEditorTS;

      LevelEditorScreenRatio = double.Parse(LEditorTS[1].PropertyData) / double.Parse(LEditorTS[2].PropertyData);
      Console.WriteLine(LevelEditorScreenRatio);
      NumOfCellsX = int.Parse(LEditorTS[1].PropertyData);
      NumOfCellsY = int.Parse(LEditorTS[2].PropertyData);

      scaleFullMapEditor();
      //FullMapGrid_Control.Template.FindName("")
    }

    public EngineEditor(String FilePath)
    {
      InitializeComponent();
      ProjectFilePath = FilePath;

      //we need to read this file. And set the project settings accordingly.

      LoadInitalVars();
      LoadFileTree(ProjectFilePath.Replace(".gem", "_Game\\Content\\"));
    }

    /// <summary>
    /// Load the item source
    /// </summary>
    private void LoadInitalVars()
    {
      PreviewMouseMove += OnPreviewMouseMove;
      EditorObjects_LB.ItemsSource = EditorObj_list;
      SearchResultList.ItemsSource = Titles;
    }

    /// <summary>
    /// Opens up a file explorer and allows a user to choose a file location.
    /// </summary>
    /// <param name="prompt">What the File Explorer Window will display</param>
    /// <param name="Open">Either a OpenFIleExplorer or a SaveFileExplorer</param>
    /// <returns>File path as a String</returns>
    public static String getFilePath(String prompt, bool Open = false)
    {
      if (!Open)
      {
        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
        dlg.Title = prompt;
        dlg.FileName = "IGNORE THIS"; //default file name
        Nullable<bool> result = dlg.ShowDialog();
        // Process save file dialog box results
        string filename = "";
        if (result == true)
        {
          // Save document
          filename = dlg.FileName;
          filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
        }
        Console.WriteLine(filename);
        return filename;
      }
      else
      {
        // Prepare a dummy string, thos would appear in the dialog
        string dummyFileName = "";

        SaveFileDialog sf = new SaveFileDialog();
        // Feed the dummy name to the save dialog
        sf.Title = "OpenFileRoot";
        sf.FileName = "IGNORE THIS"; //default file name
        sf.FileName = dummyFileName;
        Nullable<bool> result = sf.ShowDialog();
        if (result == true)
        {
          // Now here's our save folder
          string savePath = System.IO.Path.GetDirectoryName(sf.FileName);
          Console.WriteLine(savePath);
          return savePath;
          // Do whatever
        }
        return "";
      }
    }

    /// <summary>
    /// opens the form to create a new project.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CreateNewProject(object sender, RoutedEventArgs e)
    {
      Window w = new NewProject_Form(this);
      w.Show();
    }

    #region This handles all the windows GUI features. Resize, fullscreen. etc

    #region Resizing
    private System.Windows.Interop.HwndSource _hwndSource;

    protected override void OnInitialized(EventArgs e)
    {
      SourceInitialized += OnSourceInitialized;
      base.OnInitialized(e);
    }

    private void OnSourceInitialized(object sender, EventArgs e)
    {
      _hwndSource = (System.Windows.Interop.HwndSource)PresentationSource.FromVisual(this);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

    protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      Rectangle rectangle = sender as Rectangle;
      switch (rectangle.Name)
      {
        case "top":
          Cursor = Cursors.SizeNS;
          ResizeWindow(ResizeDirection.Top);
          break;
        case "bottom":
          Cursor = Cursors.SizeNS;
          ResizeWindow(ResizeDirection.Bottom);
          break;
        case "left":
          Cursor = Cursors.SizeWE;
          ResizeWindow(ResizeDirection.Left);
          break;
        case "right":
          Cursor = Cursors.SizeWE;
          ResizeWindow(ResizeDirection.Right);
          break;
        case "topLeft":
          Cursor = Cursors.SizeNWSE;
          ResizeWindow(ResizeDirection.TopLeft);
          break;
        case "topRight":
          Cursor = Cursors.SizeNESW;
          ResizeWindow(ResizeDirection.TopRight);
          break;
        case "bottomLeft":
          Cursor = Cursors.SizeNESW;
          ResizeWindow(ResizeDirection.BottomLeft);
          break;
        case "bottomRight":
          Cursor = Cursors.SizeNWSE;
          ResizeWindow(ResizeDirection.BottomRight);
          break;
        default:
          break;
      }
    }

    private void ResizeWindow(ResizeDirection direction)
    {
      SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
    }

    private enum ResizeDirection
    {
      Left = 1,
      Right = 2,
      Top = 3,
      TopLeft = 4,
      TopRight = 5,
      Bottom = 6,
      BottomLeft = 7,
      BottomRight = 8,
    }

    protected void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (Mouse.LeftButton != MouseButtonState.Pressed)
        Cursor = Cursors.Arrow;
    }

    public override void OnApplyTemplate()
    {
      /* omitted */

      Grid resizeGrid = this.FindName("resizeGrid") as Grid;
      if (resizeGrid != null)
      {
        foreach (UIElement element in resizeGrid.Children)
        {
          Rectangle resizeRectangle = element as Rectangle;
          if (resizeRectangle != null)
          {
            resizeRectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
            resizeRectangle.MouseMove += ResizeRectangle_MouseMove;
          }
        }
      }

      base.OnApplyTemplate();
    }

    protected void ResizeRectangle_MouseMove(Object sender, MouseEventArgs e)
    {
      Rectangle rectangle = sender as Rectangle;
      switch (rectangle.Name)
      {
        case "top":
          Cursor = Cursors.SizeNS;
          break;
        case "bottom":
          Cursor = Cursors.SizeNS;
          break;
        case "left":
          Cursor = Cursors.SizeWE;
          break;
        case "right":
          Cursor = Cursors.SizeWE;
          break;
        case "topLeft":
          Cursor = Cursors.SizeNWSE;
          break;
        case "topRight":
          Cursor = Cursors.SizeNESW;
          break;
        case "bottomLeft":
          Cursor = Cursors.SizeNESW;
          break;
        case "bottomRight":
          Cursor = Cursors.SizeNWSE;
          break;
        default:
          break;
      }
    }
    #endregion



    private void LBind_close(object sender, RoutedEventArgs e)
    {
      System.Windows.Application.Current.Shutdown();
    }
    private void LBind_FullScreen(object sender, RoutedEventArgs e)
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

    private void LBind_Minimize(object sender, RoutedEventArgs e)
    {
      WindowState = WindowState.Minimized;
      WindowStyle = WindowStyle.None;

    }

    private void LBind_DragMove(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }
    #endregion

    #region "Dynamic Template Binding"
    private void Desc_CB_Click(object sender, RoutedEventArgs e)
    {
      if ((bool)Desc_CB.IsChecked)
        EditorObjects_LB.ItemTemplate = (DataTemplate)this.Resources["BigEdit1"];
      else
      {
        if (this.Resources.Contains("EObj_Small"))
          EditorObjects_LB.ItemTemplate = (DataTemplate)this.Resources["BigEdit"];
      }
    }

    private void EditorWindows_TC_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (EditorWindows_TC.SelectedIndex == 0)
      {
        ContentLibrary_Control.Template = (ControlTemplate)this.Resources["LevelEditorTileMap_Template"];
      }
      else
      {
        ContentLibrary_Control.Template = (ControlTemplate)this.Resources["ContentLibaray_LB_Template"];
        ((ListBox)(ContentLibrary_Control.Template.FindName("EditorObjects_LB", ContentLibrary_Control))).ItemTemplate =
          (DataTemplate)this.Resources["BigEdit"];
        ((ListBox)(ContentLibrary_Control.Template.FindName("EditorObjects_LB", ContentLibrary_Control))).ItemsSource = Titles;
        //((ListBox)(ContentLibrary_Control.Template.FindName("EditorObjects_LB"))).ItemsSource = Titles;

      }
    }
    #endregion

    #region "File/Folder Viewer"
    #region "Folder viewer Tree"
    //TODO: Multi lined label
    private void ProjectContentExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      String TempPic = "/AmethystEngine;component/images/Ame_icon_small.png";
      CurTreeView = (TreeView)sender; Titles.Clear();
      ((TreeViewItem)(CurTreeView.SelectedItem)).IsExpanded = true;
      AmethystEngine.Components.EObjectType EType = EObjectType.File;
      foreach (TreeViewItem tvi in ((TreeViewItem)(CurTreeView.SelectedItem)).Items)
      {
        bool brel = false;
        String desimg = tvi.Tag.ToString();
        String desname = tvi.Tag.ToString();
        desname = desname.Substring(desname.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
        if (tvi.Tag.ToString().Contains(';')) //its a foldername
        {
          brel = true;
          EType = EObjectType.Folder;
          desname = tvi.Header.ToString();
        }
        if (new[] { ".tif", ".jpg", ".png" }.Any(c => desimg.ToLower().Contains(c)))
        {

        }
        else { desimg = TempPic; brel = true; }

        EditorObject ed = new EditorObject(desimg, desname, brel, EType);
        Titles.Add(ed);
      }
      SearchResultList.ItemsSource = null;
      SearchResultList.ItemsSource = Titles;
    }

    private void ListDirectory(TreeView treeView, string path)
    {
      treeView.Items.Clear();
      var rootDirectoryInfo = new DirectoryInfo(path);
      treeView.Items.Add(CreateDirectoryNode(rootDirectoryInfo));
    }

    private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
    {
      String TempPic = "/AmethystEngine;component/images/folder.png";

      var directoryNode = new TreeViewItem { Header = directoryInfo.Name, Tag = TempPic, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
      foreach (var directory in directoryInfo.GetDirectories())
        directoryNode.Items.Add(CreateDirectoryNode(directory));

      foreach (var file in directoryInfo.GetFiles())
        directoryNode.Items.Add(new TreeViewItem { Visibility = System.Windows.Visibility.Collapsed, Tag = directoryInfo.FullName + "\\" + file.Name, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), Header = file.Name });
      return directoryNode;

    }
    /// <summary>
    /// Load the ContentTree View with a file Tree.
    /// </summary>
    /// <param name="ProjPath"></param>
    private void LoadFileTree(String ProjPath = "")
    {
      String Path = (ProjPath == "" ? getFilePath("Get Root Node", true) : ProjPath);
      if (Path != "")
      {
        ListDirectory(ProjectContentExplorer, Path);
      }
    }
    #endregion

    #region "File Traverse Item View"
    private void DirectoryBack_BTN_Click(object sender, RoutedEventArgs e)
    {
      if (((TreeViewItem)ProjectContentExplorer.SelectedItem).Parent != null)
        try
        {
          ((TreeViewItem)((TreeViewItem)ProjectContentExplorer.SelectedItem).Parent).IsSelected = true;
        }
        catch (InvalidCastException) { return; }
    }
    private void SearchResultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ((ListBox)sender).ItemsSource = Titles;

      int i = (((ListBox)sender).SelectedIndex);
      if (i < 0)
        return;

      switch (((EditorObject)Titles[i]).EditObjType)
      {
        case (EObjectType.None):
          return;
        case (EObjectType.File):
          return;
        case (EObjectType.Folder):
          ((TreeViewItem)(((TreeViewItem)(ProjectContentExplorer.SelectedItem)).Items[((ListBox)sender).SelectedIndex])).IsSelected = true;
          return;
      }
    }
    #endregion
    #endregion

    #region "Level Editor"

    #region "Tile Map"
    //Creates a tile brush to paint the editor. Uses selected tile from tile map.
    private void TileMap_Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      RenderTargetBitmap rtb = new RenderTargetBitmap((int)TileMap_Canvas_temp.ActualWidth,
       (int)TileMap_Canvas_temp.ActualHeight, 96d, 96d, System.Windows.Media.PixelFormats.Default);
      rtb.Render(TileMap_Canvas_temp);

      Point pp = Mouse.GetPosition(TileMap_Canvas_temp);
      Console.WriteLine(pp.ToString());
      pp.X -= Math.Floor(pp.X) % 32;  //TODO: Add the offset so we can fill the grid AFTER PAnNNG
      pp.Y -= Math.Floor(pp.Y) % 32;
      int x = (int)pp.X;
      int y = (int)pp.Y;
      Console.WriteLine(String.Format("x: {0},  y: {1}", x, y));
      Console.WriteLine("");
      var crop = new CroppedBitmap(rtb, new Int32Rect(x, y, 32, 32));
      // using BitmapImage version to prove its created successfully
      Image image2 = new Image(); image2.Source = crop; //cropped
      imgtilebrush = new ImageBrush(image2.Source);
      SelectedTile_Canvas.Children.Add(new Rectangle() { Width = 32, Height = 32, Fill = imgtilebrush, RenderTransform = new ScaleTransform(.5, .5) });
    }

    private void TileMap_Canvas_MouseMove(object sender, MouseEventArgs e)
    {
      Canvas TileMap_Canvas_temp = (Canvas)(ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control));
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
      if (imgtilebrush == null)
      {
        Console.WriteLine("no brush selected/tile");
        return;
      }
      Rectangle r = new Rectangle() { Width = 40, Height = 40, Fill = imgtilebrush };
      Point p = GetGridSnapCords(Mouse.GetPosition(LevelEditor_BackCanvas));

			int iii = 0;

			//are we clicked on a spritelayer? AND a tile layer?
			if (SceneExplorer_TreeView.SelectedValue is SpriteLayer && ((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
			{
				//what layer are we on?
				foreach(Level lev in OpenLevels)
				{
					if (lev.Layers.IndexOf( ((SpriteLayer)SceneExplorer_TreeView.SelectedValue) ) > 0)
					{
						iii = lev.Layers.IndexOf(((SpriteLayer)SceneExplorer_TreeView.SelectedItem));
						Console.WriteLine(iii);
					}
				}
				//Level TempLevel = ((Level)((TreeViewItem)SceneExplorer_TreeView.SelectedItem).Parent);
				//((SpriteLayer)SceneExplorer_TreeView.SelectedValue).get
			}
			else
				return;


      Canvas.SetLeft(r, (int)p.X); Canvas.SetTop(r, (int)p.Y); Canvas.SetZIndex(r, 0);
      LevelEditor_Canvas.Children.Add(r);

      FullMapEditorFill(p);
    }

    private Point GetGridSnapCords(Point p)
    {
      int Xoff = (int)(Math.Abs(Canvas_grid.Viewport.X)) % EditorGridWidth; Xoff = EditorGridWidth - Xoff; //offset
      int YOff = (int)(Math.Abs(Canvas_grid.Viewport.Y)) % EditorGridHeight; YOff = EditorGridHeight - YOff;

      p.X /= ZoomLevel; p.Y /= ZoomLevel;
      p.X -= Math.Floor(p.X - Xoff) % EditorGridWidth;  //TODO: Add the offset so we can fill the grid AFTER PAnNNG
      p.Y -= Math.Floor(p.Y - YOff) % EditorGridHeight;
      return p;
    }

    private void FullMapEditorFill(Point p)
    {
      //flll the data.
      int fullY = (int)FullMapLEditor_Canvas.ActualHeight; fullY -= fullY % (NumOfCellsY);
      int fullX = (int)FullMapLEditor_Canvas.ActualWidth; fullX -= fullX % (NumOfCellsX);
      fullX = fullX / (NumOfCellsX);
      fullY = fullY / (NumOfCellsY);
      TileMapData[((int)p.X + (int)Math.Abs(Canvas_grid.Viewport.X)) / EditorGridWidth,
        ((int)p.Y + (int)Math.Abs(Canvas_grid.Viewport.Y)) / EditorGridHeight] = 1;

      Rectangle r = new Rectangle() { Width = 4, Height = 4, Fill = imgtilebrush };

      int setX = (4 * ((int)p.X / EditorGridWidth)) + ((int)GridOffset.X);
      int setY = (4 * ((int)p.Y / EditorGridHeight)) + ((int)GridOffset.Y);

      Canvas.SetLeft(r, setX); Canvas.SetTop(r, setY);
      FullMapLEditor_Canvas.Children.Add(r);
      FullMapCanvasHightlight_rect = r;
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
      String point = String.Format("({0}, {1})", (int)p.X, (int)p.Y);
      LevelEditorCords_TB.Text = point;

      //which way is mouse moving?
      MPos -= (Vector)e.GetPosition(LevelEditor_Canvas);
      if (MPos.X == 0 && MPos.Y > 0) //north
        MouseMovement = CardinalDirection.N;
      if (MPos.X > 0 && MPos.Y > 0) //North East
        MouseMovement = CardinalDirection.NE;
      if (MPos.X > 0 && MPos.Y == 0) //East
        MouseMovement = CardinalDirection.E;
      if (MPos.X > 0 && MPos.Y < 0) //South East
        MouseMovement = CardinalDirection.SE;
      if (MPos.X == 0 && MPos.Y < 0) //South
        MouseMovement = CardinalDirection.S;
      if (MPos.X < 0 && MPos.Y < 0) //South West
        MouseMovement = CardinalDirection.SW;
      if (MPos.X < 0 && MPos.Y == 0) //West
        MouseMovement = CardinalDirection.W;
      if (MPos.X < 0 && MPos.Y > 0) //North West
        MouseMovement = CardinalDirection.NW;
      //is the middle mouse button down?
      if (e.MiddleButton == MouseButtonState.Pressed)
        LavelEditorPan();
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
      if (FullMapLEditor_Canvas.Children.Count == 0) return;

      int MainCurCellsX = ((int)Math.Ceiling((LevelEditor_BackCanvas.ActualWidth / Canvas_grid.Viewport.Width)));
      int MainCurCellsY = ((int)Math.Ceiling(LevelEditor_BackCanvas.ActualHeight / Canvas_grid.Viewport.Height));

      Rectangle r = new Rectangle() { Width = MainCurCellsX * 4, Height = MainCurCellsY * 4, Stroke = Brushes.White, StrokeThickness = 1, Name = "SelectionRect" };
      TileMapData = new int[(NumOfCellsX), (NumOfCellsY)];
      Canvas.SetLeft(r, 0); Canvas.SetTop(r, 0);

      FullMapLEditor_Canvas.Children.RemoveAt(0);
      FullMapLEditor_Canvas.Children.Insert(0, r);

    }

    #region "Panning"
    /// <summary>
    /// Performs the panning effect on the main level editor canvas.
    /// </summary>
    private void LavelEditorPan()
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
      FullMapLEditor_VB.Viewport = new Rect()
      {
        X = FullMapLEditor_VB.Viewport.X + MPos.X / 10,
        Y = FullMapLEditor_VB.Viewport.Y + MPos.Y / 10,
        Width = FullMapLEditor_VB.Viewport.Width,
        Height = FullMapLEditor_VB.Viewport.Height
      }; //desices how quick we will pan.
      //Moves ONLY the selection rectangle in the full map viewer.
      foreach (UIElement child in FullMapLEditor_Canvas.Children)
      {
        if (((Rectangle)child).Name == "SelectionRect")
        {
          double x = Canvas.GetLeft(child);
          double y = Canvas.GetTop(child);
          Canvas.SetLeft(child, x - MPos.X / 10); //TODO: give this an if statment so the grid cannot go off the screen.
          Canvas.SetTop(child, y - MPos.Y / 10);  //SCALE use the ratio to pan and link both accurately
        }
      }
      GridOffset.X -= MPos.X / 10; //keeps in sync
      GridOffset.Y -= MPos.Y / 10;
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
        ZoomLevel += .2;
        Canvas_grid.Transform = new ScaleTransform(ZoomLevel, ZoomLevel);
        LevelEditor_Canvas.RenderTransform = new ScaleTransform(ZoomLevel, ZoomLevel);
        Console.WriteLine(String.Format("W:{0},  H{1}", EditorGridWidth, EditorGridHeight));
        //TODO: resize selection rectangle
      }
      else  //zoom out!
      {
        ZoomLevel -= .2;
        Canvas_grid.Transform = new ScaleTransform(ZoomLevel, ZoomLevel);
        LevelEditor_Canvas.RenderTransform = new ScaleTransform(ZoomLevel, ZoomLevel);
        Console.WriteLine(String.Format("W:{0},  H{1}", EditorGridWidth, EditorGridHeight));
        //TODO: resize selection rectangle
      }
    }
    #endregion
    #endregion

    #region "Full Level Canvas"
    private void scaleFullMapEditor()
    {
      FullMapLEditor_Canvas.Width = NumOfCellsX * 2;
      FullMapLEditor_Canvas.Height = NumOfCellsY * 2;

      Console.WriteLine("tewst");
      int fullY = (int)FullMapLEditor_Canvas.Width; fullY -= fullY % (NumOfCellsY);
      int fullX = (int)FullMapLEditor_Canvas.Height; fullX -= fullX % (NumOfCellsX);
      int UniformCellHeight = (fullX / NumOfCellsX < fullY / NumOfCellsY ? fullX / NumOfCellsX : fullY / NumOfCellsY);
      UniformCellHeight = 4;

      FullMapLEditor_VB.Viewport = new Rect(0, 0, UniformCellHeight, UniformCellHeight);

      //find out the num of cells in the MAIN grid editor
      int MainCurCellsX = ((int)(LevelEditor_BackCanvas.ActualWidth / Canvas_grid.Viewport.Width));
      int MainCurCellsY = ((int)(LevelEditor_BackCanvas.ActualHeight / Canvas_grid.Viewport.Height));

      Rectangle r = new Rectangle() { Width = MainCurCellsX * 4, Height = MainCurCellsY * 4, Stroke = Brushes.White, StrokeThickness = 1, Name = "SelectionRect" };
      TileMapData = new int[(NumOfCellsX), (NumOfCellsY)];
      Canvas.SetLeft(r, 0); Canvas.SetTop(r, 0);
      FullMapLEditor_Canvas.Children.Add(r);
    }
    
    //quick move of the level editor canvas. when you click the screen renders that section.
    private void EditorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {

      //LevelEditor_Canvas.se
    }
    #region "Property Hot Reloading"

    #endregion
    #endregion

    #region "Tools"

    #endregion
    #endregion

    #region "Content Library"
    /// <summary>
    /// When the import button is pressed in the content explorer section of the editor.
    /// Functionality depends on the current open editor.
    /// IF(LEVEL) {Only allow PNG}
    /// ELSE {//TODO: WIP}
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ImportToEditor(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
      dlg.FileName = "Document"; //default file name
                                 //dlg.DefaultExt = ".xml"; //default file extension
                                 //dlg.Filter = "XML documents (.xml)|*.xml"; //filter files by extension

      // Show save file dialog box
      Nullable<bool> result = dlg.ShowDialog();
      // Process save file dialog box results kicks out if the user doesn't select an item.
      string filename = "";
      if (result == true)
        filename = dlg.FileName;
      else
        return;

      Console.WriteLine(filename);

      if (EditorWindows_TC.SelectedIndex == 0)
      {
        Canvas TileMap = (Canvas)ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control);
        System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
        Image image = new Image();
        var pic = new System.Windows.Media.Imaging.BitmapImage();
        pic.BeginInit();
        pic.UriSource = new Uri(filename); // url is from the xml
        pic.EndInit();
        image.Source = pic;
        image.Width = 512;
        image.Height = 512;

        TileMap.Children.Add(image);

      }
      else
      {
        EditorObject E = new EditorObject(filename, filename.Substring(filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) + 1, filename.LastIndexOf(".") - filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) - 1), false);
        EditorObj_list.Add(E);//Thumbnail = new Image() { Source = new BitmapImage(new Uri("images/Ame_icon_small.png", UriKind.Relative)) } });
        EditorObjects_LB.ItemsSource = null;
        //EditorObjects_LB.UpdateLayout();
        EditorObjects_LB.ItemsSource = EditorObj_list;
      }
    }
		#endregion

		#region "Scene Viewer"

		#endregion

		#region GetImageFromCanvas
		private BitmapImage GetJpgImage(BitmapSource source)
    {
      return GetImage(source, new JpegBitmapEncoder());
    }

    private BitmapImage GetPngImage(BitmapSource source)
    {
      return GetImage(source, new PngBitmapEncoder());
    }

    private BitmapImage GetImage(BitmapSource source, BitmapEncoder encoder)
    {
      var bmpImage = new BitmapImage();

      using (var srcMS = new MemoryStream())
      {
        encoder.Frames.Add(BitmapFrame.Create(source));
        encoder.Save(srcMS);

        srcMS.Position = 0;
        using (var destMS = new MemoryStream(srcMS.ToArray()))
        {
          bmpImage.BeginInit();
          bmpImage.StreamSource = destMS;
          bmpImage.CacheOption = BitmapCacheOption.OnLoad;
          bmpImage.EndInit();
          bmpImage.Freeze();
        }
      }
      return bmpImage;
    }
    #endregion


    public void DoEvents()
    {
      DispatcherFrame frame = new DispatcherFrame();
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
          new DispatcherOperationCallback(ExitFrame), frame);
      Dispatcher.PushFrame(frame);
    }
    public object ExitFrame(object f)
    {
      ((DispatcherFrame)f).Continue = false;
      return null;
    }

    private void ProjectSettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
      Window w = new ProjectSettings();
      w.Show();
    }
    
    public void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
      // (outLine.Data) <- this field contains outputData text
      Console.WriteLine(outLine.Data);
      CMDOutput.Add(outLine.Data);

      if (outLine.Data.Contains("monogame"))
      {
        Console.WriteLine("-----------------The Template is insatlled----------------------");
      }
      else
      {
        //Console.WriteLine("-----------------The Template is NOT insatlled------------------");

      }
    }

		private void NewLevel_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			LEStarting_TB.Visibility = Visibility.Hidden;
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control)).IsEnabled = true;
			LevelEditor_Canvas.IsEnabled = true;

			//TreeViewItem LevelRoot = new TreeViewItem() { Header = "NewLevel" };

			//SceneExplorer_TreeView.Items.Add(LevelRoot);

			//LevelRoot.Items.Add(new TreeViewItem() { Header = "Background Layer" });

			//create a new Level object.
			CreateLevel("testing");
			SceneExplorer_TreeView.ItemsSource = OpenLevels;
		}

		private void CreateLevel(String LevelName)
		{
			//create new Level
			Level TempLevel = new Level(LevelName);
			SpriteLayer TempLevelChild = new SpriteLayer(LayerType.Tile) { LayerName = "Background" };
			TempLevel.Layers.Add(TempLevelChild);

			////show the newly created LEVEL
			//TreeViewItem TVI = new TreeViewItem() { Header = LevelName, Tag = "Level"};
			//TreeViewItem TVI1 = new TreeViewItem() { Header = "Backgorund", Tag = "Layer" };
			//TVI.Items.Add(TVI1);
			//SceneExplorer_TreeView.Items.Add(TVI);
			OpenLevels.Add(TempLevel);
			

		}

		private void SceneExplorer_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			Console.WriteLine("Changed Scene Object");
			if(e.NewValue is Level)
				CurrentLevelEditorSceneObject = new Tuple<object, SceneObjectType>(e.NewValue, SceneObjectType.Level);
			if (e.NewValue is SpriteLayer)
				CurrentLevelEditorSceneObject = new Tuple<object, SceneObjectType>(e.NewValue, SceneObjectType.Layer);

		}
		
		private void SceneViewAdd_BTN_Click(object sender, RoutedEventArgs e)
		{
			//what editor are we currently in?
			if (EditorWindows_TC.SelectedIndex == 0)  //Level Editor
			{
				if (SceneExplorer_TreeView.HasItems) //there is no current Level we are editing.
				{
					//are we clicked on a level? Then create a new layer
					if (SceneExplorer_TreeView.SelectedValue is Level)
					{
						ContextMenu cm = this.FindResource("LevelContextMenu_Template") as ContextMenu;
						cm.PlacementTarget = sender as Button;
						cm.IsOpen = true;
					}

					//are we clicked on a Layer? 
					if(CurrentLevelEditorSceneObject.Item1 is SpriteLayer)
					{
						//What type of layer?


					}

					//create new sprite layer object data.
				}
			}
		}

		private void AddTileLayer_Click(object sender, RoutedEventArgs e)
		{
			((Level)SceneExplorer_TreeView.SelectedValue).Layers.Add(new SpriteLayer(LayerType.Tile) { LayerName = "new tile" });
		}
		private void SpriteLayer_Click(object sender, RoutedEventArgs e)
		{
			((Level)SceneExplorer_TreeView.SelectedValue).Layers.Add(new SpriteLayer(LayerType.Sprite) { LayerName = "new sprite" });
		}
		private void GameObjectLayer_Click(object sender, RoutedEventArgs e)
		{
			((Level)SceneExplorer_TreeView.SelectedValue).Layers.Add(new SpriteLayer(LayerType.Gameobject) { LayerName = "new G.O." });
		}


	}
}
