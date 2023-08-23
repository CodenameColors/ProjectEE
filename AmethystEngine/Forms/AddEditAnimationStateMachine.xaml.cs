using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BixBite.Rendering;
using BixBite.Rendering.Animation;
using Microsoft.Xna.Framework;

namespace AmethystEngine.Forms
{
	/// <summary>
	/// Interaction logic for AddEditAnimationStateMachine.xaml
	/// </summary>
	public partial class AddEditAnimationStateMachine : Window, INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		#region Delegate

		public delegate void HooKfunction(AnimationState retSpriteAnimation, String name, bool bIsAdding, bool bDefaultChange, String oldkey = "");
		public HooKfunction AddToStatemachine;


		#endregion

		#region fields

		private String _name = "";
		private String _oldkey = "";
		private int _startX = -1;
		private int _startY = -1;
		private int _framecount = -1;
		private int _framewidth = -1;
		private int _frameheight = -1;
		private int _fps = -1;
		private int _animationImporterTime_MS = -1;
		private bool _bIsAdding = false;
		private bool _bAllowImportAnimPreview = false;
		private bool _bHasChangedDefault = false;

		private BitmapImage _mainImage = new BitmapImage();
		private AnimationState  mainAnimationState = null;
		private Stopwatch AnimationImporterPreview_Stopwatch = new Stopwatch();
		private Thread animationImportPreviewThread = null;
		private List<CroppedBitmap> _animationImages_List = new List<CroppedBitmap>();

		#endregion

		#region Properties


		public String Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		public int StartX
		{
			get => _startX;
			set
			{
				_startX = value;
				OnPropertyChanged("StartX");
			}
		}

		public int StartY
		{
			get => _startY;
			set
			{
				_startY = value;
				OnPropertyChanged("StartY");
			}
		}

		public int FrameCount
		{
			get => _framecount;
			set
			{
				_framecount = value;
				OnPropertyChanged("FrameCount");
			}
		}

		public int FrameWidth
		{
			get => _framewidth;
			set
			{
				_framewidth = value;
				OnPropertyChanged("FrameWidth");
			}
		}

		public int FrameHeight
		{
			get => _frameheight;
			set
			{
				_frameheight = value;
				OnPropertyChanged("FrameHeight");
			}
		}

		public int FPS
		{
			get => _fps;
			set
			{
				_fps = value;
				OnPropertyChanged("FPS");
			}
		}


		public int AnimationImporterTime_MS
		{
			get => _animationImporterTime_MS;
			set
			{
				_animationImporterTime_MS = value;
				OnPropertyChanged("AnimationImporterTime_MS");
			}
		}

		public bool bIsDefaultState
		{
			get;
			set;
		}


		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="MainImage"></param>
		/// <param name="currSpriteAnimation">CAN BE NULL</param>
		public AddEditAnimationStateMachine(BitmapImage MainImage, AnimationStateMachine parentStateMachine, AnimationState currSpriteAnimation)
		{
			_mainImage = MainImage;
			if (currSpriteAnimation == null)
			{
				mainAnimationState = new AnimationState(parentStateMachine) {StateName = "TempName" };

				
				_bIsAdding = true;
			}
			else
			{
				mainAnimationState = currSpriteAnimation;
				_oldkey = currSpriteAnimation.StateName;

				//StateName_TB.Text = mainAnimationState.Name;
				this.Name = mainAnimationState.StateName;
				this.FrameCount = mainAnimationState.NumOfFrames;
				if (mainAnimationState.AnimationLayers.Count > 0)
				{
					this.FPS = mainAnimationState.FPS;
					this.bIsDefaultState = mainAnimationState.bIsDefaultState;
				}
				_bIsAdding = false;
			}

			InitializeComponent();

			this.DataContext = this;
			CurrentActiveAnimationName_TB.Text = parentStateMachine.CurrentState.StateName;
			animationImportPreviewThread = null;
		}

		private void AddEditAnimationStateMachine_OnLoaded(object sender, RoutedEventArgs e)
		{

			if (_bIsAdding)
			{
				EditingIndicator_TB.Visibility = Visibility.Hidden;
				AddingIndicator_TB.Visibility = Visibility.Visible;
			}
			else
			{

				StateName_TB.Text = mainAnimationState.StateName;
				EditingIndicator_TB.Visibility = Visibility.Visible;
				AddingIndicator_TB.Visibility = Visibility.Hidden;

				FinishAndClose_BTN.Content = "Finish Editing";

				RefreshImages();
			}

		}


		private void LBind_close(object sender, RoutedEventArgs e)
		{
			while (animationImportPreviewThread != null && animationImportPreviewThread.IsAlive)
			{
				animationImportPreviewThread.Abort();
				Thread.Sleep(10);
			}

			animationImportPreviewThread = null;

			this.Close();
		}

		private void LBind_DragMove(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}


		private void RefreshFrameRange_TB_KeydDown(object sender, KeyEventArgs e)
		{
			RefreshImages(sender);
		}

		private void RefreshImages(object sender = null)
		{
			int index = -1;
			BitmapImage bmp = null;
			try
			{
				//Make sure the data is correct before doing crops
				if (StartX >= 0 && StartY >= 0 &&
				    FrameWidth > 0 && FrameHeight > 0 &&
				    FrameCount > 0)
				{
					var v = FirstFrame_IMG;

					bmp = _mainImage;
					var crop = new CroppedBitmap(bmp, new Int32Rect(StartX, StartY, FrameWidth, FrameHeight));
					// using BitmapImage version to prove its created successfully
					(v as Image).Source = crop;

					var v2 = LastFrame_IMG;

					int width2 =
						(int)(StartX + ((FrameCount) * FrameWidth) > bmp.Width
							? bmp.Width - ((StartX + ((FrameCount - 1) * FrameWidth))) : FrameWidth);

					var crop2 = new CroppedBitmap(bmp, new Int32Rect(StartX + ((FrameCount - 1) * FrameWidth), StartY, width2, FrameHeight));
					(v2 as Image).Source = crop2;


					AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Valid Settings displaying previews!";
					_bAllowImportAnimPreview = true;

				}
			}
			catch (ArgumentException ae)
			{


				if (bmp.Width < FrameWidth * FrameCount)
					AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Desired Width of animation EXCEEDS the max width of given sprite sheet PNG";
				else if (bmp.Height < FrameHeight)
					AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Desired Height of animation EXCEEDS the max width of given sprite sheet PNG";
				else
					AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Invalid Parameters resetting Text box input";
				if(sender != null)
					(sender as TextBox).Text = "0";
			}


		}

		private bool _bAllowFinish = false;
		private void PreviewAnim_CB_OnChecked(object sender, RoutedEventArgs e)
		{
			if (animationImportPreviewThread == null)
			{
				animationImportPreviewThread = new Thread(() =>
				{
					while (true)
					{
						try
						{
							Thread.Sleep(15);
						}
						catch
						{
							Console.WriteLine("animationImportPreviewThread == interrupted");
							break;
						}

						try
						{
							if (_bAllowImportAnimPreview)
							{
								//if (AnimationImporterPreview_Stopwatch.ElapsedMilliseconds > 1000)
								//	AnimationImporterPreview_Stopwatch.Restart();	//we need to reset the timer.

								//Force update to MainGUI
								Dispatcher.Invoke(() =>
									AnimationImporterTime_MS = (int) AnimationImporterPreview_Stopwatch.ElapsedMilliseconds);

								Dispatcher.Invoke(() =>
								{

									var v = CurrentFrame_TB;
									var v1 = CurrentDeltaTime_TB;

									int DT = (int.Parse((v1 as TextBox).Text));

									var cb = PreviewAnim_CB;
									if ((cb as CheckBox).IsChecked == true)
									{

										if (StartX >= 0 && StartY >= 0 && FrameWidth > 0 && FrameHeight > 0 && FrameCount > 0 && FPS > 0)
										{
											int framenum = (int.Parse((v as TextBox).Text));

											if (((int) (1.0f / FPS * 1000.0f)) <
											    DT)
											{
												framenum++;
												DT = 0; //we need to reset the timer.
											}

											if (framenum > FrameCount)
											{
												framenum = 1;
											}

											(v as TextBox).Text = framenum.ToString();
											(v1 as TextBox).Text = (DT + 15).ToString();

											//Increment the Frame
											PreviewAnim_IMG.Source = _animationImages_List[framenum-1];


										}
									}
								});

								//ContentPresenter c =
								//	((ContentPresenter) AE_NewAnimStates_IC.ItemContainerGenerator.ContainerFromIndex(i));
								//var v = c.ContentTemplate.FindName("PreviewAnim_IMG", c);



								if (AnimationImporterPreview_Stopwatch.ElapsedMilliseconds > 1000)
									AnimationImporterPreview_Stopwatch.Restart(); //we need to reset the timer.
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Thread mismatch");
						}
					}
				});
				if(!animationImportPreviewThread.IsAlive)
					animationImportPreviewThread.Start();
			}

			_bAllowImportAnimPreview = false;
			Thread.Sleep(10); // Give the background thread to "pause"

			_animationImages_List.Clear();
			for (int i = 0; i < FrameCount; i++)
			{
				//this eyesore will catch bad sprite sheets.So it doesn't go out of bounds....
				int width2 = (int)(StartX + ((FrameCount) * FrameWidth) > _mainImage.Width
							? _mainImage.Width - ((StartX + ((FrameCount - 1) * FrameWidth))) : FrameWidth);


				_animationImages_List.Add(
					(new CroppedBitmap(_mainImage, new Int32Rect(StartX + (i * FrameWidth),
						StartY, width2, FrameHeight))));

				_bAllowImportAnimPreview = true; //un-pause the thread
			}

			_bAllowFinish = true;
		}

		private void AddToStateMachine_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (!_bAllowFinish)
			{
				AE_ImportStatusLog_TB.Text = DateTime.Now.ToLongTimeString() + ":   Preview the animation first!!";
			}


			mainAnimationState.StateName = StateName_TB.Text;
			mainAnimationState.NumOfFrames = FrameCount;
			mainAnimationState.FPS = FPS;
			mainAnimationState.bIsDefaultState = (bIsDefaultState);

			_bAllowImportAnimPreview = false;

			try
			{
				animationImportPreviewThread.Interrupt();
				if(!animationImportPreviewThread.Join(2000))
					animationImportPreviewThread.Abort();
			}
			catch
			{
				animationImportPreviewThread.Abort();
			}

			if(_bIsAdding)
				AddToStatemachine?.Invoke(mainAnimationState, StateName_TB.Text, bIsDefaultState, true);
			else 
				AddToStatemachine?.Invoke(mainAnimationState, StateName_TB.Text, false, bIsDefaultState, _oldkey);
			//while (animationImportPreviewThread.IsAlive)
			//{
			//	Thread.Sleep(10);
			//	animationImportPreviewThread.Abort();

			//}

			this.Close();

		}


		private void ChangedDefaultState_VB_Click(object sender, RoutedEventArgs e)
		{
			if ((sender as CheckBox).IsChecked == true)
				bIsDefaultState = true;
			else bIsDefaultState = false;
		}
	}
}
