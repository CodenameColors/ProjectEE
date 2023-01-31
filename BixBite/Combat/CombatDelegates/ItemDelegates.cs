using System;
using System.Collections.Generic;
using BixBite.Characters;
using BixBite.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Combat.CombatDelegates
{
	public class ItemDelegates
	{

		#region Attack

		public static void BigBoom_Item(CombatSystem cbsref, List<object> paramList)
		{
			//Check to make sure all the params match
			PartyMember pm = paramList[0] as PartyMember;
			List<BattleEntity> affectedBattleEntitys = paramList[1] as List<BattleEntity>;
			Item requestedItem = paramList[2] as Item;

			if (pm is null || affectedBattleEntitys is null || requestedItem is null)
				//something is missing.
				throw new Exception(String.Format("Casting of the params list failed for Skill: [{0}]", "Quick Arrow"));

			Enemy enemy = affectedBattleEntitys[0] as Enemy;
			if (enemy == null) throw new ArgumentNullException(nameof(enemy));



			//This skill hits multiple people.
			foreach (BattleEntity bc in affectedBattleEntitys)
			{
				Enemy em = bc as Enemy;
				if (em is null) continue;

				//Move enemy back to simulate a hit
				cbsref.QueueCombatAction(new CombatMoveAction(cbsref, em, (int)em.SpawnPosition.X + 50, (int)em.SpawnPosition.Y - 10, 10, false));

				//cbsref.QueueCombatAction(new CombatDelayAction(cbsref, 100));

				//Move enemy back to spawn position
				cbsref.QueueCombatAction(new CombatMoveAction(cbsref, em, (int)em.SpawnPosition.X, (int)em.SpawnPosition.Y, 10, false));

				//Get the Hit spawn Position of the TARGETED enemy. This skill can HIT MORE THAN ONE.
				//So get the middle of that sprite first.
				Vector2 TargetPosition = new Vector2(em.Position.X, em.Position.Y);
				TargetPosition.X += (float)((em.Width * em.ScaleX) / 2.0);
				TargetPosition.Y += (float)((em.Height * em.ScaleY) / 2.0);

				int TESTint = 1;

				//Load the texture from content.
				Texture2D particleImg = cbsref.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle");

				Rectangle emibounds = cbsref.SelectArrowAreaRange_Rect;

				emibounds.Height = cbsref.SelectArrowAreaRange_Rect.Bottom + 50;
				emibounds.Y = -100;
				if (affectedBattleEntitys[0] == em)
				{
					//Texture2D t = cbsref.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Arrow_white");
					//cbsref.QueueCombatAction(new CombatBoxParticleSystemAction(cbsref,
					//	new Rectangle(cbsref.SelectArrowAreaRange_Rect.X, 0, cbsref.SelectArrowAreaRange_Rect.Width, 10), 1,
					//	new Rectangle(0, 0, 1920, cbsref.SelectArrowAreaRange_Rect.Bottom - 100),
					//	new Rectangle(0, 0, t.Width, t.Height), 200,
					//	1, 2, false,
					//	t, 1f,
					//	.5f, .5f,
					//	false,
					//	1000f, 2,
					//	85, 95,
					//	800f, 1000f,
					//	1200, 1600,
					//	new Random(), TargetPosition - new Vector2(50, 50), Color.Blue));

					cbsref.QueueCombatAction(new CombatAnimationAction(cbsref, pm, "Skill_Right"));
					cbsref.QueueCombatAction(new CombatDelayAction(cbsref, 1000)); //wait before setting hit events
				}
				cbsref.QueueCombatAction(new CombatDamageAction(cbsref, null, pm, false, requestedItem, new List<Enemy>() { em }, new List<PartyMember>()));
				cbsref.QueueCombatAction(new CombatHitSparkAction(cbsref, particleImg, TargetPosition - new Vector2(50, 50), Color.Red, false) { scalarX = .25f, ScalarY = .25f });
				cbsref.QueueCombatAction(new CombatMoveAction(cbsref, em, (int)em.SpawnPosition.X, (int)em.SpawnPosition.Y, 10, false));

			}

			//move the character back to their past position.
			cbsref.QueueCombatAction(new CombatAnimationAction(cbsref, pm, "Idle_Right"));
			cbsref.QueueCombatAction(new CombatMoveAction(cbsref, (cbsref.CurrentTurnCharacter as PartyMember), (int)pm.SpawnPosition.X, (int)pm.SpawnPosition.Y, 10, true));

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
