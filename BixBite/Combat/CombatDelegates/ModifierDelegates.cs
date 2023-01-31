using System;
using System.Collections.Generic;
using System.Linq;
using BixBite.Characters;

namespace BixBite.Combat.CombatDelegates
{
	public class ModifierDelegates
	{


		#region Attack
		public static void SetFire_Dele(CombatSystem cbsref, List<object> paramList)
		{
			//Check to make sure all the params match
			BattleEntity battleEntity_instigator = paramList[0] as PartyMember;
			BattleEntity battleEntity_target = paramList[1] as PartyMember;
			ModifierData currentModifier = paramList[2] as ModifierData;

			if (battleEntity_instigator is null)
				battleEntity_instigator = paramList[0] as Enemy; //Check to make sure all the params match
			if (battleEntity_instigator is null)
				throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "AttackUpS_Dele"));//something is missing.

			if (battleEntity_target is null)
				battleEntity_target = paramList[1] as Enemy;//Check to make sure all the params match
			if (battleEntity_target is null)
				throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "AttackUpS_Dele")); //something is missing.

			//For this modifier we need to set the target on fire [status effect] for X amount of turns
			battleEntity_target.StatusEffect_List.Add(new BaseStatChange()
			{
				bIsActive = false, LengthInTurns = 3, LinkedModifierData = currentModifier, StatToChange = new BaseStats(){Max_Health = (int)(battleEntity_target.Stats.Max_Health * .05f) }
			});
		}
		#endregion


		#region Defense

		#endregion


		#region Buffs
		public static void AttackUpS_Dele(CombatSystem cbsref, List<object> paramList)
		{
			//Check to make sure all the params match
			BattleEntity battleEntity = paramList[0] as PartyMember;
			ModifierData currentModifier = paramList[1] as ModifierData;

			if (battleEntity is null)
			{
				//Check to make sure all the params match
				battleEntity = paramList[0] as Enemy;
			}
			if (battleEntity is null)
			{
				//something is missing.
				throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "AttackUpS_Dele"));
			}

			battleEntity.StatChange_List.Add(new BaseStatChange()
			{
				bIsActive = false, LengthInTurns = 0, LinkedModifierData = currentModifier,
				StatToChange = new BaseStats()
				{
					Attack = (int) Math.Ceiling(battleEntity.Stats.Attack * .1f)
				}
			});
			cbsref.ApplyStatChange(battleEntity, battleEntity.StatChange_List.Last());
		}


		public static void SpeedUpPlus_Dele(CombatSystem cbsref, List<object> paramList)
		{
			//Check to make sure all the params match
			BattleEntity battleEntity = paramList[0] as PartyMember;
			ModifierData currentModifier = paramList[1] as ModifierData;

			if (battleEntity is null)
			{
				//Check to make sure all the params match
				battleEntity = paramList[0] as Enemy;
			}
			if (battleEntity is null)
			{
				//something is missing.
				throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "AttackUpS_Dele"));
			}

			battleEntity.StatChange_List.Add(new BaseStatChange()
			{
				bIsActive = false,
				LengthInTurns = 0,
				LinkedModifierData = currentModifier,
				StatToChange = new BaseStats()
				{
					Attack = (int)Math.Ceiling(battleEntity.Stats.Agility * .1f)
				}
			});
			cbsref.ApplyStatChange(battleEntity, battleEntity.StatChange_List.Last());
		}
		#endregion


		#region Debuffs

		#endregion


	}
}
