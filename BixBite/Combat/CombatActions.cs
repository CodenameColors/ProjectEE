using System;
using System.Collections.Generic;
using BixBite.Characters;
using BixBite.Items;
using BixBite.Particles;
using BixBite.Rendering.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BixBite.Combat
{

	#region Movement

	public class CombatBatchMoveAction : CombatActions
	{
		public List<CombatMoveAction> BatchMoveActions = new List<CombatMoveAction>();
		public bool bQueuePriority;
		public CombatBatchMoveAction(CombatSystem cbs, List<CombatMoveAction> batchMoveActions, bool queuePriority) : base(cbs)
		{
			this.BatchMoveActions = new List<CombatMoveAction>(batchMoveActions); //create a local copy!
			this.bQueuePriority = queuePriority;
		}
	}

	public class CombatMoveAction : CombatActions
	{
		//The character to move, and the x, and y, interpolation per tick
		public BattleEntity CharacterToMove;
		public int? newXPos;
		public int? newYPos;
		public int? Delay_m = null;
		public int InterpolationPerTick;
		public bool bQueuePriority;

		/// <summary>
		/// Creates an MoveAction Request. This request is handled next frame
		/// </summary>
		/// <param name="cbs">Reference to the Combat System singleton instance</param>
		/// <param name="bc">A PartyMember or Enemy to move</param>
		/// <param name="x">Desire Position in the X cords</param>
		/// <param name="y">Desired Position in the Y cords</param>
		/// <param name="interpolation">The amount to move the entity every frame toward its final destination</param>
		/// <param name="queuePriority">Determines where to pause state machine execution. True means game will wait till this move done before allowing other events/input</param>
		public CombatMoveAction(CombatSystem cbs, BattleEntity bc, int? x, int? y, int interpolation, bool queuePriority) : base(cbs)
		{
			this.CharacterToMove = bc;
			this.newXPos = x;
			this.newYPos = y;
			this.InterpolationPerTick = interpolation;
			this.bQueuePriority = queuePriority;
		}

		/// <summary>
		/// Creates an MoveAction Request. This request is handled next frame
		/// </summary>
		/// <param name="cbs">Reference to the Combat System singleton instance</param>
		/// <param name="bc">A PartyMember or Enemy to move</param>
		/// <param name="x">Desire Position in the X cords</param>
		/// <param name="y">Desired Position in the Y cords</param>
		/// <param name="interpolation">The amount to move the entity every frame toward its final destination</param>
		/// <param name="queuePriority">Determines where to pause state machine execution. True means game will wait till this move done before allowing other events/input</param>
		/// <param name="delay_m">How long in milliseconds should we delay this move action for? [MAINLY good for batch moves]</param>
		public CombatMoveAction(CombatSystem cbs, BattleEntity bc, int? x, int? y, int interpolation, bool queuePriority, int delay_m) : base(cbs)
		{
			this.CharacterToMove = bc;
			this.newXPos = x;
			this.newYPos = y;
			this.InterpolationPerTick = interpolation;
			this.bQueuePriority = queuePriority;
			this.Delay_m = delay_m;
		}
	}
	#endregion

	//This region needs to have AT LEAST 3 events.
	// 1.] Spawn UI
	// 2.] Move UI
	// 3.] Kill/Despawn UI
	#region UI

	public class CombatSpawnUIAction : CombatGameUIAction
	{
		public CombatSpawnUIAction(CombatSystem cbsRef, BaseUI desiredBaseUI, int startX, int StartY) : base(cbsRef, desiredBaseUI)
		{
			this.DesiredUI.XPos = startX;
			this.DesiredUI.YPos = StartY;
			this.DesiredUI.bIsActive = true;
		}
	}

	public class CombatBatchMoveUIAction : CombatActions
	{
		public List<CombatMoveUIAction> BatchMoveUIActions;
		public bool bQUeuePriority { get; set; }

		public CombatBatchMoveUIAction(CombatSystem cbsRef, List<CombatMoveUIAction> requestedBatchMove, bool priority) : base(cbsRef)
		{
			this.BatchMoveUIActions = new System.Collections.Generic.List<CombatMoveUIAction>(requestedBatchMove);
			this.bQUeuePriority = priority;
		}
	}

	public class CombatMoveUIAction : CombatGameUIAction
	{
		public int? XPos = 0;
		public int? YPos = 0;
		public int Interpolation = 0;
		public String UIName;
		public bool bQueuePriority;

		public CombatMoveUIAction(CombatSystem cbsRef, BaseUI desiredBaseUI, int? xPos, int? yPos, int interpolation, bool priority, String uiName = "") : base(cbsRef, desiredBaseUI)
		{
			this.XPos = xPos;
			this.YPos = yPos;
			this.Interpolation = interpolation;
			this.UIName = uiName;
			this.bQueuePriority = priority;
		}

	}

	public class CombatKillUIAction : CombatGameUIAction
	{
		public String UIName;
		public CombatKillUIAction(CombatSystem cbsRef, BaseUI desiredBaseUI, String uiName = "") : base(cbsRef, desiredBaseUI)
		{
			this.UIName = uiName;
		}
	}

	public class CombatGameUIAction : CombatActions
	{

		public BaseUI DesiredUI { get; set; }


		public CombatGameUIAction(CombatSystem cbsRef, BaseUI desiredBaseUI) : base(cbsRef)
		{
			this.DesiredUI = desiredBaseUI;

		}
	}

	#endregion

	#region Damage
	public class CombatDamageAction : CombatStatChangeAction
	{
		public Dictionary<BattleEntity, float> DamageToApply_Dict = new Dictionary<BattleEntity, float>();

		public CombatDamageAction(CombatSystem cbsRef, BaseStats statChangeRequest, PartyMember Attacker, bool bIsPercent, Item requestedItem, List<Enemy> enemiesToHit, List<PartyMember> partyMembersToHit, int turnCount = 0)
			: base(cbsRef, statChangeRequest, bIsPercent, turnCount, enemiesToHit, partyMembersToHit)
		{
			//we need to apply this to the enemies
			foreach (Enemy enemy in enemiesToHit)
			{
				float damageAmount = 0;

				//Risk and Luck? which is bigger?

				//Calculate for lucky crit if luck > risk

				//calculate for risky crit if risk > luck

				//what range of numbers are allowed?

				//calculate how much damage we will do.
				if (requestedItem != null)
					damageAmount = (float)(Attacker.Stats.Attack * requestedItem.Stats.Attack);
				else
					damageAmount = (float)Attacker.Stats.Attack;
				damageAmount -= enemy.Stats.Defense;
				if (damageAmount < 0) damageAmount = 0; //Don't let them heal from being hit 
				DamageToApply_Dict.Add(enemy, damageAmount);
			}

			//We need to apply this to the party members


		}

		public CombatDamageAction(CombatSystem cbsRef, BaseStats statChangeRequest, PartyMember Attacker, bool bIsPercent, Skill requestedSkill, List<Enemy> enemiesToHit, List<PartyMember> partyMembersToHit, int turnCount = 0)
			: base(cbsRef, statChangeRequest, bIsPercent, turnCount, enemiesToHit, partyMembersToHit)
		{
			//we need to apply this to the enemies
			foreach (Enemy enemy in enemiesToHit)
			{
				float damageAmount = 0;

				//Risk and Luck? which is bigger?

				//Calculate for lucky crit if luck > risk

				//calculate for risky crit if risk > luck

				//what range of numbers are allowed?

				//calculate how much damage we will do.
				if (requestedSkill != null)
					damageAmount = (float)(Attacker.Stats.Attack * requestedSkill.Damage_Multiplier);
				else
					damageAmount = (float)Attacker.Stats.Attack;
				damageAmount -= enemy.Stats.Defense;
				if (damageAmount < 0) damageAmount = 0; //Don't let them heal from being hit 
				DamageToApply_Dict.Add(enemy, damageAmount);
			}

			//We need to apply this to the party members


		}

		public CombatDamageAction(CombatSystem cbsRef, BaseStats statChangeRequest, Enemy Attacker, bool bIsPercent, Skill requestedSkill, List<Enemy> enemiesToHit, List<PartyMember> partyMembersToHit, int turnCount = 0)
			: base(cbsRef, statChangeRequest, bIsPercent, turnCount, enemiesToHit, partyMembersToHit)
		{
			//we need to apply this to the enemies
			foreach (Enemy enemy in enemiesToHit)
			{
				float damageAmount = 0;

				//Risk and Luck? which is bigger?

				//Calculate for lucky crit if luck > risk

				//calculate for risky crit if risk > luck

				//what range of numbers are allowed?

				//calculate how much damage we will do.
				if (requestedSkill != null)
					damageAmount = (float)(Attacker.Stats.Attack * requestedSkill.Damage_Multiplier);
				else
					damageAmount = (float)Attacker.Stats.Attack;
				damageAmount -= enemy.Stats.Defense;
				if (damageAmount < 0) damageAmount = 0; //Don't let them heal from being hit 
				DamageToApply_Dict.Add(enemy, damageAmount);
			}

			//We need to apply this to the party members
			foreach (PartyMember partyMember in partyMembersToHit)
			{
				float damageAmount = 0;

				//Risk and Luck? which is bigger?

				//Calculate for lucky crit if luck > risk

				//calculate for risky crit if risk > luck

				//what range of numbers are allowed?

				//calculate how much damage we will do.
				if (requestedSkill != null)
					damageAmount = (float)(Attacker.Stats.Attack * requestedSkill.Damage_Multiplier);
				else
					damageAmount = (float)Attacker.Stats.Attack;

				damageAmount -= partyMember.Stats.Defense;
				if (damageAmount < 0) damageAmount = 0; //Don't let them heal from being hit 
				DamageToApply_Dict.Add(partyMember, damageAmount);
			}

		}


		public CombatDamageAction(CombatSystem cbsRef, BaseStats statChangeRequest, Enemy Attacker, bool bIsPercent, List<Enemy> enemiesToHit, List<PartyMember> partyMembersToHit, int turnCount)
		: base(cbsRef, statChangeRequest, bIsPercent, turnCount, enemiesToHit, partyMembersToHit)
		{
			//we need to apply this to the enemies
			foreach (Enemy enemy in enemiesToHit)
			{
				float damageAmount = 0;

				//Risk and Luck? which is bigger?

				//Calculate for lucky crit if luck > risk

				//calculate for risky crit if risk > luck

				//what range of numbers are allowed?

				//calculate how much damage we will do.
				damageAmount = Attacker.Stats.Attack;
				damageAmount -= enemy.Stats.Defense;

			}



			//We need to apply this to the party members


		}

	}

	#endregion

	#region Timing
	public class CombatDelayAction : CombatActions
	{
		public int MillisecondsDelay = 0;
		public int CurrentDelayedAmount = 0;
		public CombatDelayAction(CombatSystem cbs, int wait_millseconds) : base(cbs)
		{
			this.MillisecondsDelay = wait_millseconds;
		}
	}

	#endregion

	#region Audio

	#endregion

	#region stats
	public class CombatStatChangeAction : CombatActions
	{

		public Dictionary<BattleEntity, BaseStats> StatChangeToApply = new Dictionary<BattleEntity, BaseStats>();

		/// <summary>
		/// If True use percentage of total target stat.
		/// IF False then just flat number.
		/// </summary>
		public int turnCount = 0;

		public BaseStats StatChangeRequest = new BaseStats();

		public CombatStatChangeAction(CombatSystem cbsRef, BaseStats StatChangeRequest, bool bIsPercent, int turnCount, List<Enemy> enemiesAffected, List<PartyMember> partyMembersAffected) : base(cbsRef)
		{
			this.StatChangeRequest = StatChangeRequest;
			this.turnCount = turnCount;
			//we need to apply this to the enemies
			foreach (Enemy enemy in enemiesAffected)
			{
				if (bIsPercent)
				{
					if (StatChangeRequest.Max_Health != 0)
						StatChangeRequest.Max_Health = enemy.MaxHealth * (int)(StatChangeRequest.Max_Health / 100.0f);
					if (StatChangeRequest.Max_Mana != 0)
						StatChangeRequest.Max_Mana = enemy.MaxHealth * (int)(StatChangeRequest.Max_Mana / 100.0f);
					if (StatChangeRequest.Attack != 0)
						StatChangeRequest.Attack = enemy.Stats.Attack * (int)(StatChangeRequest.Attack / 100.0f);
					if (StatChangeRequest.Defense != 0)
						StatChangeRequest.Defense = enemy.Stats.Defense * (int)(StatChangeRequest.Defense / 100.0f);
					if (StatChangeRequest.Dexterity != 0)
						StatChangeRequest.Dexterity = enemy.Stats.Dexterity * (int)(StatChangeRequest.Dexterity / 100.0f);
					if (StatChangeRequest.Agility != 0)
						StatChangeRequest.Agility = enemy.Stats.Agility * (int)(StatChangeRequest.Agility / 100.0f);
					if (StatChangeRequest.Morality != 0)
						StatChangeRequest.Morality = enemy.Stats.Morality * (int)(StatChangeRequest.Morality / 100.0f);
					if (StatChangeRequest.Wisdom != 0)
						StatChangeRequest.Wisdom = enemy.Stats.Wisdom * (int)(StatChangeRequest.Wisdom / 100.0f);
					if (StatChangeRequest.Resistance != 0)
						StatChangeRequest.Resistance = enemy.Stats.Resistance * (int)(StatChangeRequest.Resistance / 100.0f);
					if (StatChangeRequest.Luck != 0)
						StatChangeRequest.Luck = enemy.Stats.Luck * (int)(StatChangeRequest.Luck / 100.0f);
					if (StatChangeRequest.Risk != 0)
						StatChangeRequest.Risk = enemy.Stats.Risk * (int)(StatChangeRequest.Risk / 100.0f);
					if (StatChangeRequest.Intelligence != 0)
						StatChangeRequest.Intelligence = enemy.Stats.Intelligence * (int)(StatChangeRequest.Intelligence / 100.0f);
				}

				StatChangeToApply.Add(enemy, StatChangeRequest);
			}

			//We need to apply this to the party members
			foreach (PartyMember partyMember in partyMembersAffected)
			{
				float changeAmount = 0;

				if (bIsPercent)
				{
					if (StatChangeRequest.Max_Health != 0)
						StatChangeRequest.Max_Health = partyMember.MaxHealth * (int)(StatChangeRequest.Max_Health / 100.0f);
					if (StatChangeRequest.Max_Mana != 0)
						StatChangeRequest.Max_Mana = partyMember.MaxHealth * (int)(StatChangeRequest.Max_Mana / 100.0f);
					if (StatChangeRequest.Attack != 0)
						StatChangeRequest.Attack = partyMember.Stats.Attack * (int)(StatChangeRequest.Attack / 100.0f);
					if (StatChangeRequest.Defense != 0)
						StatChangeRequest.Defense = partyMember.Stats.Defense * (int)(StatChangeRequest.Defense / 100.0f);
					if (StatChangeRequest.Dexterity != 0)
						StatChangeRequest.Dexterity = partyMember.Stats.Dexterity * (int)(StatChangeRequest.Dexterity / 100.0f);
					if (StatChangeRequest.Agility != 0)
						StatChangeRequest.Agility = partyMember.Stats.Agility * (int)(StatChangeRequest.Agility / 100.0f);
					if (StatChangeRequest.Morality != 0)
						StatChangeRequest.Morality = partyMember.Stats.Morality * (int)(StatChangeRequest.Morality / 100.0f);
					if (StatChangeRequest.Wisdom != 0)
						StatChangeRequest.Wisdom = partyMember.Stats.Wisdom * (int)(StatChangeRequest.Wisdom / 100.0f);
					if (StatChangeRequest.Resistance != 0)
						StatChangeRequest.Resistance = partyMember.Stats.Resistance * (int)(StatChangeRequest.Resistance / 100.0f);
					if (StatChangeRequest.Luck != 0)
						StatChangeRequest.Luck = partyMember.Stats.Luck * (int)(StatChangeRequest.Luck / 100.0f);
					if (StatChangeRequest.Risk != 0)
						StatChangeRequest.Risk = partyMember.Stats.Risk * (int)(StatChangeRequest.Risk / 100.0f);
					if (StatChangeRequest.Intelligence != 0)
						StatChangeRequest.Intelligence = partyMember.Stats.Intelligence * (int)(StatChangeRequest.Intelligence / 100.0f);
				}
				StatChangeToApply.Add(partyMember, StatChangeRequest);
			}


		}



		public CombatStatChangeAction(CombatSystem cbsRef, BaseStats statChangeRequest, bool bIsPercent, int turnCount, List<BattleEntity> battleEntities) : base(cbsRef)
		{
			this.StatChangeRequest = statChangeRequest;
			this.turnCount = turnCount;

			//We need to apply this to the party members
			foreach (BattleEntity partyMember in battleEntities)
			{
				float changeAmount = 0;

				if (bIsPercent)
				{
					if (StatChangeRequest.Max_Health != 0)
						StatChangeRequest.Max_Health = partyMember.MaxHealth * (int)(StatChangeRequest.Max_Health / 100.0f);
					if (StatChangeRequest.Max_Mana != 0)
						StatChangeRequest.Max_Mana = partyMember.MaxHealth * (int)(StatChangeRequest.Max_Mana / 100.0f);
					if (StatChangeRequest.Attack != 0)
						StatChangeRequest.Attack = partyMember.Stats.Attack * (int)(StatChangeRequest.Attack / 100.0f);
					if (StatChangeRequest.Defense != 0)
						StatChangeRequest.Defense = partyMember.Stats.Defense * (int)(StatChangeRequest.Defense / 100.0f);
					if (StatChangeRequest.Dexterity != 0)
						StatChangeRequest.Dexterity = partyMember.Stats.Dexterity * (int)(StatChangeRequest.Dexterity / 100.0f);
					if (StatChangeRequest.Agility != 0)
						StatChangeRequest.Agility = partyMember.Stats.Agility * (int)(StatChangeRequest.Agility / 100.0f);
					if (StatChangeRequest.Morality != 0)
						StatChangeRequest.Morality = partyMember.Stats.Morality * (int)(StatChangeRequest.Morality / 100.0f);
					if (StatChangeRequest.Wisdom != 0)
						StatChangeRequest.Wisdom = partyMember.Stats.Wisdom * (int)(StatChangeRequest.Wisdom / 100.0f);
					if (StatChangeRequest.Resistance != 0)
						StatChangeRequest.Resistance = partyMember.Stats.Resistance * (int)(StatChangeRequest.Resistance / 100.0f);
					if (StatChangeRequest.Luck != 0)
						StatChangeRequest.Luck = partyMember.Stats.Luck * (int)(StatChangeRequest.Luck / 100.0f);
					if (StatChangeRequest.Risk != 0)
						StatChangeRequest.Risk = partyMember.Stats.Risk * (int)(StatChangeRequest.Risk / 100.0f);
					if (StatChangeRequest.Intelligence != 0)
						StatChangeRequest.Intelligence = partyMember.Stats.Intelligence * (int)(StatChangeRequest.Intelligence / 100.0f);
				}
				StatChangeToApply.Add(partyMember, statChangeRequest);
			}


		}

		public CombatStatChangeAction(CombatSystem cbsRef, BaseStats statChangeRequest, bool bIsPercent, int turnCount, BattleEntity battleEntity) : base(cbsRef)
		{
			this.StatChangeRequest = statChangeRequest;
			this.turnCount = turnCount;

			float changeAmount = 0;

			if (bIsPercent)
			{
				if (StatChangeRequest.Max_Health != 0)
					StatChangeRequest.Max_Health = battleEntity.MaxHealth * (int)(StatChangeRequest.Max_Health / 100.0f);
				if (StatChangeRequest.Max_Mana != 0)
					StatChangeRequest.Max_Mana = battleEntity.MaxHealth * (int)(StatChangeRequest.Max_Mana / 100.0f);
				if (StatChangeRequest.Attack != 0)
					StatChangeRequest.Attack = battleEntity.Stats.Attack * (int)(StatChangeRequest.Attack / 100.0f);
				if (StatChangeRequest.Defense != 0)
					StatChangeRequest.Defense = battleEntity.Stats.Defense * (int)(StatChangeRequest.Defense / 100.0f);
				if (StatChangeRequest.Dexterity != 0)
					StatChangeRequest.Dexterity = battleEntity.Stats.Dexterity * (int)(StatChangeRequest.Dexterity / 100.0f);
				if (StatChangeRequest.Agility != 0)
					StatChangeRequest.Agility = battleEntity.Stats.Agility * (int)(StatChangeRequest.Agility / 100.0f);
				if (StatChangeRequest.Morality != 0)
					StatChangeRequest.Morality = battleEntity.Stats.Morality * (int)(StatChangeRequest.Morality / 100.0f);
				if (StatChangeRequest.Wisdom != 0)
					StatChangeRequest.Wisdom = battleEntity.Stats.Wisdom * (int)(StatChangeRequest.Wisdom / 100.0f);
				if (StatChangeRequest.Resistance != 0)
					StatChangeRequest.Resistance = battleEntity.Stats.Resistance * (int)(StatChangeRequest.Resistance / 100.0f);
				if (StatChangeRequest.Luck != 0)
					StatChangeRequest.Luck = battleEntity.Stats.Luck * (int)(StatChangeRequest.Luck / 100.0f);
				if (StatChangeRequest.Risk != 0)
					StatChangeRequest.Risk = battleEntity.Stats.Risk * (int)(StatChangeRequest.Risk / 100.0f);
				if (StatChangeRequest.Intelligence != 0)
					StatChangeRequest.Intelligence = battleEntity.Stats.Intelligence * (int)(StatChangeRequest.Intelligence / 100.0f);
			}
			StatChangeToApply.Add(battleEntity, statChangeRequest);
		}

	}
	#endregion

	#region Particle Systems

	public class CombatParticleSystemAction : CombatActions
	{
		public Rectangle screenbounds;
		public Texture2D emitterImage;
		public Rectangle destrect;
		public int numParticles;
		public float launchFrequency;
		public int particlePerLaunch;
		public bool bCycleParticles;
		public Texture2D ParticleImage;
		public int particleWidth;
		public int particleHeight;
		public float transparency;
		public float scalarX;
		public float ScalarY;
		public bool bColliable;
		public float gravity;
		public float wind;
		public float lowAngle;
		public float highAngle;
		public float lowSpeed;
		public float highSpeed;
		public float lowLifeSpan;
		public float highLifeSpawn;
		public Random rng;

		public Vector2 Position;
		public Color NewColor;
		public PEmitter newEmittier = null;

		/// <summary>
		/// This Method is here to change an already existing particle system.
		/// </summary>
		/// <param name="cbsRef"></param>
		/// <param name="texture"></param>
		/// <param name="newPos"></param>
		/// <param name="newColor"></param>
		public CombatParticleSystemAction(CombatSystem cbsRef, Texture2D texture, Vector2 newPos, Color newColor, bool bcycle) :
			base(cbsRef)
		{
			this.ParticleImage = texture;
			this.Position = newPos;
			this.NewColor = newColor;
			this.bCycleParticles = bcycle;
			//cbsRef.QueueHitSparkParticleSystem(texture, newPos, newColor);
		}


		////Explosion/Hit Spark
		//PEmitter pe = new PEmitter(
		//	new Rectangle(0, 0, ScreenResWidth, ScreenResHeight),
		//	particleImg, new Rectangle(0, 00, particleImg.Width, particleImg.Height), 10,
		//	0, 10, false,
		//	particleImg, (int)(particleImg.Width), (int)(particleImg.Height), 1f,
		//	.1f, .1f,
		//	true,
		//	0, 0,
		//	0, 360,
		//	400f, 500,
		//	100, 200, new Random()
		//);
		//CombatParticleEmitters_List.Add(pe);

		/// <summary>
		/// This method is here to ADD a new particle system assuming we need multiple to go off at a time.
		/// </summary>
		/// <param name="cbsRef"></param>
		public CombatParticleSystemAction(CombatSystem cbsRef,
			Rectangle screenbounds, Rectangle destrect, int numParticles,
			float launchFrequency, int particlePerLaunch, bool bCycleParticles, Texture2D particleImage,
			float transparency, float scalarX, float ScalarY,
			bool bColliable, float gravity, float wind, float lowAngle, float highAngle,
			float lowSpeed, float highSpeed, float lowLifeSpan, float highLifeSpawn, Random rng
			, Vector2 Position, Color newColor, object LinkedObject = null
		) : base(cbsRef)
		{
			this.ParticleImage = particleImage;
			this.Position = Position;
			this.NewColor = newColor;

			PEmitter pe = new PEmitter(
				screenbounds,
				particleImage, new Rectangle(0, 0, particleImage.Width, particleImage.Height), numParticles,
				launchFrequency, particlePerLaunch, bCycleParticles,
				particleImage, (int)(particleImage.Width), (int)(particleImage.Height), transparency,
				scalarX, ScalarY,
				bColliable,
				gravity, wind,
				lowAngle, highAngle,
				lowSpeed, highSpeed,
				lowLifeSpan, highLifeSpawn, rng
			);
			//cbsRef.QueueHitSparkParticleSystem(particleImage, Position, newColor, pe);

			if (LinkedObject != null)
				pe.LinkedParentObject = LinkedObject;

			this.newEmittier = pe;
		}

	}

	public class CombatParticleCyclingAction : CombatActions
	{
		public PEmitter EmitterToSop { get; set; }

		public CombatParticleCyclingAction(CombatSystem cbsRef, PEmitter emmiter) : base(cbsRef)
		{
			EmitterToSop = emmiter;
		}
	}

	public class CombatBoxParticleSystemAction : CombatParticleSystemAction
	{
		public Rectangle SpawnRectangle;
		public int EmittierMoveRate;

		public CombatBoxParticleSystemAction(CombatSystem cbsRef, Texture2D texture, Vector2 newPos, Color newColor, Rectangle spawnRectangle, int emittierMoveRate, bool bCycle) : base(cbsRef, texture, newPos, newColor, bCycle)
		{
			//PBoxEmitter pbe = new PBoxEmitter()
		}

		public CombatBoxParticleSystemAction(CombatSystem cbsRef, Rectangle spawnRectangle, int emittierMoveRate, Rectangle screenbounds, Rectangle destrect, int numParticles, float launchFrequency, int particlePerLaunch,
			bool bCycleParticles, Texture2D particleImage, float transparency, float scalarX, float ScalarY, bool bColliable, float gravity, float wind, float lowAngle, float highAngle, float lowSpeed, float highSpeed, float lowLifeSpan,
			float highLifeSpawn, Random rng, Vector2 Position, Color newColor) :
			base(cbsRef, screenbounds, destrect, numParticles, launchFrequency, particlePerLaunch, bCycleParticles, particleImage, transparency, scalarX, ScalarY,
				bColliable, gravity, wind, lowAngle, highAngle, lowSpeed, highSpeed, lowLifeSpan, highLifeSpawn, rng, Position, newColor)
		{
			this.EmittierMoveRate = emittierMoveRate;
			this.SpawnRectangle = spawnRectangle;
			PBoxEmitter pe = new PBoxEmitter(spawnRectangle, emittierMoveRate,
				screenbounds,
				particleImage, new Rectangle(0, 0, particleImage.Width, particleImage.Height), numParticles,
				launchFrequency, particlePerLaunch, bCycleParticles,
				particleImage, (int)(particleImage.Width), (int)(particleImage.Height), transparency,
				scalarX, ScalarY,
				bColliable,
				gravity, wind,
				lowAngle, highAngle,
				lowSpeed, highSpeed,
				lowLifeSpan, highLifeSpawn, rng
			);
			this.newEmittier = pe;

			pe.SetNewEmitterBounds(spawnRectangle);
		}
	}


	public class CombatHitSparkAction : CombatParticleSystemAction
	{
		public Vector2 Position;
		public Color NewColor;

		/// <summary>
		/// This Method is here to change an already existing particle system.
		/// </summary>
		/// <param name="cbsRef"></param>
		/// <param name="texture"></param>
		/// <param name="newPos"></param>
		/// <param name="newColor"></param>
		public CombatHitSparkAction(CombatSystem cbsRef, Texture2D texture, Vector2 newPos, Color newColor, bool bCycle) :
			base(cbsRef, texture, newPos, newColor, bCycle)
		{
			this.ParticleImage = texture;
			this.Position = newPos;
			this.NewColor = newColor;
		}



		public CombatHitSparkAction(CombatSystem cbsRef,
			Rectangle screenbounds, Rectangle destrect, int numParticles,
			float launchFrequency, int particlePerLaunch, bool bCycleParticles, Texture2D particleImage,
			float transparency, float scalarX, float ScalarY,
			bool bColliable, float gravity, float wind, float lowAngle, float highAngle,
			float lowSpeed, float highSpeed, float lowLifeSpan, float highLifeSpawn, Random rng
			, Vector2 Position, Color newColor
			) : base(cbsRef, screenbounds, destrect, numParticles, launchFrequency, particlePerLaunch, bCycleParticles, particleImage, transparency, scalarX, ScalarY,
			bColliable, gravity, wind, lowAngle, highAngle, lowSpeed, highSpeed, lowLifeSpan, highLifeSpawn, rng, Position, newColor)
		{
			this.ParticleImage = particleImage;
			this.Position = Position;
			this.NewColor = newColor;

			PEmitter pe = new PEmitter(
				screenbounds,
				particleImage, new Rectangle(0, 0, particleImage.Width, particleImage.Height), numParticles,
				launchFrequency, particlePerLaunch, bCycleParticles,
				particleImage, (int)(particleImage.Width), (int)(particleImage.Height), transparency,
				scalarX, ScalarY,
				bColliable,
				gravity, wind,
				lowAngle, highAngle,
				lowSpeed, highSpeed,
				lowLifeSpan, highLifeSpawn, rng
			);
			//cbsRef.QueueHitSparkParticleSystem(particleImage, Position, newColor, pe);

			this.newEmittier = pe;
		}

	}


	#endregion

	#region Animation
	public class CombatAnimationAction : CombatActions
	{

		public BattleEntity RequestedCharacter;
		public String AnimationNameRequest = String.Empty;


		public CombatAnimationAction(CombatSystem cbsRef, BattleEntity requestedCharacter, String AnimName) : base(cbsRef)
		{
			this.RequestedCharacter = requestedCharacter;
			this.AnimationNameRequest = AnimName;

		}
	}
	#endregion

	public class CombatStatusEffectChangeAction : CombatActions
	{
		public CombatStatusEffectChangeAction(CombatSystem cbsRef) : base(cbsRef)
		{
		}
	}

	public class CombatProjectileSystemAction : CombatActions
	{
		public CombatProjectileSystemAction(CombatSystem cbsRef) : base(cbsRef)
		{
		}
	}

	public class CombatSoundEffectAction : CombatActions
	{
		public CombatSoundEffectAction(CombatSystem cbsRef) : base(cbsRef)
		{
		}
	}

	public class CombatBGMAction : CombatActions
	{
		public CombatBGMAction(CombatSystem cbsRef) : base(cbsRef)
		{
		}
	}

	public class CombatCutsceneAction : CombatActions
	{
		public CombatCutsceneAction(CombatSystem cbsRef) : base(cbsRef)
		{
		}
	}

public class CombatActions
	{
		protected CombatSystem CBSRef;
		public CombatActions(CombatSystem cbsRef)
		{
			this.CBSRef = cbsRef;
		}

	}
}
