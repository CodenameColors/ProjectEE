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
    public ImageBrush imgtilebrush { get; private set; }

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

    private void LoadInitalVars()
    {

      PreviewMouseMove += OnPreviewMouseMove;
      //EditorObj_list.Add(new EditorObject(new BitmapImage(new Uri("/WPF_MORE_TESTING;component/images/Ame_icon_small.png", UriKind.RelativeOrAbsolute)), "Left"));


      //EditorObject E = new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png", "Leftt");
      ////E.Thumbnail.Source = new BitmapImage(new Uri("/WPF_MORE_TESTING;component/images/Ame_icon_small.png", UriKind.Relative));

      //EditorObj_list.Add(E);//Thumbnail = new Image() { Source = new BitmapImage(new Uri("images/Ame_icon_small.png", UriKind.Relative)) } });
      //E = new EditorObject("/AmethystEngine;component/images/__pluse_and_minun___by_celsius_ice_rose.jpg", "pluse&minun");
      //EditorObj_list.Add(E);//Thumbnail = new Image() { Source = new BitmapImage(new Uri("images/Ame_icon_small.png", UriKind.Relative)) } });
      //E = new EditorObject("/AmethystEngine;component/images/500px-Mcol_money_bag.svg.png", "Money");
      //EditorObj_list.Add(E);//Thumbnail = new Image() { Source = new BitmapImage(new Uri("images/Ame_icon_small.png", UriKind.Relative)) } });
      //E = new EditorObject("/AmethystEngine;component/images/a50afebd-a3e7-42bb-9c0b-33d394d33334.gif", "sonic");
      //EditorObj_list.Add(E);//Thumbnail = new Image() { Source = new BitmapImage(new Uri("images/Ame_icon_small.png", UriKind.Relative)) } });

      //E = new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png", "Slide 1");
      //Titles.Add(E);

      //Titles = new List<EditorObject>()
      //{
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 1"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 2"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 3"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 4"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 5"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 6"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 7"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 8"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 9"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 10"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 11"),
      //	new EditorObject("/AmethystEngine;component/images/Ame_icon_small.png","Slide 12"),
      //};

      EditorObjects_LB.ItemsSource = EditorObj_list;
      SearchResultList.ItemsSource = Titles;

      //IMGTEST();
    }

    #region "Dynamic Template Binding"

    #endregion

    #region "File/Folder Viewer"
    #region "Folder viewer Tree"

    #endregion

    #region "File Traverse Item View"

    #endregion
    #endregion

    #region "Level Editor"
    #region "Tile Map"

    #endregion

    #region "Main Editor Canvas"
    #region "Panning"

    #endregion
    #region "Zooming"

    #endregion
    #endregion

    #region "Full Level Canvas"
    #region "Property Hot Reloading"

    #endregion
    #endregion

    #region "Tools"

    #endregion
    #endregion

    #region "Content Library"

    #endregion

    #region "Scene Viewer"

    #endregion

    private void ImportButton_E(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("pressed IMPORT");


    }


    private void LoadFileTree(String ProjPath = "")
    {
      String Path = (ProjPath == "" ? getFilePath("Get Root Node", true) : ProjPath);
      if (Path != "")
      {
        ListDirectory(treeView, Path);
      }
    }

    //TODO: Multi lined label
    private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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




    // TODO: set up the TRUE file structure and implement it here.
    private void CreateNewProject(object sender, RoutedEventArgs e)
    {
      Window w = new NewProject_Form(this);
      w.Show();
    }

    private void ImportToEditor(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
      dlg.FileName = "Document"; //default file name
                                 //dlg.DefaultExt = ".xml"; //default file extension
                                 //dlg.Filter = "XML documents (.xml)|*.xml"; //filter files by extension

      // Show save file dialog box
      Nullable<bool> result = dlg.ShowDialog();

      // Process save file dialog box results
      string filename = "";
      if (result == true)
      {
        // Save document
        filename = dlg.FileName;
      }
      else
      {
        return;
      }
      Console.WriteLine(filename);

      if (EditorWindows_TB.SelectedIndex == 0)
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

    private void IMGTEST()
    {
      //Assembly myAssembly = Assembly.GetExecutingAssembly();
      //Stream myStream = myAssembly.GetManifestResourceStream(myAssembly.GetName().Name + "/images/smol megumin.jpg");
      //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(myStream);
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

    private void Desc_CB_Checked(object sender, RoutedEventArgs e)
    {

    }


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

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      Window w = new ProjectSettings();
      w.Show();
    }

    private void gridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
      Console.WriteLine(e.VerticalChange);
      //EditorObjects_LB.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
    }

    private void SearchResultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ((ListBox)sender).ItemsSource = Titles;
      Console.WriteLine("clicked on the thing");

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
          ((TreeViewItem)(((TreeViewItem)(treeView.SelectedItem)).Items[((ListBox)sender).SelectedIndex])).IsSelected = true;
          return;

      }

    }

    private void DirectoryBack_BTN_Click(object sender, RoutedEventArgs e)
    {
      if (((TreeViewItem)treeView.SelectedItem).Parent != null)
        try
        {
          ((TreeViewItem)((TreeViewItem)treeView.SelectedItem).Parent).IsSelected = true;
        }
        catch (InvalidCastException) { return; }
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

    private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      Console.WriteLine("new Height:  " + e.NewSize.Height);
      Console.WriteLine("new Width:  " + e.NewSize.Width);
      //LevelEditor_Canvas.vuew
    }

    private void LevelEditor_Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
      //Console.WriteLine("rect clicked");
      //if (e.MiddleButton == MouseButtonState.Pressed)
      //{
      //	Console.WriteLine("Middle Click");
      //	Canvas_grid.Viewport = new Rect() { X = Canvas_grid.Viewport.X - 40, Y = Canvas_grid.Viewport.Y - 40, Width = EditorGridWidth, Height = EditorGridHeight };
      //	foreach (UIElement child in LevelEditor_Canvas.Children)
      //	{
      //		double x = Canvas.GetLeft(child);
      //		double y = Canvas.GetTop(child);
      //		Canvas.SetLeft(child, x - 40);
      //		Canvas.SetTop(child, y - 40);
      //	}


      //	FullMapLEditor_VB.Viewport = new Rect()
      //	{
      //		X = FullMapLEditor_VB.Viewport.X + 4,
      //		Y = FullMapLEditor_VB.Viewport.Y + 4,
      //		Width = FullMapLEditor_VB.Viewport.Width,
      //		Height = FullMapLEditor_VB.Viewport.Height
      //	};
      //	foreach (UIElement child in FullMapLEditor_Canvas.Children)
      //	{
      //		if (((Rectangle)child).Name == "SelectionRect")
      //		{
      //			double x = Canvas.GetLeft(child);
      //			double y = Canvas.GetTop(child);
      //			Canvas.SetLeft(child, x + 4);
      //			Canvas.SetTop(child, y + 4);
      //			GridOffset.X += 1;
      //			GridOffset.Y += 1;
      //		}
      //	}
      //}
    }

    //this will allow the user to choose from a menu. most likely what type of sprite/background is this tile/sprite.
    private void LevelEditor_BackCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {

    }

    //this method is here to update the size of rectangle on the fullmap on the right.
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

    //quick move of the level editor canvas. when you click the screen renders that section.
    private void EditorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {

      //LevelEditor_Canvas.se
    }
    // ?
    private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
    {

    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      //	Console.WriteLine("tewst");
      //	int fullY = (int)FullMapLEditor_Canvas.ActualHeight; fullY -= fullY % int.Parse(NumOfCellsY);
      //	int fullX = (int)FullMapLEditor_Canvas.ActualWidth; fullX -= fullX % int.Parse(NumOfCellsX);
      //	//LevelEditorFull_Canvas.Width = fullX;
      //	//LevelEditorFull_Canvas.Height = fullY;
      //	//LevelEditorFull_Canvas.ActualHeight
      //	//Canvas_grid.TileMode = TileMode.None;
      //	FullMapLEditor_VB.Viewport = new Rect(0, 0, fullX / (NumOfCellsX), fullY / (NumOfCellsY));
      //	Rectangle r = new Rectangle() { Width = 100, Height = 100, Stroke = Brushes.Black, StrokeThickness = 1 };
      //	TileMapData = new int[(NumOfCellsX), (NumOfCellsY)];
      //	Canvas.SetLeft(r, 0); Canvas.SetTop(r, 0);
      //	FullMapLEditor_Canvas.Children.Add(r);
    }

    private void scaleFullMapEditor()
    {
      FullMapLEditor_Canvas.Width = NumOfCellsX * 2;
      FullMapLEditor_Canvas.Height = NumOfCellsY * 2;

      Console.WriteLine("tewst");
      int fullY = (int)FullMapLEditor_Canvas.Width; fullY -= fullY % (NumOfCellsY);
      int fullX = (int)FullMapLEditor_Canvas.Height; fullX -= fullX % (NumOfCellsX);
      //LevelEditorFull_Canvas.Width = fullX;
      //LevelEditorFull_Canvas.Height = fullY;
      //LevelEditorFull_Canvas.ActualHeight
      //Canvas_grid.TileMode = TileMode.None;

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


    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (imgtilebrush == null)
      {
        Console.WriteLine("no brush selected/tile");
        return;
      }

      //what is the current canvas offset?
      Console.WriteLine(Canvas_grid.Viewport);
      int Xoff = (int)(Math.Abs(Canvas_grid.Viewport.X)) % EditorGridWidth; Xoff = EditorGridWidth - Xoff;
      int YOff = (int)(Math.Abs(Canvas_grid.Viewport.Y)) % EditorGridHeight; YOff = EditorGridHeight - YOff;

      Canvas c = (Canvas)sender;
      Point p = Mouse.GetPosition(LevelEditor_BackCanvas);
      p.X /= ZoomLevel; p.Y /= ZoomLevel;
      p.X -= Math.Floor(p.X - Xoff) % EditorGridWidth;  //TODO: Add the offset so we can fill the grid AFTER PAnNNG
      p.Y -= Math.Floor(p.Y - YOff) % EditorGridHeight;
      Rectangle r = new Rectangle() { Width = 40, Height = 40, Fill = imgtilebrush };
      Canvas.SetLeft(r, (int)p.X); Canvas.SetTop(r, (int)p.Y);
      LevelEditor_Canvas.Children.Add(r);

      //flll the data.
      int fullY = (int)FullMapLEditor_Canvas.ActualHeight; fullY -= fullY % (NumOfCellsY);
      int fullX = (int)FullMapLEditor_Canvas.ActualWidth; fullX -= fullX % (NumOfCellsX);
      fullX = fullX / (NumOfCellsX);
      fullY = fullY / (NumOfCellsY);

      TileMapData[((int)p.X + (int)Math.Abs(Canvas_grid.Viewport.X)) / EditorGridWidth,
        ((int)p.Y + (int)Math.Abs(Canvas_grid.Viewport.Y)) / EditorGridHeight] = 1; //TODO: add offset for correct data
      r = new Rectangle() { Width = 4, Height = 4, Fill = imgtilebrush };

      int setX = (4 * ((int)p.X / EditorGridWidth)) + ((int)GridOffset.X);
      int setY = (4 * ((int)p.Y / EditorGridHeight)) + ((int)GridOffset.Y);

      Canvas.SetLeft(r, setX); Canvas.SetTop(r, setY);
      FullMapLEditor_Canvas.Children.Add(r);
      FullMapCanvasHightlight_rect = r;
    }

    //this method is here to allow pan movement on the main level editor window.
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
      {
        Console.WriteLine("Middle Click");

        foreach (UIElement child in LevelEditor_Canvas.Children)
        {
          double x = Canvas.GetLeft(child);
          double y = Canvas.GetTop(child);
          Canvas.SetLeft(child, x + MPos.X);
          Canvas.SetTop(child, y + MPos.Y);
        }
        Canvas_grid.Viewport = new Rect(Canvas_grid.Viewport.X + MPos.X, Canvas_grid.Viewport.Y + MPos.Y,
          Canvas_grid.Viewport.Width, Canvas_grid.Viewport.Height);
        FullMapLEditor_VB.Viewport = new Rect()
        {
          X = FullMapLEditor_VB.Viewport.X + MPos.X / 10,
          Y = FullMapLEditor_VB.Viewport.Y + MPos.Y / 10,
          Width = FullMapLEditor_VB.Viewport.Width,
          Height = FullMapLEditor_VB.Viewport.Height
        };
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
        GridOffset.X -= MPos.X / 10;
        GridOffset.Y -= MPos.Y / 10;
      }
      MPos = e.GetPosition(LevelEditor_Canvas); //set this for the iteration
    }

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

    //this needs to snap to the grid once we at finished panning
    private void LevelEditor_BackCanvas_MouseUp(object sender, MouseButtonEventArgs e)
    {

    }

    private void EditorWindows_TB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (EditorWindows_TB.SelectedIndex == 0)
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
  }
}
