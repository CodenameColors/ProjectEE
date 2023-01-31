using System.Collections.Generic;
using BixBite.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Combat.CombatDelegates
{
	class FollowUpAttacksDelegates
	{

		#region Attack
		//Knight & Bandit == Immovable
		public static void Espionage_Followup(CombatSystem cbsref, List<object> paramList)
		{
			//Check to make sure all the params match
			List<PartyMember> battleEntity_instigators = paramList[0] as List<PartyMember>;
			List<Enemy> battleEntity_targets = paramList[1] as List<Enemy>;

			//if (battleEntity_instigator is null)
			//	battleEntity_instigator = paramList[0] as Enemy; //Check to make sure all the params match
			//if (battleEntity_instigator is null)
			//	throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "AttackUpS_Dele"));//something is missing.

			//if (battleEntity_target is null)
			//	battleEntity_target = paramList[1] as Enemy;//Check to make sure all the params match
			//if (battleEntity_target is null)
			//	throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "AttackUpS_Dele")); //something is missing.

			//For this modifier we need to set the target on fire [status effect] for X amount of turns
		//	battleEntity_target.StatusEffect_List.Add(new BaseStatChange()
		//	{
		//		bIsActive = false,
		//		LengthInTurns = 3,
		//		LinkedModifierData = currentModifier,
		//		StatToChange = new BaseStats() { Max_Health = (int)(battleEntity_target.Stats.Max_Health * .05f) }
		//	});
		}

		//Sniper & Gambler == PewPew
		public static void LuckyShot_Followup(CombatSystem cbsref, List<object> paramList)
		{
			//Check to make sure all the params match
			List<PartyMember> battleEntity_instigators = paramList[0] as List<PartyMember>;
			List<Enemy> battleEntity_targets = paramList[1] as List<Enemy>;

			if(battleEntity_targets == null || battleEntity_targets.Count == 0) return;

			//QueueMoveCharacter(CurrentPartyMember_turn, (int)((1820 * .45f) - (CurrentPartyMember_turn.Width / 2.0f)), (int)CurrentPartyMember_turn.Position.Y);

			//Move each character forward to show they are ready to team attack!
			List<CombatMoveAction> batchMoveRequest = new List<CombatMoveAction>();
			foreach (PartyMember pm in battleEntity_instigators)
			{
				batchMoveRequest.Add( new CombatMoveAction(cbsref, pm, (int)((1820 * .45f) - (pm.Width / 2.0f)), (int)pm.Position.Y, 18, false));
			}
			cbsref.QueueCombatAction(new CombatBatchMoveAction(cbsref, batchMoveRequest, true));
			cbsref.QueueCombatAction(new CombatDelayAction(cbsref, 200));

			batchMoveRequest.Clear();
			foreach (PartyMember pm in battleEntity_instigators)
			{
				//forward
				batchMoveRequest.Add(new CombatMoveAction(cbsref, pm, (int)((1820 * .45f) - (pm.Width / 2.0f)) + 75, (int)pm.Position.Y - 20, 10, false));
			}
			cbsref.QueueCombatAction(new CombatBatchMoveAction(cbsref, batchMoveRequest, true));

			//Move enemy back to simulate a hit
			Enemy enemy = battleEntity_targets[0];
			cbsref.QueueCombatAction(new CombatMoveAction(cbsref, (enemy), (int)enemy.SpawnPosition.X + 50, (int)enemy.SpawnPosition.Y - 50, 15, true));
			//tcbsref.QueueCombatAction(new CombatDelayAction(cbsref, 50));

			//Get the Hit spawn Position of the TARGETED enemy. This skill ONLY hits one.
			Vector2 TargetPosition = new Vector2(enemy.Position.X, enemy.Position.Y);
			TargetPosition.X += (float)((enemy.Width * enemy.ScaleX) / 2.0);
			TargetPosition.Y += (float)((enemy.Height * enemy.ScaleY) / 2.0);

			cbsref.QueueCombatAction(new CombatHitSparkAction(cbsref, cbsref.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle"),
					TargetPosition - new Vector2(50, 50), Color.Purple, false)
				{ scalarX = .50f, ScalarY = .50f });
			//Deal damage

			//QueueCombatAction(new CombatDamageAction(this, null, (CurrentPartyMember_turn), false, SkillToUse,
			//	new List<Enemy>() { (currentSelectedEnemy) }, new List<PartyMember>()));

			Skill skill = null;
			cbsref.QueueCombatAction(new CombatDamageAction(cbsref, null, battleEntity_instigators[0], false, skill, new List<Enemy>(){enemy}, new List<PartyMember>()  )  );

			//cbsref.QueueHitSparkParticleSystem(null, TargetPosition, Color.LimeGreen);

			batchMoveRequest.Clear();
			foreach (PartyMember pm in battleEntity_instigators)
			{
				//back
				batchMoveRequest.Add(new CombatMoveAction(cbsref, pm, (int)((1820 * .45f) - (pm.Width / 2.0f)), (int)pm.Position.Y, 10, false));
			}
			cbsref.QueueCombatAction(new CombatBatchMoveAction(cbsref, batchMoveRequest, true));


			//Move the enemy that was hit back to the original position
			cbsref.QueueCombatAction(new CombatMoveAction(cbsref, (enemy), (int)enemy.SpawnPosition.X, (int)enemy.SpawnPosition.Y, 10, false));

			//move the character back to their past position.
			batchMoveRequest.Clear();
			foreach (PartyMember pm in battleEntity_instigators)
			{
				batchMoveRequest.Add(new CombatMoveAction(cbsref, pm, (int)pm.SpawnPosition.X, (int)pm.SpawnPosition.Y, 22, false));
			}
			cbsref.QueueCombatAction(new CombatBatchMoveAction(cbsref, batchMoveRequest, true));


			//if (battleEntity_instigator is null)
			//	battleEntity_instigator = paramList[0] as Enemy; //Check to make sure all the params match
			//if (battleEntity_instigator is null)
			//	throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "AttackUpS_Dele"));//something is missing.

			//if (battleEntity_target is null)
			//	battleEntity_target = paramList[1] as Enemy;//Check to make sure all the params match
			//if (battleEntity_target is null)
			//	throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "AttackUpS_Dele")); //something is missing.

			//For this modifier we need to set the target on fire [status effect] for X amount of turns
			//	battleEntity_target.StatusEffect_List.Add(new BaseStatChange()
			//	{
			//		bIsActive = false,
			//		LengthInTurns = 3,
			//		LinkedModifierData = currentModifier,
			//		StatToChange = new BaseStats() { Max_Health = (int)(battleEntity_target.Stats.Max_Health * .05f) }
			//	});
		}


		#endregion


		#region Defense

		#endregion


		#region Buffs

		#endregion


		#region Debuffs

		#endregion

	}
}
