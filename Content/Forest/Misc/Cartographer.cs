using SpiritReforged.Common.ItemCommon.Pins;
using SpiritReforged.Common.MapCommon;
using SpiritReforged.Common.Misc;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Content.Forest.Misc.Pins;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Forest.Misc;

internal class Cartographer : ModNPC
{
	protected override bool CloneNewInstances => true;

	private bool _hasPin = true;

	public override ModNPC Clone(NPC newEntity)
	{
		var newNPC = base.Clone(newEntity);
		var cartographer = newNPC as Cartographer;
		cartographer._hasPin = _hasPin;
		return newNPC;
	}

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 1;

		NPCID.Sets.ActsLikeTownNPC[Type] = true;
		NPCID.Sets.NoTownNPCHappiness[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.CloneDefaults(NPCID.SkeletonMerchant);
		NPC.townNPC = true;
		NPC.Size = new Vector2(30, 40);
	}

	public override void SetChatButtons(ref string button, ref string button2)
	{
		button = Language.GetTextValue("LegacyInterface.28");
		button2 = !PointOfInterestSystem.HasAnyInterests() || !_hasPin ? string.Empty : Language.GetTextValue("Mods.SpiritReforged.NPCs.Cartographer.Buttons.Map");
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		if (firstButton)
			shopName = "Shop";
		else
			MapFunctionality();
	}

	public override void AddShops() => new NPCShop(Type).Add<PinRed>().Add<PinYellow>().Add<PinGreen>().Add<PinBlue>().Register();

	private void MapFunctionality()
	{
		const int Radius = 60;

		InterestType type;

		do 
		{
			type = (InterestType)Main.rand.Next((int)InterestType.Count);
		} while (!PointOfInterestSystem.HasInterestType(type));

		var item = new Item(GetPinType(type));
		string pinName = item.ModItem.Name;
		bool firstPin = Main.LocalPlayer.GetModPlayer<PinPlayer>().unlockedPins.Count == 0;

		if (Main.LocalPlayer.PinUnlocked(pinName))
			Main.LocalPlayer.QuickSpawnItem(new EntitySource_Gift(NPC), item); //If the pin is already unlocked, give the player an item copy
		else
			Main.LocalPlayer.UnlockPin(pinName);

		Point16 point = PointOfInterestSystem.GetPoint(type);
		PinSystem.Place(pinName, point.ToVector2());
		PointOfInterestSystem.RemovePoint(point, type);

		string text = Language.GetTextValue("Mods.SpiritReforged.NPCs.Cartographer.Dialogue.Map." + type + "." + Main.rand.Next(3));
		if (firstPin)
			text += " " + Language.GetTextValue("Mods.SpiritReforged.NPCs.Cartographer.Dialogue.Map.FirstPin");

		Main.npcChatText = text;
		Main.npcChatCornerItem = item.type;

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

		_hasPin = false;

		static int GetPinType(InterestType interest)
		{
			int type = interest switch
			{
				InterestType.FloatingIsland => ModContent.ItemType<PinSky>(),
				InterestType.EnchantedSword => ModContent.ItemType<PinSword>(),
				InterestType.ButterflyShrine => ModContent.ItemType<PinButterfly>(),
				InterestType.Shimmer => ModContent.ItemType<PinFaeling>(),
				InterestType.Savanna => ModContent.ItemType<PinSavanna>(),
				InterestType.Hive => ModContent.ItemType<PinHive>(),
				InterestType.Curiosity => ModContent.ItemType<PinCuriosity>(),
				_ => Main.rand.Next([.. ModContent.GetContent<PinItem>()]).Type //Random
			};

			return type;
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (Main.dedServ)
			return;

		for (int d = 0; d < 8; d++)
			Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(NPC.getRect()), DustID.Blood,
				Main.rand.NextVector2Unit() * 1.5f, 0, default, Main.rand.NextFloat(1f, 1.5f));
	}
}