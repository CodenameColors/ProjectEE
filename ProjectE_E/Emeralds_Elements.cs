using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BixBite;
using BixBite.Rendering;
using BixBite.Characters;
using ProjectE_E.Components;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using BixBite.Rendering.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using NodeEditor;
using NodeEditor.Components;

namespace ProjectE_E
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
	public class Emeralds_Elements : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D t2;
		Map map;
		Map C_map; //for map change testing
		Player Player;
		Camera camera;

		private List<UIComponent> _uiComponents;
		private GameUI TestGameUIFromEngine;

		string MainLevelPath = "";
		private Color _backgroundColour;

		private List<UIComponent> dialoguechoicebtns= new List<UIComponent>();

		private DialogueNodeBlock curDiaBlock;

		private DialogueNodeBlock diaTestBlock_1;
		private DialogueNodeBlock diaTestBlock_2;
		private DialogueNodeBlock diaTestBlock_3;

		private DialogueNodeBlock diaTestBlock_choices1;

		private bool _TestKeyDown = false;

		public Emeralds_Elements()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.PreferredBackBufferHeight = 1000;
			graphics.PreferredBackBufferWidth = 1000;

			Directory.SetCurrentDirectory(@"..\..\..\..\..");
			string TempDir = Directory.GetCurrentDirectory();
			string[] filePaths = Directory.GetFiles(TempDir, "*.gem");
			if (filePaths.Length == 0)
			{
				return;
			}
			using (StreamReader file = new StreamReader(filePaths[0]))
			{
				int counter = 0;
				string ln;

				while ((ln = file.ReadLine()) != null)
				{
					if (ln.Contains("MainLevel"))
					{
						ln = file.ReadLine();
						MainLevelPath = ln;
					}
				}
					counter++;
				
				file.Close();
			}
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			//create map
			map = new Map();
			C_map = new Map();
			Player = new Player();
			TestGameUIFromEngine = GameUI.ImportGameUI("C:\\Users\\Antonio\\Documents\\createst\\test2\\test2_Game\\Content\\UI\\EE_right.UI");

			//Dialogue choice testing
			diaTestBlock_choices1 = new DialogueNodeBlock("Emma", true);
			diaTestBlock_choices1.DialogueTextOptions = new ObservableCollection<string>(){"Opition 1", "Opition 2", "Opition 3"};
			diaTestBlock_choices1.OutputNodes.Add(new ConnectionNode(diaTestBlock_choices1, "OutputNode1", ECOnnectionType.Exit));
			diaTestBlock_choices1.OutputNodes.Add(new ConnectionNode(diaTestBlock_choices1, "OutputNode2", ECOnnectionType.Exit));
			diaTestBlock_choices1.OutputNodes.Add(new ConnectionNode(diaTestBlock_choices1, "OutputNode3", ECOnnectionType.Exit));

			//dialogue block traversal testing
			StartBlockNode start =new StartBlockNode();

			diaTestBlock_1 = new DialogueNodeBlock("Emma", true);
			diaTestBlock_1.DialogueTextOptions[0] = "Hello!";
			curDiaBlock = diaTestBlock_1;

			diaTestBlock_2 = new DialogueNodeBlock("Emma", true);
			diaTestBlock_2.DialogueTextOptions[0] = "how are you?!";

			diaTestBlock_3 = new DialogueNodeBlock("Emma", true);
			diaTestBlock_3.DialogueTextOptions[0] = "Hey! are you there?!";

			//create outputs and link them together.
			start.ExitNode.ConnectedNodes.Add(diaTestBlock_1.EntryNode);
			diaTestBlock_1.EntryNode.ConnectedNodes.Add(start.ExitNode);
			diaTestBlock_1.OutputNodes.Add(new ConnectionNode(diaTestBlock_1, "OutputNode1", ECOnnectionType.Exit));
			diaTestBlock_2.OutputNodes.Add(new ConnectionNode(diaTestBlock_1, "OutputNode1", ECOnnectionType.Exit));
			diaTestBlock_3.OutputNodes.Add(new ConnectionNode(diaTestBlock_1, "OutputNode1", ECOnnectionType.Exit));

			diaTestBlock_1.OutputNodes[0].ConnectedNodes.Add(diaTestBlock_2.EntryNode);
			diaTestBlock_2.EntryNode.ConnectedNodes.Add(diaTestBlock_1.OutputNodes.First());

			diaTestBlock_2.OutputNodes[0].ConnectedNodes.Add(diaTestBlock_3.EntryNode);
			diaTestBlock_3.EntryNode.ConnectedNodes.Add(diaTestBlock_2.OutputNodes.First());

			diaTestBlock_3.OutputNodes[0].ConnectedNodes.Add(diaTestBlock_choices1.EntryNode);
			diaTestBlock_choices1.EntryNode.ConnectedNodes.Add(diaTestBlock_3.OutputNodes.First());


			diaTestBlock_choices1.OutputNodes[0].ConnectedNodes.Add(diaTestBlock_1.EntryNode);
			diaTestBlock_choices1.OutputNodes[1].ConnectedNodes.Add(diaTestBlock_2.EntryNode);
			diaTestBlock_choices1.OutputNodes[2].ConnectedNodes.Add(diaTestBlock_3.EntryNode);



			this.IsMouseVisible = true;
			base.Initialize();
		}


		protected async void Load_async()
		{
			 map.level = await map.GetLevelFromFile(MainLevelPath);
			
		}

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Texture2D t = this.Content.Load<Texture2D>("smolmegumin");
			//t2 = this.Content.Load<Texture2D>("Images/PathAndObjects");
			//Texture2D t = this.Content.Load<Texture2D>("smolmegumin");


			//TestSprite.setTexture(t); //set the image.
			Tile.Content = this.Content;

			camera = new Camera(GraphicsDevice.Viewport);
			
		//load the assets.
		//this.Content.Load<Texture2D>("Images/Tile1");


		//Tile.Content = this.Content;

			Load_async();

			Thread.Sleep(System.TimeSpan.FromMilliseconds(100));

			map.LoadTileMaps(this.GraphicsDevice, map.level);
			map.LoadSprites(this.GraphicsDevice, map.level);
			map.GenerateLevel(map.level, this.GraphicsDevice, spriteBatch);
			

			Player.Load(this.Content);




			//UI TESTING STARTS HEREE
			var randomButton = new GameButton(Content.Load<Texture2D>("Images/Button"), Content.Load<SpriteFont>("Fonts/File"))
			{
				Position = new Vector2(350, 800),
				Text = "Random",
			};

			randomButton.Click += RandomButton_Click;

			var quitButton = new GameButton(Content.Load<Texture2D>("Images/Button"), Content.Load<SpriteFont>("Fonts/File"))
			{
				Position = new Vector2(350, 850),
				Text = "Quit",
			};

			quitButton.Click += QuitButton_Click;

			_uiComponents = new List<UIComponent>()
			{
				randomButton,
				quitButton,
			};


			foreach (GameUI gameUi in TestGameUIFromEngine.UIElements)
			{
				if (gameUi is GameTextBlock GTB)
				{
					GTB._font = Content.Load<SpriteFont>("Fonts/File");
					GTB.Position = new Vector2(350, 400);
					GTB.graphicsDevice = GraphicsDevice;
					GTB.SetUITexture(); //set the current desired texture to be drawn on draw/update
				}
				else if (gameUi is GameIMG GIMG)
				{
					GIMG.Position = new Vector2(350, 450);
					GIMG.SetGraphicsDeviceRef(GraphicsDevice);
					GIMG.SetUITexture(); //set the current desired texture to be drawn on draw/update
				}
				_uiComponents.Add(gameUi);
			}
			// TODO: use this.Content to load your game content here

			((GameTextBlock)_uiComponents.First(x => x is GameTextBlock)).Text = diaTestBlock_1.DialogueTextOptions[0];

			//C_map.level = Level.ImportLevel("C:\\Users\\Antonio\\Documents\\createst\\test2\\test2_Game\\Content\\Levels\\LevelChangeTestNew.lvl");
		}

		private void QuitButton_Click(object sender, System.EventArgs e)
		{
			Exit();
		}

		private void RandomButton_Click(object sender, System.EventArgs e)
		{
			var random = new Random();

			_backgroundColour = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
		}



		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
				// TODO: Unload any non ContentManager content here
				//TestSprite.unload
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			Player.Update(gameTime);
			CheckMapCollision(); //check collision of map tile for game events

			//move the camera to follow the player
			if (map != null)
			{
				camera.Update(Player.Position, map.Width, map.Height);
				//if (Player.Position.Y > 1000)
				//{
				//	map.UnloadMap();
				//	map = null;
				//}
			}
			else
			{
				
				camera.Update(Player.Position, 0, 0);
			}
			//}
			if (Keyboard.GetState().IsKeyDown(Keys.T))
			{

				//(_uiComponents.Last() as GameTextBlock).Text = "What's sup FUCKERS";
				//((GameTextBlock) _uiComponents.First(x=> x is GameTextBlock)).Text = "What's sup FUCKERS";

				//if (curDiaBlock.OnStartNodeBlockExecution(ref curDiaBlock))
				//{
				//	if (curDiaBlock.NodeBlockExecution(ref curDiaBlock))
				//	{
				//		curDiaBlock.OnEndNodeBlockExecution(ref curDiaBlock);
				//	}
				//}

				if(!(curDiaBlock.NextDialogueNodeBlock is null))
					curDiaBlock = curDiaBlock.NextDialogueNodeBlock;


				//if(diaTestBlock.OutputNodes.Count > 1)
				if (curDiaBlock.DialogueTextOptions.Count> 1)
					DisplayDialogueChoices(diaTestBlock_choices1, new Rectangle(0, 400, 500, 300), 20);
				else
					dialoguechoicebtns.Clear();
					((GameTextBlock)_uiComponents.First(x => x is GameTextBlock)).Text = (curDiaBlock as DialogueNodeBlock)?.DialogueTextOptions[0];
				Thread.Sleep(100);
			}

			//if (Keyboard.GetState().IsKeyDown(Keys.M))
			//{
			//	map = C_map;
			//	map.LoadTileMaps(this.GraphicsDevice, map.level);
			//	map.LoadSprites(this.GraphicsDevice, map.level);
			//	map.GenerateLevel(map.level, this.GraphicsDevice, spriteBatch);
			//	System.Console.WriteLine("M DOWN");
			//}

			foreach (UIComponent ui in _uiComponents)
			{
				ui.Update(gameTime);
			}
			foreach (UIComponent ui in dialoguechoicebtns)
			{
				ui.Update(gameTime);
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This method is here to display multiple dialogue choices to the screen for the user.
		/// it uses the dialogue block properties inside to read and display said data.
		/// </summary>
		/// <param name="diablock"></param>
		/// <param name="drawAreaRectangle"></param>
		private void DisplayDialogueChoices(DialogueNodeBlock diablock, Rectangle drawAreaRectangle, int buttonspacing)
		{
			dialoguechoicebtns.Clear();
			int numofchoices = diablock.DialogueTextOptions.Count;

			//we are going to assume i want these dialogue choices to be displayed in the center
			//create some buttons
			for (int i = 0; i < numofchoices; i++)
			{
				GameButton tempbtn = new GameButton(
					Content.Load<Texture2D>("Images/Button"), Content.Load<SpriteFont>("Fonts/File"))
				{
					Position = new Vector2(
						drawAreaRectangle.X + (drawAreaRectangle.Width / 2),
						drawAreaRectangle.Y + ((drawAreaRectangle.Height / 2)) + ((buttonspacing + 30) * i)),
					Text = diablock.DialogueTextOptions[i]
				};
				tempbtn.Click += (object sender, EventArgs e) =>
				{
					((GameTextBlock)_uiComponents.First(x => x is GameTextBlock)).Text = (curDiaBlock as DialogueNodeBlock)?.DialogueTextOptions[dialoguechoicebtns.IndexOf(sender as GameButton)];
					((GameIMG)_uiComponents.First(x => x is GameIMG)).SetProperty("Image", "C:\\Users\\Antonio\\Documents\\createst\\test2\\test2_Game\\Content\\Images\\PC_girl\\listening_right.PNG");
					((GameIMG)_uiComponents.First(x => x is GameIMG)).SetUITexture();
					(curDiaBlock as DialogueNodeBlock).ChoiceVar = dialoguechoicebtns.IndexOf(sender as GameButton);
					Console.WriteLine("Choose Option:{0}", dialoguechoicebtns.IndexOf(sender as GameButton));
				};
				dialoguechoicebtns.Add(tempbtn);
				
			}


		}

		private void CheckMapCollision()
		{
			//this is collision detection
			//foreach (Tile tile in map.MapTiles)
			for (int i = 0; i < map.MapTiles.Count; i++)
			{
				Player.Collision(map.MapTiles[i].Rectangle, map.Width, map.Height, map.MapTiles[i].EventGroup);
				if (map.EventLUT.Count > 0 && map.MapTiles[i].EventGroup > 0 && (int)Player.Position.X / 40 == map.MapTiles[i].Rectangle.X / 40 && (int)Player.Position.Y / 40 == map.MapTiles[i].Rectangle.Y / 40)
				{
					var v = map.GetMapEvent(map.MapTiles[i].EventGroup);
					if (v != null)
					{
						if (map.MapTiles[i].EventType == 1) // level transition
						{
							GameEvent g = map.MapEvents[map.MapTiles[i].EventGroup];
							List<object> Prams = new List<object>
							{
								this.GraphicsDevice,
								this.spriteBatch,
								Player,
								map,
								g.datatoload.NewFileToLoad,
								(int)g.datatoload.newx,
								(int)g.datatoload.newy,
								(int)g.datatoload.MoveTime
							};

							map.level = null;
							map.MapEvents.Clear();
							map.ClearEventLUT();
							map.MapTiles.Clear();
							v.Invoke(null, Prams.ToArray());

							System.Console.WriteLine("M Change");
							return;
						}

					}
				}
			}
		}

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(_backgroundColour);
			//applying the camera into the parameters is how i move the camera with the player.
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
			//spriteBatch.Draw(t2, new Vector2(0, 0));
			if(map != null)
				map.Draw(spriteBatch);
			Player.Draw(gameTime ,spriteBatch);

			foreach (UIComponent ui in _uiComponents)
			{
				ui.Draw(gameTime, spriteBatch); //draw UI to screen. BUT it doesn't handle events!
			}

			foreach (UIComponent ui in dialoguechoicebtns)
			{
				ui.Draw(gameTime, spriteBatch); //draw UI to screen. BUT it doesn't handle events!
			}

			//TestGameUIFromEngine.Draw(gameTime, spriteBatch);

			spriteBatch.End();
			base.Draw(gameTime);
		}
  }
}
