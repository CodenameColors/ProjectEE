#define DEBUG //comment for not Antonio

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using BixBite.Characters;
using BixBite.Items;
using BixBite.Particles;
using BixBite.Rendering.UI;
using BixBite.Rendering.UI.ListBox;
using BixBite.Rendering.UI.ListBox.ListBoxItems;
using BixBite.Rendering.UI.TextBlock;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SQLite;

namespace BixBite.Combat
{



	//these are all the possible states of the combat state machine
	//this doesn't include the sending of data data back to the master database.
	public enum ECombatState
	{
		NONE,
		BattleStart,
		BattleEnd,
		StartTurn,
		EndTurn,
		StartEntityTurn,
		EndEntityTurn,
		WaitingForInput,
		StartPhysicalAttack,
		Attack,
		EndAttack,
		StartSkillsAttack,
		SkillsAttack,
		EndSkillsAttack,
		StartItemUse,
		ItemUse,
		EndItemUse,
		Defense,
		ChooseTarget,
		ApplyAttackEffects,
		StartFollowupAttacks,
		FollowUpAttacks,
		EndFollowUpAttacks,
		MainUISpawn,						//ONLY USE FOR PAST STATE
		SkillsUISpawn,          //ONLY USE FOR PAST STATE
		ItemsUISpawn,           //ONLY USE FOR PAST STATE

	}

	public enum EBattleCommand
	{
		NONE,
		WEAPON,
		CURSOR,
		ESSENCE,
		STANCE,
		ATTACK,
		SKILL,
		ITEM,
		DEFENSE,
		SELECT,
		BACK,
		MENU1,
		MENU2,
		MENU3,
		MENU4,
		MENU5,
		MENU6,

	}

	//This class is here to hold data for the current combat system in play.
	//Only one combat system can happen at once for this game. So we can make this a SingleTon
	public class CombatSystem
	{

		//THESE ARE HERE FOR IZZY ONLY. DELETE AFTER IT WORKS.
		#region ForIzzy
		public Texture2D AttacktSticker_UI_izzy;
		public Texture2D DefensetSticker_UI_izzy;
		public Texture2D SkillSticker_UI_izzy;

		public Texture2D WeaponLeftArrow_UI_izzy;
		public Texture2D WeaponRightArrow_UI_izzy;
		public Texture2D StanceLeftArrow_UI_izzy;
		public Texture2D StanceRightArrow_UI_izzy;

		public Texture2D InventorySticker_UI_izzy;
		public Texture2D WeaponCardSticker_UI_izzy;

		public Dictionary<String, Texture2D> WeaponCards_Textures_Dict = new Dictionary<string, Texture2D>();

		public List<Tuple<int, int, int, int>> CustomSizes_Offsets_Izzy = new List<Tuple<int, int, int, int>>();

		private int AttackStickerWidth_Izzy { get => CustomSizes_Offsets_Izzy[0].Item1; }
		private int AttackStickerHeight_izzy { get => CustomSizes_Offsets_Izzy[0].Item2; }
		private int AttackStickerXOffset_Izzy { get => CustomSizes_Offsets_Izzy[0].Item3; }
		private int AttackStickerYOffset_Izzy { get => CustomSizes_Offsets_Izzy[0].Item4; }

		private int DefenseStickerWidth_Izzy { get => CustomSizes_Offsets_Izzy[1].Item1; }
		private int DefenseStickerHeight_izzy { get => CustomSizes_Offsets_Izzy[1].Item2; }
		private int DefenseStickerXOffset_Izzy { get => CustomSizes_Offsets_Izzy[1].Item3; }
		private int DefenseStickerYOffset_Izzy { get => CustomSizes_Offsets_Izzy[1].Item4; }

		private int SkillsStickerWidth_Izzy { get => CustomSizes_Offsets_Izzy[2].Item1; }
		private int SkillsStickerHeight_izzy { get => CustomSizes_Offsets_Izzy[2].Item2; }
		private int SkillsStickerXOffset_Izzy { get => CustomSizes_Offsets_Izzy[2].Item3; }
		private int SkillsStickerYOffset_Izzy { get => CustomSizes_Offsets_Izzy[2].Item4; }

		private int InventoryStickerWidth_Izzy { get => CustomSizes_Offsets_Izzy[3].Item1; }
		private int InventoryStickerHeight_izzy { get => CustomSizes_Offsets_Izzy[3].Item2; }
		private int InventoryStickerXOffset_Izzy { get => CustomSizes_Offsets_Izzy[3].Item3; }
		private int InventoryStickerYOffset_Izzy { get => CustomSizes_Offsets_Izzy[3].Item4; }

		private int LeftStanceStickerWidth_Izzy { get => CustomSizes_Offsets_Izzy[4].Item1; }
		private int LeftStanceStickerHeight_izzy { get => CustomSizes_Offsets_Izzy[4].Item2; }
		private int LeftStanceStickerXOffset_Izzy { get => CustomSizes_Offsets_Izzy[4].Item3; }
		private int LeftStanceStickerYOffset_Izzy { get => CustomSizes_Offsets_Izzy[4].Item4; }

		private int RightStanceStickerWidth_Izzy { get => CustomSizes_Offsets_Izzy[5].Item1; }
		private int RightStanceStickerHeight_izzy { get => CustomSizes_Offsets_Izzy[5].Item2; }
		private int RightStanceStickerXOffset_Izzy { get => CustomSizes_Offsets_Izzy[5].Item3; }
		private int RightStanceStickerYOffset_Izzy { get => CustomSizes_Offsets_Izzy[5].Item4; }

		private int LeftWeaponStickerWidth_Izzy { get => CustomSizes_Offsets_Izzy[6].Item1; }
		private int LeftWeaponStickerHeight_izzy { get => CustomSizes_Offsets_Izzy[6].Item2; }
		private int LeftWeaponStickerXOffset_Izzy { get => CustomSizes_Offsets_Izzy[6].Item3; }
		private int LeftWeaponStickerYOffset_Izzy { get => CustomSizes_Offsets_Izzy[6].Item4; }

		private int RightWeaponStickerWidth_Izzy { get => CustomSizes_Offsets_Izzy[7].Item1; }
		private int RightWeaponStickerHeight_izzy { get => CustomSizes_Offsets_Izzy[7].Item2; }
		private int RightWeaponStickerXOffset_Izzy { get => CustomSizes_Offsets_Izzy[7].Item3; }
		private int RightWeaponStickerYOffset_Izzy { get => CustomSizes_Offsets_Izzy[7].Item4; }

		private int WeaponCardStickerWidth_Izzy { get => CustomSizes_Offsets_Izzy[8].Item1; }
		private int WeaponCardStickerHeight_izzy { get => CustomSizes_Offsets_Izzy[8].Item2; }
		private int WeaponCardStickerXOffset_Izzy { get => CustomSizes_Offsets_Izzy[8].Item3; }
		private int WeaponCardStickerYOffset_Izzy { get => CustomSizes_Offsets_Izzy[8].Item4; }

		#endregion


		public ContentManager ContentRef;

		public List<PEmitter> CombatParticleEmitters_List = new List<PEmitter>();
		public List<BaseUI> CombatUI_List = new List<BaseUI>();

		//DEBUG
		private GameTextBlock debug_LogBlock;


		#region Fields

		private List<FollowUpAttack> FollowUpAttacks;
		
		#region KeySettings
		private Keys LeftKey;
		private Keys RightKey;
		private Keys UpKey;
		private Keys DownKey;

		private Keys AttackKey;
		private Keys SkillKey;
		private Keys DefenseKey;
		private Keys ItemKey;
		private Keys EssenceDownKey;
		private Keys EssenceUpKey;
		private Keys EssenceLeftKey;
		private Keys EssenceRightKey;

		private Keys StanceLeftKey;
		private Keys StanceRightKey;

		private Keys BackKey;
		private Keys SelectKey;

		private Keys CBMenuSelect1;
		private Keys CBMenuSelect2;
		private Keys CBMenuSelect3;
		private Keys CBMenuSelect4;
		private Keys CBMenuSelect5;
		private Keys CBMenuSelect6;
		#endregion
		private KeyboardState prevKeyState;
		private SQLiteConnection _sqlite_conn;

		private Vector2 TargetPosition_hitspark;

		private Enemy currentSelectedEnemy
		{
			get => BattleEnemyList[CurrentEnemiesNames[currentEnemyIndex]];
		}

		private PartyMember currentSelectedPartyMember
		{
			get => PartyMembers[CurrentPartyMembersNames[currentPartyMemberIndex]];
		}

		private PartyMember CurrentPartyMember_turn
		{
			get
			{
				if (CurrentTurnCharacter is PartyMember)
					return CurrentTurnCharacter as PartyMember;
				else return null;
			}
		}

		private Enemy currentEnemy_turn
		{
			get
			{
				if (CurrentTurnCharacter is Enemy)
					return CurrentTurnCharacter as Enemy;
				else return null;
			}
		}

		private Skill SkillToUse = null;
		private Item ItemToUse =  null;

		//These are here to hold the data/keep track of what modifiers to activate/apply during an event.
		private List<Tuple<BattleEntity, ModifierData>> EntityAttackEffectModifier_List = new List<Tuple<BattleEntity, ModifierData>>();
		private List<Tuple<BattleEntity, ModifierData>> EntityAttackTraitModifier_List = new List<Tuple<BattleEntity, ModifierData>>();

		private List<Tuple<BattleEntity, ModifierData>> EntityDefenseEffectModifier_List = new List<Tuple<BattleEntity, ModifierData>>();
		private List<Tuple<BattleEntity, ModifierData>> EntityDefenseTraitModifier_List = new List<Tuple<BattleEntity, ModifierData>>();

		private List<Tuple<BattleEntity, ModifierData>> EntityBuffEffectModifier_List = new List<Tuple<BattleEntity, ModifierData>>();
		private List<Tuple<BattleEntity, ModifierData>> EntityBuffTraitModifier_List = new List<Tuple<BattleEntity, ModifierData>>();

		private List<Tuple<BattleEntity, ModifierData>> EntityDebuffEffectModifier_List = new List<Tuple<BattleEntity, ModifierData>>();
		private List<Tuple<BattleEntity, ModifierData>> EntityDebuffTraitModifier_List = new List<Tuple<BattleEntity, ModifierData>>();

		private List<Tuple<BattleEntity, EStanceType>> followUpAttack_List = new List<Tuple<BattleEntity, EStanceType>>();
		private List<Tuple<String, List<PartyMember>>> RequestedFollowupAttacks = new List<Tuple<string, List<PartyMember>>>();

		#endregion

		#region Properties
		public String DatabasePath { get; set; }
		public SpriteFont Font { get; set; }
		#endregion


		//UI
		private GameListBox TurnQueue_GameListBox;
		private List<UIComponent> _uiComponents;

		private Dictionary<String, GameListBox> _partyMemberSkillListBoxes_dict = new Dictionary<string, GameListBox>();
		private Dictionary<String, GameListBox> _partyMemberItemsListBoxes_dict = new Dictionary<string, GameListBox>();

		//Sticker Ui
		private bool bDrawAttackSticker_UI { get; set; }
		public Texture2D AttackSticker_UI;
		private Rectangle AttackSticker_UI_rect;

		private bool bDrawDefenseSticker_UI { get; set; }
		public Texture2D DefenseSticker_UI;
		private Rectangle DefenseSticker_UI_rect;

		private bool bDrawSkillsSticker_UI { get; set; }
		public Texture2D SkillsSticker_UI;
		private Rectangle SkillsSticker_UI_rect;

		//Stances and Weapon Arrows.
		private bool bDrawWeaponLeftArrow_UI { get; set; }
		public Texture2D WeaponLeftArrow_UI;
		private Rectangle WeaponLeftArrow_UI_rect;

		private bool bDrawWeaponRightArrow_UI { get; set; }
		public Texture2D WeaponRightArrow_UI;
		private Rectangle WeaponRightArrow_UI_rect;

		private bool bDrawStanceLeftArrow_UI { get; set; }
		public Texture2D StanceLeftArrow_UI;
		private Rectangle StanceLeftArrow_UI_rect;

		private bool bDrawStanceRightArrow_UI { get; set; }
		public Texture2D StanceRightArrow_UI;
		private Rectangle StanceRightArrow_UI_rect;

		//Inventory
		private bool bDrawInventorySticker_UI { get; set; }
		public Texture2D InventorySticker_UI;
		private Rectangle InventorySticker_UI_rect;

		//Inventory
		private bool bDrawWeaponCardSticker_UI { get; set; }
		public Texture2D WeaponCardSticker_UI;
		private Rectangle WeaponCardSticker_UI_rect;


		//Skill menu
		private bool bDrawSkillMenu_UI { get; set; }
		public Texture2D SkillMenu_UI;
		private Vector2 SKillsMenu_UI_pos = new Vector2(0,0);

		//Select Arrow
		private bool bDrawSelectArrowUI { get; set; }
		public Texture2D SelectArrowUI;
		private Vector2 SelectArrowUI_Pos = new Vector2(0, 0);

		private bool bDrawSelectArrowAreaRange;
		public Texture2D SelectArrowAreaRange_UI;
		public Rectangle SelectArrowAreaRange_Rect = Rectangle.Empty;
		public Effect textureEffect;

		//Textbox for Stances
		private bool bDrawStanceTextBox = false;
		private GameTextBlock Stance_GTB;

		//Textbox for Stances
		private bool bDrawWeaponTextBox = false;
		private GameTextBlock Weapon_GTB;

		//Essence Bar
		public BixBite.Rendering.UI.ProgressBar.GameCustomProgressBar EssenceBar;

		private int PlayerCounter = 0;
		private int DefaultInterlopation = 10;
		private int PlayerMenuIndex = 0;

		private static Lazy<CombatSystem> combatSystem = new Lazy<CombatSystem>(() => new CombatSystem());
		public EBattleCommand CommandRequest = EBattleCommand.NONE;
		private ECombatState pastCombatState;

		private ECombatState combatState = ECombatState.NONE;
		public ECombatState CombatState
		{
			get => combatState;
			set
			{
				//pastCombatState = combatState; //store the past state for state machine/graph traversal
				combatState = value;
			}
		}

		//Action Queue to handle combat actions in an order.
		LinkedList<CombatActions> combatActions_Queue = new LinkedList<CombatActions>();

		//Combat system needs to hold the fighters inside it
		public Dictionary<String, PartyMember> PartyMembers = new Dictionary<String, PartyMember>();
		public Dictionary<String, Enemy> BattleEnemyList = new Dictionary<string, Enemy>();

		//Names of the characters that are active from the FULL list of party members
		private int currentPartyMemberIndex = 0;
		private int MaxPartyMembersIndex { get => CurrentPartyMembersNames.Count - 1; }
		public List<String> CurrentPartyMembersNames = new List<string>();

		private int currentEnemyIndex = 0;
		private int MaxEnemyIndex { get => CurrentEnemiesNames.Count - 1; }
		public List<String> CurrentEnemiesNames = new List<string>();

		//The character to move, and the x, and y, interpolation per tick
		//public Queue<Tuple<BattleEntity, int, int, int>> MovementQueue = new Queue<Tuple<BattleEntity, int, int, int >>();
		public Queue<BattleEntity> TurnQueue = new Queue<BattleEntity>();

		// TODO: Change the object to the Interface type after i make it.
		public Stack<object> FollowUpStack = new Stack<object>();

		//TODO: Change the object to the environment type when i make it.
		public object Environment = new object();

		public int ComboDamage = 0;
		public int HighestComboDamage = 0;

		public BattleEntity CurrentTurnCharacter = new BattleEntity();

		public static CombatSystem Instance
		{
			get => combatSystem.Value;
		}

		private bool bMovementInProgress = false;

		private Random combatRNG = new Random();
		private Player PlayerData_Ref = new Player();


		public BixBite.Rendering.UI.ProgressBar.GameCustomProgressBar PartyMemberEssenceBar;

		public CombatSystem()
		{
			//Set the keyboard settings for use.
			LeftKey = Properties.Settings.Default.Left;
			RightKey = Properties.Settings.Default.Right;
			UpKey = Properties.Settings.Default.Up;
			DownKey = Properties.Settings.Default.Down;

			AttackKey = Properties.Settings.Default.Attack;
			SkillKey = Properties.Settings.Default.Skills;
			DefenseKey = Properties.Settings.Default.Defense;
			ItemKey = Properties.Settings.Default.Items;

			EssenceDownKey = Properties.Settings.Default.EssenceDown;
			EssenceUpKey = Properties.Settings.Default.EssenceUp;
			EssenceLeftKey = Properties.Settings.Default.EssenceLeft;
			EssenceRightKey = Properties.Settings.Default.EssenceRight;

			StanceLeftKey = Properties.Settings.Default.StanceLeft;
			StanceRightKey = Properties.Settings.Default.StanceRight;

			BackKey = Properties.Settings.Default.Back;
			SelectKey = Properties.Settings.Default.Select;

			CBMenuSelect1 = Properties.Settings.Default.CBMenuSelect1;
			CBMenuSelect2 = Properties.Settings.Default.CBMenuSelect2;
			CBMenuSelect3 = Properties.Settings.Default.CBMenuSelect3;
			CBMenuSelect4 = Properties.Settings.Default.CBMenuSelect4;
			CBMenuSelect5 = Properties.Settings.Default.CBMenuSelect5;
			CBMenuSelect6 = Properties.Settings.Default.CBMenuSelect6;

			//DEBUG TEXTBLOCK
			var tbtest = new GameTextBlock("LogTextblock", 0, 00, 0, 0, 1, "#00000000", "testing");

			debug_LogBlock = (tbtest);
			((GameTextBlock)debug_LogBlock).Position = new Vector2(20, 1080 - 120);
			((GameTextBlock)debug_LogBlock).PenColour = Color.White;

			//Stance TEXTBLOCK
			var stancetb = new GameTextBlock("StanceTextBlock", 160, 40, 0, 0, 0, "#00000000", "[PUTSTANCENAMEHERE]");

			Stance_GTB = (stancetb);
			((GameTextBlock)Stance_GTB).Position = new Vector2(20, 20);
			((GameTextBlock)Stance_GTB).PenColour = Color.White;
			Stance_GTB.bMiddleHorizontal = true;
			Stance_GTB.bMiddleVertical = true;


			//weapon TEXTBLOCK
			var weapontb = new GameTextBlock("WeaponTextBlock", 160, 40, 0, 0, 1, "#00000000", "[PUTWEAPONNAMEHERE]");

			Weapon_GTB = (weapontb);
			((GameTextBlock)Weapon_GTB).Position = new Vector2(20, 20);
			((GameTextBlock)Weapon_GTB).PenColour = Color.White;
			Weapon_GTB.bMiddleHorizontal = true;
			Weapon_GTB.bMiddleVertical = true;

			//Turn Queue
			TurnQueue_GameListBox = new GameListBox("Turn_Queue_LB", 10,10, 1920 - 100 - 600, 100, 100,100, 10, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, 
				10, false, EPositionType.Horizontal);

		}

		public void LoadIzzysUIBindingForTest(GraphicsDevice graphicsDevice, String file1, String file2, String file3, String arrowleft, String arrowright, String inventory)
		{
			using (var stream = new System.IO.FileStream(file1, FileMode.Open))
			{
				AttacktSticker_UI_izzy = Texture2D.FromStream(graphicsDevice, stream);
			}

			using (var stream = new System.IO.FileStream(file2, FileMode.Open))
			{
				DefensetSticker_UI_izzy = Texture2D.FromStream(graphicsDevice, stream);
			}

			using (var stream = new System.IO.FileStream(file3, FileMode.Open))
			{
				SkillSticker_UI_izzy = Texture2D.FromStream(graphicsDevice, stream);
			}


			using (var stream = new System.IO.FileStream(arrowleft, FileMode.Open))
			{
				StanceLeftArrow_UI_izzy = Texture2D.FromStream(graphicsDevice, stream);
			}

			using (var stream = new System.IO.FileStream(arrowleft, FileMode.Open))
			{
				WeaponLeftArrow_UI_izzy = Texture2D.FromStream(graphicsDevice, stream);
			}


			using (var stream = new System.IO.FileStream(arrowright, FileMode.Open))
			{
				StanceRightArrow_UI_izzy = Texture2D.FromStream(graphicsDevice, stream);
			}

			using (var stream = new System.IO.FileStream(arrowright, FileMode.Open))
			{
				WeaponRightArrow_UI_izzy = Texture2D.FromStream(graphicsDevice, stream);
			}

			using (var stream = new System.IO.FileStream(inventory, FileMode.Open))
			{
				InventorySticker_UI_izzy = Texture2D.FromStream(graphicsDevice, stream);
			}

			//using (var stream = new System.IO.FileStream(WeaponCard, FileMode.Open))
			//{
			//	WeaponCardSticker_UI_izzy = Texture2D.FromStream(graphicsDevice, stream);
			//}


		}

		public void LoadPlayerUserDatRef(ref Player p)
		{
			PlayerData_Ref = p;
		}

		public void LoadFollowUpAttacks(List<FollowUpAttack> followUpAttacks)
		{
			this.FollowUpAttacks = followUpAttacks;
		}

		public void LoadShaderEffects()
		{
			
		}

		public void LoadBoxParticleSystem(Rectangle spawnRectangle, int emitterMoveRate, Rectangle screenBounds, Texture2D particleImg, int index, float scaleX, float scaleY, bool bCycle)
		{
			(CombatParticleEmitters_List[index] as PBoxEmitter).SetScreenBounds(screenBounds);
			(CombatParticleEmitters_List[index] as PBoxEmitter).SetParticleTexture(particleImg);
			(CombatParticleEmitters_List[index] as PBoxEmitter).SetParticleScale(scaleX,scaleY);
			(CombatParticleEmitters_List[index] as PBoxEmitter).SetNewEmitterBounds(spawnRectangle);
			(CombatParticleEmitters_List[index] as PBoxEmitter).emitterMoveRate = emitterMoveRate;
			(CombatParticleEmitters_List[index] as PBoxEmitter).SetCycleStatus(bCycle);

		}

		public void LoadHitSparkParticleSystem(Rectangle screenBounds, Texture2D particleImg, int index, float scaleX, float scaleY, bool bCycle)
		{
			(CombatParticleEmitters_List[index] as HitSparkPEmitter).SetScreenBounds(screenBounds);
			(CombatParticleEmitters_List[index] as HitSparkPEmitter).SetParticleTexture(particleImg);
			(CombatParticleEmitters_List[index] as HitSparkPEmitter).SetParticleScale(scaleX, scaleY);
			(CombatParticleEmitters_List[index] as HitSparkPEmitter).SetCycleStatus(bCycle);
		}

		//public void LoadHitSparkParticleSystem(int ScreenResWidth, int ScreenResHeight, Texture2D particleImg)
		//{
		//	//Explosion/Hit Spark
		//	PEmitter pe = new PEmitter(
		//		new Rectangle(0, 0, ScreenResWidth, ScreenResHeight),
		//		particleImg, new Rectangle(0, 00, particleImg.Width, particleImg.Height), 10,
		//		0, 10, false,
		//		particleImg, (int)(particleImg.Width), (int)(particleImg.Height), 1f,
		//		.1f, .1f,
		//		true,
		//		0, 0,
		//		0, 360,
		//		400f, 500,
		//		100, 200, new Random()
		//	);
		//	CombatParticleEmitters_List.Add(pe);
		//}

		//public void LoadHitSparkParticleSystem(int ScreenResWidth, int ScreenResHeight, Texture2D particleImg, float scalex, float scaley)
		//{
		//	//Explosion/Hit Spark
		//	PEmitter pe = new PEmitter(
		//		new Rectangle(0, 0, ScreenResWidth, ScreenResHeight),
		//		particleImg, new Rectangle(0, 00, particleImg.Width, particleImg.Height), 10,
		//		0, 10, false,
		//		particleImg, (int)(particleImg.Width), (int)(particleImg.Height), 1f,
		//		scalex, scaley,
		//		true,
		//		0, 0,
		//		0, 360,
		//		400f, 500,
		//		100, 200, new Random()
		//	);
		//	CombatParticleEmitters_List.Add(pe);
		//}


		//public void LoadCombatParticleSystem(int ScreenResWidth, int ScreenResHeight, Texture2D particleImg)
		//{
		//	//Explosion/Hit Spark
		//	CombatParticleSystem = new PEmitter(
		//		new Rectangle(0, 0, ScreenResWidth, ScreenResHeight),
		//		particleImg, new Rectangle(0, 00, 2, 5), 10,
		//		1, 10, false,
		//		particleImg, (int)(particleImg.Width), (int)(particleImg.Height), 1f,
		//		1f, 1f,
		//		true,
		//		0, 0,
		//		0, 360,
		//		100f, 200f,
		//		200, 400, new Random()
		//	);
		//}


		public void LoadDebugFont(SpriteFont sf)
		{
			(debug_LogBlock as GameTextBlock)._font = sf;
			(debug_LogBlock as GameTextBlock).PenColour = Color.White;

			(Stance_GTB as GameTextBlock)._font = sf;
			(Stance_GTB as GameTextBlock).PenColour = Color.White;

			(Weapon_GTB as GameTextBlock)._font = sf;
			(Weapon_GTB as GameTextBlock).PenColour = Color.White;
		}

		public void ChangeCharacterTurn()
		{
			CurrentTurnCharacter = TurnQueue.Dequeue();
		}

		/// <summary>
		/// Moves the battle enitiy in LERP profile to its destination
		/// </summary>
		/// <returns>True = Done.</returns>
		private bool ApplyBattleEntityMovement(BattleEntity desiredEntity, int? desired_X, int? desired_Y, int interpolation)
		{
			bool retb = false;
			int desiredX = 0; int desiredY = 0;

			if (desired_X == null)
				desiredX = (int)desiredEntity.Position.X;
			else desiredX = (int) desired_X;
			if (desired_Y == null)
				desiredY = (int)desiredEntity.Position.Y;
			else desiredY = (int)desired_Y;

			if (PartyMembers.Values.Contains(desiredEntity))
			{
				BattleEntity b = (BattleEntity)PartyMembers.Values.Single(x => x == desiredEntity);
				if (Math.Abs(desiredX - b.Position.X) < (interpolation / 2) +1 && Math.Abs(desiredX - b.Position.X) > 0)
					b.SetPosition(desiredX, null);
				else if ((desiredX - b.Position.X) < 0)
					b.SetPosition((int) (b.Position.X + interpolation * -1), null);
				else if ((desiredX - b.Position.X) > 0)
					b.SetPosition((int)(b.Position.X + interpolation ), null);

				if (Math.Abs(desiredY - b.Position.Y) < (interpolation / 2)+1 && Math.Abs(desiredY - b.Position.Y) > 0)
					b.SetPosition(null, desiredY);
				else if ((desiredY - b.Position.Y) < 0)
					b.SetPosition(null, (int)(b.Position.Y + interpolation * -1));
				else if ((desiredY - b.Position.Y) > 0)
					b.SetPosition(null, (int)(b.Position.Y + interpolation));

				if (Math.Abs(desiredY - b.Position.Y) == 0 &&
				    Math.Abs(desiredX - b.Position.X) == 0)
				{
					retb = true;
				}
			}

			if (BattleEnemyList.Values.Contains(desiredEntity))
			{
				BattleEntity b = (BattleEntity)BattleEnemyList.Values.Single(x => x == desiredEntity);
				if (Math.Abs(desiredX - b.Position.X) < (interpolation / 2)+1 && Math.Abs(desiredX - b.Position.X) > 0)
					b.SetPosition(desiredX, null);
				else if ((desiredX - b.Position.X) < 0)
					b.SetPosition((int)(b.Position.X + interpolation * -1), null);
				else if ((desiredX - b.Position.X) > 0)
					b.SetPosition((int)(b.Position.X + interpolation), null);

				if (Math.Abs(desiredY - b.Position.Y) < (interpolation / 2)+1 && Math.Abs(desiredY - b.Position.Y) > 0)
					b.SetPosition(null, desiredY);
				else if ((desiredY - b.Position.Y) < 0)
					b.SetPosition(null, (int)(b.Position.Y + interpolation * -1));
				else if ((desiredY - b.Position.Y) > 0)
					b.SetPosition(null, (int)(b.Position.Y + interpolation));

				if (Math.Abs(desiredY - b.Position.Y) == 0 &&
				    Math.Abs(desiredX - b.Position.X) == 0)
				{
					retb = true;
				}
				
			}

			//Does the battle entity have a particle system tied to them?
			foreach ( PEmitter pEmitter in CombatParticleEmitters_List)
			{
				if (pEmitter.LinkedParentObject == desiredEntity)
				{
					pEmitter.SetPosition( desiredEntity.Position + (new Vector2(desiredEntity.Width/2, desiredEntity.Height) * new Vector2(desiredEntity.ScaleX, desiredEntity.ScaleY)) );
				}
			}
			return retb;
		}

		public bool ApplyUIMovementAction(BaseUI requestedBaseUI, int? desiredx, int? desiredy, int interpolation)
		{
			bool retb = false;
			int desiredX =0; int desiredY = 0;

			if (desiredx == null)
				desiredX = requestedBaseUI.XPos;
			else desiredX = (int)desiredx;
			if (desiredy == null)
				desiredY = requestedBaseUI.YPos;
			else desiredY = (int)desiredy;

			if (Math.Abs(desiredX - requestedBaseUI.XPos) < (interpolation / 2) + 1 && Math.Abs(desiredX - requestedBaseUI.XPos) > 0)
				requestedBaseUI.XPos = desiredX; 
			else if ((desiredX - requestedBaseUI.XPos) < 0)
				 requestedBaseUI.XPos =  (int)(requestedBaseUI.XPos + interpolation * -1);
			else if ((desiredX - requestedBaseUI.XPos) > 0)
				requestedBaseUI.XPos = ((int)(requestedBaseUI.XPos + interpolation));

			if (Math.Abs(desiredY - requestedBaseUI.YPos) < (interpolation / 2) + 1 && Math.Abs(desiredY - requestedBaseUI.YPos) > 0)
				requestedBaseUI.YPos = desiredY;
			else if ((desiredY - requestedBaseUI.YPos) < 0)
				requestedBaseUI.YPos = (int)(requestedBaseUI.YPos + interpolation * -1);
			else if ((desiredY - requestedBaseUI.YPos) > 0)
				requestedBaseUI.YPos =  (int)(requestedBaseUI.YPos + interpolation);

			if (Math.Abs(desiredY - requestedBaseUI.YPos) == 0 &&
			    Math.Abs(desiredX - requestedBaseUI.XPos) == 0)
			{
				retb = true;
			}
			return retb;
		}


		public void QueueCombatAction(CombatActions ca)
		{
			combatActions_Queue.AddLast(ca);
		}


		public void ApplyUISpawnAction(BaseUI baseui)
		{
			CombatUI_List.Add(baseui);
		}

		public bool ApplyKillUIAction(BaseUI baseui)
		{
			if (CombatUI_List.Contains(baseui))
			{
				CombatUI_List.Remove(baseui);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="combatAction"></param>
		/// <param name="deltaTime"></param>
		/// <returns> returns true IF we removed the action from queue</returns>
		public bool HandleCombatActionEvents(LinkedListNode<CombatActions> combatAction, int deltaTime)
		{
			bool retb = false;
			switch (combatAction.Value)
			{
				#region ParticleSystems
				case CombatParticleCyclingAction combatParticleCycling:
					combatParticleCycling.EmitterToSop?.StopCyclying();
					combatActions_Queue.RemoveFirst();
					retb = true;

					break;
				case CombatBoxParticleSystemAction combatBoxParticleSystemAction:
					//Did we make a new emitter?
					if (combatBoxParticleSystemAction.newEmittier == null)
					{
						QueueBoxParticleSystem(combatBoxParticleSystemAction.ParticleImage, combatBoxParticleSystemAction.Position, combatBoxParticleSystemAction.NewColor,
							combatBoxParticleSystemAction.scalarX, combatBoxParticleSystemAction.ScalarY, combatBoxParticleSystemAction.bCycleParticles);
					}
					else
					{
						QueueBoxParticleSystem(combatBoxParticleSystemAction.ParticleImage, combatBoxParticleSystemAction.Position, combatBoxParticleSystemAction.NewColor,
							combatBoxParticleSystemAction.scalarX, combatBoxParticleSystemAction.ScalarY, combatBoxParticleSystemAction.bCycleParticles, combatBoxParticleSystemAction.newEmittier as PBoxEmitter);
					}

					combatActions_Queue.RemoveFirst();
					retb = true;
					break;

				case CombatHitSparkAction combatHitSparkAction:

					//Did we make a new Emitter?
					if (combatHitSparkAction.newEmittier == null)
					{
						QueueHitSparkParticleSystem(combatHitSparkAction.ParticleImage, combatHitSparkAction.Position, combatHitSparkAction.NewColor,
							combatHitSparkAction.scalarX, combatHitSparkAction.ScalarY, combatHitSparkAction.bCycleParticles, null);
					}
					else
					{
						QueueHitSparkParticleSystem(combatHitSparkAction.ParticleImage, combatHitSparkAction.Position, combatHitSparkAction.NewColor,
							combatHitSparkAction.scalarX, combatHitSparkAction.ScalarY, combatHitSparkAction.bCycleParticles, combatHitSparkAction.newEmittier as HitSparkPEmitter);
					}
					combatActions_Queue.RemoveFirst();
					retb = true;
					break;
				case CombatParticleSystemAction combatParticleSystemAction:
					Vector2 pos = Vector2.Zero;
					pos = combatParticleSystemAction.Position;

					if (combatParticleSystemAction.newEmittier != null)
						QueueGenericParticleSystem(combatParticleSystemAction.ParticleImage, pos, combatParticleSystemAction.NewColor,
							combatParticleSystemAction.scalarX, combatParticleSystemAction.ScalarY, combatParticleSystemAction.bCycleParticles, combatParticleSystemAction.newEmittier as PEmitter);
					else
						QueueGenericParticleSystem(combatParticleSystemAction.ParticleImage, pos, combatParticleSystemAction.NewColor,
							combatParticleSystemAction.scalarX, combatParticleSystemAction.ScalarY, combatParticleSystemAction.bCycleParticles, combatParticleSystemAction.newEmittier as PEmitter);
					combatActions_Queue.RemoveFirst();
					retb = true;
					break;

				#endregion

				#region Animation
				case CombatAnimationAction combatAnimationAction:
					//Use the queued information to change the requested characters animation.
					combatAnimationAction.RequestedCharacter.GetSpriteSheet().ChangeAnimation(combatAnimationAction.AnimationNameRequest);
					combatActions_Queue.RemoveFirst();
					retb = true;
					break;
				#endregion

				#region Audio

				#endregion

				#region StatChanges

				#endregion

				#region Cutscnees

				#endregion

				#region Movement
				case CombatMoveAction combatMoveAction:
					//Move, and only remove from Queue if entity has reached their destination.
					if (ApplyBattleEntityMovement(combatMoveAction.CharacterToMove, combatMoveAction.newXPos,
						combatMoveAction.newYPos, combatMoveAction.InterpolationPerTick))
					{
						combatActions_Queue.RemoveFirst();
						retb = true;
					}

					break;
				case CombatBatchMoveAction batchMoveAction:
					if (batchMoveAction.BatchMoveActions.Count > 0)
					{
						int index = 0;
						do
						{
							if (ApplyBattleEntityMovement(
								batchMoveAction.BatchMoveActions[index].CharacterToMove,
								batchMoveAction.BatchMoveActions[index].newXPos, batchMoveAction.BatchMoveActions[index].newYPos,
								batchMoveAction.BatchMoveActions[index].InterpolationPerTick))
							{
								//We can remove it. As it is done.
								batchMoveAction.BatchMoveActions.RemoveAt(index);
							}
							else
							{
								//We finished moving for this frame. But we still have more. Do not remove it!
								index++;
							}
						} while (index < batchMoveAction.BatchMoveActions.Count);
					}
					else
					{
						combatActions_Queue.RemoveFirst();
						retb = true;
					}
					break;
				#endregion

				#region Timing
				case CombatDelayAction combatDelay: //Simple delay action.

				if (combatDelay.CurrentDelayedAmount > combatDelay.MillisecondsDelay)
				{
					combatActions_Queue.RemoveFirst(); // the time has past and the delay is over.
					retb = true;
				}
				else
						combatDelay.CurrentDelayedAmount += deltaTime;
					break;
				#endregion

				#region UI
				case CombatSpawnUIAction combatSpawnUIAction:
					ApplyUISpawnAction(combatSpawnUIAction.DesiredUI);
					combatActions_Queue.RemoveFirst();
					retb = true;
					break;

				case CombatBatchMoveUIAction combatBatchMoveUI:
					if (combatBatchMoveUI.BatchMoveUIActions.Count > 0)
					{
						int index = 0;
						do
						{
							if (combatBatchMoveUI.BatchMoveUIActions[index].DesiredUI == null && combatBatchMoveUI.BatchMoveUIActions[index].UIName != "")
							{
								int arrayindex = CombatUI_List.FindIndex(x => x.UIName == combatBatchMoveUI.BatchMoveUIActions[index].UIName);
								if (arrayindex != -1)
								{
									if (ApplyUIMovementAction(CombatUI_List[arrayindex], combatBatchMoveUI.BatchMoveUIActions[index].XPos, combatBatchMoveUI.BatchMoveUIActions[index].YPos,
										combatBatchMoveUI.BatchMoveUIActions[index].Interpolation))
									{
										combatBatchMoveUI.BatchMoveUIActions.RemoveAt(index);
									}
									else
									{
										//Don't remove so inc
										index++;
									}
								}
							}
							else if (ApplyUIMovementAction(
								combatBatchMoveUI.BatchMoveUIActions[index].DesiredUI,
								combatBatchMoveUI.BatchMoveUIActions[index].XPos, combatBatchMoveUI.BatchMoveUIActions[index].YPos,
								combatBatchMoveUI.BatchMoveUIActions[index].Interpolation))
							{
								//We can remove it. As it is done.
								combatBatchMoveUI.BatchMoveUIActions.RemoveAt(index);
							}
							else
							{
								//We finished moving for this frame. But we still have more. Do not remove it!
								index++;
							}
						} while (index < combatBatchMoveUI.BatchMoveUIActions.Count);
					}
					else
					{
						combatActions_Queue.RemoveFirst();
						retb = true;
					}
					break;
				case CombatMoveUIAction combatMoveUIAction:

					if (combatMoveUIAction.DesiredUI == null && combatMoveUIAction.UIName != "")
					{
						int index = CombatUI_List.FindIndex(x => x.UIName == combatMoveUIAction.UIName);
						if (index != -1)
						{
							if (ApplyUIMovementAction(CombatUI_List[index], combatMoveUIAction.XPos, combatMoveUIAction.YPos,
								combatMoveUIAction.Interpolation))
							{
								combatActions_Queue.RemoveFirst();
								retb = true;
							}
						}
					}
					else if (ApplyUIMovementAction(combatMoveUIAction.DesiredUI, combatMoveUIAction.XPos, combatMoveUIAction.YPos,
						combatMoveUIAction.Interpolation))
					{
						combatActions_Queue.RemoveFirst();
						retb = true;
					}
					break;
				case CombatKillUIAction combatKillUIAction:

					if (combatKillUIAction.DesiredUI == null && combatKillUIAction.UIName != "")
					{
						int index = CombatUI_List.FindIndex(x => x.UIName == combatKillUIAction.UIName);
						if (index != -1)
						{
							ApplyKillUIAction(CombatUI_List[index]);
							combatActions_Queue.RemoveFirst();
							retb = true;
						}
					}
					else
					{
						ApplyKillUIAction(combatKillUIAction.DesiredUI);
						combatActions_Queue.RemoveFirst();
						retb = true;
					}

					break;

				#endregion

				case null:
					break;

				case CombatDamageAction combatDamageAction:
					ApplyDamage(combatDamageAction.DamageToApply_Dict);
					combatActions_Queue.RemoveFirst();
					retb = true;
					break;

				case CombatStatChangeAction combatStatChangeAction:
					QueueStatChange(combatStatChangeAction.StatChangeToApply, combatStatChangeAction.StatChangeRequest, combatStatChangeAction.turnCount);
					combatActions_Queue.RemoveFirst();
					retb = true;
					break;

				case CombatBGMAction combatBgmAction:

					break;
				
				case CombatCutsceneAction combatCutsceneAction:

					break;
				
				case CombatProjectileSystemAction combatProjectileSystemAction:

					break;
				case CombatSoundEffectAction combatSoundEffectAction:

					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}
			return retb;
		}

		public void QueueStatChange(Dictionary<BattleEntity, BaseStats> entities, BaseStats statType, int turnCount)
		{
			foreach (BattleEntity entity in entities.Keys)
			{
				entity.StatChange_List.Add(new BaseStatChange() {StatToChange = entities[entity], LengthInTurns = turnCount});
			}
		}

		/// <summary>
		/// This method will apply the character stat changes by adding it to the entity list.
		/// </summary>
		/// <param name="entities"></param>
		public void ApplyStatChange(BattleEntity entity, BaseStatChange baseStatChange)
		{
			if(baseStatChange.bIsActive) return;

			if (baseStatChange.StatToChange.Max_Health != 0)
				entity.MaxHealth += baseStatChange.StatToChange.Max_Health;
			if (baseStatChange.StatToChange.Max_Mana != 0)
				entity.MaxMana += baseStatChange.StatToChange.Max_Mana;
			if (baseStatChange.StatToChange.Attack != 0)
				entity.Stats.Attack += baseStatChange.StatToChange.Attack;
			if (baseStatChange.StatToChange.Defense != 0)
				entity.Stats.Defense += baseStatChange.StatToChange.Defense;
			if (baseStatChange.StatToChange.Dexterity != 0)
				entity.Stats.Dexterity += baseStatChange.StatToChange.Dexterity;
			if (baseStatChange.StatToChange.Agility != 0)
				entity.Stats.Agility += baseStatChange.StatToChange.Agility;
			if (baseStatChange.StatToChange.Morality != 0)
				entity.Stats.Morality += baseStatChange.StatToChange.Morality;
			if (baseStatChange.StatToChange.Wisdom != 0)
				entity.Stats.Wisdom += baseStatChange.StatToChange.Wisdom;
			if (baseStatChange.StatToChange.Resistance != 0)
				entity.Stats.Resistance += baseStatChange.StatToChange.Resistance;
			if (baseStatChange.StatToChange.Luck != 0)
				entity.Stats.Luck += baseStatChange.StatToChange.Luck;
			if (baseStatChange.StatToChange.Risk != 0)
				entity.Stats.Risk += baseStatChange.StatToChange.Risk;
			if (baseStatChange.StatToChange.Intelligence != 0)
				entity.Stats.Intelligence += baseStatChange.StatToChange.Intelligence;
			
			baseStatChange.bIsActive = true;
		}

		public void DeApplyStatChange(BattleEntity entity, BaseStatChange baseStatChange)
		{
			if (baseStatChange==null ||  !baseStatChange.bIsActive) return;

			if (baseStatChange.StatToChange.Max_Health != 0)
				entity.MaxHealth -= baseStatChange.StatToChange.Max_Health;
			if (baseStatChange.StatToChange.Max_Mana != 0)
				entity.MaxMana -= baseStatChange.StatToChange.Max_Mana;
			if (baseStatChange.StatToChange.Attack != 0)
				entity.Stats.Attack -= baseStatChange.StatToChange.Attack;
			if (baseStatChange.StatToChange.Defense != 0)
				entity.Stats.Defense -= baseStatChange.StatToChange.Defense;
			if (baseStatChange.StatToChange.Dexterity != 0)
				entity.Stats.Dexterity -= baseStatChange.StatToChange.Dexterity;
			if (baseStatChange.StatToChange.Agility != 0)
				entity.Stats.Agility -= baseStatChange.StatToChange.Agility;
			if (baseStatChange.StatToChange.Morality != 0)
				entity.Stats.Morality -= baseStatChange.StatToChange.Morality;
			if (baseStatChange.StatToChange.Wisdom != 0)
				entity.Stats.Wisdom -= baseStatChange.StatToChange.Wisdom;
			if (baseStatChange.StatToChange.Resistance != 0)
				entity.Stats.Resistance -= baseStatChange.StatToChange.Resistance;
			if (baseStatChange.StatToChange.Luck != 0)
				entity.Stats.Luck -= baseStatChange.StatToChange.Luck;
			if (baseStatChange.StatToChange.Risk != 0)
				entity.Stats.Risk -= baseStatChange.StatToChange.Risk;
			if (baseStatChange.StatToChange.Intelligence != 0)
				entity.Stats.Intelligence -= baseStatChange.StatToChange.Intelligence;
			
			baseStatChange.bIsActive = true;
		}

		public void ApplyDamage(Dictionary<BattleEntity, float> entities)
		{
			for (int i = 0; i < entities.Count; i++)
			{
				
				if (entities.Keys.ToList()[i] is Enemy em)
				{

					//We have gotten here then we know that the attack modifiers have been rolled. So we need to handle them
					foreach (Tuple<BattleEntity, ModifierData> tup in EntityAttackTraitModifier_List)
					{
						//Apply the modifiers to current character if there are any
						List<object> paramList = new List<object>();
						paramList.Add(tup.Item1); //instigator
						paramList.Add(em); //target
						paramList.Add(tup.Item2); //target
						
						//Get the Current Skill that we are going to use FUNCTION NAME
						String funcname = tup.Item2.Function_PTR;
						MethodInfo SkillMethod = Type.GetType("CombatSystem.Components.Combat.CombatDelegates.ModifierDelegates").GetMethod(funcname);
						SkillMethod.Invoke(null, new object[] { this, paramList }); //Invoke this method 
					}
					foreach (Tuple<BattleEntity, ModifierData> tup in EntityAttackEffectModifier_List)
					{
						//Apply the modifiers to current character if there are any
						List<object> paramList = new List<object>();
						paramList.Add(tup.Item1); //instigator
						paramList.Add(em); //target
						paramList.Add(tup.Item2); //target

						//Get the Current Skill that we are going to use FUNCTION NAME
						String funcname = tup.Item2.Function_PTR;
						MethodInfo SkillMethod = Type.GetType("CombatSystem.Components.Combat.CombatDelegates.ModifierDelegates").GetMethod(funcname);
						SkillMethod.Invoke(null, new object[] { this, paramList }); //Invoke this method 
					}


					em.CurrentHealth -= (int)entities[em];
					if (em.CurrentHealth == 0)
					{
						em.bIsDead = true;
						em.GetSpriteSheet().ChangeAnimation("Dead_Left");
					}
					em.ShaderHitFlash = 2;
					em.ShaderHitTimer = 6;
					em.ShaderHitActuatTimer = 6;
				}
				else if (entities.Keys.ToList()[i] is PartyMember partyMember)
				{
					partyMember.CurrentHealth -= (int)entities[partyMember];
					if (partyMember.CurrentHealth == 0)
						partyMember.bIsDead = true;

					partyMember.ShaderHitFlash = 2;
					partyMember.ShaderHitTimer = 6;
					partyMember.ShaderHitActuatTimer = 6;

				}

			}
			
			//int j = CombatParticleEmitters_List.FindIndex(x => x is PBoxEmitter);
			//if(j >= 0)
			//	CombatParticleEmitters_List[j].StopCyclying();
		}

		public void QueueBoxParticleSystem(Texture2D texture, Vector2 spawnpos, Color colortint, float sx, float sy, bool bCycle, PBoxEmitter newEmitter = null)
		{
			//Find a DEAD system and reuse it.
			int i = 0;
			i = CombatParticleEmitters_List.FindIndex(x => x.IsActive() == false && x is PBoxEmitter);

			if (i == -1)
			{
				if (newEmitter == null)
				{
					throw new NotImplementedException();
				}
				//There is no DEAD PBoxEmitter found. So we need to add the one we created before this call.
				CombatParticleEmitters_List.Add(newEmitter);
				CombatParticleEmitters_List.Last().SetPosition(spawnpos);
				CombatParticleEmitters_List.Last().SetColorTint(colortint);
				CombatParticleEmitters_List.Last().SetCycleStatus(bCycle);
				CombatParticleEmitters_List.Last().Start();
			}
			else
			{
				//LoadBoxParticleSystem(newEmitter.GetEmitterBounds(), newEmitter.emitterMoveRate, newEmitter.GetScreenBounds(), texture, i, sx,sy );
				CombatParticleEmitters_List[i] = newEmitter;
				CombatParticleEmitters_List[i].SetPosition(spawnpos);
				CombatParticleEmitters_List[i].SetColorTint(colortint);
				CombatParticleEmitters_List[i].SetCycleStatus(bCycle);
				CombatParticleEmitters_List[i].SetParticleScale(sx, sy);
				CombatParticleEmitters_List[i].Start();
			}

		}

		public void QueueHitSparkParticleSystem(Texture2D texture, Vector2 spawnpos, Color colortint, float sx, float sy, bool bCycle,
			HitSparkPEmitter newEmitter = null)
		{
			int i = 0;
			i = CombatParticleEmitters_List.FindIndex(x => x.IsActive() == false && x is HitSparkPEmitter);
			if (i == -1)
			{
				//No Emitter is dead so we need to make a new one/use the one we created before this.
				if (newEmitter == null)
				{
					//We will need to create a default setting Hit Spark Particle System and use that. Since we were not given one
					CombatParticleEmitters_List.Add(new HitSparkPEmitter(
					new Rectangle(0,0,1920, 1080), null, new Rectangle(0,0,texture.Width, texture.Height),
					15, 0, 15, false, texture, texture.Width, texture.Height, 1.0f, sx, sy,
					false, 0,0, 0, 0, 0, 0,0,0, new Random() //the Zeros are defined in the object.
					));
					CombatParticleEmitters_List.Last().SetPosition(spawnpos);
					CombatParticleEmitters_List.Last().SetColorTint(colortint);
					CombatParticleEmitters_List.Last().SetCycleStatus(bCycle);
					CombatParticleEmitters_List.Last().Start();
				}
				else
				{
					CombatParticleEmitters_List.Add(newEmitter);
					CombatParticleEmitters_List.Last().SetPosition(spawnpos);
					CombatParticleEmitters_List.Last().SetColorTint(colortint);
					CombatParticleEmitters_List.Last().SetCycleStatus(bCycle);
					CombatParticleEmitters_List.Last().Start();
				}
			}
			else
			{
				LoadHitSparkParticleSystem(CombatParticleEmitters_List[i].GetScreenBounds(), texture, i, sx,sy, bCycle); 
				CombatParticleEmitters_List[i].SetPosition(spawnpos);
				CombatParticleEmitters_List[i].SetColorTint(colortint);
				CombatParticleEmitters_List[i].SetParticleScale(sx, sy);
				CombatParticleEmitters_List[i].Start();

			}
		}

		public void QueueGenericParticleSystem(Texture2D texture, Vector2 spawnpos, Color colortint, float sx, float sy, bool bCycle,
			PEmitter newEmitter = null)
		{
			int i = 0;
			i = CombatParticleEmitters_List.FindIndex(x => x.IsActive() == false && !(x is HitSparkPEmitter) && !(x is PBoxEmitter));
			if (i == -1)
			{
				//No Emitter is dead so we need to make a new one/use the one we created before this.
				if (newEmitter == null)
				{
					//We will need to create a default setting Hit Spark Particle System and use that. Since we were not given one
					CombatParticleEmitters_List.Add(new PEmitter(
						new Rectangle(0, 0, 1920, 1080), null, new Rectangle(0, 0, texture.Width, texture.Height),
						100, 10, 10, true, texture, texture.Width, texture.Height, 1.0f, sx, sy,
						false, 0, 0, 0, 0, 0, 0, 0, 0, new Random() //the Zeros are defined in the object.
					));
					CombatParticleEmitters_List.Last().SetPosition(spawnpos);
					CombatParticleEmitters_List.Last().SetColorTint(colortint);
					CombatParticleEmitters_List.Last().SetCycleStatus(bCycle);
					CombatParticleEmitters_List.Last().Start();
				}
				else
				{
					CombatParticleEmitters_List.Add(newEmitter);
					CombatParticleEmitters_List.Last().SetPosition(spawnpos);
					CombatParticleEmitters_List.Last().SetColorTint(colortint);
					CombatParticleEmitters_List.Last().SetCycleStatus(bCycle);
					CombatParticleEmitters_List.Last().Start();
				}
			}
			else
			{
				//LoadHitSparkParticleSystem(CombatParticleEmitters_List[i].GetScreenBounds(), texture, i, sx, sy);

				CombatParticleEmitters_List[i] = newEmitter;
				CombatParticleEmitters_List[i].SetPosition(spawnpos);
				CombatParticleEmitters_List[i].SetColorTint(colortint);
				CombatParticleEmitters_List[i].SetCycleStatus(bCycle);
				CombatParticleEmitters_List[i].Start();

			}
		}

		/// <summary>
			/// Change the HitSpark Particle system properties.
			/// </summary>
			/// <param name="texture">New Texture to load. IF NULL then don't change</param>
			/// <param name="spawnpos">new position to place the Particle System</param>
			/// <param name="colortint">Color to apply on all the textures spawning from the particle system</param>
		//	public void QueueHitSparkParticleSystem(Texture2D texture, Vector2 spawnpos, Color colortint, float sx, float sy, HitSparkPEmitter newEmitter = null)
		//{

		//	//
		//	//Explosion/Hit Spark
		//	//HitSparkParticleSystem = new PEmitter(
		//	//	new Rectangle(0, 0, ScreenResWidth, ScreenResHeight),
		//	//	particleImg, new Rectangle(0, 00, particleImg.Width, particleImg.Height), 10,
		//	//	0, 10, false,
		//	//	particleImg, (int)(particleImg.Width), (int)(particleImg.Height), 1f,
		//	//	.1f, .1f,
		//	//	true,
		//	//	0, 0,
		//	//	0, 360,
		//	//	400f, 500,
		//	//	100, 200, new Random()
		//	//);

		//	//Find a DEAD system and reuse it.
		//	int i = 0;
		//	if(newEmitter != null)
		//		i = CombatParticleEmitters_List.FindIndex(x => x.IsActive() == false &&  x.GetType() == newEmitter?.GetType());
		//	else
		//		i = CombatParticleEmitters_List.FindIndex(x => x.IsActive() == false && !(x is PBoxEmitter));
		//	if (i == -1)
		//	{
		//		if (newEmitter != null)
		//		{
		//			CombatParticleEmitters_List.Add(newEmitter);
		//			CombatParticleEmitters_List.Last().SetPosition(spawnpos);
		//			CombatParticleEmitters_List.Last().SetColorTint(colortint);
		//			CombatParticleEmitters_List.Last().Start();
		//		}
		//		else
		//		{
		//			//no emitter found when needed so activate the factory.
		//			LoadHitSparkParticleSystem(1920, 1080, texture, sx, sy);
		//			CombatParticleEmitters_List.Last().SetPosition(spawnpos);
		//			CombatParticleEmitters_List.Last().SetColorTint(colortint);
		//			CombatParticleEmitters_List.Last().Start();

		//			//throw new NotImplementedException();
		//		}
		//	}
		//	else if (CombatParticleEmitters_List[i] is PBoxEmitter Pbe)
		//	{
		//		Pbe.SetNewEmitterBounds((newEmitter as PBoxEmitter).GetEmitterBounds());
		//		Pbe.SetScreenBounds(newEmitter.GetScreenBounds());

		//		if (texture != null)
		//			CombatParticleEmitters_List[i].SetParticleTexture(texture);
		//		CombatParticleEmitters_List[i].SetPosition(spawnpos);
		//		CombatParticleEmitters_List[i].SetColorTint(colortint);

		//		CombatParticleEmitters_List[i].Start(); // the data has been loaded. So we need to start it.
		//	}
		//	else if (CombatParticleEmitters_List[i] is PEmitter Pe)
		//	{
		//		if (texture != null)
		//			CombatParticleEmitters_List[i].SetParticleTexture(texture);

		//		//newEmitter?? = Pe.SetScreenBounds(newEmitter?.GetScreenBounds());
		//		CombatParticleEmitters_List[i].SetPosition(spawnpos);
		//		CombatParticleEmitters_List[i].SetColorTint(colortint);

		//		CombatParticleEmitters_List[i].Start(); // the data has been loaded. So we need to start it.
		//	}



			//if (i == -1) //No system is dead and can be reused at this time. So we need to create a new one.
				//{
				//	if (newEmitter != null)
				//	{
				//		CombatParticleEmitters_List.Add(newEmitter);
				//		CombatParticleEmitters_List.Last().SetPosition(spawnpos);
				//		CombatParticleEmitters_List.Last().SetColorTint(colortint);
				//		CombatParticleEmitters_List.Last().Start();
				//	}
				//	else
				//	{
				//		//no emitter found when needed so activate the factory.
				//		LoadHitSparkParticleSystem(1920, 1080, texture, sx, sy);
				//		CombatParticleEmitters_List.Last().SetPosition(spawnpos);
				//		CombatParticleEmitters_List.Last().SetColorTint(colortint);
				//		CombatParticleEmitters_List.Last().Start();

				//	//throw new NotImplementedException();
				//	}
				//}
				//else // Found a dead system that can be reused.
				//{
				//	if (CombatParticleEmitters_List[i] is PBoxEmitter cbe)
				//	{
				//		cbe.SetNewEmitterBounds((newEmitter as PBoxEmitter).GetEmitterBounds());
				//	}

				//	if (texture != null)
				//		CombatParticleEmitters_List[i].SetParticleTexture(texture);
				//	CombatParticleEmitters_List[i].SetPosition(spawnpos);
				//	CombatParticleEmitters_List[i].SetColorTint(colortint);

				//	CombatParticleEmitters_List[i].Start(); // the data has been loaded. So we need to start it.

				//}

		//}

		public int GetEnemyIndex()
		{
			return currentEnemyIndex;
		}
		
		public void Update(GameTime gameTime)
		{

			KeyboardState keyboardState = Keyboard.GetState();

			#region Tests

			
			if (keyboardState.IsKeyDown(Keys.Q))
				Console.WriteLine("Inner Button Press");
			if (keyboardState.IsKeyDown(Keys.M))
			{
				//PartyMembers.Values.ToList()[1].Position.X += 100;
				PartyMembers.Values.ToList()[1].SetPosition((int)(PartyMembers.Values.ToList()[1].Position.X + 100), -1); 
				Thread.Sleep(100);
				//map = C_map;
				//map.LoadTileMaps(this.GraphicsDevice, map.level);
				//map.LoadSprites(this.GraphicsDevice, map.level);
				//map.GenerateLevel(map.level, this.GraphicsDevice, spriteBatch);
				//System.Console.WriteLine("M DOWN");
			}
			if (keyboardState.IsKeyDown(Keys.N))
			{
				PartyMembers.Values.ToList()[1].SetPosition((int)(PartyMembers.Values.ToList()[1].Position.X - 100), -1);
				Thread.Sleep(100);
			}

			if (keyboardState.IsKeyDown(Keys.V))
			{
				//gpb.decrementBar(5);
				PartyMembers["Pete"].CurrentHealth -= 10;
				if (PartyMembers["Pete"].CurrentHealth <= 0)
				{
					PartyMembers["Pete"].GetSpriteSheet().ChangeAnimation("Dead_Right");
				}
			}

			if (keyboardState.IsKeyDown(Keys.B))
			{
				//gpb.IncrementBar(5);
				PartyMembers["Pete"].CurrentHealth += 20;
				if (PartyMembers["Pete"].CurrentHealth > 0 && PartyMembers["Pete"].GetSpriteSheet().CurrentAnimation.Name == "Dead_Right")
				{
					PartyMembers["Pete"].GetSpriteSheet().ChangeAnimation("Idle_Right");
				}
			}
			if (keyboardState.IsKeyDown(Keys.Z))
			{
				//QueueMoveCharacter(PartyMembers.Values.ToList()[1], (int)(1820 * .45f) - (PartyMembers.Values.ToList()[1].Width / 2), (int)PartyMembers.Values.ToList()[1].Position.Y, true);
				//QueueMoveCharacter(PartyMembers.Values.ToList()[1], (int)(1820 * .9) - (PartyMembers.Values.ToList()[1].Width / 2), (int)PartyMembers.Values.ToList()[1].Position.Y, true);
				Thread.Sleep(100); //this is here to avoid multiple calls. Since I a human usually press a button for more than one frame... like a bitch i know.
			}
			if (keyboardState.IsKeyDown(Keys.LeftAlt))
			{
				QueueCombatAction(new CombatSpawnUIAction(this, 
					new BixBite.Rendering.UI.Image.GameImage("TestImage", 0,0, 50,50 , 1, false, 0,0, "", "#00000000", ContentRef.Load<Texture2D>("Images/TestCombatSprites/bernieBESTGIRL") ),
					-1000,1000));
				Thread.Sleep(100); //this is here to avoid multiple calls. Since I a human usually press a button for more than one frame... like a bitch i know.
			}
			if (keyboardState.IsKeyDown(Keys.RightAlt))
			{
				QueueCombatAction(new CombatKillUIAction(this, CombatUI_List.Last()));
				Thread.Sleep(100); //this is here to avoid multiple calls. Since I a human usually press a button for more than one frame... like a bitch i know.
			}

			if (keyboardState.IsKeyDown(Keys.RightControl))
			{
				QueueCombatAction(new CombatMoveUIAction(this, CombatUI_List.Last(), 500, 500, 5, false));
				//Thread.Sleep(100); //this is here to avoid multiple calls. Since I a human usually press a button for more than one frame... like a bitch i know.
			}
		

			if (Mouse.GetState() is MouseState mouse && mouse.LeftButton == ButtonState.Pressed)
			{


			}

			#endregion

			#region DEBUGGING

			if (keyboardState.IsKeyDown(Keys.OemTilde))
			{
				((GameTextBlock)debug_LogBlock).SetProperty("Width", 1000);
				((GameTextBlock)debug_LogBlock).SetProperty("Height", 30);

				Console.WriteLine("Dubug Log activate");
			}

			#endregion

			foreach (PartyMember p in PartyMembers.Values)
			{
				p.Update(gameTime);
			}

			foreach (enemy e in BattleEnemyList.Values)
			{
				e.Update(gameTime);
			}

			foreach (PEmitter pe in CombatParticleEmitters_List)
			{
				if (!pe.IsActive())
					continue;
				pe.Update(gameTime.ElapsedGameTime.Milliseconds);
			}

			foreach (BaseUI baseui in CombatUI_List)
			{
				if (!baseui.bIsActive)
					continue;
				if (baseui is BixBite.Rendering.UI.Image.GameImage b1)
					b1.Update(gameTime);
				else if (baseui is BixBite.Rendering.UI.ListBox.GameListBox b2)
					b2.Update(gameTime);
				else if (baseui is BixBite.Rendering.UI.TextBlock.GameTextBlock b3)
					b3.Update(gameTime);
				else if (baseui is BixBite.Rendering.UI.TextBlock.GameTextBlock b4)
					b4.Update(gameTime);
				else if (baseui is BixBite.Rendering.UI.ProgressBar.GameProgressBar b5)
					b5.Update(gameTime);
				else if (baseui is BixBite.Rendering.UI.Button.GameButton b6)
					b6.Update(gameTime);
			}

			//Handle queued up combat actions!
			if (combatActions_Queue.Count > 0)
			{
				LinkedListNode<CombatActions> desiredAction = combatActions_Queue.First;
				bool retval = HandleCombatActionEvents(combatActions_Queue.First, (int)gameTime.ElapsedGameTime.Milliseconds);

				//Should we continue to apply handles?
				if (!retval)
				{
					if (desiredAction.Value is CombatDelayAction)
						return;
					if (desiredAction.Value is CombatMoveAction cma)
					{
						if (cma.bQueuePriority) return;
					}
					else if (desiredAction.Value is CombatBatchMoveAction cma_batch)
					{
						if (cma_batch.bQueuePriority) return;
					}
					else if (desiredAction.Value is CombatMoveUIAction cma_ui)
					{
						if (cma_ui.bQueuePriority) return;
					}
				}


				//At this point it is important to NOTE: that the first node might not be the same. Use retval chack.
				if (combatActions_Queue.Count > 0)
				{
EventSkipOver:
					//Recalc the first
					if (!retval)
						desiredAction = desiredAction.Next;
					else desiredAction = combatActions_Queue.First;

					if (desiredAction == null) return;
					
					//If we come across a delay then we gotta refresh the frame!
					if (desiredAction.Value is CombatDelayAction)
						return;
					else if(desiredAction.Value is CombatMoveAction cma)
					{
						retval = HandleCombatActionEvents(desiredAction, (int)gameTime.ElapsedGameTime.Milliseconds);
						if (!cma.bQueuePriority) goto EventSkipOver;
					}
					else if (desiredAction.Value is CombatBatchMoveAction cma_batch)
					{
						retval = HandleCombatActionEvents(desiredAction, (int)gameTime.ElapsedGameTime.Milliseconds);
						if (!cma_batch.bQueuePriority) goto EventSkipOver;
					}
					else if (desiredAction.Value is CombatMoveUIAction cma_ui)
					{
						retval = HandleCombatActionEvents(desiredAction, (int)gameTime.ElapsedGameTime.Milliseconds);
						if (!cma_ui.bQueuePriority) goto EventSkipOver;
					}
				}
				
			}
			//Do we need to add a stat?
			foreach (BaseStatChange bsc in CurrentTurnCharacter.StatChange_List)
			{
				ApplyStatChange(CurrentTurnCharacter, bsc);
			}

			#region StateMachine
			CommandRequest = EBattleCommand.NONE;
			Keys pressedkey; // = Keyboard.GetState().GetPressedKeys()[0];
			switch (combatState)
			{
				case ECombatState.NONE:
					break;
				case ECombatState.BattleStart:

					pastCombatState = ECombatState.NONE;
					//spawn both sides of characters to the screen. For now they just pop in. Later ill make them fly and fade in

					//To Spawn them in we need to decide which character the player is using.
					//TODO: This is spawned at a STATIC position. the equation is not in this file yet.
					//CurrentEnemiesNames = new List<string>() { "Slime", "Slime_s", "Bunny", "Bunny_s" };
					//Set the stats of each fighting entity. So health, mana, essence, ETC

					//Set the start of the turn
					CombatState = ECombatState.StartTurn;
					//CurrentTurnCharacter = PartyMembers["Emma"];



					//This is the start of the battle so we need to figure which good and bad guys to spawn.

					//Which Good guys are party members?
					//CurrentPartyMembersNames = new List<String>() { "Emma", "Pete", "Liam", "PC_Girl" }; //spawn via update function
					CurrentPartyMembersNames = new List<String>() { "PC_Girl", "Emma", "Liam", "Pete" }; //spawn via update function

					//which bad guys do i spawn from the pool?
					CurrentEnemiesNames = new List<string>() {"Slime", "Slime_s", "Bunny_s"}; //, "Slime_s", "Bunny", "Bunny_s" }; //spawn


					//Before we checked the modifiers. Now we need to check the equipment for stat increases
					foreach (PartyMember pm in PartyMembers.Values)
					{
						foreach (Equipable equipable in pm.GetEquipment())
						{
							if (equipable is null || equipable is Weapon) continue;
							pm.StatChange_List.Add(new BaseStatChange()
							{
								LengthInTurns = 10000,
								bIsActive = false,
								StatToChange = equipable.Stats
							});
							ApplyStatChange(pm, pm.StatChange_List.Last());
						}
					}
					foreach (Enemy em in BattleEnemyList.Values)
					{
						foreach (Equipable equipable in em.GetEquipment())
						{
							if (equipable is null || equipable is Weapon) continue;
							em.StatChange_List.Add(new BaseStatChange()
							{
								LengthInTurns = 10000,
								bIsActive = false,
								StatToChange = equipable.Stats
							});
							ApplyStatChange(em, em.StatChange_List.Last());
						}
					}
					//State => StartTurn
					combatState = ECombatState.StartTurn;

					

					break;
				case ECombatState.BattleEnd:
					break;
				case ECombatState.StartTurn:
					//Beginning of the WHOLE turn so we need to figure out the order.
					FillTurnQueue();
					//Use the Stats of the spawned characters to determine the queue order.
					CurrentTurnCharacter = TurnQueue.Dequeue();
					//state => StartEntityTurn
					combatState = ECombatState.StartEntityTurn;
					break;
				case ECombatState.EndTurn:

					//We have reached the end of the turn. We can now check for follow up attacks.
					combatState = ECombatState.StartFollowupAttacks;

					//Reset Turn Queue!
					FillTurnQueue();

					//State => StartTurn
					//CombatState = ECombatState.StartTurn;

					break;
				case ECombatState.StartEntityTurn:
					//This is the start of an Entities turn (good or bad)
					//Remove stat changes if we need too.
					//TODO: make this a do while
					for (int i = 0; i < CurrentTurnCharacter.StatusEffect_List.Count; i++)
					{

						//TODO: find the type of status effect type. For now i assume burning
						if (CurrentTurnCharacter.StatusEffect_List[i].LengthInTurns != 0)
						{
							CurrentTurnCharacter.Stats.Current_Health -= (int)CurrentTurnCharacter.StatusEffect_List[i].StatToChange.Current_Health;
							CurrentTurnCharacter.StatusEffect_List[i].DecrementTurnCounter();
							//Move enemy back to simulate a hit
							QueueCombatAction(new CombatMoveAction(this, (CurrentTurnCharacter), (int)CurrentTurnCharacter.SpawnPosition.X + 50, (int)CurrentTurnCharacter.SpawnPosition.Y - 10, 10, true));

							//Get the Hit spawn Position of the CurrentTurnCharacter. This skill ONLY hits one.
							Vector2 TargetPosition = new Vector2(CurrentTurnCharacter.Position.X, CurrentTurnCharacter.Position.Y);
							TargetPosition.X += (float)((CurrentTurnCharacter.Width * CurrentTurnCharacter.ScaleX) / 2.0);
							TargetPosition.Y += (float)((CurrentTurnCharacter.Height * CurrentTurnCharacter.ScaleY) / 2.0);

							QueueCombatAction(new CombatHitSparkAction(this, this.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle"),
									TargetPosition - new Vector2(50, 50), Color.Red, false)
								{ scalarX = .25f, ScalarY = .25f });

							//move the character back to their past position.
							QueueCombatAction(new CombatMoveAction(this, CurrentTurnCharacter, (int)CurrentTurnCharacter.SpawnPosition.X, (int)CurrentTurnCharacter.SpawnPosition.Y, 10, true));
							QueueCombatAction(new CombatDelayAction(this, 500)); //wait before setting hit events
							//bMovementInProgress = true; //Do not allow delay UNTIL movement is done.
						}
						else
						{
							CurrentTurnCharacter.StatusEffect_List.RemoveAt(i);
						}
					}


					if (CurrentTurnCharacter is Enemy enemyturn)
					{
						if (enemyturn.bIsDead)
							CombatState = ECombatState.EndEntityTurn;
						else
						{
							//TODO: Apply Status Effect Damage

							//Are they dead now?
							if (enemyturn.bIsDead)
								CombatState = ECombatState.EndEntityTurn;
							else	//They still aren't Dead they can take a turn.
							{
								//Should the enemy use an item?				TODO: write this helper

								//Should the enemy use a skill?				TODO: write this helper

								//should the enemy Block?							TODO: write this helper

								//Should the enemy attack normally?		TODO: write this helper
								combatState = ECombatState.StartPhysicalAttack;

							}
						}
					}
					if (CurrentTurnCharacter is PartyMember)
					{
						if (CurrentPartyMember_turn.bIsDead)
							CombatState = ECombatState.EndEntityTurn;
						else
						{
							//TODO: Apply Status effect damage
							if(CurrentPartyMember_turn.bIsDead)
								//Are they dead now?
								CombatState = ECombatState.EndEntityTurn;
							else
							{
								//Show and place the essence bar for this character!
								PartyMemberEssenceBar.XPos = (int)CurrentPartyMember_turn.GetCharacterStatUILocation().X - 95;
								PartyMemberEssenceBar.YPos = (int)CurrentPartyMember_turn.GetCharacterStatUILocation().Y;
								PartyMemberEssenceBar.bIsActive = true;

								//move the player forward to show whose turn it is!
								//TODO: Remember it doesn't always have to be to the right! If they are ambused it can be to left OR right!
								//QueueCombatAction(new CombatAnimationAction(this, CurrentPartyMember_turn, "WalkRight"));

								//TODO: Make this work with layered animation after i make it.
								String requestedWalk = String.Format("{0}_{1}", "WalkRight", Enum.GetName(typeof(EWeaponType), (EWeaponType)CurrentTurnCharacter.CurrentWeapon.Value.Weapon_Type));
								QueueCombatAction(new CombatAnimationAction(this, CurrentPartyMember_turn, requestedWalk));
								QueueCombatAction(new CombatMoveAction(this, CurrentPartyMember_turn, (int)((1820 * .45f) - (CurrentPartyMember_turn.Width / 2.0f)), (int)CurrentPartyMember_turn.Position.Y, 20, true));
								
								CombatState = ECombatState.WaitingForInput; //set state.
							}
						}
					}


					//Is this person dead?
						//If so skip them and 
						//State => EndEntityTurn
					//Else they are alive
						//Apply status effect damage
							//If Dead skip 
								//State => EndEntityTurn
							//Else they are alive so
								//Set to current character
								//Move The character forward 
								//State => MainUISpawn

					//Does this entity have any modifiers we need to apply?
					CheckForEntityModifiers_Buff(CurrentTurnCharacter);
					CheckForEntityModifiers_Debuff(CurrentTurnCharacter);


					//Apply the modifiers to current character if there are any
					foreach (Tuple<BattleEntity, ModifierData> tup in EntityBuffTraitModifier_List)
					{
						List<object> paramList = new List<object>();
						paramList.Add(tup.Item1);
						paramList.Add(tup.Item2);
						//Get the Current Skill that we are going to use FUNCTION NAME
						String funcname = tup.Item2.Function_PTR;
						MethodInfo SkillMethod = Type.GetType("CombatSystem.Components.Combat.CombatDelegates.ModifierDelegates").GetMethod(funcname);
						SkillMethod.Invoke(null, new object[] { this, paramList }); //Invoke this method 
					}

					break;
				case ECombatState.EndEntityTurn:
					//Do we have to move?
					if(combatActions_Queue.Count > 0) return;
					
					
					//if Win state or Loss state found?
					//end battle(win/loss state)


					//Remove stat changes if we need too.
					for (int i = 0; i < CurrentTurnCharacter.StatChange_List.Count; i++)
					{
						if (CurrentTurnCharacter.StatChange_List[i].LengthInTurns == 0)
							DeApplyStatChange(CurrentTurnCharacter, CurrentTurnCharacter.StatChange_List[i]);
						else
							CurrentTurnCharacter.StatChange_List[i].DecrementTurnCounter();
					}

					//Is this the LAST character in the turn queue
					if (TurnQueue.Count == 0)
					{
						// State => FollowUpAttacks
						//TODO: State => FollowUpAttacks
						CombatState = ECombatState.EndTurn;
					}
					else
					{

						//TurnQueue increment
						TurnQueue_GameListBox.IncrementSelectedIndex();
						if (CurrentTurnCharacter is PartyMember && !followUpAttack_List.Any(x => x.Item1 == CurrentPartyMember_turn))
						{
							followUpAttack_List.Add(new Tuple<BattleEntity, EStanceType>(CurrentPartyMember_turn, CurrentPartyMember_turn.CurrentStance.Value));
						}

						//There are more characters in the queue.
						CurrentTurnCharacter = TurnQueue.Dequeue();
						pastCombatState = ECombatState.NONE;
						CombatState = ECombatState.StartEntityTurn;
					}

					//clear the modifiers for the next person.
					EntityAttackEffectModifier_List.Clear();
					EntityAttackTraitModifier_List.Clear();
					EntityDefenseEffectModifier_List.Clear();
					EntityDefenseTraitModifier_List.Clear();
					EntityBuffEffectModifier_List.Clear();
					EntityBuffTraitModifier_List.Clear();
					EntityDebuffEffectModifier_List.Clear();
					EntityDebuffTraitModifier_List.Clear();
					break;
				case ECombatState.WaitingForInput:

					if(combatActions_Queue.Count > 0) //Events still need to be handled
						return;

					//movement is done, so spawn the UI!
					// Run the animation that will be used to "show" the player options. So like "unfolding" etc.
					if (CurrentTurnCharacter is PartyMember)
					{
						//At this point we need to go back to IDLE!
						String currentIdle = String.Format("{0}_{1}", "IdleRight", Enum.GetName(typeof(EWeaponType), (EWeaponType)CurrentTurnCharacter.CurrentWeapon.Value.Weapon_Type));
						if(!CurrentTurnCharacter.GetSpriteSheet().CurrentAnimation.Name.Contains("Idle"))
							QueueCombatAction(new CombatAnimationAction(this, CurrentPartyMember_turn, currentIdle));

						//What weapon are we using?


						#region Attack Sticker UI
						//Load the position for the attack sticker. and get ready to display it.
						AttackSticker_UI_rect =
							new Rectangle((int)CurrentTurnCharacter.Position.X - 50 + AttackStickerXOffset_Izzy,
								(int)CurrentTurnCharacter.Position.Y - 50 + AttackStickerYOffset_Izzy, AttackStickerWidth_Izzy, AttackStickerHeight_izzy);

						if (AttacktSticker_UI_izzy != null)
							AttackSticker_UI = AttacktSticker_UI_izzy;
						else
							AttackSticker_UI = ContentRef.Load<Texture2D>("Images/UI/sticker_atk_small");
						bDrawAttackSticker_UI = true;
						#endregion

						#region Defense Sticker UI
						//Load the position for the defense sticker. and get ready to display it.
						DefenseSticker_UI_rect =
							new Rectangle((int)CurrentTurnCharacter.Position.X + 40 + DefenseStickerXOffset_Izzy,
								(int)CurrentTurnCharacter.Position.Y - 90 + DefenseStickerYOffset_Izzy, DefenseStickerWidth_Izzy, DefenseStickerHeight_izzy);

						//QUICK Binding debugs for IZZY
						if (DefensetSticker_UI_izzy != null)
							DefenseSticker_UI = DefensetSticker_UI_izzy;
						else
							DefenseSticker_UI = ContentRef.Load<Texture2D>("Images/UI/sticker_atk_small");
						bDrawDefenseSticker_UI = true;
						#endregion

						#region Skill Sticker UI

						//Load the position for the skills sticker. and get ready to display it.
						SkillsSticker_UI_rect =
							new Rectangle((int)CurrentTurnCharacter.Position.X + 130 + SkillsStickerXOffset_Izzy,
								(int)CurrentTurnCharacter.Position.Y - 50 + SkillsStickerYOffset_Izzy, SkillsStickerWidth_Izzy, SkillsStickerHeight_izzy);

						//Quick Bindings for Izzy
						if (SkillSticker_UI_izzy != null)
							SkillsSticker_UI = SkillSticker_UI_izzy;
						else
							SkillsSticker_UI = ContentRef.Load<Texture2D>("Images/UI/sticker_atk_small");

						bDrawSkillsSticker_UI = true;


						#endregion


						#region Weapon Left Arrow Sticker UI

						WeaponLeftArrow_UI_rect =
							new Rectangle((int)CurrentTurnCharacter.Position.X - 70 + LeftWeaponStickerXOffset_Izzy,
								 (int)CurrentTurnCharacter.Position.Y + (int)(CurrentTurnCharacter.Height * CurrentTurnCharacter.ScaleY) - 30 + LeftWeaponStickerYOffset_Izzy, LeftWeaponStickerWidth_Izzy, LeftWeaponStickerHeight_izzy);

						//Quick Bindings for Izzy
						if (WeaponLeftArrow_UI_izzy != null)
							WeaponLeftArrow_UI = WeaponLeftArrow_UI_izzy;
						else
							WeaponLeftArrow_UI = ContentRef.Load<Texture2D>("Images/UI/Arrow_Left");

						bDrawWeaponLeftArrow_UI = true;

						#endregion

						#region Stance Left Arrow Sticker UI

						StanceLeftArrow_UI_rect =
							new Rectangle((int)CurrentTurnCharacter.Position.X - 30 + LeftStanceStickerXOffset_Izzy,
								(int)CurrentTurnCharacter.Position.Y + (int)(CurrentTurnCharacter.Height * CurrentTurnCharacter.ScaleY) + 30 + LeftStanceStickerYOffset_Izzy, LeftStanceStickerWidth_Izzy, LeftStanceStickerHeight_izzy);

						//Quick Bindings for Izzy
						if (StanceLeftArrow_UI_izzy != null)
							StanceLeftArrow_UI = StanceLeftArrow_UI_izzy;
						else
							StanceLeftArrow_UI = ContentRef.Load<Texture2D>("Images/UI/Arrow_Left");

						bDrawStanceLeftArrow_UI = true;

						#endregion

						#region Weapon Right Arrow Sticker UI

						WeaponRightArrow_UI_rect =
							new Rectangle((int)CurrentTurnCharacter.Position.X + (int)(CurrentTurnCharacter.Width * CurrentTurnCharacter.ScaleX) + 30 + RightWeaponStickerXOffset_Izzy,
								(int)CurrentTurnCharacter.Position.Y + (int)(CurrentTurnCharacter.Height * CurrentTurnCharacter.ScaleY) - 30 + RightWeaponStickerYOffset_Izzy, RightWeaponStickerWidth_Izzy, RightWeaponStickerHeight_izzy);

						//Quick Bindings for Izzy
						if (WeaponRightArrow_UI_izzy != null)
							WeaponRightArrow_UI = WeaponRightArrow_UI_izzy;
						else
							WeaponRightArrow_UI = ContentRef.Load<Texture2D>("Images/UI/Arrow_Right");

						bDrawWeaponRightArrow_UI = true;

						#endregion

						#region Stance Right Arrow Sticker UI

						StanceRightArrow_UI_rect =
							new Rectangle((int)CurrentTurnCharacter.Position.X + (int)(CurrentTurnCharacter.Width * CurrentTurnCharacter.ScaleX) - 10 + RightStanceStickerXOffset_Izzy,
								(int)CurrentTurnCharacter.Position.Y + (int)(CurrentTurnCharacter.Height * CurrentTurnCharacter.ScaleY) + 30 + RightStanceStickerYOffset_Izzy, RightStanceStickerWidth_Izzy, RightStanceStickerHeight_izzy);

						//Quick Bindings for Izzy
						if (StanceRightArrow_UI_izzy != null)
							StanceRightArrow_UI = StanceRightArrow_UI_izzy;
						else
							StanceRightArrow_UI = ContentRef.Load<Texture2D>("Images/UI/Arrow_Right");

						bDrawStanceRightArrow_UI = true;

						#endregion


						#region Inventory Sticker UI
						//Load the position for the attack sticker. and get ready to display it.
						InventorySticker_UI_rect =
							new Rectangle((int)CurrentTurnCharacter.Position.X - 50 + InventoryStickerXOffset_Izzy,
								(int)CurrentTurnCharacter.Position.Y + 30 + InventoryStickerYOffset_Izzy, InventoryStickerWidth_Izzy, InventoryStickerHeight_izzy);

						if (InventorySticker_UI_izzy != null)
							InventorySticker_UI = InventorySticker_UI_izzy;
						else
							InventorySticker_UI = ContentRef.Load<Texture2D>("Images/UI/backpack");
						bDrawInventorySticker_UI = true;
						#endregion


						#region Weapon Card UI
						//Load the position for the attack sticker. and get ready to display it.
						WeaponCardSticker_UI_rect =
							new Rectangle((int)CurrentTurnCharacter.Position.X + 50 + WeaponCardStickerXOffset_Izzy,
								(int)CurrentTurnCharacter.Position.Y + CurrentTurnCharacter.Height - 140 + WeaponCardStickerYOffset_Izzy, WeaponCardStickerWidth_Izzy, WeaponCardStickerHeight_izzy);

						if (WeaponCardSticker_UI == null)
							WeaponCardSticker_UI = ContentRef.Load<Texture2D>("Images/UI/WeaponCards/weapons_cards_Default");
						bDrawWeaponCardSticker_UI = true;
						#endregion

						#region Weapon Label UI
						//Set up the testing textbox to switch on command
						
						Weapon_GTB.Position = WeaponLeftArrow_UI_rect.Location.ToVector2() + new Vector2(40, 20);
						Weapon_GTB.SetProperty("Width",
							WeaponRightArrow_UI_rect.X - WeaponLeftArrow_UI_rect.X - WeaponLeftArrow_UI.Width);
						Weapon_GTB.SetProperty("Height", 40);
						Weapon_GTB.Text = Enum.GetName(typeof(EWeaponType), CurrentPartyMember_turn.CurrentWeapon.Value.Weapon_Type);
						#endregion

						#region Stance Label UI
						//We should apply the stance to the text box
						Stance_GTB.Text = CurrentPartyMember_turn.CurrentStance.Value.ToString();

						Vector2 pos = StanceLeftArrow_UI_rect.Location.ToVector2() + new Vector2(StanceLeftArrow_UI.Width, 0); ;
						Stance_GTB.Position = pos;
						Stance_GTB.SetProperty("Width", (StanceRightArrow_UI_rect.X - StanceLeftArrow_UI_rect.Location.X - StanceLeftArrow_UI.Width));

						CurrentPartyMember_turn.StanceIndicator_Color = StanceToColor(CurrentPartyMember_turn.CurrentStance.Value);
						#endregion

					}

					pressedkey = Keys.None;
					//At this point the state machine cannot proceed without user input.
					if (keyboardState.GetPressedKeys().Length != 0)
					{
						if (keyboardState.IsKeyDown(keyboardState.GetPressedKeys()[0]) & prevKeyState.IsKeyUp(keyboardState.GetPressedKeys()[0]))
						{
							pressedkey = keyboardState.GetPressedKeys()[0]; //assume ONE KEY
							CommandRequest = GetInputCommand_Keys(pressedkey); //Is there a valid request?
						}
					}

					//These decisions are here for THE beginning of the characters turn
					if(pastCombatState == ECombatState.NONE)  // || pastCombatState == ECombatState.ItemsUISpawn || pastCombatState == ECombatState.SkillsUISpawn) {
					{
						switch (CommandRequest)
						{
							case EBattleCommand.NONE:
								break;
								//return;
							case EBattleCommand.WEAPON:
								ChangeWeapons(combatState, CommandRequest, pressedkey);

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Weapon Change Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());

								break;
							case EBattleCommand.CURSOR:
								MoveCursor_EnemySide(combatState, CommandRequest, pressedkey);

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Cursor Change Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());

								break;
							case EBattleCommand.ESSENCE:
								ApplyEssence(combatState, pressedkey);

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Use Essence Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());

								break;
							case EBattleCommand.STANCE:
								ApplyStance(combatState, CommandRequest, pressedkey);

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Change Stance Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());

								break;
							case EBattleCommand.ATTACK:
								//Attack is special there is no input UI that needs to spawn so we can save the past state.
								//saving the past state will allow the state machine to advance to choose target.
								//State => StartPhysicalAttack
								//CombatState = ECombatState.StartPhysicalAttack;

								//State => ChooseTarget
								pastCombatState = ECombatState.StartPhysicalAttack;
								CombatState = ECombatState.ChooseTarget;

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Attack UI Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());

								break;
							case EBattleCommand.SKILL:
								//We have asked to open the skills UI. so we need to change the UI
								CombatState = ECombatState.StartSkillsAttack;

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Skills UI Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());

								break;
							case EBattleCommand.ITEM:
								combatState = ECombatState.StartItemUse;

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Items UI Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());
								break;
							case EBattleCommand.DEFENSE:
								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Guard Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());
								break;
							case EBattleCommand.SELECT:
								//if (pastCombatState == ECombatState.SkillsUISpawn)
								//{
								//	CommandRequest = EBattleCommand.SKILL;
								//	CombatState = ECombatState.ChooseTarget;

								//	debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Skill Attack Request | Key: {1}",
								//		combatState.ToString(), pressedkey.ToString());

								//}
								//else if (pastCombatState == ECombatState.ItemsUISpawn)
								//{
								//	CommandRequest = EBattleCommand.ITEM;
								//	CombatState = ECombatState.ChooseTarget;

								//	debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Item Use Request | Key: {1}",
								//		combatState.ToString(), pressedkey.ToString());

								//}

								break;
							case EBattleCommand.BACK:
								//if (pastCombatState == ECombatState.SkillsUISpawn)
								//{


								//}
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
					else if (pastCombatState == ECombatState.SkillsUISpawn)
					{
						switch (CommandRequest)
						{
							//these don't matter for this current state. so ignore.
							case EBattleCommand.NONE:
								break;
							case EBattleCommand.WEAPON:
								break;
							case EBattleCommand.STANCE:
								ApplyStance(combatState, CommandRequest, pressedkey);

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Change Stance Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());
								break;
							case EBattleCommand.ATTACK:
							case EBattleCommand.SKILL:
							case EBattleCommand.ITEM:
							case EBattleCommand.DEFENSE:
							case EBattleCommand.SELECT:
								break;

							case EBattleCommand.MENU1:
								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Menu Option 1 Request | Key: {1}", combatState.ToString(), pressedkey.ToString());
								SetUpSkillMenuCommandParameters((CurrentPartyMember_turn), 0);
								break;
							case EBattleCommand.MENU2:
								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Menu Option 2 Request | Key: {1}", combatState.ToString(), pressedkey.ToString());
								SetUpSkillMenuCommandParameters((CurrentPartyMember_turn), 1);
								break;
							case EBattleCommand.MENU3:
								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Menu Option 3 Request | Key: {1}", combatState.ToString(), pressedkey.ToString());
								SetUpSkillMenuCommandParameters((CurrentPartyMember_turn), 2);
								break;
							case EBattleCommand.MENU4:
								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Menu Option 4 Request | Key: {1}", combatState.ToString(), pressedkey.ToString());
								SetUpSkillMenuCommandParameters((CurrentPartyMember_turn), 3);
								break;
							case EBattleCommand.MENU5:
								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Menu Option 5 Request | Key: {1}", combatState.ToString(), pressedkey.ToString());
								SetUpSkillMenuCommandParameters((CurrentPartyMember_turn), 4);
								break;
							case EBattleCommand.MENU6:
								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Menu Option 6 Request | Key: {1}", combatState.ToString(), pressedkey.ToString());
								SetUpSkillMenuCommandParameters((CurrentPartyMember_turn), 5);
								break;
							case EBattleCommand.ESSENCE:
								break;
							case EBattleCommand.CURSOR:
								break;
							case EBattleCommand.BACK:
								pastCombatState = ECombatState.NONE;
								bDrawSkillMenu_UI = false;
								_partyMemberSkillListBoxes_dict[CurrentPartyMember_turn.First_Name].SetActiveStatus(false);

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Skill UI Back Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());
								break;

							default:
								throw new ArgumentOutOfRangeException();
						}
					}
					else if (pastCombatState == ECombatState.ItemsUISpawn)
					{
						switch (CommandRequest)
						{
							case EBattleCommand.NONE:
							case EBattleCommand.WEAPON:
							case EBattleCommand.CURSOR:
							case EBattleCommand.ESSENCE:
							case EBattleCommand.STANCE:
							case EBattleCommand.ATTACK:
							case EBattleCommand.SKILL:
							case EBattleCommand.ITEM:
							case EBattleCommand.DEFENSE:
							case EBattleCommand.SELECT:
								break;

							case EBattleCommand.BACK:

								pastCombatState = ECombatState.NONE;
								_partyMemberItemsListBoxes_dict[CurrentPartyMember_turn.First_Name].SetActiveStatus(false);

								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Items UI Back Request | Key: {1}",
									combatState.ToString(), pressedkey.ToString());

								break;
							case EBattleCommand.MENU1:
								debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Menu Option 1 Request | Key: {1}", combatState.ToString(), pressedkey.ToString());
								SetupItemMenuParameters((CurrentPartyMember_turn), 0);

								break;
							case EBattleCommand.MENU2:
								break;
							case EBattleCommand.MENU3:
								break;
							case EBattleCommand.MENU4:
								break;
							case EBattleCommand.MENU5:
								break;
							case EBattleCommand.MENU6:
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
					else //pastCombatState has been set. so we can use advance to choose target.
					{
						switch (CommandRequest)
						{
							case EBattleCommand.NONE:
								break;
							case EBattleCommand.ATTACK:
							case EBattleCommand.SKILL: //I don't think this is line is needed
							case EBattleCommand.ITEM: //I don't think this line is needed..
								//If you reach here you have a valid command to access the choose target state.
								//State => ChooseTarget
								combatState = ECombatState.ChooseTarget; 
								break;
							case EBattleCommand.BACK:
								//If this is pressed then the skills UI, Items UI, etc. Need to go away.
								//And we need to set it back to the main UI.
								ClearPastState();
								DespawnUI();
								//State => MainUISpawn
								combatState = ECombatState.MainUISpawn;
								break;
							//default:
								
								//throw new ArgumentOutOfRangeException();
						}
					}
					break;
				case ECombatState.StartPhysicalAttack:

					//Do not allow state machine traversal IF events still need handling
					if(combatActions_Queue.Count > 0) return;

					if (CurrentTurnCharacter is Enemy)
					{
						CombatState = ECombatState.Attack;
					}
					else if (CurrentTurnCharacter is PartyMember)
					{
						combatState = ECombatState.WaitingForInput;
						pastCombatState = ECombatState.StartPhysicalAttack;
						//save the command type
						CommandRequest = EBattleCommand.ATTACK;
						
					}
					break;
				case ECombatState.Attack:

					if (CurrentTurnCharacter is Enemy)
					{
						//Enemies don't need to do any fancy UI things. 

						//Does this enemy have any modifiers we need to apply?
						//CheckForEntityModifiers_Attack(currentEnemy_turn);

						////Apply the modifiers to current character if there are any
						//foreach (Tuple<BattleEntity, ModifierData> tup in EntityAttackTraitModifier_List)
						//{
						//	List<object> paramList = new List<object>();
						//	paramList.Add(tup.Item1);
						//	//Get the Current Skill that we are going to use FUNCTION NAME
						//	String funcname = tup.Item2.Function_PTR;
						//	MethodInfo SkillMethod = Type.GetType("CombatSystem.Components.Combat.CombatDelegates.ModifierDelegates").GetMethod(funcname);
						//	SkillMethod.Invoke(null, new object[] { this, paramList }); //Invoke this method 
						//}

						//Run a function to figure out which this enemy should attack. TODO: do this comment for now im doing RNG
						PartyMember target = null;
						do
						{
							target = PartyMembers[CurrentPartyMembersNames[new Random().Next(0, CurrentPartyMembersNames.Count)]];
						} while (target.bIsDead);

						foreach (Equipable equipable in currentEnemy_turn.GetEquipment())
						{
							if (equipable == null) continue;
							foreach (ModifierData md in equipable.Traits)
								RollAttackModifiersChanceRate(currentEnemy_turn, target, md);
							foreach (ModifierData md in equipable.Effects)
								RollAttackModifiersChanceRate(currentEnemy_turn, target, md);
						}

						//Move this character forward to simulate attacking
						QueueCombatAction(new CombatMoveAction(this, (currentEnemy_turn), (int)currentEnemy_turn.SpawnPosition.X - 50, (int)currentEnemy_turn.SpawnPosition.Y - 10, 12, true));

						//Spawn the hit spark on that enemy that was attacked.
						TargetPosition_hitspark = new Vector2(target.Position.X, target.Position.Y);
						TargetPosition_hitspark.X += (float)((target.Width * target.ScaleX) / 2.0);
						TargetPosition_hitspark.Y += (float)((target.Height * target.ScaleY) / 2.0);

						//Apply a hit spark explosion
						QueueCombatAction(new CombatHitSparkAction(this, ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle"), TargetPosition_hitspark, Color.OrangeRed, false)
							{ scalarX = .2f, ScalarY = .2f });
						QueueCombatAction(new CombatDamageAction(this, null, currentEnemy_turn, false, SkillToUse,
							new List<Enemy>(), new List<PartyMember>() { target }));

						//Move the target back to simulate being hit.
						QueueCombatAction(new CombatMoveAction(this, (target), (int)target.SpawnPosition.X - 50, (int)target.SpawnPosition.Y - 10, 8, true));

						//move the character back to their past position.
						QueueCombatAction(new CombatMoveAction(this, (target), (int)(target).SpawnPosition.X, (int)(target).SpawnPosition.Y, 8, false));

						//Move enemy back to spawn position
						QueueCombatAction(new CombatMoveAction(this, (currentEnemy_turn), (int)currentEnemy_turn.SpawnPosition.X, (int)currentEnemy_turn.SpawnPosition.Y, 8, false));

						//End the attack
						combatState = ECombatState.EndAttack;
					}
					if (CurrentTurnCharacter is PartyMember)
					{
						////Does this party member have any modifiers we need to apply?
						//CheckForEntityModifiers_Attack(CurrentPartyMember_turn);

						foreach (Equipable equipable in CurrentPartyMember_turn.GetEquipment())
						{
							if (equipable == null) continue;
							foreach (ModifierData md in equipable.Traits)
								RollAttackModifiersChanceRate(CurrentPartyMember_turn, currentSelectedEnemy, md);
							foreach (ModifierData md in equipable.Effects)
								RollAttackModifiersChanceRate(CurrentPartyMember_turn, currentSelectedEnemy, md);
						}

						////Apply the modifiers to current character if there are any
						//foreach (Tuple<BattleEntity, ModifierData> tup in EntityAttackTraitModifier_List)
						//{
						//	List<object> paramList = new List<object>();
						//	paramList.Add(tup.Item1);
						//	//Get the Current Skill that we are going to use FUNCTION NAME
						//	String funcname = tup.Item2.Function_PTR;
						//	MethodInfo SkillMethod = Type.GetType("CombatSystem.Components.Combat.CombatDelegates.ModifierDelegates").GetMethod(funcname);
						//	SkillMethod.Invoke(null, new object[] { this, paramList }); //Invoke this method 
						//}

						//Get the location of the Entity that we will attack.

						//Load the Animation that need to play when attacking.

						//Move the character forward to show they attacked something.
						QueueCombatAction(new CombatMoveAction(this, CurrentTurnCharacter, (int)(CurrentPartyMember_turn).Position.X + 50, (int)(CurrentPartyMember_turn).Position.Y - 10, 8, true));
						//Move enemy back to simulate a hit
						//QueueCombatAction(new CombatMoveAction(this, (currentSelectedEnemy), (int)currentSelectedEnemy.SpawnPosition.X + 50, (int)currentSelectedEnemy.SpawnPosition.Y - 10, 8, false));


						//Spawn the hit spark on that enemy that was attacked.
						TargetPosition_hitspark = new Vector2(
							currentSelectedEnemy.Position.X,
							currentSelectedEnemy.Position.Y);

						TargetPosition_hitspark.X +=
							(float) ((currentSelectedEnemy.Width *
							          currentSelectedEnemy.ScaleX) / 2.0);

						TargetPosition_hitspark.Y +=
							(float) ((currentSelectedEnemy.Height *
							          currentSelectedEnemy.ScaleY) / 2.0);

						//Apply a hit spark explosion
						QueueCombatAction(new CombatHitSparkAction(this, ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle"), TargetPosition_hitspark, Color.OrangeRed, false)
							{ scalarX = .2f, ScalarY = .2f });
						QueueCombatAction(new CombatDamageAction(this, null, (CurrentPartyMember_turn), false, SkillToUse,
							 new List<Enemy>(){ (currentSelectedEnemy)}, new List<PartyMember>() 	));

						//Move enemy back to simulate a hit
						QueueCombatAction(new CombatMoveAction(this, (currentSelectedEnemy), (int)currentSelectedEnemy.SpawnPosition.X + 50, (int)currentSelectedEnemy.SpawnPosition.Y - 10, 8, true));


						//QueueCombatAction(new CombatDelayAction(this, 100));

						////Apply a Hit Flash with shaders.
						//currentSelectedEnemy.ShaderHitFlash = 3;
						//currentSelectedEnemy.ShaderHitTimer = 6;
						//currentSelectedEnemy.ShaderHitActuatTimer = 6;

						//Move enemy back to spawn position
						QueueCombatAction(new CombatMoveAction(this, (currentSelectedEnemy), (int)currentSelectedEnemy.SpawnPosition.X, (int)currentSelectedEnemy.SpawnPosition.Y,8, false));

						//move the character back to their past position.
						QueueCombatAction(new CombatMoveAction(this, (CurrentPartyMember_turn), (int)(CurrentPartyMember_turn).SpawnPosition.X, (int)(CurrentPartyMember_turn).SpawnPosition.Y, 20, true));


						bDrawSelectArrowUI = false; // Hide the Selection Icon!

						//Play the animation.

						//Apply any SFX to the screen if needed.

						//State => EndAttack.
						combatState = ECombatState.EndAttack;
					}

					break;
				case ECombatState.EndAttack:
					//Events still need to be handled
					while(combatActions_Queue.Count > 0)
						return;

					//Calculate the damage dealt.

					//Subtract the damage from the enemy health

					//move the character back to their past position.

					//State => ApplyAttackEffects.
					combatState = ECombatState.ApplyAttackEffects;
					break;
				case ECombatState.StartSkillsAttack:	//We have started the skills attack portion of the state machine.
					
					//Spawn the SKILLS UI
					if (CurrentTurnCharacter is PartyMember)
					{
						SKillsMenu_UI_pos =
							new Vector2(CurrentTurnCharacter.Position.X + CurrentTurnCharacter.Width, CurrentTurnCharacter.Position.Y + 120);
						bDrawSkillMenu_UI = true;

						_partyMemberSkillListBoxes_dict[CurrentPartyMember_turn.First_Name].SetPosition(SKillsMenu_UI_pos);
						_partyMemberSkillListBoxes_dict[CurrentPartyMember_turn.First_Name].SetActiveStatus(true);
					}

					//State => WaitingFor Input
					combatState = ECombatState.WaitingForInput;
					pastCombatState = ECombatState.SkillsUISpawn;
					break;
				case ECombatState.SkillsAttack:

					if (CurrentTurnCharacter is PartyMember)
					{
						//First == Requested Character
						//Second == Affected Characters.
						//Third == skill object being casted/used.
						List<object> paramList = new List<object>();
						paramList.Add(CurrentPartyMember_turn);

						_partyMemberSkillListBoxes_dict[CurrentPartyMember_turn.First_Name].SetActiveStatus(false); // Remove Skills UI from the Screen

						//TODO: DELETE
						_partyMemberItemsListBoxes_dict[CurrentPartyMember_turn.First_Name].SetActiveStatus(false);


						//Is the skill we are using an AoE skills?
						List<BattleEntity> affectedBattleEntitys = new List<BattleEntity>();

						//who are we targeting?
						if (SkillToUse.bAllies)
						{
							if (bDrawSelectArrowAreaRange)
							{
								//Who in the AOE range?
								foreach (String name in CurrentPartyMembersNames)
								{
									PartyMember partymem = PartyMembers[name];
									Rectangle r = Rectangle.Intersect(partymem.GetSpriteSheet().CurrentAnimation.GetScreenRectangle(),
										SelectArrowAreaRange_Rect);

									if (!r.IsEmpty)
									{
										affectedBattleEntitys.Add(partymem);
									}
								}
							}
							else
							{
								affectedBattleEntitys.Add(currentSelectedPartyMember);
							}
						}
						else
						{
							if (bDrawSelectArrowAreaRange)
							{
								//Who in the AOE range?
								foreach (Enemy enemy in BattleEnemyList.Values)
								{
									Rectangle r = Rectangle.Intersect(enemy.GetSpriteSheet().CurrentAnimation.GetScreenRectangle(),
										SelectArrowAreaRange_Rect);

									if (!r.IsEmpty)
									{
										affectedBattleEntitys.Add(enemy);
									}
								}
							}
							else
							{
								affectedBattleEntitys.Add(currentSelectedEnemy);
							}
						}

						paramList.Add(affectedBattleEntitys);
						paramList.Add(CurrentPartyMember_turn.Skills[PlayerMenuIndex]);

						//Get the Current Skill that we are going to use FUNCTION NAME
						String funcname = CurrentPartyMember_turn.Skills[PlayerMenuIndex].Function_PTR;
						MethodInfo SkillMethod = Type.GetType("CombatSystem.Components.Combat.CombatDelegates.SkillDelegates").GetMethod(funcname);
						SkillMethod.Invoke(null, new object[]{this, paramList }); //Invoke this method 


						//Get the location of the Entity that we will use a skill on.
						Vector2 TargetPosition = currentSelectedEnemy.Position;

						bDrawSelectArrowUI = false;// Hide the Selection Icon!
						bDrawSelectArrowAreaRange = false;
						//Play the animation.

						//Apply any SFX to the screen if needed.

						//State => EndAttack.
						combatState = ECombatState.EndSkillsAttack;
					}

					break;
				case ECombatState.EndSkillsAttack:
					//We still need to handle some Combat action events.
					if(combatActions_Queue.Count > 0) return;

					if (CurrentTurnCharacter is PartyMember)
					{
						//State => ApplyAttackEffects.
						combatState = ECombatState.ApplyAttackEffects;
						SkillToUse = null;


					}

					break;
				case ECombatState.StartItemUse:   //We have started the items portion of the state machine.

					//Spawn the ITEMS UI

					Vector2 ItemsMenu_UI_pos = new Vector2(CurrentTurnCharacter.Position.X + CurrentTurnCharacter.Width, CurrentTurnCharacter.Position.Y + 120);
					bDrawSkillMenu_UI = true;

					_partyMemberItemsListBoxes_dict[CurrentPartyMember_turn.First_Name].SetPosition(ItemsMenu_UI_pos + new Vector2(-400, 0));
					_partyMemberItemsListBoxes_dict[CurrentPartyMember_turn.First_Name].SetActiveStatus(true);

					//State => WaitingFor Input
					combatState = ECombatState.WaitingForInput;
					pastCombatState = ECombatState.ItemsUISpawn;
					break;
				case ECombatState.ItemUse:

					if (CurrentTurnCharacter is PartyMember)
					{

						//First == Requested Character
						//Second == Affected Characters.
						//Third == skill object being casted/used.
						List<object> paramList = new List<object>();
						paramList.Add(CurrentPartyMember_turn);
						
						_partyMemberItemsListBoxes_dict[CurrentPartyMember_turn.First_Name].SetActiveStatus(false);

						//Is the skill we are using an AoE skills?
						List<BattleEntity> affectedBattleEntitys = new List<BattleEntity>();

						//who are we targeting?
						if (ItemToUse.bAllies)
						{
							if (bDrawSelectArrowAreaRange)
							{
								//Who in the AOE range?
								foreach (String name in CurrentPartyMembersNames)
								{
									PartyMember partymem = PartyMembers[name];
									Rectangle r = Rectangle.Intersect(partymem.GetSpriteSheet().CurrentAnimation.GetScreenRectangle(),
										SelectArrowAreaRange_Rect);

									if (!r.IsEmpty)
									{
										affectedBattleEntitys.Add(partymem);
									}
								}
							}
							else
							{
								affectedBattleEntitys.Add(currentSelectedPartyMember);
							}
						}
						else
						{
							if (bDrawSelectArrowAreaRange)
							{
								//Who in the AOE range?
								foreach (Enemy enemy in BattleEnemyList.Values)
								{
									Rectangle r = Rectangle.Intersect(enemy.GetSpriteSheet().CurrentAnimation.GetScreenRectangle(),
										SelectArrowAreaRange_Rect);

									if (!r.IsEmpty)
									{
										affectedBattleEntitys.Add(enemy);
									}
								}
							}
							else
							{
								affectedBattleEntitys.Add(currentSelectedEnemy);
							}
						}

						paramList.Add(affectedBattleEntitys);
						paramList.Add(CurrentPartyMember_turn.Items[PlayerMenuIndex]);

						//Get the Current Skill that we are going to use FUNCTION NAME
						String funcname = CurrentPartyMember_turn.Items[PlayerMenuIndex].Function_PTR;
						MethodInfo ItemMethod = Type.GetType("CombatSystem.Components.Combat.CombatDelegates.ItemDelegates").GetMethod(funcname);
						ItemMethod.Invoke(null, new object[] { this, paramList }); //Invoke this method 


						//Get the location of the Entity that we will use a skill on.
						Vector2 TargetPosition = currentSelectedEnemy.Position;

						bDrawSelectArrowUI = false;// Hide the Selection Icon!
						bDrawSelectArrowAreaRange = false;
						//Play the animation.

						//Apply any SFX to the screen if needed.

						//State => EndAttack.


						//Get the location of the Entity that we use an item on.

						//Load the Animation that need to play when attacking.

						//Move the character.

						//Play the animation.

						//Apply any SFX to the screen if needed.

						//State => EndAttack.

					}
					else
					{
						//enemies
					}


					combatState = ECombatState.EndItemUse;
					break;
				case ECombatState.EndItemUse:
					//wait for movement queue to be empty

					//Calculate the damage/heal dealt.

					//Subtract the damage from the Entity health or add to Entity health

					//move the character back to their past position.

					//State => ApplyAttackEffects.
					combatState = ECombatState.ApplyAttackEffects;


					break;
				case ECombatState.Defense:
					//Apply guard animation

					//apply some shader to indicate gaurd.

					//State => EndEntityTurn
					CombatState = ECombatState.EndEntityTurn;
					break;
				case ECombatState.ChooseTarget:
					//If we have reached this state we have gone through any and all UI and inputs needed.
					//Choosing target is its own subset of "WaitingForInput"

					//Spawn the Choose target UI and despawn all other character UI.
					bDrawAttackSticker_UI = false;
					bDrawDefenseSticker_UI = false;
					bDrawSkillsSticker_UI = false;
					bDrawSkillMenu_UI = false;

					bDrawStanceLeftArrow_UI = false;
					bDrawWeaponLeftArrow_UI = false;
					bDrawStanceRightArrow_UI = false;
					bDrawWeaponRightArrow_UI = false;

					bDrawInventorySticker_UI = false;

					if (!bDrawSelectArrowUI)
					{
						//make sure the person you select is ALIVE
						if ( SkillToUse != null && SkillToUse.bAllies)
						{
							bDrawSelectArrowUI = true;
						}
						else
						{
							//MoveCursor_EnemySide(combatState, CommandRequest, pressedkey);



							do
							{
								if(currentEnemyIndex == 0)
									currentEnemyIndex = MaxEnemyIndex;
								else currentEnemyIndex--;
							} while (currentSelectedEnemy.bIsDead);

							bDrawSelectArrowUI = true;
							SelectArrowUI_Pos = currentSelectedEnemy.Position;
							SelectArrowUI_Pos.Y -= 50;
							SelectArrowUI_Pos.X += (float) ((currentSelectedEnemy.Width * currentSelectedEnemy.ScaleX) / 2.0);

							//Aoe?

							#region AOE Range Init

							if (bDrawSelectArrowAreaRange)
							{
								Vector2 textureOrigin = currentSelectedEnemy.Position;
								textureOrigin.X += (currentSelectedEnemy.Width * (float) currentSelectedEnemy.ScaleX / 2);
								textureOrigin.Y += (currentSelectedEnemy.Height * (float) currentSelectedEnemy.ScaleY / 2);

								SelectArrowAreaRange_Rect.X = (int) textureOrigin.X - (SelectArrowAreaRange_Rect.Width / 2);
								SelectArrowAreaRange_Rect.Y = (int) textureOrigin.Y - (SelectArrowAreaRange_Rect.Height / 2);

								foreach (Enemy enemy in BattleEnemyList.Values)
								{
									Rectangle r = Rectangle.Intersect(enemy.GetSpriteSheet().CurrentAnimation.GetScreenRectangle(),
										SelectArrowAreaRange_Rect);

									if (!r.IsEmpty)
									{
										enemy.ShaderHitFlash = 2;
										enemy.ShaderHitTimer = 6;
										enemy.ShaderHitActuatTimer = 6;
									}
									else
									{
										enemy.ShaderHitFlash = 0;
										enemy.ShaderHitTimer = 0;
										enemy.ShaderHitActuatTimer = 0;
									}
								}
							}

							#endregion

						}
					}

					//At this point the state machine cannot proceed without user input.
					pressedkey = Keys.None;
					if (keyboardState.GetPressedKeys().Length != 0)
					{
						if (keyboardState.IsKeyDown(keyboardState.GetPressedKeys()[0]) & prevKeyState.IsKeyUp(keyboardState.GetPressedKeys()[0]))
						{
							pressedkey = keyboardState.GetPressedKeys()[0]; //assume ONE KEY
							CommandRequest = GetInputCommand_Keys(pressedkey); //Is there a valid request?
						}
					}
					switch (CommandRequest)
					{
						case EBattleCommand.MENU1:
						case EBattleCommand.MENU2:
						case EBattleCommand.MENU3:
						case EBattleCommand.MENU4:
						case EBattleCommand.MENU5:
						case EBattleCommand.MENU6:
						case EBattleCommand.ESSENCE:
						case EBattleCommand.STANCE:
							ApplyStance(combatState, CommandRequest, pressedkey);

							debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | Change Stance Request | Key: {1}",
								combatState.ToString(), pressedkey.ToString());
							break;
						case EBattleCommand.ATTACK:
						case EBattleCommand.SKILL:
						case EBattleCommand.ITEM:
						case EBattleCommand.DEFENSE:
						case EBattleCommand.NONE:
						case EBattleCommand.WEAPON:
							break;


						case EBattleCommand.CURSOR:
							//Skills targeting
							if(SkillToUse != null && SkillToUse.bAllies)
								MoveCursor_PartyMemberSide(combatState, CommandRequest, pressedkey);
							else if(SkillToUse != null && !SkillToUse.bAllies)
								MoveCursor_EnemySide(combatState, CommandRequest, pressedkey);

							//Item Targeting
							if (ItemToUse != null && ItemToUse.bAllies)
								MoveCursor_PartyMemberSide(combatState, CommandRequest, pressedkey);
							else if (ItemToUse != null && !ItemToUse.bAllies)
								MoveCursor_EnemySide(combatState, CommandRequest, pressedkey);

							//Basic Attack Targeting.
							if(ItemToUse == null && SkillToUse == null)
								MoveCursor_EnemySide(combatState, CommandRequest, pressedkey);

							break;
						
						case EBattleCommand.SELECT:
							//If we got here then YAY we gonna slap a mother fucker, or hug them
							if (pastCombatState == ECombatState.StartSkillsAttack)
							{
								combatState = ECombatState.SkillsAttack;
							}
							else if (pastCombatState == ECombatState.StartItemUse)
							{
								combatState = ECombatState.ItemUse;
							}
							else if (pastCombatState == ECombatState.StartPhysicalAttack)
							{
								combatState = ECombatState.Attack;
							}
							break;
						case EBattleCommand.BACK:
							//If we got here we need to go back to the past UI that player was on.

							//De spawn the DICEUI (Targeting)

							//Remove character focus.
							if (pastCombatState == ECombatState.SkillsUISpawn)
							{
								//go back to the SkillsUI for this current character.
							}
							else if (pastCombatState == ECombatState.ItemsUISpawn)
							{
								//go back to the items UI for the current character.
							}
							else if (pastCombatState == ECombatState.StartPhysicalAttack)
							{
								//Go back to the MAIN UI for the specific character.
							}
							break;
						
						//default:
						//	throw new ArgumentOutOfRangeException();
					}
					break;
				case ECombatState.ApplyAttackEffects:
					//We are still handling events
					if(combatActions_Queue.Count > 0 ) return;

					if (CurrentTurnCharacter is Enemy)
					{
						//Any status effects to add?

						//apply them.

						//State => EndEntityTurn
						combatState = ECombatState.EndEntityTurn;
					}
					if (CurrentTurnCharacter is PartyMember)
					{
						//Any status effects to add?

						//apply them.

						//State => EndEntityTurn
						combatState = ECombatState.EndEntityTurn;

					}
					

					break;
				case ECombatState.StartFollowupAttacks:

					//Do we have any follow up attacks?
					var v = followUpAttack_List.GroupBy(x => x.Item2).ToList();

					for (int i =0; i < v.Count(); i++)
					{
						List<Tuple<BattleEntity, Job>> tempJobs = new List<Tuple<BattleEntity, Job>>();
						for (int j = 0; j < (v as IEnumerable<IGrouping<EStanceType, Tuple<BattleEntity, EStanceType>>>).ToList()[i].Count(); j++)
						{
							CombatParticleEmitters_List.FindAll(x => x.IsActive() && x.LinkedParentObject ==
								(v as IEnumerable<IGrouping<EStanceType, Tuple<BattleEntity, EStanceType>>>).ToList()[i].ToList()[j].Item1).ToList()
									.ForEach(y => this.QueueCombatAction(new CombatParticleCyclingAction(this, y)));

							tempJobs.Add( new Tuple<BattleEntity, Job>(((v as IEnumerable<IGrouping<EStanceType, Tuple<BattleEntity, EStanceType>>>).ToList()[i].ToList()[j].Item1 as PartyMember),
								((v as IEnumerable<IGrouping<EStanceType, Tuple<BattleEntity, EStanceType>>>).ToList()[i].ToList()[j].Item1 as PartyMember).Job));
						}
						//Use this to gather the follow up attack data from the database.
						for (int j = tempJobs.Count; j < 4; j++) 
							tempJobs.Add(new Tuple<BattleEntity, Job>(null, new Job(){Name = "NONE"}));

						int index = 0;
						while (index < FollowUpAttacks.Count)
						{
							String s = String.Format("{0}{1}{2}{3}", FollowUpAttacks[index].Job_1, FollowUpAttacks[index].Job_2,
								FollowUpAttacks[index].Job_3, FollowUpAttacks[index].Job_4);

							int charindex = s.IndexOf(tempJobs[0].Item2.Name);
							if (charindex != -1)
								s = s.Remove(charindex, tempJobs[0].Item2.Name.Length);

							charindex = s.IndexOf(tempJobs[1].Item2.Name);
							if (charindex != -1)
								s = s.Remove(charindex, tempJobs[1].Item2.Name.Length);

							charindex = s.IndexOf(tempJobs[2].Item2.Name);
							if (charindex != -1)
								s = s.Remove(charindex, tempJobs[2].Item2.Name.Length);

							charindex = s.IndexOf(tempJobs[3].Item2.Name);
							if (charindex != -1)
								s = s.Remove(charindex, tempJobs[3].Item2.Name.Length);

							if (s == String.Empty)
							{
								//We found it!
								s = "";
								List<PartyMember> tempBattleEntities = new List<PartyMember>();
								for (int k = 0; k < tempJobs.Count; k++)
								{
									if(tempJobs[k].Item1 != null)
										tempBattleEntities.Add(tempJobs[k].Item1 as PartyMember);
								}
								RequestedFollowupAttacks.Add(new Tuple<string, List<PartyMember>>(FollowUpAttacks[index].Function_PTR, tempBattleEntities));
								break;
							}
							index++;
						}

						//(v as IEnumerable<IGrouping<EStanceType, Tuple<BattleEntity, EStanceType>>>).ToList()[i].ToList();
						
						//FollowUpAttack f =  FollowUpAttacks[index];

					}
					followUpAttack_List.Clear();
					combatState = ECombatState.FollowUpAttacks;
					
					
					break;
				case ECombatState.FollowUpAttacks:
					//We are still moving or handling Actions. So skip until those are done.
					if (combatActions_Queue.Count != 0) return;

					#region Kill past UI!
					//bool bSkip = true;
					////Assuming that we are here repeating this again!
					//for (int i = 0; i < CombatUI_List.Count; i++)
					//{
					//	if (CombatUI_List[i].UIName.Contains("FollowUpBanner_"))
					//	{
					//		combatActions_Queue.Enqueue(new CombatKillUIAction(this, CombatUI_List[i]));
					//		bSkip &= false;

					//	}
					//}

					//if (!bSkip) return;//we need to kill those ui first before spawning more!
					#endregion

					//Let's handle one of those requests!
					if (RequestedFollowupAttacks.Count > 0)
					{
						//First Up Spawn the banners
						#region Banner spawn
						//Get the start location
						int startLoc = 0;
						int innerW = 0;
						int bannerW = 230;
						int bannerH = 900;
						int maxW = 1720;
						for (int i = 0; i < RequestedFollowupAttacks[0].Item2.Count; i++) 
							innerW += bannerW + 5;

						List<CombatMoveUIAction> requestedBatchMove = new List<CombatMoveUIAction>();

						startLoc = 0 + (maxW - innerW) / 2;

						for (int i = 0; i < RequestedFollowupAttacks[0].Item2.Count; i++)
						{
							QueueCombatAction(new CombatSpawnUIAction(this, 
								new BixBite.Rendering.UI.Image.GameImage("FollowUpBanner_" + RequestedFollowupAttacks[0].Item2[i].First_Name + i, startLoc, 0 - bannerH, bannerW, bannerH, 2, false,
									0, 0, "", "#00000000", ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Banner1")){ bIsActive = true, Transparency = .5f} , startLoc,  0 - bannerH ));
							startLoc += bannerW + 5;
						}
						#endregion

						//Drop the banners.
						#region Drop the banneers

						requestedBatchMove.Clear();
						for (int i = 0; i < RequestedFollowupAttacks[0].Item2.Count; i++)
						{
							requestedBatchMove.Add( new CombatMoveUIAction(this, null, null,0, 90, false, "FollowUpBanner_" + RequestedFollowupAttacks[0].Item2[i].First_Name + i ));
						}
						QueueCombatAction(new CombatBatchMoveUIAction(this, requestedBatchMove, true));

						#endregion
						//Delay so the banners can fall before attack logic
						QueueCombatAction(new CombatDelayAction(this, 500));

						//move out of screen.
						requestedBatchMove.Clear();
						for (int i = 0; i < RequestedFollowupAttacks[0].Item2.Count; i++)
						{
							requestedBatchMove.Add(new CombatMoveUIAction(this, null, null, -900, 80, false, "FollowUpBanner_" + RequestedFollowupAttacks[0].Item2[i].First_Name + i));
						}
						QueueCombatAction(new CombatBatchMoveUIAction(this, requestedBatchMove, true));

						//Remove from the screen!
						for (int i = 0; i < RequestedFollowupAttacks[0].Item2.Count; i++)
						{
							QueueCombatAction(new CombatKillUIAction(this, null, "FollowUpBanner_" + RequestedFollowupAttacks[0].Item2[i].First_Name + i));
						}

						//Invoke Reflection to call the specific method for this follow up attack!
						MethodInfo FollowUpAttack = Type.GetType("CombatSystem.Components.Combat.CombatDelegates.FollowUpAttacksDelegates").GetMethod(RequestedFollowupAttacks[0].Item1);

						List<object> paramList = new List<object>();
						//paramList.Add(this);
						//paramList.Add(PartyMembers.Values.ToList().FindAll(x => CurrentPartyMembersNames.Contains(x.First_Name)));
						paramList.Add(RequestedFollowupAttacks[0].Item2);

						List<Enemy> enemiesToHit = new List<Enemy>();
						for (int i = 0; i < BattleEnemyList.Keys.Count; i++)
						{
							if (CurrentEnemiesNames.Contains(BattleEnemyList.Keys.ToList()[i]))
								enemiesToHit.Add(BattleEnemyList.Values.ToList()[i]);
						}
						paramList.Add(enemiesToHit);
						FollowUpAttack.Invoke(null, new object[] { this, paramList }); //Invoke this method 
					}

					if (RequestedFollowupAttacks.Count == 0)
					{
						CombatState = ECombatState.EndFollowUpAttacks;
					}
					else
					{
						//Remove this from the list
						RequestedFollowupAttacks.RemoveAt(0);
					}
					break;

				case ECombatState.EndFollowUpAttacks:
					// The turn is now FULLY over. So we can do applying of stats, and status effects
					// As well as their SFX and VFX counterparts.
					//combatState = ECombatState.ApplyAttackEffects;

					CombatState = ECombatState.StartTurn;
					break;
				case ECombatState.MainUISpawn:
					//This is the true beginning of the entities turn, where input is allowed etc.

					//Is something still moving? If so ignore until move queue is empty

					//is good character?
						//spawn the MainUI around the current character
						//we need player input, so we need to wait for it
						//State => WaitingForInput
					//is enemy?


					break;
				case ECombatState.ItemsUISpawn:
					break;
				case ECombatState.SkillsUISpawn:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			#endregion

			//TurnQueue
			TurnQueue_GameListBox.Update(gameTime);

			prevKeyState = keyboardState;
			//If you have pressed two buttons or more we need to ignore this.
			if(prevKeyState.GetPressedKeys().Length > 1)
			{
				prevKeyState = new KeyboardState();
			}

			//PartyMemberEssenceBar.Update(gameTime);
		}

		#region Combat System Helpers

		public void SetupItemMenuParameters(PartyMember partym, int menuChoice)
		{
			//Does this skill ACTUALLY EXIST?
			if ((CurrentPartyMember_turn).Skills.Count >= 1 && menuChoice == 0)
			{
				CombatState = ECombatState.ChooseTarget;
				pastCombatState = ECombatState.StartItemUse;
				currentEnemyIndex = 0;
				currentPartyMemberIndex = 0;

				PlayerMenuIndex = 0; // keeps track of player menu decision for later.
				ItemToUse = partym.Items[PlayerMenuIndex];
				//Is this skill AOE?
				if ((CurrentPartyMember_turn).Items[0].AoE_W != 0 &&
						(CurrentPartyMember_turn).Items[0].AoE_H != 0)
				{
					//IT IS AOE.
					bDrawSelectArrowAreaRange = true;
					SelectArrowAreaRange_Rect = new Rectangle(0, 0, (CurrentPartyMember_turn).Items[0].AoE_W, (CurrentPartyMember_turn).Items[0].AoE_H);
				}
			}
		
		}

		public void SetUpSkillMenuCommandParameters(PartyMember partym, int menuChoice )
		{
			//Does this skill ACTUALLY EXIST?
			if ((CurrentPartyMember_turn).Skills.Count >= 1 && menuChoice == 0)
			{
				//Is this skill valid for use?

				if (!(CurrentPartyMember_turn).Skills[0].bIsAllowed)
				{
					debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | SKILL Request Invalid Due to Wrong Weapon Type | Current Weapon: {1} !=  Desired Weapon Type {2}",
						combatState.ToString(), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).CurrentWeapon.Value.Weapon_Type), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).Skills[0].Weapon_Type));
					return;
				}

				CombatState = ECombatState.ChooseTarget;
				pastCombatState = ECombatState.StartSkillsAttack;
				currentEnemyIndex = 0;

				PlayerMenuIndex = 0; // keeps track of player menu decision for later.
				SkillToUse = partym.Skills[PlayerMenuIndex];
				//Is this skill AOE?
				if ((CurrentPartyMember_turn).Skills[0].AOE_w != 0 &&
						(CurrentPartyMember_turn).Skills[0].AOE_h != 0)
				{
					//IT IS AOE.
					bDrawSelectArrowAreaRange = true;
					SelectArrowAreaRange_Rect = new Rectangle(0, 0, (CurrentPartyMember_turn).Skills[0].AOE_w, (CurrentPartyMember_turn).Skills[0].AOE_h);
				}
			}
			//Does this skill ACTUALLY EXIST?
			else if ((CurrentPartyMember_turn).Skills.Count >= 2 && menuChoice == 1)
			{
				if (!(CurrentPartyMember_turn).Skills[1].bIsAllowed)
				{
					debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | SKILL Request Invalid Due to Wrong Weapon Type | Current Weapon: {1} !=  Desired Weapon Type {2}",
						combatState.ToString(), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).CurrentWeapon.Value.Weapon_Type), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).Skills[1].Weapon_Type));
					return;
				}

				CombatState = ECombatState.ChooseTarget;
				pastCombatState = ECombatState.StartSkillsAttack;
				currentEnemyIndex = 0;

				PlayerMenuIndex = 1; // keeps track of player menu decision for later.
				SkillToUse = partym.Skills[PlayerMenuIndex];
				//Is this skill AOE?
				if ((CurrentPartyMember_turn).Skills[1].AOE_w != 0 &&
				    (CurrentPartyMember_turn).Skills[1].AOE_h != 0)
				{
					//IT IS AOE.
					bDrawSelectArrowAreaRange = true;
					SelectArrowAreaRange_Rect = new Rectangle(0, 0, (CurrentPartyMember_turn).Skills[1].AOE_w, (CurrentPartyMember_turn).Skills[1].AOE_h);
				}
			}
			//Does this skill ACTUALLY EXIST?
			if ((CurrentPartyMember_turn).Skills.Count >= 3 && menuChoice == 2)
			{
				if (!(CurrentPartyMember_turn).Skills[2].bIsAllowed)
				{
					debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | SKILL Request Invalid Due to Wrong Weapon Type | Current Weapon: {1} !=  Desired Weapon Type {2}",
						combatState.ToString(), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).CurrentWeapon.Value.Weapon_Type), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).Skills[2].Weapon_Type));
					return;
				}

				CombatState = ECombatState.ChooseTarget;
				pastCombatState = ECombatState.StartSkillsAttack;
				currentEnemyIndex = 0;

				PlayerMenuIndex = 2; // keeps track of player menu decision for later.
				SkillToUse = partym.Skills[PlayerMenuIndex];
				//Is this skill AOE?
				if ((CurrentPartyMember_turn).Skills[2].AOE_w != 0 &&
				    (CurrentPartyMember_turn).Skills[2].AOE_h != 0)
				{
					//IT IS AOE.
					bDrawSelectArrowAreaRange = true;
					SelectArrowAreaRange_Rect = new Rectangle(0, 0, (CurrentPartyMember_turn).Skills[2].AOE_w, (CurrentPartyMember_turn).Skills[2].AOE_h);
				}
			}
			//Does this skill ACTUALLY EXIST?
			if ((CurrentPartyMember_turn).Skills.Count >= 4 && menuChoice == 3)
			{
				if (!(CurrentPartyMember_turn).Skills[3].bIsAllowed)
				{
					debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | SKILL Request Invalid Due to Wrong Weapon Type | Current Weapon: {1} !=  Desired Weapon Type {2}",
						combatState.ToString(), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).CurrentWeapon.Value.Weapon_Type), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).Skills[3].Weapon_Type));
					return;
				}

				CombatState = ECombatState.ChooseTarget;
				pastCombatState = ECombatState.StartSkillsAttack;
				currentEnemyIndex = 0;

				PlayerMenuIndex = 3; // keeps track of player menu decision for later.
				SkillToUse = partym.Skills[PlayerMenuIndex];

				//Is this skill AOE?
				if ((CurrentPartyMember_turn).Skills[3].AOE_w != 0 &&
				    (CurrentPartyMember_turn).Skills[3].AOE_h != 0)
				{
					//IT IS AOE.
					bDrawSelectArrowAreaRange = true;
					SelectArrowAreaRange_Rect = new Rectangle(0, 0, (CurrentPartyMember_turn).Skills[3].AOE_w, (CurrentPartyMember_turn).Skills[3].AOE_h);
				}
			}
			//Does this skill ACTUALLY EXIST?
			if ((CurrentPartyMember_turn).Skills.Count >= 5 && menuChoice == 4)
			{
				if (!(CurrentPartyMember_turn).Skills[4].bIsAllowed)
				{
					debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | SKILL Request Invalid Due to Wrong Weapon Type | Current Weapon: {1} !=  Desired Weapon Type {2}",
						combatState.ToString(), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).CurrentWeapon.Value.Weapon_Type), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).Skills[4].Weapon_Type));
					return;
				}

				CombatState = ECombatState.ChooseTarget;
				pastCombatState = ECombatState.StartSkillsAttack;
				currentEnemyIndex = 0;

				PlayerMenuIndex = 4; // keeps track of player menu decision for later.
				SkillToUse = partym.Skills[PlayerMenuIndex];

				//Is this skill AOE?
				if ((CurrentPartyMember_turn).Skills[4].AOE_w != 0 &&
				    (CurrentPartyMember_turn).Skills[4].AOE_h != 0)
				{
					//IT IS AOE.
					bDrawSelectArrowAreaRange = true;
					SelectArrowAreaRange_Rect = new Rectangle(0, 0, (CurrentPartyMember_turn).Skills[4].AOE_w, (CurrentPartyMember_turn).Skills[4].AOE_h);
				}
			}
			//Does this skill ACTUALLY EXIST?
			if ((CurrentPartyMember_turn).Skills.Count >= 6 && menuChoice == 5)
			{
				if (!(CurrentPartyMember_turn).Skills[5].bIsAllowed)
				{
					debug_LogBlock.Text = String.Format("LAST HIT => CBS {0} | SKILL Request Invalid Due to Wrong Weapon Type | Current Weapon: {1} !=  Desired Weapon Type {2}",
						combatState.ToString(), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).CurrentWeapon.Value.Weapon_Type), Enum.GetName(typeof(EWeaponType), (CurrentPartyMember_turn).Skills[5].Weapon_Type));
					return;
				}

				CombatState = ECombatState.ChooseTarget;
				pastCombatState = ECombatState.StartSkillsAttack;
				currentEnemyIndex = 0;

				PlayerMenuIndex = 5; // keeps track of player menu decision for later.
				SkillToUse = partym.Skills[PlayerMenuIndex];

				//Is this skill AOE?
				if ((CurrentPartyMember_turn).Skills[5].AOE_w != 0 &&
				    (CurrentPartyMember_turn).Skills[5].AOE_h != 0)
				{
					//IT IS AOE.
					bDrawSelectArrowAreaRange = true;
					SelectArrowAreaRange_Rect = new Rectangle(0, 0, (CurrentPartyMember_turn).Skills[5].AOE_w, (CurrentPartyMember_turn).Skills[5].AOE_h);
				}
			}
		}


		private EBattleCommand GetInputCommand_Keys(Keys key)
		{
			EBattleCommand retBattleCommand = EBattleCommand.NONE;

			//USE THE SETTINGS SET BY THE USER.
			if (key == (LeftKey) && CombatState == ECombatState.WaitingForInput)
				retBattleCommand = EBattleCommand.WEAPON;
			else if (key == (RightKey) && CombatState == ECombatState.WaitingForInput)
				retBattleCommand = EBattleCommand.WEAPON;

			else if (key == (LeftKey))
				retBattleCommand = EBattleCommand.CURSOR;
			else if (key == (RightKey))
				retBattleCommand = EBattleCommand.CURSOR;
			else if (key == (UpKey))
				retBattleCommand = EBattleCommand.CURSOR;
			else if (key == (DownKey))
				retBattleCommand = EBattleCommand.CURSOR;

			else if (key == (AttackKey) && pastCombatState == ECombatState.NONE)
				retBattleCommand = EBattleCommand.ATTACK;
			else if (key == (SkillKey) && pastCombatState == ECombatState.NONE)
				retBattleCommand = EBattleCommand.SKILL;
			else if (key == (DefenseKey) && pastCombatState == ECombatState.NONE)
				retBattleCommand = EBattleCommand.DEFENSE;
			else if (key == (ItemKey))
				retBattleCommand = EBattleCommand.ITEM;
			else if (key == (EssenceDownKey))
				retBattleCommand = EBattleCommand.ESSENCE;
			else if (key == (EssenceUpKey))
				retBattleCommand = EBattleCommand.ESSENCE;
			else if (key == (EssenceLeftKey))
				retBattleCommand = EBattleCommand.ESSENCE;
			else if (key == (EssenceRightKey))
				retBattleCommand = EBattleCommand.ESSENCE;


			else if (key == (StanceLeftKey))
				retBattleCommand = EBattleCommand.STANCE;
			else if (key == (StanceRightKey))
				retBattleCommand = EBattleCommand.STANCE;

			else if (key == (BackKey))
				retBattleCommand = EBattleCommand.BACK;
			else if (key == (SelectKey))
				retBattleCommand = EBattleCommand.SELECT;
			
			else if (key == (CBMenuSelect1))
				retBattleCommand = EBattleCommand.MENU1;
			else if (key == (CBMenuSelect2))
				retBattleCommand = EBattleCommand.MENU2;
			else if (key == (CBMenuSelect3))
				retBattleCommand = EBattleCommand.MENU3;
			else if (key == (CBMenuSelect4))
				retBattleCommand = EBattleCommand.MENU4;
			else if (key == (CBMenuSelect5))
				retBattleCommand = EBattleCommand.MENU5;
			else if (key == (CBMenuSelect6))
				retBattleCommand = EBattleCommand.MENU6;

			return retBattleCommand;
		}

		//TODO: Change it so this uses equation of stats.
		//Currently im using the DEFAULT setting of FUCK it.
		public void FillTurnQueue()
		{
			TurnQueue.Clear();
			//which is bigger.
			int goodcnt = CurrentPartyMembersNames.Count;
			int badcnt = CurrentEnemiesNames.Count;

			List<BattleEntity> tempBC = new List<BattleEntity>();
			List<int> tempi = new List<int>();

			foreach (PartyMember partyMembersValue in PartyMembers.Values)
			{
				tempi.Add(partyMembersValue.Stats.Agility);
				tempBC.Add(partyMembersValue);
			}

			foreach (String enemyName in CurrentEnemiesNames)
			{
				if (BattleEnemyList.TryGetValue(enemyName, out Enemy emval))
				{
					tempi.Add(emval.Stats.Agility);
					tempBC.Add(emval);
				}
			}

			tempi = tempi.OrderByDescending(x => x).ToList();

			int i = 0;
			while (tempi.Count != 0)
			{
				if (tempBC[i] is Enemy em)
				{
					if (em.Stats.Agility == tempi[0] && !TurnQueue.Contains(em))
					{
						TurnQueue.Enqueue(em);
						tempi.RemoveAt(0);
						i = 0;
						continue;
					}
				}
				else if (tempBC[i] is PartyMember pm)
				{
					pm.StanceIndicator_Color = Color.White;
					if (pm.Stats.Agility == tempi[0] && !TurnQueue.Contains(pm))
					{
						TurnQueue.Enqueue(pm);
						tempi.RemoveAt(0);
						i = 0;
						continue;
					}
				}

				i++;
			}

			//We have the Queue. Now we just need to load them into the turn queue
			TurnQueue_GameListBox.SetActiveStatus(true);
			TurnQueue_GameListBox.Items.Clear();
			foreach (BattleEntity battleEntity in TurnQueue.ToList())
			{

				TurnQueue_GameListBox.Items.Add(new GameListBoxItem(TurnQueue_GameListBox, 100, 100));
				TurnQueue_GameListBox.Items.Last().LoadBorderTexture(ContentRef.Load<Texture2D>("Images/UI/TurnQueueBackground"));
				
				//TurnQueue_GameListBox.Items.Last().Controls.Add(new GameTextBlock("ItemDesc", 100, 25, 40, 0, 1, "#00000000", "Skill " + (i + 1))
				//	{ _font = Content.Load<SpriteFont>("Font/BaseFont"), PenColour = Color.White, Position = testListBox_Vert.Items[i].AbsolutePosition, bMiddleVertical = true });

			}
			TurnQueue_GameListBox.SetSpacing(20);
			TurnQueue_GameListBox.SetAbsolutePosition_Items( new Vector2(10, 10), 0, 100);
			TurnQueue_GameListBox.SetHighlightedPositions_Items(4, 4);
			TurnQueue_GameListBox.LoadHighlightedTexture(ContentRef.Load<Texture2D>("Images/UI/TurnQueueBackground_Highlight"));

			int counter = 0;
			foreach (BattleEntity battleEntity in TurnQueue.ToList())
			{

				GameIMG img = new GameIMG("Icon", 60, 60, 0, 20, 20)
					{ Position = TurnQueue_GameListBox.Items[counter].AbsolutePosition };
				//img.SetGraphicsDeviceRef(GraphicsDevice);
				img.SetUITexture(true, battleEntity.Icon);

				TurnQueue_GameListBox.Items[counter].Controls.Add(img);
				TurnQueue_GameListBox.Items[counter++].SetActiveStatus(true);
			}
			TurnQueue_GameListBox.ResetSelected();

		}

		public void LoadInPartyMember(String Name, PartyMember pm)
		{
			//Add a PartyMember to the List & Screen
			pm.SpawnPosition = pm.Position;
			//pm.SetSize(); //obsolete since i added spritesheet file importing.
			PartyMembers.Add(Name, pm);

			//We need to create a UI for skills
			_partyMemberSkillListBoxes_dict.Add(pm.First_Name, new GameListBox(Vector2.Zero, 300, 250, 250, 40, 6, DownKey, UpKey, Keys.None, Keys.None, SelectKey, BackKey, 20, false));
			_partyMemberSkillListBoxes_dict.Last().Value.SelectRequest_Hook = delegate (int value) { Console.WriteLine("Selected: " + value); _partyMemberSkillListBoxes_dict.Last().Value.SetActiveStatus(false); };
			_partyMemberSkillListBoxes_dict.Last().Value.SetActiveStatus(false);
			_partyMemberSkillListBoxes_dict.Last().Value.SetSpacing(0);

			foreach (Skill skill in pm.Skills)
			{
				//Create the Items.
				_partyMemberSkillListBoxes_dict.Last().Value.Items.Add(new GameListBoxItem(_partyMemberSkillListBoxes_dict.Last().Value, 150, 40));
				_partyMemberSkillListBoxes_dict.Last().Value.Items.Last().LoadBorderTexture(ContentRef.Load<Texture2D>("Images/UI/CombatListBox_Background"));
				_partyMemberSkillListBoxes_dict.Last().Value.SetAbsolutePosition_Items(new Vector2(0, 0), 40); //Set the Items Added Positions
				//FIll in the Item data
				_partyMemberSkillListBoxes_dict.Last().Value.Items.Last().Controls.Add
				(
				new GameTextBlock("SkillName", 100, 40, 50, 0, 1, "#000000000", skill.Name)
					{ bMiddleVertical = true, PenColour = Color.White, _font = Font, Position = _partyMemberSkillListBoxes_dict.Last().Value.Items.Last().AbsolutePosition }	
				);
			}

			//We need to create a UI for items
			_partyMemberItemsListBoxes_dict.Add(pm.First_Name, new GameListBox(Vector2.Zero, 300, 250, 250, 40, 6, DownKey, UpKey, Keys.None, Keys.None, SelectKey, Keys.Back, 20, false ));
			_partyMemberItemsListBoxes_dict.Last().Value.SelectRequest_Hook = delegate(int value)
			{
				Console.WriteLine("Selected: " + value);
				_partyMemberItemsListBoxes_dict.Last().Value.SetActiveStatus(false);
			};
			_partyMemberSkillListBoxes_dict.Last().Value.SetActiveStatus(false);
			_partyMemberSkillListBoxes_dict.Last().Value.SetSpacing(0);

			foreach (Item item in pm.Items)
			{
				_partyMemberItemsListBoxes_dict.Last().Value.Items.Add(new GameListBoxItem(_partyMemberItemsListBoxes_dict.Last().Value, 150, 40));
				_partyMemberItemsListBoxes_dict.Last().Value.Items.Last().LoadBorderTexture(ContentRef.Load<Texture2D>("Images/UI/CombatListBox_Background"));
				_partyMemberItemsListBoxes_dict.Last().Value.SetAbsolutePosition_Items(new Vector2(0,0), 40);
				_partyMemberItemsListBoxes_dict.Last().Value.Items.Last().Controls.Add
				(
				new GameTextBlock("ItemName", 100, 40, 30, 0, 1, "#00000000", item.ID)
				{ bMiddleVertical = true, PenColour = Color.White, _font = Font, Position = _partyMemberItemsListBoxes_dict.Last().Value.Items.Last().AbsolutePosition}
				);
			}

			pm.StanceIndicator_Texture2D = ContentRef.Load<Texture2D>("Images/UI/StanceCircle");
			pm.StanceIndicator_RectangleOffset = new Rectangle(10,pm.Height - pm.StanceIndicator_Texture2D.Height - 50, pm.StanceIndicator_Texture2D.Width, pm.StanceIndicator_Texture2D.Height);



		}

		public void LoadInEnemy(String Name, Enemy enemy)
		{
			enemy.SpawnPosition = enemy.Position;
			//enemy.SetSize(); //obsolete since i added spritesheet file importing.
			BattleEnemyList.Add(Name, enemy);
		}

		private void ChangeWeapons(ECombatState currentCombatState, EBattleCommand currentBattleCommand, Keys pressedkey )
		{
			foreach (ModifierData md in CurrentPartyMember_turn.CurrentWeapon.Value.Traits)
			{
				EntityBuffTraitModifier_List.Remove(new Tuple<BattleEntity, ModifierData>(CurrentPartyMember_turn, md));
				EntityDebuffTraitModifier_List.Remove(new Tuple<BattleEntity, ModifierData>(CurrentPartyMember_turn, md));

				DeApplyStatChange(CurrentPartyMember_turn, CurrentPartyMember_turn.StatChange_List.Find(x => x.LinkedModifierData == md));
				CurrentPartyMember_turn.StatChange_List.RemoveAll(x => x.LinkedModifierData == md);
			}

			foreach (ModifierData md in CurrentPartyMember_turn.CurrentWeapon.Value.Effects)
			{
				EntityBuffEffectModifier_List.Remove(new Tuple<BattleEntity, ModifierData>(CurrentPartyMember_turn, md));
				EntityDebuffEffectModifier_List.Remove(new Tuple<BattleEntity, ModifierData>(CurrentPartyMember_turn, md));

				DeApplyStatChange(CurrentPartyMember_turn, CurrentPartyMember_turn.StatChange_List.Find(x => x.LinkedModifierData == md));
				CurrentPartyMember_turn.StatChange_List.RemoveAll(x => x.LinkedModifierData == md);
			}

			//if this is called the PLAYER MUST BE IN THE MainUISPawn State.
			if (currentCombatState == ECombatState.MainUISpawn || currentCombatState == ECombatState.WaitingForInput)
			{
				//VALID 
				if (currentBattleCommand == EBattleCommand.WEAPON && pressedkey == LeftKey)
				{
					CurrentPartyMember_turn.CurrentWeapon =
						(CurrentPartyMember_turn.CurrentWeapon.Previous == null ?
						CurrentPartyMember_turn.Weapons.Last : CurrentPartyMember_turn.CurrentWeapon.Previous);

				}
				else if (currentBattleCommand == EBattleCommand.WEAPON && pressedkey == RightKey)
				{
					CurrentPartyMember_turn.CurrentWeapon =
						(CurrentPartyMember_turn.CurrentWeapon.Next == null ?
						CurrentPartyMember_turn.Weapons.First : CurrentPartyMember_turn.CurrentWeapon.Next);

					//Apply Skill Filtering Logic here:

				}
				else
				{
#if DEBUG
					Console.WriteLine("CBSYS CW:  INVALID KEY FOUND IGNORING [{0}]", pressedkey.ToString());
#endif

					return; //do nothing.
				}
			}
			else
			{
#if DEBUG
				Console.WriteLine("CBSYS CW:  INVALID STATE IGNORING [{0}]", currentCombatState);
#endif
				//invalid
				return;
			}

			//Change the weapon card
			WeaponCardSticker_UI =
				WeaponCards_Textures_Dict[Enum.GetName(typeof(EWeaponType), CurrentPartyMember_turn.CurrentWeapon.Value.Weapon_Type)];

			foreach (Skill skill in CurrentPartyMember_turn.Skills)
			{
				//Apply Skill Filtering Logic here:
				if (skill.Weapon_Type == -1 || skill.Weapon_Type == 0)
				{
					//Valid
					skill.bIsAllowed = true;
				}
				else if (CurrentPartyMember_turn.CurrentWeapon.Value.Weapon_Type == skill.Weapon_Type)
				{
					//valid
					skill.bIsAllowed = true;
				}
				else
				{
					//invalid 
					skill.bIsAllowed = false;
				}
			}

			//Does this entity have any modifiers we need to apply?
			CheckForEntityModifiers_Buff(CurrentTurnCharacter, 5);
			CheckForEntityModifiers_Debuff(CurrentTurnCharacter, 5);

			//Apply the modifiers to current character if there are any
			foreach (Tuple<BattleEntity, ModifierData> tup in EntityBuffTraitModifier_List)
			{
				List<object> paramList = new List<object>();
				paramList.Add(tup.Item1);
				paramList.Add(tup.Item2);
				//Get the Current Skill that we are going to use FUNCTION NAME
				String funcname = tup.Item2.Function_PTR;
				MethodInfo SkillMethod = Type.GetType("CombatSystem.Components.Combat.CombatDelegates.ModifierDelegates").GetMethod(funcname);
				SkillMethod.Invoke(null, new object[] { this, paramList }); //Invoke this method 
			}


			//Set up the testing textbox to switch on command
			Vector2 pos = WeaponLeftArrow_UI_rect.Location.ToVector2() + new Vector2(40, 20);
			Weapon_GTB.Position = pos;
			Weapon_GTB.SetProperty("Width",
				WeaponRightArrow_UI_rect.X - WeaponLeftArrow_UI_rect.X - WeaponLeftArrow_UI.Width);
			Weapon_GTB.SetProperty("Height", 40);
			Weapon_GTB.Text = Enum.GetName(typeof(EWeaponType), CurrentPartyMember_turn.CurrentWeapon.Value.Weapon_Type);
		}

		private void MoveCursor_PartyMemberSide(ECombatState currentCombatState, EBattleCommand currentBattleCommand, Keys pressedkey)
		{

			//Are we using a skill or an item?
			if (pastCombatState == ECombatState.StartSkillsAttack || pastCombatState == ECombatState.StartItemUse)
			{
				//VALID 
				if (currentBattleCommand == EBattleCommand.CURSOR && pressedkey == LeftKey)
				{
					//We need to make sure this entity is allive before we place the cursor.
					do
					{
						if (currentPartyMemberIndex == 0)
							currentPartyMemberIndex = MaxPartyMembersIndex;
						else currentPartyMemberIndex--;
					} while (currentSelectedPartyMember.bIsDead);

					//Change the arrow position.
					SelectArrowUI_Pos = currentSelectedPartyMember.Position;
					SelectArrowUI_Pos.Y -= 100;
					SelectArrowUI_Pos.X += (currentSelectedPartyMember.Width * (float)currentSelectedPartyMember.ScaleX / 2);
				}
				else if (currentBattleCommand == EBattleCommand.CURSOR && pressedkey == RightKey)
				{
					do
					{
						if (currentPartyMemberIndex == MaxPartyMembersIndex)
							currentPartyMemberIndex = 0;
						else currentPartyMemberIndex++;
					} while (currentSelectedPartyMember.bIsDead);

					SelectArrowUI_Pos = currentSelectedPartyMember.Position;
					SelectArrowUI_Pos.Y -= 100;
					SelectArrowUI_Pos.X += (currentSelectedPartyMember.Width * (float)currentSelectedPartyMember.ScaleX / 2);
				}
				else
				{
#if DEBUG
					Console.WriteLine("CBSYS MC: INVALID KEY FOUND IGNORING [{0}]", pressedkey.ToString());
#endif
					return; //do nothing.
				}
			}
//			//This is here for BASIC attacking.
//			else if (currentCombatState == ECombatState.ChooseTarget)
//			{
//				//VALID 
//				if (currentBattleCommand == EBattleCommand.CURSOR && pressedkey == LeftKey)
//				{
//					do
//					{
//						if (currentEnemyIndex == 0)
//							currentEnemyIndex = MaxEnemyIndex;
//						else currentEnemyIndex--;
//					} while (currentSelectedEnemy.bIsDead);

//					//Change the arrow position.
//					SelectArrowUI_Pos = currentSelectedEnemy.Position;
//					SelectArrowUI_Pos.Y -= 50;
//					SelectArrowUI_Pos.X += (currentSelectedEnemy.Width * (float)
//																currentSelectedEnemy.ScaleX / 2);
//				}
//				else if (currentBattleCommand == EBattleCommand.CURSOR && pressedkey == RightKey)
//				{
//					do
//					{
//						if (currentEnemyIndex == MaxEnemyIndex)
//							currentEnemyIndex = 0;
//						else currentEnemyIndex++;
//					} while (currentSelectedEnemy.bIsDead);

//					SelectArrowUI_Pos = currentSelectedEnemy.Position;
//					SelectArrowUI_Pos.Y -= 50;
//					SelectArrowUI_Pos.X += (currentSelectedEnemy.Width * (float)currentSelectedEnemy.ScaleX / 2);
//				}
//				else
//				{
//#if DEBUG
//					Console.WriteLine("CBSYS MC:  INVALID KEY FOUND IGNORING [{0}]", pressedkey.ToString());
//#endif
//					return; //do nothing.
//				}
//			}

			//Aoe?
			if (bDrawSelectArrowAreaRange)
			{
				Vector2 textureOrigin = currentSelectedPartyMember.Position;
				textureOrigin.X += (currentSelectedPartyMember.Width * (float)currentSelectedPartyMember.ScaleX / 2);
				textureOrigin.Y += (currentSelectedPartyMember.Height * (float)currentSelectedPartyMember.ScaleY / 2);

				SelectArrowAreaRange_Rect.X = (int)textureOrigin.X - (SelectArrowAreaRange_Rect.Width / 2);
				SelectArrowAreaRange_Rect.Y = (int)textureOrigin.Y - (SelectArrowAreaRange_Rect.Height / 2);

				//Who in the AOE range?
				foreach (String pmname in CurrentPartyMembersNames)
				{
					PartyMember pm = PartyMembers[pmname];
					Rectangle r = Rectangle.Intersect(pm.GetSpriteSheet().CurrentAnimation.GetScreenRectangle(), SelectArrowAreaRange_Rect);

					if (!r.IsEmpty)
					{
						pm.ShaderHitFlash = 2;
						pm.ShaderHitTimer = 6;
						pm.ShaderHitActuatTimer = 6;
					}
					else
					{
						pm.ShaderHitFlash = 0;
						pm.ShaderHitTimer = 0;
						pm.ShaderHitActuatTimer = 0;
					}
				}
			}
		}

		private void MoveCursor_EnemySide(ECombatState currentCombatState, EBattleCommand currentBattleCommand, Keys pressedkey)
		{
			//Are we using a skill or an item?
			if (pastCombatState == ECombatState.StartSkillsAttack || pastCombatState == ECombatState.StartItemUse)
			{
				//VALID 
				if (currentBattleCommand == EBattleCommand.CURSOR && pressedkey == LeftKey)
				{
					//We need to make sure this entity is allive before we place the cursor.
					do
					{
						if (currentEnemyIndex == 0)
							currentEnemyIndex = MaxEnemyIndex;
						else currentEnemyIndex--;
					} while (currentSelectedEnemy.bIsDead);
					
					//Change the arrow position.
					SelectArrowUI_Pos = currentSelectedEnemy.Position;
					SelectArrowUI_Pos.Y -= 50;
					SelectArrowUI_Pos.X += (currentSelectedEnemy.Width * (float) currentSelectedEnemy.ScaleX / 2);
				}
				else if (currentBattleCommand == EBattleCommand.CURSOR && pressedkey == RightKey)
				{
					do
					{
						if (currentEnemyIndex == MaxEnemyIndex)
							currentEnemyIndex = 0;
						else currentEnemyIndex++;
					} while (currentSelectedEnemy.bIsDead);

					SelectArrowUI_Pos = currentSelectedEnemy.Position;
					SelectArrowUI_Pos.Y -= 50;
					SelectArrowUI_Pos.X += (currentSelectedEnemy.Width * (float) currentSelectedEnemy.ScaleX / 2);
				}
				else
				{
#if DEBUG
					Console.WriteLine("CBSYS MC: INVALID KEY FOUND IGNORING [{0}]", pressedkey.ToString());
#endif
					return; //do nothing.
				}
			}
			//This is here for BASIC attacking.
			else if (currentCombatState == ECombatState.ChooseTarget)
			{
				//VALID 
				if (currentBattleCommand == EBattleCommand.CURSOR && pressedkey == LeftKey)
				{
					do
					{
						if (currentEnemyIndex == 0)
							currentEnemyIndex = MaxEnemyIndex;
						else currentEnemyIndex--;
					} while (currentSelectedEnemy.bIsDead);

					//Change the arrow position.
					SelectArrowUI_Pos = currentSelectedEnemy.Position;
					SelectArrowUI_Pos.Y -= 50;
					SelectArrowUI_Pos.X += (currentSelectedEnemy.Width * (float)
					                      currentSelectedEnemy.ScaleX / 2);
				}
				else if (currentBattleCommand == EBattleCommand.CURSOR && pressedkey == RightKey)
				{
					do
					{
						if (currentEnemyIndex == MaxEnemyIndex)
							currentEnemyIndex = 0;
						else currentEnemyIndex++;
					} while (currentSelectedEnemy.bIsDead);

					SelectArrowUI_Pos = currentSelectedEnemy.Position;
					SelectArrowUI_Pos.Y -= 50;
					SelectArrowUI_Pos.X += (currentSelectedEnemy.Width * (float) currentSelectedEnemy.ScaleX / 2);
				}
				else
				{
#if DEBUG
					Console.WriteLine("CBSYS MC:  INVALID KEY FOUND IGNORING [{0}]", pressedkey.ToString());
#endif
					return; //do nothing.
				}
			}

			//Aoe?
			if (bDrawSelectArrowAreaRange)
			{
				Vector2 textureOrigin = currentSelectedEnemy.Position;
				textureOrigin.X += (currentSelectedEnemy.Width * (float) currentSelectedEnemy.ScaleX / 2);
				textureOrigin.Y += (currentSelectedEnemy.Height * (float) currentSelectedEnemy.ScaleY / 2);

				SelectArrowAreaRange_Rect.X = (int)textureOrigin.X - (SelectArrowAreaRange_Rect.Width / 2);
				SelectArrowAreaRange_Rect.Y = (int)textureOrigin.Y - (SelectArrowAreaRange_Rect.Height / 2);

				//Who in the AOE range?
				foreach (Enemy enemy in BattleEnemyList.Values)
				{
					Rectangle r = Rectangle.Intersect(enemy.GetSpriteSheet().CurrentAnimation.GetScreenRectangle(), SelectArrowAreaRange_Rect);

					if (!r.IsEmpty)
					{
						enemy.ShaderHitFlash = 2;
						enemy.ShaderHitTimer = 6;
						enemy.ShaderHitActuatTimer = 6;
					}
					else
					{
						enemy.ShaderHitFlash = 0;
						enemy.ShaderHitTimer = 0;
						enemy.ShaderHitActuatTimer = 0;
					}
				}


				
			}
		}

		private void ApplyEssence(ECombatState currentCombatState, Keys pressedkey)
		{
			if (currentCombatState == ECombatState.MainUISpawn || currentCombatState == ECombatState.SkillsUISpawn || 
				currentCombatState == ECombatState.ChooseTarget ||  currentCombatState == ECombatState.WaitingForInput)
			{
				if (pressedkey == EssenceDownKey)
				{
					if (PartyMemberEssenceBar.CurrentVal > 0 )
					{
						CurrentPartyMember_turn.Essence_Points--;
						PartyMemberEssenceBar.CurrentVal--;
						EssenceBar.CurrentVal++;
					}
				}
				else if (pressedkey == EssenceUpKey)
				{
					if (PartyMemberEssenceBar.CurrentVal <= PartyMemberEssenceBar.MaxVal && EssenceBar.CurrentVal > 0)
					{
						CurrentPartyMember_turn.Essence_Points++;
						PartyMemberEssenceBar.CurrentVal++;
						EssenceBar.CurrentVal--;
					}
				}
				else if (pressedkey == EssenceLeftKey)
				{

				}
				else if (pressedkey == EssenceRightKey)
				{

				}
				else
				{
#if DEBUG
					Console.WriteLine("CBSYS AE:  INVALID KEY FOUND IGNORING [{0}]", pressedkey.ToString());
#endif
					return;
				}
			}
			else
			{
#if DEBUG
				Console.WriteLine("CBSYS AE:  INVALID STATE IGNORING [{0}]", currentCombatState);
#endif
				return;
			}
		}

		private void ApplyStance(ECombatState currentCombatState, EBattleCommand currentBattleCommand, Keys pressedkey)
		{
			EStanceType pastStanceType = CurrentPartyMember_turn.CurrentStance.Value;
			if (currentCombatState == ECombatState.WaitingForInput || currentCombatState == ECombatState.MainUISpawn || 
				currentCombatState == ECombatState.SkillsUISpawn || currentCombatState == ECombatState.ChooseTarget)
			{
				if (pressedkey == StanceLeftKey)
				{
					//We should apply the stance to the textbox
					CurrentPartyMember_turn.CurrentStance = (CurrentPartyMember_turn.CurrentStance.Previous == null ? 
						CurrentPartyMember_turn.Stances_LL.Last : CurrentPartyMember_turn.CurrentStance.Previous);
					Stance_GTB.Text = CurrentPartyMember_turn.CurrentStance.Value.ToString();

					Vector2 pos = StanceLeftArrow_UI_rect.Location.ToVector2() + new Vector2(StanceLeftArrow_UI.Width ,0);
					Stance_GTB.Position = pos;
					Stance_GTB.SetProperty("Width", (StanceRightArrow_UI_rect.X - StanceLeftArrow_UI_rect.Location.X - StanceLeftArrow_UI.Width));


				}
				else if (pressedkey == StanceRightKey)
				{
					//We should apply the stance to the textbox
					CurrentPartyMember_turn.CurrentStance = (CurrentPartyMember_turn.CurrentStance.Next == null ?
						CurrentPartyMember_turn.Stances_LL.First : CurrentPartyMember_turn.CurrentStance.Next);
					Stance_GTB.Text = CurrentPartyMember_turn.CurrentStance.Value.ToString();

					Vector2 pos = StanceLeftArrow_UI_rect.Location.ToVector2() + new Vector2(StanceLeftArrow_UI.Width, 0); ;
					Stance_GTB.Position = pos;
					Stance_GTB.SetProperty("Width", (StanceRightArrow_UI_rect.X - StanceLeftArrow_UI_rect.Location.X - StanceLeftArrow_UI.Width));
				}
				else
				{
#if DEBUG
						Console.WriteLine("CBSYS AS:  INVALID KEY FOUND IGNORING [{0}]", pressedkey.ToString());
#endif
					return;
				}
			}
			else
			{
#if DEBUG
				Console.WriteLine("CBSYS AS:  INVALID STATE IGNORING [{0}]", currentCombatState);
#endif
			}

			CurrentPartyMember_turn.StanceIndicator_Color = StanceToColor(CurrentPartyMember_turn.CurrentStance.Value);
			
			//At this point the stance has been set/found.
			int pm = (followUpAttack_List.FindIndex(x => x.Item1 == CurrentPartyMember_turn));
			
			if (pm != -1)
			{
				//It has been found so remove it.
				CombatParticleEmitters_List.FindAll(x => x.LinkedParentObject == CurrentPartyMember_turn)?.ToList().ForEach(x=>x.HardReset());
				followUpAttack_List.RemoveAt(pm);
				followUpAttack_List.Add(new Tuple<BattleEntity, EStanceType>(CurrentPartyMember_turn, CurrentPartyMember_turn.CurrentStance.Value));
			}
			else
				followUpAttack_List.Add(new Tuple<BattleEntity, EStanceType>(CurrentPartyMember_turn, CurrentPartyMember_turn.CurrentStance.Value));





			//Now check for a match
			//TODO: Make this a helper method instead.
			List<Tuple<BattleEntity, EStanceType>> tempfollowattacks = followUpAttack_List.FindAll(x => x.Item2 == CurrentPartyMember_turn.CurrentStance.Value);
			List<Tuple<BattleEntity, EStanceType>> tempfollowattacks_Past = followUpAttack_List.FindAll(x => x.Item2 == pastStanceType);
			if (tempfollowattacks_Past.Count < 2)
			{
				CombatParticleEmitters_List.FindAll(x => x.LinkedParentObject == tempfollowattacks_Past.FirstOrDefault()?.Item1)?.ToList().ForEach(x=>x.HardReset());
			}
			if (tempfollowattacks.Count >= 2)
			{
				foreach (Tuple<BattleEntity, EStanceType> pmTuple in tempfollowattacks)
				{
					if (CombatParticleEmitters_List.Any(x => x.LinkedParentObject == pmTuple.Item1))
					{
						//THERE IS A MATCH! However we already Particle systems for this character. So just reenable them!
						CombatParticleEmitters_List.FindAll(x => x.LinkedParentObject == pmTuple.Item1)?.ToList().ForEach(x => x.Start());
						continue;
					}

					//there is a MATCH! And we have yet spawn a particle system for this character stance matching. So let's do that.
					Texture2D particleImg = this.ContentRef.Load<Texture2D>("Images/TestSprites_Ari/Sparkle");
					this.QueueCombatAction(new CombatParticleSystemAction(this,
						new Rectangle(0, 0, 1920, 1080),
						new Rectangle(0, 0, particleImg.Width, particleImg.Height), 20,
						200, 2, true,
						particleImg, 1f,
						.15f, .15f,
						false,
						-50f, 100,
						255, 285,
						200, 400,
						500, 1000,
						new Random(), pmTuple.Item1.Position + (new Vector2(pmTuple.Item1.Width/2, pmTuple.Item1.Height) * new Vector2(pmTuple.Item1.ScaleX, pmTuple.Item1.ScaleY)),
							(pmTuple.Item1 as PartyMember).StanceIndicator_Color, pmTuple.Item1
						){bCycleParticles = true});

				}


			}

		}

		public Color StanceToColor(EStanceType eStance)
		{
			switch (eStance)
			{
				case EStanceType.NONE:
					return Color.White;
					break;
				case EStanceType.Immovable:
					return new Color(255, 66, 0); //red orange
					break;
				case EStanceType.PewPew:
					return new Color(9, 0, 255); //blue
					break;
				case EStanceType.Reading:
					return new Color(9, 255, 0); //green
					break;
				case EStanceType.Praying:
					return new Color(255, 211, 0); //gold
					break;
				case EStanceType.Focus:
					return new Color(155, 0, 255); //purple
					break;
				case EStanceType.Bloody:
					return new Color(215, 9, 47); //gold
					break;
				case EStanceType.Singing:
					return new Color(255, 92, 235); //green
					break;
				case EStanceType.Feather:
					return new Color(0, 255, 173); //teal
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eStance), eStance, null);
			}
		}

		private void ClearPastState()
		{
			pastCombatState = ECombatState.NONE;
		}

		private void DespawnUI()
		{

		}


		private void CheckForEntityModifiers_Attack(BattleEntity battleEntity, int startindex = 0)
		{

			if (battleEntity is Enemy currentEnemy)
			{
				//Does this enemy have any modifiers we need to apply?
				for (int i = startindex; i < currentEnemy.GetEquipment().Length; i++)
				{
					if (currentEnemy.GetEquipment()[i] != null)
					{
						//Effects
						foreach (ModifierData md in currentEnemy.GetEquipment()[i].Effects)
						{
							if (!(md.Use_Type == EModifierUseType.ATTACK || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityAttackEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
							else
							{
								//it should always happen.
								EntityAttackEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
						}

						//Traits
						foreach (ModifierData md in currentEnemy.GetEquipment()[i].Traits)
						{
							if (!(md.Use_Type == EModifierUseType.ATTACK || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityAttackTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
							else
							{
								//it should always happen.
								EntityAttackTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
						}
					}
				}
			}
			else if (battleEntity is PartyMember currentPartyMember)
			{
				//Does this enemy have any modifiers we need to apply?
				for (int i = startindex; i < currentPartyMember.GetEquipment().Length; i++)
				{
					if (currentPartyMember.GetEquipment()[i] != null)
					{
						//Effects
						foreach (ModifierData md in currentPartyMember.GetEquipment()[i].Effects)
						{
							if (!(md.Use_Type == EModifierUseType.ATTACK || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityAttackEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
							else
							{
								//it should always happen.
								EntityAttackEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
						}

						//Traits
						foreach (ModifierData md in currentPartyMember.GetEquipment()[i].Traits)
						{
							if (!(md.Use_Type == EModifierUseType.ATTACK || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityAttackTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
							else
							{
								//it should always happen.
								EntityAttackTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
						}
					}
				}
			}
		}

		private void CheckForEntityModifiers_Defense(BattleEntity battleEntity, int startindex =0)
		{

			if (battleEntity is Enemy currentEnemy)
			{
				//Does this enemy have any modifiers we need to apply?
				for (int i = startindex; i < currentEnemy.GetEquipment().Length; i++)
				{
					if (currentEnemy.GetEquipment()[i] != null)
					{
						//Effects
						foreach (ModifierData md in currentEnemy.GetEquipment()[i].Effects)
						{
							if (!(md.Use_Type == EModifierUseType.DEFENSE || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int) md.Chance_Modifiers))
									EntityDefenseEffectModifier_List.Add( new Tuple<BattleEntity, ModifierData>(currentEnemy,md));
							}
							else
							{
								//it should always happen.
								EntityDefenseEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
						}

						//Traits
						foreach (ModifierData md in currentEnemy.GetEquipment()[i].Traits)
						{
							if (!(md.Use_Type == EModifierUseType.DEFENSE || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int) md.Chance_Modifiers))
									EntityDefenseTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
							else
							{
								//it should always happen.
								EntityDefenseTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
						}
					}
				}
			}
			else if (battleEntity is PartyMember currentPartyMember)
			{
				//Does this enemy have any modifiers we need to apply?
				for (int i = startindex; i < currentPartyMember.GetEquipment().Length; i++)
				{
					if (currentPartyMember.GetEquipment()[i] != null)
					{
						//Effects
						foreach (ModifierData md in currentPartyMember.GetEquipment()[i].Effects)
						{
							if (!(md.Use_Type == EModifierUseType.DEFENSE || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityDefenseEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
							else
							{
								//it should always happen.
								EntityDefenseEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
						}

						//Traits
						foreach (ModifierData md in currentPartyMember.GetEquipment()[i].Traits)
						{
							if (!(md.Use_Type == EModifierUseType.DEFENSE || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityDefenseTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
							else
							{
								//it should always happen.
								EntityDefenseTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
						}
					}
				}
			}
		}

		private void CheckForEntityModifiers_Buff(BattleEntity battleEntity, int startindex = 0)
		{

			if (battleEntity is Enemy currentEnemy)
			{
				//Does this enemy have any modifiers we need to apply?
				for (int i = startindex; i < currentEnemy.GetEquipment().Length; i++)
				{
					if (currentEnemy.GetEquipment()[i] != null)
					{
						//Effects
						foreach (ModifierData md in currentEnemy.GetEquipment()[i].Effects)
						{
							if (!(md.Use_Type == EModifierUseType.BUFF || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityBuffEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
							else
							{
								//it should always happen.
								EntityBuffEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
						}

						//Traits
						foreach (ModifierData md in currentEnemy.GetEquipment()[i].Traits)
						{
							if (!(md.Use_Type == EModifierUseType.BUFF || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityBuffTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
							else
							{
								//it should always happen.
								EntityBuffTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
						}
					}
				}
			}
			else if (battleEntity is PartyMember currentPartyMember)
			{
				//Does this enemy have any modifiers we need to apply?
				for (int i = startindex; i < currentPartyMember.GetEquipment().Length; i++)
				{
					if (currentPartyMember.GetEquipment()[i] != null)
					{
						//Effects
						foreach (ModifierData md in currentPartyMember.GetEquipment()[i].Effects)
						{
							if (!(md.Use_Type == EModifierUseType.BUFF || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityBuffEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
							else
							{
								//it should always happen.
								EntityBuffEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
						}

						//Traits
						foreach (ModifierData md in currentPartyMember.GetEquipment()[i].Traits)
						{
							if (!(md.Use_Type == EModifierUseType.BUFF || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityBuffTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
							else
							{
								//it should always happen.
								EntityBuffTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
						}
					}
				}
			}
		}

		private void CheckForEntityModifiers_Debuff(BattleEntity battleEntity, int startindex = 0)
		{

			if (battleEntity is Enemy currentEnemy)
			{
				//Does this enemy have any modifiers we need to apply?
				for (int i = startindex; i < currentEnemy.GetEquipment().Length; i++)
				{
					if (currentEnemy.GetEquipment()[i] != null)
					{
						//Effects
						foreach (ModifierData md in currentEnemy.GetEquipment()[i].Effects)
						{
							if (!(md.Use_Type == EModifierUseType.DEBUFF || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityDebuffEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
							else
							{
								//it should always happen.
								EntityDebuffEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
						}

						//Traits
						foreach (ModifierData md in currentEnemy.GetEquipment()[i].Traits)
						{
							if (!(md.Use_Type == EModifierUseType.DEBUFF || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityDebuffTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
							else
							{
								//it should always happen.
								EntityDebuffTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentEnemy, md));
							}
						}
					}
				}
			}
			else if (battleEntity is PartyMember currentPartyMember)
			{
				//Does this enemy have any modifiers we need to apply?
				for (int i = startindex; i < currentPartyMember.GetEquipment().Length; i++)
				{
					if (currentPartyMember.GetEquipment()[i] != null)
					{
						//Effects
						foreach (ModifierData md in currentPartyMember.GetEquipment()[i].Effects)
						{
							if (!(md.Use_Type == EModifierUseType.DEBUFF || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityDebuffEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
							else
							{
								//it should always happen.
								EntityDebuffEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
						}

						//Traits
						foreach (ModifierData md in currentPartyMember.GetEquipment()[i].Traits)
						{
							if (!(md.Use_Type == EModifierUseType.DEBUFF || md.Use_Type == EModifierUseType.WILDCARD)) continue;
							//chance time?
							if (md.Chance_Modifiers > 0)
							{
								//roll for chance
								if (RollCheck((int)md.Chance_Modifiers))
									EntityDebuffTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
							else
							{
								//it should always happen.
								EntityDebuffTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(currentPartyMember, md));
							}
						}
					}
				}
			}
		}

		private bool RollCheck(int rolltype)
		{
			bool bret = false;
			Random r = new Random();
			if (rolltype <= 1) //5%
			{
				if (r.Next(100) > 5)
					bret = true;
			}
			else if (rolltype <= 2) //10%
			{
				if (r.Next(100) > 10)
					bret = true;
			}
			else if (rolltype <= 4) //20%
			{
				if (r.Next(100) > 20)
					bret = true;
			}
			else if (rolltype <= 8) //30%
			{
				if (r.Next(100) > 30)
					bret = true;
			}
			else if (rolltype <= 16) //40%
			{
				if (r.Next(100) > 40)
					bret = true;
			}
			else if (rolltype <= 32) //50%
			{
				if (r.Next(100) > 50)
					bret = true;
			}
			else if (rolltype <= 64) //100%
			{
				bret = true;
			}
			return bret;
		}

		#endregion

		//private void WaitingForInput(EBattleCommand command)
		//{
		//	if (command != EBattleCommand.NONE)
		//	{
		//		//We will decide what to do BASED on the past state
		//		switch (pastCombatState)
		//		{
		//			case ECombatState.StartPhysicalAttack:
		//				CombatState = ECombatState.WaitingForInput;
		//				break;
		//			case ECombatState.StartSkillsAttack:
		//				//At this point the player has chosen the skill they want to use. But not the target.
		//				//Change state to => ChoosingTarget.
		//				break;
		//			case ECombatState.StartItemUse:
		//				//At this point the player has chosen the item they want to use. But not the target.
		//				//Change State to => "ChoosingTarget"
		//				break;
		//			//case ECombatState.ItemUse:
		//			//	break;
		//			case ECombatState.ChoosingOptions:
		//				ChooseOptions(command);
		//				break;
		//			case ECombatState.ChooseTarget:
		//				ChooseTarget(command);
		//				break;
		//			default:
		//				break;
		//		}
		//	}
		//}

		///// <summary>
		///// Choose the options bases on the PAST States.
		///// </summary>
		//private void ChooseOptions(EBattleCommand command)
		//{
		//	//We have found an input, but we need to figure out what the player has chosen.
		//	switch (command)
		//	{
		//		case EBattleCommand.NONE:
		//			break;
		//		case EBattleCommand.ATTACK:
		//			//Physical => change state to "StartPhysicalAttack"
		//			pastCombatState = ECombatState.ChoosingOptions;
		//			CombatState = ECombatState.StartPhysicalAttack;
		//			break;
		//		case EBattleCommand.SKILLS:
		//			//Skills => change state to "StartSkillsAttack"
		//			break;
		//		case EBattleCommand.DEFENSE:
		//			//Defense => change state to "Defense"
		//			break;
		//		case EBattleCommand.ITEMS:
		//			//Item => change state to "StartItemUse" with the Item Chosen UI data.
		//			break;
		//		default:
		//			throw new ArgumentOutOfRangeException();
		//	}
		//}

		///// <summary>
		///// Choose the target This is based on PAST states.
		///// </summary>
		//private void ChooseTarget(EBattleCommand command)
		//{
		//	//We need to decide do here. So 
		//	//if Directional Arrow Move the Chosen Target and arrow to the next guy.
		//	//if Execute Command use the saved Command Data to decide next state AND save the target.
		//	switch (command)
		//	{
		//		case EBattleCommand.NONE:
		//			break;
		//		case EBattleCommand.ATTACK:
		//			//Physical => change state to "Attack"
		//			break;
		//		case EBattleCommand.SKILLS:
		//			//Skills => change state to "Attack"
		//			break;
		//		case EBattleCommand.ITEMS:
		//			//Item => change state to "ItemUse"
		//			break;
		//		default:
		//			throw new ArgumentOutOfRangeException();
		//	}
		//}

		public void StartNewBattle()
		{
			combatState = ECombatState.BattleStart;
		}

		/// <summary>
		/// //there are two stats here that matter. One Luck, Two Risk.
		/// //Luck is base. In otherwords its El(l) - El(2)
		/// 
		/// 
		/// //Risk is different. It will roll a dice number. If greater than X Vs Y then it will return this number as a decimal.
		/// //With this decimal we will multiply it by a base whole number. then TEMP add to luck stat. For the actual roll if this modifier goes through.
		/// //IF ELSE the risk roll fails then we take the base number and multiply it by the returned decimal value. THEN we subtract the luck and roll for real.
		/// 
		/// //It should go without saying that the higher the risk the higher the reward. 
		/// </summary>
		/// <param name="Instigator"></param>
		/// <param name="Target"></param>
		/// <param name="currentModifierData"></param>
		private void RollAttackModifiersChanceRate(BattleEntity Instigator, BattleEntity Target, ModifierData currentModifierData)
		{
			if(currentModifierData.Use_Type != EModifierUseType.ATTACK) return;

			int modifierchance = 0; //TODO: add the modifier chance data
			double templuck = Instigator.Stats.Luck - Target.Stats.Luck;
			double rng = combatRNG.NextDouble();
			//The higher level you are, the harder it will be to gain a risk roll == true. that's what this does. 
			//If you do succeed it will add this, but if you fail it will subtract this, but with a cushion of half.
			templuck += (rng * 4.096 > Math.Pow(2, ((double)Instigator.Stats.Risk / 8.0)) / 1000.0 ? 100 * rng : 100 * rng * -.5);

			//The returned Luck value will be added or subtracts to the Roll threshold. Assuming we scale all entity luck with levels
			if (combatRNG.Next(0, 100) < 1 + (templuck <= 0 ? 1 : templuck) + modifierchance)
			{
				//Success on roll!
				if(currentModifierData.bEffect)
					EntityAttackEffectModifier_List.Add(new Tuple<BattleEntity, ModifierData>(Instigator, currentModifierData));
				else
					EntityAttackTraitModifier_List.Add(new Tuple<BattleEntity, ModifierData>(Instigator, currentModifierData));

				//TODO: Handle case to add boosts IF extreme amounts of risk are found.

			}
			else
			{
				return; //failure on roll
			}
		}

	public void LoadCombatDataFromDatabase(String databasepath)
		{

			//create a partymemeber and fill out the objects internal object references.
			String masterfile = (databasepath);
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = "";

				Createsql = "SELECT * FROM `base_stats`;";
				List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

			}
			catch (Exception ex)
			{
				Console.WriteLine(String.Format("Error Found in loading database to combat system {0}", ex.Message));
			}
			finally
			{
				//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
				//GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
				//GameplayModifierName_CB.SelectedIndex = absindex;
			}

		}


		public void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
		{
			//Local flag(s) to indicate a shader flash is needed
			List<BattleEntity> toApplyShadersTo = new List<BattleEntity>();

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

			if (debug_LogBlock != null && (debug_LogBlock as GameTextBlock)._font != null)
			{
				debug_LogBlock.Draw(gameTime, spriteBatch);
			}

			if (Stance_GTB != null && (Stance_GTB as GameTextBlock)._font != null)
			{
				Stance_GTB.Draw(gameTime, spriteBatch);
			}

			if (Weapon_GTB != null && (Weapon_GTB as GameTextBlock)._font != null)
			{
				Weapon_GTB.Draw(gameTime, spriteBatch);
			}

			if (CurrentPartyMembersNames.Count == 0)
			{
				spriteBatch.End();
				return;
			}
			
			foreach (String TempName in CurrentPartyMembersNames)
			{
				if (PartyMembers.ContainsKey(TempName))
				{ 
					if((PartyMembers[TempName].ShaderHitTimer > 0 && PartyMembers[TempName].ShaderHitFlash > 0))
						toApplyShadersTo.Add(PartyMembers[TempName]);
					else if (!PartyMembers[TempName].bIsDead )
						PartyMembers[TempName].Draw(gameTime, spriteBatch);
				}
				else
				{
					//throw new Exception(String.Format("Invaild Character Name {0} Found WHEN Drawing!", TempName));
				}
			}


			foreach (String TempName in CurrentEnemiesNames)
			{
				if (BattleEnemyList.ContainsKey(TempName))
				{
					if ((BattleEnemyList[TempName].ShaderHitTimer > 0 && BattleEnemyList[TempName].ShaderHitFlash > 0))
						toApplyShadersTo.Add(BattleEnemyList[TempName]);
					else if (!BattleEnemyList[TempName].bIsDead)
						BattleEnemyList[TempName].Draw(gameTime, spriteBatch);
				}
				else
				{
					//throw new Exception(String.Format("Invaild Character Name {0} Found WHEN Drawing!", TempName));
				}
			}
			//Ui for the beginning ofm the characters turn
			if(bDrawAttackSticker_UI)
				spriteBatch.Draw(AttackSticker_UI, AttackSticker_UI_rect, Color.White);
			if (bDrawDefenseSticker_UI)
				spriteBatch.Draw(DefenseSticker_UI, DefenseSticker_UI_rect, Color.White);
			if (bDrawSkillsSticker_UI)
				spriteBatch.Draw(SkillsSticker_UI, SkillsSticker_UI_rect, Color.White);
			
			//Stances and weapons Arrows
			if(bDrawStanceLeftArrow_UI)
				spriteBatch.Draw(StanceLeftArrow_UI, StanceLeftArrow_UI_rect, Color.White);
			if (bDrawWeaponLeftArrow_UI)
				spriteBatch.Draw(WeaponLeftArrow_UI, WeaponLeftArrow_UI_rect, Color.White);
			if (bDrawStanceRightArrow_UI)
				spriteBatch.Draw(StanceRightArrow_UI, StanceRightArrow_UI_rect, Color.White);
			if (bDrawWeaponRightArrow_UI)
				spriteBatch.Draw(WeaponRightArrow_UI, WeaponRightArrow_UI_rect, Color.White);

			//Inventory Sticker
			if (bDrawInventorySticker_UI)
				spriteBatch.Draw(InventorySticker_UI, InventorySticker_UI_rect, Color.White);
			//Weapon Card 
			if (bDrawWeaponCardSticker_UI)
				spriteBatch.Draw(WeaponCardSticker_UI, WeaponCardSticker_UI_rect, Color.White);


			if (bDrawSelectArrowUI)
				spriteBatch.Draw(SelectArrowUI, new Rectangle(new Point((int)SelectArrowUI_Pos.X, (int)SelectArrowUI_Pos.Y), new Point(50,50) ), Color.AliceBlue); //scaling
			if (bDrawSelectArrowAreaRange)
				spriteBatch.Draw(SelectArrowAreaRange_UI, SelectArrowAreaRange_Rect , Color.Red); //AOE cursor.



			if (bDrawSkillMenu_UI)
			{
				//spriteBatch.Draw(SkillMenu_UI, new Rectangle(
				//		new Point((int) SKillsMenu_UI_pos.X, (int) SKillsMenu_UI_pos.Y), new Point(
				//			(int) (267 * .6),
				//			(int) (255 * .6))),
				//	Color.AliceBlue); //scaling
			}

			foreach (GameListBox GLB in _partyMemberSkillListBoxes_dict.Values)
			{
				GLB.Draw(gameTime, spriteBatch);
			}

			foreach (GameListBox GLB in _partyMemberItemsListBoxes_dict.Values)
			{
				GLB.Draw(gameTime, spriteBatch);
			}
			
			spriteBatch.End();

			
			//This is here for flashing the character sprites if they get hit.
			if (toApplyShadersTo.Count > 0)
			{
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				for (int i = 0; i < toApplyShadersTo.Count; i++) // (BattleEntity bc in toApplyShadersTo)
				{
					BattleEntity bc = toApplyShadersTo[i];

					if (combatState != ECombatState.ChooseTarget)
						bc.ShaderHitActuatTimer--;
					if (bc.ShaderHitActuatTimer <= 0)
					{
						bc.ShaderHitFlash--;
						//textureEffect.CurrentTechnique.Passes[1].Apply(); //red
						if (bc.ShaderHitFlash > 0)
						{
							bc.ShaderHitActuatTimer = bc.ShaderHitTimer;
						}
						else
						{
							toApplyShadersTo.Remove(bc);

						}
					}
					else
					{
						if (bc.ShaderHitFlash % 2 == 1 && combatState != ECombatState.ChooseTarget)
						{
							textureEffect.CurrentTechnique.Passes[2].Apply(); //red
						}
						else
						{
							if(combatState != ECombatState.ChooseTarget)
								textureEffect.CurrentTechnique.Passes[1].Apply(); //grey
							else
								textureEffect.CurrentTechnique.Passes[9].Apply(); //blue highlight
						}
					}
					bc.Draw(gameTime, spriteBatch);
				}
				spriteBatch.End();
			}

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

			//We need MORE than one particle system at time. 
			for (int i = 0; i < CombatParticleEmitters_List.Count; i++)
			{
				CombatParticleEmitters_List[i]?.Draw(spriteBatch, gameTime, Color.Red);
			}

			for (int i = 0; i < CombatUI_List.Count; i++)
			{
				if (!CombatUI_List[i].bIsActive)
					continue;
				if (CombatUI_List[i] is BixBite.Rendering.UI.Image.GameImage b1)
					b1.Draw(gameTime, spriteBatch);
				else if (CombatUI_List[i] is BixBite.Rendering.UI.ListBox.GameListBox b2)
					b2.Draw(gameTime, spriteBatch);
				else if (CombatUI_List[i] is BixBite.Rendering.UI.TextBlock.GameTextBlock b3)
					b3.Draw(gameTime, spriteBatch);
				else if (CombatUI_List[i] is BixBite.Rendering.UI.TextBlock.GameTextBlock b4)
					b4.Draw(gameTime, spriteBatch);
				else if (CombatUI_List[i] is BixBite.Rendering.UI.ProgressBar.GameProgressBar b5)
					b5.Draw(gameTime, spriteBatch);
				else if (CombatUI_List[i] is BixBite.Rendering.UI.Button.GameButton b6)
					b6.Draw(gameTime, spriteBatch);
				System.Diagnostics.Debug.WriteLine("X: {0} || y: {1}", CombatUI_List[i].XPos, CombatUI_List[i].YPos);
			}

			//TurnQueue
			TurnQueue_GameListBox.Draw(gameTime, spriteBatch);

			//Essence Bar Main
			EssenceBar.Draw(gameTime, spriteBatch);
			PartyMemberEssenceBar.Draw(gameTime, spriteBatch);

			spriteBatch.End();


		}

	}
}
