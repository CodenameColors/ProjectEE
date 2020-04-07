using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BixBite.Rendering.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Characters
{
	public class PartyMember : BaseCharacter
	{
		/// <summary>
		/// This is the health, mana, etc indicator for this character.
		/// </summary>
		private GameUI CombatStatsIndicator;

		public Vector2 SpawnPosition = new Vector2();

		public bool bIsDead { get; set; }
		public int MaxHealth { get; set; }

		private int _currentHealth;
		public int CurrentHealth {
			get=> _currentHealth;
			set
			{
				if (value < 0)
				{
					_currentHealth = 0;
					return;
				}
				if (value > MaxHealth)
				{
					_currentHealth = MaxHealth;
					return;
				}
				_currentHealth = value;
				if (CombatStatsIndicator.UIElements[1] is GameProgressBar gpb)
					gpb.SetBarLength(_currentHealth);
			}
		}

		/// <summary>
		/// DEFAULT constructor
		/// </summary>
		public PartyMember()
		{

		}

		/// <summary>
		/// Constructor for COMBAT
		/// </summary>
		/// <param name="BaseCombatUI"> The UI that we will add everything too. </param>
		/// <param name="CombatIcon"></param>
		/// <param name="healthBar"></param>
		/// <param name="manaBar"></param>
		public PartyMember(GameUI BaseCombatUI, GameIMG CombatIcon, GameProgressBar healthBar, GameProgressBar manaBar)
		{
			//Create the UI
			CombatIcon.SetUITexture(); //set the 
			BaseCombatUI.UIElements.Add(CombatIcon);
			BaseCombatUI.UIElements.Add(healthBar);
			BaseCombatUI.UIElements.Add(manaBar);
			CombatStatsIndicator = BaseCombatUI;
		}


		public override void Draw(GameTime gameTime ,SpriteBatch spriteBatch)
		{
			base.Draw( gameTime ,spriteBatch);
			CombatStatsIndicator?.Draw(gameTime, spriteBatch);

		}
	}
}
