using Microsoft.CodeAnalysis.Operations;
using SpiritReforged.Common.ItemCommon.Backpacks;
using SpiritReforged.Common.ItemCommon.Pins;
using SpiritReforged.Common.MapCommon;
using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Content.Savanna.Items.Gar;
using Terraria;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace SpiritReforged.Content.Forest.Misc;

internal class Hiker : PlayerContainerNPC, INPCButtons
{
	private class HikerInfo
	{
		public bool hasBundle = true;
		public bool priceOff = false;
		public bool hasPin = true;
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
		var hiker = newNPC as Hiker;
		hiker._info = _info;
		return newNPC;
	}

	public override void SetStaticDefaults()
	{
		NPCID.Sets.ActsLikeTownNPC[Type] = true;
		NPCID.Sets.NoTownNPCHappiness[Type] = true;
	}

	public override void Defaults()
	{
		NPC.CloneDefaults(NPCID.SkeletonMerchant);
		NPC.townNPC = true;
		NPC.Size = new Vector2(30, 40);

		_info = new();
	}

	protected override void PreDrawPlayer()
	{
		_drawDummy.head = ArmorIDs.Head.ArchaeologistsHat;
		_drawDummy.body = ArmorIDs.Body.ArchaeologistsJacket;
		_drawDummy.legs = ArmorIDs.Legs.ArchaeologistsPants;
		_drawDummy.back = EquipLoader.GetEquipSlot(Mod, "LeatherBackpack", EquipType.Back);
		_drawDummy.front = EquipLoader.GetEquipSlot(Mod, "LeatherBackpack", EquipType.Front);
		_drawDummy.hair = 2;
		_drawDummy.eyeColor = Color.Blue;
		_drawDummy.hairColor = new Color(63, 42, 33);
		_drawDummy.pantsColor = Color.Brown;
		_drawDummy.skinColor = new Color(206, 143, 76);
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

		button2 = !PointOfInterestSystem.HasAnyInterests() || !_info.hasPin ? "" : Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Buttons.Map");
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		if (firstButton) // Buy bundle
		{
			int cost = Item.buyPrice(0, 0, 20, 0);

			if (Main.LocalPlayer.CanAfford(cost) && Main.LocalPlayer.PayCurrency(cost) && _info.hasBundle)
				SpawnBundle();
		}
		else
			MapFunctionality();
	}

	private void MapFunctionality()
	{
		var item = new Item();
		PinItem pin = Main.rand.Next([.. ModContent.GetContent<PinItem>()]);
		item.SetDefaults(pin.Type);

		Main.LocalPlayer.QuickSpawnItem(new EntitySource_Gift(NPC), item);
		Main.NewText("111");

		InterestType type;

		do 
		{
			type = (InterestType)Main.rand.Next((int)InterestType.Count);
		} while (!PointOfInterestSystem.HasInterestType(type));

		Point16 point = PointOfInterestSystem.GetPoint(type);
		ModContent.GetInstance<PinSystem>().SetPin(pin.PinName, point.ToVector2());
		PointOfInterestSystem.RemovePoint(point, type);

		Main.NewText("222");

		Main.npcChatText = Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Dialogue.Map." + type + "." + Main.rand.Next(3));
		Main.npcChatCornerItem = pin.Type;

		RevealMap.DrawMap(point.X, point.Y, 60);

		Main.NewText("333");

		_info.hasPin = false;
	}

	private void SpawnBundle()
	{
		var newItem = new Item(ModContent.ItemType<LeatherBackpack>());
		var backpack = newItem.ModItem as BackpackItem;
		
		for (int i = 0; i < backpack.Items.Length; ++i)
		{
			Item item = backpack.Items[i];
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
}
