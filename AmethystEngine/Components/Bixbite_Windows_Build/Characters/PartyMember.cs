using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using BixBite.Combat;
using BixBite.Combat.Equipables;
using BixBite.Combat.Equipables.Weapons;
using BixBite.Rendering.UI;
using BixBite.Rendering.UI.Button;
using BixBite.Rendering.UI.Checkbox;
using BixBite.Rendering.UI.Image;
using BixBite.Rendering.UI.ListBox;
using BixBite.Rendering.UI.ListBox.ListBoxItems;
using BixBite.Rendering.UI.ProgressBar;
using BixBite.Rendering.UI.TabControl;
using BixBite.Rendering.UI.TextBlock;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace BixBite.Characters
{
	public class PartyMember : party_member
	{
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
			get {
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


		public int MaxEssence_Points { get; set; }
		public int Essence_Points { get; set; }
		public EMagicType EssenceType { get; set; }

		//public party_member PMData = new party_member();

		/// <summary>
		/// This is the health, mana, etc indicator for this character.
		/// </summary>
		private BaseUI CombatStatsIndicator;

		public bool bIsDead { get; set; }

		public int MaxHealth
		{
			get { return Stats.Max_Health; }
			set { Stats.Max_Health = value; }
		}

		public int CurrentHealth {
			get=> Stats.Current_Health;
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
					if (CombatStatsIndicator.UIElements[1] is GameProgressBar gp)
						gp.SetBarWidth(1.0f);
					return;
				}
				Stats.Current_Health = value;
				if (CombatStatsIndicator.UIElements[1] is GameProgressBar gpb)
					gpb.SetBarWidth(((float)Stats.Current_Health/ (float)MaxHealth));
			}
		}

		public int MaxMana
		{
			get { return Stats.Max_Mana; }
			set { Stats.Max_Mana = value; }
		}

		public Job Job = new Job();

		public Texture2D StanceIndicator_Texture2D;

		public Rectangle StanceIndicator_RectangleOffset { get; set; }
		public Rectangle StanceIndicator_Rectangle => StanceIndicator_RectangleOffset.AddRectanglePosition(Position);

		public Color StanceIndicator_Color = Color.White;

		public LinkedList<EStanceType> Stances_LL = new LinkedList<EStanceType>();
		public LinkedListNode<EStanceType> CurrentStance;

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

		//Accessories
		public Accessory AttackAccessory1
		{
			get => (Accessory)Equipement[3];
			set => Equipement[3] = value;
		}
		public Accessory AttackAccessory2
		{
			get => (Accessory)Equipement[4];
			set => Equipement[4] = value;
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
		public PartyMember(BaseUI BaseCombatUI, Texture2D CombatIcon, GameProgressBar healthBar, GameProgressBar manaBar)
		{
			//Create the UI

			if (CombatIcon != null)
			{
				//BaseCombatUI.UIElements.Add(CombatIcon);
				//CombatIcon.SetUITexture(); //set the 
				Icon = CombatIcon;
			}
			if(healthBar != null)
				BaseCombatUI.UIElements.Add(healthBar);
			if(manaBar != null)
				BaseCombatUI.UIElements.Add(manaBar);
			CombatStatsIndicator = BaseCombatUI;

			CurrentStance = Stances_LL.First;
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

		public override bool CanAttack()
		{
			bool returnBool = true;

			if ((EWeaponType)CurrentWeapon.Value.Weapon_Type == EWeaponType.Bow || (EWeaponType)CurrentWeapon.Value.Weapon_Type == EWeaponType.Gun)
				returnBool = CurrentWeapon.Value.CanShoot();
			return returnBool;
		}

		/// <summary>
		/// This function is here to handle all things that need to be done when a battle entity attacks
		/// Examples include stealing stats, stealing money, subtracting ammo/charge count, other stat changes etc.
		/// THIS means the attacking entity NOT the target entity
		/// </summary>
		public override void Attack()
		{
			if ((EWeaponType)CurrentWeapon.Value.Weapon_Type == EWeaponType.Bow || (EWeaponType)CurrentWeapon.Value.Weapon_Type == EWeaponType.Gun )
			{
				// we should have already checked if we can attack if we are calling this. so let's attack!

				// First up subtract the ammo type.
				CurrentWeapon.Value.SubtractAmmo();

				// TODO: Check the weapons modifiers for stats based stuff. We do this in the state machine currently.
			}
			else if (CurrentWeapon.Value is MagicWeapon magicWeapon)
			{
				throw new NotImplementedException("Not Created anything to do with magic based weapons.");
			}
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

		/// <summary>
		/// Constructor for COMBAT
		/// </summary>
		/// <param name="BaseCombatUI"> The UI that we will add everything too. </param>
		/// <param name="CombatIcon"></param>
		/// <param name="healthBar"></param>
		/// <param name="manaBar"></param>
		public void LoadPartyMemberUIData(BaseUI BaseCombatUI, Texture2D CombatIcon, GameProgressBar healthBar,
			GameProgressBar manaBar)
		{
			//Create the UI
			if (CombatIcon != null)
			{
				//BaseCombatUI.UIElements.Add(CombatIcon);
				//CombatIcon.SetUITexture(); //set the 
				Icon = CombatIcon;
			}
			if (healthBar != null)
				BaseCombatUI.UIElements.Add(healthBar);
			if (manaBar != null)
				BaseCombatUI.UIElements.Add(manaBar);
			CombatStatsIndicator = BaseCombatUI;
		}

		private void DrawCombatStatsUI(GameTime gameTime ,SpriteBatch spriteBatch)
		{
			foreach (var uiElement in CombatStatsIndicator.UIElements)
			{
				switch (uiElement)
				{
					case GameButton gameButton:
						gameButton.Draw(gameTime, spriteBatch);
						break;
					case GameCheckBox gameCheckBox:
						gameCheckBox.Draw(gameTime, spriteBatch);
						break;
					case GameImage gameImage:
						gameImage.Draw(gameTime, spriteBatch);
						break;
					case GameCustomListBox gameCustomListBox:
						gameCustomListBox.Draw(gameTime, spriteBatch);
						break;
					case GameMultiSelectionListbox gameMultiSelectionListbox:
						gameMultiSelectionListbox.Draw(gameTime, spriteBatch);
						break;
					case GameListBoxItemSelectable gameListBoxItemSelectable:
						gameListBoxItemSelectable.Draw(gameTime, spriteBatch);
						break;
					case GameListBoxItem gameListBoxItem:
						gameListBoxItem.Draw(gameTime, spriteBatch);
						break;
					case GameCustomProgressBar gameCustomProgressBar:
						gameCustomProgressBar.Draw(gameTime, spriteBatch);
						break;
					case GameProgressBar gameProgressBar:
						gameProgressBar.Draw(gameTime, spriteBatch);
						break;
					case GameTabControl gameTabControl:
						gameTabControl.Draw(gameTime, spriteBatch);
						break;
					case GameTextBlock gameTextBlock:
						gameTextBlock.Draw(gameTime, spriteBatch);
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(uiElement));
				}
			}
		}

		public override void Draw(GameTime gameTime ,SpriteBatch spriteBatch)
		{
			base.Draw( gameTime ,spriteBatch);

			//CombatStatsIndicator?.Draw(gameTime, spriteBatch);
			DrawCombatStatsUI(gameTime, spriteBatch);

			if (StanceIndicator_Texture2D != null)
			{
				spriteBatch.Draw(StanceIndicator_Texture2D, StanceIndicator_Rectangle, StanceIndicator_Color);
			}
		}


		public Rectangle GetCharacterStatUILocation()
		{
			return (CombatStatsIndicator.UIElements[0] as GameImage).DrawRectangle;
		}

		public override string ToString()
		{
			return String.Format( "{0}: {1}",First_Name ,base.ToString());
		}
	}

	public class party_member : BattleEntity
	{
		#region Database Linking

		public String First_Name { get; set; }
		public String Last_Name { get; set; }

		public int Friendship_Points { get; set; }
		
		//Foreign Keys
		public int Main_Job_FK { get; set; }
		public int Sub_Job_FK { get; set; }

		//public BaseStats Stats = new BaseStats();
		//public WeakStrength WeaknessAndStrengths = new WeakStrength();

		//public List<Skill> Skills = new List<Skill>(6);
		//public List<Weapon> Weapons = new List<Weapon>(3);
		//public List<gamelb.Item> gamelb = new List<gamelb.Item>(6);


		#endregion
	}

}
