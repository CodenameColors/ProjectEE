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
		string MainLevelPath = "";
		private Color _backgroundColour;

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

			

			// TODO: use this.Content to load your game content here

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
			//if (Keyboard.GetState().IsKeyDown(Keys.T)) {
			//	foreach (SpriteLayer sl in map.level.Layers) {
			//		if (sl.layerType == LayerType.GameEvent) {
			//			map.FillDictLUT(((System.Tuple<int[,], List<GameEvent>>)sl.LayerObjects).Item2);
			//		}
			//		System.Console.WriteLine("T DOWN");
			//	}
			//	Player.SetPosition(0, 0);
			//}

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

			base.Update(gameTime);
		}

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(_backgroundColour);



			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
			//spriteBatch.Draw(t2, new Vector2(0, 0));
			if(map != null)
				map.Draw(spriteBatch);
			Player.Draw(spriteBatch);

			foreach (UIComponent ui in _uiComponents)
			{
				ui.Draw(gameTime, spriteBatch);
			}

			spriteBatch.End();
			base.Draw(gameTime);
		}
  }
}
