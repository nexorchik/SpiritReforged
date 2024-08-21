using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;

namespace SpiritReforged.Content.Ocean.NPCs;

[AutoloadCritter]
public class Crinoid : ModNPC
{
	private int pickedType;

	public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 6;

	public override void SetDefaults()
	{
		NPC.dontCountMe = true;
		NPC.width = 22;
		NPC.height = 22;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 5;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = 0f;
		NPC.aiStyle = -1;
		NPC.npcSlots = 0;
		NPC.alpha = 255;
		AIType = NPCID.WebbedStylist;
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		bestiaryEntry.UIInfoProvider = new CritterUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type]);
		bestiaryEntry.AddInfo(this, "Ocean");
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.scale = Main.rand.NextFloat(.6f, 1f);
		pickedType = Main.rand.Next(3);
		NPC.netUpdate = true;
	}

	public override void AI() => NPC.alpha = Math.Max(NPC.alpha - 5, 0); //Fade in

	public override void FindFrame(int frameHeight)
	{
		NPC.frame.Width = 46;
		NPC.frame.X = NPC.frame.Width * pickedType;

		NPC.frameCounter += 0.22f;
		NPC.frameCounter %= Main.npcFrameCount[Type];
		int frame = (int)NPC.frameCounter;
		NPC.frame.Y = frame * frameHeight;

		//if (NPC.IsABestiaryIconDummy && frame == 5)
		//{
		//	pickedType++;

		//	if (pickedType > 2)
		//		pickedType = 0;
		//}
	}

	public override void SendExtraAI(BinaryWriter writer) => writer.Write(pickedType);

	public override void ReceiveExtraAI(BinaryReader reader) => pickedType = reader.ReadInt32();

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);
		Color color = NPC.GetNPCColorTintedByBuffs(NPC.IsABestiaryIconDummy ? Color.White : NPC.GetAlpha(drawColor));
		var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

		spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
		
		return false;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life > 0 || Main.netMode == NetmodeID.Server)
			return;

		string goreType = pickedType switch
		{
			1 => "RedCrinoid",
			2 => "YellowCrinoid",
			_ => "PinkCrinoid"
		};

		for (int i = 0; i < 6; i++) //Spawn a pair of gores 6 times
		{
			for (int t = 1; t < 3; t++)
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>(goreType + t).Type, Main.rand.NextFloat(.5f, 1.2f));
		}
	}
}
