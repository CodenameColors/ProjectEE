using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat.Equipables.Weapons
{
	public interface IRangedWeapons
	{
		void UpdateMaxAmmoOperand();

		void UpdateBulletsPerUseOperand();

		/// <summary>
		/// Subtract one from the turn counter. Remove the modifier if this operation would make this  <= 0;
		/// </summary>
		void UpdateModifierOperandTurnCounters();

		void Reload(); 

		bool CanShoot();

		void AddMaxAmmoModiferOperand(AmmoStatChange ammoStatChange);

		void SubtractAmmo();
	}


	public class RangedWeapons : Weapon
	{
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


		private int _maxAmmo = 0;
		public int MaxAmmo
		{
			get => _maxAmmo + _maxAmmoModiferOperand;
			set => _maxAmmo = value;
		}

		public int CurrentAmmo { get; set; }

		private int _bulletUsagePerUse = 0;
		public int BulletUsagePer
		{
			get => _bulletUsagePerUse + _bulletPerUseModiferOperand;
			set => _bulletUsagePerUse = value;

		}

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
			for (int i = _listBulletsPerUseModiferOperands.Count-1; i >= 0; i--)
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
			if (BulletUsagePer - CurrentAmmo < 0)
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
			if (BulletUsagePer - CurrentAmmo < 0)
				CurrentAmmo = 0;
			else CurrentAmmo -= BulletUsagePer;
		}

	}
}
