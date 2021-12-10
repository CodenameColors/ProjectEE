using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{

	public enum EModifierUseType
	{
		NONE = 0,
		ATTACK = 1,
		DEFENSE = 2,
		BUFF = 3,
		DEBUFF = 4,
		WILDCARD = 5,
	}

	#region Enums
	
	public enum ERarityType
	{
		NONE = 0,
		Common = 1,
		Uncommon = 2,
		Rare = 3,
		UltraRare = 4,
		Legendary = 5,
		WTF = 6,
	}
	public enum EChanceEffectModifiers
	{
		NONE = 0,
		prayHarder = 1,
		Rare = 2,
		KindaOften = 4,
		Possible = 8,
		Common = 16,
		CoinFlip = 32, //50%
		Always = 64, 
	}

	public enum ESeverityEffectModifiers
	{
		NONE = 0,
		Tiny = 1,
		Small = 2,
		Medium = 4,
		Large = 8,
		Whale = 16,
		Rediculous = 32,
	}

	public enum ETurnEffectModifiers
	{
		NONE = 0,
		OneTurnEffect = 1, //EFFECT ONLY
		TwoTurnEffect = 2, //EFFECT ONLY
		ThreeTurnEffect = 4, //EFFECT ONLY 
		RestOfBattleEffect = 8,
		TimeDependent = 16,
		GoAgain = 32,// EFFECT ONLY
	}

	public enum EDamageModifiers
	{
		NONE = 0,
		PhyDamageModifier = 1,
		MagicDamageModifier = 2,
		RangedDamageModifier = 4,
		SummonDamageModifier = 8,
	}

	public enum EMagicDamageModifiers
	{
		NONE = 0,
		FireDamageModifier = 1,
		IceDamageModifier = 2,
		EarthDamageModifier = 4,
		WaterDamageModifier = 8,
		LightiningDamageModifier = 16,
		ExplosiveDamageModifier = 32,
	}

	public enum EMagicDefenseModifiers
	{
		NONE = 0,
		Null_FireDamage = 1,
		Null_IceDamage = 2,
		Null_EarthDamage = 4,
		Null_WaterDamage = 8,
		Null_LightiningDamage = 16,
		Null_ExplosiveDamage = 32,
		AbsorbFireDamage = 64,
		AbsorbIceDamage = 128,
		AbsorbEarthDamage = 256,
		AbsorbWaterDamage = 512,
		AbsorbLightiningDamage = 1024,
		AbsorbExplosiveDamage = 2048,
	}

	public enum EStatEffectModifiers
	{
		NONE = 0,
		HPRecovery = 1,
		MPRecovery = 2,
		EPRecovery = 4,
		AttackModifier = 8,
		DefenseModifier = 16,
		DexterityModifier = 32,
		MoralityModifier = 64,
		WisdomModifier = 128,
		ResistanceModifier = 256,
		LuckModifier = 512,
		RiskModifier = 1024,
		AgilityModifier = 2048
	}

	public enum EStatusEffectModifiers
	{
		NONE = 0,
		Burning = 1,
		Shocked = 2,
		Tired = 4,
		Depressed = 8,
		Angry = 16,
		Lusting = 32,
		Greedy = 64,
		Bloodthirsty = 128,
		ManaThirsty = 256,
		Clumsy = 512,
		Emo = 1024,
		TriggerHappy = 2048,
		Weaken = 4096,
		Curse = 8192,
		Bramble = 16384,
	}

	public enum ENullifyStatusEffectModifiers
	{
		NONE = 0,
		Null_Burning = 1,
		Null_Shocked = 2,
		Null_Tired = 4,
		Null_Depressed = 8,
		Null_Angry = 16,
		Null_Lusting = 32,
		Null_Greedy = 64,
		Null_Bloodthirsty = 128,
		Null_ManaThirsty = 256,
		Null_Clumsy = 512,
		Null_Emo = 1024,
		Null_TriggerHappy = 2048,
		Null_Weaken = 4096,
		Null_Curse = 8192,
		Null_Bramble = 16384,
	}

	public enum ENonBattleEffectModifiers
	{
		NONE = 0,
		BetterScavenger = 1,
		BetterLooter = 2,
		Spacious = 4,
		PersonalityTypesModifier = 8,
	}

	public enum ESpecialEffectModifiers
	{
		NONE = 0,
		AutoGetup = 1, //ONCE EFFECT ONLY
		AlwaysEscape = 2,
		AoEUp = 4,
		AutoRevive = 8,
		OnDeathActivate = 16,
		OnLowHealthActivate = 32,
		StealStat = 64,
		DualWield = 128,
		DoubleShot = 256,
		MultiShot = 512,
		ChainShot = 1024,
		EssenceUp = 2048,
		EssenceDropUp = 4096,
		ManaConsumptionModifier = 8192,
		EssenceConsumptionModifier = 16384,
		HeathConsumptionModifier = 32768,
		PersonalityTypesModifier = 65536,
		DodgeDeath = 131072,
	}

	#endregion

	//Some of these are ints because of bit wise enumeration
	public class ModifierData	
	{
		public String Id { get; set; }
		public int? Chance_Modifiers { get; set; }
		public int? Severity_Modifiers { get; set; }
		public int? Turn_Modifiers { get; set; }
		public int? Damage_Modifiers { get; set; }
		public int? Magic_Damage_Modifiers { get; set; }
		public int? Magic_Defense_Modifiers { get; set; }
		public int? Stat_Modifiers { get; set; }
		public int? Status_Effect_Modifiers { get; set; }
		public int? Nullify_Status_Effect_Modifiers { get; set; }
		public int? NonBattle_Modifiers { get; set; }
		public int? Special_Modifiers { get; set; }
		public String Function_PTR { get; set; }
		public bool bEffect { get; set; }

		private EModifierUseType _modifierusetype = EModifierUseType.NONE;
		public EModifierUseType Use_Type
		{
			get => _modifierusetype;
			set
			{
				if (value is int)
				{
					_modifierusetype = (EModifierUseType) value;
				}
				else
				{
					_modifierusetype = value;
				}
			}
		}

		public String Skills_FK { get; set; }

	}

	public class GamePlayTrait :GameplayModifier
	{
		public override ModifierData ModifierData { get; set; }

		public GamePlayTrait()
		{
			
		}

		
	}

	public class GamePlayEffect : GameplayModifier
	{
		public override ModifierData ModifierData { get; set; }
	}

	public class GameplayModifier
	{
		public virtual ModifierData ModifierData { get; set; }
	}
	
}
