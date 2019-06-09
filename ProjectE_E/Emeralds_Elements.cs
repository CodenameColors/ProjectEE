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
		Player Player;
		Camera camera;

		string MainLevelPath = "";

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

			Thread.Sleep(System.TimeSpan.FromMilliseconds(50));

			map.LoadTileMaps(this.GraphicsDevice, map.level);
			map.LoadSprites(this.GraphicsDevice, map.level);
			map.GenerateLevel(map.level, this.GraphicsDevice, spriteBatch);

			Player.Load(this.Content);
			

			// TODO: use this.Content to load your game content here
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
			//foreach (CollisionTiles tile in map.CollisionTiles)
			//{
			//	Player.Collision(tile.Rectangle, map.Width, map.Height);
			camera.Update(Player.Position, map.Width, map.Height);
			//}
			
			base.Update(gameTime);
		}

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			//SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform
			spriteBatch.Begin();
			//spriteBatch.Draw(t2, new Vector2(0, 0));
			map.Draw(spriteBatch);
			Player.Draw(spriteBatch);
			spriteBatch.End();
			base.Draw(gameTime);
		}
  }
}
