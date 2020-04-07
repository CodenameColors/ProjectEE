using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BixBite.Characters
{
	public class Enemy : BaseCharacter
	{
		public Vector2 SpawnPosition = new Vector2();

		public bool bIsDead { get; set; }
		public int MaxHealth { get; set; }

		private int _currentHealth;
		public int CurrentHealth
		{
			get => _currentHealth;
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
				//if (CombatStatsIndicator.UIElements[1] is GameProgressBar gpb)
				//	gpb.SetBarLength(_currentHealth);
			}
		}

	}
}
