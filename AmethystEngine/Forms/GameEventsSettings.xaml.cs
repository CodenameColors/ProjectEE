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
				if (GameEventLayers_LB.Items.Count == 0) return; //there are no game event layers...
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
			GameEvents_LB.Items.Clear();
			List<GameEvent> lge = ((Tuple<int[,], List<GameEvent>>)spriteLayer.LayerObjects).Item2;
			foreach (GameEvent ge in lge)
				GameEvents_LB.Items.Add(ge.EventName);
		}

		private void SetGameEventProperties(GameEvent curGameEvent)
		{
			EventName_TB.Text = curGameEvent.GetProperty("EventName").ToString();
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
			bool bLName = true;
			String s = "";
			ret &= (AddEventName_TB.Text == "" ? false : true);
			ret &= (AddEventGroup_TB.Text == "" ? false : true);
			ret &= (AddEventDelegateName_TB.Text == "" ? false : true); ;
			ret &= (AddEventGroup_TB.Text == "" ? false : true); ;


			//okay so all of those should be filled out to have a valid event!
			bLName = System.Text.RegularExpressions.Regex.IsMatch(AddEventName_TB.Text, @"^[A-Za-z][A-Za-z0-9]+");
			bLName &= System.Text.RegularExpressions.Regex.IsMatch(AddEventDelegateName_TB.Text, @"^[A-Za-z][A-Za-z0-9]+");
			int group, newx, newy, movetime = 0;


			//What Event Type is this?
			if (AddEventType_CB.SelectedIndex == 4 || AddEventType_CB.SelectedIndex == 5)
			{
				AddEventData_CC.Visibility = Visibility.Hidden;

				if (ret && bLName)
				{
					return true;
				}
				else
				{
					Console.WriteLine("Invaild Data given to create game event");
					return false; //not valid
				}
			}
			else
			{
				bLName &= Int32.TryParse(AddEventGroup_TB.Text, out group);
				bLName &= Int32.TryParse(AddEventNewPosX_TB.Text, out newx);
				bLName &= Int32.TryParse(AddEventNewPosY_TB.Text, out newy);
				bLName &= Int32.TryParse(AddEventMoveTime_TB.Text, out movetime);
				AddEventData_CC.Visibility = Visibility.Visible;
				ret &= (AddFileToLoad_TB_TB.Text == "" ? false : true);
				ret &= (AddEventNewPosX_TB.Text == "" ? false : true);
				ret &= (AddEventNewPosY_TB.Text == "" ? false : true);
				ret &= (AddEventMoveTime_TB.Text == "" ? false : true);

				if (ret && bLName)
				{
					return true;
				}
				else
				{
					Console.WriteLine("Invaild Data given to create game event");
					return false; //not valid
				}

			}
		}

		private void CreateNewGameEvent_Click(object sender, RoutedEventArgs e)
		{
			if (CanCreateEvent())
			{
				bool bLName = true;
				int group, newx, newy, movetime = 0;
				bLName &= Int32.TryParse(AddEventGroup_TB.Text, out group);
				bLName &= Int32.TryParse(AddEventNewPosX_TB.Text, out newx);
				bLName &= Int32.TryParse(AddEventNewPosY_TB.Text, out newy);
				bLName &= Int32.TryParse(AddEventMoveTime_TB.Text, out movetime);

				//create a new GameEvent
				GameEvent ge = new GameEvent(AddEventName_TB.Text, (EventType)AddEventType_CB.SelectedIndex, group);
				//properties!
				//ge.SetProperty("ActivationButton", )
				ge.SetProperty("DelegateEventName", AddEventDelegateName_TB.Text);
				ge.SetProperty("ActivationButton", "NONE");


				if (AddEventType_CB.SelectedIndex != 4 || AddEventType_CB.SelectedIndex != 5)
				{ 
					ge.datatoload.MoveTime = movetime;
					ge.datatoload.newx = newx;
					ge.datatoload.newy = newy;
					////ge.datatoload.NewFileToLoad= movetime;
				}

				//add the event!
				((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item2.Add(ge);

			}
		}

		private void AddEventType_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AddEventType_CB.SelectedIndex == 4 || AddEventType_CB.SelectedIndex == 5)
			{
				AddEventData_CC.Visibility = Visibility.Hidden;
				if(AddEventType_CB.SelectedIndex == 5)
				{
					AddEventGroup_TB.IsEnabled = false;
					AddEventGroup_TB.Text= "-1";
				}
			}
			else
			{
				AddEventGroup_TB.IsEnabled = true;
				AddEventData_CC.Visibility = Visibility.Visible;
			}
		}

		private void GameEvents_LB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (GameEvents_LB.SelectedIndex == -1) return;
				SetGameEventProperties(((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item2[GameEvents_LB.SelectedIndex]);
		}



		//Allowed Event Names since i need to auto gen this code later


		//EventName_TB.Text = curGameEvent.GetProperty("Name").ToString();
		//int num = (int)curGameEvent.eventType; EventType_CB.SelectedItem = EventType_CB.Items[num];
		//EventGroup_TB.Text = curGameEvent.GetProperty("group").ToString();
		//EventDelegateName_TB.Text = curGameEvent.GetProperty("DelegateEventName").ToString();
		////TODO: Add the inputs after the input section in the project settings is added.

		//FileToLoad_CB.Text = curGameEvent.datatoload.NewFileToLoad;
		//EventNewPosX_TB.Text = curGameEvent.datatoload.newx.ToString();
		//EventNewPosY_TB.Text = curGameEvent.datatoload.newy.ToString();
		//EventMoveTime_TB.Text = curGameEvent.datatoload.MoveTime.ToString();


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

		private void EventName_TB_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				if (System.Text.RegularExpressions.Regex.IsMatch(((TextBox)sender).Text, @"^[A-Za-z][A-Za-z0-9]+"))
				{
					//change the property on this layer and event
					GameEvent g = ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].
						LayerObjects).Item2[GameEvents_LB.SelectedIndex];
					g.EventName = ((TextBox)sender).Text;
					g.SetProperty("EventName", ((TextBox)sender).Text);

				}
			}
		}

		private void EventType_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AddEventType_CB.SelectedIndex == 4 || AddEventType_CB.SelectedIndex == 5)
			{
				AddEventData_CC.Visibility = Visibility.Hidden;
			}
			else
			{
				AddEventData_CC.Visibility = Visibility.Visible;
			}
			int num = 0;
				//change the property on this layer and event
				GameEvent g = ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].
					LayerObjects).Item2[GameEvents_LB.SelectedIndex];
				g.eventType = (EventType)EventType_CB.SelectedIndex;
		}

		private void EventGroup_TB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				int num = 0;
				if (Int32.TryParse(((TextBox)sender).Text, out num))
				{
					//change the property on this layer and event
					GameEvent g = ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].
						LayerObjects).Item2[GameEvents_LB.SelectedIndex];
					g.SetProperty("group", num);
				}
			}
		}

		private void EventDelegateName_TB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (System.Text.RegularExpressions.Regex.IsMatch(((TextBox)sender).Text, @"^[A-Za-z][A-Za-z0-9]+"))
				{
					//change the property on this layer and event
					GameEvent g = ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].
						LayerObjects).Item2[GameEvents_LB.SelectedIndex];
					g.EventName = ((TextBox)sender).Text;
					g.SetProperty("DelegateEventName", ((TextBox)sender).Text);
				}
			}
		}

		private void EventNewPosX_TB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				int num = 0;
				if (Int32.TryParse(((TextBox)sender).Text, out num))
				{
					//change the property on this layer and event
					GameEvent g = ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].
						LayerObjects).Item2[GameEvents_LB.SelectedIndex];
					g.datatoload.newx = num;
				}
			}
		}

		private void EventNewPosY_TB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				int num = 0;
				if (Int32.TryParse(((TextBox)sender).Text, out num))
				{
					//change the property on this layer and event
					GameEvent g = ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].
						LayerObjects).Item2[GameEvents_LB.SelectedIndex];
					g.datatoload.newy = num;
				}
			}
		}

		private void EventMoveTime_TB_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				int num = 0;
				if (Int32.TryParse(((TextBox)sender).Text, out num))
				{
					//change the property on this layer and event
					GameEvent g = ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].
						LayerObjects).Item2[GameEvents_LB.SelectedIndex];
					g.datatoload.MoveTime = num;
				}
			}
		}
	}
}
