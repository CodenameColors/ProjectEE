using BixBite;
using BixBite.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectE_E.Components;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ExpirementalGameTester : Game
    {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D t2;
		Map map;
		Map C_map; //for map change testing
		Player Player;
		Camera camera;

		string MainLevelPath = "";

		public ExpirementalGameTester()
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
			
			//TestSprite.setTexture(t); //set the image.
			Tile.Content = this.Content;

			camera = new Camera(GraphicsDevice.Viewport);
			Load_async();

			Thread.Sleep(System.TimeSpan.FromMilliseconds(100));

			map.LoadTileMaps(this.GraphicsDevice, map.level);
			map.LoadSprites(this.GraphicsDevice, map.level);
			map.GenerateLevel(map.level, this.GraphicsDevice, spriteBatch);


			Player.Load(this.Content);
			// TODO: use this.Content to load your game content here

			//C_map.level = Level.ImportLevel("C:\\Users\\Antonio\\Documents\\createst\\test2\\test2_Game\\Content\\Levels\\LevelChangeTestNew.lvl");

		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
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
			}
			else
			{
				camera.Update(Player.Position, 0, 0);
			}


			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);


			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
			if (map != null)
				map.Draw(spriteBatch);
			Player.Draw(spriteBatch);
			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
