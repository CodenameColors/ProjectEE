﻿using AmethystEngine.Components;
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
using System.Runtime.Remoting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using BixBite.Resources;
using PropertyGridEditor;
using BixBite.Rendering.UI;
using System.Threading;
using TimelinePlayer.Components;
using BixBite.Characters;
using NodeEditor;
using NodeEditor.Components;
using NodeEditor.Components.Arithmetic;
using NodeEditor.Components.Logic;

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

	public enum NewUITool
	{
		NONE,
		Textbox,
		CheckBox,

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
		public List<EditorObject> ContentLibaryObjList = new List<EditorObject>();
		TreeViewItem CurProjectTreeNode = new TreeViewItem();
		TreeView CurrentProjectTreeView = new TreeView();

		//Fields for the Level Editor
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

		#region UIEditorVars
		NewUITool CurrentNewUI = NewUITool.NONE;
		ContentControl SelectedBaseUIControl;
		ContentControl SelectedUIControl;
		//Dictionary<String, GameUI> OpenUIEdits = new Dictionary<string, GameUI>();
		public ObservableCollection<GameUI> OpenUIEdits { get; set; }
		GameUI SelectedUI;
		Dictionary<String, GameUI> CurrentUIDictionary = new Dictionary<String, GameUI>();
		#endregion

		#region DialogueEditor

		#region Pointers
		ObservableCollection<DialogueScene> ActiveDialogueScenes = new ObservableCollection<DialogueScene>();
		DialogueScene CurActiveDialogueScene;
		TreeView Dialogue_CE_Tree;
		List<Tuple<ContentControl, ContentControl>> CurSceneCharacterDisplays = new List<Tuple<ContentControl, ContentControl>>();
		CollapsedPropertyGrid.CollapsedPropertyGrid CPGrid = new CollapsedPropertyGrid.CollapsedPropertyGrid();
		object DialogueEditorSelectedControl = null;
		#endregion

		#region Vars
		ObservableCollection<object> PropertyBags { get; set; }
		#endregion

		#endregion

		TreeView SceneExplorer_TreeView = new TreeView();
		ListBox EditorObjects_LB = new ListBox();

		EditorTool CurrentTool = EditorTool.None;
		SelectTool selectTool = new SelectTool();

   
    Point MPos = new Point();
    List<String> CMDOutput = new List<string>();    

    String ProjectFilePath = "";
		String MainLevelPath = "";
		String CurrentWorkingDirectory = "";

		ObservablePropertyDictionary EditorObjectProperties = new ObservablePropertyDictionary();

    public EngineEditor()
    {
      InitializeComponent();
			

			LoadInitalVars();
      //LevelEditorMain_Canvas.Background = new DrawingBrush();
    }

		/// <summary>
		/// When the GUI is loaded we need to set the Control pointers
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.MaxHeight = SystemParameters.MaximumWindowTrackHeight;
			PropertyBags = new ObservableCollection<object>();
			//Find and set level editor controls!
      FullMapLEditor_Canvas = (Canvas)ObjectProperties_Control.Template.FindName("LevelEditor_Canvas", ObjectProperties_Control);
      FullMapLEditor_VB = (VisualBrush)ObjectProperties_Control.Template.FindName("FullLeditorGrid_VB", ObjectProperties_Control);
			
			EditorObjects_LB = (ListBox)ContentLibrary_Control.Template.FindName("EditorObjects_LB", ContentLibrary_Control);
      TileMap_Canvas = (Canvas)(ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control));
			TileSets_CB = (ComboBox)(ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control));
			TileMapTiles_Rect = (Rectangle)ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB_Rect", ContentLibrary_Control);
			TileMap_VB = (VisualBrush)ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB", ContentLibrary_Control);
			TileMapGrid_Rect = (Rectangle)ContentLibrary_Control.Template.FindName("TileMapGrid_Rect", ContentLibrary_Control);
			SceneExplorer_TreeView = (TreeView)SceneExplorer_Control.Template.FindName("LESceneExplorer_TreeView", SceneExplorer_Control);

			OpenLevels = new ObservableCollection<Level>();
			OpenUIEdits = new ObservableCollection<GameUI>();

			LoadMainLevel(ProjectFilePath);
    }

		/// <summary>
		/// This is the constructor that is used to load a recent project.
		/// Sets up the Project Explorer
		/// Also set the Currently working directory.
		/// </summary>
		/// <param name="FilePath"></param>
		/// <param name="ProjectName"></param>
		/// <param name="LevelPath"></param>
		public EngineEditor(String FilePath, String ProjectName = "", String LevelPath = "")
    {
      InitializeComponent();
      ProjectFilePath = FilePath;
			
			//we need to read this file. And set the project settings accordingly.
			ProjectName_LBL.Content = ProjectName;
			MainLevelPath = LevelPath;
			LoadInitalVars();
			LoadFileTree(ProjectFilePath.Replace(".gem", "_Game\\Content\\"));
			CurrentWorkingDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\");
    }
		
		/// <summary>
		/// This loaded the main file and searches for the main level path.
		/// then loads it to the screen via ImportLevel();
		/// </summary>
		/// <param name="filepath"></param>
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
						if (ln.Contains("FILL") || ln == "")
							return;
						else
						{
							if (File.Exists(ln))
								ImportLevel(ln);
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
			LESelectRect = new Rectangle() { Tag = "Selection" }; LESelectRect.MouseUp += LevelEditor_BackCanvas_MouseLeftButtonUp;
			EditorObjects_LB.ItemsSource = EditorObj_list;
      SearchResultList.ItemsSource = ContentLibaryObjList;
    }

    /// <summary>
    /// Opens up a file explorer and allows a user to choose a file location.
    /// </summary>
    /// <param name="prompt">What the File Explorer Window will display</param>
    /// <param name="Open">Either a OpenFIleExplorer or a SaveFileExplorer</param>
    /// <returns>File path as a String</returns>
    public static String GetFilePath(String prompt, bool Open = false)
    {
      if (!Open)
      {
				Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
				{
					Title = prompt,
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
        Console.WriteLine(filename);
        return filename;
      }
      else
      {
        // Prepare a dummy string, thos would appear in the dialog
        string dummyFileName = "";

				SaveFileDialog sf = new SaveFileDialog
				{
					// Feed the dummy name to the save dialog
					Title = "OpenFileRoot",
					FileName = "IGNORE THIS" //default file name
				};
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

		/// <summary>
		/// Open up the project settings window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

			if (this.FindName("resizeGrid") is Grid resizeGrid)
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
	      WindowState = WindowState.Normal;
				WindowStyle = WindowStyle.None;
				WindowState = WindowState.Maximized;
				ResizeMode = ResizeMode.NoResize;
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
		/// <summary>
		/// This method is activated when the user presses the Desc button in the Editor Objects section.
		/// It is used to change the list objects from pictures, to pictures with a name caption.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Desc_CB_Click(object sender, RoutedEventArgs e)
		{
			if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level")) //we are in the level editor.
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

		/// <summary>
		/// This method is here to change the templates and pointers when the user wants to change the editors via tab control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditorWindows_TC_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (((TabItem)EditorWindows_TC.SelectedItem).Header == null) { EditorWindows_TC.SelectedIndex = 4; return; }
			if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level"))
      {
        ContentLibrary_Control.Template = (ControlTemplate)this.Resources["LevelEditorTileMap_Template"];
				ObjectProperties_Control.Template = (ControlTemplate)this.Resources["LevelEditorProperty_Template"];
				SceneExplorer_Control.Template = (ControlTemplate)this.Resources["LevelEditorSceneExplorer_Template"];
				EditorToolBar_CC.Template = (ControlTemplate)this.Resources["LevelEditorTileMapToolBar_Template"];


				UpdateLayout(); //update the templates to the ptrs won't be null! :D
				//Find and set level editor controls!
				FullMapLEditor_Canvas = (Canvas)ObjectProperties_Control.Template.FindName("LevelEditor_Canvas", ObjectProperties_Control);
				FullMapLEditor_VB = (VisualBrush)ObjectProperties_Control.Template.FindName("FullLeditorGrid_VB", ObjectProperties_Control);

				EditorObjects_LB = (ListBox)ContentLibrary_Control.Template.FindName("EditorObjects_LB", ContentLibrary_Control);
				TileMap_Canvas = (Canvas)(ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control));
				TileSets_CB = (ComboBox)(ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control));
				TileMapTiles_Rect = (Rectangle)ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB_Rect", ContentLibrary_Control);
				TileMap_VB = (VisualBrush)ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB", ContentLibrary_Control);
				TileMapGrid_Rect = (Rectangle)ContentLibrary_Control.Template.FindName("TileMapGrid_Rect", ContentLibrary_Control);
				SceneExplorer_TreeView = (TreeView)SceneExplorer_Control.Template.FindName("LESceneExplorer_TreeView", SceneExplorer_Control);
				SceneExplorer_TreeView.ItemsSource = OpenLevels;
				SceneExplorer_TreeView.Items.Refresh();
				SceneExplorer_TreeView.UpdateLayout();
			}
			else if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Dialogue"))
			{
				ObjectProperties_Control.Template = (ControlTemplate)this.Resources["DialogueEditorProperty_Template"];
				ContentLibrary_Control.Template = (ControlTemplate)this.Resources["DialogueEditorObjects_Template"];
				EditorToolBar_CC.Template = (ControlTemplate)this.Resources["DialogueEditorObjectExplorer_Template"];
				UpdateLayout();
				if (((CollapsedPropertyGrid.CollapsedPropertyGrid) (ObjectProperties_Control.Template.FindName(
					    "DialoguePropertyGrid", ObjectProperties_Control))).ItemsSource == null)
				{
					try
					{
						((CollapsedPropertyGrid.CollapsedPropertyGrid)(ObjectProperties_Control.Template.FindName("DialoguePropertyGrid", ObjectProperties_Control))).ItemsSource = PropertyBags;
					}
					catch
					{
						((CollapsedPropertyGrid.CollapsedPropertyGrid)(ObjectProperties_Control.Template.FindName("DialoguePropertyGrid", ObjectProperties_Control))).ItemsSource = null;
						((CollapsedPropertyGrid.CollapsedPropertyGrid)(ObjectProperties_Control.Template.FindName("DialoguePropertyGrid", ObjectProperties_Control))).ItemsSource = PropertyBags;
					}
				}

				Dialogue_CE_Tree = (TreeView)ContentLibrary_Control.Template.FindName("DialogueEditor_CE_TV", ContentLibrary_Control);
			}
      else if(((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("UI"))
      {
				ContentLibrary_Control.Template = (ControlTemplate)this.Resources["UIEditorObjects_Template"];
				ObjectProperties_Control.Template = (ControlTemplate)this.Resources["UIEditorProperty_Template"];
				EditorToolBar_CC.Template = (ControlTemplate)this.Resources["UIEditorObjectExplorer_Template"];
				if (SceneExplorer_Control != null)
				{
					ControlTemplate cc = (ControlTemplate)this.Resources["UIEditorSceneExplorer_Template"];
					SceneExplorer_Control.Template = cc;
					Console.WriteLine(SceneExplorer_Control.Template.ToString());
					TreeView tv = (TreeView)cc.FindName("UISceneExplorer_TreeView", SceneExplorer_Control); //(TreeView)SceneExplorer_Control.Template.FindName("UISceneExplorer_TreeView", SceneExplorer_Control);
					if (tv == null) return;
					SceneExplorer_TreeView = tv;
					SceneExplorer_TreeView.ItemsSource = OpenUIEdits;
				}
				

			}
    }

    #endregion

    #region "File/Folder Viewer"
    #region "Folder viewer Tree"
    //TODO: Multi lined label
		/// <summary>
		/// This method is called when the user is traversing through the project tree view 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
    private void ProjectContentExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      String TempPic = "/AmethystEngine;component/images/Ame_icon_small.png";
      CurrentProjectTreeView = (TreeView)sender; ContentLibaryObjList.Clear();
			if (CurrentProjectTreeView.Items.Count == 0) return; 
      ((TreeViewItem)(CurrentProjectTreeView.SelectedItem)).IsExpanded = true;
			CurrentWorkingDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\" + ((TreeViewItem)(CurrentProjectTreeView.SelectedItem)).Header + "\\");
			AmethystEngine.Components.EObjectType EType = EObjectType.File;
      foreach (TreeViewItem tvi in ((TreeViewItem)(CurrentProjectTreeView.SelectedItem)).Items)
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
					ContentLibaryObjList.Add(ed);
				}
        else if(!tvi.Header.ToString().Contains(".")){
					//desimg = TempPic; brel = true;
					EditorObject ed = new EditorObject(desimg, desname, brel, EType);
					ContentLibaryObjList.Add(ed);
				}
				else
				{
					desimg = TempPic; brel = true;
					EditorObject ed = new EditorObject(desimg, desname, brel, EObjectType.File);
					ContentLibaryObjList.Add(ed);
				}

      }
      SearchResultList.ItemsSource = null;
      try
      {
	      SearchResultList.ItemsSource = ContentLibaryObjList;
      }
      catch
      {
	      return;
      }
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
      String Path = (ProjPath == "" ? GetFilePath("Get Root Node", true) : ProjPath);
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

			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				FileName = "assets", //default file 
				Title = "Import New Assest",
				DefaultExt = "All files (*.*)|*.*", //default file extension
				Filter = "Level files (*.lvl)|*.lvl|png file (*.png)|*.png|All files (*.*)|*.*"
			};

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
			destfilename = CurrentWorkingDirectory + importstartlocation.Substring(
				importstartlocation.LastIndexOfAny(new char[] { '\\', '/' }) + 1, len - 1);

			//copy the file
			File.Copy(importfilename, destfilename, true);
			//LoadInitalVars();
			var v = CurrentProjectTreeView.SelectedValuePath;
			LoadFileTree(ProjectFilePath.Replace(".gem", "_Game\\Content\\")); //reload the project to show the new file.
			CurrentProjectTreeView.SelectedValuePath = v;

			ContentLibaryObjList.Add(new EditorObject(destfilename, importstartlocation.Substring(
				importstartlocation.LastIndexOfAny(new char[] { '\\', '/' }) + 1, len - 1), true, EObjectType.File));

			SearchResultList.ItemsSource = null;
			SearchResultList.ItemsSource = ContentLibaryObjList;

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
      ((ListBox)sender).ItemsSource = ContentLibaryObjList;

      int i = (((ListBox)sender).SelectedIndex);
      if (i < 0)
        return;

      switch (((EditorObject)ContentLibaryObjList[i]).EditObjType)
      {
        case (EObjectType.None):
          return;
        case (EObjectType.File):
          return;
        case (EObjectType.Folder):
					CurrentWorkingDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\"+ ((EditorObject)((ListBox)sender).SelectedItem).Name + "\\");
					((TreeViewItem)(((TreeViewItem)(ProjectContentExplorer.SelectedItem)).Items[((ListBox)sender).SelectedIndex])).IsSelected = true;
          return;
      }
    }
    #endregion
    #endregion

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
			xtile = CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item3; ytile = CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item4;

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
				TileSetOffest += (int)((img.Width/ CurrentLevel.TileSet[i].Item3) * (img.Height / CurrentLevel.TileSet[i].Item4));
			}

			TileMapGrid_Rect.Visibility = Visibility.Hidden;
			SelectedTile_Canvas.Children.Clear();
			Point pp = Mouse.GetPosition(TileMap_Canvas);
      pp.X -= Math.Floor(pp.X) % xtile;  //TODO: Add the offset so we can fill the grid AFTER PAnNNG
      pp.Y -= Math.Floor(pp.Y) % ytile;
      int x = (int)pp.X;
      int y = (int)pp.Y;
      Console.WriteLine(String.Format("x: {0},  y: {1}", x, y));
      Console.WriteLine("");

			BitmapImage bmp = new BitmapImage(new Uri(CurrentLevel.TileSet[TileSets_CB.SelectedIndex].Item2));
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

			SelectedTile_Canvas.Children.Add(new Rectangle() { Width = xtile, Height = ytile,
				Fill = Imgtilebrush, Tag = tilenumdata, RenderTransform = new ScaleTransform(.8,.8)});
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
					//Are we allowed to paint?
					if (Canvas_grid.Viewport.X > 0 || Canvas_grid.Viewport.Y > 0)
						return; //we are out of the bounds negative wise (viewport)
					if ((int)CurrentLevel.GetPropertyData("xCells") < p.X / 40 || (int)CurrentLevel.GetPropertyData("yCells") < p.Y / 40)
						return;

					if (SelectedTile_Canvas.Children.Count == 0) return;
					Rectangle r = new Rectangle() { Width = 40, Height = 40, Fill = Imgtilebrush }; //create the tile that we wish to add to the grid. always 40 becasue thats the base. 

					Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), 
						curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
					if (rr != null)
					{
						//If Desired tile is the same as the painted tile. Do nothing
						int x = (int)Canvas.GetTop(rr) / 40;
						int y = (int)Canvas.GetLeft(rr) / 40;
						int arrval = ((int[,])CurrentLevel.Layers[curLayer].LayerObjects)[x,y];
						Console.WriteLine(arrval);
						if (int.Parse(((Rectangle)SelectedTile_Canvas.Children[0]).Tag.ToString()) == arrval)
						{ //the value of data 
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
					Canvas.SetLeft(r, (int)p.X); Canvas.SetTop(r, (int)p.Y); Canvas.SetZIndex(r, curLayer); //place the tile position wise
					LevelEditor_Canvas.Children.Add(r); //actual place it on the canvas

					//add offset to point P to turn rel to Abs pos.
					p.X += Math.Ceiling(Math.Abs(Canvas_grid.Viewport.X));
					p.Y += Math.Ceiling(Math.Abs(Canvas_grid.Viewport.Y));
					p.X = (int)p.X; p.Y = (int)p.Y;
					FullMapEditorFill(p, curLayer); //update the fullmap display to reflect this change
				}
				else if (CurrentTool == EditorTool.Select)
				{
					//find the tile
					if (SceneExplorer_TreeView.SelectedValue is SpriteLayer && ((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.Tile)
					{
						SelectionRectPoints[0] = new Point((int)e.GetPosition(LevelEditor_BackCanvas).X, (int)e.GetPosition(LevelEditor_BackCanvas).Y); //the first point of selection.
					}
					else if(SceneExplorer_TreeView.SelectedValue is SpriteLayer && ((SpriteLayer)SceneExplorer_TreeView.SelectedValue).layerType == LayerType.GameEvent)
					{
						//this selection works similar to tile. But instead it looks for borders not rectangles. It also will retrieve the game event group number
						Rectangle r = new Rectangle() { Tag = "selection", Width = 40, Height = 40, Fill = new SolidColorBrush(Color.FromArgb(100, 0, 20, 100)) };
					}
				}
				else if (CurrentTool == EditorTool.Eraser)
				{
					//find the tile on the layer that is selected.
					Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), 
						curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
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
						Image img = new Image
						{
							Source = bitmap
						};
						Rectangle r = new Rectangle() { Width = currentimg.Width, Height = currentimg.Height, Fill = new ImageBrush(img.Source) };//Make a rectange teh size of the image
						Canvas.SetLeft(r, pos.X); Canvas.SetTop(r, pos.Y); Canvas.SetZIndex(r, curLayer);
						//LevelEditor_Canvas.Children.Add(r);

						ContentControl CC = ((ContentControl)this.TryFindResource("MoveableImages_Template"));
						CC.Width = currentimg.Width;
						CC.Height = currentimg.Height;

						Canvas.SetLeft(CC, pos.X); Canvas.SetTop(CC, pos.Y); Canvas.SetZIndex(CC, curLayer);
						Selector.SetIsSelected(CC, false);
						Selector.AddUnselectedHandler(CC, Sprite_OnUnselected); //an event to call when we un select a sprite!
						CC.MouseRightButtonDown += ContentControl_MouseLeftButtonDown;
						var m = (CC.FindName("ResizeImage_rect"));
						((Rectangle)CC.Content).Fill = new ImageBrush(img.Source);
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
					Border bor = SelectTool.FindBorder(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Border>().ToList(),
						curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
					if (bor != null) return; //the game event is already declared here!

					List<GameEvent> layergameevents = ((Tuple<int[,], List<GameEvent>>)((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerObjects).Item2;
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
						Text = ((int)layergameevents[GameEventNum].GetPropertyData("group") == -1 ? "" : layergameevents[GameEventNum].GetPropertyData("group").ToString()),
						Tag = layergameevents[GameEventNum].GetPropertyData("group").ToString(),
						Foreground = new SolidColorBrush(Colors.Black),
						Background = ((int)layergameevents[GameEventNum].GetPropertyData("group") == -1 ? new SolidColorBrush(Color.FromArgb(100, 255, 0, 0)) : new SolidColorBrush(Color.FromArgb(100, 100, 100, 100))),
					};
					Border b = new Border() { Width = 40, Height = 40 };
					b.Child = tb;

					Canvas.SetLeft(b, (int)p.X); Canvas.SetTop(b, (int)p.Y); Canvas.SetZIndex(b, curLayer); //place the tile position wise
					LevelEditor_Canvas.Children.Add(b); //actual place it on the canvas

					//create event data for the game event.
					
					
					CurrentLevel.Layers[curLayer].AddToLayer((int)layergameevents[GameEventNum].GetPropertyData("group"), (int)p.Y/40, (int)p.X/40, null);
					EventData ed = new EventData()
					{
						newx = 0,
						newy = 0,
						MoveTime = 0,
						NewFileToLoad = ""
					};

					//add the event data. how the palyer will change FROM this event
					//((Tuple<int[,], List<GameEvent>>)((SpriteLayer)CurrentLevel.Layers[curLayer]).LayerObjects).
					//	Item2.Last().AddEventData(ed, "DeleTest1", "NULL");
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

					PropGrid PGrid = ((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
					PGrid.ClearProperties();
					int i = 0;
					foreach (object o in ge.GetProperties().Select(m=>m.Item1))
					{
						if (o is String || o is int)
						{
							PGrid.AddProperty(ge.GetProperties().Select(m=>m.Item2).ToList()[i].ToString(), new TextBox() { IsEnabled = false }, o.ToString(), ge.PropertyCallback);
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

			ContentControl cc = SelectTool.FindSpriteControl(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<ContentControl>().ToList(), 
				curLayer, (int)pos.X, (int)pos.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
			Sprite spr = SelectTool.FindSprite(lsprites, cc);

			if (spr == null) return; //cannot find to return.

			int i = 0;
			//display properties to the properties grid
			PropGrid LB = ((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
			LB.ClearProperties();
			foreach (object o in spr.GetProperties().Select(m=>m.Item2))
			{
				if (o is String || o is int)
				{
					LB.AddProperty(spr.GetProperties().Select(m=>m.Item1).ToList()[i], new TextBox(), o.ToString(), spr.PropertyTBCallback);
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
						int columns = (int)(RelativeGridSnap(SelectionRectPoints[1]).X - RelativeGridSnap(SelectionRectPoints[0]).X);
						int rows = (int)(RelativeGridSnap(SelectionRectPoints[1]).Y - RelativeGridSnap(SelectionRectPoints[0]).Y);

						columns /= relgridsize;
						rows /= relgridsize;

						Point begginning = RelativeGridSnap(SelectionRectPoints[0]);
						begginning.X = (int)begginning.X; begginning.Y = (int)begginning.Y;
						for (int i = 0; i < columns; i++)
						{
							for (int j = 0; j < rows; j++)
							{
								//find the rectangle
								Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(),
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
						for (int i = 0; i < columns; i++)
						{
							for (int j = 0; j < rows; j++)
							{
								absrow = ((int)begginning.Y + (relgridsize * j) + (int)Math.Abs(Canvas_grid.Viewport.Y)) / LEGridHeight;
								abscol = ((int)begginning.X + (relgridsize * i) + (int)Math.Abs(Canvas_grid.Viewport.X)) / LEGridWidth;
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
						if(sl.LayerName == ((SpriteLayer)treeitem.SelectedItem).LayerName)
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

		/// <summary>
		/// Used to get the location to snap to based on the current grid scale. TOP LEFT snap
		/// </summary>
		/// <param name="p"></param>
		/// <param name="abs"></param>
		/// <returns></returns>
		private Point GetGridSnapCords(Point p)
    {
			int Xoff = 0; int YOff = 0;
			if (p.X >= 40 || (int)(Math.Abs(Canvas_grid.Viewport.X)) > 0)
				{ Xoff = (int)(Math.Abs(Canvas_grid.Viewport.X)) % LEGridWidth; Xoff = LEGridWidth - Xoff; } //offset
			if (p.Y >= 40 || (int)(Math.Abs(Canvas_grid.Viewport.Y)) > 0)
				{ YOff = (int)(Math.Abs(Canvas_grid.Viewport.Y)) % LEGridHeight; YOff = LEGridHeight - YOff; }

      p.X /= LEZoomLevel; p.Y /= LEZoomLevel;
      p.X -= Math.Floor(p.X - Xoff) % LEGridWidth;  //TODO: Add the offset so we can fill the grid AFTER PAnNNG
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
					int wid = (int)GetGridSnapCords(p).X - ((int)pp.X);
					int heigh = (int)GetGridSnapCords(p).Y - ((int)pp.Y);

					int[] CurrentPos = { (int)GetGridSnapCords(p).X, (int)GetGridSnapCords(p).Y};

					if (wid >= 0 && heigh < 0) //Quadrant [+ -]
					{
						LESelectRect.Width = CurrentPos[0] - pp.X + 40;
						LESelectRect.Height = pp.Y + 40 - CurrentPos[1];
						Canvas.SetLeft(LESelectRect, pp.X); Canvas.SetTop(LESelectRect, CurrentPos[1]); Canvas.SetZIndex(LESelectRect, 100);
						//SelectionRectPoints[1] = new Point
					}
					else if (wid < 0 && heigh >= 0) //Quadrant [- +]
					{
						LESelectRect.Width = pp.X - CurrentPos[0] + 40; LESelectRect.Height = CurrentPos[1] - pp.Y + 40;
						Canvas.SetLeft(LESelectRect, CurrentPos[0]); Canvas.SetTop(LESelectRect, pp.Y); Canvas.SetZIndex(LESelectRect, 100);
					}
					else if (wid >= 0 && heigh >= 0) //Quadrant [+ +]
					{
						LESelectRect.Width = CurrentPos[0] - pp.X + 40; LESelectRect.Height = CurrentPos[1] - pp.Y + 40;
						Canvas.SetLeft(LESelectRect, pp.X); Canvas.SetTop(LESelectRect, pp.Y); Canvas.SetZIndex(LESelectRect, 100);
					}
					else //Quadrant [- -]
					{
						LESelectRect.Width = (wid * -1) +40; LESelectRect.Height = (heigh *-1) +40;
						Canvas.SetLeft(LESelectRect, CurrentPos[0]); Canvas.SetTop(LESelectRect, CurrentPos[1]); Canvas.SetZIndex(LESelectRect, 100);
					}



					Console.WriteLine(String.Format("W:{0}, H{1}", wid, heigh));

					//the drawing, and data manuplation will have to occur on LEFTMOUSEBUTTONUP
					LESelectRect.Fill = new SolidColorBrush(Color.FromArgb(100, 0, 20, 100));
					LevelEditor_Canvas.Children.Remove(LESelectRect); LevelEditor_Canvas.Children.Add(LESelectRect);
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

			FullMapSelection_Rect = new Rectangle() { Width = MainCurCellsX * 10, Height = MainCurCellsY * 10, Stroke = Brushes.White, StrokeThickness = 1, Name = "SelectionRect" };
      Canvas.SetLeft(FullMapSelection_Rect, 0); Canvas.SetTop(FullMapSelection_Rect, 0);
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
			if (Int32.TryParse(((TextBox)sender).Text, out int numval) && Int32.TryParse(XCellsHeight_TB.Text, out int pixval))
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
          Canvas.SetTop(child, y - MPos.Y / 10);  //SCALE use the ratio to pan and link both accurately

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
      else  //zoom out!
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

			ZoomFactor_TB.Text = String.Format("({0})%  ({1}x{1})" , 100 * LEZoomLevel, LEGridWidth * LEZoomLevel);
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
				((int)p.Y + (int)Math.Abs(Canvas_grid.Viewport.Y)) / LEGridHeight, //current row
				((int)p.X + (int)Math.Abs(Canvas_grid.Viewport.X)) / LEGridWidth,  // current column
				int.Parse(((Rectangle)SelectedTile_Canvas.Children[0]).Tag.ToString())); //the value of data 

			Rectangle r = new Rectangle() { Width = 10, Height = 10, Fill = Imgtilebrush };

			int setX = (10 * (((int)p.X / LEGridWidth)));
			int setY = (10 * (((int)p.Y / LEGridHeight)));

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
			Canvas.SetLeft(r, (int)spr.GetPropertyData("x") / 4); Canvas.SetTop(r, (int)spr.GetPropertyData("y") / 4); Canvas.SetZIndex(r, zindex); // divide 4 because scaling.
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
				if (SceneExplorer_TreeView.SelectedItem is SpriteLayer && ((SpriteLayer)SceneExplorer_TreeView.SelectedItem).layerType == LayerType.Sprite)
					SetSpriteHitState(true);
			}
			if (SceneExplorer_TreeView.SelectedItem is SpriteLayer && ((SpriteLayer)SceneExplorer_TreeView.SelectedItem).layerType == LayerType.Sprite)
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
					if ((int)CurrentLevel.GetPropertyData("xCells")-1 <= i || (int)CurrentLevel.GetPropertyData("yCells")-1 <= j)
						break;

					Point p = new Point(begginning.X + (int)(40 * i), begginning.Y + (int)(40 * j));
					int iii = GetTileZIndex(SceneExplorer_TreeView);
					Rectangle r = new Rectangle() { Width = 40, Height = 40, Fill = Imgtilebrush }; //create the tile that we wish to add to the grid.
					r.MouseLeftButtonUp += LevelEditor_BackCanvas_MouseLeftButtonUp;
					SpriteLayer curlayer = (SpriteLayer)SceneExplorer_TreeView.SelectedValue;
					curlayer.AddToLayer(((int)p.Y + (int)Math.Abs(Canvas_grid.Viewport.Y)) / LEGridHeight,
						((int)p.X + (int)Math.Abs(Canvas_grid.Viewport.X)) / LEGridWidth,
						int.Parse(((Rectangle)SelectedTile_Canvas.Children[0]).Tag.ToString()));

					Canvas.SetLeft(r, (int)p.X); Canvas.SetTop(r, (int)p.Y); Canvas.SetZIndex(r, iii); //place the tile position wise
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
				if (selectTool.SelectedTiles.Count == 0) return;//Do we have a selected area?
																												//is there a layer above the current to transfer the data to?
																												//we need to get the current Sprite layer that is currently clicked.
				int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
				if (curLayer - 1 < 0) return;

				//ONLY move this data to a layer with the same Layer type
				int deslayer = -1;
				for (int i = curLayer - 1; i >= 0; i--)
				{
					if (CurrentLevel.Layers[curLayer].layerType == CurrentLevel.Layers[i].layerType)
					{ deslayer = i; break; } // we found a matching layer
				}
				if (deslayer == -1) return; //no layer found

				//from here all the prereqs are fulfilled so we can apply the up layer VISUALLY 
				for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
				{
					Canvas.SetZIndex(selectTool.SelectedTiles[i], deslayer);
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
				if (selectTool.SelectedTiles.Count == 0) return;//Do we have a selected area?
				int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
				if (CurrentLevel.Layers.Count - 1 == curLayer) return; //last layer.

				//ONLY move this data to a layer with the same Layer type
				int deslayer = -1;
				for (int i = curLayer +1; i < CurrentLevel.Layers.Count; i++)
				{
					if (CurrentLevel.Layers[curLayer].layerType == CurrentLevel.Layers[i].layerType)
						{ deslayer = i; break; } // we found a matching layer
				}
				if (deslayer == -1) return; //no layer found

				//from here all the prereqs are fulfilled so we can apply the up layer VISUALLY 
				for (int i = 0; i < selectTool.SelectedTiles.Count; i++)
				{
					Canvas.SetZIndex(selectTool.SelectedTiles[i], deslayer);
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
			if(selectTool.SelectedTiles.Count != 0)
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

			CurrentLevel.ExportLevel(dlg.FileName + (dlg.FileName.Contains(".lvl") ? "" : ".lvl"), TileSetImages, celldim);
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
					if(fe is ContentControl)
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
					if ((int)CurrentLevel.GetPropertyData("xCells") - 1 <= i || (int)CurrentLevel.GetPropertyData("yCells") - 1 <= j)
						break;
					//find the tile on the layer that is selected.
					Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), 
						curLayer, (int)begginning.X + (i*40), (int)begginning.Y + (j * 40),
						(int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
					if (rr == null) continue; //if you click on a empty rect return
																	//find the rect in the current layers displayed tiles
					int k = LevelEditor_Canvas.Children.IndexOf(rr);
					int x = -1; int y = -1;
					if (k >= 0)
					{
						y = (int)Canvas.GetLeft(rr) / 40;
						x = (int)Canvas.GetTop(rr) / 40;
						LevelEditor_Canvas.Children.RemoveAt(k); //delete it.
					}
					if (x < 0 || y < 0) { Console.WriteLine("desired cell to delete DNE"); continue; }
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
				List<GameEvent> layergameevents = ((Tuple<int[,], List<GameEvent>>)((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerObjects).Item2;
				MenuItem MI = ((MenuItem)(EditorToolBar_CC.Template.FindName("DeclareGameEvent_MI", EditorToolBar_CC)));
				MI.Items.Clear(); int i = 0;
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

		/// <summary>
		/// This method is activated when the user selects a different object in the scene explorer.
		/// It will print out properties to the properties grid, and change displayed level if needed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SceneExplorer_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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

				int i = 0;
				PropGrid LB = ((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
				LB.ClearProperties();
				foreach (object o in ((Level)e.NewValue).GetProperties().Select(m => m.Item2))
				{
					if (o is String || o is int)
					{
						LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(), ((Level)e.NewValue).PropertyTBCallback);
					}
					else if (o is bool)
					{
						LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new CheckBox(), (bool)o, ((Level)e.NewValue).PropertyCheckBoxCallback);
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
				PropGrid LB = ((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
				SpriteLayer SL = (SpriteLayer)e.NewValue;
				LB.ClearProperties();
				foreach (object o in SL.GetProperties().Select(m=>m.Item2))
				{
					if (o is String || o is int)
					{
						LB.AddProperty(SL.GetProperties().Select(m=>m.Item1).ToList()[i], new TextBox(), o.ToString(), SL.PropertyTBCallback);
					}
					else if (o is bool)
					{
						LB.AddProperty(SL.GetProperties().Select(m=>m.Item1).ToList()[i], new CheckBox(), (bool)o, SL.PropertyCheckBoxCallback);
					}

					i++;
				}

				if (SL.layerType == LayerType.Sprite)
					SetSpriteHitState(false);
				else
				{ SetSpriteHitState(false); DeselectSprites(); }


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
						Image img = new Image
						{
							Source = bitmap
						};
						Rectangle r = new Rectangle() { Width = (int)sprite.GetPropertyData("width"), Height = (int)sprite.GetPropertyData("height"), Fill = new ImageBrush(img.Source) };//Make a rectange the size of the image

						ContentControl CC = ((ContentControl)this.TryFindResource("MoveableImages_Template"));
						CC.Width = (int)sprite.GetPropertyData("width");
						CC.Height = (int)sprite.GetPropertyData("height");

						Canvas.SetLeft(CC, (int)sprite.GetPropertyData("x")); Canvas.SetTop(CC, (int)sprite.GetPropertyData("y")); Canvas.SetZIndex(CC, Zindex);
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
							if(griddata[i,j] != 0)
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
									Background = (griddata[i, j] != -1 ? new SolidColorBrush(Color.FromArgb(100, 100, 100, 100)) : new SolidColorBrush(Color.FromArgb(100, 255, 0, 0))),
								};
								Border b = new Border() { Width = 40, Height = 40 };
								b.Child = tb;

								Canvas.SetLeft(b, j*40); Canvas.SetTop(b, i*40); Canvas.SetZIndex(b, Zindex); //place the tile position wise
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
					LESpriteObjectList.Add(new EditorObject(t.Item2, t.Item1, false));
				}
				ListBox SpriteLibary_LB = (ListBox)ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control);
				SpriteLibary_LB.ItemsSource = null;
				SpriteLibary_LB.ItemsSource = LESpriteObjectList;

				TileSets_CB.SelectedIndex = 0;
				//draw the level
				RedrawLevel(CurrentLevel);

				//set visabilty. 
				Grid Prob_Grid = (Grid)ContentLibrary_Control.Template.FindName("TileSetProperties_Grid", ContentLibrary_Control);
				Prob_Grid.Visibility = Visibility.Hidden;
				((ScrollViewer)ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control)).Visibility = Visibility.Visible;
				((ComboBox)ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control)).Visibility = Visibility.Visible;
				((Label)ContentLibrary_Control.Template.FindName("TileSet_LBL", ContentLibrary_Control)).Visibility = Visibility.Visible;


				int i = 0;
				PropGrid LB = ((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
				foreach (object o in CurrentLevel.GetProperties().Select(m => m.Item2))
				{
					if (o is String || o is int)
					{
						LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(), CurrentLevel.PropertyTBCallback);
					}
					else if (o is bool)
					{
						LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new CheckBox(), (bool)o, CurrentLevel.PropertyCheckBoxCallback);
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

			int i = 0;
			PropGrid LB = ((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
			foreach (object o in CurrentLevel.GetProperties().Select(m => m.Item2))
			{
				if (o is String || o is int)
				{
					LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(), CurrentLevel.PropertyTBCallback);
				}
				else if (o is bool)
				{
					LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new CheckBox(), (bool)o, CurrentLevel.PropertyCheckBoxCallback);
				}

				i++;
			}
			//CurrentLevel.setProperties(((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary);
			//((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary = CurrentLevel.getProperties();


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

			TempLevel.SetProperty("xCells", XCellsVal);
			TempLevel.SetProperty("yCells",YCellsVal);

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

			int i = 0;
			PropGrid LB = ((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
			foreach (object o in CurrentLevel.GetProperties().Select(m => m.Item2))
			{
				if (o is String || o is int)
				{
					LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new TextBox(), o.ToString(), CurrentLevel.PropertyTBCallback);
				}
				else if (o is bool)
				{
					LB.AddProperty(CurrentLevel.GetProperties().Select(m => m.Item1).ToList()[i], new CheckBox(), (bool)o, CurrentLevel.PropertyCheckBoxCallback);
				}

				i++;
			}
			//CurrentLevel.setProperties(((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary);
			//((PropGrid)ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)).PropDictionary = CurrentLevel.getProperties();

			LevelEditor_Canvas.Children.Clear();
			FullMapLEditor_Canvas.Children.Clear();
		}
		#endregion // end of Level editor region
		
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
			if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level"))
			{
				//get the LevelEditor content Libary tab control
				TabControl LELibary_TC = (TabControl)ContentLibrary_Control.Template.FindName("LevelEditorLibary_TabControl", ContentLibrary_Control);
				if (LELibary_TC.SelectedIndex == 0)
				{

					Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
					{
						FileName = "picture", //default file 
						DefaultExt = "*.png", //default file extension
						Filter = "png (*.png)|*.png" //filter files by extension
					};

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
					Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
					{
						FileName = "picture", //default file 
						DefaultExt = "*.png", //default file extension
						Filter = "png (*.png)|*.png" //filter files by extension
					};

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
					LESpriteObjectList.Add(E);
					SpriteLibary_LB.ItemsSource = null;
					SpriteLibary_LB.ItemsSource = LESpriteObjectList;

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
			int Height = 0;

			TextBox Width_TB = (TextBox)ContentLibrary_Control.Template.FindName("TileWidth_TB", ContentLibrary_Control);
			TextBox Height_TB = (TextBox)ContentLibrary_Control.Template.FindName("TileHeight_TB", ContentLibrary_Control);
			TextBox Name_TB = (TextBox)ContentLibrary_Control.Template.FindName("TileMapName_TB", ContentLibrary_Control);
			if (Int32.TryParse(Width_TB.Text, out int Width))
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
				Image Timg = new Image
				{
					Width = img.Width,
					Height = img.Height,
					Source = pic
				};

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
			TempLevel.Layers.Last().DefineLayerDataType(LayerType.Tile, (int)TempLevel.GetPropertyData("xCells"), (int)TempLevel.GetPropertyData("yCells"));
		}
		private void SpriteLayer_Click(object sender, RoutedEventArgs e)
		{
			Level TempLevel = ((Level)SceneExplorer_TreeView.SelectedValue);
			TempLevel.AddLayer("new sprite", LayerType.Sprite);
			TempLevel.Layers.Last().DefineLayerDataType(LayerType.Sprite);
		}
		private void GameObjectLayer_Click(object sender, RoutedEventArgs e)
		{
			Level TempLevel = ((Level)SceneExplorer_TreeView.SelectedValue);
			TempLevel.AddLayer("new GOL", LayerType.GameEvent);
			TempLevel.Layers.Last().DefineLayerDataType(LayerType.GameEvent, (int)TempLevel.GetPropertyData("xCells"), (int)TempLevel.GetPropertyData("yCells"));


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

		#region "WIP"
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
			if (ic.ItemContainerGenerator.ContainerFromItem(o) is TreeViewItem tvi) return tvi;
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

		private void SceneViewAdd_BTN_Click(object sender, RoutedEventArgs e)
		{
			//what editor are we currently in?
			if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level"))  //Level Editor
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
		}
		
		//this is methods that im working now... they suck for now.
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
			int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer)SceneExplorer_TreeView.SelectedValue).LayerName);
			int relgridsize = (((int)(40 * LevelEditor_Canvas.RenderTransform.Value.M11)));
      //use the current rectange that the user is in to get the (x,y) cords
      //compare these values to the current MOUSE POS
      Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(), curLayer,
				(int)p.X, (int)p.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
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

		private void TxtQuantity_KeyDown(object sender, KeyEventArgs e)
		{
			Console.WriteLine("Text Property Key down " + ((TextBox)sender).Tag.ToString());

		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameTestRun_BTN_Click(object sender, RoutedEventArgs e)
		{

		}

		private void LevelEditorTIleMap_SV_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			Console.WriteLine(String.Format("VertOff: {0},  HoriOff: {1}", e.VerticalOffset, e.HorizontalOffset));

			TileMapScrollVals.X = e.HorizontalOffset;
			TileMapScrollVals.Y = e.VerticalOffset;

		}

		private void Sprite_OnUnselected(object sender, RoutedEventArgs e)
		{
			//Item.Content = ((ListBoxItem)sender).Name + " was unselected.";
			Console.WriteLine("Sprite Was un selected.");

		}
		#endregion

		/// <summary>
		/// this button will auto generate and MSBuild the games project files to allow game evnent use. IF code compiles w/no errors
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CodeCompiling_BTN_Click(object sender, RoutedEventArgs e)
		{
			Dictionary<String, List<GameEvent>> ProjGE = Cuprite.GetProjectGameEvents(ProjectFilePath);
			List<List<String>> codelines = new List<List<string>>();
			foreach (String s in ProjGE.Keys)
			{
				foreach (GameEvent ge in ProjGE[s])
					codelines.Add(Cuprite.GetMethodTemplate(ge));
			}

			System.Collections.Generic.IEnumerable<String> l = File.ReadLines(Cuprite.GetFilePath(ProjectFilePath));
			List<String> lines = l.ToList();
			int i = 0; int j = 0;
			foreach (String Key in ProjGE.Keys)
			{
				foreach (List<String> linesofcode in codelines)
				{
					if (ProjGE[Key].Count == 0) break;
					String Tests = ProjGE[Key][i++].GetPropertyData("DelegateEventName").ToString();
					Cuprite.GenerateMethod(codelines[j++], ref lines, Tests, Key);
					if (i > ProjGE[Key].Count - 1)
						break;
				}
				i = 0;
			}

			using (StreamWriter writer = new StreamWriter(Cuprite.GetFilePath(ProjectFilePath), false))
			{
				foreach (String s in lines)
					writer.WriteLine(s);
			}
			Cuprite.BuildGameProjectFiles(ProjectFilePath);

		}

		//These are methods that have to deal with the UI editor tools
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
			String point = String.Format("({0}, {1}) OFF:({2}, {3})", (int)p.X, (int)p.Y, (int)Canvas_grid.Viewport.X, (int)Canvas_grid.Viewport.Y);
			LevelEditorCords_TB.Text = point;

			//which way is mouse moving?
			MPos -= (Vector)e.GetPosition(LevelEditor_Canvas);

			//is the middle mouse button down?
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				//LevelEditorPan();
			}

		}

		/// <summary>
		/// This method is called when a moveable image is clicked on. (SPRITES)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContentControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level"))
			{
				if (CurrentTool == EditorTool.Select)
				{
					if (LESelectedSpriteControl != null) Selector.SetIsSelected(LESelectedSpriteControl, false);
					LESelectedSpriteControl = (ContentControl)sender;
					Selector.SetIsSelected(LESelectedSpriteControl, true);

					LESelectedSprite = SelectTool.FindSprite(((List<Sprite>)((SpriteLayer)SceneExplorer_TreeView.SelectedItem).LayerObjects), LESelectedSpriteControl);

					Point point = new Point(Canvas.GetLeft(LESelectedSpriteControl), Canvas.GetTop(LESelectedSpriteControl));
					Console.WriteLine(point.ToString());
				}
			}
			else if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("UI"))
			{
				if (SelectedUIControl != null) Selector.SetIsSelected(SelectedUIControl, false);
				SelectedUIControl = (ContentControl)sender;
				Selector.SetIsSelected(SelectedUIControl, true);

				if (SelectedUIControl.Tag.ToString() == "Border")
					SelectedBaseUIControl = (ContentControl)sender;
				SelectedUI = CurrentUIDictionary[((Control)sender).Name];
				int i = 0;
				PropGrid LB = ((PropGrid)(ObjectProperties_Control.Template.FindName("UIPropertyGrid", ObjectProperties_Control)));
				LB.ClearProperties();
				foreach (object o in CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item2))
				{
					if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] == "Zindex")
					{
						TextBox TB = new TextBox(); TB.KeyDown += GameUI_ZIndex_Changed;
						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i], TB,
							o.ToString(), CurrentUIDictionary[SelectedUIControl.Name].PropertyCallbackTB);
					}
					else if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] == "ShowBorder")
					{
						CheckBox CB = new CheckBox() { VerticalAlignment = VerticalAlignment.Center };
						CB.Click += SetBorderVisibility;
						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i],
							CB,
							o);
					}
					else if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] == "Image")
					{
						ComboBox CB = new ComboBox() { Height = 50, ItemTemplate = (DataTemplate)this.Resources["CBIMGItems"] };
						CB.SelectionChanged += GameImage_SelectionChanged;
						List<EditorObject> ComboItems = new List<EditorObject>();
						foreach (String filepath in GetAllProjectImages())
						{
							ComboItems.Add(new EditorObject(filepath, filepath.Substring(filepath.LastIndexOfAny(new char[] { '\\', '/' })), false));
						}
						CB.ItemsSource = ComboItems;

						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i],
							CB,
							new List<String>());
					}
					else if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] == "Background")
					{
						DropDownCustomColorPicker.CustomColorPicker TB = new DropDownCustomColorPicker.CustomColorPicker();
						TB.SelectedColorChanged += customCP_BackgroundColorChanged;
						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i], TB,
							o.ToString());
					}
					else if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] == "FontColor")
					{
						DropDownCustomColorPicker.CustomColorPicker TB = new DropDownCustomColorPicker.CustomColorPicker();
						TB.SelectedColorChanged += customCP_FontColorChanged;
						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i], TB,
							o.ToString());
					}
					else
					{
						TextBox TB = new TextBox(); TB.KeyDown += UIPropertyCallback;
						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i], TB,
							o.ToString(), CurrentUIDictionary[SelectedUIControl.Name].PropertyCallbackTB);
					}
					i++;
				}
			}
			else if (((TabItem)EditorWindows_TC.SelectedItem).Header.ToString().Contains("Dialogue"))
			{
				//if (SelectedUIControl != null) Selector.SetIsSelected(((ContentControl)sender), false);
				SelectedUIControl = (ContentControl)sender;
				Selector.SetIsSelected(SelectedUIControl, true);
			}
		}

		/// <summary>
		/// This method is here when a movable image has been clicked and let go. (SPRITES)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContentControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (CurrentTool == EditorTool.Select)
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

			((Grid)CC.Content).Children.Add(new Border() { BorderThickness = new Thickness(2), BorderBrush = Brushes.Gray, Background = Brushes.Transparent, IsHitTestVisible = false });
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


			int i = 1; String name = "NewTextBox";
			while (CurrentUIDictionary.ContainsKey(name))
			{
				name += i++;
			}
			CC.Name = name;
			UIEditor_Canvas.Children.Add(CC);
			CurrentUIDictionary.Add(name, new GameTextBlock(name, 50, 50, 0, 0, 1));
			OpenUIEdits[0].AddUIElement(CurrentUIDictionary.Values.Last()); //TODO: use the selection Treeview
		}

		/// <summary>
		/// This method will add a Image to the UI editor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UIEditoGameIMG_BTN_Click(object sender, RoutedEventArgs e)
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
			((Grid)CC.Content).Children.Add(new Border() { BorderThickness = new Thickness(2), BorderBrush = Brushes.Gray, IsHitTestVisible = false });
			((Grid)CC.Content).Children.Add(new Rectangle() { });
			((Grid)CC.Content).Children.Add(img);
			
			int i = 1; String name = "NewImgBox";
			while (CurrentUIDictionary.ContainsKey(name))
			{
				name += i++;
			}
			CC.Name = name;
			UIEditor_Canvas.Children.Add(CC);
			CurrentUIDictionary.Add(name, new GameIMG(name, 50, 50, 1, 0, 0));
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
			for(int i = 0; i < g.RowDefinitions.Count; i++)
			{
				for(int j = 0; j < g.ColumnDefinitions.Count; j++)
				{
					Image img = new Image() { IsHitTestVisible = false, Stretch = Stretch.Fill };
					Grid.SetRow(img, i); Grid.SetColumn(img, j);
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
					((TextBox)((Grid)SelectedUIControl.Content).Children[((Grid)SelectedUIControl.Content).Children.Count-1]).FontSize =
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

			if (SelectedUI is GameIMG)
			{
				SelectedUI.SetProperty("Image", ((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
			}

			if (SelectedUIControl.Tag.ToString() == "TEXTBOX")
			{
				//this sets it display wise
				SetTBBackgroundImage(((Grid)SelectedUIControl.Content), ((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
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
				double width = 0; double height = 0;
				double x = 0; double y = 0;

				BitmapImage bmp = new BitmapImage(new Uri(IMGPath));

				if (vert != 1) { height = 90;  }
				else
				{
					height = bmp.Height-180; y = 90;
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

		/// <summary>
		/// change the Z index of the UI object.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameUI_ZIndex_Changed(object sender, KeyEventArgs e)
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
		/// This method is here for when the user clicks and drags a GameUI object to move.
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
					if (SelectedBaseUIControl != null && (SelectedUI is GameTextBlock || SelectedUI is GameIMG))
					{
						Vector RelOrigin = new Vector((int)Canvas.GetLeft(SelectedBaseUIControl), (int)Canvas.GetTop(SelectedBaseUIControl));
						Vector ControlPos = new Vector((int)Canvas.GetLeft(SelectedUIControl), (int)Canvas.GetTop(SelectedUIControl));
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

			OpenUIEdits.Add(new GameUI("NewUITool", 50, 50, 1));
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

			GameUI g = GameUI.ImportGameUI(dlg.FileName);
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
		public ContentControl DrawUIToScreen(Canvas CurrentEditorCanvas, Canvas CurrentEditorCanvas_Back, GameUI gameUI, bool bcomps, String DesiredImageName_CC)
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
			((Grid)BaseUI.Content).Children.Add(new Border() { BorderThickness = new Thickness(2), BorderBrush = (((bool)gameUI.GetPropertyData("ShowBorder")) ? Brushes.Gray : Brushes.Transparent) });

			//get the middle!
			Point mid = new Point(CurrentEditorCanvas_Back.ActualWidth / 2, CurrentEditorCanvas_Back.ActualHeight / 2);
			//get the true center point. Since origin is top left.
			mid.X -= BaseUI.Width / 2; mid.Y -= BaseUI.Height / 2;
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

					CurrentUIDictionary.Add(childUI.UIName, childUI);
				}
				CurrentUIDictionary.Add(OpenUIEdits.Last().UIName, OpenUIEdits.Last());

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
					}
					else if (childUI is GameIMG)
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
			Point mid = new Point(CurrentEditorCanvas_Back.ActualWidth / 2, CurrentEditorCanvas_Back.ActualHeight/2);
			//get the true center point. Since origin is top left.
			mid.X -= BaseUI.Width / 2; mid.Y -= BaseUI.Height / 2;
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

					CurrentUIDictionary.Add(childUI.UIName, childUI);
				}
				CurrentUIDictionary.Add(OpenUIEdits.Last().UIName, OpenUIEdits.Last());
				
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
					}
					else if (childUI is GameIMG)
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
						((Grid)BaseUI.Content).Children.Add(CUI);
					}
					else if (childUI is GameButton)
					{

					}
				}
			}
			SelectedBaseUIControl = BaseUI;
			SelectedUI = OpenUIEdits.Last();
			CurrentEditorCanvas.Children.Add(BaseUI);
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
//CurSceneCharacterDisplays[timelineind].Item2

			if (hori == HorizontalAlignment.Left && vert == VerticalAlignment.Top)
			{
				Canvas.SetLeft(CurSceneCharacterDisplays[timelineind].Item2, 0);
				Canvas.SetTop(CurSceneCharacterDisplays[timelineind].Item2, 0);

				CurSceneCharacterDisplays[timelineind].Item2.HorizontalAlignment = HorizontalAlignment.Left;
				CurSceneCharacterDisplays[timelineind].Item2.VerticalAlignment = VerticalAlignment.Top;

				//for file data
				CurActiveDialogueScene.Characters[timelineind].HorizontalAnchor =
					CurSceneCharacterDisplays[timelineind].Item2.HorizontalAlignment.ToString();
				CurActiveDialogueScene.Characters[timelineind].VerticalAnchor =
					CurSceneCharacterDisplays[timelineind].Item2.VerticalAlignment.ToString();
			}
			else if (hori == HorizontalAlignment.Right && vert == VerticalAlignment.Top)
			{
				Canvas.SetLeft(CurSceneCharacterDisplays[timelineind].Item2, gridw - CurSceneCharacterDisplays[timelineind].Item2.Width);
				Canvas.SetTop(CurSceneCharacterDisplays[timelineind].Item2, 0);

				CurSceneCharacterDisplays[timelineind].Item2.HorizontalAlignment = HorizontalAlignment.Right;
				CurSceneCharacterDisplays[timelineind].Item2.VerticalAlignment = VerticalAlignment.Top;

				//for file data
				CurActiveDialogueScene.Characters[timelineind].HorizontalAnchor =
					CurSceneCharacterDisplays[timelineind].Item2.HorizontalAlignment.ToString();
				CurActiveDialogueScene.Characters[timelineind].VerticalAnchor =
					CurSceneCharacterDisplays[timelineind].Item2.VerticalAlignment.ToString();
			}
			else if (hori == HorizontalAlignment.Left && vert == VerticalAlignment.Bottom)
			{
				Canvas.SetLeft(CurSceneCharacterDisplays[timelineind].Item2, 0);
				Canvas.SetTop(CurSceneCharacterDisplays[timelineind].Item2, gridh - CurSceneCharacterDisplays[timelineind].Item2.Height);

				CurSceneCharacterDisplays[timelineind].Item2.HorizontalAlignment = HorizontalAlignment.Left;
				CurSceneCharacterDisplays[timelineind].Item2.VerticalAlignment = VerticalAlignment.Bottom;

				//for file data
				CurActiveDialogueScene.Characters[timelineind].HorizontalAnchor =
					CurSceneCharacterDisplays[timelineind].Item2.HorizontalAlignment.ToString();
				CurActiveDialogueScene.Characters[timelineind].VerticalAnchor =
					CurSceneCharacterDisplays[timelineind].Item2.VerticalAlignment.ToString();
			}
			else if (hori == HorizontalAlignment.Right && vert == VerticalAlignment.Bottom)
			{
				Canvas.SetLeft(CurSceneCharacterDisplays[timelineind].Item2, gridw - CurSceneCharacterDisplays[timelineind].Item2.Width);
				Canvas.SetTop(CurSceneCharacterDisplays[timelineind].Item2, gridh - CurSceneCharacterDisplays[timelineind].Item2.Height);

				CurSceneCharacterDisplays[timelineind].Item2.HorizontalAlignment = HorizontalAlignment.Right;
				CurSceneCharacterDisplays[timelineind].Item2.VerticalAlignment = VerticalAlignment.Bottom;
				
				//for file data
				CurActiveDialogueScene.Characters[timelineind].HorizontalAnchor =
					CurSceneCharacterDisplays[timelineind].Item2.HorizontalAlignment.ToString();
				CurActiveDialogueScene.Characters[timelineind].VerticalAnchor =
					CurSceneCharacterDisplays[timelineind].Item2.VerticalAlignment.ToString();
			}
		}

		private void DialogueEditor_BackCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			double gridw = DialogueEditor_BackCanvas.ActualWidth;
			double gridh = DialogueEditor_BackCanvas.ActualHeight;
			foreach (Tuple<ContentControl, ContentControl> GameUICC in CurSceneCharacterDisplays)
			{
				HorizontalAlignment hori = GameUICC.Item2.HorizontalAlignment;
				VerticalAlignment vert = GameUICC.Item2.VerticalAlignment;
				

				if (hori == HorizontalAlignment.Left && vert == VerticalAlignment.Top)
				{
					Canvas.SetLeft(GameUICC.Item2, 0);
					Canvas.SetTop(GameUICC.Item2, 0);
				}
				else if (hori == HorizontalAlignment.Right && vert == VerticalAlignment.Top)
				{
					Canvas.SetLeft(GameUICC.Item2,
						gridw - GameUICC.Item2.ActualWidth);
					Canvas.SetTop(GameUICC.Item2, 0);
				}
				else if (hori == HorizontalAlignment.Left && vert == VerticalAlignment.Bottom)
				{
					Canvas.SetLeft(GameUICC.Item2, 0);
					Canvas.SetTop(GameUICC.Item2,
						gridh - GameUICC.Item2.ActualHeight);
				}
				else if (hori == HorizontalAlignment.Right && vert == VerticalAlignment.Bottom)
				{
					Canvas.SetLeft(GameUICC.Item2,
						gridw - GameUICC.Item2.ActualWidth);
					Canvas.SetTop(GameUICC.Item2,
						gridh - GameUICC.Item2.ActualHeight);
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
			
			CollapsedPropertyGrid.CollapsedPropertyGrid LB = ((CollapsedPropertyGrid.CollapsedPropertyGrid)(ObjectProperties_Control.Template.FindName("DialoguePropertyGrid", ObjectProperties_Control)));
			if (sender is null) return;
			else if(sender is TimeBlock)
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
						new Tuple<string, object, Control>("Type", "Time Block", new TextBox() {IsEnabled = false}));
				}
				catch { return; }

				TextBox startime_TB = new TextBox(); startime_TB.KeyDown += DE_SetStartTime;
				timeblockPropertyBag.Properties.Add(new Tuple<string, object, Control>("Start Time", TimeB.StartTime.ToString(), startime_TB));
				TextBox endtime_TB = new TextBox(); endtime_TB.KeyDown += DE_SetEndTime;
				timeblockPropertyBag.Properties.Add(new Tuple<string, object, Control>("End Time", TimeB.EndTime.ToString(), endtime_TB));
				TextBox durationime_TB = new TextBox(); durationime_TB.KeyDown += DE_SetDurationTime;
				timeblockPropertyBag.Properties.Add(new Tuple<string, object, Control>("Duration", TimeB.Duration.ToString(), durationime_TB));

				int i = 0;
				foreach (String s in (TimeB.LinkedDialogueBlock as DialogueNodeBlock)?.DialogueTextOptions)
				{
					ComboBox CB = new ComboBox() {Height = 50, ItemTemplate = (DataTemplate) this.Resources["CBIMGItems"]};
					CB.SelectionChanged += SetSpriteImagePath_Dia;
					List<EditorObject> ComboItems = new List<EditorObject>();
					foreach (Sprite filepath in CurActiveDialogueScene
						.Characters[DialogueEditor_Timeline.GetTimelinePosition(((TimeBlock) sender).TimelineParent)]
						.DialogueSprites)
					{
						try
						{
							ComboItems.Add(new EditorObject(filepath.ImgPathLocation,
								filepath.ImgPathLocation.Substring(filepath.ImgPathLocation.LastIndexOfAny(new char[] {'\\', '/'})),
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
					if(index >= 0) CB.SelectedIndex = index;

					dialoguePropertyBag.Properties.Add(
						new Tuple<string, object, Control>("Sprite Image "+i, new List<String>(), CB));
					TextBox tb = new TextBox();
					tb.KeyDown += SetDialogueText;
					dialoguePropertyBag.Properties.Add(new Tuple<string, object, Control>("Dialogue Text "+i,
						s, tb));

					ComboBox CB1 = new ComboBox() {Height = 50};
					CB1.SelectionChanged += SetLinkedTBName;
					List<String> ComboItems1 = new List<String>();
					foreach (GameUI Gameui in CurActiveDialogueScene
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
					dialoguePropertyBag.Properties.Add(new Tuple<string, object, Control>("Linked TextBox "+i, new List<String>(), CB1));
					i++;
				}
			}
			else if(sender is DialogueNodeBlock dialogueNodeBlock)
			{
				DialogueEditorSelectedControl = sender;
				PropertyBag timeblockPropertyBag = new PropertyBag(sender);
				timeblockPropertyBag.Name = "Time Block Properties";
				PropertyBag dialoguePropertyBag = new PropertyBag(sender);
				dialoguePropertyBag.Name = "Dialogue Data";

				TimeBlock TimeB = dialogueNodeBlock.LinkedTimeBlock as TimeBlock;
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
						new Tuple<string, object, Control>("Type", "Dialogue Block", new TextBox() {IsEnabled = false}));
				}
				catch
				{
					return;
				}

				TextBox startime_TB = new TextBox(); startime_TB.KeyDown += DE_SetStartTime;
				timeblockPropertyBag.Properties.Add(new Tuple<string, object, Control>("Start Time", TimeB.StartTime.ToString(), startime_TB));
				TextBox endtime_TB = new TextBox(); endtime_TB.KeyDown += DE_SetEndTime;
				timeblockPropertyBag.Properties.Add(new Tuple<string, object, Control>("End Time", TimeB.EndTime.ToString(), endtime_TB));
				TextBox durationime_TB = new TextBox(); durationime_TB.KeyDown += DE_SetDurationTime;
				timeblockPropertyBag.Properties.Add(new Tuple<string, object, Control>("Duration", TimeB.Duration.ToString(), durationime_TB));

				int i = 0;
				foreach (String s in dialogueNodeBlock.DialogueTextOptions)
				{
					ComboBox CB = new ComboBox() { Height = 50, ItemTemplate = (DataTemplate)this.Resources["CBIMGItems"] };
					CB.SelectionChanged += SetSpriteImagePath_Dia;
					List<EditorObject> ComboItems = new List<EditorObject>();
					foreach (Sprite filepath in CurActiveDialogueScene.Characters[DialogueEditor_Timeline.GetTimelinePosition(null, TimeB)].DialogueSprites)
					{
						try
						{
							ComboItems.Add(new EditorObject(filepath.ImgPathLocation,
								filepath.ImgPathLocation.Substring(filepath.ImgPathLocation.LastIndexOfAny(new char[] {'\\', '/'})),
								false));
						}
						catch (ArgumentException ae){
							Console.WriteLine(ae.Message);
							continue; }
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
					foreach (GameUI Gameui in CurActiveDialogueScene
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
					dialoguePropertyBag.Properties.Add(new Tuple<string, object, Control>("Linked TextBox " + i, new List<String>(), CB1));
					i++;
				}
			}
			else if(sender is Timeline)
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
				else if (DialogueEditorSelectedControl is DialogueNodeBlock dialogue)
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
				else if (DialogueEditorSelectedControl is DialogueNodeBlock dialogue)
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
				else if (DialogueEditorSelectedControl is DialogueNodeBlock dialogue)
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
				if(DialogueEditorSelectedControl is TimeBlock timeBlock)
				{
					(timeBlock.LinkedDialogueBlock as DialogueNodeBlock).DialogueTextOptions[(Grid.GetRow(sender as TextBox)) / 3] = (sender as TextBox).Text; //set the dialogue data
					timeBlock.CurrentDialogue = (sender as TextBox).Text; //set the timeblock data
				}
				else if(DialogueEditorSelectedControl is DialogueNodeBlock dialogue)
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
			else if (DialogueEditorSelectedControl is DialogueNodeBlock dialogue)
			{
				(dialogue.LinkedTimeBlock as TimeBlock).TrackSpritePath = (((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath);
				(dialogue.LinkedTimeBlock as TimeBlock).Trackname = dialogue.Header;
				dialogue.DialogueSprites.Add(new Sprite(
					((EditorObject)((ComboBox)sender).SelectedValue).Name,
					((EditorObject)((ComboBox)sender).SelectedValue).Thumbnail.AbsolutePath, 
					0,0,0,0));
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
			if(DialogueEditor_Timeline.SelectedControl is Timeline)
			{

			}
			else if (DialogueEditorSelectedControl is TimeBlock timeBlock)
			{
				timeBlock.LinkedTextBoxName = ((ComboBox)sender).SelectedItem.ToString();
				//
				//((TimeBlock)sender).
			}
			else if (DialogueEditorSelectedControl is DialogueNodeBlock dialogue)
			{
				(dialogue.LinkedTimeBlock as TimeBlock).LinkedTextBoxName = (sender as ComboBox).SelectedItem.ToString();
			}
		}

		/// <summary>
		/// When adding a character to scene it will call this method. AFTER correct info is given.
		/// This method right now will add a sprite, that is can be linked to change, and a UI object to the screen.
		/// </summary>
		/// <param name="c"></param>
		public void AddCharacterHook(SceneCharacter c, GameUI gameUi, String GameFileUI, String LinkedTextboxName, String LinkedDialogueImage_Text = null)
		{
			CurActiveDialogueScene.Characters.Add(c);
			DialogueEditor_Timeline.AddTimeline(c.Name);

			if (LinkedDialogueImage_Text == null)
			{
				//add moveable scaleable sprite control.
				ContentControl CC = ((ContentControl) this.TryFindResource("MoveableControls_Template"));
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
				((Grid) CC.Content).Children.Add(new Border()
					{BorderThickness = new Thickness(2), BorderBrush = Brushes.Gray, IsHitTestVisible = false});
				((Grid) CC.Content).Children.Add(new Rectangle() { });
				((Grid) CC.Content).Children.Add(img);

				DialogueEditore_Canvas.Children.Add(CC);
				CurActiveDialogueScene.Characters.Last().DialogueSprites.Add(
					new Sprite(img.Source.ToString(), img.Source.ToString(), 0, 0, (int) img.ActualWidth, (int) ActualHeight));

				OpenUIEdits.Add(gameUi);
				DrawUIToScreen(DialogueEditore_Canvas, DialogueEditor_BackCanvas, OpenUIEdits.Last(), false);
				CurActiveDialogueScene.DialogueBoxes.Add(OpenUIEdits.Last());
				CurActiveDialogueScene.DialogueBoxesFilePaths.Add(GameFileUI);

				//create the pointers to the content controls. which is what displays my images to the screen
				CurSceneCharacterDisplays.Add(new Tuple<ContentControl, ContentControl>(CC, SelectedBaseUIControl));
			}
			else
			{
				OpenUIEdits.Add(gameUi);
				ContentControl CC= DrawUIToScreen(DialogueEditore_Canvas, DialogueEditor_BackCanvas, OpenUIEdits.Last(), false, LinkedDialogueImage_Text);
				Image img = (CC.Content as Grid).Children[2] as Image;

				CurActiveDialogueScene.Characters[CurActiveDialogueScene.Characters.IndexOf(c)].DialogueSprites.Add(
					new Sprite(img.Source.ToString(), img.Source.ToString(), 0, 0, (int)img.ActualWidth, (int)ActualHeight));
				CurActiveDialogueScene.Characters.Last().LinkedImageBox = LinkedDialogueImage_Text;


				CurActiveDialogueScene.DialogueBoxes.Add(OpenUIEdits.Last());
				CurActiveDialogueScene.DialogueBoxesFilePaths.Add(GameFileUI);

				//create the pointers to the content controls. which is what displays my images to the screen
				CurSceneCharacterDisplays.Add(new Tuple<ContentControl, ContentControl>(CC, SelectedBaseUIControl));
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
			object t =  DialogueEditor_Timeline.GetTimelines()[0];
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

			SceneCharacter c = new SceneCharacter(HorizontalAlignment.Left.ToString(), VerticalAlignment.Bottom.ToString());
			Window w = new AddCharacterForm(ProjectFilePath) { AddToScene = AddCharacterHook};
			w.ShowDialog();

			DialogueEditor_NodeGraph.SceneCharacters_list.Add(CurActiveDialogueScene.Characters.Last().Name);

			//CurActiveDialogueScene.Characters.Add(c);
			//DialogueEditor_Timeline.AddTimeline(c.Name);

		}

		/// <summary>
		/// This method occurs AS the timeline is playing and the timeline has "entered" a new time block for that row.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ActiveTBblocks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
				{//this means we need to display the choices to the user.

					//first hide the current UI controls.
					for (int i = 0; i < CurSceneCharacterDisplays.Count; i++)
					{
						CurSceneCharacterDisplays[i].Item1.Visibility = Visibility.Hidden;
						CurSceneCharacterDisplays[i].Item2.Visibility = Visibility.Hidden;
						Selector.SetIsSelected(CurSceneCharacterDisplays[i].Item2, false);
					}

					//FOR NOW display a list box of the options the user will choose from.
					//TODO: change this to the GamelistBox after this is created
					ListBox lb = new ListBox();
					lb.VerticalAlignment = VerticalAlignment.Bottom;
					lb.HorizontalAlignment = HorizontalAlignment.Stretch;
					foreach(String datachoices in (choiceTimeBlock.LinkedDialogueBlock as DialogueNodeBlock).DialogueTextOptions)
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
						((Image)((Grid)CurSceneCharacterDisplays[
									DialogueEditor_Timeline.GetTimelinePosition(((TimeBlock)e.NewItems[0]).TimelineParent)].Item1.Content)
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

					GameUI gtb = null;
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

					gtb.SetProperty("ContentText", ((((TimeBlock)e.NewItems[0]).LinkedDialogueBlock) as DialogueNodeBlock).DialogueTextOptions[0]);

					UIElementCollection uie = ((Grid)CurSceneCharacterDisplays[DialogueEditor_Timeline.GetTimelinePosition(((TimeBlock)e.NewItems[0]).TimelineParent)].Item2.Content).Children;
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
							((TextBox)c).Text = gtb.GetPropertyData("ContentText").ToString();
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
			for (int i = 0; i < CurSceneCharacterDisplays.Count; i++)
			{
				CurSceneCharacterDisplays[i].Item1.Visibility = Visibility.Visible;
				CurSceneCharacterDisplays[i].Item2.Visibility = Visibility.Visible;
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

		private void NodeEditorCurrentErrorsOnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
			Sprite s = new Sprite("new sprite", "/AmethystEngine;component/images/Ame_icon_small.png",0 , 0, 400, 400);
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
			DialogueEditor_Timeline.GetTimelines()[0].timeBlocksLL.First.Value.TrackSpritePath = "/AmethystEngine;component/images/Ame_icon_small.png";
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
				PropGrid LB = ((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));

			}
			else if (sender is Timeline)
			{
				Timeline tl = (Timeline)sender;
				PropGrid LB = ((PropGrid)(ObjectProperties_Control.Template.FindName("Properties_Grid", ObjectProperties_Control)));
			}
			else Console.WriteLine("Unsupported type"); 

		}

		public object CreateDialogueBlockForTimeline(object Timeblock, bool bChoices)
		{
			int i = DialogueEditor_Timeline.GetTimelinePosition(DialogueEditor_Timeline.selectedTimeline, null);
			DialogueNodeBlock dialogueNode = new DialogueNodeBlock(CurActiveDialogueScene.Characters[i].Name);
			DialogueEditor_NodeGraph.AddDialogueBlockToGraph(dialogueNode, CurActiveDialogueScene.Characters[i].Name, Timeblock);

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
			TimeBlock timeBlock = new TimeBlock(DialogueEditor_Timeline.GetTimelines()[0], 0);
			timeBlock.Trackname = (DialogueBlock as DialogueNodeBlock)?.Header;

			//DialogueEditor_NodeGraph.AddDialogueBlockToGraph(timeBlock, CurActiveDialogueScene.Characters[i].Name, DialogueBlock);
			(DialogueBlock as DialogueNodeBlock).LinkedTimeBlock = timeBlock;
			timeBlock.LinkedDialogueBlock = (DialogueBlock as DialogueNodeBlock);
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
			BaseNodeBlock currentBlock = DialogueEditor_NodeGraph.StartExecutionBlock;
			AddTimeBlocksTillExit(ref retLL, currentBlock);
			
			return retLL;
		}

		private LinkedList<TimeBlock> GetChoiceTimeBlocks_LL(BaseNodeBlock currentBlock, int choice)
		{
			if (currentBlock.OutputNodes[choice].ConnectedNodes[0] != null &&
			    currentBlock.OutputNodes[choice].ConnectedNodes[0].ParentBlock is ExitBlockNode) return null;
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
			{ return retLL;}

			return retLL;
		}

		private void AddTimeBlocksTillExit(ref LinkedList<TimeBlock> output, BaseNodeBlock currentBlock)
		{
			//add until exit is found.
			try
			{
				while (!(currentBlock is ExitBlockNode))
				{
					if (currentBlock is StartBlockNode start)
					{
						currentBlock = start.ExitNode.ConnectedNodes[0].ParentBlock;
					}
					else if (currentBlock is DialogueNodeBlock dialogue)
					{
						if (!output.Contains(dialogue.LinkedTimeBlock as TimeBlock))
							output.AddLast(dialogue.LinkedTimeBlock as TimeBlock);
						currentBlock = dialogue.OutputNodes[0].ConnectedNodes[0].ParentBlock;
					}
					else if (currentBlock is SetConstantNodeBlock setConstant)
					{
						currentBlock = setConstant.ExitNode.ConnectedNodes[0].ParentBlock;
					}
					else if (currentBlock is ConditionalNodeBlock conditional)
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
			List<String> chars =new List<string>();
			foreach (SceneCharacter character in CurActiveDialogueScene.Characters)
			{
				chars.Add(character.Name);
			}
			DialogueEditor_Timeline.AddTimeblock_LL(desiredLL, chars);
		}

		private void DisplayTimeBlocksAfter_LL(LinkedList<TimeBlock> desiredLL)
		{
			List<String> chars = new List<string>();
			foreach (SceneCharacter character in CurActiveDialogueScene.Characters)
			{
				chars.Add(character.Name);
			}
			DialogueEditor_Timeline.AddTimeblock_LL(desiredLL, chars, false);

			foreach (Timeline timeline in DialogueEditor_Timeline.GetTimelines())
			{
				if (timeline.TimelineisNull_flag)
				{ timeline.ActiveBlock = timeline.timeBlocksLL.First; }
				else if (timeline.ActiveBlock != null)
					timeline.ActiveBlock = timeline.ActiveBlock.Next;
				else if (timeline.ActiveBlock == null)
					timeline.ActiveBlock = timeline.timeBlocksLL.First;
				timeline.TimelineisNull_flag = false;
			}

		}

		private void LoadTimelineWithSelectedPath(object selectedblock)
		{
			if (!(selectedblock is BaseNodeBlock)) return;
			DialogueEditor_Timeline.DeleteAllTimeBlocks(); //delete all timeline data

			//next up use the current block and build the new list from that.
			BaseNodeBlock tempblock = selectedblock as BaseNodeBlock;
			LinkedList<TimeBlock> timeBlocks = new LinkedList<TimeBlock>();

			AddTimeblocksTillStart(ref timeBlocks, tempblock);
			AddTimeBlocksTillExit(ref timeBlocks, tempblock);

			DisplayTimeBlocks_LL(timeBlocks);
		}

		private void AddTimeblocksTillStart(ref LinkedList<TimeBlock> output, BaseNodeBlock tempblock)
		{
			try
			{ 
				while (!(tempblock is StartBlockNode))
				{
					//back track and add.
					if (tempblock is ExitBlockNode exit)
					{
						tempblock = exit.EntryNode.ConnectedNodes[0].ParentBlock;
					}
					else if (tempblock is DialogueNodeBlock dialogue)
					{
						tempblock = dialogue.EntryNode.ConnectedNodes[0].ParentBlock;
						output.AddFirst(dialogue.LinkedTimeBlock as TimeBlock);
					}
					else if (tempblock is SetConstantNodeBlock setConstant)
					{
						tempblock = setConstant.EntryNode.ConnectedNodes[0].ParentBlock;
					}
					else if (tempblock is ConditionalNodeBlock conditional)
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

			List<BaseNodeBlock> BlockNodes = new List<BaseNodeBlock>();
			foreach (UIElement uie in DialogueEditor_NodeGraph.NodeCanvas.Children)
			{
				if (uie is BaseNodeBlock bn)
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

			List<Tuple<String, String, int, String ,String, int>> connectiList = new List<Tuple<string, String, int, String, string, int>>();

			DialogueScene dia = DialogueScene.ImportScene(dlg.FileName, ref connectiList);
			
			foreach (Timeline tl in dia.Timelines)
			{
				tl.TrackImagePath = dia.Characters[charcnt++].DialogueSprites[0].ImgPathLocation;
				DialogueEditor_Timeline.AddTimeline(tl);
			}
			//this is an import function so we need to remove the DEFAULT start and End Block.
			//AND SET THE DialogueNodegraph.Start ref.
			DialogueEditor_NodeGraph.NodeCanvas.Children.Clear();
			foreach (BaseNodeBlock bn in dia.DialogueBlockNodes)
			{
				if (bn is StartBlockNode start)
					DialogueEditor_NodeGraph.StartExecutionBlock = start;
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
				CurActiveDialogueScene.DialogueBoxes.Add(GameUI.ImportGameUI(CurActiveDialogueScene.DialogueBoxesFilePaths[i]));
				DialogueEditor_NodeGraph.SceneCharacters_list.Add(CurActiveDialogueScene.Characters[i].Name);
			}

			Console.WriteLine(Canvas.GetLeft(dia.DialogueBlockNodes[dia.DialogueBlockNodes.Count-1]));


			DialogueEditor_NodeGraph.NodeCanvas.UpdateLayout();
			//connect all the blocks that we just added.
			foreach (Tuple<String, String, int, String , String, int> tup in connectiList)
			{
				try
				{
					BaseNodeBlock fromBlock = dia.DialogueBlockNodes.Find(x => x.Name == tup.Item1);
					ConnectionNode fromBlockNode = GetFromConnectionNode(fromBlock, tup);


					BaseNodeBlock toBlock = dia.DialogueBlockNodes.Find(x => x.Name == tup.Item4);
					ConnectionNode toBlockNode = GetToConnectionNode(toBlock, tup);

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
			foreach (GameUI gameUi in dia.DialogueBoxes)
			{
				OpenUIEdits.Add(gameUi);
				ContentControl CC = DrawUIToScreen(DialogueEditore_Canvas, DialogueEditor_BackCanvas, OpenUIEdits.Last(), false, CurActiveDialogueScene.Characters[charcnt].LinkedImageBox );

				//(DialogueEditor_Grid.Children[DialogueEditor_Grid.Children.Count - 1] as ContentControl).HorizontalAlignment =
				//	HorizontalAlignment.Right;
				//(DialogueEditor_Grid.Children[DialogueEditor_Grid.Children.Count - 1] as ContentControl).VerticalAlignment= VerticalAlignment.Bottom;
				//DrawUIToScreen(DialogueEditore_Canvas, DialogueEditor_BackCanvas, OpenUIEdits.Last(), false);

				//create the pointers to the content controls. which is what displays my images to the screen
				CurSceneCharacterDisplays.Add(new Tuple<ContentControl, ContentControl>(CC, SelectedBaseUIControl));

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

		private ConnectionNode GetFromConnectionNode(BaseNodeBlock fromBlock, Tuple<String, String, int, String, String, int> tup)
		{
			ConnectionNode fromBlockNode = null;
			if (fromBlock is StartBlockNode start)//|| fromBlock is ExitBlockNode)
			{
				fromBlockNode = start.ExitNode;
			}
			else if (fromBlock is BaseArithmeticBlock arth)
			{
				fromBlockNode = arth.OutValue;
			}
			else if (fromBlock is BaseLogicNodeBlock logi)
			{
				fromBlockNode = logi.Output;
			}
			else if (fromBlock is ConditionalNodeBlock condi)
			{
				fromBlockNode = (tup.Item3 == 0 ? condi.TrueOutput : condi.FalseOutput);
			}
			else if (fromBlock is SetConstantNodeBlock seti)
			{
				if (tup.Item2 == "Exit")
					fromBlockNode = seti.ExitNode;
				else fromBlockNode = seti.OutValue;
			}
			else if (fromBlock is GetConstantNodeBlock getvar)
			{
				fromBlockNode = getvar.output;
			}
			else if (fromBlock is DialogueNodeBlock dialogue)
			{
				fromBlockNode = dialogue.OutputNodes[tup.Item3];
			}
			return fromBlockNode;
		}

		private ConnectionNode GetToConnectionNode(BaseNodeBlock fromBlock, Tuple<String, String, int, String, String, int> tup)
		{
			ConnectionNode fromBlockNode = null;
			if (fromBlock is ExitBlockNode exit)//|| fromBlock is ExitBlockNode)
			{
				fromBlockNode = exit.EntryNode;
			}
			else if (fromBlock is BaseArithmeticBlock arth)
			{
				fromBlockNode = arth.InputNodes[tup.Item6];
			}
			else if (fromBlock is BaseLogicNodeBlock logi)
			{
				fromBlockNode = logi.InputNodes[tup.Item6];
			}
			else if (fromBlock is ConditionalNodeBlock condi)
			{
				if (tup.Item5 == "Enter")
					fromBlockNode = condi.EntryNode;
				else fromBlockNode = condi.InputNodes[tup.Item6];
			}
			else if (fromBlock is SetConstantNodeBlock seti)
			{
				if (tup.Item5 == "Enter")
					fromBlockNode = seti.EntryNode;
				else fromBlockNode = seti.InputNodes[tup.Item6];
			}
			else if (fromBlock is DialogueNodeBlock dialogue)
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
			foreach (BaseNodeBlock bn in dia.DialogueBlockNodes)
			{
				//display the nodes.
				Canvas.SetLeft(bn, bn.LocX);
				Canvas.SetTop(bn, bn.LocY);

				AllDialogueEditor_Grid.Visibility = Visibility.Visible;
				bn.ApplyTemplate();
				DialogueEditor_NodeGraph.AddToNodeEditor(bn);
				bn.ApplyTemplate();
				//var v = bn.FindResource("MainBorder");
				if (bn is DialogueNodeBlock dialo)
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
				//CurSceneCharacterDisplays.Add(new Tuple<ContentControl, ContentControl>(CC, SelectedBaseUIControl));
			}
		}

		private Point GetNodePosition(BaseNodeBlock NodeBlock, ConnectionNode DesNode)
		{
			Point retPoint = new Point(0, 0);

			if (NodeBlock is StartBlockNode || NodeBlock is ExitBlockNode)
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
			else if (NodeBlock is BaseArithmeticBlock || NodeBlock is BaseLogicNodeBlock
			                                     || NodeBlock is ConditionalNodeBlock 
			                                     || NodeBlock is SetConstantNodeBlock)
			{
				if (NodeBlock.EntryNode == DesNode)
				{
					retPoint.X = Canvas.GetLeft(NodeBlock);
					retPoint.Y = Canvas.GetTop(NodeBlock) + 10;
				}
				else if(NodeBlock.ExitNode == DesNode)
				{
					retPoint.X = Canvas.GetLeft(NodeBlock) + 150;
					retPoint.Y = Canvas.GetTop(NodeBlock) + 10;
				}
				else if (NodeBlock.InputNodes.FindIndex(x=> x == DesNode) >=0)
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
			else if (NodeBlock is GetConstantNodeBlock getvar)
			{
				if (getvar.output == DesNode)
				{
					retPoint.X = Canvas.GetLeft(NodeBlock) + 95;
					retPoint.Y = Canvas.GetTop(NodeBlock) + 40;
				}
			}
			else if (NodeBlock is DialogueNodeBlock)
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
					retPoint.Y = 20 + (NodeBlock.InputNodes.Count*40) + Canvas.GetTop(NodeBlock) + ((idx * 40) + 20);
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

//NOTES TO MY SELF
/*
 * System.drawing.Image causes memory leaks
 * RenderTargetBitmap also causes memory leaks
 * 
 * REASON? They are not IDisposable
 * 
 * 
 * 
 */
