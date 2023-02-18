using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BixBite.Combat.Equipables.Weapons
{

	public enum EWeaponType
	{
		NONE = 0,
		OneHandSword = 1,
		TwoHandSword = 2,
		Shield = 4,
		// Axe = 8,
		Bow = 16,
		Spear = 32,
		Staff = 64,
		Scythe = 128,
		// Rapier = 256,
		Gauntlets = 512,
		Dagger = 1024,
		Wand = 2048,
		Gun = 4096,
		Explosive = 8192,
	}

	public class Weapon : Equipable, IRangedWeapons
	{
		#region Database Linking
		public int? Elemental { get; set; }
		public int Weapon_Type { get; set; }

		public List<Skill> WeaponSkills = new List<Skill>();
		public String Function_PTR { get; set; }
		#endregion

		#region Ranged Weapons


		#region properties/fields

		/// <summary>
		/// This is the value that is set by all the modifiers, and traits to modify the base max ammo amount
		/// Needs to be a list to allow for the stacking of multiple modifiers
		/// </summary>
		private List<AmmoStatChange> _listMaxAmmoModiferOperands = new List<AmmoStatChange>();

		/// <summary>
		/// This is the value that is set by all the modifiers, and traits to modify the base max ammo amount
		/// Needs to be a list to allow for the stacking of multiple modifiers
		/// </summary>
		private List<AmmoStatChange> _listBulletsPerUseModiferOperands = new List<AmmoStatChange>();

		/// <summary>
		/// This is here as the FINAL operand. Calculated using "Update maxAmmo Operand.
		/// </summary>
		private int _maxAmmoModiferOperand = 0;

		/// <summary>
		/// This is here as the FINAL operand. Calculated using "UpdateBulletsPerUseOperand".
		/// </summary>
		private int _bulletPerUseModiferOperand = 0;


		private int _maxAmmo = 6;
		public int MaxAmmo
		{
			get => _maxAmmo + _maxAmmoModiferOperand;
			set => _maxAmmo = value;
		}

		public int CurrentAmmo { get; set; }

		private int _bulletUsagePerUse = 1;
		public int BulletUsagePer
		{
			get => _bulletUsagePerUse + _bulletPerUseModiferOperand;
			set => _bulletUsagePerUse = value;

		}

		#endregion

		#region Methods

		public void UpdateMaxAmmoOperand()
		{
			// Allow for a stacking check
			int temp = 0;
			foreach (AmmoStatChange operand in _listMaxAmmoModiferOperands)
			{
				temp += operand.StatToChange.Max_Ammo;
			}
			_maxAmmoModiferOperand = temp;
		}

		public void UpdateBulletsPerUseOperand()
		{
			// Allow for a stacking check
			int temp = 0;
			foreach (AmmoStatChange operand in _listBulletsPerUseModiferOperands)
			{
				temp += operand.StatToChange.Bullets_Per_Use;
			}
			_bulletPerUseModiferOperand = temp;
		}

		/// <summary>
		/// Subtract one from the turn counter. Remove the modifier if this operation would make this  <= 0;
		/// </summary>
		public void UpdateModifierOperandTurnCounters()
		{
			for (int i = _listBulletsPerUseModiferOperands.Count - 1; i >= 0; i--)
			{
				AmmoStatChange ammoStatChange = _listBulletsPerUseModiferOperands[i];
				if (ammoStatChange.LengthInTurns - 1 <= 0)
					_listBulletsPerUseModiferOperands.Remove(ammoStatChange);
				else ammoStatChange.DecrementTurnCounter();
			}

			for (int i = _listMaxAmmoModiferOperands.Count - 1; i >= 0; i--)
			{
				AmmoStatChange ammoStatChange = _listMaxAmmoModiferOperands[i];
				if (ammoStatChange.LengthInTurns - 1 <= 0)
					_listMaxAmmoModiferOperands.Remove(ammoStatChange);
				else ammoStatChange.DecrementTurnCounter();
			}
		}

		public void Reload()
		{
			CurrentAmmo = MaxAmmo;
		}

		public bool CanShoot()
		{
			if (CurrentAmmo - BulletUsagePer < 0)
				return false;
			return true;
		}

		///
		public void AddMaxAmmoModiferOperand(AmmoStatChange ammoStatChange)
		{
			_listMaxAmmoModiferOperands.Add(ammoStatChange);
		}

		public void SubtractAmmo()
		{
			if (CurrentAmmo - BulletUsagePer < 0)
				CurrentAmmo = 0;
			else CurrentAmmo -= BulletUsagePer;
		}

		#endregion

		#endregion

	}

	public class Weapons : Weapon
	{

	}

	public class Created_Weapon : Created_Equipable
	{

	}

}
