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
using BixBite;
using BixBite.Rendering;

namespace AmethystEngine.Forms
{
	/// <summary>
	/// Interaction logic for GameEventsSettings.xaml
	/// </summary>
	public partial class GameEventsSettings : Window
	{
		List<int> Layernum = new List<int>();
		Level CurrentLevel;

		public GameEventsSettings()
		{
			InitializeComponent();
		}

		public GameEventsSettings(ref Level CurrentLevel, int TCindex)
		{
			InitializeComponent();
			GameEvent_TC.SelectedIndex = TCindex;
			SetGameEventLayers(CurrentLevel);
			
			this.CurrentLevel = CurrentLevel;
			GameEventLayers_LB.SelectedIndex = 0;
			if (TCindex == 0)
			{
				SetGameEvents(CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]]);
				GameEvents_LB.SelectedIndex = 0;
				if (GameEvents_LB.Items.Count == 0) return; // there is no game events... 

				//set the properties!
				SetGameEventProperties(((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item2[GameEvents_LB.SelectedIndex]);
			}
			else
			{

			}
		}


		private void SetGameEventLayers(Level CurrentLevel)
		{
			int i = 0;
			foreach (SpriteLayer spriteLayer in CurrentLevel.Layers)
			{
				//add ONLY gameevent layers to the list box
				if (spriteLayer.layerType == LayerType.GameEvent)
				{
					GameEventLayers_LB.Items.Add(spriteLayer.LayerName);
					Layernum.Add(i);
				}
				i++;
			}
		}

		private void SetGameEvents(SpriteLayer spriteLayer)
		{
			List<GameEvent> lge = ((Tuple<int[,], List<GameEvent>>)spriteLayer.LayerObjects).Item2;
			foreach (GameEvent ge in lge)
				GameEvents_LB.Items.Add(ge.EventName);
		}

		private void SetGameEventProperties(GameEvent curGameEvent)
		{
			EventName_TB.Text = curGameEvent.GetProperty("Name").ToString();
			int num = (int)curGameEvent.eventType; EventType_CB.SelectedItem = EventType_CB.Items[num]; 
			EventGroup_TB.Text = curGameEvent.GetProperty("group").ToString();
			EventDelegateName_TB.Text = curGameEvent.GetProperty("DelegateEventName").ToString();
			//TODO: Add the inputs after the input section in the project settings is added.

			if (curGameEvent.eventType == EventType.Cutscene || curGameEvent.eventType == EventType.DialougeScene
				|| curGameEvent.eventType == EventType.LevelTransistion || curGameEvent.eventType == EventType.BGM)
				EventData_CC.Visibility = Visibility.Visible;
			else
			{
				EventData_CC.Visibility = Visibility.Hidden;
				return;
			}
			FileToLoad_CB.Text = curGameEvent.datatoload.NewFileToLoad;
			EventNewPosX_TB.Text = curGameEvent.datatoload.newx.ToString();
			EventNewPosY_TB.Text = curGameEvent.datatoload.newy.ToString();
			EventMoveTime_TB.Text = curGameEvent.datatoload.MoveTime.ToString();


		}

		//allow uses to choose what file they want to load on activation.
		//It loads the different types of files the editor creates.
		//Also it changes the selection based on what event type is choosen
		private void SetFiles()
		{

		}

		//allows the user to choose an input to use to activate the gameevent
		//the options are the options that have been set in the project settings window.
		private void SetInputs()
		{

		}

		private bool CanCreateEvent()
		{
			bool ret = true;
			String s = EventName_TB.Text;
			//EventName_TB.Text = curGameEvent.GetProperty("Name").ToString();
			//int num = (int)curGameEvent.eventType; EventType_CB.SelectedItem = EventType_CB.Items[num];
			//EventGroup_TB.Text = curGameEvent.GetProperty("group").ToString();
			//EventDelegateName_TB.Text = curGameEvent.GetProperty("DelegateEventName").ToString();
			////TODO: Add the inputs after the input section in the project settings is added.

			//FileToLoad_CB.Text = curGameEvent.datatoload.NewFileToLoad;
			//EventNewPosX_TB.Text = curGameEvent.datatoload.newx.ToString();
			//EventNewPosY_TB.Text = curGameEvent.datatoload.newy.ToString();
			//EventMoveTime_TB.Text = curGameEvent.datatoload.MoveTime.ToString();

			return ret;
		}

		private void GameEventSettings_DragMove(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}
		
		private void GameEventSettings_close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
		private void GameEventSettings_FullScreen(object sender, RoutedEventArgs e)
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

		private void GameEventSettings_Minimize(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
			WindowStyle = WindowStyle.None;

		}

		private void GameEventLayers_LB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//load the new game events list.
			GameEvents_LB.Items.Clear();
			SetGameEvents(CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]]);
		}
	}
}
