using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BixBite.Rendering;
using BixBite.Characters;
using ProjectE_E.Components;

namespace ProjectE_E
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
	public class Emeralds_Elements : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Sprite TestSprite = new Sprite();

		Map map;
		Player Player;
		Camera camera;

		public Emeralds_Elements()
		{
				graphics = new GraphicsDeviceManager(this);
				Content.RootDirectory = "Content";
				graphics.PreferredBackBufferHeight = 520;
				graphics.PreferredBackBufferWidth = 800;
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

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Texture2D t = this.Content.Load<Texture2D>("smolmegumin");
			TestSprite.setTexture(t); //set the image.

			camera = new Camera(GraphicsDevice.Viewport);

			//load the assets.
			//this.Content.Load<Texture2D>("Images/Tile1");
			//this.Content.Load<Texture2D>("Images/Tile2");

			Tiles.Content = this.Content;
			map.Generate(new int[,]
			{
					//{ 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
					//{ 2,1,1,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,},
					//{ 2,2,27,1,0,0,1,1,1,2,2,2,1,0,0,0,0,0,0,0,},
					//{ 2,0,0,0,0,1,2,2,2,2,2,2,2,1,0,0,0,0,0,0,},
					//{ 2,0,0,1,1,2,2,2,2,2,2,2,2,2,1,1,1,0,0,0,},
					//{ 2,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,1,1,},

				//my dumb test map LUL
				{ 0,0,0,2,2,2,2,2,2,0,0,2,2,2,2,2,2,2,2,2,},
				{ 0,0,0,2,2,2,2,2,0,0,0,0,0,2,2,2,2,2,0,2,},
				{ 0,0,0,0,0,0,2,2,0,0,0,0,0,2,2,2,2,2,0,2,},
				{ 1,0,0,0,0,0,2,0,0,0,0,0,0,2,2,2,2,2,0,2,},
				{ 2,1,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,0,2,},
				{ 2,2,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,0,2,},
				{ 2,2,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,0,2,},
				{ 2,2,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,},
				{ 2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,},
				{ 2,2,1,0,0,0,0,0,1,1,0,0,0,0,0,2,0,0,0,0,},
				{ 2,2,2,1,0,0,0,1,2,2,2,2,0,0,0,0,0,0,0,0,},
				{ 2,2,2,2,1,1,1,2,2,0,1,1,1,1,1,1,1,1,1,1,},
				{ 2,2,2,2,2,2,2,2,2,0,2,2,2,2,2,2,2,2,2,2,},

			}, 64);

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
			foreach (CollisionTiles tile in map.CollisionTiles)
			{
				Player.Collision(tile.Rectangle, map.Width, map.Height);
				camera.Update(Player.Position, map.Width, map.Height);
			}
			
			base.Update(gameTime);
		}

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
			//spriteBatch.Draw(TestSprite.getTexture(), new Vector2(0, 0));
			map.Draw(spriteBatch);
			Player.Draw(spriteBatch);
			// TODO: Add your drawing code here
			spriteBatch.End();
			base.Draw(gameTime);
		}
  }
}
