using AmethystEngine.Components;
using AmethystEngine.Components.Tools;
using BixBite;
using BixBite.Rendering;
using BixBite.Resources;
using PropertyGridEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AmethystEngine.Components.Dolomite;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using Color = System.Windows.Media.Color;
using ComboBox = System.Windows.Controls.ComboBox;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Control = System.Windows.Controls.Control;
using Cursors = System.Windows.Input.Cursors;
using Image = System.Windows.Controls.Image;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Label = System.Windows.Controls.Label;
using ListBox = System.Windows.Controls.ListBox;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TabControl = System.Windows.Controls.TabControl;
using TextBox = System.Windows.Controls.TextBox;
using TreeView = System.Windows.Controls.TreeView;

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


	/// <summary>
	/// Interaction logic for EngineEditor.xaml
	/// </summary>
	public partial class EngineEditor : Window, INotifyPropertyChanged
	{
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32")]
		private static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

		[DllImport("user32")]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

		// Define Win32 API functions and constants
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

		// Constants for show commands (used with ShowWindow)
		public const int SW_RESTORE = 9;
		public const int SW_SHOW = 5;

		private const int SWP_NOZORDER = 0x0004;
		private const int SWP_NOACTIVATE = 0x0010;
		private const int GWL_STYLE = -16;
		private const int WS_CAPTION = 0x00C00000;
		private const int WS_THICKFRAME = 0x00040000;
		private const int WS_EX_LAYERED = 0x80000;
		private const int GWL_EXSTYLE = -20;
		private const int LWA_ALPHA = 0x2;


		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}


		public List<EditorObject> EditorObj_list = new List<EditorObject>();

		//List<Item> Titles { get; set; }
		public List<EditorObject> ContentLibaryObjList = new List<EditorObject>();
		TreeViewItem CurProjectTreeNode = new TreeViewItem();
		TreeView CurrentProjectTreeView = new TreeView();

		#region Fields
		private Process linkedGameProcess;
		private MonoGameContentBuilder _monoGameContentBuilder;
		#endregion

		#region Properties


		#endregion

		TreeView SceneExplorer_TreeView = new TreeView();
		ListBox EditorObjects_LB = new ListBox();

		EditorTool CurrentTool = EditorTool.None;
		SelectTool selectTool = new SelectTool();


		Point MPos = new Point();
		List<String> CMDOutput = new List<string>();

		String ProjectFilePath = "";
		String MainLevelPath = "";
		String ProjectDirectory = "";
		String ProjectContentDirectory = "";

		ObservablePropertyDictionary EditorObjectProperties = new ObservablePropertyDictionary();

		public EngineEditor()
		{
			InitializeComponent();

			this.DataContext = this;
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

			LevelEditorLoaded();
			UIEditorLoaded();
			DialogueEditorLoaded();
			SpriteSheetEditorLoaded();
			AnimationEditorLoaded();

			EditorObjects_LB = (ListBox) ContentLibrary_Control.Template.FindName("EditorObjects_LB", ContentLibrary_Control);
			TileMap_Canvas = (Canvas) (ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control));
			TileSets_CB = (ComboBox) (ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control));

			this.DataContext = this;

			SetupPreviewAnimationThread_CE();
			SetupSelectedAnimationThread();

			// Set up the tab control
			AE_CurrentAnimSM_Grid.Visibility = Visibility.Hidden;
			AE_NewAnimSM_MainGrid.Visibility = Visibility.Visible;


			InitializeComponent();

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
			DataContext = this;
			InitializeComponent();
			ProjectFilePath = FilePath;

			//we need to read this file. And set the project settings accordingly.
			ProjectName_LBL.Content = ProjectName;
			MainLevelPath = LevelPath;
			LoadInitalVars();
			LoadFileTree(ProjectFilePath.Replace(".gem", "_Game\\Content\\"));
			ProjectDirectory = ProjectFilePath;
			ProjectContentDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\");

			_monoGameContentBuilder = new MonoGameContentBuilder(ProjectFilePath.Substring(0, ProjectFilePath.LastIndexOf("\\")));

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
			LESelectRect = new Rectangle() {Tag = "Selection"};
			LESelectRect.MouseUp += LevelEditor_BackCanvas_MouseLeftButtonUp;
			EditorObjects_LB.ItemsSource = EditorObj_list;
			SearchResultList.ItemsSource = ContentLibaryObjList;
		}

		/// <summary>
		/// Opens up a file explorer and allows a user to choose a file location.
		/// </summary>
		/// <param name="prompt">What the File Explorer Window will display</param>
		/// <param name="Open">Either a OpenFIleExplorer or a SaveFileExplorer</param>
		/// <returns>File path as a String</returns>
		public static String GetFilePath(String prompt, bool Open = false, bool wholeFilePath = false)
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
					filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] {'/', '\\'}));
				}

				Console.WriteLine(filename);
				return filename;
			}
			else
			{
				// Prepare a dummy string, thos would appear in the dialog
				string dummyFileName = "";

				Microsoft.Win32.OpenFileDialog sf = new Microsoft.Win32.OpenFileDialog
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
					String savePath = "";
					if (!wholeFilePath)
						savePath = System.IO.Path.GetDirectoryName(sf.FileName);
					else savePath = sf.FileName;
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
			_hwndSource = (System.Windows.Interop.HwndSource) PresentationSource.FromVisual(this);
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
			SendMessage(_hwndSource.Handle, 0x112, (IntPtr) (61440 + direction), IntPtr.Zero);
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
			if (animationImportPreviewThread != null)
			{
				try
				{
					animationImportPreviewThread.Interrupt();
					if (!animationImportPreviewThread.Join(2000))
						animationImportPreviewThread.Abort();
				}
				catch
				{
					animationImportPreviewThread.Abort();
				}
			}

			if (selectedAnimThread_CE != null)
			{
				try
				{
					selectedAnimThread_CE.Interrupt();
					if (!selectedAnimThread_CE.Join(2000))
						selectedAnimThread_CE.Abort();
				}
				catch
				{
					selectedAnimThread_CE.Abort();
				}
			}

			if (previewAnimThread_CE != null)
			{
				try
				{
					previewAnimThread_CE.Interrupt();
					if (!previewAnimThread_CE.Join(2000))
						previewAnimThread_CE.Abort();
				}
				catch
				{
					previewAnimThread_CE.Abort();
				}
			}

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
			if (((TabItem) EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level")) //we are in the level editor.
			{
				TabControl LELibary_TC =
					(TabControl) ContentLibrary_Control.Template.FindName("LevelEditorLibary_TabControl", ContentLibrary_Control);
				if (LELibary_TC.SelectedIndex == 1) //sprite libary
				{
					ListBox SpriteLibary_LB =
						(ListBox) ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control);
					if ((bool) Desc_CB.IsChecked)
						SpriteLibary_LB.ItemTemplate = (DataTemplate) this.Resources["BigEdit1"];
					else
					{
						if (this.Resources.Contains("EObj_Small"))
							SpriteLibary_LB.ItemTemplate = (DataTemplate) this.Resources["BigEdit"];
					}
				}
			}
			else // other editors WIP
			{
				if ((bool) Desc_CB.IsChecked)
					EditorObjects_LB.ItemTemplate = (DataTemplate) this.Resources["BigEdit1"];
				else
				{
					if (this.Resources.Contains("EObj_Small"))
						EditorObjects_LB.ItemTemplate = (DataTemplate) this.Resources["BigEdit"];
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
			if (((TabItem) EditorWindows_TC.SelectedItem).Header == null)
			{
				EditorWindows_TC.SelectedIndex = 4;
				return;
			}

			if (((TabItem) EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level"))
			{
				ContentLibrary_Control.Template = (ControlTemplate) this.Resources["LevelEditorTileMap_Template"];
				ObjectProperties_Control.Template = (ControlTemplate) this.Resources["LevelEditorProperty_Template"];
				SceneExplorer_Control.Template = (ControlTemplate) this.Resources["LevelEditorSceneExplorer_Template"];
				EditorToolBar_CC.Template = (ControlTemplate) this.Resources["LevelEditorTileMapToolBar_Template"];


				UpdateLayout(); //update the templates to the ptrs won't be null! :D
				//Find and set level editor controls!
				FullMapLEditor_Canvas =
					(Canvas) ObjectProperties_Control.Template.FindName("LevelEditor_Canvas", ObjectProperties_Control);
				FullMapLEditor_VB =
					(VisualBrush) ObjectProperties_Control.Template.FindName("FullLeditorGrid_VB", ObjectProperties_Control);

				EditorObjects_LB =
					(ListBox) ContentLibrary_Control.Template.FindName("EditorObjects_LB", ContentLibrary_Control);
				TileMap_Canvas = (Canvas) (ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control));
				TileSets_CB =
					(ComboBox) (ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control));
				TileMapTiles_Rect =
					(Rectangle) ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB_Rect",
						ContentLibrary_Control);
				TileMap_VB =
					(VisualBrush) ContentLibrary_Control.Template.FindName("LevelEditorTileMapCanvas_VB", ContentLibrary_Control);
				TileMapGrid_Rect =
					(Rectangle) ContentLibrary_Control.Template.FindName("TileMapGrid_Rect", ContentLibrary_Control);
				SceneExplorer_TreeView =
					(TreeView) SceneExplorer_Control.Template.FindName("LESceneExplorer_TreeView", SceneExplorer_Control);
				SceneExplorer_TreeView.ItemsSource = OpenLevels;
				SceneExplorer_TreeView.Items.Refresh();
				SceneExplorer_TreeView.UpdateLayout();
			}
			else if (((TabItem) EditorWindows_TC.SelectedItem).Header.ToString().Contains("Dialogue"))
			{
				ObjectProperties_Control.Template = (ControlTemplate) this.Resources["DialogueEditorProperty_Template"];
				ContentLibrary_Control.Template = (ControlTemplate) this.Resources["DialogueEditorObjects_Template"];
				EditorToolBar_CC.Template = (ControlTemplate) this.Resources["DialogueEditorObjectExplorer_Template"];
				UpdateLayout();
				if (((CollapsedPropertyGrid.CollapsedPropertyGrid) (ObjectProperties_Control.Template.FindName(
					"DialoguePropertyGrid", ObjectProperties_Control))).ItemsSource == null)
				{
					try
					{
						((CollapsedPropertyGrid.CollapsedPropertyGrid) (ObjectProperties_Control.Template.FindName(
							"DialoguePropertyGrid", ObjectProperties_Control))).ItemsSource = PropertyBags;
					}
					catch
					{
						((CollapsedPropertyGrid.CollapsedPropertyGrid) (ObjectProperties_Control.Template.FindName(
							"DialoguePropertyGrid", ObjectProperties_Control))).ItemsSource = null;
						((CollapsedPropertyGrid.CollapsedPropertyGrid) (ObjectProperties_Control.Template.FindName(
							"DialoguePropertyGrid", ObjectProperties_Control))).ItemsSource = PropertyBags;
					}
				}

				Dialogue_CE_Tree =
					(TreeView) ContentLibrary_Control.Template.FindName("DialogueEditor_CE_TV", ContentLibrary_Control);
			}
			else if (((TabItem) EditorWindows_TC.SelectedItem).Header.ToString().Contains("UI"))
			{
				ContentLibrary_Control.Template = (ControlTemplate) this.Resources["UIEditorObjects_Template"];
				ObjectProperties_Control.Template = (ControlTemplate) this.Resources["UIEditorProperty_Template"];
				EditorToolBar_CC.Template = (ControlTemplate) this.Resources["UIEditorObjectExplorer_Template"];
				if (SceneExplorer_Control != null)
				{
					ControlTemplate cc = (ControlTemplate) this.Resources["UIEditorSceneExplorer_Template"];
					SceneExplorer_Control.Template = cc;
					Console.WriteLine(SceneExplorer_Control.Template.ToString());
					TreeView
						tv = (TreeView) cc.FindName("UISceneExplorer_TreeView",
							SceneExplorer_Control); //(TreeView)SceneExplorer_Control.Template.FindName("UISceneExplorer_TreeView", SceneExplorer_Control);
					if (tv == null) return;
					SceneExplorer_TreeView = tv;
					SceneExplorer_TreeView.ItemsSource = OpenUIEdits;
				}


			}
			else if (((TabItem) EditorWindows_TC.SelectedItem).Header.ToString().Contains("Spritesheet"))
			{
				ContentLibrary_Control.Template = (ControlTemplate) this.Resources["SpriteSheetObjects_Template"];
				SceneExplorer_Control.Template = (ControlTemplate)this.Resources["AnimationEditorSceneExplorer_Template"];
				EditorToolBar_CC.Template = (ControlTemplate) this.Resources["SpritesheetEditorObjectExplorer_Template"];
				ObjectProperties_Control.Template = (ControlTemplate) this.Resources["SpritesheetEditorProperties_Template"];
				UpdateLayout();

				ControlTemplate cc = (ControlTemplate)this.Resources["SpriteSheetObjects_Template"];


				SpriteSheet_CE_Tree = (TreeView)cc.FindName("SpriteSheetEditor_CE_TV", ContentLibrary_Control);
				
			}
			else if (((TabItem) EditorWindows_TC.SelectedItem).Header.ToString().Contains("Animation"))
			{
				ContentLibrary_Control.Template = (ControlTemplate) this.Resources["AnimationEditorObjects_Template"];
				SceneExplorer_Control.Template = (ControlTemplate) this.Resources["AnimationEditorSceneExplorer_Template"];
				EditorToolBar_CC.Template = (ControlTemplate) this.Resources["AnimationEditorObjectExplorer_Template"];
				ObjectProperties_Control.Template = (ControlTemplate) this.Resources["AnimationEditorProperties_Template"];
				
				UpdateLayout();


				//ObjectProperties_Control.Template = (ControlTemplate)this.Resources["UIEditorProperty_Template"];
				//EditorToolBar_CC.Template = (ControlTemplate)this.Resources["UIEditorObjectExplorer_Template"];
				//if (SceneExplorer_Control != null)
				//{
				//	ControlTemplate cc = (ControlTemplate)this.Resources["UIEditorSceneExplorer_Template"];
				//	SceneExplorer_Control.Template = cc;
				//	Console.WriteLine(SceneExplorer_Control.Template.ToString());
				//	TreeView
				//		tv = (TreeView)cc.FindName("UISceneExplorer_TreeView",
				//			SceneExplorer_Control); //(TreeView)SceneExplorer_Control.Template.FindName("UISceneExplorer_TreeView", SceneExplorer_Control);
				//	if (tv == null) return;
				//	SceneExplorer_TreeView = tv;
				//	SceneExplorer_TreeView.ItemsSource = OpenUIEdits;
				//}"AnimationEditor_CE_TV"
				ControlTemplate cc = (ControlTemplate) this.Resources["AnimationEditorSceneExplorer_Template"];
				SceneExplorer_TreeView = (TreeView) cc.FindName("AnimationSceneExplorer_TreeView", SceneExplorer_Control);

				SceneExplorer_TreeView.ItemsSource = ActiveAnimationStateMachines;
				Animation_CE_Tree =
					(TreeView) ContentLibrary_Control.Template.FindName("AnimationEditor_CE_TV", ContentLibrary_Control);
				Animation_CE_Tree.ItemsSource = CurrentAnimationStateMachine?.States?.Values;
				AnimationStateProperties_ItemsControl =
					(ItemsControl) ObjectProperties_Control.Template.FindName("AnimationEditor_Events_IC",
						ObjectProperties_Control);
				AnimationLayerProperties_ItemsControl =
					(ItemsControl)ObjectProperties_Control.Template.FindName("AnimationEditor_Layer_Events_IC",
						ObjectProperties_Control);
				AnimationProperties_ItemsControl =
					(ItemsControl)ObjectProperties_Control.Template.FindName("AnimationEditor_Animation_Events_IC",
						ObjectProperties_Control);
				AnimationChangeEvents_Properties_ScrollViewer = 
					(ScrollViewer)ObjectProperties_Control.Template.FindName("AnimationEditor_Events_SV",
					ObjectProperties_Control);

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
			CurrentProjectTreeView = (TreeView) sender;
			ContentLibaryObjList.Clear();
			if (CurrentProjectTreeView.Items.Count == 0) return;
			((TreeViewItem) (CurrentProjectTreeView.SelectedItem)).IsExpanded = true;
			ProjectDirectory = ProjectFilePath.Replace(".gem",
				"_Game\\Content\\" + ((TreeViewItem) (CurrentProjectTreeView.SelectedItem)).Header + "\\");
			AmethystEngine.Components.EObjectType EType = EObjectType.File;
			foreach (TreeViewItem tvi in ((TreeViewItem) (CurrentProjectTreeView.SelectedItem)).Items)
			{
				bool brel = false;
				String desimg = tvi.Tag.ToString();
				String desname = tvi.Tag.ToString();
				desname = desname.Substring(desname.LastIndexOfAny(new char[] {'/', '\\'}) + 1);
				if (tvi.Tag.ToString().Contains(';')) //its a foldername
				{
					brel = true;
					EType = EObjectType.Folder;
					desname = tvi.Header.ToString();
				}

				if (new[] {".tif", ".jpg", ".png"}.Any(c => desname.ToLower().Contains(c)))
				{
					EditorObject ed = new EditorObject(desimg, desname, brel, EObjectType.File);
					ContentLibaryObjList.Add(ed);
				}
				else if (!tvi.Header.ToString().Contains("."))
				{
					//desimg = TempPic; brel = true;
					EditorObject ed = new EditorObject(desimg, desname, brel, EType);
					ContentLibaryObjList.Add(ed);
				}
				else
				{
					desimg = TempPic;
					brel = true;
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

			var directoryNode = new TreeViewItem
				{Header = directoryInfo.Name, Tag = TempPic, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255))};
			foreach (var directory in directoryInfo.GetDirectories())
				directoryNode.Items.Add(CreateDirectoryNode(directory));

			foreach (var file in directoryInfo.GetFiles())
				directoryNode.Items.Add(new TreeViewItem
				{
					Visibility = System.Windows.Visibility.Collapsed, Tag = directoryInfo.FullName + "\\" + file.Name,
					Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), Header = file.Name
				});
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

		static bool ContainsFolder(string directoryPath, string targetFolderName)
		{
			// Check if the directory itself has the target folder
			if (Directory.Exists(Path.Combine(directoryPath, targetFolderName)))
			{
				return true;
			}

			// Check subdirectories recursively
			string[] subdirectories = Directory.GetDirectories(directoryPath);
			foreach (string subdirectory in subdirectories)
			{
				if (ContainsFolder(subdirectory, targetFolderName))
				{
					return true;
				}
			}

			return false;
		}

		private void ContentImportAndBuildXNB_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			// Are we in the right directory...?
			if (ContainsFolder(ProjectDirectory, "Content"))
			{
				// Are we in the right directory...?
				if (ContainsFolder(ProjectDirectory, "Images"))
				{
					// We are going to be handling an image build here.
					Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
					{
						FileName = "images", //default file 
						Title = "Import and build new Image file",
						DefaultExt = "All files (*.png)|*.png", //default file extension
						Filter = "png file (*.png)|*.png"
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


					_monoGameContentBuilder.AttemptToBuildPNGToXNBFile(importfilename, ProjectFilePath.Replace(".gem", "_Game\\Content\\Images"),
						ProjectFilePath.Replace(".gem", "_Game\\Content\\Content.mgcb"));
				}


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
			int len = importstartlocation.Length - importstartlocation.LastIndexOfAny(new char[] {'/', '\\'});
			destfilename = ProjectDirectory + importstartlocation.Substring(
				importstartlocation.LastIndexOfAny(new char[] {'\\', '/'}) + 1, len - 1);

			//copy the file
			File.Copy(importfilename, destfilename, true);
			//LoadInitalVars();
			var v = CurrentProjectTreeView.SelectedValuePath;
			LoadFileTree(ProjectFilePath.Replace(".gem", "_Game\\Content\\")); //reload the project to show the new file.
			CurrentProjectTreeView.SelectedValuePath = v;

			ContentLibaryObjList.Add(new EditorObject(destfilename, importstartlocation.Substring(
				importstartlocation.LastIndexOfAny(new char[] {'\\', '/'}) + 1, len - 1), true, EObjectType.File));

			SearchResultList.ItemsSource = null;
			SearchResultList.ItemsSource = ContentLibaryObjList;

		}

		#endregion

		#region "File Traverse Item View"

		private void DirectoryBack_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (((TreeViewItem) ProjectContentExplorer.SelectedItem) == null) return; //you need to click on something...

			if (((TreeViewItem) ProjectContentExplorer.SelectedItem).Parent != null)
				try
				{
					((TreeViewItem) ((TreeViewItem) ProjectContentExplorer.SelectedItem).Parent).IsSelected = true;
				}
				catch (InvalidCastException)
				{
					return;
				}
		}

		private void SearchResultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((ListBox) sender).ItemsSource = ContentLibaryObjList;

			int i = (((ListBox) sender).SelectedIndex);
			if (i < 0)
				return;

			switch (((EditorObject) ContentLibaryObjList[i]).EditObjType)
			{
				case (EObjectType.None):
					return;
				case (EObjectType.File):
					return;
				case (EObjectType.Folder):
					ProjectDirectory = ProjectFilePath.Replace(".gem",
						"_Game\\Content\\" + ((EditorObject) ((ListBox) sender).SelectedItem).Name + "\\");
					((TreeViewItem) (((TreeViewItem) (ProjectContentExplorer.SelectedItem)).Items[
						((ListBox) sender).SelectedIndex])).IsSelected = true;
					return;
			}
		}

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
			string filename = "";

			//level editor
			if (((TabItem) EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level"))
			{
				//get the LevelEditor content Libary tab control
				TabControl LELibary_TC =
					(TabControl) ContentLibrary_Control.Template.FindName("LevelEditorLibary_TabControl", ContentLibrary_Control);
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
					Grid Prob_Grid =
						(Grid) ContentLibrary_Control.Template.FindName("TileSetProperties_Grid", ContentLibrary_Control);
					Prob_Grid.Visibility = Visibility.Visible;
					((ScrollViewer) ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control))
						.Visibility = Visibility.Hidden;
					((ComboBox) ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control)).Visibility
						= Visibility.Hidden;
					((Label) ContentLibrary_Control.Template.FindName("TileSet_LBL", ContentLibrary_Control)).Visibility =
						Visibility.Hidden;

					TextBox Name_TB =
						(TextBox) ContentLibrary_Control.Template.FindName("TileMapName_TB", ContentLibrary_Control);
					Name_TB.Text = filename;
				}
				else if (LELibary_TC.SelectedIndex == 1) //we are in the sprite tab
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

					ListBox SpriteLibary_LB =
						(ListBox) ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control);

					EditorObject E = new EditorObject(filename,
						filename.Substring(filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) + 1,
							filename.LastIndexOf(".") - filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) - 1), false);
					LESpriteObjectList.Add(E);
					SpriteLibary_LB.ItemsSource = null;
					SpriteLibary_LB.ItemsSource = LESpriteObjectList;

					//add the sprite to level object
					CurrentLevel.sprites.Add(new Tuple<string, string>(
						filename.Substring(filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) + 1,
							filename.LastIndexOf(".") - filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) - 1), filename));
				}
			}
			else
			{
				//get the sprite listbox
				TabControl SpriteLibary_LB =
					(TabControl) ContentLibrary_Control.Template.FindName("SpriteLibary_LB", ContentLibrary_Control);

				EditorObject E = new EditorObject(filename,
					filename.Substring(filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) + 1,
						filename.LastIndexOf(".") - filename.LastIndexOf((filename.Contains("\\") ? "\\" : "/")) - 1), false);
				EditorObj_list.Add(E);
				EditorObjects_LB.ItemsSource = null;
				EditorObjects_LB.ItemsSource = EditorObj_list;
			}
		}

		private void CreateTileMap_BTN_Click(object sender, RoutedEventArgs e)
		{
			int Height = 0;

			TextBox Width_TB = (TextBox) ContentLibrary_Control.Template.FindName("TileWidth_TB", ContentLibrary_Control);
			TextBox Height_TB = (TextBox) ContentLibrary_Control.Template.FindName("TileHeight_TB", ContentLibrary_Control);
			TextBox Name_TB = (TextBox) ContentLibrary_Control.Template.FindName("TileMapName_TB", ContentLibrary_Control);
			if (Int32.TryParse(Width_TB.Text, out int Width))
			{
				if (Int32.TryParse(Height_TB.Text, out Height))
				{
					if (Width > 0 && Height > 0)
					{
						CreateTileMap(Name_TB.Text, Width, Height);
					}
				}
			}

			//set visabilty. 
			Grid Prob_Grid =
				(Grid) ContentLibrary_Control.Template.FindName("TileSetProperties_Grid", ContentLibrary_Control);
			Prob_Grid.Visibility = Visibility.Hidden;
			((ScrollViewer) ContentLibrary_Control.Template.FindName("LevelEditorTIleMap_SV", ContentLibrary_Control))
				.Visibility = Visibility.Visible;
			((ComboBox) ContentLibrary_Control.Template.FindName("TileSetSelector_CB", ContentLibrary_Control)).Visibility =
				Visibility.Visible;
			((Label) ContentLibrary_Control.Template.FindName("TileSet_LBL", ContentLibrary_Control)).Visibility =
				Visibility.Visible;

		}

		private void CreateTileMap(String FileName, int x, int y)
		{

			if (FileName == "")
			{
				MessageBox.Show("Filename is invaild!");
				return;
			}

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

			int len = pic.UriSource.ToString().LastIndexOf('.') -
			          pic.UriSource.ToString().LastIndexOfAny(new char[] {'/', '\\'});
			String Name = pic.UriSource.ToString()
				.Substring(pic.UriSource.ToString().LastIndexOfAny(new char[] {'/', '\\'}) + 1, len - 1);

			CurrentLevel.TileSet.Add(
				new Tuple<string, string, int, int>(Name, FileName, x, y)); //add the tile set to the current level object.

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
			if (TileSets_CB.SelectedIndex >= 0)
			{

				Canvas TileMap = (Canvas) ContentLibrary_Control.Template.FindName("TileMap_Canvas", ContentLibrary_Control);

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
			Level TempLevel = ((Level) SceneExplorer_TreeView.SelectedValue);
			TempLevel.AddLayer("new tile", LayerType.Tile);
			TempLevel.Layers.Last().DefineLayerDataType(LayerType.Tile, (int) TempLevel.GetPropertyData("xCells"),
				(int) TempLevel.GetPropertyData("yCells"));
		}

		private void SpriteLayer_Click(object sender, RoutedEventArgs e)
		{
			Level TempLevel = ((Level) SceneExplorer_TreeView.SelectedValue);
			TempLevel.AddLayer("new sprite", LayerType.Sprite);
			TempLevel.Layers.Last().DefineLayerDataType(LayerType.Sprite);
		}

		private void GameObjectLayer_Click(object sender, RoutedEventArgs e)
		{
			Level TempLevel = ((Level) SceneExplorer_TreeView.SelectedValue);
			TempLevel.AddLayer("new GOL", LayerType.GameEvent);
			TempLevel.Layers.Last().DefineLayerDataType(LayerType.GameEvent, (int) TempLevel.GetPropertyData("xCells"),
				(int) TempLevel.GetPropertyData("yCells"));


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
			((DispatcherFrame) f).Continue = false;
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
			if (((TabItem) EditorWindows_TC.SelectedItem).Header.ToString().Contains("Level")) //Level Editor
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
					if (SceneExplorer_TreeView.SelectedValue is SpriteLayer)
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
			int curLayer = CurrentLevel.FindLayerindex(((SpriteLayer) SceneExplorer_TreeView.SelectedValue).LayerName);
			int relgridsize = (((int) (40 * LevelEditor_Canvas.RenderTransform.Value.M11)));
			//use the current rectange that the user is in to get the (x,y) cords
			//compare these values to the current MOUSE POS
			Rectangle rr = SelectTool.FindTile(LevelEditor_Canvas, LevelEditor_Canvas.Children.OfType<Rectangle>().ToList(),
				curLayer,
				(int) p.X, (int) p.Y, (int) Canvas_grid.Viewport.X, (int) Canvas_grid.Viewport.Y);
			Point snappedpoints = RelativeGridSnap(shiftpoints[2]);
			if (rr != null)
				return BixBite.BixBiteTypes.CardinalDirection.None;
			else
			{
				if (p.X < snappedpoints.X && (p.Y > snappedpoints.Y && p.Y < snappedpoints.Y + relgridsize))
				{
					//west
					Console.WriteLine("moved west");
					return BixBite.BixBiteTypes.CardinalDirection.W;
				}
				else if (p.X > snappedpoints.X + relgridsize && (p.Y > snappedpoints.Y && p.Y < snappedpoints.Y + relgridsize))
				{
					//east
					Console.WriteLine("moved East");
					return BixBite.BixBiteTypes.CardinalDirection.E;
				}
				else if (p.Y > snappedpoints.Y + relgridsize && (p.X > snappedpoints.X && p.X < snappedpoints.X + relgridsize))
				{
					//east
					Console.WriteLine("moved South");
					return BixBite.BixBiteTypes.CardinalDirection.S;
				}
				else if (p.Y < snappedpoints.Y && (p.X > snappedpoints.X && p.X < snappedpoints.X + relgridsize))
				{
					//east
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
			Console.WriteLine("Text Property Key down " + ((TextBox) sender).Tag.ToString());

		}


		private void SetOpacityForChildWindow(IntPtr childWindowHandle, byte opacity, int style)
		{
			// Enable WS_EX_LAYERED style to make the window layered (supports transparency)
			int extendedStyle = 0; // = style;
			extendedStyle |= GetWindowLong(childWindowHandle, GWL_EXSTYLE);
			extendedStyle |= (extendedStyle | WS_EX_LAYERED);
			extendedStyle &= ~WS_CAPTION & ~WS_THICKFRAME;
			SetWindowLong(childWindowHandle, GWL_EXSTYLE, extendedStyle);

			// Set the window opacity using SetLayeredWindowAttributes
			SetLayeredWindowAttributes(childWindowHandle, 0, opacity, LWA_ALPHA);
			
			style = style & ~WS_CAPTION & ~WS_THICKFRAME;
			SetWindowLong(linkedGameProcess.MainWindowHandle, GWL_STYLE, style);
		}

		private void LaunchChildProcess()
		{
			string path =ProjectFilePath.Replace(".gem", "");
			path += String.Format( "{0}",  "_Game") ;
			path += String.Format("\\{0}\\{1}\\{2}\\{3}\\Game1.exe", "bin", "DesktopGL", "AnyCPU", "Debug");
			
			Console.WriteLine( File.Exists(path));
			linkedGameProcess = new Process();
			linkedGameProcess.StartInfo = new ProcessStartInfo(path)
			{
				WorkingDirectory = path,
			};
			linkedGameProcess.Start();
			Thread.Sleep(2000);
			linkedGameProcess.WaitForInputIdle();

			var helper = new WindowInteropHelper(this);

			SetParent(linkedGameProcess.MainWindowHandle, helper.Handle);
			Thread.Sleep(1000);

			// remove control box
			int style = GetWindowLong(linkedGameProcess.MainWindowHandle, GWL_STYLE);
			style = style & ~WS_CAPTION & ~WS_THICKFRAME;
			SetOpacityForChildWindow(linkedGameProcess.MainWindowHandle, Byte.MaxValue, style);

			ResizeEmbeddedApp();
		}

		private void ResizeEmbeddedApp()
		{
			if (linkedGameProcess == null)
				return;

			// Assuming you have a reference to the DockPanel you want to get the position of
			DockPanel dockPanel = Game_DockPanel; // Replace with your actual DockPanel reference

			// Get the position of the DockPanel relative to its parent container
			Point positionRelativeToParent = dockPanel.TransformToAncestor(this).Transform(new Point(0, 0));


			// The positionRelativeToParent now contains the X and Y coordinates of the DockPanel
			int x = (int)positionRelativeToParent.X;
			int y = (int)positionRelativeToParent.Y;

			SetWindowPos(linkedGameProcess.MainWindowHandle, IntPtr.Zero, x, y, (int)dockPanel.ActualWidth, (int)dockPanel.ActualHeight, SWP_NOZORDER | SWP_NOACTIVATE);
		}

		protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
		{
			System.Windows.Size size = base.MeasureOverride(availableSize);
			ResizeEmbeddedApp();
			return size;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GameTestRun_BTN_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				EditorWindows_TC.SelectedIndex = 7;
				LaunchChildProcess();
				//SetOpacityForChildWindow(linkedGameProcess.MainWindowHandle, Byte.MaxValue);

			}
			catch (Exception ex)
			{
				// Handle any exceptions that occurred during assembly loading or type retrieval
				Console.WriteLine("An error occurred: " + ex.Message);
			}

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
		/// this button will auto generate and MSBuild the games project files to allow game event use. IF code compiles w/no errors
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
			int i = 0;
			int j = 0;
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

					LESelectedSprite =
						SelectTool.FindSprite(((List<Sprite>)((SpriteLayer)SceneExplorer_TreeView.SelectedItem).LayerObjects),
							LESelectedSpriteControl);

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
				PropGrid LB =
					((PropGrid)(ObjectProperties_Control.Template.FindName("UIPropertyGrid", ObjectProperties_Control)));
				LB.ClearProperties();
				foreach (object o in CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item2))
				{
					if (o is null)
						continue;
					if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] == "Zindex")
					{
						TextBox TB = new TextBox();
						TB.KeyDown += BaseUI_ZIndex_Changed;

						// TODO: FIX PROP GRID CALLBACKS

						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i],
							TB,
							o.ToString(), (sender1, args) =>
							{
								Console.WriteLine("BROKEN CALLBACK :(");
								PropertyCallbackTB(sender1, args, CurrentUIDictionary[SelectedUIControl.Name]);
							}); //CurrentUIDictionary[SelectedUIControl.Name].PropertyCallbackTB);
					}
					else if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] ==
									 "ShowBorder")
					{
						CheckBox CB = new CheckBox() { VerticalAlignment = VerticalAlignment.Center };
						CB.Click += SetBorderVisibility;
						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i],
							CB,
							o);
					}
					else if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] ==
									 "Image")
					{
						ComboBox CB = new ComboBox() { Height = 50, ItemTemplate = (DataTemplate)this.Resources["CBIMGItems"] };
						CB.SelectionChanged += GameImage_SelectionChanged;
						List<EditorObject> ComboItems = new List<EditorObject>();
						foreach (String filepath in GetAllProjectImages())
						{
							ComboItems.Add(new EditorObject(filepath,
								filepath.Substring(filepath.LastIndexOfAny(new char[] { '\\', '/' })), false));
						}

						CB.ItemsSource = ComboItems;

						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i],
							CB,
							new List<String>());
					}
					else if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] ==
									 "Background")
					{
						DropDownCustomColorPicker.CustomColorPicker TB = new DropDownCustomColorPicker.CustomColorPicker();
						TB.SelectedColorChanged += customCP_BackgroundColorChanged;
						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i],
							TB,
							o.ToString());
					}
					else if (CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i] ==
									 "FontColor")
					{
						DropDownCustomColorPicker.CustomColorPicker TB = new DropDownCustomColorPicker.CustomColorPicker();
						TB.SelectedColorChanged += customCP_FontColorChanged;
						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i],
							TB,
							o.ToString());
					}
					else
					{
						TextBox TB = new TextBox();
						TB.KeyDown += UIPropertyCallback;

						// TODO: FIX PROP GRID CALLBACKS

						LB.AddProperty(CurrentUIDictionary[SelectedUIControl.Name].GetProperties().Select(m => m.Item1).ToList()[i],
							TB,
							o.ToString(), (sender1, args) =>
							{
								Console.WriteLine("BROKEN CALLBACK :(");
								PropertyCallbackTB(sender1, args, CurrentUIDictionary[SelectedUIControl.Name]);
							}); //CurrentUIDictionary[SelectedUIControl.Name].PropertyCallbackTB);
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


		private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
		{
			// BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

			using (MemoryStream outStream = new MemoryStream())
			{
				BitmapEncoder enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(bitmapImage));
				enc.Save(outStream);
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

				return new Bitmap(bitmap);
			}
		}

		public BitmapImage ToBitmapImage(Bitmap bitmap)
		{
			using (var memory = new MemoryStream())
			{
				bitmap.SetResolution(96.0f, 96.0f);
				bitmap.Save(memory, ImageFormat.Png);
				memory.Position = 0;

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.DecodePixelWidth = bitmap.Width;
				bitmapImage.DecodePixelHeight = bitmap.Height;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();

				return bitmapImage;
			}
		}


	}
}

//NOTES TO MY SELF
/*
 * System.drawing.Image causes memory leaks
 *
 * RenderTargetBitmap also causes memory leaks
 * 
 * REASON? They are not IDisposable
 * 
 * 
 * 
 */
