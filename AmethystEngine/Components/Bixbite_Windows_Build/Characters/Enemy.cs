using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Combat;
using BixBite.Combat.Equipables;
using Microsoft.Xna.Framework;

namespace BixBite.Characters
{
	public class Enemy : enemy
	{

		//Clothes
		public Clothes HeadClothes
		{
			get => (Clothes)Equipement[0];
			set => Equipement[0] = value;
		}
		public Clothes BodyClothes
		{
			get => (Clothes)Equipement[1];
			set => Equipement[1] = value;
		}
		public Clothes LegsClothes
		{
			get => (Clothes)Equipement[2];
			set => Equipement[2] = value;
		}

		#region ICombat Interface
		/// <summary>
		/// This function is here to handle stats that need to change on the loading of a battle entity 
		/// in the combat system. Simple things, like adding and subtracting stats, status effects. etc
		/// </summary>
		public override void Initialize()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This function is here to handle all things that need to be done when a battle entity attacks
		/// Examples include stealing stats, stealing money, subtracting ammo/charge count, other stat changes etc.
		/// THIS means the attacking entity NOT the target entity
		/// </summary>
		public override void Attack()
		{

		}

		/// <summary>
		/// Handles all the stat changes that are needed when an entity chooses to defend
		/// </summary>
		public override void Defend()
		{
			throw new NotImplementedException();

		}

		/// <summary>
		/// Handles all the required changes that need to be done when an entity gets hit.
		/// Stat changes, take damage, maybe deal damage (thorns), etc
		/// </summary>
		public override void GotHit(BattleEntity attackingBattleEntity)
		{
			throw new NotImplementedException();

		}
		#endregion



	}


	public class enemy : BattleEntity
	{
		#region Database Linking

		public String Name { get; set; }

		public int Size_Type { get; set; }
		public int EXP { get; set; }
		public int Rarity { get; set; }

		public List<Items.Item> Drops = new List<Items.Item>();


		#endregion
	}

}
