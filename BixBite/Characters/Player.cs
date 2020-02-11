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
	public class Player : PartyMember
	{
		public Player()
		{
			Position = new Vector2(100, 100);
		}

		public void Load(ContentManager Content)
		{
			 Texture = Content.Load<Texture2D>("Images/Player");
		}

		public void Update(GameTime gameTime)
		{
			Position += Velocity;
			DrawPosRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

			Input(gameTime);

			//UNCOMMENT FOR PLATFORMING
			//if (Velocity.Y < 10)
			//	Velocity.Y += .4f;

		}

		private void Input(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.D))
				Velocity.X = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;

			else if (Keyboard.GetState().IsKeyDown(Keys.A))
				Velocity.X = -(float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;

			else Velocity.X = 0f;

			//UNCOMMENT FOR PLATFORMING
			//if (Keyboard.GetState().IsKeyDown(Keys.Space) && hasJumped == false)
			//{
			//	Position.Y -= 5f;
			//	Velocity.Y = -9f;
			//	hasJumped = true;
			//}
			if (Keyboard.GetState().IsKeyDown(Keys.S))
				Velocity.Y = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;

			else if (Keyboard.GetState().IsKeyDown(Keys.W))
				Velocity.Y = -(float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;

			else Velocity.Y = 0f;


			if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.T))
			{
				Console.WriteLine("Testing Button for Character");
			}

		}


		public void Triggering(int EventGroup)
		{
			
		}
	}
}
