using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Characters
{
	public class Player
	{
		private Texture2D texture;
		private Vector2 position;
		private Vector2 velocity;
		private Rectangle rectangle;

		private bool hasJumped = false;

		public Vector2 Position
		{
			get { return position; }
		}

		public Player()
		{
			position = new Vector2(0, 0);
		}

		public void Load(ContentManager Content)
		{
			texture = Content.Load<Texture2D>("Images/Player");
		}

		public void Update(GameTime gameTime)
		{
			position += velocity;
			rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

			Input(gameTime);

			//UNCOMMENT FOR PLATFORMING
			//if (velocity.Y < 10)
			//	velocity.Y += .4f;

		}

		private void Input(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.D))
				velocity.X = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;

			else if (Keyboard.GetState().IsKeyDown(Keys.A))
				velocity.X = -(float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;

			else velocity.X = 0f;

			//UNCOMMENT FOR PLATFORMING
			//if (Keyboard.GetState().IsKeyDown(Keys.Space) && hasJumped == false)
			//{
			//	position.Y -= 5f;
			//	velocity.Y = -9f;
			//	hasJumped = true;
			//}
			if (Keyboard.GetState().IsKeyDown(Keys.S))
				velocity.Y = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;

			else if (Keyboard.GetState().IsKeyDown(Keys.W))
				velocity.Y = -(float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;

			else velocity.Y = 0f;


			if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.T))
			{
				Console.WriteLine("Testing Button for Character");
			}

		}

		public void Collision(Rectangle newRectangle, int xOffset, int yOffset, int EventGroup)
		{
			if (EventGroup != -1) return;

			if (rectangle.TouchTopOf(newRectangle))
			{
				rectangle.Y = newRectangle.Y - rectangle.Height;
				velocity.Y = 0f;
				hasJumped = false;
			}
			if (rectangle.TouchLeftOf(newRectangle))
			{
				position.X = newRectangle.X - rectangle.Width - 2;
			}
			if (rectangle.TouchRightOf(newRectangle))
			{
				position.X = newRectangle.X + rectangle.Width + 10;
			}
			if (rectangle.TouchBottomOf(newRectangle))
			{
				velocity.Y = 0;
			}

			//this is for checking the map. Don't allow out of bounds.
			if (position.X < 0) position.X = 0;
			if (position.X > xOffset - rectangle.Width) position.X = xOffset - rectangle.Width;
			if (position.Y < 0) velocity.Y = 1f;
			if (position.Y > yOffset - rectangle.Height) position.Y = yOffset - rectangle.Height;

		}

		public void Triggering(int EventGroup)
		{
			
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, rectangle, Color.White);
		}



	}
}
