using BixBite.Rendering.UI.Image;
using BixBite.Rendering.UI.ProgressBar;
using BixBite.Rendering.UI.TextBlock;
using BixBite.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.QuickTimeEvents
{
	public class GameQuickTimeEvent : BaseQuickTimeEvents, IProperties 
	{

		#region Delegates

		#endregion

		#region Properties


		#endregion

		#region Fields
		private SpriteFont spriteFont;

		private GameProgressBar QuickTimeEventTimerBar = null;
		private GameTextBlock QuickTimeEventTimer_TB = null;

		private GameTextBlock QuickTimeEventKeyMessage_TB = null;
		private GameTextBlock QuickTimeStatusMessage_TB = null;
		private GameImage QuickTimeEvent_Image = null;

		#endregion

		#region Methods

		#region Constructors
		public GameQuickTimeEvent(int totalRunTime, int requiredKey, SpriteFont font, 
			EQuickTimeEventEffect eventEffect, EQuickTimeEventDisplayType displayType)
		{
			this._totalRunTimeInMs = totalRunTime;
			this.spriteFont = font;
			this._requiredKey = requiredKey;
			this.QuickTimeEventEffect = eventEffect;
			this.DisplayQuickTimeEventType = displayType;
		}
		#endregion

		#region Helpers
		#region Properties Interface
		#region IPropertiesImplementation
		public override void SetNewProperties(ObservableCollection<Tuple<string, object>> NewProperties)
		{
			Properties = NewProperties;
		}

		public override void ClearProperties()
		{
			Properties.Clear();
		}

		public override void SetProperty(string Key, object Property)
		{
			if (Properties.Any(m => m.Item1 == Key))
				Properties[GetPropertyIndex(Key)] = new Tuple<string, object>(Key, Property);
			else throw new PropertyNotFoundException(Key);

		}

		public override void AddProperty(string Key, object data)
		{
			if (!Properties.Any(m => m.Item1 == Key))
				Properties.Add(new Tuple<String, object>(Key, data));
		}

		public override Tuple<String, object> GetProperty(String Key)
		{
			if (Properties.Any(m => m.Item1 == Key))
				return Properties.Single(m => m.Item1 == Key);
			else throw new PropertyNotFoundException();
		}

		public override object GetPropertyData(string Key)
		{
			int i = GetPropertyIndex(Key);
			if (-1 == i) throw new PropertyNotFoundException(Key);
			return Properties[i].Item2;
		}

		public override ObservableCollection<Tuple<String, object>> GetProperties()
		{
			return Properties;
		}

		#endregion

		#region Helper
		public override int GetPropertyIndex(String Key)
		{
			int i = 0;
			foreach (Tuple<String, object> tuple in Properties)
			{
				if (tuple.Item1 == Key)
					return i;
				i++;
			}
			return -1;
		}

		#endregion
		#endregion

		/// <summary>
		/// Init the required UI by giving desired UI.
		/// </summary>
		/// <param name="QuickTimeEventTimerBar"></param>
		/// <param name="QuickTimeEventTimer_TB"></param>
		/// <param name="QuickTimeEventKeyMessage_TB"></param>
		/// <param name="QuickTimeStatusMessage_TB"></param>
		/// <param name="QuickTimeEvent_Image"></param>
		public void InitBixbiteUI(GameProgressBar QuickTimeEventTimerBar, GameTextBlock QuickTimeEventTimer_TB,
								GameTextBlock QuickTimeEventKeyMessage_TB, GameTextBlock QuickTimeStatusMessage_TB,
								GameImage QuickTimeEvent_Image	)
		{
			this.QuickTimeEventTimerBar = QuickTimeEventTimerBar;
			this.QuickTimeEventTimer_TB = QuickTimeEventTimer_TB;
			this.QuickTimeEventKeyMessage_TB = QuickTimeEventKeyMessage_TB;
			this.QuickTimeStatusMessage_TB = QuickTimeStatusMessage_TB;
			this.QuickTimeEvent_Image= QuickTimeEvent_Image;

		}

		#endregion

		#region Monogame

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		public void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			

			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		public void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		private void UpdateUI(GameTime gameTime)
		{
			if (QuickTimeEventTimer_TB != null)
			{
				QuickTimeEventTimer_TB.Update(gameTime);
				QuickTimeEventTimer_TB.Text = string.Format("{0} / {1} ms", (int)_currentRunTimeInMs, _totalRunTimeInMs);
			}

			if (QuickTimeEventKeyMessage_TB != null)
			{
				QuickTimeEventKeyMessage_TB.Update(gameTime);
			}

			if (QuickTimeStatusMessage_TB != null)
			{
				QuickTimeStatusMessage_TB.Update(gameTime);
			}

			if (QuickTimeEvent_Image != null)
			{
				QuickTimeEvent_Image.Update(gameTime);
			}

			// We need to updatea Progress bar.
			if (DisplayQuickTimeEventType == EQuickTimeEventDisplayType.ProgressBar)
			{
				if (QuickTimeEventTimerBar != null)
				{
					QuickTimeEventTimerBar.Update(gameTime);
				}
			}

		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public void Update(GameTime gameTime, KeyboardState prevKeyboardState, KeyboardState currentKeyboardState)
		{
			UpdateUI(gameTime);

			// Increment the currentRunTime.
			_currentRunTimeInMs = MathHelper.Max(0, (_currentRunTimeInMs - gameTime.ElapsedGameTime.Milliseconds));
			QuickTimeEventTimerBar.CurrentVal = (int)(((float)QuickTimeEventTimerBar.MaxVal) * (_currentRunTimeInMs / _totalRunTimeInMs));


			// the time has elapsed 
			if (_currentRunTimeInMs == 0)
			{
				if (OnQuickTimeEventFinished != null)
				{
					if (QuickTimeEventEffect == EQuickTimeEventEffect.Crop ||
						QuickTimeEventEffect == EQuickTimeEventEffect.Dispapear ||
						QuickTimeEventEffect == EQuickTimeEventEffect.Fade)
					{
						bIsActive = false;
					}

					OnQuickTimeEventFinished(new List<object>());
				}
			}

			// Hanlde the input Call delegates/Events for correct and incorrect choices if needed.
			if (currentKeyboardState.GetPressedKeys().Length > 0)
			{
				if ((int)currentKeyboardState.GetPressedKeys()[0] == _requiredKey && prevKeyboardState.IsKeyUp((Keys)_requiredKey))
				{
					if(OnQuickTimeEventCorrectKey != null)
					{
						OnQuickTimeEventCorrectKey(new List<object>());
					}
				}
				else if ((int)currentKeyboardState.GetPressedKeys()[0] != _currentKey && prevKeyboardState.IsKeyUp((Keys)_currentKey))
				{
					if (OnQuickTimeEventWrongKey != null)
					{
						OnQuickTimeEventWrongKey(new List<object>());
					}
				}
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			
			spriteBatch.Begin();

			if (QuickTimeEventTimerBar != null)
			{
				QuickTimeEventTimerBar.Draw(gameTime, spriteBatch);
			}

			if (QuickTimeEventTimer_TB != null)
			{
				QuickTimeEventTimer_TB.Draw(gameTime, spriteBatch);
			}

			if (QuickTimeEventKeyMessage_TB != null)
			{
				QuickTimeEventKeyMessage_TB.Draw(gameTime, spriteBatch);
			}

			if (QuickTimeStatusMessage_TB != null)
			{
				QuickTimeStatusMessage_TB.Draw(gameTime, spriteBatch);
			}

			if (QuickTimeEvent_Image != null)
			{
				QuickTimeEvent_Image.Draw(gameTime, spriteBatch);
			}
			
			spriteBatch.End();

			
		}


		#endregion

		#endregion


	}
}
