﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Items;

namespace BixBite.Characters
{
	public class Player : player
	{

		#region Delegates

		#endregion

		#region Fields

		#endregion

		#region Properties
		public List<Item> Inventory_Main = new List<Item>();
		public List<PartyMember> AllPartyMembers = new List<PartyMember>();
		public List<PartyMember> ActivePartyMembers = new List<PartyMember>();
		
		#endregion

		#region Delegates

		#endregion

		#region Constructors

		public Player()
		{
			Position = new Vector2(100, 100);
		}
		#endregion


		#region Methods
		//public void Load(ContentManager Content)
		//{
		//	Texture = Content.Load<Texture2D>("Images/Player");
		//}

		//public void LoadPlayerImage(ContentManager Content, String ContentPath)
		//{
		//	Texture = Content.Load<Texture2D>(ContentPath);
		//	this.Width = Texture.Width;
		//	this.Height = Texture.Height;
		//}

		public override void Update(GameTime gameTime)
		{
			Position += Velocity;
			DrawPosRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

			Input(gameTime);


			//UNCOMMENT FOR PLATFORMING
			//if (Velocity.Y < 10)
			//	Velocity.Y += .4f;

		}

		private void Input(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.A))
				Velocity.X = 0;
			else if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				Velocity.X = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;
				//spriteAnimationSheet.CurrentAnimation = spriteAnimationSheet.SpriteAnimations[0];
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				Velocity.X = -(float)gameTime.ElapsedGameTime.TotalMilliseconds / 3;
				//spriteAnimationSheet.CurrentAnimation = spriteAnimationSheet.SpriteAnimations[1];
			}

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
		#endregion

	}

	/// <summary>
	/// Use this for Database Linking!
	/// </summary>
	public class player : BattleEntity
	{

		public String ID { get; set; }

		//Social Stats.
		public int Smarts { get; set; }
		public int Confidence { get; set; }
		public int Skilled { get; set; }
		public int Popularity { get; set; }
		public int NiceGuy { get; set; }

		//Combat Essence Trackers
		public int Essence { get; set; }
		public int Max_Essence { get; set; }
		public int Essence_Portion { get; set; }
		public int Max_Essence_Portion { get; set; }
		

	}


}
