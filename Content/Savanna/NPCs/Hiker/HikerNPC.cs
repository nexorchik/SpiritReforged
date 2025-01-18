using SpiritReforged.Common.NPCCommon;
using Terraria.DataStructures;
using SpiritReforged.Common.ItemCommon.Backpacks;
using SpiritReforged.Content.Forest.Backpacks;
using SpiritReforged.Content.Savanna.Items.Gar;
using Terraria.Utilities;

namespace SpiritReforged.Content.Savanna.NPCs.Hiker;

internal class HikerNPC : ModNPC, INPCButtons
{
	/// <summary>
	/// Stores all information for the hiker to pass properly between clones.
	/// </summary>
	private class HikerInfo
	{
		/// <summary>
		/// If the hiker has a bundle to sell.
		/// </summary>
		public bool hasBundle = true;

		/// <summary>
		/// If the hiker has been fed, and now gives away the bundle for free.
		/// </summary>
		public bool priceOff = false;
	}

	public static WeightedRandom<(int, Range)> ItemPool
	{
		get
		{
			WeightedRandom<(int, Range)> pool = new();
			pool.Add((ItemID.Glowstick, 6..12), 1);
			pool.Add((ItemID.Rope, 25..35), 1);
			pool.Add((ItemID.SwiftnessPotion, 1..3), 0.8f);
			pool.Add((ItemID.Bomb, 5..10), 0.5f);
			pool.Add((ModContent.ItemType<QuenchPotion>(), 1..2), 0.3f);
			pool.Add((ItemID.Dynamite, 1..2), 0.1f);
			return pool;
		}
	}

	protected override bool CloneNewInstances => true;

	public bool Hungry
	{
		get => NPC.ai[3] == 0;
		set => NPC.ai[3] = value ? 0 : 1;
	}

	private HikerInfo _info = new();

	public override ModNPC Clone(NPC newEntity)
	{
		var newNPC = base.Clone(newEntity);
		var hiker = newNPC as HikerNPC;
		hiker._info = _info;
		return newNPC;
	}

	public override void SetStaticDefaults()
	{
		NPCID.Sets.ActsLikeTownNPC[Type] = true;
		NPCID.Sets.NoTownNPCHappiness[Type] = true;

		Main.npcFrameCount[Type] = 24;
		NPCID.Sets.ActsLikeTownNPC[Type] = true;
		NPCID.Sets.NoTownNPCHappiness[Type] = true;
		NPCID.Sets.ExtraFramesCount[Type] = 9;
		NPCID.Sets.AttackFrameCount[Type] = 4;
		NPCID.Sets.DangerDetectRange[Type] = 600;
		NPCID.Sets.AttackType[Type] = -1;
		NPCID.Sets.AttackTime[Type] = 20;
		NPCID.Sets.HatOffsetY[Type] = 2;
	}

	public override void SetDefaults()
	{
		NPC.CloneDefaults(NPCID.SkeletonMerchant);
		NPC.townNPC = true;
		NPC.Size = new Vector2(30, 40);
		NPC.aiStyle = NPCAIStyleID.Passive;

		AIType = NPCID.Guide;
		AnimationType = NPCID.Guide;

		_info = new();
	}

	public override List<string> SetNPCNameList()
	{
		List<string> nameList = [];

		for (int i = 0; i < 6; i++)
			nameList.Add(Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Name." + i));

		return nameList;
	}

	public override void SetChatButtons(ref string button, ref string button2)
	{
		if (_info.hasBundle)
		{
			string silver = Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Buttons.Silver");
			button = Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Buttons.Supplies") + (_info.priceOff ? "" : $"([c/AAAAAA:{silver}])");
		}
		else
			button = "";
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		if (firstButton) // Buy bundle
		{
			int cost = Item.buyPrice(0, 0, 20, 0);

			if (Main.LocalPlayer.CanAfford(cost) && Main.LocalPlayer.PayCurrency(cost) && _info.hasBundle)
			{
				SpawnBundle();

				Main.npcChatText = Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Dialogue.Purchase." + Main.rand.Next(5));
			}
			else
				Main.npcChatText = Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Dialogue.FailPurchase." + Main.rand.Next(3));
		}
	}

	private void SpawnBundle()
	{
		var newItem = new Item(ModContent.ItemType<LeatherBackpack>());
		var backpack = newItem.ModItem as BackpackItem;

		for (int i = 0; i < backpack.items.Length; ++i)
		{
			Item item = backpack.items[i];
			(int type, Range stackRange) = ItemPool.Get();
			item.SetDefaults(type);
			item.stack = Main.rand.Next(stackRange.Start.Value, stackRange.End.Value + 1);
		}

		Main.LocalPlayer.QuickSpawnItem(new EntitySource_Gift(NPC), newItem);
		_info.hasBundle = false;
	}

	public override string GetChat()
	{
		if (Hungry && !_info.priceOff && PlayerHasFood(out int type))
			return Language.GetText("Mods.SpiritReforged.NPCs.Hiker.Dialogue.Hungry.Asking." + Main.rand.Next(4)).WithFormatArgs($"[i:{type}]").Value;

		return Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Dialogue.Idle." + Main.rand.Next(5));
	}

	private static bool PlayerHasFood(out int itemId)
	{
		itemId = -1;

		for (int i = 0; i < Main.LocalPlayer.inventory.Length; ++i)
		{
			Item item = Main.LocalPlayer.inventory[i];

			if (item.potion || ItemID.Sets.IsFood[item.type])
			{
				itemId = item.type;
				return true;
			}
		}

		return false;
	}

	public ButtonText[] AddButtons()
	{
		if (_info.hasBundle && !_info.priceOff && PlayerHasFood(out _))
			return [new ButtonText("Feed", Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Buttons.Feed"))];

		return [];
	}

	public void OnClickButton(ButtonText button)
	{
		if (button.Name == "Feed" && PlayerHasFood(out int id))
		{
			Main.LocalPlayer.ConsumeItem(id);
			Main.npcChatText = Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Dialogue.Hungry.Thanks." + Main.rand.Next(4));

			AdvancedPopupRequest request = new()
			{
				Color = Color.Red,
				DurationInFrames = 120,
				Text = $"-1 {Lang.GetItemNameValue(id)}",
				Velocity = new Vector2(0, -16)
			};

			PopupText.NewText(request, Main.LocalPlayer.Center);

			Hungry = false;
			_info.priceOff = true;
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			for (int i = 0; i < 6; ++i)
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Hiker_" + i).Type, 1f);
	}
}