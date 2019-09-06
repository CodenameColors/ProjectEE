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
		String ProjectPath = "";
		Dictionary<String, String> ProjectLevels = new Dictionary<string, string>();
		Dictionary<String, GameEvent> ProjectGameEvents = new Dictionary<string, GameEvent>();

		public int AddNewX = 0;
		public int AddnewY = 0;

		public GameEventsSettings()
		{
			InitializeComponent();
		}

		public GameEventsSettings(ref Level CurrentLevel, String ProjectPath, int TCindex)
		{
			InitializeComponent();
			GameEvent_TC.SelectedIndex = TCindex;
			SetGameEventLayers(CurrentLevel);
			
			this.ProjectPath = ProjectPath;
			//load all the levels for this project
			foreach (String s in System.IO.Directory.GetFiles(ProjectPath.Replace(".gem", "_Game\\Content\\Levels")))
			{
				String LevelName = (s.Substring(s.LastIndexOfAny(new char[] { '/', '\\' }) + 1));
				ProjectLevels.Add(s, LevelName);
			}

			this.CurrentLevel = CurrentLevel;
			GameEventLayers_LB.SelectedIndex = 0;
			if (TCindex == 0)
			{
				if (GameEventLayers_LB.Items.Count == 0) return; //there are no game event layers...
				SetGameEvents(CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]]);
				GameEvents_LB.SelectedIndex = 0;
				if (GameEvents_LB.Items.Count == 0) return; // there is no game events... 


				FillProjectsGEDict();//find all the methods that the user has created!

				//set the properties!
				SetGameEventProperties(((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item2[GameEvents_LB.SelectedIndex]);
			}
			else
			{

			}
		}


		private void FillProjectsGEDict()
		{
			foreach (String lev in ProjectLevels.Keys)
			{
				Level Tlev = Level.ImportLevel(lev); //create level
				foreach (SpriteLayer sl in Tlev.Layers)
				{
					if(sl.layerType == LayerType.GameEvent)
					{
						foreach (GameEvent GE in ((Tuple<int[,], List<GameEvent>>)sl.LayerObjects).Item2)
						{
							if (!ProjectGameEvents.ContainsKey(GE.GetPropertyData("DelegateEventName").ToString()))
								ProjectGameEvents.Add(GE.GetPropertyData("DelegateEventName").ToString(), GE);
						}
					}
				}
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

		//load the Form the selected Gameevent data.
		private void SetGameEventProperties(GameEvent curGameEvent)
		{
			EventName_TB.Text = curGameEvent.GetPropertyData("EventName").ToString();
			int num = (int)curGameEvent.eventType; EventType_CB.SelectedItem = EventType_CB.Items[num]; 
			EventGroup_TB.Text = curGameEvent.GetPropertyData("group").ToString();
			EventDelegateName_TB.Text = curGameEvent.GetPropertyData("DelegateEventName").ToString();
			//TODO: Add the inputs after the input section in the project settings is added.

			if (curGameEvent.eventType == EventType.Cutscene || curGameEvent.eventType == EventType.DialougeScene
				|| curGameEvent.eventType == EventType.LevelTransition || curGameEvent.eventType == EventType.BGM)
				EventData_CC.Visibility = Visibility.Visible;
			else
			{
				EventData_CC.Visibility = Visibility.Hidden;
				return;
			}

			//load the FileToLoad Combobox based on the event type.
			FileToLoad_CB.Items.Clear(); FileToLoad_CB.Items.Add("None");
			if(EventType_CB.SelectedIndex == 1)
			{
				foreach (String key in ProjectLevels.Keys)
					FileToLoad_CB.Items.Add(ProjectLevels[key]);
			}
			//TODO: Add other types of gameevents triggering.

			//is this a valid level for this project?
			if (ProjectLevels.ContainsKey(curGameEvent.datatoload.NewFileToLoad))
			{
				int num1 = ProjectLevels.Keys.ToList().IndexOf(curGameEvent.datatoload.NewFileToLoad);
				FileToLoad_CB.SelectedIndex = num1;
			}
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
			GameEventLog_TB.Text = "";
			bool ret = true;
			bool bLName = true;
			String s = "";
			ret &= GameEventLayers_LB.Items.Count > 0;
			ret &= (AddEventName_TB.Text == "" ? false : true);
			ret &= (AddEventGroup_TB.Text == "" ? false : true);
			ret &= (AddEventDelegateName_TB.Text == "" ? false : true); ;
			ret &= (AddEventGroup_TB.Text == "" ? false : true); ;
			
			if (ProjectGameEvents.ContainsKey(AddEventDelegateName_TB.Text))
			{
				GameEventLog_TB.Text = "Delegate Name is already used in this project!"; return false;
			}

			//okay so all of those should be filled out to have a valid event!
			bLName &= System.Text.RegularExpressions.Regex.IsMatch(AddEventName_TB.Text, @"^[A-Za-z][A-Za-z0-9]+");
			bLName &= System.Text.RegularExpressions.Regex.IsMatch(AddEventDelegateName_TB.Text, @"^[A-Za-z][A-Za-z0-9]+");

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
					GameEventLog_TB.Text = ("Invaild Data given to create game event");
					return false; //not valid
				}
			}
			else
			{
				int val;
				bLName &= Int32.TryParse(AddEventGroup_TB.Text, out int group);
				bLName &= Int32.TryParse(AddEventNewPosX_TB.Text, out int newx);
				bLName &= Int32.TryParse(AddEventNewPosY_TB.Text, out int newy);
				bLName &= Int32.TryParse(AddEventMoveTime_TB.Text, out int movetime);
				AddEventData_CC.Visibility = Visibility.Visible;
				ret &= (AddFileToLoad_CB.SelectedIndex  > 0 ? true : false);
				//are the valid numbers  > 0
				ret &= (newx >= 0 ? true : false);
				ret &= (newy >= 0 ? true : false);
				ret &= (0 >= 0 ? true : false);

				if (ret && bLName)
				{
					return true;
				}
				else
				{
					GameEventLog_TB.Text = ("Invaild Data given to create game event");
					return false; //not valid
				}

			}
		}

		private void CreateNewGameEvent_Click(object sender, RoutedEventArgs e)
		{
			if (CanCreateEvent())
			{
				bool bLName = true;
				bLName &= Int32.TryParse(AddEventGroup_TB.Text, out int group);
				bLName &= group > 0; //-1 = collision, 0 is nothing. Only allow 1 -> n
				bLName &= Int32.TryParse(AddEventNewPosX_TB.Text, out int newx);
				bLName &= Int32.TryParse(AddEventNewPosY_TB.Text, out int newy);
				bLName &= Int32.TryParse(AddEventMoveTime_TB.Text, out int movetime);

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
					if (AddEventType_CB.SelectedIndex == 1 && AddFileToLoad_CB.SelectedIndex > 0)
						ge.datatoload.NewFileToLoad = ProjectLevels.Keys.ToList()[AddFileToLoad_CB.SelectedIndex - 1];
				}

				//add the event!
				((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item2.Add(ge);

			}
		}

		private void AddEventType_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AddEventData_CC == null) return;
			if (AddEventType_CB.SelectedIndex == 4 || AddEventType_CB.SelectedIndex == 5)
			{
				AddEventData_CC.Visibility = Visibility.Hidden;
				if(AddEventType_CB.SelectedIndex == 5)
				{
					AddEventGroup_TB.IsEnabled = false;
					AddEventGroup_TB.Text= "-1";
				}
				else AddEventGroup_TB.IsEnabled = true;
			}
			else
			{
				AddEventData_CC.Visibility = Visibility.Visible;
				AddEventGroup_TB.IsEnabled = true;
				
				//load the FileToLoad Combobox based on the event type.
				AddFileToLoad_CB.Items.Clear(); AddFileToLoad_CB.Items.Add("None");
				if (AddEventType_CB.SelectedIndex == 1)
				{
					foreach (String key in ProjectLevels.Keys)
						AddFileToLoad_CB.Items.Add(ProjectLevels[key]);
				}
				//TODO: Add other types of gameevents triggering.


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
				if (Int32.TryParse(((TextBox)sender).Text, out int num))
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
				if (Int32.TryParse(((TextBox)sender).Text, out int num))
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
				if (Int32.TryParse(((TextBox)sender).Text, out int num))
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
				if (Int32.TryParse(((TextBox)sender).Text, out int num))
				{
					//change the property on this layer and event
					GameEvent g = ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].
						LayerObjects).Item2[GameEvents_LB.SelectedIndex];
					g.datatoload.MoveTime = num;
				}
			}
		}

		private void RemoveGE_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (GameEvents_LB.SelectedIndex >= 0)
			{
				//removes the gameevent itself.
				GameEvent gameE = ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item2[GameEvents_LB.SelectedIndex];
				int TileGoupNum = (int)gameE.GetPropertyData("group");
				((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item2.RemoveAt(GameEvents_LB.SelectedIndex);

				

				//remove EVERY int value in the game event tile array of this event.
				for (int i = 0; i < ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item1.GetLength(0); i++)
				{
					for (int j = 0; j < ((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item1.GetLength(1); j++)
					{
						if (((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item1[i, j] == TileGoupNum)
							((Tuple<int[,], List<GameEvent>>)CurrentLevel.Layers[Layernum[GameEventLayers_LB.SelectedIndex]].LayerObjects).Item1[i, j] = 0;
					}
				}
			}
		}

		private void MapView_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (AddFileToLoad_CB.SelectedIndex > 0 || FileToLoad_CB.SelectedIndex > 0)
			{
				Window f;
				if (GameEvent_TC.SelectedIndex == 0)
				{
					f = new NewMapChange(ProjectLevels.Keys.ToList()[FileToLoad_CB.SelectedIndex - 1], this, false);
					f.Show();
				}
				else
				{
					f = new NewMapChange(ProjectLevels.Keys.ToList()[AddFileToLoad_CB.SelectedIndex - 1], this, true);
					f.Show();
				}
			}
		}



	}
}
