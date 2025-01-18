using SpiritReforged.Common.ItemCommon.Pins;
using SpiritReforged.Common.MapCommon;
using SpiritReforged.Common.NPCCommon;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Content.Forest.Misc.Pins;
using SpiritReforged.Content.Forest.Misc;
using SpiritReforged.Content.Savanna.Biome;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;

internal class Hiker : ModNPC
{
	//protected override bool CloneNewInstances => true;

	//private bool _hasPin = true;

	//public override ModNPC Clone(NPC newEntity)
	//{
	//	var cartographer = base.Clone(newEntity) as Cartographer;
	//	cartographer._hasPin = _hasPin;
	//	return cartographer;
	//}

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 25;

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
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.Size = new Vector2(30, 40);

		AnimationType = NPCID.Guide;
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "Surface");

	public override bool CanChat() => true;
	public override string GetChat() => Language.GetTextValue("Mods.SpiritReforged.NPCs.HikerNPC.Dialogue." + Main.rand.Next(4));

	public override List<string> SetNPCNameList()
	{
		List<string> names = [];

		for (int i = 0; i < 6; ++i)
			names.Add(Language.GetTextValue("Mods.SpiritReforged.NPCs.Cartographer.Names." + i));

		return names;
	}

	public override void SetChatButtons(ref string button, ref string button2)
	{
		button = Language.GetTextValue("LegacyInterface.28");
		//button2 = !PointOfInterestSystem.HasAnyInterests() || !_hasPin ? string.Empty : Language.GetTextValue("Mods.SpiritReforged.NPCs.Cartographer.Buttons.Map");
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		if (firstButton)
			shopName = "Shop";
		else
			MapFunctionality();
	}

	private void MapFunctionality()
	{
		
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (Main.dedServ)
			return;

		if (NPC.life <= 0)
		{
			for (int i = 1; i < 7; i++)
			{
				int goreType = Mod.Find<ModGore>(nameof(Cartographer) + i).Type;
				Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.velocity, goreType);
			}
		}

		for (int d = 0; d < 8; d++)
			Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(NPC.getRect()), DustID.Blood,
				Main.rand.NextVector2Unit() * 1.5f, 0, default, Main.rand.NextFloat(1f, 1.5f));
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (ModContent.GetInstance<WorldNPCFlags>().cartographerSpawned || spawnInfo.Invasion || spawnInfo.Water)
			return 0; //Never spawn during an invasion, in water or if already spawned that day

		if (spawnInfo.SpawnTileY > Main.worldSurface && spawnInfo.SpawnTileY < Main.UnderworldLayer)
			return .00018f; //Rarely spawn in caves above underworld height

		if ((spawnInfo.Player.InModBiome<SavannaBiome>() || spawnInfo.Player.ZoneDesert || spawnInfo.Player.ZoneJungle || OuterThirds(spawnInfo.SpawnTileX) && spawnInfo.Player.InZonePurity() && !spawnInfo.Player.ZoneSkyHeight) && Main.dayTime)
			return .0019f; //Spawn most commonly in the Savanna, Desert, Jungle, and outer thirds of the Forest during the day

		return 0;

		static bool OuterThirds(int x) => x < Main.maxTilesX / 3 || x > Main.maxTilesX - Main.maxTilesY / 3;
	}

	public override void OnSpawn(IEntitySource source) => ModContent.GetInstance<WorldNPCFlags>().cartographerSpawned = true;
}