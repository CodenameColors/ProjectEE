using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BixBite.Combat;
using BixBite.Combat.Equipables;
using BixBite.Rendering.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Characters
{
	public class BattleEntity : BaseCharacter
	{

		//public enemy EnemyData;

		public override int Width
		{
			get
			{
				if (GetSpriteSheet() != null)
					return GetSpriteSheet().CurrentAnimation.GetFrameWidth();
				else return base.Width;
			}
		}
		public override int Height
		{
			get
			{
				if (GetSpriteSheet() != null)
					return GetSpriteSheet().CurrentAnimation.GetFrameHeight();
				else return base.Height;
			}
		}

		public override float ScaleX
		{
			get
			{
				if (GetSpriteSheet() != null)
					return GetSpriteSheet().CurrentAnimation.ScalarX;
				else return base.Width;
			}
			set
			{
				if (GetSpriteSheet() != null)
					GetSpriteSheet().CurrentAnimation.ScalarX = value;
				else base.ScaleX = value;
			}
		}

		public override float ScaleY
		{
			get
			{
				if (GetSpriteSheet() != null)
					return GetSpriteSheet().CurrentAnimation.ScalarY;
				else return base.Width;
			}
			set
			{
				if (GetSpriteSheet() != null)
					GetSpriteSheet().CurrentAnimation.ScalarY = value;
				else base.ScaleY = value;
			}
		}

		//public party_member PMData = new party_member();

		/// <summary>
		/// This is the health, mana, etc indicator for this character.
		/// </summary>
		private GameUI CombatStatsIndicator;

		public bool bIsDead { get; set; }

		public int MaxHealth
		{
			get { return Stats.Max_Health; }
			set { Stats.Max_Health = value; }
		}

		public int CurrentHealth
		{
			get => Stats.Current_Health;
			set
			{
				if (value < 0)
				{
					Stats.Current_Health = 0;
					return;
				}
				if (value > MaxHealth)
				{
					Stats.Current_Health = MaxHealth;
					if (CombatStatsIndicator != null && CombatStatsIndicator.UIElements[1] is GameProgressBar gp)
						gp.SetBarWidth(1.0f);
					return;
				}
				Stats.Current_Health = value;
				if (CombatStatsIndicator != null && CombatStatsIndicator.UIElements[1] is GameProgressBar gpb)
					gpb.SetBarWidth(((float)Stats.Current_Health / (float)MaxHealth));
			}
		}

		public int CurrentMana { get; set; }


		public int MaxMana
		{
			get { return Stats.Max_Mana; }
			set { Stats.Max_Mana = value; }
		}

		public List<BaseStatChange> StatChange_List = new List<BaseStatChange>();
		public List<BaseStatChange> StatusEffect_List = new List<BaseStatChange>();

		public Texture2D Icon { get; set; }

		//Foreign Keys
		public int Stats_FK { get; set; }
		public int Weak_Strength_FK { get; set; }

		public BaseStats Stats = new BaseStats();
		public WeakStrength WeaknessAndStrengths = new WeakStrength();

		public List<Skill> Skills = new List<Skill>(8);
		public LinkedList<Weapon> Weapons = new LinkedList<Weapon>();
		public LinkedListNode<Weapon> _currentweapon;
		public LinkedListNode<Weapon> CurrentWeapon
		{
			get => _currentweapon;
			set
			{
				_currentweapon = value;
				if(value != null)
					Equipement[5] = value.Value;
			}
		}
		public List<Items.Item> Items = new List<Items.Item>(4);


		protected Equipable[] Equipement = {null, null, null, null, null, null};
		public String HeadGear_FK { get; set; }
		public String BodyGear_FK { get; set; }
		public String LegGear_FK { get; set; }

		public String Accessory1_FK { get; set; }
		public String Accessory2_FK { get; set; }

		public int Level { get; set; }

		public BattleEntity()
		{
			CurrentWeapon = Weapons.First;
		}

		public Equipable[] GetEquipment()
		{
			return Equipement;
		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			base.Draw(gameTime, spriteBatch);
		}
	}
}
