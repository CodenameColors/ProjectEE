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
