using AmethystEngine.Components;
using AmethystEngine.Components.Tools;
using BixBite;
using BixBite.Rendering;
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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using BixBite.Resources;

namespace AmethystEngine.Forms
{

	public enum SceneObjectType
	{
		None,
		Level,
		Layer,
		LayerData
	}

	public enum EditorTool
	{
		None,
		Select,
		Eraser,
		Move,
		Brush,
    Fill,
		Image,
		Gameevent,
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
		public List<EditorObject> SpriteObjectList = new List<EditorObject>();
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
		VisualBrush FullMapLEditor_VB = new VisualBrush();
		Rectangle FullMapCanvasHightlight_rect = new Rectangle();

		VisualBrush TileMap_VB = new VisualBrush();
		Canvas TileMap_Canvas = new Canvas();
		ComboBox TileSets_CB = new ComboBox();
		Rectangle TileMapTiles_Rect = new Rectangle();
		Rectangle FullMapSelection_Rect = new Rectangle();
		
		EditorTool CurrentTool = EditorTool.None;
		SelectTool selectTool = new SelectTool();

		public ObservableCollection<Level> OpenLevels { get; set;}
		public Level CurrentLevel = new Level();
		public ImageBrush imgtilebrush { get; private set; }
		public Control currentCC = new ContentControl();
		public Rectangle curect = new Rectangle();

		private Tuple<object, SceneObjectType> CurrentLevelEditorSceneObject;
    Point[] SelectionRectPoints = new Point[2];
		Point[] shiftpoints = new Point[3]; //0 = mouse down , 1 = mouse up , 2 on shifting "tick"
		private List<Image> TileSets = new List<Image>();

		int EditorGridHeight = 40;
    int EditorGridWidth = 40;
    int NumOfCellsX = 10;
    int NumOfCellsY = 10;
    Point GridOffset = new Point();
    Point MPos = new Point();
    List<String> CMDOutput = new List<string>();
    public double ZoomLevel = 1;
    BixBite.BixBiteTypes.CardinalDirection MouseMovement = BixBite.BixBiteTypes.CardinalDirection.None;
		
    double LevelEditorScreenRatio = 0.0f;

    String ProjectFilePath = "";
		String MainLevelPath = "";

		ObservablePropertyDictionary EditorObjectProperties = new ObservablePropertyDictionary();

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
      TileMap_Canvas = (Canvas)(ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control));
			TileSets_CB = (ComboBox)(ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control));
			TileMapTiles_Rect = (Rectangle)ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB_Rect", ContentLibrary_Control);
			TileMap_VB = (VisualBrush)ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB", ContentLibrary_Control);

			//LevelEditorScreenRatio = Math.Round(LevelEditor_BackCanvas.ActualWidth / LevelEditor_BackCanvas.ActualHeight,1);
			LEditorTS = new List<LevelEditorProp>()
			{
				new LevelEditorProp(){ PropertyName = "Level Name", PropertyData="Default" },
				new LevelEditorProp(){ PropertyName = "Map Width(cells)", PropertyData="30" },
				new LevelEditorProp(){ PropertyName = "Map Height(cells)", PropertyData="50" },
				//new LevelEditorProp("test 2")								
			};



			OpenLevels = new ObservableCollection<Level>();


			ListBox LB = ((ListBox)(FullMapGrid_Control.Template.FindName("LEditProperty_LB", FullMapGrid_Control)));
			LB.ItemsSource = null;
			//LB.ItemsSource = LEditorTS;

      LevelEditorScreenRatio = double.Parse(LEditorTS[1].PropertyData) / double.Parse(LEditorTS[2].PropertyData);
      Console.WriteLine(LevelEditorScreenRatio);
      NumOfCellsX = int.Parse(LEditorTS[1].PropertyData);
      NumOfCellsY = int.Parse(LEditorTS[2].PropertyData);

			//scaleFullMapEditor();
			//FullMapGrid_Control.Template.FindName("")

			//load main level
			LoadMainLevel(ProjectFilePath);


			//can i get the property dictionary to work?
			EditorObjectProperties.Add("Name", new Tuple<String, string>("TextBoxPropertyTemplate", "50"));
			EditorObjectProperties.Add("Width", new Tuple<String, string>("TextBoxPropertyTemplate", "50"));
			EditorObjectProperties.Add("Height", new Tuple<String, string>("TextBoxPropertyTemplate", "50"));
			LB.ItemsSource = EditorObjectProperties;

		}

		public EngineEditor(String FilePath, String ProjectName = "", String LevelPath = "")
    {
      InitializeComponent();
      ProjectFilePath = FilePath;
			
			//we need to read this file. And set the project settings accordingly.
			ProjectName_LBL.Content = ProjectName;
			MainLevelPath = LevelPath;
			LoadInitalVars();
			LoadFileTree(ProjectFilePath.Replace(".gem", "_Game\\Content\\"));
    }
		
		private void LoadMainLevel(String filepath)
		{
			using (StreamReader file = new StreamReader(filepath))
			{
				int counter = 0;
				string ln;

				while ((ln = file.ReadLine()) != null)
				{
					if (ln.Contains("MainLevel"))
					{
						ln = file.ReadLine();
						if (ln.Contains("FILL"))
							return;
						else
						{
							if (File.Exists(ln))
								importLevel(ln);
							else
								Console.WriteLine("file DNE: " + ln);
						}
					}
				}
				file.Close();
			}
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

		private void ProjectSettingsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Window w = new ProjectSettings(ProjectFilePath);
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
					if (element is Rectangle resizeRectangle)
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
			if (EditorWindows_TC.SelectedIndex == 0) //we are in the level editor.
			{
				TabControl LELibary_TC = (TabControl)ContentLibrary_Control.Template.FindName("LevelEditorLibary_TabControl", ContentLibrary_Control);
				if (LELibary_TC.SelectedIndex == 1) //sprite libary
				{
					ListBox SpriteLibary_LB = (ListBox)ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control);
					if ((bool)Desc_CB.IsChecked)
						SpriteLibary_LB.ItemTemplate = (DataTemplate)this.Resources["BigEdit1"];
					else
					{
						if (this.Resources.Contains("EObj_Small"))
							SpriteLibary_LB.ItemTemplate = (DataTemplate)this.Resources["BigEdit"];
					}
				}
			}
			else // other editors WIP
			{
				if ((bool)Desc_CB.IsChecked)
					EditorObjects_LB.ItemTemplate = (DataTemplate)this.Resources["BigEdit1"];
				else
				{
					if (this.Resources.Contains("EObj_Small"))
						EditorObjects_LB.ItemTemplate = (DataTemplate)this.Resources["BigEdit"];
				}
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
        //ContentLibrary_Control.Template = (ControlTemplate)this.Resources["ContentLibaray_LB_Template"];
        //((ListBox)(ContentLibrary_Control.Template.FindName("EditorObjects_LB", ContentLibrary_Control))).ItemTemplate =
        //  (DataTemplate)this.Resources["BigEdit"];
        //((ListBox)(ContentLibrary_Control.Template.FindName("EditorObjects_LB", ContentLibrary_Control))).ItemsSource = Titles;
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
			if (CurTreeView.Items.Count == 0) return; 
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
        if (new[] { ".tif", ".jpg", ".png" }.Any(c => desname.ToLower().Contains(c)))
        {
					EditorObject ed = new EditorObject(desimg, desname, brel, EObjectType.File);
					Titles.Add(ed);
				}
        else if(!tvi.Header.ToString().Contains(".")){
					//desimg = TempPic; brel = true;
					EditorObject ed = new EditorObject(desimg, desname, brel, EType);
					Titles.Add(ed);
				}
				else
				{
					desimg = TempPic; brel = true;
					EditorObject ed = new EditorObject(desimg, desname, brel, EObjectType.File);
					Titles.Add(ed);
				}

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

		/// <summary>
		/// imports files to the current games content folder.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContentExplorerImport_BTN_Click(object sender, RoutedEventArgs e)
		{

			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.FileName = "assets"; //default file 
			dlg.Title = "Import New Assest";
			dlg.DefaultExt = "All files (*.*)|*.*"; //default file extension
			dlg.Filter = "Level files (*.lvl)|*.lvl|png file (*.png)|*.png|All files (*.*)|*.*";

			// Show save file dialog box
			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results kicks out if the user doesn't select an item.
			String importfilename, destfilename = "";
			if (result == true)
			{
				importfilename = dlg.FileName;

			}
			else
				return;

			String importendlocation, importstartlocation = "";
			importendlocation = ProjectFilePath.Replace(".gem", "_Game\\Content\\");
			importstartlocation = dlg.FileName;

			//filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			int len = importstartlocation.Length - importstartlocation.LastIndexOfAny(new char[] { '/', '\\' });
			destfilename = importendlocation + importstartlocation.Substring(
				importstartlocation.LastIndexOfAny(new char[] { '\\', '/' }) + 1, len - 1);

			//copy the file
			File.Copy(importfilename, destfilename, true);
			//LoadInitalVars();
			LoadFileTree(ProjectFilePath.Replace(".gem", "_Game\\Content\\")); //reload the project to show the new file.

			Titles.Add(new EditorObject(destfilename, importstartlocation.Substring(
				importstartlocation.LastIndexOfAny(new char[] { '\\', '/' }) + 1, len - 1), true, EObjectType.File));

			SearchResultList.ItemsSource = null;
			SearchResultList.ItemsSource = Titles;

		}

		#endregion

		#region "File Traverse Item View"
		private void DirectoryBack_BTN_Click(object sender, RoutedEventArgs e)
    {
			if (((TreeViewItem)ProjectContentExplorer.SelectedItem) == null) return;	//you need to click on something...

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
			int xtile, ytile, TileSetOffest = 0;
			xtile = CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item3; ytile = CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item4;

			//there can be multiple tile sets per level file. 
			for(int i = 0; i < TileSets_CB.SelectedIndex; i++)
			{
				BitmapImage im1 = new BitmapImage();
				im1.BeginInit();
				im1.UriSource = new Uri(CurrentLevel.TileSet[i].Item2);
				im1.EndInit();

				System.Drawing.Image img = System.Drawing.Image.FromFile(CurrentLevel.TileSet[i].Item2);
				//img.Source = im1;
				TileSetOffest += (int)((img.Width/ CurrentLevel.TileSet[i].Item3) * (img.Height / CurrentLevel.TileSet[i].Item4));
			}

      SelectedTile_Canvas.Children.Clear(); imgtilebrush = null;
      RenderTargetBitmap rtb = new RenderTargetBitmap((int)TileMap_Canvas.ActualWidth,
       (int)TileMap_Canvas.ActualHeight, 96d, 96d, System.Windows.Media.PixelFormats.Default);
      rtb.Render(TileMap_Canvas);

      Point pp = Mouse.GetPosition(TileMap_Canvas);
      Console.WriteLine(pp.ToString());
      pp.X -= Math.Floor(pp.X) % xtile;  //TODO: Add the offset so we can fill the grid AFTER PAnNNG
      pp.Y -= Math.Floor(pp.Y) % ytile;
      int x = (int)pp.X;
      int y = (int)pp.Y;
      Console.WriteLine(String.Format("x: {0},  y: {1}", x, y));
      Console.WriteLine("");
      var crop = new CroppedBitmap(rtb, new Int32Rect(x, y, xtile, ytile));
      // using BitmapImage version to prove its created successfully
      Image image2 = new Image(); image2.Source = crop; //cropped
      imgtilebrush = new ImageBrush(image2.Source);
			//calculate the int value in canvas "array"
			int tilenumdata = ((y / ytile) * ((int)TileMap_Canvas.ActualWidth / xtile)) + (x / xtile);
			tilenumdata += TileSetOffest;

			SelectedTile_Canvas.Children.Add(new Rectangle() { Width = xtile, Height = ytile,
				Fill = imgtilebrush, Tag = tilenumdata, RenderTransform = new ScaleTransform(.8,.8)});
			SelectedTile_Canvas.ToolTip = tilenumdata;

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
					if (SelectedTile_Canvas.Children.Count == 0) return;
					Rectangle r = new Rectangle() { Width = 40, Height = 40, Fill = imgtilebrush }; //create the tile that we wish to add to the grid. always 40 becasue thats the base. 

					Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), curLayer, (int)pos.X, (int)pos.Y);
					if (rr != null)
					{ Console.WriteLine(String.Format("Cell ({0},{1}) already is filled", (int)pos.X, (int)pos.Y)); return; }//check to see if the current tile exists. if so then don't add.

					Canvas.SetLeft(r, (int)p.X); Canvas.SetTop(r, (int)p.Y); Canvas.SetZIndex(r, curLayer); //place the tile position wise
					LevelEditor_Canvas.Children.Add(r); //actual place it on the canvas

					//add offset to point P to turn rel to Abs pos.
					p.X += Math.Ceiling(Math.Abs(Canvas_grid.Viewport.X));
					p.Y += Math.Ceiling(Math.Abs(Canvas_grid.Viewport.Y));
					FullMapEditorFill(p, curLayer); //update the fullmap display to reflect this change
				}
				else if (CurrentTool == EditorTool.Select)
				{
					//find the tile
					if (SceneExplorer_TreeView.SelectedValue is SpriteLayer && ((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
					{
						Rectangle r = new Rectangle() { Tag = "selection", Width = 40, Height = 40, Fill = new SolidColorBrush(Color.FromArgb(100, 0, 20, 100)) };
						Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), curLayer, (int)pos.X, (int)pos.Y);
						Canvas.SetLeft(r, (int)p.X); Canvas.SetTop(r, (int)p.Y); Canvas.SetZIndex(r, 100);

						//don't add another selection rectangle on an existing selection rectangle
						Rectangle sr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), 100, (int)pos.X, (int)pos.Y);

						if (sr != null) return;
						selectTool.SelectedTiles.Add(rr);
						LevelEditor_Canvas.Children.Add(r);

						SelectionRectPoints[0] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
					}
					else if(SceneExplorer_TreeView.SelectedValue is SpriteLayer && ((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.GameEvent)
					{
						//this selection works similar to tile. But instead it looks for borders not rectangles. It also will retrieve the game event group number
						Rectangle r = new Rectangle() { Tag = "selection", Width = 40, Height = 40, Fill = new SolidColorBrush(Color.FromArgb(100, 0, 20, 100)) };


						//Rectangle r = new Rectangle() { Tag = "selection", Width = 40, Height = 40, Fill = new SolidColorBrush(Color.FromArgb(100, 0, 20, 100)) };
						//Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), curLayer, (int)pos.X, (int)pos.Y);
						//Canvas.SetLeft(r, (int)p.X); Canvas.SetTop(r, (int)p.Y); Canvas.SetZIndex(r, 100);

						////don't add another selection rectangle on an existing selection rectangle
						//Rectangle sr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), 100, (int)pos.X, (int)pos.Y);

						//if (sr != null) return;
						//selectTool.SelectedTiles.Add(rr);
						//LevelEditor_Canvas.Children.Add(r);

						//SelectionRectPoints[0] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.

					}
				}
				else if (CurrentTool == EditorTool.Eraser)
				{
					//find the tile on the layer that is selected.
					Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), curLayer, (int)pos.X, (int)pos.Y);
					if (rr == null) return; //if you click on a empty rect return
																	//find the rect in the current layers displayed tiles
					int i = LevelEditor_Canvas.Children.IndexOf(rr);
					int x = -1; int y = -1;
					if (i >= 0)
					{
						y = (int)Canvas.GetLeft(rr) / 40;
						x = (int)Canvas.GetTop(rr) / 40;
						LevelEditor_Canvas.Children.RemoveAt(i); //delete it.
					}
					if (x < 0 || y < 0) { Console.WriteLine("desired cell to delete DNE"); return; }
					//find the data in the level objects sprite layer. And then clear it.
					SpriteLayer curlayer = (SpriteLayer)SceneExplorer_TreeView.SelectedValue;
					curlayer.DeleteFromLayer(x, y);

					//delect
					Deselect();
				}
				else if (CurrentTool == EditorTool.Move)
				{
					//SelectionRectPoints[0] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
					shiftpoints[0] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
					shiftpoints[2] = shiftpoints[0];
				}
				
			}
			else if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Sprite)
			{
				TabControl Content_TC = (TabControl)(ContentLibrary_Control.Template.FindName("LevelEditorLibary_TabControl", ContentLibrary_Control));
				//make sure we are in the correct tab of the level editor content libary
				if (Content_TC.SelectedIndex == 1 && CurrentTool == EditorTool.Image)
				{
					//is there a sprite selected?
					ListBox Sprite_LB = (ListBox)(ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control));
					if (Sprite_LB.SelectedIndex >= 0)
					{
						List<EditorObject> sprites = (List<EditorObject>)Sprite_LB.ItemsSource;
						EditorObject currentobj = sprites[Sprite_LB.SelectedIndex];
						Console.WriteLine(currentobj.Thumbnail.AbsolutePath); //prints the path

						System.Drawing.Image currentimg = System.Drawing.Image.FromFile(currentobj.Thumbnail.AbsolutePath);
						BitmapImage bitmap = new BitmapImage(new Uri(currentobj.Thumbnail.AbsolutePath, UriKind.Absolute));
						Image img = new Image(); img.Source = bitmap;
						Rectangle r = new Rectangle() { Width = currentimg.Width, Height = currentimg.Height, Fill = new ImageBrush(img.Source) };//Make a rectange teh size of the image
						Canvas.SetLeft(r, pos.X); Canvas.SetTop(r, pos.Y); Canvas.SetZIndex(r, curLayer);
						//LevelEditor_Canvas.Children.Add(r);

						ContentControl CC = ((ContentControl)this.TryFindResource("MoveableImages_Template"));
						CC.Width = currentimg.Width;
						CC.Height = currentimg.Height;

						Canvas.SetLeft(CC, pos.X); Canvas.SetTop(CC, pos.Y); Canvas.SetZIndex(CC, curLayer);
						Selector.SetIsSelected(CC, false);
						CC.MouseRightButtonDown += ContentControl_MouseLeftButtonDown;
						var m = (CC.FindName("ResizeImage_rect"));
						((Rectangle)CC.Content).Fill = new ImageBrush(img.Source) ;
						LevelEditor_Canvas.Children.Add(CC);

						//add to minimap
						Sprite s = new Sprite("new sprite", currentobj.Thumbnail.AbsolutePath, (int)pos.X, (int)pos.Y, currentimg.Width, currentimg.Height);
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
					TextBlock tb = new TextBlock()
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						TextAlignment = TextAlignment.Center,
						TextWrapping = TextWrapping.Wrap,
						Width = 40,
						Height = 40,
						FontSize = 18,
						Text = "N",
						Tag = "N",
						Foreground = new SolidColorBrush(Colors.Black),
						Background = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100)),
					};
					Border b = new Border() { Width = 40, Height = 40 };
					b.Child = tb;

					Canvas.SetLeft(b, (int)p.X); Canvas.SetTop(b, (int)p.Y); Canvas.SetZIndex(b, curLayer); //place the tile position wise
					LevelEditor_Canvas.Children.Add(b); //actual place it on the canvas
				}
				else if (CurrentTool == EditorTool.Select) //selected a game event square.
				{
					Console.WriteLine("GE - Selected");

					
				}
			}
		}
		
		private void ContentControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Console.WriteLine("testing");
			Selector.SetIsSelected(((Control)currentCC), false);
			curect.IsHitTestVisible = true;

			//the current CC has been changed. so we need to reflect that in the data
			//TODO:
			if(currentCC != null)
			{

			}

			currentCC = ((ContentControl)sender);
			Selector.SetIsSelected(((Control)currentCC), true);
			curect.IsHitTestVisible = false;
		}

		private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Console.WriteLine("sjdsjdns");
			curect.IsHitTestVisible = true;
			curect = ((Rectangle)sender);
			curect.IsHitTestVisible = false;
			//var m = Application.Current.MainWindow.FindResource("DesignerItemStyle");
		}

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

						SelectionRectPoints[1] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y);
						Console.WriteLine("Select MUP");

						//At this point the entire selection area should be drawn.
						int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
						int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
						int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

						columns /= relgridsize;
						rows /= relgridsize;

						Point begginning = RelativeGridSnap(SelectionRectPoints[0]);
						begginning.X = (int)begginning.X; begginning.Y = (int)begginning.Y;
						for (int i = 0; i <= columns; i++)
						{
							for (int j = 0; j <= rows; j++)
							{
								//find the rectangle
								Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(),
									curLayer, (int)begginning.X + (relgridsize * i) + 1, (int)begginning.Y + (relgridsize * j) + 1);
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
						begginning.X = (int)begginning.X; begginning.Y = (int)begginning.Y;
						shiftpoints[1] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y);

						int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
						int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);
						//how much to move/shift the data.
						int shiftcolumns = (int)(RelativeGridSnap(shiftpoints[1]).X - RelativeGridSnap(shiftpoints[0]).X);
						int shiftrows = (int)(RelativeGridSnap(shiftpoints[1]).Y - RelativeGridSnap(shiftpoints[0]).Y);
						int absrow = 0; int abscol = 0; Point p = GetGridSnapCords(Mouse.GetPosition(LevelEditor_BackCanvas));
						columns /= relgridsize; shiftcolumns /= relgridsize;
						rows /= relgridsize; shiftrows /= relgridsize;
						for (int i = 0; i <= columns; i++)
						{
							for (int j = 0; j <= rows; j++)
							{
								absrow = ((int)begginning.Y + (relgridsize * j) + (int)Math.Abs(Canvas_grid.Viewport.Y)) / EditorGridHeight;
								abscol = ((int)begginning.X + (relgridsize * i) + (int)Math.Abs(Canvas_grid.Viewport.X)) / EditorGridWidth;
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
						if(sl.LayerName == ((SpriteLayer)treeitem.SelectedItem).LayerName)
						{
							return lev.Layers.IndexOf(sl);
						}
					}
				}
			}
			return -1;
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
			Point relpoint = new Point((int)((p.X - Xoff)/ relgridsize), (int)((p.Y - YOff )/ relgridsize));
			relpoint.X *= (relgridsize);
			relpoint.Y *= (relgridsize);

			if (abs) { //return the abs size. Base 40x40 grid.
				return new Point(relpoint.X + Xoff, relpoint.Y + YOff);//this gives us the cell number. Use this and multiply by the base value.
			}
			else //rel grid size
			{
				return new Point();
			}
			

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
      String point = String.Format("({0}, {1}) OFF:({2}, {3})", (int)p.X, (int)p.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
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
					BixBite.BixBiteTypes.CardinalDirection cd = GetDCirectionalMove(p, 0);                //TODO: CHANGE THE Z INDEX TO VARIABLE
					switch (cd)
					{
						//TODO: ADD the logic to change the data positions.
						case (BixBite.BixBiteTypes.CardinalDirection.N):
							for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
							{
								Canvas.SetTop(selectTool.SelectedTiles[i], Canvas.GetTop(selectTool.SelectedTiles[i]) - selectTool.SelectedTiles[i].ActualWidth);
							}
							shiftpoints[2] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
							break;
						case (BixBite.BixBiteTypes.CardinalDirection.S):
							for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
							{
								Canvas.SetTop(selectTool.SelectedTiles[i], Canvas.GetTop(selectTool.SelectedTiles[i]) + selectTool.SelectedTiles[i].ActualWidth);
							}
							shiftpoints[2] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
							break;
						case (BixBite.BixBiteTypes.CardinalDirection.W):
							for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
							{
								Canvas.SetLeft(selectTool.SelectedTiles[i], Canvas.GetLeft(selectTool.SelectedTiles[i]) - selectTool.SelectedTiles[i].ActualWidth);
							}

							shiftpoints[2] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
							break;
						case (BixBite.BixBiteTypes.CardinalDirection.E):
							for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
							{
								Canvas.SetLeft(selectTool.SelectedTiles[i], Canvas.GetLeft(selectTool.SelectedTiles[i]) + selectTool.SelectedTiles[i].ActualWidth);
							}

							shiftpoints[2] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
							break;
					}
				}
				else if (e.LeftButton == MouseButtonState.Pressed && CurrentTool == EditorTool.Select)
				{
					Point pp = GetGridSnapCords(SelectionRectPoints[0]);

					Point Snapped = RelativeGridSnap(p); //If we have then find the bottom right cords of that cell.
					int wid = (int)GetGridSnapCords(p).X - (int)pp.X + 40;
					int heigh = (int)GetGridSnapCords(p).Y - (int)pp.Y + 40;

					//the drawing, and data manuplation will have to occur on LEFTMOUSEBUTTONUP
					Rectangle r = new Rectangle() { Tag = "selection", Width = wid, Height = heigh, Fill = new SolidColorBrush(Color.FromArgb(100, 0, 20, 100)) };
					r.MouseLeftButtonUp += LevelEditor_BackCanvas_MouseLeftButtonUp;
					Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, layertiles: LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(),
																						 zindex: curLayer, x: (int)p.X, y: (int)p.Y);
					Canvas.SetLeft(r, (int)pp.X); Canvas.SetTop(r, (int)pp.Y); Canvas.SetZIndex(r, 100);
					Deselect();

					LevelEditor_Canvas.Children.Add(r);
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
      if (FullMapLEditor_Canvas.Children.Count == 0) return;

      int MainCurCellsX = ((int)Math.Ceiling((LevelEditor_BackCanvas.ActualWidth / Canvas_grid.Viewport.Width)));
      int MainCurCellsY = ((int)Math.Ceiling(LevelEditor_BackCanvas.ActualHeight / Canvas_grid.Viewport.Height));


			FullMapLEditor_Canvas.Children.RemoveAt(0);

			FullMapSelection_Rect = new Rectangle() { Width = MainCurCellsX * 10, Height = MainCurCellsY * 10, Stroke = Brushes.White, StrokeThickness = 1, Name = "SelectionRect" };
      Canvas.SetLeft(FullMapSelection_Rect, 0); Canvas.SetTop(FullMapSelection_Rect, 0);
			Canvas.SetZIndex(FullMapSelection_Rect, 100);
      
      FullMapLEditor_Canvas.Children.Add(FullMapSelection_Rect);

    }

		private void XCellsVal_TB_TextChanged(object sender, TextChangedEventArgs e)
		{
			int numval, pixval = 0;
			if (Int32.TryParse(((TextBox)sender).Text, out numval) && Int32.TryParse(XCellsWidth_TB.Text, out pixval))
			{
				LevelWidth_TB.Text = (numval * pixval).ToString();
			}
			else LevelWidth_TB.Text = "0";
		}

		private void YCellsVal_TB_TextChanged(object sender, TextChangedEventArgs e)
		{
			int numval, pixval = 0;
			if (Int32.TryParse(((TextBox)sender).Text, out numval) && Int32.TryParse(XCellsHeight_TB.Text, out pixval))
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
				int xnum, ynum = 0;
				if (Int32.TryParse(XCellsVal_TB.Text, out xnum))
				{
					if (Int32.TryParse(YCellsVal_TB.Text, out ynum))
					{
						CreateLevel(LName, xnum, ynum);
						//SceneExplorer_TreeView.ItemsSource = OpenLevels;
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
			if (ZoomLevel == .2) return; //do not allow this be 0 which in turn / by 0;

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
			ZoomFactor_TB.Text = String.Format("({0})%  ({1}x{1})" , 100 * ZoomLevel, EditorGridWidth * ZoomLevel);
			scaleFullMapEditor();
		}
    #endregion
    #endregion

    #region "Full Level Canvas"
    private void scaleFullMapEditor()
    {
			Level TempLevel = CurrentLevel;
			if (TempLevel == null) return; //TODO: Remove after i make force select work on tree view.
			FullMapLEditor_Canvas.Width = TempLevel.xCells * 10;
      FullMapLEditor_Canvas.Height = TempLevel.yCells * 10;

      FullMapLEditor_VB.Viewport = new Rect(0, 0, 10, 10);
			
      int MainCurCellsX = (int)Math.Ceiling(LevelEditor_BackCanvas.RenderSize.Width / (40 * ZoomLevel));
      int MainCurCellsY = (int)Math.Ceiling(LevelEditor_BackCanvas.RenderSize.Height / (40 * ZoomLevel));

			double pastx, pasty = 0;
			pastx = Canvas.GetLeft(FullMapSelection_Rect);
			pasty = Canvas.GetTop(FullMapSelection_Rect);

			FullMapLEditor_Canvas.Children.Remove(FullMapSelection_Rect);
			FullMapSelection_Rect = new Rectangle() { Width = MainCurCellsX * 10, Height = MainCurCellsY * 10, Stroke = Brushes.White, StrokeThickness = 1, Name = "SelectionRect"};
      Canvas.SetLeft(FullMapSelection_Rect, pastx);
			Canvas.SetTop(FullMapSelection_Rect, pasty);
			Canvas.SetZIndex(FullMapSelection_Rect, 100);  //100 is the selection layer.
			
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
				((int)p.Y + (int)Math.Abs(Canvas_grid.Viewport.Y)) / EditorGridHeight, //current row
				((int)p.X + (int)Math.Abs(Canvas_grid.Viewport.X)) / EditorGridWidth,  // current column
				int.Parse(((Rectangle)SelectedTile_Canvas.Children[0]).Tag.ToString())); //the value of data 

			Rectangle r = new Rectangle() { Width = 10, Height = 10, Fill = imgtilebrush };

			int setX = (10 * ((int)p.X / EditorGridWidth)) + ((int)GridOffset.X);
			int setY = (10 * ((int)p.Y / EditorGridHeight)) + ((int)GridOffset.Y);

			Canvas.SetLeft(r, setX); Canvas.SetTop(r, setY); Canvas.SetZIndex(r, zindex);
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
			Canvas.SetLeft(r, spr.xpos / 4); Canvas.SetTop(r, spr.ypos / 4); Canvas.SetZIndex(r, zindex); // divide 4 because scaling.
			FullMapLEditor_Canvas.Children.Add(r);
		}

		#region "Property Hot Reloading"

		#endregion
		#endregion

		#region "Tools"
		private void LevelEditorSelection_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Select;
		}
	
		private void LevelEditorBrush_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Brush;
		}

		private void Fill_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Fill;
			int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
			int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
			int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

			columns /= relgridsize;
			rows /= relgridsize;

			Point begginning = GetGridSnapCords(SelectionRectPoints[0]);
			for (int i = 0; i <= columns; i++)
			{
				for (int j = 0; j <= rows; j++)
				{
					Point p = new Point(begginning.X + (int)(40 * i), begginning.Y + (int)(40 * j));
					int iii = GetTileZIndex(SceneExplorer_TreeView);
					Rectangle r = new Rectangle() { Width = 40, Height = 40, Fill = imgtilebrush }; //create the tile that we wish to add to the grid.
					r.MouseLeftButtonUp += LevelEditor_BackCanvas_MouseLeftButtonUp;
					SpriteLayer curlayer = (SpriteLayer)SceneExplorer_TreeView.SelectedValue;
					curlayer.AddToLayer(((int)p.Y + (int)Math.Abs(Canvas_grid.Viewport.Y)) / EditorGridHeight,
						((int)p.X + (int)Math.Abs(Canvas_grid.Viewport.X)) / EditorGridWidth,
						int.Parse(((Rectangle)SelectedTile_Canvas.Children[0]).Tag.ToString()));

					Canvas.SetLeft(r, (int)p.X); Canvas.SetTop(r, (int)p.Y); Canvas.SetZIndex(r, iii); //place the tile position wise
					LevelEditor_Canvas.Children.Add(r); //actual place it on the canvas
					FullMapEditorFill(p, iii); //update the fullmap display to reflect this change
				}
			}
			Deselect();
		}

		private void DownLaverLevelEditor_BTN_Click(object sender, RoutedEventArgs e)
		{
			//we need to make sure that we have selected a sprite layer in the tree view.
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;

			if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
			{
				if (selectTool.SelectedTiles.Count == 0) return;//Do we have a selected area?
																												//is there a layer above the current to transfer the data to?
																												//we need to get the current Sprite layer that is currently clicked.
				int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
				if (curLayer - 1 < 0) return;

				//from here all the prereqs are fulfilled so we can apply the up layer VISUALLY 
				for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
				{
					Canvas.SetZIndex(selectTool.SelectedTiles[i], Canvas.GetZIndex(selectTool.SelectedTiles[i]) - 1);
				}

				//Change the data for the level objects 
				int absrow = 0; int abscol = 0; Point p = GetGridSnapCords(Mouse.GetPosition(LevelEditor_BackCanvas));
				int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
				int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
				int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

				columns /= relgridsize;
				rows /= relgridsize;

				Point begginning = RelativeGridSnap(SelectionRectPoints[0]);
				begginning.X = (int)begginning.X; begginning.Y = (int)begginning.Y;
				for (int i = 0; i <= columns; i++)
				{
					for (int j = 0; j <= rows; j++)
					{
						absrow = ((int)begginning.Y + (relgridsize * j) + (int)Math.Abs(Canvas_grid.Viewport.Y)) / EditorGridHeight;
						abscol = ((int)begginning.X + (relgridsize * i) + (int)Math.Abs(Canvas_grid.Viewport.X)) / EditorGridWidth;
						CurrentLevel.ChangeLayer(absrow, abscol, curLayer, curLayer - 1, LayerType.Tile);
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

		private void UpLaverLevelEditor_BTN_Click(object sender, RoutedEventArgs e)
		{
			//we need to make sure that we have selected a sprite layer in the tree view.
			if (!(SceneExplorer_TreeView.SelectedValue is SpriteLayer)) return;
			if (((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
			{
				if (selectTool.SelectedTiles.Count == 0) return;//Do we have a selected area?
																												//is there a layer above the current to transfer the data to?
																												//we need to get the current Sprite layer that is currently clicked.
				int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
				if (CurrentLevel.Layers.Count - 1 == curLayer) return;

				//from here all the prereqs are fulfilled so we can apply the up layer VISUALLY 
				for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
				{
					Canvas.SetZIndex(selectTool.SelectedTiles[i], Canvas.GetZIndex(selectTool.SelectedTiles[i]) + 1);
				}

				//Change the data for the level objects 
				int absrow = 0; int abscol = 0; Point p = GetGridSnapCords(Mouse.GetPosition(LevelEditor_BackCanvas));
				int relgridsize = (int)(40 * Math.Round(LevelEditor_Canvas.RenderTransform.Value.M11, 1));
				int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
				int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

				columns /= relgridsize;
				rows /= relgridsize;

				Point begginning = RelativeGridSnap(SelectionRectPoints[0]);
				begginning.X = (int)begginning.X; begginning.Y = (int)begginning.Y;
				for (int i = 0; i <= columns; i++)
				{
					for (int j = 0; j <= rows; j++)
					{
						absrow = ((int)begginning.Y + (relgridsize * j) + (int)Math.Abs(Canvas_grid.Viewport.Y)) / EditorGridHeight;
						abscol = ((int)begginning.X + (relgridsize * i) + (int)Math.Abs(Canvas_grid.Viewport.X)) / EditorGridWidth;
						CurrentLevel.ChangeLayer(absrow, abscol, curLayer, curLayer + 1, LayerType.Tile);
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

		private void LevelEditorEraser_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Eraser;

		}

		private void LevelEditorMove_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Move;
		}

		private void SaveLevel_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.Title = "New Level File";
			dlg.FileName = ""; //default file name
			dlg.Filter = "txt files (*.lvl)|*.lvl|All files (*.*)|*.*";
			dlg.FilterIndex = 2;
			dlg.InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Levels");
			dlg.RestoreDirectory = true;

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

			CurrentLevel.ExportLevel(dlg.FileName + (dlg.FileName.Contains(".lvl") ? "" : ".lvl"), TileSetImages, celldim);
		}

		private async void OpenLevel_MenuItem_ClickAsync(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.Title = "New Level File";
			dlg.FileName = ""; //default file name
			dlg.InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Levels");
			dlg.Filter = "txt files (*.xml)|*.xml|All files (*.*)|*.*";
			dlg.FilterIndex = 2;
			dlg.RestoreDirectory = true;

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

			await importLevelAsync(dlg.FileName);

		}

		private void GameEvent_BTN_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Gameevent;
		}

		private void NewLevel_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			NewLevelData_CC.Visibility = Visibility.Visible;
			LevelEditor_BackCanvas.Visibility = Visibility.Hidden;
			LevelEditorStatusBar_Grid.Visibility = Visibility.Hidden;
		}

		private void Deselect()
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

		private void LevelEditorImage_Click(object sender, RoutedEventArgs e)
		{
			CurrentTool = EditorTool.Image;
		}

		#endregion

		//cchanges the toolbar to the currenttool bar depedning on the tilemap tool selected
		private void LevelEditorLibary_TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabControl LELibary_TC = (TabControl)ContentLibrary_Control.Template.FindName("LevelEditorLibary_TabControl", ContentLibrary_Control);
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

							Rectangle r = new Rectangle() { Width = 10, Height = 10, Fill = imgtilebrush };

							Canvas.SetLeft(r, j * 10); Canvas.SetTop(r, i * 10); Canvas.SetZIndex(r, Zindex);
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
						BitmapImage bitmap = new BitmapImage(new Uri(sprite.ImgPathLocation, UriKind.Absolute));
						Image img = new Image(); img.Source = bitmap;
						Rectangle r = new Rectangle() { Width = sprite.Width, Height = sprite.Height, Fill = new ImageBrush(img.Source) };//Make a rectange teh size of the image

						ContentControl CC = ((ContentControl)this.TryFindResource("MoveableImages_Template"));
						CC.Width = sprite.Width;
						CC.Height = sprite.Height;

						Canvas.SetLeft(CC, sprite.xpos); Canvas.SetTop(CC, sprite.ypos); Canvas.SetZIndex(CC, Zindex);
						Selector.SetIsSelected(CC, false);
						CC.MouseRightButtonDown += ContentControl_MouseLeftButtonDown;
						((Rectangle)CC.Content).Fill = new ImageBrush(img.Source);
						LevelEditor_Canvas.Children.Add(CC);
					}
				}
				else if (layer.layerType == LayerType.GameEvent)
				{
					Console.WriteLine("Gameevent");
				}
				Zindex++;
			}
		}

		private void importLevel(String filename)
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
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control)).IsEnabled = true;
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

				int len = pic.UriSource.ToString().LastIndexOf('.') - pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' });
				String Name = pic.UriSource.ToString().Substring(pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' }) + 1, len - 1);

				TileSets_CB.Items.Add(Name);

				//CreateTileMap(tilesetTuples.Item2, tilesetTuples.Item3, tilesetTuples.Item4);
			}

			foreach (Tuple<string, string> t in CurrentLevel.sprites)
			{
				SpriteObjectList.Add(new EditorObject(t.Item2, t.Item1, false));
			}
			ListBox SpriteLibary_LB = (ListBox)ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control);
			SpriteLibary_LB.ItemsSource = null;
			SpriteLibary_LB.ItemsSource = SpriteObjectList;

			TileSets_CB.SelectedIndex = 0;
			//draw the level
			RedrawLevel(CurrentLevel);
			
			//set visabilty. 
			Grid Prob_Grid = (Grid)ContentLibrary_Control.Template.FindName("TileSetProperties_Grid", ContentLibrary_Control);
			Prob_Grid.Visibility = Visibility.Hidden;
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control)).Visibility = Visibility.Visible;
			((ComboBox)ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control)).Visibility = Visibility.Visible;
			((Label)ContentLibrary_Control.Template.FindName("TileSet_LBL", ContentLibrary_Control)).Visibility = Visibility.Visible;
		}

		private async System.Threading.Tasks.Task importLevelAsync(String filename)
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
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control)).IsEnabled = true;
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

				int len = pic.UriSource.ToString().LastIndexOf('.') - pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' });
				String Name = pic.UriSource.ToString().Substring(pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' }) + 1, len - 1);

				TileSets_CB.Items.Add(Name);

				//CreateTileMap(tilesetTuples.Item2, tilesetTuples.Item3, tilesetTuples.Item4);
			}
			TileSets_CB.SelectedIndex = 0;
			//draw the level
			RedrawLevel(CurrentLevel);

			//set visabilty. 
			Grid Prob_Grid = (Grid)ContentLibrary_Control.Template.FindName("TileSetProperties_Grid", ContentLibrary_Control);
			Prob_Grid.Visibility = Visibility.Hidden;
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control)).Visibility = Visibility.Visible;
			((ComboBox)ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control)).Visibility = Visibility.Visible;
			((Label)ContentLibrary_Control.Template.FindName("TileSet_LBL", ContentLibrary_Control)).Visibility = Visibility.Visible;
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

			TempLevel.xCells = XCellsVal;
			TempLevel.yCells = YCellsVal;

			CurrentLevel = TempLevel;

			//change focus:
			OpenLevels.Add(TempLevel);
			SceneExplorer_TreeView.ItemsSource = OpenLevels;
			SceneExplorer_TreeView.Items.Refresh();
			SceneExplorer_TreeView.UpdateLayout();

			NewLevelData_CC.Visibility = Visibility.Hidden;
			LevelEditor_BackCanvas.Visibility = Visibility.Visible;
			LevelEditorStatusBar_Grid.Visibility = Visibility.Visible;
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control)).IsEnabled = true;
			LevelEditor_Canvas.IsEnabled = true;
			ContentLibaryImport_BTN.IsEnabled = true;

			LEditorTS = new List<LevelEditorProp>()
			{
				new LevelEditorProp(){ PropertyName = "Level Name", PropertyData="Default" },
				new LevelEditorProp(){ PropertyName = "Map Width(cells)", PropertyData="30" },
				new LevelEditorProp(){ PropertyName = "Map Height(cells)", PropertyData="50" },
				//new LevelEditorProp("test 2")								
			};
		}
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
			string filename = "";

			//level editor
			if (EditorWindows_TC.SelectedIndex == 0)
			{
				//get the LevelEditor content Libary tab control
				TabControl LELibary_TC = (TabControl)ContentLibrary_Control.Template.FindName("LevelEditorLibary_TabControl", ContentLibrary_Control);
				if (LELibary_TC.SelectedIndex == 0)
				{

					Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
					dlg.FileName = "picture"; //default file 
					dlg.DefaultExt = "*.png"; //default file extension
					dlg.Filter = "png (*.png)|*.png"; //filter files by extension

					// Show save file dialog box
					Nullable<bool> result = dlg.ShowDialog();
					// Process save file dialog box results kicks out if the user doesn't select an item.
					filename = "";
					if (result == true)
						filename = dlg.FileName;
					else
						return;

					Console.WriteLine(filename);

					//show the properties grid for input.
					Grid Prob_Grid = (Grid)ContentLibrary_Control.Template.FindName("TileSetProperties_Grid", ContentLibrary_Control);
					Prob_Grid.Visibility = Visibility.Visible;
					((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control)).Visibility = Visibility.Hidden;
					((ComboBox)ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control)).Visibility = Visibility.Hidden;
					((Label)ContentLibrary_Control.Template.FindName("TileSet_LBL", ContentLibrary_Control)).Visibility = Visibility.Hidden;

					TextBox Name_TB = (TextBox)ContentLibrary_Control.Template.FindName("TileMapName_TB", ContentLibrary_Control);
					Name_TB.Text = filename;
				}
				else if(LELibary_TC.SelectedIndex == 1) //we are in the sprite tab
				{
					Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
					dlg.FileName = "picture"; //default file 
					dlg.DefaultExt = "*.png"; //default file extension
					dlg.Filter = "png (*.png)|*.png"; //filter files by extension

					// Show save file dialog box
					Nullable<bool> result = dlg.ShowDialog();
					// Process save file dialog box results kicks out if the user doesn't select an item.
					filename = "";
					if (result == true)
						filename = dlg.FileName;
					else
						return;

					ListBox SpriteLibary_LB = (ListBox)ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control);

					EditorObject E = new EditorObject(filename, filename.Substring(filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) + 1, filename.LastIndexOf(".") - filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) - 1), false);
					SpriteObjectList.Add(E);
					SpriteLibary_LB.ItemsSource = null;
					SpriteLibary_LB.ItemsSource = SpriteObjectList;

					//add the sprite to level object
					CurrentLevel.sprites.Add(new Tuple<string, string>(filename.Substring(filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) + 1, filename.LastIndexOf(".") - filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) - 1), filename));
				}
			}
      else
      {
				//get the sprite listbox
				TabControl SpriteLibary_LB = (TabControl)ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control);
				
				EditorObject E = new EditorObject(filename, filename.Substring(filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) + 1, filename.LastIndexOf(".") - filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) - 1), false);
        EditorObj_list.Add(E);
				EditorObjects_LB.ItemsSource = null;
				EditorObjects_LB.ItemsSource = EditorObj_list;
      }
    }

		private void CreateTileMap_BTN_Click(object sender, RoutedEventArgs e)
		{
			int Width, Height = 0;

			TextBox Width_TB = (TextBox)ContentLibrary_Control.Template.FindName("TileWidth_TB", ContentLibrary_Control);
			TextBox Height_TB = (TextBox)ContentLibrary_Control.Template.FindName("TileHeight_TB", ContentLibrary_Control);
			TextBox Name_TB = (TextBox)ContentLibrary_Control.Template.FindName("TileMapName_TB", ContentLibrary_Control);
			if (Int32.TryParse(Width_TB.Text, out Width))
			{
				if (Int32.TryParse(Height_TB.Text, out Height))
				{
					if(Width > 0 && Height > 0)
					{
						CreateTileMap(Name_TB.Text, Width, Height);
					}
				}
			}

			//set visabilty. 
			Grid Prob_Grid = (Grid)ContentLibrary_Control.Template.FindName("TileSetProperties_Grid", ContentLibrary_Control);
			Prob_Grid.Visibility = Visibility.Hidden;
			((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control)).Visibility = Visibility.Visible;
			((ComboBox)ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control)).Visibility = Visibility.Visible;
			((Label)ContentLibrary_Control.Template.FindName("TileSet_LBL", ContentLibrary_Control)).Visibility = Visibility.Visible;

		}

		private void CreateTileMap(String FileName, int x, int y)
		{
			
			if (FileName == "") { MessageBox.Show("Filename is invaild!"); return; }
			Image image = new Image();
			var pic = new System.Windows.Media.Imaging.BitmapImage();
			pic.BeginInit();
			pic.UriSource = new Uri(FileName); // url is from the xml
			pic.EndInit();

			System.Drawing.Image img = System.Drawing.Image.FromFile(FileName);
			image.Source = pic;
			image.Width = img.Width;
			image.Height = img.Height;
			//Interaction interaction

			int len = pic.UriSource.ToString().LastIndexOf('.') - pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' });
			String Name= pic.UriSource.ToString().Substring(pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' }) + 1, len - 1);

			CurrentLevel.TileSet.Add(new Tuple<string, string, int, int>(Name, FileName, x, y)); //add the tile set to the current level object.

			TileSets_CB.Items.Add(Name);
			TileSets_CB.SelectedIndex = 0;
		}

		/// <summary>
		/// Displays the Correct Tileset depending on the selected item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TileSetSelector_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(TileSets_CB.SelectedIndex >= 0)
			{

				Canvas TileMap = (Canvas)ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control);
				
				BitmapImage pic = new BitmapImage();
				pic.BeginInit();
				pic.UriSource = new Uri(CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item2);
				pic.EndInit();

				System.Drawing.Image img = System.Drawing.Image.FromFile(CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item2);

				//TODO: MAKE THIS WORK WITH VARIABLE SIZES NOT JUST 32x32
				Image Timg = new Image();
				Timg.Width = img.Width;
				Timg.Height = img.Height;
				Timg.Source = pic;

				TileMap.Children.Clear();
				TileMap.Children.Add(Timg);
				
			}
		}

		#endregion

		#region "Scene Viewer"
		//TODO: make it work with background tile grid sizes.
		private void AddTileLayer_Click(object sender, RoutedEventArgs e)
		{
			Level TempLevel = ((Level)SceneExplorer_TreeView.SelectedValue);
			TempLevel.AddLayer("new tile", LayerType.Tile);
			TempLevel.Layers.Last().DefineLayerDataType(LayerType.Tile, TempLevel.xCells, TempLevel.yCells);
		}
		private void SpriteLayer_Click(object sender, RoutedEventArgs e)
		{
			((Level)SceneExplorer_TreeView.SelectedValue).AddLayer("new sprite", LayerType.Sprite);
		}
		private void GameObjectLayer_Click(object sender, RoutedEventArgs e)
		{
			((Level)SceneExplorer_TreeView.SelectedValue).AddLayer("new GOL", LayerType.GameEvent);
		}
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
		
		public static TreeViewItem FindTviFromObjectRecursive(ItemsControl ic, object o)
		{
			//Search for the object model in first level children (recursively)
			TreeViewItem tvi = ic.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
			if (tvi != null) return tvi;
			//Loop through user object models
			foreach (object i in ic.Items)
			{
				//Get the TreeViewItem associated with the iterated object model
				TreeViewItem tvi2 = ic.ItemContainerGenerator.ContainerFromItem(i) as TreeViewItem;
				tvi = FindTviFromObjectRecursive(tvi2, o);
				if (tvi != null) return tvi;
			}
			return null;
		}
		
		private void SceneExplorer_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			Console.WriteLine("Changed Scene Object");
			if(e.NewValue is Level)
			{
				//set the current level PTR
				CurrentLevel = (Level)e.NewValue;


				LEditorTS = new List<LevelEditorProp>()
				{
					new LevelEditorProp(){ PropertyName = "Level Name", PropertyData=((Level)e.NewValue).LevelName },
					new LevelEditorProp(){ PropertyName = "Map Width(cells)", PropertyData=((Level)e.NewValue).xCells.ToString() },
					new LevelEditorProp(){ PropertyName = "Map Height(cells)", PropertyData=((Level)e.NewValue).yCells.ToString() },
					//new LevelEditorProp("test 2")								
				};
				ListBox LB = ((ListBox)(FullMapGrid_Control.Template.FindName("LEditProperty_LB", FullMapGrid_Control)));
				LB.ItemsSource = null;
				LB.ItemsSource = LEditorTS;

				TileSets_CB.Items.Clear(); //remove the past data.
				foreach (Tuple<String, String, int, int> tilesetTuples in ((Level)e.NewValue).TileSet)
				{
					TileSets_CB.Items.Add(tilesetTuples.Item1);
					//CreateTileMap(tilesetTuples.Item2, tilesetTuples.Item3, tilesetTuples.Item4); //fill in the new data.

					Image image = new Image();
					var pic = new System.Windows.Media.Imaging.BitmapImage();
					pic.BeginInit();
					pic.UriSource = new Uri(tilesetTuples.Item2); // url is from the xml
					pic.EndInit();

					System.Drawing.Image img = System.Drawing.Image.FromFile(tilesetTuples.Item2);
					image.Source = pic;
					image.Width = img.Width;
					image.Height = img.Height;
					//Interaction interaction

					int len = pic.UriSource.ToString().LastIndexOf('.') - pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' });
					String Name = pic.UriSource.ToString().Substring(pic.UriSource.ToString().LastIndexOfAny(new char[] { '/', '\\' }) + 1, len - 1);

					TileSets_CB.SelectedIndex = 0;

				}

			}
				
			if (e.NewValue is SpriteLayer)
			{
				LEditorTS = new List<LevelEditorProp>()
				{
					new LevelEditorProp(){ PropertyName = "SpriteLayer Name:", PropertyData=((SpriteLayer)e.NewValue).LayerName },
					new LevelEditorProp(){ PropertyName = "Layer Type:", PropertyData=((SpriteLayer)e.NewValue).layerType.ToString() },
					//new LevelEditorProp("test 2")								
				};
				ListBox LB = ((ListBox)(FullMapGrid_Control.Template.FindName("LEditProperty_LB", FullMapGrid_Control)));
				LB.ItemsSource = null;
				LB.ItemsSource = LEditorTS;
			}
			

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
					if(SceneExplorer_TreeView.SelectedValue is SpriteLayer)
					{
						//What type of layer?


					}

					//create new sprite layer object data.
				}
			}
		}
		
    private void Test_Click(object sender, RoutedEventArgs e)
		{
			//moving test
			//Canvas.SetLeft(selectTool.SelectedTiles[0], Canvas.GetLeft(selectTool.SelectedTiles[0]) + selectTool.SelectedTiles[0].ActualWidth);

			//layer moving test.
			List<String> TileSetImages = new List<string>();
			foreach (Tuple<String, String, int, int> tilesetstuple in CurrentLevel.TileSet)
			{
				TileSetImages.Add(tilesetstuple.Item2); //URI isn't supported so turn it local!
			}
			//CurrentLevel.ExportLevel("C: \\Users\\Antonio\\Documents\\text.xml", TileSetImages);
			CurrentLevel.Layers[2].AddToLayer("Testing", "",0,0,0,0);

			RedrawLevel(CurrentLevel);

		}
		
		//this is methods that im working now... they suck for now.
		#region "WIP"

		private bool CanUseTool()
		{
			//what tool are you using?
			if (CurrentTool == EditorTool.Brush)
			{

			}
			else if (CurrentTool == EditorTool.Select)
			{

			}
			else if (CurrentTool == EditorTool.Eraser)
			{

			}
			else if (CurrentTool == EditorTool.Fill)
			{

			}
			else if (CurrentTool == EditorTool.Move)
			{

			}

			return false;
		}
		
    /// <summary>
    /// this method is here to determine whether the user has moved to the next cell.
    /// </summary>
    /// <returns>The direction in which they have moved.</returns>
    private BixBite.BixBiteTypes.CardinalDirection GetDCirectionalMove(Point p, int zIndex)
    {
      int relgridsize = (((int)(40 * LevelEditor_Canvas.RenderTransform.Value.M11)));
      //use the current rectange that the user is in to get the (x,y) cords
      //compare these values to the current MOUSE POS
      Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), 0, (int)p.X, (int)p.Y);
      Point snappedpoints = RelativeGridSnap(shiftpoints[2]);
      if (rr != null)
          return BixBite.BixBiteTypes.CardinalDirection.None;
      else
      {
        if (p.X < snappedpoints.X && (p.Y > snappedpoints.Y && p.Y < snappedpoints.Y + relgridsize))
        {  //west
          Console.WriteLine("moved west");
          return BixBite.BixBiteTypes.CardinalDirection.W;
        }
        else if (p.X > snappedpoints.X + relgridsize && (p.Y > snappedpoints.Y && p.Y < snappedpoints.Y + relgridsize))
        {  //east
          Console.WriteLine("moved East");
          return BixBite.BixBiteTypes.CardinalDirection.E;
        }
        else if (p.Y > snappedpoints.Y + relgridsize && (p.X > snappedpoints.X && p.X < snappedpoints.X + relgridsize))
        {  //east
          Console.WriteLine("moved South");
          return BixBite.BixBiteTypes.CardinalDirection.S;
        }
        else if (p.Y < snappedpoints.Y && (p.X > snappedpoints.X && p.X < snappedpoints.X + relgridsize))
        {  //east
          Console.WriteLine("moved North");
          return BixBite.BixBiteTypes.CardinalDirection.N;
        }
      }
      //if now which direction did you move?
      return BixBite.BixBiteTypes.CardinalDirection.None;
    }
		
		#endregion

		private void UnlockMapProperties_CB_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("WIP Not ready yet...");
		}

		private void SearchResultList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//double clicked
			Console.WriteLine("doubleclicked");
		}

		//saves the current project data into the project file (.gem)
		private void SaveCurrentProject_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ResizeRect_MouseLMBTN_Up(object sender, MouseButtonEventArgs e)
		{
			Console.WriteLine("Moved CC Sprite");
		}

		private void DeselectedResizeRect(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Deslected CC Sprite");
		}

		private void ContentControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Console.WriteLine("changed selection CC Sprite");
		}

		private void TxtQuantity_KeyDown(object sender, KeyEventArgs e)
		{
			Console.WriteLine("Text Property Key down " + ((TextBox)sender).Tag.ToString());

		}
	}
}

//NOTES TO MY SELF
/*
 * System.drawing.Image casuses memory leaks
 * RenderTargetBitmap also causes memory leaks
 * 
 * REASON? They are not IDisposable
 * 
 * 
 * 
 */