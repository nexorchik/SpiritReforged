using SpiritReforged.Common.ItemCommon.Backpacks;
using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Content.Savanna.Items.BaobabFruit;
using SpiritReforged.Content.Savanna.Items.Gar;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace SpiritReforged.Content.Forest.Misc;

internal class Hiker : ModNPC, INPCButtons
{
	/// <summary> Stores all information for the hiker to pass properly between clones. </summary>
	private class HikerInfo
	{
		/// <summary> If the hiker has a bundle to sell. </summary>
		public bool hasBundle = true;

		/// <summary> If the hiker has been fed, and now gives away the bundle for free. </summary>
		public bool priceOff = false;

		/// <summary> If the hiker has a pin to put on your map. </summary>
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

	private static Asset<Texture2D> stickTexture;
	private static Profiles.StackedNPCProfile npcProfile;

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
		Main.npcFrameCount[Type] = 25;

		NPCID.Sets.ActsLikeTownNPC[Type] = true;
		NPCID.Sets.NoTownNPCHappiness[Type] = true;
		NPCID.Sets.ExtraFramesCount[Type] = 9;
		NPCID.Sets.AttackFrameCount[Type] = 4;
		NPCID.Sets.DangerDetectRange[Type] = 500;
		NPCID.Sets.PrettySafe[Type] = 50;
		NPCID.Sets.AttackType[Type] = 3;
		NPCID.Sets.AttackTime[Type] = 20;
		NPCID.Sets.HatOffsetY[Type] = 2;
		NPCID.Sets.AttackAverageChance[Type] = 30;

		stickTexture = ModContent.Request<Texture2D>(Texture + "Stick");
		npcProfile = new Profiles.StackedNPCProfile(new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"));
	}

	public override void SetDefaults()
	{
		NPC.CloneDefaults(NPCID.SkeletonMerchant);
		NPC.townNPC = true;
		NPC.Size = new Vector2(30, 40);

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
			button = string.Empty;

		//button2 = !PointOfInterestSystem.HasAnyInterests() || !_info.hasPin ? "" : Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Buttons.Map");
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		if (firstButton) // Buy bundle
		{
			int cost = Item.buyPrice(0, 0, 20, 0);

			if (Main.LocalPlayer.CanAfford(cost) && Main.LocalPlayer.PayCurrency(cost) && _info.hasBundle)
				SpawnBundle();
		}
		//else
		//	MapFunctionality();
	}

	/*private void MapFunctionality()
	{
		var item = new Item();
		PinItem pin = Main.rand.Next([.. ModContent.GetContent<PinItem>()]);
		item.SetDefaults(pin.Type);

		Main.LocalPlayer.QuickSpawnItem(new EntitySource_Gift(NPC), item);

		InterestType type;

		do 
		{
			type = (InterestType)Main.rand.Next((int)InterestType.Count);
		} while (!PointOfInterestSystem.HasInterestType(type));

		Point16 point = PointOfInterestSystem.GetPoint(type);
		PinSystem.Place(pin.PinName, point.ToVector2());
		PointOfInterestSystem.RemovePoint(point, type);

		Main.npcChatText = Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Dialogue.Map." + type + "." + Main.rand.Next(3));
		Main.npcChatCornerItem = pin.Type;

		const int Radius = 60;

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			NetMessage.SendData(MessageID.TileSection, -1, -1, null, point.X - Radius, point.Y - Radius, Radius * 2, Radius * 2);

			ModPacket packet = SpiritReforgedMod.Instance.GetPacket(ReforgedMultiplayer.MessageType.RevealMap, 4);
			packet.Write((byte)RevealMap.MapSyncId.DrawMap);
			packet.Write(point.X);
			packet.Write(point.Y);
			packet.Write((short)60);
			packet.Send();
		}
		else	
			RevealMap.DrawMap(point.X, point.Y, Radius);

		_info.hasPin = false;
	}*/

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

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (Main.dedServ)
			return;

		if (NPC.life <= 0)
		{
			for (int i = 1; i < 6; i++)
			{
				int goreType = Mod.Find<ModGore>(nameof(DevourerOfSoil) + i).Type;
				Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.velocity, goreType);
			}
		}

		for (int d = 0; d < 8; d++)
			Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(NPC.getRect()), DustID.Blood, 
				Main.rand.NextVector2Unit() * 1.5f, 0, default, Main.rand.NextFloat(1f, 1.5f));
	}

	public override ITownNPCProfile TownNPCProfile() => npcProfile;

	public override void TownNPCAttackStrength(ref int damage, ref float knockback)
	{
		damage = 15;
		knockback = 3f;
	}

	public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight) => itemWidth = itemHeight = 30;

	public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
	{
		item = stickTexture.Value;
		itemFrame = stickTexture.Frame();
		itemSize = 30;
	}
}
