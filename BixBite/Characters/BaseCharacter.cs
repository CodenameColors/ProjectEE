﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Characters
{
	public class BaseCharacter
	{
		public Texture2D Texture;
		public Vector2 Velocity = new Vector2();
		public Rectangle DrawPosRectangle = new Rectangle();
		public Vector2 Position = new Vector2();
		public bool bHasJumped;
		public int Zindex = 1;

		public BaseCharacter()
		{

		}


		public void SetPosition(int x, int y)
		{
			Position.X = x;
			Position.Y = y;
		}

		public void Collision(Rectangle newRectangle, int xOffset, int yOffset, int EventGroup)
		{
			if (EventGroup != -1) return;

			if (DrawPosRectangle.TouchTopOf(newRectangle))
			{
				//Rectangle.Y = newRectangle.Y - Rectangle.Height;
				//Velocity.Y = 0f;
				bHasJumped = false;
				Position.Y = newRectangle.Y - DrawPosRectangle.Height - 2;
			}
			if (DrawPosRectangle.TouchLeftOf(newRectangle))
			{
				Position.X = newRectangle.X - DrawPosRectangle.Width - 2;
			}
			if (DrawPosRectangle.TouchRightOf(newRectangle))
			{
				Position.X = newRectangle.X + DrawPosRectangle.Width + 10;
			}
			if (DrawPosRectangle.TouchBottomOf(newRectangle))
			{
				Position.Y = newRectangle.Y + DrawPosRectangle.Height + 10;
			}

			//this is for checking the map. Don't allow out of bounds.
			if (Position.X < 0) Position.X = 0;
			if (Position.X > xOffset - DrawPosRectangle.Width) Position.X = xOffset - DrawPosRectangle.Width;
			if (Position.Y < 0) Velocity.Y = 1f;
			if (Position.Y > yOffset - DrawPosRectangle.Height) Position.Y = yOffset - DrawPosRectangle.Height;

		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, DrawPosRectangle, Color.White);
		}

	}
}
