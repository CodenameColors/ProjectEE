using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AmethystEngine.Components;
using BixBite.Rendering;
using BixBite.Rendering.Animation;
using Image = System.Windows.Controls.Image;

namespace AmethystEngine.Forms
{
	public partial class EngineEditor
	{

		private String _newAnimimationStatemachineFileName = "";
		private String _newSpriteSheetCharacterName = "";
		private String _newAnimimationStatemachineFileLocation = "";

		private bool _allowAnimationExporting = false;
		private bool _bNewAnimStateMachine = false;
		private bool _bAllAnimsOneLineEach = true;
		private bool _bAllowImportAnimPreview = false;
		private int _newAnimimationStatemachineTotalWidth = -1;
		private int _newAnimimationStatemachineTotalHeight = -1;

		private Image PreviewAnimUI_Image_PTR = null;
		private AnimationState PreviewAnim_Data_PTR = null;
		private Thread previewAnimThread_CE;
		private Thread selectedAnimThread_CE;
		private List<CroppedBitmap> CurrentAnimPreviewImages_CE = new List<CroppedBitmap>();
		private List<CroppedBitmap> CurrentSelectedAnimImages_List = new List<CroppedBitmap>();
		private List<List<CroppedBitmap>> AnimationSubLayerImages_List = new List<List<CroppedBitmap>>();


		TreeView Animation_CE_Tree;
		TreeView AnimationChangeEvents_Properties_Tree;
		TreeView AnimationAudioEvents_Properties_Tree;
		private Thread animationImportPreviewThread;
		public Stopwatch AnimationImporterPreview_Stopwatch = new Stopwatch();
		public Stopwatch Animation_CE_Preview_Stopwatch = new Stopwatch();
		public Stopwatch AnimationSelected_Stopwatch = new Stopwatch();

		private String _animationState_TV_QueriedKey = String.Empty;

		public String NewAnimimationStatemachineFileName
		{
			get => _newAnimimationStatemachineFileName;
			set
			{
				_newAnimimationStatemachineFileName = value;
				OnPropertyChanged("NewAnimimationStatemachineFileName");
			}
		}

		public String NewSpriteSheetCharacterName
		{
			get => _newSpriteSheetCharacterName;
			set
			{
				_newSpriteSheetCharacterName = value;
				OnPropertyChanged("NewSpriteSheetCharacterName");
			}
		}

		public String NewAnimimationStatemachineLocation
		{
			get => _newAnimimationStatemachineFileLocation;
			set
			{
				_newAnimimationStatemachineFileLocation = value;
				OnPropertyChanged("NewAnimimationStatemachineLocation");
			}
		}

		public bool bNewAnimStateMachine
		{
			get => _bNewAnimStateMachine;
			set
			{
				_bNewAnimStateMachine = value;
				OnPropertyChanged("bNewAnimStateMachine");
			}
		}

		public bool bAllowImportAnimPreview
		{
			get => _bAllowImportAnimPreview;
			set
			{
				_bAllowImportAnimPreview = value;
				OnPropertyChanged("bAllowImportAnimPreview");
			}
		}

		public bool bAllAnimsOneLineEach
		{
			get => _bAllAnimsOneLineEach;
			set
			{
				_bAllAnimsOneLineEach = value;
				OnPropertyChanged("bAllAnimsOneLineEach");
			}
		}

		private int _animationImporterTime_MS = -1;

		public int AnimationImporterTime_MS
		{
			get => _animationImporterTime_MS;
			set
			{
				_animationImporterTime_MS = value;
				OnPropertyChanged("AnimationImporterTime_MS");
			}
		}


		public int NewAnimimationStatemachineTotalWidth
		{
			get => _newAnimimationStatemachineTotalWidth;
			set
			{
				_newAnimimationStatemachineTotalWidth = value;
				OnPropertyChanged("NewAnimimationStatemachineTotalWidth");
			}
		}

		public int NewAnimimationStatemachineTotalHeight
		{
			get => _newAnimimationStatemachineTotalHeight;
			set
			{
				_newAnimimationStatemachineTotalHeight = value;
				OnPropertyChanged("NewAnimimationStatemachineTotalHeight");
			}
		}



		private void AnimationEditorLoaded()
		{

		}

		private void ImportNewSpriteSheetFile_BTN(object sender, RoutedEventArgs e)
		{
			//String filename = "";
			//Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			//{
			//	FileName = "SpriteSheet", //default file 
			//	DefaultExt = "*.spritesheet", //default file extension
			//	Filter = "spritesheet (*.spritesheet)|*.spritesheet" //filter files by extension
			//};

			//// Show save file dialog box
			//Nullable<bool> result = dlg.ShowDialog();
			//// Process save file dialog box results kicks out if the user doesn't select an item.
			//filename = "";
			//if (result == true)
			//	filename = dlg.FileName;
			//else
			//	return;

			//Console.WriteLine(filename);

			//// NEW CODE for NEW SPRITESHEET
			//CanvasSpritesheet tempCanvasSpritesheet = CanvasSpritesheet.ImportSpriteSheet(filename);
			//NewAnimimationStatemachineFileName = tempCanvasSpritesheet.Name;
			//NewAnimimationStatemachineLocation = tempCanvasSpritesheet.ImagePath;
			//NewAnimimationStatemachineTotalWidth = tempCanvasSpritesheet.Width;
			//NewAnimimationStatemachineTotalHeight = tempCanvasSpritesheet.Height;

			//Image img = new Image();
			//BitmapImage bmi = new BitmapImage();

			//bmi.BeginInit();
			//bmi.CacheOption = BitmapCacheOption.OnLoad;
			//bmi.UriSource = new Uri(NewAnimimationStatemachineLocation, UriKind.Absolute);
			//bmi.EndInit();
			//Bitmap pain = BitmapImage2Bitmap(bmi);
			//pain.SetResolution(96.0f, 96.0f);
			//bmi = ToBitmapImage(pain);
			//img.Source = bmi;

			//CurrentSpriteSheet_Image = bmi;

			//bNewAnimStateMachine = true;

			//// We have created a new WPF spritesheet. GET THE BIXBITE ONE
			//CurrentAnimationStateMachine = canvaSpriteSheetToAnimationStateMachine(tempCanvasSpritesheet);

			//AE_NewAnimStates_IC.ItemsSource = null;

			//ActiveAnimationStateMachines.Add((CurrentAnimationStateMachine));

			//bAllowImportAnimPreview = true;
			//AnimationImporterPreview_Stopwatch = new Stopwatch();
			//AnimationImporterPreview_Stopwatch.Start();


			//// Set up the Editor Objects section
			//Animation_CE_Tree.ItemsSource = CurrentAnimationStateMachine.States.Values;
			//AE_NewAnimStates_IC.ItemsSource = CurrentAnimationStateMachine.States.Values;

			//SceneExplorer_TreeView.ItemsSource = ActiveAnimationStateMachines;

			//if (animationImportPreviewThread == null)
			//{
			//	animationImportPreviewThread = new Thread(() =>
			//	{
			//		while (true)
			//		{
			//			try
			//			{
			//				Thread.Sleep(15);
			//			}
			//			catch
			//			{
			//				Console.WriteLine("animation preview thread == interrupted");
			//				break;
			//			}

			//			try
			//			{
			//				if (bAllowImportAnimPreview)
			//				{
			//					//if (AnimationImporterPreview_Stopwatch.ElapsedMilliseconds > 1000)
			//					//	AnimationImporterPreview_Stopwatch.Restart();	//we need to reset the timer.

			//					//Force update to MainGUI
			//					Dispatcher.Invoke(() =>
			//						AnimationImporterTime_MS = (int)AnimationImporterPreview_Stopwatch.ElapsedMilliseconds);

			//					//Okay let's update the animations!
			//					for (int i = 0; i < AE_NewAnimStates_IC.Items.Count; i++)
			//					{
			//						AnimationState currentSpriteAnimation = CurrentAnimationStateMachine.States.Values.ToList()[i];
			//						if (currentSpriteAnimation. == null)
			//							currentSpriteAnimation.CurrentFrameRect = currentSpriteAnimation.FrameDrawRects.First;
			//						Dispatcher.Invoke(() =>
			//						{
			//							ContentPresenter c =
			//								((ContentPresenter)AE_NewAnimStates_IC.ItemContainerGenerator.ContainerFromIndex(i));
			//							var v = c.ContentTemplate.FindName("CurrentFrame_TB", c);
			//							var v1 = c.ContentTemplate.FindName("CurrentDeltaTime_TB", c);

			//							int DT = (int.Parse((v1 as TextBox).Text));

			//							var cb = c.ContentTemplate.FindName("PreviewAnim_CB", c);
			//							if ((cb as CheckBox).IsChecked == true)
			//							{

			//								if (currentSpriteAnimation.FrameCount > 0 && currentSpriteAnimation.FPS > 0)
			//								{
			//									int framenum = (int.Parse((v as TextBox).Text));

			//									if (((int)(1.0f / currentSpriteAnimation.FPS * 1000.0f)) < DT)
			//									{
			//										framenum++;
			//										currentSpriteAnimation.CurrentFrameRect = currentSpriteAnimation.CurrentFrameRect.Next;
			//										DT = 0; //we need to reset the timer.
			//									}

			//									if (framenum > currentSpriteAnimation.FrameCount)
			//									{
			//										framenum = 1;
			//										currentSpriteAnimation.CurrentFrameRect = currentSpriteAnimation.FrameDrawRects.First;

			//									}

			//									(v as TextBox).Text = framenum.ToString();
			//									(v1 as TextBox).Text = (DT + 15).ToString();

			//									//Increment the Frame
			//									var vimg = c.ContentTemplate.FindName("PreviewAnim_IMG", c);

			//									BitmapImage bmp = bmi;

			//									//int width2 =
			//									//	(int)(currentSpriteAnimation.StartXPos +
			//									//		((currentSpriteAnimation.FrameCount) * currentSpriteAnimation.FrameWidth) > bmp.Width
			//									//			? bmp.Width - ((currentSpriteAnimation.StartXPos +
			//									//											((currentSpriteAnimation.FrameCount - 1) *
			//									//											 currentSpriteAnimation.FrameWidth)))
			//									//			: currentSpriteAnimation.FrameWidth);


			//									var crop = new CroppedBitmap(bmp, new Int32Rect(
			//										(int)currentSpriteAnimation.CurrentFrameRect.Value.XPos,
			//										(int)currentSpriteAnimation.CurrentFrameRect.Value.YPos,
			//										(int)currentSpriteAnimation.CurrentFrameRect.Value.Width,
			//										(int)currentSpriteAnimation.CurrentFrameRect.Value.Height));
			//									// using BitmapImage version to prove its created successfully
			//									(vimg as Image).Source = crop;


			//								}
			//							}
			//						});

			//						//ContentPresenter c =
			//						//	((ContentPresenter) AE_NewAnimStates_IC.ItemContainerGenerator.ContainerFromIndex(i));
			//						//var v = c.ContentTemplate.FindName("PreviewAnim_IMG", c);

			//					}

			//					if (AnimationImporterPreview_Stopwatch.ElapsedMilliseconds > 1000)
			//						AnimationImporterPreview_Stopwatch.Restart(); //we need to reset the timer.
			//				}
			//			}
			//			catch (Exception ex)
			//			{
			//				//Console.WriteLine("Thread mismatch");
			//			}
			//		}
			//	});

			//}

			//if (!animationImportPreviewThread.IsAlive)
			//	animationImportPreviewThread.Start();

			//bNewAnimStateMachine = true;

			//AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Imported SpriteSheet PNG Success!!";
		}

		private void SetupNewAnimation_BTN_Click(object sender, RoutedEventArgs e)
		{
			//bAllowImportAnimPreview = false;
			//Animation_CE_Tree.ItemsSource = null;

			////Lamo this wrong Af
			////AE_NewAnimStates_IC.Items.Add(new object());

			//SpriteAnimation tempAnimation = new SpriteAnimation(CurrentAnimationStateMachine,
			//	"Anim " + AE_NewAnimStates_IC.Items.Count, 0, 0);
			//CurrentAnimationStateMachine.SpriteAnimations.Add(tempAnimation.Name, tempAnimation);
			//AE_NewAnimStates_IC.ItemsSource = CurrentAnimationStateMachine.SpriteAnimations.Values.ToList();

			//Animation_CE_Tree.ItemsSource = CurrentAnimationStateMachine.SpriteAnimations_List;
			//bAllowImportAnimPreview = true;

			////AE_NewAnimStates_IC.ItemsSource = CurrentAnimationStateMachine.SpriteAnimations.Values.ToList();
		}

		private void RefreshFrameRange_TB_KeydDown(object sender, KeyEventArgs e)
		{
			//int index = -1;
			//BitmapImage bmi = new BitmapImage();
			//try
			//{
			//	//Let's get the index of list
			//	index = AE_NewAnimStates_IC.Items.IndexOf((VisualTreeHelper.GetParent(sender as TextBox) as Grid)
			//		.DataContext);
			//	SpriteAnimation currentSpriteAnimation = CurrentAnimationStateMachine.SpriteAnimations.Values.ToList()[index];
			//	//Make sure the data is correct before doing crops
			//	if (currentSpriteAnimation.FrameCount > 0)
			//	{
			//		ContentPresenter c =
			//			((ContentPresenter)AE_NewAnimStates_IC.ItemContainerGenerator.ContainerFromIndex(index));
			//		var v = c.ContentTemplate.FindName("FirstFrame_IMG", c);


			//		bmi.BeginInit();
			//		bmi.CacheOption = BitmapCacheOption.OnLoad;
			//		bmi.UriSource = new Uri(NewAnimimationStatemachineLocation, UriKind.Absolute);
			//		bmi.EndInit();

			//		var crop = new CroppedBitmap(bmi, new Int32Rect((int)currentSpriteAnimation.CurrentFrameRect.Value.XPos,
			//			(int)currentSpriteAnimation.CurrentFrameRect.Value.YPos,
			//			(int)currentSpriteAnimation.CurrentFrameRect.Value.Width,
			//			(int)currentSpriteAnimation.CurrentFrameRect.Value.Height));
			//		// using BitmapImage version to prove its created successfully
			//		(v as Image).Source = crop;

			//		var v2 = c.ContentTemplate.FindName("LastFrame_IMG", c);

			//		//int width2 =
			//		//	(int) (currentSpriteAnimation.StartXPos +
			//		//		((currentSpriteAnimation.FrameCount) * currentSpriteAnimation.FrameWidth) > bmi.Width
			//		//			? bmi.Width - ((currentSpriteAnimation.StartXPos +
			//		//			                ((currentSpriteAnimation.FrameCount - 1) * currentSpriteAnimation.FrameWidth)))
			//		//			: currentSpriteAnimation.FrameWidth);

			//		var crop2 = new CroppedBitmap(bmi,
			//			new Int32Rect(
			//				(int)currentSpriteAnimation.CurrentFrameRect.Value.XPos,
			//				(int)currentSpriteAnimation.CurrentFrameRect.Value.YPos,
			//				(int)currentSpriteAnimation.CurrentFrameRect.Value.Width,
			//				(int)currentSpriteAnimation.CurrentFrameRect.Value.Height
			//			));
			//		(v2 as Image).Source = crop2;


			//		AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Valid Settings displaying previews!";

			//	}
			//}
			//catch (ArgumentException ae)
			//{
			//	// We shouldn't get here anymore because of the sprite sheet editor

			//	//if(bmi.Width < CurrentAnimationStateMachine.SpriteAnimations_List[index].FrameWidth * CurrentAnimationStateMachine.SpriteAnimations_List[index].FrameCount)
			//	//	AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Desired Width of animation EXCEEDS the max width of given sprite sheet PNG";
			//	//else if (bmi.Height < CurrentAnimationStateMachine.SpriteAnimations_List[index].FrameHeight )
			//	//	AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Desired Height of animation EXCEEDS the max width of given sprite sheet PNG";
			//	//else
			//	AE_ImportStatusLog_TB.Text =
			//		DateTime.Now.ToLongTimeString() + ":   Invalid Parameters resetting Text box input";
			//	(sender as TextBox).Text = "0";
			//}
		}

		private void PreviewAnim_CB_OnChecked(object sender, RoutedEventArgs e)
		{
			//AnimationImporterPreview_Stopwatch.Restart();

			//int index = AE_NewAnimStates_IC.Items.IndexOf((VisualTreeHelper.GetParent(sender as CheckBox) as Grid)
			//	.DataContext);

			//ContentPresenter c = ((ContentPresenter)AE_NewAnimStates_IC.ItemContainerGenerator.ContainerFromIndex(index));
			//var v = c.ContentTemplate.FindName("CurrentFrame_TB", c);
			//(v as TextBox).Text = "1";

			//if (CurrentAnimationStateMachine.SpriteAnimations.Values.ToList()[index].FPS == 0)
			//{
			//	AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() +
			//															 ":   FPS CANNOT BE ZERO PREVIEW WILL NOT RUN. Please change";
			//}
			//else
			//{
			//	AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Valid FPS given. Running animation preview";
			//}

		}

		private void FinishImportingNewAnimation_SM_BTN_Click(object sender, RoutedEventArgs e)
		{

			//SceneExplorer_TreeView.ItemsSource = null;


			////In order to deem this as a CORRECT Spritesheet anim State machine... so many words.
			////We need to make sure every SINGLE State we created has valid data!

			//int numOfDefault = 0;
			//bool bstartX, bstartY, bwidth, bheight, bframecount, bFPS, bName;
			//bstartX = bstartY =
			//	bwidth = bheight = bframecount = bFPS = bName = true; //annoying but simple fix for instant weirdness
			//foreach (SpriteAnimation anim in CurrentAnimationStateMachine.SpriteAnimations.Values)
			//{
			//	if (anim.FrameCount <= 0) bframecount &= false;
			//	if (anim.FPS <= 0) bFPS &= false;
			//	if (anim.bIsDefaultState) numOfDefault++;
			//	if (anim.Name == "") bName &= false;
			//}

			//if ((!(bstartX && bstartY && bwidth && bheight && bframecount && bFPS && bName)) || numOfDefault != 1)
			//{
			//	AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Failed Import!";
			//	return;
			//}

			//if (NewSpriteSheetCharacterName == String.Empty)
			//{
			//	AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() +
			//															 ":   Failed Import! Need to name the character who will use this sprite sheet";
			//	return;
			//}

			//if (NewAnimimationStatemachineFileName == String.Empty)
			//{
			//	AE_ImportStatusLog_TB.Text =
			//		DateTime.Now.ToLongTimeString() + ":   Failed Import! Need to name the sprite sheet!";
			//	return;
			//}

			////we need to rename the states to the correct names. So delete and recreate the dictionary entries

			//AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Import Success!";

			//CurrentAnimationStateMachine.SheetName = NewAnimimationStatemachineFileName;
			//CurrentAnimationStateMachine.ImgPathLocation = NewAnimimationStatemachineLocation;
			//CurrentAnimationStateMachine.CharacterName = NewSpriteSheetCharacterName;

			////We need to recreate the dictionary with the correct keys
			//List<SpriteAnimation> tempsSpriteAnimations =
			//	new List<SpriteAnimation>(CurrentAnimationStateMachine.SpriteAnimations.Values);
			//CurrentAnimationStateMachine.SpriteAnimations.Clear();
			//CurrentAnimationStateMachine.Width = NewAnimimationStatemachineTotalWidth;
			//CurrentAnimationStateMachine.Height = NewAnimimationStatemachineTotalHeight;

			//SceneExplorer_TreeView.ItemsSource = ActiveAnimationStateMachines;

			//Image img = new Image();
			//BitmapImage bmi = new BitmapImage();

			//bmi.BeginInit();
			//bmi.CacheOption = BitmapCacheOption.OnLoad;
			//bmi.UriSource = new Uri(NewAnimimationStatemachineLocation, UriKind.Absolute);
			//bmi.EndInit();
			//Bitmap pain = BitmapImage2Bitmap(bmi);
			//pain.SetResolution(96.0f, 96.0f);
			//bmi = ToBitmapImage(pain);
			//img.Source = bmi;

			//CurrentSpriteSheet_Image = bmi;

			//int count = 0;
			//foreach (var spriteanim in tempsSpriteAnimations)
			//{
			//	// spriteanim.FrameDrawRects.Clear();
			//	//for (int i = 0; i < spriteanim.FrameCount; i++)
			//	//{
			//	//	spriteanim.FrameDrawRects.AddLast(new Rect(
			//	//		(int)spriteanim.CurrentFrameRect.Value.X,
			//	//		(int)spriteanim.CurrentFrameRect.Value.Y,
			//	//		(int)spriteanim.CurrentFrameRect.Value.Width,
			//	//		(int)spriteanim.CurrentFrameRect.Value.Height
			//	//		)
			//	//	);

			//	//}

			//	CurrentAnimationStateMachine.SpriteAnimations.Add(spriteanim.Name, spriteanim);

			//	TreeView tempTV =
			//		(TreeView)ContentLibrary_Control.Template.FindName("AnimationEditor_CE_TV", ContentLibrary_Control);
			//	var vvv = (ContentLibrary_Control.Template.FindName("AnimationEditor_CE_TV", ContentLibrary_Control));
			//	var vv = tempTV.ItemContainerGenerator.ContainerFromIndex(count++);

			//	(vv as TreeViewItem).ExpandSubtree();
			//	(vv as TreeViewItem).ApplyTemplate();
			//	//var v = (vv as TreeViewItem).ItemTemplate.FindName("Thumbnail", (vv as TreeViewItem));


			//	var v = FindElementByName<Image>((vv as TreeViewItem), "Thumbnail");


			//	var crop = new CroppedBitmap(bmi, new Int32Rect(
			//		(int)spriteanim.CurrentFrameRect.Value.XPos,
			//		(int)spriteanim.CurrentFrameRect.Value.YPos,
			//		(int)spriteanim.CurrentFrameRect.Value.Width,
			//		(int)spriteanim.CurrentFrameRect.Value.Height
			//	));
			//	// using BitmapImage version to prove its created successfully
			//	(v as Image).Source = crop;

			//	if (count == 1)
			//	{
			//		CurrentlySelectedAnimationState = spriteanim;
			//	}

			//}

			//// CurrentSpriteSheet_Image = bmi;

			////dummy binding force because 2 years ago me was DUMB
			//SceneExplorer_TreeView.ItemsSource = ActiveAnimationStateMachines;
			//_allowAnimationExporting = true;

			////Reset the Animation Importer
			//_allowAnimationExporting = false;

			//AE_NewAnimSM_MainGrid.Visibility = Visibility.Hidden;
			//AE_CurrentAnimSM_Grid.Visibility = Visibility.Visible;

			////Reset all the properties!
			//NewAnimimationStatemachineFileName = "";
			//NewAnimimationStatemachineLocation = "";
			//NewSpriteSheetCharacterName = "";
			//NewAnimimationStatemachineTotalWidth = -1;
			//NewAnimimationStatemachineTotalHeight = -1;
			//bNewAnimStateMachine = false;

			//bAllowImportAnimPreview = false;
			//AE_NewAnimStates_IC.ItemsSource = null;


		}

		/// <summary>
		/// this is hear to allow you to add new states to the sprite sheet state machine!
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddAnimationStateToSM_BTN_Click(object sender, RoutedEventArgs e)
		{
			//if (ActiveAnimationStateMachines.Count > 0 && CurrentAnimationStateMachine != null)
			//{

			//	//throw new NotImplementedException();
			//	Window ae = new AddEditAnimationStateMachine(CurrentSpriteSheet_Image, CurrentAnimationStateMachine, null)
			//	{ AddToStatemachine = AddToStatemachineFromForm };
			//	ae.ShowDialog();
			//}
			//else
			//{
			//	EngineOutputLog.AddErrorLogItem(-4, "New Animation Machine hasn't been created or opened yet!",
			//		"Animation Editor", true);
			//	EngineOutputLog.AddLogItem("Animation Machine error. See Error log for details");
			//	if (resizeGrid.RowDefinitions.Last().Height.Value < 100)
			//		resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
			//	OutputLogSpliter.IsEnabled = true;
			//}
		}

		private void AddToStatemachineFromForm(AnimationState retAnimationState, String name, bool bIsAdding,
			bool bDefaultChanged, String oldkey = "")
		{
			//SceneExplorer_TreeView.ItemsSource = null;
			//Animation_CE_Tree.ItemsSource = null;

			////Reset ALL sprite Animations Default values
			//if (bDefaultChanged)
			//{
			//	foreach (SpriteAnimation spriteAnimation in CurrentAnimationStateMachine.SpriteAnimations.Values)
			//	{
			//		spriteAnimation.bIsDefaultState = false;
			//	}

			//	retAnimationState.bIsDefaultState = bDefaultChanged;
			//}



			//if (bIsAdding)
			//	CurrentAnimationStateMachine.SpriteAnimations.Add(name, retspriteanimation);
			//else
			//{
			//	//We are editing. Let's assume the user wants to change the name which means we need to remove and re add to dictionary

			//	CurrentAnimationStateMachine.SpriteAnimations.Remove(oldkey);
			//	CurrentAnimationStateMachine.SpriteAnimations.Add(name, retspriteanimation);
			//}

			////ActiveAnimationStateMachines.Add(CurrentAnimationStateMachine);
			//SceneExplorer_TreeView.ItemsSource = ActiveAnimationStateMachines;
			//Animation_CE_Tree.ItemsSource = CurrentAnimationStateMachine.SpriteAnimations.Values;

		}


		public T FindElementByName<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
		{
			T childElement = null;
			var nChildCount = VisualTreeHelper.GetChildrenCount(element);
			for (int i = 0; i < nChildCount; i++)
			{
				FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

				if (child == null)
					continue;

				if (child is T && child.Name.Equals(sChildName))
				{
					childElement = (T)child;
					break;
				}

				childElement = FindElementByName<T>(child, sChildName);

				if (childElement != null)
					break;
			}

			return childElement;
		}

		private void AnimationPreviewThumbnail_Image_Enter(object sender, MouseEventArgs e)
		{
			//if (CurrentSpriteSheet_Image.UriSource == null) return;

			////CurrentAnimPreviewImages_CE 
			//CurrentAnimPreviewImages_CE.Clear();

			////First up which Item are we in/binded too?
			//var item = new object();
			//if (sender is StackPanel sp)
			//{
			//	item = (sender as StackPanel).DataContext;
			//}
			//else
			//{
			//	item = (VisualTreeHelper.GetParent(sender as Image) as StackPanel).DataContext;
			//}

			//var index =
			//	(ContentLibrary_Control.Template.FindName("AnimationEditor_CE_TV", ContentLibrary_Control) as TreeView).Items
			//	.IndexOf(item);

			//if (index >= 0)
			//{
			//	//Now that we know which Animation we are in let's Preload the Frames to avoid uneeded GC every couple frames
			//	SpriteAnimation spriteanim = CurrentAnimationStateMachine.SpriteAnimations.Values.ToList()[index];
			//	for (int i = 0; i < spriteanim.FrameCount; i++)
			//	{
			//		//this eyesore will catch bad sprite sheets.So it doesn't go out of bounds....
			//		//int width2 =
			//		//	(int)(spriteanim.StartXPos +
			//		//		((spriteanim.FrameCount) * spriteanim.FrameWidth) > CurrentSpriteSheet_Image.Width
			//		//			? CurrentSpriteSheet_Image.Width - ((spriteanim.StartXPos + ((spriteanim.FrameCount - 1) * spriteanim.FrameWidth))) : spriteanim.FrameWidth);


			//		CurrentAnimPreviewImages_CE.Add(
			//			(new CroppedBitmap(CurrentSpriteSheet_Image, new Int32Rect(
			//				(int)spriteanim.CurrentFrameRect.Value.XPos,
			//				(int)spriteanim.CurrentFrameRect.Value.YPos,
			//				(int)spriteanim.CurrentFrameRect.Value.Width,
			//				(int)spriteanim.CurrentFrameRect.Value.Height
			//			))));
			//	}
			//}

			//if (sender is StackPanel ss)
			//{
			//	PreviewAnimUI_Image_PTR = ss.Children[0] as Image;

			//}
			//else
			//{
			//	PreviewAnimUI_Image_PTR = sender as Image;
			//}

			//PreviewAnimUI_Image_PTR.Tag = 1;
			//Animation_CE_Preview_Stopwatch.Start();
			//PreviewAnim_Data_PTR = (SpriteAnimation)item;


		}

		private void AnimationPreviewThumbnail_Image_Leave(object sender, MouseEventArgs e)
		{
			//PreviewAnimUI_Image_PTR = null;
			//Animation_CE_Preview_Stopwatch.Reset();
		}

		//Called on Load, so this always happens
		private void SetupPreviewAnimationThread_CE()
		{
			//previewAnimThread_CE = new Thread(() =>
			//{
			//	while (true)
			//	{

			//		try
			//		{
			//			Thread.Sleep(15);
			//		}
			//		catch
			//		{
			//			Console.WriteLine("animation preview thread FOR CE == interrupted");
			//			break;
			//		}

			//		if (PreviewAnimUI_Image_PTR != null)
			//		{
			//			//Let's make the image "move"
			//			if (Animation_CE_Preview_Stopwatch.ElapsedMilliseconds > (1.0f / PreviewAnim_Data_PTR.FPS * 1000.0f))
			//			{
			//				Animation_CE_Preview_Stopwatch.Restart(); //Reset
			//				Dispatcher.Invoke(() =>
			//				{
			//					//inc frame
			//					int? frame = PreviewAnimUI_Image_PTR?.Tag as int?;
			//					if (frame != null)
			//					{
			//						if (frame == PreviewAnim_Data_PTR.FrameCount)
			//						{
			//							frame = 1;
			//						}
			//						else
			//						{
			//							frame++;
			//						}

			//						PreviewAnimUI_Image_PTR.Source = CurrentAnimPreviewImages_CE[(int)frame - 1];
			//						PreviewAnimUI_Image_PTR.Tag = frame;
			//					}
			//				});
			//			}
			//		}
			//	}
			//});
			//previewAnimThread_CE.Start();

		}

		// Called on Load, so this always happens
		// Used to run the animation preview on the canvas
		private void SetupSelectedAnimationThread()
		{
			//selectedAnimThread_CE = new Thread(() =>
			//{
			//	while (true)
			//	{
			//		try
			//		{
			//			Thread.Sleep(15);
			//		}
			//		catch
			//		{
			//			Console.WriteLine("selected animation thread == interrupted");
			//			break;
			//		}

			//		if (CurrentSelectedAnimImages_List.Count > 0 && AE_CurrentAnimSM_Grid.Visibility == Visibility.Visible &&
			//				CurrentlySelectedAnimationState != null)
			//		{
			//			//Let's make the image "move"
			//			if (AnimationSelected_Stopwatch.ElapsedMilliseconds > (1.0f / CurrentlySelectedAnimationState.FPS * 1000.0f))
			//			{
			//				AnimationSelected_Stopwatch.Restart(); //Reset
			//				try
			//				{
			//					Dispatcher.Invoke(() =>
			//					{
			//						//inc frame
			//						int? frame = CurrentBaseLayerAnimation_Img?.Tag as int?;
			//						if (frame != null)
			//						{

			//							if (frame == CurrentlySelectedAnimationState.FrameCount)
			//							{
			//								frame = 1;
			//							}
			//							else
			//							{
			//								frame++;
			//							}

			//							// CurrentBaseLayerAnimation_Img.Source = CurrentSelectedAnimImages_List[(int) frame - 1];
			//							AnimationEditor_Canvas.Children.Remove(CurrentBaseLayerAnimation_Img);
			//							CurrentBaseLayerAnimation_Img = new Image();
			//							CurrentBaseLayerAnimation_Img.Source = CurrentSelectedAnimImages_List[(int)frame - 1];
			//							CurrentBaseLayerAnimation_Img.Stretch = Stretch.Fill;
			//							CurrentBaseLayerAnimation_Img.Width =
			//								CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int)frame) - 1].Width;
			//							CurrentBaseLayerAnimation_Img.MaxWidth =
			//								CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int)frame) - 1].Width;
			//							CurrentBaseLayerAnimation_Img.Height =
			//								CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int)frame) - 1].Height;
			//							CurrentBaseLayerAnimation_Img.MaxHeight =
			//								CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int)frame) - 1].Height;
			//							CurrentBaseLayerAnimation_Img.StretchDirection = StretchDirection.Both;
			//							AnimationEditor_Canvas.Children.Add(CurrentBaseLayerAnimation_Img);
			//							CurrentBaseLayerAnimation_Img.UpdateLayout();
			//							AnimationEditor_Canvas.UpdateLayout();


			//							CurrentBaseLayerAnimation_Img.Tag = frame;
			//							CurrentActiveAnimationCurrentFrame_TB.Text = frame.ToString();

			//							int middleX = (int)(AnimationEditor_BackCanvas.ActualWidth / 2.0f);
			//							int middleY = (int)(AnimationEditor_BackCanvas.ActualHeight / 2.0f);
			//							int newX = middleX - CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int)frame) - 1]
			//								.RenderPointX;
			//							//newX = newX + ((CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int) frame) - 1].Width -
			//							//                (int) CurrentBaseLayerAnimation_Img.ActualWidth) / 2);

			//							int newY = middleY - CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int)frame) - 1]
			//								.RenderPointY;
			//							//newY = newY + ((CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int) frame) - 1].Height -
			//							//                (int) CurrentBaseLayerAnimation_Img.Height) / 2);
			//							//Canvas.SetLeft(CurrentBaseLayerAnimation_Img, middleX - ((int)(CurrentBaseLayerAnimation_Img.ActualWidth/ 2.0f)));
			//							//Canvas.SetTop(CurrentBaseLayerAnimation_Img, middleY - ((int)(CurrentBaseLayerAnimation_Img.ActualHeight/ 2.0f)));

			//							Canvas.SetLeft(CurrentBaseLayerAnimation_Img, newX);
			//							Canvas.SetTop(CurrentBaseLayerAnimation_Img, newY);

			//							Console.WriteLine(String.Format(
			//								"w:{0}, h:{1} newX:{2}, newY:{3} | iWidth:{4}, iHeight:{5}, mx:{6}, my:{7}, FRAME:{8}",
			//								CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int)frame) - 1].Width,
			//								CurrentlySelectedAnimationState.FrameDrawRects.ToList()[((int)frame) - 1].Height,
			//								newX, newY, (int)CurrentBaseLayerAnimation_Img.ActualWidth,
			//								(int)CurrentBaseLayerAnimation_Img.ActualHeight,
			//								middleX, middleY, frame - 1));

			//						}



			//						for (int i = 0; i < AnimationSubLayerImages_List.Count; i++)
			//						{
			//							if (frame > AnimationSubLayerImages_List[i].Count)
			//								continue;
			//							(AnimationLayersPreview_Canvas_IC.Children[i] as Image).Source =
			//								AnimationSubLayerImages_List[i][(int)frame - 1];
			//							(AnimationLayersPreview_Canvas_IC.Children[i] as Image).Tag = frame.ToString();
			//						}


			//						//int count = 0;
			//						//foreach (List<CroppedBitmap> anim in AnimationSubLayerImages_List)
			//						//{
			//						//	//Step one what frame is it?
			//						//	int? framesublayer = frame;//(AnimationLayersPreview_Canvas_IC.Children[count] as Image)?.Tag as int?;

			//						//	SpriteSheet sheet = currentLayeredSpriteSheet.ActiveSubLayerSheet
			//						//		[currentLayeredSpriteSheet.subLayerSpritesheets_Dict.Keys.ToList()[count]];

			//						//	//make the image actually appear
			//						//	//this needs the +1 because of base layer
			//						//	(AnimationLayersPreview_Canvas_IC.Children[count+1] as Image).Source =
			//						//		anim[(int)framesublayer];

			//						//	//Check for frames
			//						//	if (framesublayer == sheet.CurrentAnimation.FrameCount)
			//						//	{
			//						//		framesublayer = 1;
			//						//	}
			//						//	else
			//						//	{
			//						//		framesublayer++;
			//						//	}

			//						//	//this needs the +1 because of base layer
			//						//	(AnimationLayersPreview_Canvas_IC.Children[count+1] as Image).Tag = framesublayer.ToString();
			//						//	count++;
			//						//}



			//					});
			//				}
			//				catch
			//				{
			//					Console.WriteLine("Error in Anim Preiview Thread");
			//				}
			//			}
			//		}
			//	}
			//});
			//selectedAnimThread_CE.Start();

		}


		private void OpenLayeredSpriteSheetFile_MI_Click(object sender, RoutedEventArgs e)
		{
			//SceneExplorer_TreeView.ItemsSource = null;
			//Animation_CE_Tree.ItemsSource = null;
			//Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			//{
			//	Title = "Open Layered Animation State machine File",
			//	FileName = "", //default file name
			//	InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Animations"),
			//	Filter = "Layered Animation State Machine files (*.lanim)|*.lanim",
			//	RestoreDirectory = true
			//};

			//Nullable<bool> result = dlg.ShowDialog();
			//// Process save file dialog box results
			//string filename = "";
			//if (result == true)
			//{
			//	// Save document
			//	filename = dlg.FileName;
			//	filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			//}
			//else return; //invalid name

			//Console.WriteLine(dlg.FileName);
			//LayeredSpriteSheet ret = LayeredSpriteSheet.ImportlayeredAnimationSheet(dlg.FileName);

			////Now that we have a layed sheet imported let's add all that data to the screen
			//currentLayeredSpriteSheet = ret;

			//Console.WriteLine(AnimationSubLayer_CB.Items.Count);
			//for (int i = AnimationSubLayer_CB.Items.Count - 1; i >= 2; i--)
			//{
			//	AnimationSubLayer_CB.Items.RemoveAt(i)
			//		;
			//}

			//AnimationLayersPreview_Canvas_IC.Children.Clear();
			//for (int i = 0; i < ret.subLayerSpritesheets_Dict.Keys.Count; i++)
			//{
			//	AnimationSubLayer_CB.Items.Add(ret.subLayerSpritesheets_Dict.Keys.ToList()[i]);
			//	//we have added the layer Data wise, but not display wise don't forget
			//	Image img = new Image() { Tag = "1" };
			//	Grid.SetZIndex(img, currentLayeredSpriteSheet.subLayerSpritesheets_Dict.Count);
			//	AnimationLayersPreview_Canvas_IC.Children.Add(img);
			//	AnimationSubLayerImages_List.Add(new List<CroppedBitmap>());

			//	if (i == 0) AnimationSubLayer_CB.SelectedIndex = 2;
			//}

			//if (AnimationSubLayer_CB.SelectedIndex == 2)
			//{
			//	CurrentSubLayerSpriteSheets_LB.ItemsSource =
			//		ret.subLayerSpritesheets_Dict[AnimationSubLayer_CB.Items[2].ToString()];


			//	if (currentLayeredSpriteSheet.subLayerSpritesheets_Dict.Keys.Count > 0)
			//	{
			//		CurrentSubLayerAnimStates_LB.ItemsSource =
			//			currentLayeredSpriteSheet
			//				.subLayerSpritesheets_Dict[currentLayeredSpriteSheet.subLayerSpritesheets_Dict.Keys.ToList()[0]]
			//				.ToList();
			//	}
			//}

			////at this point we need to make sure the user can access the actual animations and change them
			////at will
			//CurrentAnimationStateMachine = ret.BaseLayer;
			//ActiveAnimationStateMachines.Add(CurrentAnimationStateMachine);
			//SceneExplorer_TreeView.ItemsSource = ActiveAnimationStateMachines;
			//Animation_CE_Tree.ItemsSource = CurrentAnimationStateMachine.SpriteAnimations.Values;

			//Animation_CE_Tree.UpdateLayout();
			//SceneExplorer_TreeView.UpdateLayout();

			////Set up the PNG This will allow it to NOT be locked
			//BitmapImage bmi = new BitmapImage();
			//bmi.BeginInit();
			//bmi.CacheOption = BitmapCacheOption.OnLoad;
			//bmi.UriSource = new Uri(CurrentAnimationStateMachine.ImgPathLocation, UriKind.Absolute);
			//bmi.EndInit();
			//CurrentSpriteSheet_Image = bmi;

			////Set up the first frame thumbnails for animation states
			//int count = 0;
			//foreach (var spriteanim in CurrentAnimationStateMachine.SpriteAnimations.Values)
			//{
			//	if (count == 1)
			//	{
			//		//CurrentBaseLayerAnimation_Img.Source = crop;

			//		CurrentActiveAnimationName_TB.Text = spriteanim.Name;
			//		CurrentActiveAnimationFPS_TB.Text = spriteanim.FPS.ToString();
			//		CurrentActiveAnimationCurrentFrame_TB.Text = "1";
			//		CurrentBaseLayerAnimation_Img.Tag = 1;
			//		CurrentlySelectedAnimationState = spriteanim;

			//	}
			//}

			//AE_NewAnimSM_MainGrid.Visibility = Visibility.Hidden;
			//AE_CurrentAnimSM_Grid.Visibility = Visibility.Visible;

			//if (!AnimationSelected_Stopwatch.IsRunning)
			//	AnimationSelected_Stopwatch.Start();



		}


		private void OpenSpriteSheetFile_MI_Click(object sender, RoutedEventArgs e)
		{

			//CurrentSubLayerSpriteSheets_LB.ItemsSource = null;
			//SceneExplorer_TreeView.ItemsSource = null;
			//Animation_CE_Tree.ItemsSource = null;
			//Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			//{
			//	Title = "Open Animation State machine File",
			//	FileName = "", //default file name
			//	InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Animations"),
			//	Filter = "Animation State Machine files (*.anim)|*.anim",
			//	RestoreDirectory = true
			//};

			//Nullable<bool> result = dlg.ShowDialog();
			//// Process save file dialog box results
			//string filename = "";
			//if (result == true)
			//{
			//	// Save document
			//	filename = dlg.FileName;
			//	filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			//}
			//else return; //invalid name

			//Console.WriteLine(dlg.FileName);

			//SpriteSheet retSheet = SpriteSheet.ImportSpriteSheet(dlg.FileName);
			//CurrentAnimationStateMachine = retSheet;
			//ActiveAnimationStateMachines.Add(CurrentAnimationStateMachine);
			//SceneExplorer_TreeView.ItemsSource = ActiveAnimationStateMachines;
			//Animation_CE_Tree.ItemsSource = CurrentAnimationStateMachine.SpriteAnimations.Values;

			//Animation_CE_Tree.UpdateLayout();
			//SceneExplorer_TreeView.UpdateLayout();

			////Set up the PNG This will allow it to NOT be locked
			//BitmapImage bmi = new BitmapImage();
			//bmi.BeginInit();
			//bmi.CacheOption = BitmapCacheOption.OnLoad;
			//bmi.UriSource = new Uri(CurrentAnimationStateMachine.ImgPathLocation, UriKind.Absolute);
			//bmi.EndInit();
			//CurrentSpriteSheet_Image = bmi;

			////Set up the first frame thumbnails for animation states
			//int count = 0;
			//foreach (var spriteanim in CurrentAnimationStateMachine.SpriteAnimations.Values)
			//{
			//	if (count == 1)
			//	{
			//		//CurrentBaseLayerAnimation_Img.Source = crop;

			//		CurrentActiveAnimationName_TB.Text = spriteanim.Name;
			//		CurrentActiveAnimationFPS_TB.Text = spriteanim.FPS.ToString();
			//		CurrentActiveAnimationCurrentFrame_TB.Text = "1";
			//		CurrentBaseLayerAnimation_Img.Tag = 1;
			//		CurrentlySelectedAnimationState = spriteanim;

			//	}
			//}

			//AE_NewAnimSM_MainGrid.Visibility = Visibility.Hidden;
			//AE_CurrentAnimSM_Grid.Visibility = Visibility.Visible;

			//if (!AnimationSelected_Stopwatch.IsRunning)
			//	AnimationSelected_Stopwatch.Start();

			////Adding this to the layered Sprite sheet. THIS import must be changed later.
			//currentLayeredSpriteSheet = new LayeredSpriteSheet(retSheet);


		}

		private void SaveSpriteSheetFileAs_MI_Click(object sender, RoutedEventArgs e)
		{

			//Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
			//{
			//	Title = "Sprite Sheet Machine",
			//	FileName = "", //default file name
			//	Filter = "Animation State Machine Files (*.anim)|*.anim",
			//	FilterIndex = 0,
			//	InitialDirectory = ProjectFilePath.Replace(".gem", "_Game\\Content\\Animations"),
			//	RestoreDirectory = true
			//};

			//Nullable<bool> result = dlg.ShowDialog();
			//// Process save file dialog box results
			//string filename = "";
			//if (result == true)
			//{
			//	// Save document
			//	filename = dlg.FileName;
			//	filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			//}
			//else return; //invalid name

			//SpriteSheet.ExportSpriteSheet(CurrentAnimationStateMachine, dlg.FileName.Replace(".anim", ""));

		}

		private void NewSpriteSheetMachine_MenuItem_OnClick(object sender, RoutedEventArgs e)
		{

			//_allowAnimationExporting = false;

			//CurrentlySelectedAnimationState = null;

			//AE_NewAnimSM_MainGrid.Visibility = Visibility.Visible;
			//AE_CurrentAnimSM_Grid.Visibility = Visibility.Hidden;

			////Reset all the properties!
			//NewAnimimationStatemachineFileName = "";
			//NewAnimimationStatemachineLocation = "";
			//NewSpriteSheetCharacterName = "";
			//NewAnimimationStatemachineTotalWidth = -1;
			//NewAnimimationStatemachineTotalHeight = -1;
			//bNewAnimStateMachine = false;

			//bAllowImportAnimPreview = false;
			//AE_NewAnimStates_IC.ItemsSource = null;

			//if (CurrentAnimationStateMachine != null)
			//{
			//	CurrentAnimationStateMachine.SpriteAnimations.Clear();
			//	CurrentAnimationStateMachine = null;
			//}
		}

		private void AnimationSceneExplorer_TreeView_OnSelectedItemChanged(object sender,
			RoutedPropertyChangedEventArgs<object> e)
		{
			//SpriteSheet sheet = (sender as TreeViewItem).DataContext as SpriteSheet;

		}

		public BitmapSource ChangeDpi(BitmapSource source, double dpiX, double dpiY)
		{
			var dpiTransform = new TransformedBitmap(source, new ScaleTransform(dpiX / 96, dpiY / 96));
			return dpiTransform;
		}

		private BitmapImage ChangeImageDpi(BitmapImage sourceImage, double newDpiX, double newDpiY)
		{
			var bitmapFrame = BitmapFrame.Create(sourceImage);
			var metadata = (BitmapMetadata)bitmapFrame.Metadata.Clone();

			// Update the DPI values in the metadata
			metadata.SetQuery("/app1/ifd/{ushort=282}", newDpiX);
			metadata.SetQuery("/app1/ifd/{ushort=283}", newDpiY);

			var encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bitmapFrame, bitmapFrame.Thumbnail, metadata, bitmapFrame.ColorContexts));

			// Create a new MemoryStream to hold the encoded image data
			using (var memoryStream = new MemoryStream())
			{
				// Save the encoded image data to the MemoryStream
				encoder.Save(memoryStream);

				// Create a new BitmapImage from the MemoryStream
				var newImage = new BitmapImage();
				newImage.BeginInit();
				newImage.CacheOption = BitmapCacheOption.OnLoad;
				newImage.StreamSource = new MemoryStream(memoryStream.ToArray());
				newImage.EndInit();

				return newImage;
			}
		}

		private void ChangeActiveSpriteSheet_TVI_Click(object sender, MouseButtonEventArgs e)
		{
			//AnimationSelected_Stopwatch.Stop();
			//Animation_CE_Tree.ItemsSource = null;

			//SpriteSheet sheet = (sender as StackPanel).DataContext as SpriteSheet;
			//if (sheet != null)
			//{
			//	CurrentAnimationStateMachine = sheet;
			//	//ActiveAnimationStateMachines.Add(CurrentAnimationStateMachine);
			//	//SceneExplorer_TreeView.ItemsSource = ActiveAnimationStateMachines;
			//	Animation_CE_Tree.ItemsSource = CurrentAnimationStateMachine.SpriteAnimations.Values;

			//	Animation_CE_Tree.UpdateLayout();
			//	//SceneExplorer_TreeView.UpdateLayout();

			//	//Set up the PNG This will allow it to NOT be locked
			//	BitmapImage bmi = new BitmapImage();
			//	bmi.BeginInit();
			//	bmi.CacheOption = BitmapCacheOption.OnLoad;
			//	bmi.UriSource = new Uri(CurrentAnimationStateMachine.ImgPathLocation, UriKind.Absolute);

			//	bmi.EndInit();
			//	CurrentSpriteSheet_Image = bmi;

			//	//Set up the first frame thumbnails for animation states
			//	int count = 0;
			//	foreach (var spriteanim in CurrentAnimationStateMachine.SpriteAnimations.Values)
			//	{
			//		if (count == 1)
			//		{
			//			//CurrentBaseLayerAnimation_Img.Source = crop;

			//			CurrentActiveAnimationName_TB.Text = spriteanim.Name;
			//			CurrentActiveAnimationFPS_TB.Text = spriteanim.FPS.ToString();
			//			CurrentActiveAnimationCurrentFrame_TB.Text = "1";
			//			CurrentBaseLayerAnimation_Img.Tag = 1;
			//			CurrentlySelectedAnimationState = spriteanim;

			//		}
			//	}

			//	AE_NewAnimSM_MainGrid.Visibility = Visibility.Hidden;
			//	AE_CurrentAnimSM_Grid.Visibility = Visibility.Visible;

			//	if (!AnimationSelected_Stopwatch.IsRunning)
			//		AnimationSelected_Stopwatch.Start();

			//}

			//Animation_CE_Tree.ItemsSource = CurrentAnimationStateMachine.SpriteAnimations.Values;
		}

		private void PlayCurrentSelectedAnimation_BTN_Click(object sender, RoutedEventArgs e)
		{
			//	if (!AnimationSelected_Stopwatch.IsRunning)
			//		AnimationSelected_Stopwatch.Start();
		}

		private void PauseCurrentSelectedAnimation_BTN_Click(object sender, RoutedEventArgs e)
		{
			//AnimationSelected_Stopwatch.Stop();
		}

		private void ResetCurrentSelectedAnimation_BTN_Click(object sender, RoutedEventArgs e)
		{
			////throw new NotImplementedException();
			//CurrentActiveAnimationCurrentFrame_TB.Text = "1";
			//AnimationSelected_Stopwatch.Restart();

		}

		private void AnimationEditor_CE_TV_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			//CurrentSelectedAnimImages_List.Clear();
			//TreeView tvtemp = sender as TreeView;
			//if (tvtemp != null)
			//{
			//	SpriteAnimation item = tvtemp.SelectedItem as SpriteAnimation;
			//	item.CurrentFrameRect = item.FrameDrawRects.First;
			//	if (item == null) return;

			//	// First up we need to get all the frames, and create CROPPED images for them!
			//	for (int i = 0; i < item.FrameCount; i++)
			//	{
			//		CurrentSelectedAnimImages_List.Add(
			//			(new CroppedBitmap(CurrentSpriteSheet_Image, new Int32Rect(
			//				(int)item.FrameDrawRects.ToList()[i].XPos,
			//				(int)item.FrameDrawRects.ToList()[i].YPos,
			//				(int)item.FrameDrawRects.ToList()[i].Width,
			//				(int)item.FrameDrawRects.ToList()[i].Height
			//			))));
			//		item.CurrentFrameRect = item.CurrentFrameRect.Next;
			//	}

			//	AnimationSubLayer_CB.ItemsSource = item.NamesOfSubLayers;
			//	//foreach (var namesOfSubLayer in item.NamesOfSubLayers)
			//	//{
			//	//	AnimationSubLayer_CB.Items.Add(namesOfSubLayer);
			//	//}

			//	CurrentActiveAnimationName_TB.Text = item.Name;
			//	CurrentActiveAnimationFPS_TB.Text = item.FPS.ToString();
			//	CurrentActiveAnimationCurrentFrame_TB.Text = "1";
			//	CurrentBaseLayerAnimation_Img.Tag = 1;
			//	CurrentlySelectedAnimationState = item;
			//}

			//if (!AnimationSelected_Stopwatch.IsRunning)
			//	AnimationSelected_Stopwatch.Start();

			//AnimationChangeEvents_Properties_Tree.ItemsSource = null;
			//AnimationAudioEvents_Properties_Tree.ItemsSource = null;

			//AnimationChangeEvents_Properties_Tree.ItemsSource =
			//	CurrentlySelectedAnimationState.GetAnimationEvents().FindAll(x => x is ChangeAnimationEvent);
			//AnimationAudioEvents_Properties_Tree.ItemsSource =
			//	CurrentlySelectedAnimationState.GetAnimationEvents().FindAll(x => x is AudioEvent);

		}

		public static BitmapSource ConvertBitmapTo96DPI(BitmapImage bitmapImage)
		{
			double dpi = 96;
			int width = bitmapImage.PixelWidth;
			int height = bitmapImage.PixelHeight;

			int stride = width * bitmapImage.Format.BitsPerPixel;
			byte[] pixelData = new byte[stride * height];
			bitmapImage.CopyPixels(pixelData, stride, 0);

			return BitmapSource.Create(width, height, dpi, dpi, bitmapImage.Format, null, pixelData, stride);
		}

		private void Thumbnail_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			//StackPanel sp = sender as StackPanel;
			//if (sp != null)
			//{
			//	//if(MouseRightButtonDown = "Thumbnail_OnMouseRightButtonDown")
			//	//We need to show the Context menu

			//	//are we clicked on a level? Then create a new layer
			//	ContextMenu cm = this.FindResource("EditorObjectAnimationPreviewsList_Template") as ContextMenu;
			//	cm.PlacementTarget = sp;
			//	cm.IsOpen = true;
			//	cm.StaysOpen = true;

			//	//First up which Item are we in/binded too?
			//	if (CurrentAnimationStateMachine.SpriteAnimations.ContainsValue(
			//		((sender as StackPanel).DataContext) as SpriteAnimation))
			//	{
			//		var sa = CurrentAnimationStateMachine.SpriteAnimations.FirstOrDefault(
			//			x => x.Value == ((sp).DataContext) as SpriteAnimation);

			//		_animationState_TV_QueriedKey = sa.Key;

			//	}

			//}
		}


		private void EditAnimationState_MI_Click(object sender, RoutedEventArgs e)
		{

			////throw new NotImplementedException();
			//Window ae = new AddEditAnimationStateMachine(CurrentSpriteSheet_Image, CurrentAnimationStateMachine,
			//		CurrentAnimationStateMachine.SpriteAnimations[_animationState_TV_QueriedKey])
			//{ AddToStatemachine = AddToStatemachineFromForm };
			//ae.ShowDialog();

		}

		private void RemoveAnimationState_MI_Click(object sender, RoutedEventArgs e)
		{
			//Animation_CE_Tree.ItemsSource = null;
			//CurrentAnimationStateMachine.SpriteAnimations.Remove(_animationState_TV_QueriedKey);
			//Animation_CE_Tree.ItemsSource = CurrentAnimationStateMachine.SpriteAnimations.Values;

			//_animationState_TV_QueriedKey = String.Empty;

		}

		//Called when i update the Animation State machine sources. After edit, or add.
		private void UpdateAnimationStateThumbnail_Event(object sender, RoutedEventArgs e)
		{
			//if (CurrentAnimationStateMachine == null || ActiveAnimationStateMachines.Count <= 0) return;
			//Console.WriteLine("updated TREEVIEW");
			//if (CurrentAnimationStateMachine.SpriteAnimations.Count > 0)
			//{
			//	StackPanel sp = sender as StackPanel;

			//	if (sp != null)
			//	{
			//		SpriteAnimation item = null;
			//		item = sp.DataContext as SpriteAnimation;
			//		//var index = (ContentLibrary_Control.Template.FindName("AnimationEditor_CE_TV", ContentLibrary_Control) as TreeView).Items.IndexOf(item);

			//		BitmapImage bmp = CurrentSpriteSheet_Image;
			//		//var crop = new CroppedBitmap(bmp, new Int32Rect(
			//		//	(int)item.CurrentFrameRect.Value.X, 
			//		//	(int)item.CurrentFrameRect.Value.Y, 
			//		//	(int)item.CurrentFrameRect.Value.Width, 
			//		//	(int)item.CurrentFrameRect.Value.Height 
			//		//	));
			//		//(sp.Children[0] as Image).Source = crop;

			//	}
			//}

		}

		private void TestingAnimProperties_BTN_Click(object sender, RoutedEventArgs e)
		{
			//AnimationChangeEvents_Properties_Tree.ItemsSource = null;

			//List<ChangeAnimationEvent> testing = new List<ChangeAnimationEvent>();

			//testing.Add(new ChangeAnimationEvent("This guy", "other dude", true));

			//AnimationChangeEvents_Properties_Tree.Items.Add(testing[0]);


			//AnimationChangeEvents_Properties_Tree.UpdateLayout();
		}

		private void AddNewAnimationChangeEvent_BTN_Click(object sender, RoutedEventArgs e)
		{
			//if (CurrentlySelectedAnimationState == null || CurrentAnimationStateMachine == null) return;

			//AnimationChangeEvents_Properties_Tree.ItemsSource = null;
			//CurrentlySelectedAnimationState.AddAnimationEvents(
			//	new ChangeAnimationEvent(CurrentlySelectedAnimationState.Name, "NONE", true));
			//AnimationChangeEvents_Properties_Tree.ItemsSource =
			//	CurrentlySelectedAnimationState.GetAnimationEvents().FindAll(x => x is ChangeAnimationEvent);
		}

		private void AddAnimationAudioEvent_BTN_Click(object sender, RoutedEventArgs e)
		{
			//if (CurrentlySelectedAnimationState == null || CurrentAnimationStateMachine == null) return;

			//AnimationAudioEvents_Properties_Tree.ItemsSource = null;
			////CurrentAudioEvent_List.Add(new SoundEffectAnimationEvent(0, 0, "None"));
			//CurrentlySelectedAnimationState.AddAnimationEvents(new AudioEvent(0, 0, "None"));
			//AnimationAudioEvents_Properties_Tree.ItemsSource = CurrentlySelectedAnimationState.GetAnimationEvents()
			//	.FindAll(x => x is AudioEvent);
		}

		private void AnimationAudioEvent_TB_KeyUp(object sender, KeyEventArgs e)
		{
			////Lamooo this is so dumb...
			//String stotal = (sender as TextBox).Text + e.Key.ToString().Replace("D", "");
			//if (int.TryParse(stotal, out int val))
			//{
			//	if (val > CurrentlySelectedAnimationState.FrameCount)
			//	{
			//		EngineOutputLog.AddErrorLogItem(-5, "Frame Entered for event Exceeds Max Frame Count!", "Animation Editor",
			//			true);
			//		EngineOutputLog.AddLogItem("Animation Machine error. See Error log for details");
			//		if (resizeGrid.RowDefinitions.Last().Height.Value < 100)
			//			resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
			//		OutputLogSpliter.IsEnabled = true;

			//		(sender as TextBox).Text = "0";

			//	}

			//	if (val < 0)
			//	{
			//		EngineOutputLog.AddErrorLogItem(-5, "Frame Entered cannot be negative!!", "Animation Editor", true);
			//		EngineOutputLog.AddLogItem("Animation Machine error. See Error log for details");
			//		if (resizeGrid.RowDefinitions.Last().Height.Value < 100)
			//			resizeGrid.RowDefinitions.Last().Height = new GridLength(100);
			//		OutputLogSpliter.IsEnabled = true;

			//		(sender as TextBox).Text = "0";

			//	}

			//}

		}

		private void AnimationChangeEventItem_Loaded(object sender, RoutedEventArgs e)
		{
			//if (CurrentAnimationStateMachine == null || CurrentlySelectedAnimationState == null) return;
			////Console.WriteLine("updated TREEVIEW");
			//if (CurrentAnimationStateMachine.SpriteAnimations.Count > 0)
			//{
			//	Grid sp = sender as Grid;

			//	if (sp != null)
			//	{
			//		SpriteAnimation item = null;
			//		if (!CurrentAnimationStateMachine.SpriteAnimations.TryGetValue(
			//			(sp.DataContext as ChangeAnimationEvent).FromAnimationName, out item))
			//			return;
			//		//var index = (ContentLibrary_Control.Template.FindName("AnimationEditor_CE_TV", ContentLibrary_Control) as TreeView).Items.IndexOf(item);

			//		BitmapImage bmp = CurrentSpriteSheet_Image;

			//		int foundToAnimIndex = -1;
			//		int count = 0;
			//		foreach (String key in CurrentAnimationStateMachine.SpriteAnimations.Keys)
			//		{
			//			(sp.Children[4] as ComboBox).Items.Add(key);
			//			if (key == (sp.DataContext as ChangeAnimationEvent).ToAnimationName)
			//				foundToAnimIndex = count;
			//			count++;
			//		}

			//		if (foundToAnimIndex >= 0)
			//		{
			//			(sp.Children[4] as ComboBox).SelectedIndex = foundToAnimIndex;

			//			item = CurrentAnimationStateMachine.SpriteAnimations[(sp.DataContext as ChangeAnimationEvent).ToAnimationName];
			//		}
			//		else
			//		{
			//			item = CurrentAnimationStateMachine.SpriteAnimations[
			//				(sp.DataContext as ChangeAnimationEvent).FromAnimationName];
			//		}

			//		var crop = new CroppedBitmap(bmp, new Int32Rect(
			//			(int)item.CurrentFrameRect.Value.XPos,
			//			(int)item.CurrentFrameRect.Value.YPos,
			//			(int)item.CurrentFrameRect.Value.Width,
			//			(int)item.CurrentFrameRect.Value.Height
			//		));
			//		(sp.Children[0] as Image).Source = crop;


			//	}
			//}

		}

		private void AnimationPreviewThumbnail_SubLayer_EVENT_Image_Enter(object sender, MouseEventArgs e)
		{
			//if (currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.SelectedItem.ToString()].ToList()[
			//	CurrentSubLayerSpriteSheets_LB.SelectedIndex].ImgPathLocation == null) return;
			//if (currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.SelectedItem.ToString()].ToList()
			//	[CurrentSubLayerSpriteSheets_LB.SelectedIndex].SpriteAnimations.TryGetValue(
			//		((sender as Grid).DataContext as ChangeAnimationEvent).ToAnimationName,
			//		out SpriteAnimation animval))
			//{

			//	String imgpath =
			//		currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.SelectedItem.ToString()].ToList()[
			//			CurrentSubLayerSpriteSheets_LB.SelectedIndex].ImgPathLocation;

			//	//CurrentAnimPreviewImages_CE 
			//	CurrentAnimPreviewImages_CE.Clear();

			//	//Now that we know which Animation we are in let's Preload the Frames to avoid uneeded GC every couple frames
			//	for (int i = 0; i < animval.FrameCount; i++)
			//	{
			//		//this eyesore will catch bad sprite sheets.So it doesn't go out of bounds....
			//		//int width2 =
			//		//	(int)(animval.StartXPos +
			//		//		((animval.FrameCount) * animval.FrameWidth) > CurrentSpriteSheet_Image.Width
			//		//			? CurrentSpriteSheet_Image.Width - ((animval.StartXPos + ((animval.FrameCount - 1) * animval.FrameWidth))) : animval.FrameWidth);

			//		BitmapImage bmi = new BitmapImage();
			//		bmi.BeginInit();
			//		bmi.CacheOption = BitmapCacheOption.OnLoad;
			//		bmi.UriSource = new Uri(imgpath, UriKind.Absolute);
			//		bmi.EndInit();

			//		CurrentAnimPreviewImages_CE.Add(
			//			(new CroppedBitmap(bmi, new Int32Rect(
			//				(int)animval.CurrentFrameRect.Value.XPos,
			//				(int)animval.CurrentFrameRect.Value.YPos,
			//				(int)animval.CurrentFrameRect.Value.Width,
			//				(int)animval.CurrentFrameRect.Value.Height
			//			))));
			//	}

			//	PreviewAnimUI_Image_PTR = (sender as Grid).Children[0] as Image;
			//	PreviewAnimUI_Image_PTR.Tag = 1;
			//	Animation_CE_Preview_Stopwatch.Start();
			//	PreviewAnim_Data_PTR = (SpriteAnimation)animval;
			//}
		}


		private void AnimationPreviewThumbnail_EVENT_Image_Enter(object sender, MouseEventArgs e)
		{
			//if (CurrentSpriteSheet_Image.UriSource == null) return;
			//if (CurrentAnimationStateMachine.SpriteAnimations.TryGetValue(
			//	((sender as Grid).DataContext as ChangeAnimationEvent).ToAnimationName,
			//	out SpriteAnimation animval))
			//{
			//	//CurrentAnimPreviewImages_CE 
			//	CurrentAnimPreviewImages_CE.Clear();

			//	//Now that we know which Animation we are in let's Preload the Frames to avoid uneeded GC every couple frames
			//	for (int i = 0; i < animval.FrameCount; i++)
			//	{
			//		//this eyesore will catch bad sprite sheets.So it doesn't go out of bounds....
			//		//int width2 =
			//		//	(int)(animval.StartXPos +
			//		//		((animval.FrameCount) * animval.FrameWidth) > CurrentSpriteSheet_Image.Width
			//		//			? CurrentSpriteSheet_Image.Width - ((animval.StartXPos + ((animval.FrameCount - 1) * animval.FrameWidth))) : animval.FrameWidth);


			//		CurrentAnimPreviewImages_CE.Add(
			//			(new CroppedBitmap(CurrentSpriteSheet_Image, new Int32Rect(
			//				(int)animval.CurrentFrameRect.Value.XPos,
			//				(int)animval.CurrentFrameRect.Value.YPos,
			//				(int)animval.CurrentFrameRect.Value.Width,
			//				(int)animval.CurrentFrameRect.Value.Height
			//			))));
			//	}

			//	PreviewAnimUI_Image_PTR = (sender as Grid).Children[0] as Image;
			//	PreviewAnimUI_Image_PTR.Tag = 1;
			//	Animation_CE_Preview_Stopwatch.Start();
			//	PreviewAnim_Data_PTR = (SpriteAnimation)animval;
			//}
		}


		private void AnimTestLayer_BTN_Click(object sender, RoutedEventArgs e)
		{
			//BitmapImage bmi = new BitmapImage();
			//bmi.BeginInit();
			//bmi.CacheOption = BitmapCacheOption.OnLoad;
			//bmi.UriSource = new Uri("C:\\Users\\Antonio\\Documents\\ProjectE_E\\ProjectEE\\AmethystEngine"
			//												+ "/images/Ame_icon_small.png", UriKind.Absolute);
			//bmi.EndInit();



			//AnimationLayersPreview_Canvas_IC.Children.Add(new Image() { Source = bmi });
			//Grid.SetZIndex(AnimationLayersPreview_Canvas_IC.Children[1] as Image, 1);
		}

		private void AnimTestLayerSwitchZindex_BTN_Click(object sender, RoutedEventArgs e)
		{
			//Image img = AnimationLayersPreview_Canvas_IC.Children[1] as Image;
			//Grid.SetZIndex(AnimationLayersPreview_Canvas_IC.Children[0] as Image, 1);


			//Grid.SetZIndex(img, 0);

		}

		private void ChangeAnimationSubLayer_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//ComboBox cb = sender as ComboBox;
			//if (cb.SelectedIndex == 0)
			//{
			//	//We need to change grids, so the user can add a sprite layer
			//	AnimationAddSubLayer_Grid.Visibility = Visibility.Visible;
			//	AnimationLayersSettings_Grid.Visibility = Visibility.Hidden;
			//}
			//else if (cb.SelectedIndex > 1 && cb.Items[cb.SelectedIndex] as ComboBoxItem != null)
			//{
			//	//this is an actual sub layer, So we need to load all the possible Spritesheets to the screen
			//	CurrentSubLayerSpriteSheets_LB.ItemsSource =
			//		currentLayeredSpriteSheet.subLayerSpritesheets_Dict
			//			[(cb.Items[cb.SelectedIndex] as ComboBoxItem).Content.ToString()].ToList();


			//}
		}

		private void AddAnimationSubLayer_BTN_Click(object sender, RoutedEventArgs e)
		{
			//if (AddSubLayerName_TB.Text != "")
			//{
			//	if (!currentLayeredSpriteSheet.AddSubLayer(AddSubLayerName_TB.Text)) return; //failed


			//	AnimationSubLayer_CB.Items.Add(new ComboBoxItem() { Content = AddSubLayerName_TB.Text });
			//	AnimationSubLayer_CB.SelectedIndex = AnimationSubLayer_CB.Items.Count - 1;

			//	Image img = new Image() { Tag = "1" };
			//	Grid.SetZIndex(img, currentLayeredSpriteSheet.subLayerSpritesheets_Dict.Count);
			//	AnimationLayersPreview_Canvas_IC.Children.Add(img);

			//	AnimationSubLayerImages_List.Add(new List<CroppedBitmap>());

			//	AddSubLayerName_TB.Text = "";
			//	AnimationAddSubLayer_Grid.Visibility = Visibility.Hidden;
			//	AnimationLayersSettings_Grid.Visibility = Visibility.Visible;
			//}
		}

		/// <summary>
		/// give this a valid CanvasSpriteSheet and it will return a BIXBITE spritesheet
		/// </summary>
		/// <param name="wpfCanvasSpritesheet"></param>
		/// <returns></returns>
		private AnimationStateMachine canvaSpriteSheetToAnimationStateMachine(CanvasSpritesheet wpfCanvasSpritesheet)
		{
			//// NEW CODE for NEW SPRITESHEET
			CanvasSpritesheet tempCanvasSpritesheet = wpfCanvasSpritesheet;

			//// We have created a new SpriteSheet
			AnimationStateMachine returnAnimationStateMachine = new AnimationStateMachine();
			////AnimationStateMachine returnSpriteSheet = new AnimationStateMachine(NewAnimimationStatemachineFileName, NewAnimimationStatemachineLocation,
			////	0, 0, NewAnimimationStatemachineTotalWidth, NewAnimimationStatemachineTotalHeight);

			//// Let's take the canvas sprite sheet andEditorToolBar_CC use that data to fill in the Game spritesheet
			//foreach (CanvasAnimation canvasAnimation in tempCanvasSpritesheet.AllAnimationOnSheet)
			//{
			//	returnAnimationStateMachine.States.Add(canvasAnimation.AnimName,
			//		new AnimationState(returnAnimationStateMachine)
			//		{
			//			StateName = canvasAnimation.AnimName,
			//			NumOfFrames = canvasAnimation.CanvasFrames.Count
			//		}
			//	);

			//	// We need to add all the sub layers names!
			//	foreach (var namesOfSubLayer in canvasAnimation.NamesOfSubLayers)
			//	{
			//		returnAnimationStateMachine.SpriteAnimations.Last().Value.NamesOfSubLayers.Add(namesOfSubLayer);
			//	}

			//	// Add the frame positions
			//	foreach (CanvasImageProperties canvasImageProperties in canvasAnimation.CanvasFrames)
			//	{
			//		returnAnimationStateMachine.SpriteAnimations.Last().Value.FrameDrawRects.AddLast(
			//			new LinkedListNode<FrameInfo>(new FrameInfo(canvasImageProperties.X, canvasImageProperties.Y,
			//				canvasImageProperties.W, canvasImageProperties.H, canvasImageProperties.RX, canvasImageProperties.RY)));

			//		foreach (var subLayerPoint in canvasImageProperties.SubLayerPoints)
			//		{
			//			returnAnimationStateMachine.SpriteAnimations.Last().Value.FrameDrawRects.Last().AddSubLayerPoint(
			//				subLayerPoint.LayerName, subLayerPoint.RX, subLayerPoint.RY);
			//		}
			//	}
			//}
			return returnAnimationStateMachine;
		}

		private void AddSpriteSheetToSubLayer_BTN_Click(object sender, RoutedEventArgs e)
		{
			//	CurrentSubLayerSpriteSheets_LB.ItemsSource = null;

			//	// Get a new image fileCurrentSubLayerSpriteSheets_LB_OnSelectionChanged
			//	Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			//	{
			//		Title = "Import Sprite Sheet File",
			//		FileName = "", //default file name
			//		Filter = "Sprite Sheet files (*.spritesheet)|*.spritesheet",
			//		RestoreDirectory = true
			//	};

			//	Nullable<bool> result = dlg.ShowDialog();
			//	// Process save file dialog box results
			//	string filename = "";
			//	if (result == true)
			//	{
			//		// Save document
			//		filename = dlg.FileName;
			//		// filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] {'/', '\\'}));
			//	}
			//	else return; //invalid name

			//	Console.WriteLine(dlg.FileName);

			//	CanvasSpritesheet tempCanvasSpritesheet = CanvasSpritesheet.ImportSpriteSheet(dlg.FileName);
			//	SpriteSheet tempBixbiteSpriteSheet = canvaSpriteSheetToAnimationStateMachine(tempCanvasSpritesheet);

			//	// We need to play WPF games... aka find the template, and then with that find the acutal control (treeview)
			//	ControlTemplate spriteSheeControlTemplate = (ControlTemplate)this.Resources["AnimationEditorObjects_Template"];
			//	TreeView spritesheetTreeView = (TreeView)spriteSheeControlTemplate.FindName("AnimationEditor_CE_TV", ContentLibrary_Control);
			//	SpriteAnimation currentSelectedAnimation = spritesheetTreeView.SelectedItem as SpriteAnimation;

			//	//if (CurrentAnimationStateMachine.SpriteAnimations.ContainsKey(currentSelectedAnimation.Name))
			//	//{
			//	//	if (!CurrentAnimationStateMachine.SpriteAnimations[currentSelectedAnimation.Name].AddSubLayer(AnimationSubLayer_CB.Text, ))
			//	//	{

			//	//	}
			//	//}

			//	////AnimationEditor_CE_TV

			//	////Adding this to the layered Sprite sheet. THIS import must be changed later.
			//	////currentLayeredSpriteSheet = new LayeredSpriteSheet(retSheet);
			//	//currentLayeredSpriteSheet.AddSpriteSheetToSubLayer(
			//	//	currentLayeredSpriteSheet.subLayerSpritesheets_Dict.Keys.ToList()[AnimationSubLayer_CB.SelectedIndex - 2],
			//	//	retSheet);

			//	CurrentSubLayerSpriteSheets_LB.ItemsSource =
			//		currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.Text].ToList();
		}

		private void CurrentSubLayerSpriteSheets_LB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//	if (AnimationSubLayer_CB.SelectedIndex >= 2)
			//	{
			//		String name =
			//			currentLayeredSpriteSheet.subLayerSpritesheets_Dict.Keys.ToList()[AnimationSubLayer_CB.SelectedIndex - 2];

			//		SpriteSheet spriteSheet = currentLayeredSpriteSheet.ActiveSubLayerSheet[name];
			//		if (spriteSheet != null && (sender as ListBox).SelectedItem != null)
			//		{
			//			CurrentSubLayerAnimStates_LB.ItemsSource = spriteSheet.SpriteAnimations.Values;
			//			currentLayeredSpriteSheet.ChangeActiveSheetString(AnimationSubLayer_CB.Text,
			//				currentLayeredSpriteSheet.ActiveSubLayerSheet[AnimationSubLayer_CB.Text].SheetName,
			//				((sender as ListBox).SelectedItem as SpriteSheet).SheetName);
			//		}
			//	}
		}

		private void CurrentSubLayerAnimStates_LB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//if (AnimationSubLayer_CB.SelectedIndex >= 2)
			//{
			//	Image img = AnimationLayersPreview_Canvas_IC.Children[AnimationSubLayer_CB.SelectedIndex - 2] as Image;
			//	if (img != null)
			//	{

			//		String name =
			//			currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.Text][
			//				CurrentSubLayerSpriteSheets_LB.SelectedIndex].SheetName;

			//		SpriteSheet spriteSheet = currentLayeredSpriteSheet.ActiveSubLayerSheet[AnimationSubLayer_CB.Text];
			//		if (spriteSheet == null || (sender as ListBox).SelectedIndex == -1) return;
			//		spriteSheet.ChangeAnimation(((sender as ListBox).SelectedItem as SpriteAnimation).Name);

			//		SpriteAnimation item = spriteSheet.CurrentAnimation;

			//		BitmapImage bmp = new BitmapImage();

			//		bmp.BeginInit();
			//		bmp.CacheOption = BitmapCacheOption.OnLoad;
			//		bmp.UriSource = new Uri(spriteSheet.ImgPathLocation, UriKind.Absolute);
			//		bmp.EndInit();

			//		var crop = new CroppedBitmap(bmp, new Int32Rect(
			//			(int)item.CurrentFrameRect.Value.XPos,
			//			(int)item.CurrentFrameRect.Value.YPos,
			//			(int)item.CurrentFrameRect.Value.Width,
			//			(int)item.CurrentFrameRect.Value.Height
			//		));
			//		img.Source = crop;

			//		List<CroppedBitmap> templist = new List<CroppedBitmap>();
			//		for (int i = 0; i < item.FrameCount; i++)
			//		{
			//			//this eyesore will catch bad sprite sheets.So it doesn't go out of bounds....
			//			//int width2 =
			//			//	(int)(item.StartXPos +
			//			//		((item.FrameCount) * item.FrameWidth) > CurrentSpriteSheet_Image.Width
			//			//			? CurrentSpriteSheet_Image.Width - ((item.StartXPos + ((item.FrameCount - 1) * item.FrameWidth))) : item.FrameWidth);

			//			templist.Add(crop);
			//		}

			//		AnimationSubLayerImages_List[AnimationSubLayer_CB.SelectedIndex - 2] = templist;

			//		//Now we need to set the animation events to the screen

			//		if (currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.Text]
			//			.ToList()[CurrentSubLayerSpriteSheets_LB.SelectedIndex]
			//			.SpriteAnimations_List[CurrentSubLayerAnimStates_LB.SelectedIndex]
			//			.GetAnimationEvents()
			//			.Count > 0)

			//		{
			//			CurrentSubLayerStateChanges_TV.ItemsSource =
			//				currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.Text]
			//					.ToList()[CurrentSubLayerSpriteSheets_LB.SelectedIndex]
			//					.SpriteAnimations_List[CurrentSubLayerAnimStates_LB.SelectedIndex]
			//					.GetAnimationEvents().FindAll(x => x is ChangeAnimationEvent);
			//		}


			//	}
			//}
		}

		public void AddEventToSubLayerAnimation_BTN_Click(object sender, RoutedEventArgs e)
		{
			//if (currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.SelectedItem.ToString()].ToList()[
			//	CurrentSubLayerSpriteSheets_LB.SelectedIndex] == null) return;
			//if (currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.SelectedItem.ToString()].ToList()[
			//	CurrentSubLayerSpriteSheets_LB.SelectedIndex].CurrentAnimation == null) return;


			//SpriteAnimation spriteAnimation =
			//	currentLayeredSpriteSheet.subLayerSpritesheets_Dict[AnimationSubLayer_CB.SelectedItem.ToString()].ToList()[
			//		CurrentSubLayerSpriteSheets_LB.SelectedIndex].CurrentAnimation;

			////if (CurrentlySelectedAnimationState == null || CurrentAnimationStateMachine == null) return;

			//CurrentSubLayerStateChanges_TV.ItemsSource = null;
			//spriteAnimation.AddAnimationEvents(
			//	new ChangeAnimationEvent(spriteAnimation.Name, "NONE", true));
			//CurrentSubLayerStateChanges_TV.ItemsSource =
			//	spriteAnimation.GetAnimationEvents().FindAll(x => x is ChangeAnimationEvent);
		}


	}
}
