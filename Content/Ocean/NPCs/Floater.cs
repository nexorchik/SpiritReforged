using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using SpiritReforged.Content.Ocean.Items.JellyCandle;
using SpiritReforged.Content.Vanilla.Items.Food;
using Terraria.ModLoader.Utilities;

namespace SpiritReforged.Content.Ocean.NPCs;

[AutoloadCritter]
public class Floater : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 40;
		Main.npcCatchable[Type] = true;
		NPCID.Sets.CountsAsCritter[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.width = 18;
		NPC.height = 22;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.dontCountMe = true;
		NPC.lifeMax = 5;
		NPC.HitSound = SoundID.NPCHit25;
		NPC.DeathSound = SoundID.NPCDeath28;
		NPC.knockBackResist = .35f;
		NPC.aiStyle = 18;
		NPC.noGravity = true;
		NPC.npcSlots = 0;
		AIType = NPCID.PinkJellyfish;
    }

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		bestiaryEntry.UIInfoProvider = new CritterUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type]);
		bestiaryEntry.AddInfo(this, "NightTime Ocean Moon");
	}

	public override void AI() => Lighting.AddLight((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f), .3f, .2f, .3f);

	public override void FindFrame(int frameHeight)
	{
		NPC.frameCounter += 0.15f;
		NPC.frameCounter %= Main.npcFrameCount[Type];
		int frame = (int)NPC.frameCounter;
		NPC.frame.Y = frame * frameHeight;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		Color color = NPC.GetNPCColorTintedByBuffs(NPC.IsABestiaryIconDummy ? Color.White : NPC.GetAlpha(drawColor * 1.25f));

		spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
		
		return false;
	}

	// TODO
	//public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowmaskUtils.DrawNPCGlowMask(spriteBatch, NPC, ModContent.Request<Texture2D>("SpiritMod/NPCs/Critters/Ocean/Floater_Critter_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, screenPos);

	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int k = 0; k < 30; k++)
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.PinkTorch, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, Main.rand.NextFloat(.2f, .8f));
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.AddCommon(ModContent.ItemType<JellyCandle>(), 75);
		npcLoot.AddCommon<RawFish>();
	}
}

//The NPC used to control group spawning logic for floaters and the type actually added to the spawn pool.
//As far as the player is concerned, this doesn't exist.
public class FloaterGroup : ModNPC
{
	public override string Texture => base.Texture.Replace("Group", string.Empty);

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 40;

		var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
	}

	public override void OnSpawn(IEntitySource source) //Spawn a group of floaters
	{
		int childType = ModContent.NPCType<Floater>();
		int amount = Main.rand.Next(6, 8);

		for (int i = 0; i < amount; i++)
		{
			NPC.TargetClosest();

			var position = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(20f);

			var child = NPC.NewNPCDirect(new EntitySource_Parent(NPC), (int)position.X, (int)position.Y, childType, NPC.whoAmI);
			child.velocity = NPC.DirectionTo(Main.player[NPC.target].Center);
			child.netUpdate = true;
		}

		NPC.Transform(childType);
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (spawnInfo.PlayerSafe || Main.dayTime)
			return 0f;

		return SpawnCondition.OceanMonster.Chance * 0.173f;
	}
}
