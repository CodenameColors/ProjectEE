using System;
using System.Collections.Generic;
using BixBite.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Combat.CombatDelegates
{
	public class SkillDelegates
	{

		#region Attack

		/// <summary>
		/// This method is here to apply effect to the screen when the player uses the Quick Arrow skill.
		/// </summary>
		/// <param name="cbsref">Holds a reference to the combat system instance. So we can use these methods.</param>
		/// <param name="paramList">
		/// <para>[0] = Requested Character</para>
		/// <para>[1] = List of Entities this skill will affect</para>
		/// <para>[2] = The skill object that is being used</para>
		/// </param>
		public static void QuickArrow_Skill( CombatSystem cbsref, List<object> paramList)
		{
			//Check to make sure all the params match
			PartyMember pm = paramList[0] as PartyMember;
			List<BattleEntity> affectedBattleEntitys = paramList[1] as List<BattleEntity>;
			Skill requestedSkill = paramList[2] as Skill;

			if (pm is null || affectedBattleEntitys is null || requestedSkill is null)
				//something is missing.
				throw new Exception(String.Format( "Casting of the params list failed for Skill: [{0}]", "Quick Arrow"));

			Enemy enemy = affectedBattleEntitys[0] as Enemy;
			if (enemy == null) throw new ArgumentNullException(nameof(enemy));

			//Move the character back to show they attacked something.																								
			cbsref.QueueCombatAction(new CombatMoveAction(cbsref, pm, (int)pm.Position.X - 50, (int)pm.Position.Y, 20, false));
			//Move enemy back to simulate a hit
			cbsref.QueueCombatAction(new CombatMoveAction(cbsref, (enemy), (int)enemy.SpawnPosition.X + 50, (int)enemy.SpawnPosition.Y - 10, 10, false));

			//Get the Hit spawn Position of the TARGETED enemy. This skill ONLY hits one.
			Vector2 TargetPosition = new Vector2(enemy.Position.X, enemy.Position.Y);
			TargetPosition.X += (float)((enemy.Width * enemy.ScaleX) / 2.0);
			TargetPosition.Y += (float)((enemy.Height * enemy.ScaleY) / 2.0);

			cbsref.QueueCombatAction(new CombatHitSparkAction(cbsref, cbsref.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle"),
				TargetPosition - new Vector2(50, 50), Color.Red, false) { scalarX = .25f, ScalarY = .25f });
			//Deal damage
			cbsref.QueueCombatAction(new CombatDamageAction(cbsref, null, pm, 
				false, requestedSkill, new List<Enemy>(){enemy}, new List<PartyMember>() ));

			//cbsref.QueueHitSparkParticleSystem(null, TargetPosition, Color.LimeGreen);

			//Move back to their OG spot
			cbsref.QueueCombatAction(new CombatMoveAction(cbsref, (enemy), (int)enemy.SpawnPosition.X, (int)enemy.SpawnPosition.Y,10, false));

			//move the character back to their past position.
			cbsref.QueueCombatAction(new CombatMoveAction(cbsref, (cbsref.CurrentTurnCharacter as PartyMember), (int)pm.SpawnPosition.X, (int)pm.SpawnPosition.Y, 10, true));

		}

		public static void ArrowRain_Skill(CombatSystem cbsref, List<object> paramList)
		{
			//Check to make sure all the params match
			PartyMember pm = paramList[0] as PartyMember;
			List<BattleEntity> affectedBattleEntitys = paramList[1] as List<BattleEntity>;
			Skill requestedSkill = paramList[2] as Skill;

			if (pm is null || affectedBattleEntitys is null || requestedSkill is null)
				//something is missing.
				throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "Quick Arrow"));

			//This skill hits multiple people.
			CombatBoxParticleSystemAction cbaction = null;
			foreach (BattleEntity bc in affectedBattleEntitys)
			{
				Enemy em = bc as Enemy;
				if (em is null) continue;

				//Move enemy back to simulate a hit
				cbsref.QueueCombatAction(new CombatMoveAction(cbsref, em, (int) em.SpawnPosition.X + 50, (int) em.SpawnPosition.Y - 10, 10, false));

				cbsref.QueueCombatAction(new CombatDelayAction(cbsref, 100));

				//Move enemy back to spawn position
				cbsref.QueueCombatAction(new CombatMoveAction(cbsref, em, (int) em.SpawnPosition.X, (int) em.SpawnPosition.Y, 10, false ));

				//Get the Hit spawn Position of the TARGETED enemy. This skill can HIT MORE THAN ONE.
				//So get the middle of that sprite first.
				Vector2 TargetPosition = new Vector2(em.Position.X, em.Position.Y);
				TargetPosition.X += (float) ((em.Width * em.ScaleX) / 2.0);
				TargetPosition.Y += (float) ((em.Height * em.ScaleY) / 2.0);

				int TESTint = 1;

				//Load the texture from content.
				Texture2D particleImg = cbsref.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle");

				Rectangle emibounds = cbsref.SelectArrowAreaRange_Rect;

				emibounds.Height = cbsref.SelectArrowAreaRange_Rect.Bottom + 50;
				emibounds.Y = -100;
			
				if (affectedBattleEntitys[0] == em)
				{
					Texture2D t = cbsref.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Arrow_white");

					cbaction = new CombatBoxParticleSystemAction(cbsref,
						new Rectangle(cbsref.SelectArrowAreaRange_Rect.X, 0, cbsref.SelectArrowAreaRange_Rect.Width, 10), 1,
						new Rectangle(0, 0, 1920, cbsref.SelectArrowAreaRange_Rect.Bottom - 100),
						new Rectangle(0, 0, t.Width, t.Height), 200,
						1, 2, true,
						t, 1f,
						.5f, .5f,
						false,
						1000f, 2,
						85, 95,
						800f, 1000f,
						1200, 1600,
						new Random(), TargetPosition - new Vector2(50, 50), Color.Blue);

					cbsref.QueueCombatAction(cbaction);

					cbsref.QueueCombatAction(new CombatAnimationAction(cbsref, pm, "Skill_Right"));
					cbsref.QueueCombatAction(new CombatDelayAction(cbsref, 1250)); //wait before setting hit events
				}
				cbsref.QueueCombatAction(new CombatDamageAction(cbsref, null, pm, false, requestedSkill, new List<Enemy>() {em}, new List<PartyMember>()));
				cbsref.QueueCombatAction(new CombatHitSparkAction(cbsref, particleImg, TargetPosition - new Vector2(50, 50), Color.Red, false) {scalarX = .25f, ScalarY = .25f});
				cbsref.QueueCombatAction(new CombatMoveAction(cbsref, em, (int)em.SpawnPosition.X, (int)em.SpawnPosition.Y, 10, false));
				
			}
			cbsref.QueueCombatAction(new CombatParticleCyclingAction(cbsref, cbaction.newEmittier));
			cbsref.QueueCombatAction(new CombatAnimationAction(cbsref, pm, "Idle_Right"));
			cbsref.QueueCombatAction(new CombatMoveAction(cbsref, pm, (int)pm.SpawnPosition.X, (int)pm.SpawnPosition.Y, 10, true));
		}

		#endregion

		#region Defense

		#endregion

		#region Buffs

		public static void SpeedUp_Skill(CombatSystem cbsref, List<object> paramList)
		{
			//Check to make sure all the params match
			PartyMember pm = paramList[0] as PartyMember;
			List<BattleEntity> affectedBattleEntitys = paramList[1] as List<BattleEntity>;
			Skill requestedSkill = paramList[2] as Skill;

			if (pm is null || affectedBattleEntitys is null || requestedSkill is null)
				//something is missing.
				throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "Quick Arrow"));

			PartyMember affectedBattleEntity = affectedBattleEntitys[0] as PartyMember;
			if (affectedBattleEntity == null) throw new ArgumentNullException(nameof(affectedBattleEntity));

			//Get the Hit spawn Position of the TARGETED enemy. This skill ONLY hits one.
			Vector2 TargetPosition = new Vector2(affectedBattleEntity.Position.X, affectedBattleEntity.Position.Y);
			TargetPosition.X += (float)((affectedBattleEntity.Width * affectedBattleEntity.ScaleX) / 2.0);
			TargetPosition.Y += (float)((affectedBattleEntity.Height * affectedBattleEntity.ScaleY));

			cbsref.QueueCombatAction((new CombatStatChangeAction(cbsref, new BaseStats(){Agility = 25}, true, 3,
				new List<Enemy>(), new List<PartyMember>() {affectedBattleEntity} )));

			Texture2D particleImg = cbsref.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle");

			CombatParticleSystemAction temParticleSystemAction = new CombatParticleSystemAction(cbsref,
					new Rectangle(0, 0, 1920, 1080),
					new Rectangle(0, 0, particleImg.Width, particleImg.Height), 20,
					200, 2, true,
					particleImg, 1f,
					.25f, .25f,
					false,
					-50f, 100,
					255, 285,
					200, 400,
					500, 1000,
					new Random(), TargetPosition - new Vector2(50, 50), Color.Green)
				{scalarX = .5f, ScalarY = .5f};

			cbsref.QueueCombatAction(temParticleSystemAction);

			cbsref.QueueCombatAction(new CombatDelayAction(cbsref, 1000)); //wait before setting hit events
			cbsref.QueueCombatAction(new CombatParticleCyclingAction(cbsref, temParticleSystemAction.newEmittier));
			cbsref.QueueCombatAction(new CombatStatChangeAction(cbsref, new BaseStats(){Agility = 10}, false, 3, new List<Enemy>(), new List<PartyMember>(){affectedBattleEntity} ));

			//cbsref.QueueCombatAction(new CombatHitSparkAction(cbsref, cbsref.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle"),
			//		TargetPosition - new Vector2(50, 50), Color.Green)
			//	{ scalarX = .25f, ScalarY = .25f });

			//move the character back to their past position.
			cbsref.QueueCombatAction(new CombatMoveAction(cbsref, (cbsref.CurrentTurnCharacter as PartyMember), (int)pm.SpawnPosition.X, (int)pm.SpawnPosition.Y, 10, true));

		}

		#endregion

		#region Debuffs

		#endregion



	}
}
